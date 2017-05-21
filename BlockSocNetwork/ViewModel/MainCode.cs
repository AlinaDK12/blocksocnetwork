using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using System.Net;
using System.Net.Sockets;
using System.Windows.Data;
using System.Timers;
using NetFwTypeLib;
using Newtonsoft.Json;
using System.IO;

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
                 
            //если файла с вебсайтами для блокировки не существует, то создается новый
            if (!File.Exists(pathWebsites))
            {
                DefaultWebsites.Write();
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

            //таймер
            timeBlock.Interval = 10000;
            timeBlock.Enabled = true;
            timeBlock.Elapsed += new ElapsedEventHandler(CheckBlockTime);

            timerCheckDate.Interval = 600000;
            timerCheckDate.Enabled = true;
            timerCheckDate.Elapsed += new ElapsedEventHandler(CheckDate);
            timerCheckDate.Start();
        }

        //---------------------------КОМАНДЫ-----------------------------
        public ICommand CommandChangeSetting { get; set; }

        //---------------------------ПОЛЯ-----------------------------
        private Socket socket;                                     //прослушивающее устройство
        private byte[] buffer;                                     //полученные пакеты
        private string hostIPAddress;                              // IP прослушиваемого устройства
                                   
        private string _status = "Социальные сети разблокированы!";
        private string _maxTime = "00:03:00";
        private string _maxDayTime = "00:05:00";
        private string _blockTime = "00:03:00";

        private bool isBlocked = false;
        private bool isDayBlocked = false;
        public static bool isCheckedTime = true;
        public static bool isCheckedDayTime = true;

        DefaultWebsites DefaultWebsites = new DefaultWebsites();   //запись ip вебсайтов для блокировки при первом запуске
        Block block = new Block();
        CheckRange checkRange = new CheckRange();
        
        const string pathWebsites = "websites.json";               //файл с вебсайтами для блокировки

        private TimeSpan currentTime;
        private TimeSpan startTime = TimeSpan.Parse("00:00:00");
        private TimeSpan prevTime;
        private TimeSpan difCurrPrevTime;
        private TimeSpan difCurrStartTime;
        private TimeSpan maxDifCurrPrevTime = TimeSpan.Parse("00:01:00");
        private TimeSpan maxDifCurrStartTime = TimeSpan.Parse("00:03:00");
        private TimeSpan blockTime = TimeSpan.Parse("00:03:00");
        private TimeSpan stopBlockTime;
        private TimeSpan totalDayTime;
        private TimeSpan maxTotalDayTime = TimeSpan.Parse("00:05:00");
        private string[] strStopBlockTime;
        private string date = DateTime.Now.ToString().Split(' ')[0];
        private string currDate = DateTime.Now.ToString().Split(' ')[0];
        
        Timer timeBlock = new Timer();
        Timer timerCheckDate = new Timer();


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

        //здесь начинается прослушивание хоста
        private void OnReceive(IAsyncResult ar)
        {
           
            try
            {
                int nReceived = socket.EndReceive(ar);
                CheckIP(buffer, nReceived);
                buffer = new byte[4096];
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None,
                    OnReceive, null);
            }
            catch
            {
                MessageBox.Show("Error!");
            }
        }
     
        //проверка IP и блокировка сайтов
        private void CheckIP (byte[] buf, int len)
        {
            IPHeader ipHeader = new IPHeader(buf, len);   

            if (!isBlocked)
            {
                if (checkRange.IsInRange(ipHeader.DestinationAddress) && (ipHeader.DestinationAddress.ToString() != "87.240.165.82"))
                {
                    currentTime = TimeSpan.Parse(DateTime.Now.ToString().Split(' ')[1]);
                    if (startTime == TimeSpan.Parse("00:00:00"))
                    {
                        startTime = currentTime;
                        prevTime = currentTime;
                        stopBlockTime = currentTime + blockTime;
                    }
                    else
                    {
                        difCurrPrevTime = currentTime - prevTime;
                        if (isCheckedDayTime)
                        {
                            totalDayTime += difCurrPrevTime;
                            if (totalDayTime >= maxTotalDayTime)
                            {
                                Status = "Социальные сети заблокированы до конца суток!";
                                isBlocked = true;
                                isDayBlocked = true;
                                block.SetBlock();                               
                                return;
                            }
                        }
                        if (difCurrPrevTime < maxDifCurrPrevTime && isCheckedTime)
                        {
                            difCurrStartTime = currentTime - startTime;
                            if (difCurrStartTime > maxDifCurrStartTime)
                            {
                                Status = "Социальные сети заблокированы!";
                                MessageBox.Show("Зашел");
                                //если время для блокировки наступит на следующие сутки
                                strStopBlockTime = (currentTime + blockTime).ToString().Split('.');
                                if (strStopBlockTime.Length == 1)
                                    stopBlockTime = TimeSpan.Parse(strStopBlockTime[0]);
                                else
                                    stopBlockTime = TimeSpan.Parse(strStopBlockTime[1]);

                                isBlocked = true;
                                block.SetBlock();
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
                }
            }
        }

        //проверка заблокированного времени и разблокировка сайтов
        private void CheckBlockTime (object sender, EventArgs e)
        {
            currentTime = TimeSpan.Parse(DateTime.Now.ToString().Split(' ')[1]);
            if (currentTime > stopBlockTime)
            {
                startTime = TimeSpan.Parse("00:00:00");
                isBlocked = false;
                block.DeleteBlock();
                Status = "Социальные сети разблокированы!";
                timeBlock.Stop();
            }
            
        }

        //проверка текущей даты
        private void CheckDate (object sender, EventArgs e)
        {
            currDate = DateTime.Now.ToString().Split(' ')[0];
            if (date != currDate)
            {
                date = currDate;
                totalDayTime = TimeSpan.Parse("00:00:00");
                if  (isDayBlocked)
                {
                    block.DeleteBlock();
                    isBlocked = false;
                    isDayBlocked = false;
                    Status = "Социальные сети разблокированы!";
                }
            }
        }

        private void ChangeSetting()
        {
            try
            {
                maxDifCurrStartTime = TimeSpan.Parse(MaxTime);
                maxTotalDayTime = TimeSpan.Parse(MaxDayTime);
                blockTime = TimeSpan.Parse(BlockTime);
                MessageBox.Show("Настройки изменены!");
            }
            catch
            {
                MessageBox.Show("Произошла ошибка! Проверьте ввод данных!");
            }
        }
    }
}
