using System.Linq;
using System.Text;
using System.IO;
using System.Windows;


namespace BlockSocNetwork
{
    public class StreamHosts
    {
        const string path = @"C:\Windows\System32\drivers\etc\hosts";
        bool isBlocked = false;

        public void SetBlock()
        {        
            if (!isBlocked)
            {
                StreamWriter sw = new StreamWriter(path, true);
                string borderBlock = "\n #---------- SocBlock ----------#";
                string sitetoblock = "\r\n 127.0.0.1 vk.com \r\n 127.0.0.1 www.vk.com";

                sw.Write(borderBlock);
                sw.Write(sitetoblock);
                sw.Write(borderBlock);
                sw.Close();
                isBlocked = true;

                //очистка кэша
               // Process.Start(@"C:\Windows\System32\rundll32.exe", "InetCpl.cpl,ClearMyTracksByProcess 255");

                MessageBox.Show("Социальные сети заблокированы!");
            }              
        }

        public void DeleteBlock()
        {
            string text = "";
            string borderBlock = "#---------- SocBlock ----------#";
            using (StreamReader sr = new StreamReader(path, Encoding.Default))
            {
                text = sr.ReadToEnd();
            }
            var lineText = text.Split('\n');
            string currentLine;
            string newText = "";
            int i = 0;
            bool isSocBlock = false;
            while (i < lineText.Count())
            {
                currentLine = lineText[i].Trim(' ');
                if ((currentLine == borderBlock))
                {
                    isSocBlock = isSocBlock ? false : true;
                    lineText[i] = "";
                }
                if (isSocBlock)
                {
                    lineText[i] = "";
                }
                newText += lineText[i] + "\n";
                i++;
            }
            using (StreamWriter sw = new StreamWriter(path, false, Encoding.Default))
            {
                sw.WriteLine(newText);
                sw.Flush();
                sw.Close();
            }

            isBlocked = false;
        }
       
    }
}
