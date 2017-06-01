using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using NetFwTypeLib;
using System.IO;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Text;

namespace BlockSocNetwork
{
    public partial class MainCode : BaseViewModel
    {
        //получение ссылки на менеджер брандмауэра
        private const string CLSID_FIREWALL_MANAGER = "{304CE942-6E39-40D8-943A-B913C40C9CD4}";
        private static INetFwMgr GetFirewallManager()
        {
            Type objectType = Type.GetTypeFromCLSID(
                  new Guid(CLSID_FIREWALL_MANAGER));
            return Activator.CreateInstance(objectType)
                  as INetFwMgr;
        }

        //конструктор
        public MainCode()
        {
            CommandChangeSetting = new RelayCommand(arg => ChangeSetting());
            CommandAddSite = new RelayCommand(arg => AddSite());
            CommandChangeBlockSites = new RelayCommand(arg => ChangeBlockSites());
                 
            //если файла с вебсайтами для блокировки не существует, то создается новый
            if (!File.Exists(pathWebsites))
            {
                defaultWebsites.Write();
            }

            //проверяется, включен ли брандмауэр и, если нет, то включается
            INetFwMgr manager = GetFirewallManager();
            bool isFirewallEnabled = manager.LocalPolicy.CurrentProfile.FirewallEnabled;
            if (isFirewallEnabled == false)
                manager.LocalPolicy.CurrentProfile.FirewallEnabled = true;

            //получаем прослушиваемое устройство
            GetHostIPAddress();
            //старт сниффера
            StartSniffer();
            //проверяем при запуске, были ли уже заблокированы сайты
            CheckBlock();
            //получаем данные для грида
            GetGridData();

            //таймер для проверки заблокированного интервала времени
            timeBlock.Interval = 10000;
            timeBlock.Elapsed += new ElapsedEventHandler(CheckBlockTime);
            //таймер для проверки текущей даты
            timerCheckDate.Interval = 60000;
            timerCheckDate.Elapsed += new ElapsedEventHandler(CheckDate);
            if (!timerCheckDate.Enabled)
                timerCheckDate.Start();

           // Properties.Settings.Default.password = "";
        }

        //---------------------------КОМАНДЫ-----------------------------
        public ICommand CommandChangeSetting { get; set; }
        public ICommand CommandAddSite { get; set; }
        public ICommand CommandChangeBlockSites { get; set; }

        #region Fields
        //---------------------------ПОЛЯ и ОБЪЕКТЫ-----------------------------
        private Socket socket;                                                                              //прослушивающее устройство
        private byte[] buffer;                                                                              //полученные пакеты
        private string hostIPAddress;                                                                       // IP прослушиваемого устройства
                                   
        private string _status = "Социальные сети разблокированы";
        private string _maxTime = Properties.Settings.Default.maxTime;
        private string _maxDayTime = Properties.Settings.Default.maxDayTime;
        private string _blockTime = Properties.Settings.Default.blockTime;
        private string _newDomen;
        private string _newIpDns;
        private string _newIpRange;
        private int _quantity = 0;
        ObservableCollection<WebsitesModel> _gridWebsites;

        public static bool isCheckedTime = Properties.Settings.Default.isCheckedTime;                       //checkbox блокировка по интервалу
        public static bool isCheckedDayTime = Properties.Settings.Default.isCheckedDayTime;                 //checkbox блокировка на сутки

        DefaultWebsites defaultWebsites = new DefaultWebsites();                                            
        Block block = new Block();
        CheckRange checkRange = new CheckRange();
        AddWebsite addSite = new AddWebsite();
        ListBlockWebsites listBlockWebsites = new ListBlockWebsites();
        Statistics statistics = new Statistics();
        public List<WebsiteStatisticsModel> statisticsWebsites = new List<WebsiteStatisticsModel>();
        
        const string pathWebsites = "websites.json";                                                        //файл с вебсайтами для блокировки

