using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace BlockSocNetwork
{
    public class ListBlockWebsites
    {
        const string pathWebsites = "websites.json";
        List<WebsitesModel> listWebsites;
        Block block = new Block();

        //список для вывода в грид
        public List<WebsitesModel> GetList ()
        {
            string serializedText = "";

            using (StreamReader sr = new StreamReader(pathWebsites, Encoding.Default))
            {
                serializedText = sr.ReadToEnd();
            }
            BlockWebsitesModel collection = JsonConvert.DeserializeObject<BlockWebsitesModel>(serializedText);
            listWebsites = new List<WebsitesModel>();

            foreach (var item in collection.Websites)
            {
                WebsitesModel site = new WebsitesModel()
                {
                    Name = item.Name,
                    RangeIP = item.IPAddressStart[0] + " - " + item.IPAddressEnd[0],
                    IsSelected = item.IsSelected,
                };
                listWebsites.Add(site);
            }
            return listWebsites;
        }

        public bool ChangeBlockWebsites (List<WebsitesModel> newList)
        {
            try
            {
                string serializedText = "";

                using (StreamReader sr = new StreamReader(pathWebsites, Encoding.Default))
                {
                    serializedText = sr.ReadToEnd();
                }
                BlockWebsitesModel collection = JsonConvert.DeserializeObject<BlockWebsitesModel>(serializedText);

                for (int i = 0; i < collection.Websites.Length; i++)
                {
                    collection.Websites[i].IsSelected = newList[i].IsSelected;
                }

                //в json
                string serialized = JsonConvert.SerializeObject(collection);

                //запись в файл
                StreamWriter sw = new StreamWriter(pathWebsites);
                sw.Write(serialized);
                sw.Close();
                block.DeleteRule();
                return true;
            }
            catch
            {
                return false;
            }
            
        }
    }
}
