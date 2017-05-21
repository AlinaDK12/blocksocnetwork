using System;
using System.Text;
using NetFwTypeLib;
using System.IO;
using Newtonsoft.Json;

namespace BlockSocNetwork
{
    public class Block
    {
        const string nameRule = "Block SocNetwork";
        const string path = "websites.json";        

        public void SetBlock()
        {
            INetFwRule2 firewallRule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));

            firewallRule.Name = nameRule;
            firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
            firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
            firewallRule.Enabled = true;
            firewallRule.InterfaceTypes = "All";
            firewallRule.RemoteAddresses = GetAllRemoteAddresses();           

            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.Rules.Add(firewallRule);
            
        }

        public void DeleteBlock()
        {
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.Rules.Remove(nameRule);
        }

        //получаем все IP для блокировки
        private string GetAllRemoteAddresses()
        {
            string str = "";
            string serializedText = "";

            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                serializedText = sr.ReadToEnd();
            }
            BlockWebsites newCollection = JsonConvert.DeserializeObject<BlockWebsites>(serializedText);

            foreach (var item in newCollection.Websites)
            {
                foreach (var ip in item.IP)
                {
                    str += ip + ",";
                }
            }
            //удаление последней ,
            str.Remove(str.Length - 1, 1);

            return str;
        }
    }
}
