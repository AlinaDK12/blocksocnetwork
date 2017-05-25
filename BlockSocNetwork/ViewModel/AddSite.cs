using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace BlockSocNetwork
{
    public class AddSite
    {
        const string path = "websites.json";

        public bool CheckData (string _domen, string _ipDns, string _ipRange)
        {
            //проверка введенного домена
            string[] domen = _domen.Trim().Split('.');
            if (domen.Length < 2)
            {
                return false;
            }

            //проверка введенного ip
            string[] ipDns = _ipDns.Trim().Split(',');
            string[] ipBlockArray;
            int ipBlock;
            for (int i = 0; i < ipDns.Length; i++)
            {
                ipBlockArray = ipDns[i].Split('.');
                if (ipBlockArray.Length == 4)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        try
                        {
                            ipBlock = Convert.ToInt32(ipBlockArray[j]);
                        }
                        catch
                        {
                            return false;
                        }
                        if ((ipBlock < 0) || (ipBlock > 255))
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }

            //проверка диапазона ip
            string[] ipRange = _ipRange.Trim().Split('-');
            string[] ipBlockRangeArray;
            int ipBlockRange;
            if (ipRange.Length == 2)
            {
                for (int i = 0; i < 2; i++)
                {
                    ipBlockRangeArray = ipRange[i].Split('.');
                    if (ipBlockRangeArray.Length == 4)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            try
                            {
                                ipBlockRange = Convert.ToInt32(ipBlockRangeArray[j]);
                            }
                            catch
                            {
                                return false;
                            }
                            if ((ipBlockRange < 0) || (ipBlockRange > 255))
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        public bool Add (string _domen, string _ipDns, string _ipRange)
        {
            try
            {
                //считываем текущие сайты
                string serializedText = "";

                using (StreamReader sr = new StreamReader(path, Encoding.Default))
                {
                    serializedText = sr.ReadToEnd();
                }
                BlockWebsites collection = JsonConvert.DeserializeObject<BlockWebsites>(serializedText);

                //создаем объект с данными нового сайта
                string[] ipDns = _ipDns.Trim().Split(',');
                int length = ipDns.Length;
                string[] ipRange = _ipRange.Trim().Split('-');
                int number = collection.Websites.Length;
                List<string> listIP = new List<string>();
                for (int i = 0; i < ipDns.Length; i++)
                {
                    listIP.Add(ipDns[i]);
                }

                BlockWebsites newCollection = new BlockWebsites();
                newCollection.Websites = new Website[number + 1];
                for (int i = 0; i < collection.Websites.Length; i++)
                {
                    newCollection.Websites[i] = collection.Websites[i];
                }

                newCollection.Websites[number] = new Website()
                {
                    Name = _domen,
                    IP = listIP.ToArray<string>(),
                    IPAddressStart = new string[1] { ipRange[0] },
                    IPAddressEnd = new string[1] { ipRange[1] },
                    IsSelected = true,

                };

                //в json
                string serialized = JsonConvert.SerializeObject(newCollection);

                //записываем в файл обратно
                StreamWriter sw = new StreamWriter(path);
                sw.Write(serialized);
                sw.Close();

                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