        private TimeSpan currentTime;                                                                       //время текущего ip
        private TimeSpan startTime = TimeSpan.Parse("00:00:00");                                            //время начала использования соц сетей
        private TimeSpan prevTime;                                                                          //время предыдущего ip
        private TimeSpan difCurrPrevTime;                                                                   //время м/у текущим и предыдущим ip
        private TimeSpan difCurrStartTime;                                                                  //время м/у текущим и первым ip
        private TimeSpan maxDifCurrPrevTime = TimeSpan.Parse("00:05:00");                                   //максимальное время м/у текущим и предыдущим ip 
        private TimeSpan maxDifCurrStartTime = TimeSpan.Parse(Properties.Settings.Default.maxTime);         //максимальный допустимый интервал времени использования соц сетей
        private TimeSpan blockTime = TimeSpan.Parse(Properties.Settings.Default.blockTime);                 //время блокировки соц сетей
        private TimeSpan maxTotalDayTime = TimeSpan.Parse(Properties.Settings.Default.maxDayTime);          //максимальное время использования соц сетей в сутки
        private string[] strStopBlockTime;                                                                  //время разблокировки соц сетей
        private string currDate = DateTime.Now.ToString().Split(' ')[0];                                    //текущая дата
        
        Timer timeBlock = new Timer();
        Timer timerCheckDate = new Timer();
        #endregion

        #region Properties
        //--------------------------СВОЙСТВА--------------------------
        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

        public string MaxTime
        {
            get { return _maxTime; }
            set
            {
                if (_maxTime != value)
                {
                    _maxTime = value;
                    OnPropertyChanged("MaxTime");
                }
            }
        }

        public string MaxDayTime
        {
            get { return _maxDayTime; }
            set
            {
                if (_maxDayTime != value)
                {
                    _maxDayTime = value;
                    OnPropertyChanged("MaxDayTime");
                }
            }
        }

        public string BlockTime
        {
            get { return _blockTime; }
            set
            {
                if (_blockTime != value)
                {
                    _blockTime = value;
                    OnPropertyChanged("BlockTime");
                }
            }
        }

        public string NewDomen
        {
            get { return _newDomen; }
            set
            {
                if (_newDomen != value)
                {
                    _newDomen = value;
                    OnPropertyChanged("NewDomen");
                }
            }
        }

        public string NewIpDns
        {
            get { return _newIpDns; }
            set
            {
                if (_newIpDns != value)
                {
                    _newIpDns = value;
                    OnPropertyChanged("NewIpDns");
                }
            }
        }

        public string NewIpRange
        {
            get { return _newIpRange; }
            set
            {
                if (_newIpRange != value)
                {
                    _newIpRange = value;
                    OnPropertyChanged("NewIpRange");
                }
            }
        }

