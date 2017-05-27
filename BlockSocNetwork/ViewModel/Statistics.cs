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

                int countOldList = collection.Count;
                int countNewList = statisticsWebsites.Count;
                bool isInList = false;

                for (int i = 0; i < countNewList; i++)
                {
                    isInList = false;
                    for (int j = 0; j < countOldList; j++)
                    {
                        if (collection[j].Name == statisticsWebsites[i].Name && collection[j].Date == statisticsWebsites[i].Date)
                        {
                            collection[j].Time += statisticsWebsites[i].Time;
                            isInList = true;
                        }
                    }
                    if (!isInList)
                    {
                        collection.Add(statisticsWebsites[i]);
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

        public List<WebsiteStatisticsModel> Get()
        {
            if (File.Exists(pathStatistics))
            {
                string serializedText = "";

                using (StreamReader sr = new StreamReader(pathStatistics, Encoding.Default))
                {
                    serializedText = sr.ReadToEnd();
                }
                List<WebsiteStatisticsModel> collection = JsonConvert.DeserializeObject<List<WebsiteStatisticsModel>>(serializedText);
                return collection;
            }
            else return null;
        }
    }
}
