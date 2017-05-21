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

        //реализация команд
        public ICommand CommandSelectionHostIPAddress { get; set; }

        //конструктор
        public MainCode()
        {
                 
            //если файла с вебсайтами для блокировки не существует, то создается новый
            if (!File.Exists(pathWebsites))
            {
                DefaultWebsites.Write();
            }

            //получаем прослушиваемое устройство
            GetHostIPAddress();
            //старт сниффера
            StartSniffer();


            //таймер
            timeBlock.Interval = 10000;
            timeBlock.Enabled = true;
            timeBlock.Elapsed += new ElapsedEventHandler(CheckBlockTime);
        }

        //---------------------------ПОЛЯ-----------------------------
        private Socket socket;                                     //прослушивающее устройство
        private byte[] buffer;                                     //полученные пакеты
        private string hostIPAddress;                              // IP прослушиваемого устройства
        private bool isBlocked = false;                            
        private string _status = "Социальные сети разблокированы!";

        DefaultWebsites DefaultWebsites = new DefaultWebsites();   //запись ip вебсайтов для блокировки при первом запуске
        Block block = new Block();
        CheckRange checkRange = new CheckRange();
        
        const string pathWebsites = "websites.json";               //файл с вебсайтами для блокировки


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

        private TimeSpan currentTime;                       
        private TimeSpan startTime = TimeSpan.Parse("00:00:00");
        private TimeSpan prevTime;
        private TimeSpan difCurrPrevTime;
        private TimeSpan difCurrStartTime;
        private TimeSpan maxDifCurrPrevTime = TimeSpan.Parse("00:01:00");
        private TimeSpan maxDifCurrStartTime = TimeSpan.Parse("00:03:00");
        private TimeSpan blockTime = TimeSpan.Parse("00:03:00");
        private TimeSpan stopBlockTime;
        private string[] strStopBlockTime;

        private string date = DateTime.Now.ToString().Split(' ')[0];

        Timer timeBlock = new Timer();


        //проверка IP и блокировка сайтов
        private void CheckIP (byte[] buf, int len)
        {
            IPHeader ipHeader = new IPHeader(buf, len);

            //IPHostEntry sourceDns = Dns.GetHostEntry(ipHeader.SourceAddress);
            //IPHostEntry destinationDns = Dns.GetHostEntry(ipHeader.DestinationAddress);      

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
                        if (difCurrPrevTime < maxDifCurrPrevTime)
                        {
                            difCurrStartTime = currentTime - startTime;
                            if (difCurrStartTime > maxDifCurrStartTime)
                            {
                                Status = "Социальные сети заблокированы!";

                                //если время для блокировки наступит на следующие сутки
                                strStopBlockTime = (currentTime + blockTime).ToString().Split('.');
                                if (strStopBlockTime.Length == 1)
                                    stopBlockTime = TimeSpan.Parse(strStopBlockTime[0]);
                                else
                                    stopBlockTime = TimeSpan.Parse(strStopBlockTime[1]);  
                                               
                                isBlocked = true;
                                //block.SetBlock();
                                timeBlock.Start();
                                MessageBox.Show(currentTime.ToString() + "   " + stopBlockTime.ToString());
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
        private void CheckBlockTime(object sender, EventArgs e)
        {
            currentTime = TimeSpan.Parse(DateTime.Now.ToString().Split(' ')[1]);
            if (currentTime > stopBlockTime)
            {
                //MessageBox.Show("Пора разблокировать   " + currentTime.ToString() + "   " + stopBlockTime.ToString());
                startTime = TimeSpan.Parse("00:00:00");
                isBlocked = false;
                //block.DeleteBlock();
                timeBlock.Stop();
            }
            
        }


    }
}
