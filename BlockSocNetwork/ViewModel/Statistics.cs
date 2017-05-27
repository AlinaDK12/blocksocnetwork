using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace BlockSocNetwork
{
    public class Statistics
    {
        const string pathWebsites = "websites.json";
        const string pathStatistics = "statistics.json";

        public void Write (List<WebsiteStatisticsModel> statisticsWebsites)
        {
            if (File.Exists(pathStatistics))
            {
                string serializedText = "";

                using (StreamReader sr = new StreamReader(pathStatistics, Encoding.Default))
                {
                    serializedText = sr.ReadToEnd();
                }
                List<WebsiteStatisticsModel> collection = JsonConvert.DeserializeObject<List<WebsiteStatisticsModel>>(serializedText);

                foreach (var col in collection)
                {
                    foreach (var site in statisticsWebsites)
                    {
                        if (col.Name == site.Name && col.Date == site.Date)
                        {
                            col.Time += site.Time;
                        }
                        else
                        {
                            collection.Add(site);
                        }
                    }
                }

                //в json
                string serialized = JsonConvert.SerializeObject(collection);

                //запись в файл
                StreamWriter sw = new StreamWriter(pathStatistics);
                sw.Write(serialized);
                sw.Close();
            }
            else
            {
                //в json
                string serialized = JsonConvert.SerializeObject(statisticsWebsites);

                //запись в файл
                StreamWriter sw = new StreamWriter(pathStatistics);
                sw.Write(serialized);
                sw.Close();
            }
        }
    }
}
