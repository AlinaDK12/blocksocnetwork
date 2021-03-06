﻿using Newtonsoft.Json;
using System.IO;

namespace BlockSocNetwork
{
    public class DefaultWebsites
    {
        const string path = "websites.json";

        public void Write()
        {
            //вебсайты для блокировки по умолчанию
            BlockWebsitesModel collection = new BlockWebsitesModel();
            collection.Websites = new Website[4];
            collection.Websites[0] = new Website()
            {
                Name = "vk.com",
                IP = new string[2] { "87.240.165.82", "95.213.11.180" },
                IPAddressStart = new string[2] { "95.213.4.0", "87.240.160.0" },
                IPAddressEnd = new string[2] { "95.213.7.255", "87.240.191.255" },
                IsSelected = true,
            };
            collection.Websites[1] = new Website()
            {
                Name = "facebook.com",
                IP = new string[1] { "31.13.72.36" },
                IPAddressStart = new string[2] { "31.13.93.0", "31.13.72.0" },
                IPAddressEnd = new string[2] { "31.13.93.255", "31.13.72.255" },
                IsSelected = true,
            };
            collection.Websites[2] = new Website()
            {
                Name = "ok.ru",
                IP = new string[3] { "5.61.23.5", "217.20.156.159", "217.20.155.58" },
                IPAddressStart = new string[2] { "5.61.23.0", "217.20.152.0" },
                IPAddressEnd = new string[2] { "5.61.23.255", "217.20.159.255" },    
                IsSelected = true,          
            };
            collection.Websites[3] = new Website()
            {
                Name = "mail.ru",
                IP = new string[4] { "217.69.139.199", "217.69.139.202", "94.100.180.199", "94.100.180.202" },
                IPAddressStart = new string[2] { "94.100.176.0", "217.69.136.0" },
                IPAddressEnd = new string[2] { "94.100.183.255", "217.69.141.255" },
                IsSelected = true,
            };

            //в json
            string serialized = JsonConvert.SerializeObject(collection);

            //запись в файл
            StreamWriter sw = new StreamWriter(path);
            sw.Write(serialized);
            sw.Close();
        }
    }
}
