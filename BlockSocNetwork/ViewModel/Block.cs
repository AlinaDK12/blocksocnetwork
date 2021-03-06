﻿using System;
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
        INetFwRule currentFirewallRule;
        INetFwRule2 firewallRule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));      
        INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));

        public void SetBlock()
        {
            //если правило существует, то оно включается
            try
            {
                currentFirewallRule = firewallPolicy.Rules.Item(nameRule);
                if (!currentFirewallRule.Enabled)
                {
                    currentFirewallRule.Enabled = true;
                }
            }
            //если правило не существует, то создается и добавляется новое
            catch
            {
                firewallRule.Name = nameRule;
                firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
                firewallRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
                firewallRule.Enabled = true;
                firewallRule.InterfaceTypes = "All";
                firewallRule.RemoteAddresses = GetAllRemoteAddresses();

                firewallPolicy.Rules.Add(firewallRule);             
            }                            
        }

        //для разблокировки выключается соответствующее правило в брандмауэре
        public void DeleteBlock()
        {
            try
            {
                currentFirewallRule = firewallPolicy.Rules.Item(nameRule);
                if (currentFirewallRule.Enabled)
                {
                    currentFirewallRule.Enabled = false;
                }
            }
            catch { }
        }

        //при изменении списка блокируемых сайтов правило удаляется из брандмауэра, чтобы потом добавить новое с новыми сайтами
        public void DeleteRule()
        {
            try
            {
                firewallPolicy.Rules.Remove(nameRule);
            }
            catch { }
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
            BlockWebsitesModel newCollection = JsonConvert.DeserializeObject<BlockWebsitesModel>(serializedText);

            foreach (var item in newCollection.Websites)
            {
                if (item.IsSelected == true)
                {
                    foreach (var ip in item.IP)
                    {
                        str += ip + ",";
                    }
                }
            }
            //удаление последней ,
            str.Remove(str.Length - 1, 1);

            return str;
        }
    }
}
