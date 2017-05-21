using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace BlockSocNetwork
{
    public class CheckRange
    {
        const string path = "websites.json";
        BlockWebsites sites = new BlockWebsites();

        public bool IsInRange(IPAddress _ip)
        {
            sites = GetWebsites();
            foreach (var site in sites.Websites)
            {
                for (int i = 0; i < site.IPAddressStart.Length; i++)
                {
                    IPAddress ipAddressStart = IPAddress.Parse(site.IPAddressStart[i]);
                    IPAddress ipAddressEnd = IPAddress.Parse(site.IPAddressEnd[i]);
                    byte[] ipStart = ipAddressStart.GetAddressBytes();
                    byte[] ipEnd = ipAddressEnd.GetAddressBytes();
                    byte[] ip = _ip.GetAddressBytes();

                    if (
                        //проверка на нижний предел диапазона
                        ip[0] == ipStart[0] &&
                        ip[1] == ipStart[1] &&
                        ip[2] >= ipStart[2] &&
                        //проверка на верхний предел диапазона
                        ip[0] == ipEnd[0] &&
                        ip[1] == ipEnd[1] &&
                        ip[2] <= ipEnd[2]
                        )
                        return true;
                }
            }          
           return false;
        }

        //получаем все файлы из сайта
        private BlockWebsites GetWebsites()
        {
            string serializedText = "";

            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                serializedText = sr.ReadToEnd();
            }
            BlockWebsites newCollection = JsonConvert.DeserializeObject<BlockWebsites>(serializedText);

            return newCollection;
        }
    }
}