        public int Quantity
        {
            get { return _quantity; }
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged("Quantity");
                }
            }
        }

        public ObservableCollection<WebsitesModel> GridWebsites
        {
            get { return _gridWebsites; }
            set
            {
                if (_gridWebsites != value)
                {
                    _gridWebsites = value;
                    OnPropertyChanged("GridWebsites");
                }
            }
        }
        #endregion

        //---------------------------МЕТОДЫ---------------------------
        //получаем прослушиваемое устройство
        private void GetHostIPAddress()
        {
            IPHostEntry HostEntry = Dns.GetHostEntry((Dns.GetHostName()));
            if (HostEntry.AddressList.Length > 0)
            {
                IList<HostIPAddress> listHostIP = new List<HostIPAddress>();
                foreach (IPAddress ip in HostEntry.AddressList)
                {
                    listHostIP.Add(new HostIPAddress(ip.ToString()));
                }
                hostIPAddress = listHostIP[0].ToString(); ;
            }
        }

        //сниффер
        private void StartSniffer()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
            socket.Bind(new IPEndPoint(IPAddress.Parse(hostIPAddress), 0));
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.HeaderIncluded, true);

            byte[] byInc = new byte[] { 1, 0, 0, 0 };
            byte[] byOut = new byte[4];
            buffer = new byte[4096];
            socket.IOControl(IOControlCode.ReceiveAll, byInc, byOut);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, null);
        }

        //начало прослушивания хоста
        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                int nReceived = socket.EndReceive(ar);
                CheckIP(buffer, nReceived);
                buffer = new byte[4096];
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, null);
            }
            catch { }
        }
     
        //проверка IP и блокировка сайтов
        private void CheckIP (byte[] buf, int len)

        {
            IPHeader ipHeader = new IPHeader(buf, len);   
            if (!Properties.Settings.Default.isBlocked)
            {
                if (checkRange.IsInRange(ipHeader.DestinationAddress) && (ipHeader.DestinationAddress.ToString() != "87.240.165.82"))
                {
                    currentTime = TimeSpan.Parse(DateTime.Now.ToString().Split(' ')[1]);
                    if (startTime == TimeSpan.Parse("00:00:00"))
                    {
                        startTime = currentTime;
                        prevTime = currentTime;
                    }
                    else
                    {
                        difCurrPrevTime = currentTime - prevTime;

                        //для статистики
                        WebsiteStatisticsModel statisticsWebsite = new WebsiteStatisticsModel();
                        statisticsWebsite.Name = checkRange.GetWebsiteName();
                        statisticsWebsite.Date = DateTime.Now.ToString().Split(' ')[0];
                        statisticsWebsite.Time = difCurrPrevTime;
                        bool isInList = false;
                        int countList = statisticsWebsites.Count;
                        if (statisticsWebsites.Count != 0)
                        {
                            for (int i = 0; i < countList; i++)
                            {
                                //если в списке уже есть этот сайт, добавляем время
                                if (statisticsWebsites[i].Name == statisticsWebsite.Name)
                                {
                                    statisticsWebsites[i].Time += difCurrPrevTime;
                                    isInList = true;
                                }                               
                            }
                            if (!isInList)
                            {
                                statisticsWebsites.Add(statisticsWebsite);
                            }
                        }
                        else
                        {
                            statisticsWebsites.Add(statisticsWebsite);
                        }


                        //блокировка на сутки
                        if (isCheckedDayTime)
                        {
                            Properties.Settings.Default.totalDayTime += difCurrPrevTime;
                            if (Properties.Settings.Default.totalDayTime >= maxTotalDayTime)
                            {
                                Status = "Социальные сети заблокированы до конца суток";
                                Properties.Settings.Default.isBlocked = true;
                                Properties.Settings.Default.isDayBlocked = true;
                                Properties.Settings.Default.Save();
                                block.SetBlock();                               
                                return;
                            }
                        }
                        //блокировка по интервалам
                        if (difCurrPrevTime < maxDifCurrPrevTime && isCheckedTime)
                        {
                            difCurrStartTime = currentTime - startTime;
                            if (difCurrStartTime > maxDifCurrStartTime)
                            {
                                Status = "Социальные сети заблокированы";

                                //если время для блокировки наступит на следующие сутки
                                strStopBlockTime = (currentTime + blockTime).ToString().Split('.');
                                if (strStopBlockTime.Length == 1)
                                    Properties.Settings.Default.stopBlockTime = TimeSpan.Parse(strStopBlockTime[0]);
                                else
                                    Properties.Settings.Default.stopBlockTime = TimeSpan.Parse(strStopBlockTime[1]);

                                Properties.Settings.Default.isBlocked = true;
                                block.SetBlock();
                                if (!timeBlock.Enabled)
                                    timeBlock.Start();
                            }
                            else
                            {
                                prevTime = currentTime;
                            }
                        }
                        else
                        {
                            startTime = TimeSpan.Parse("00:00:00");
                        }
                    }
                    ++Quantity;
                    Properties.Settings.Default.Save();
                }               
            }        
        }

        //проверка заблокированного времени и разблокировка сайтов
        private void CheckBlockTime (object sender = null, EventArgs e = null)
        {
            currentTime = TimeSpan.Parse(DateTime.Now.ToString().Split(' ')[1]);
            if (currentTime > Properties.Settings.Default.stopBlockTime)
            {
                startTime = TimeSpan.Parse("00:00:00");
                Properties.Settings.Default.isBlocked = false;
                Properties.Settings.Default.Save();
                block.DeleteBlock();
                Status = "Социальные сети разблокированы";
                timeBlock.Stop();
            }
            else
            {
                //если при новом запуске программы сайты должны быть все еще заблокированы и таймер не запущен 
                if (!timeBlock.Enabled)
                {
                    timeBlock.Start();
                }
            }
        }

        //проверка текущей даты и разблокировка сайтов
        private void CheckDate (object sender = null, EventArgs e = null)
        {
            currDate = DateTime.Now.ToString().Split(' ')[0];
            if (Properties.Settings.Default.date != currDate)
            {
                Properties.Settings.Default.date = currDate;
                Properties.Settings.Default.totalDayTime = TimeSpan.Parse("00:00:00");
                if  (Properties.Settings.Default.isDayBlocked)
                {
                    block.DeleteBlock();
                    startTime = TimeSpan.Parse("00:00:00");
                    Properties.Settings.Default.isBlocked = false;
                    Properties.Settings.Default.isDayBlocked = false;                   
                    Status = "Социальные сети разблокированы";
                    statistics.Write(statisticsWebsites);
                    statisticsWebsites.Clear();
                }
                Properties.Settings.Default.Save();
            }
        }

        //измененние настроек пользователем
        private void ChangeSetting()
        {
            try
            {
                maxDifCurrStartTime = TimeSpan.Parse(MaxTime);
                maxTotalDayTime = TimeSpan.Parse(MaxDayTime);
                blockTime = TimeSpan.Parse(BlockTime);

                Properties.Settings.Default.blockTime = BlockTime;
                Properties.Settings.Default.maxTime = MaxTime;
                Properties.Settings.Default.maxDayTime = MaxDayTime;
                Properties.Settings.Default.Save();

                MessageBox.Show("Настройки изменены!");
            }
            catch
            {
                MessageBox.Show("Произошла ошибка! Проверьте ввод данных!");
            }
        }

        //проверка на блокировку соц сетей при новом запуске программы
        private void CheckBlock()
        {
            if (Properties.Settings.Default.isDayBlocked)
            {
                Status = "Социальные сети заблокированы до конца суток";
                CheckDate();
            }
            else
            {
                if (Properties.Settings.Default.isBlocked)
                {
                    Status = "Социальные сети заблокированы";
                    CheckBlockTime();
                }
            }                    
        }

        //добавить сайт
        private void AddSite()
        {
            if (addSite.CheckData(NewDomen, NewIpDns, NewIpRange))
            {
                if (addSite.Add(NewDomen, NewIpDns, NewIpRange))
                {
                    MessageBox.Show("Сайт добавлен!");
                    block.DeleteRule();
                    GetGridData();                  
                }
                else
                {
                    MessageBox.Show("Произошла ошибка! Сайт не был добавлен!");
                }
            }
            else
            {
                MessageBox.Show("Проверьте ввод данных!");
                return;
            }
        }

        //получить данные для грида 
        private void GetGridData ()
        {
            GridWebsites = new ObservableCollection<WebsitesModel>(listBlockWebsites.GetList());
        }

        //изменить список блокируемых сайтов
        private void ChangeBlockSites ()
        {
            List<WebsitesModel> newList = new List<WebsitesModel>(GridWebsites);
            bool result = listBlockWebsites.ChangeBlockWebsites(newList);
            if (result)
            {
                MessageBox.Show("Все супер!");
            }
            else
            {
                MessageBox.Show("Печалька");
            }            
        }

    }

}
