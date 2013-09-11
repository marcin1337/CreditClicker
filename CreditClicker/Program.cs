using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace gmail
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        public static Form1 MyForm1;

        private static string sDir = Environment.CurrentDirectory + @"/Email.xml";
        private static string sWildcardDir = Environment.CurrentDirectory + @"/Wildcard.xml";
        private static string sGmailDir = Environment.CurrentDirectory + @"/gmail.txt";
        private static string sFolderDir = Environment.CurrentDirectory + @"/Folder.xml";

        public static List<WildCard> WildCards = new List<WildCard>();
        public static List<EmailAccount> EmailAccounts = new List<EmailAccount>();
        public static List<Folder> Folders = new List<Folder>();

        // TheCleaner only accepts .txt in a specific format
        public static bool SaveGmailtxt()
        {
            try
            {
                StringBuilder sb = new StringBuilder();

                foreach (EmailAccount w in EmailAccounts)
                {
                    sb.AppendLine("L: " + w.Login);
                    sb.AppendLine("P: " + w.Password);
                }

                using (StreamWriter sw = new StreamWriter(sGmailDir))
                {
                    sw.Write(sb.ToString());
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        public static bool GetFolders()
        {
            try
            {
                using (FileStream fs = new FileStream(sFolderDir, FileMode.Open))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(List<Folder>));

                    Folders = (List<Folder>)xml.Deserialize(fs);
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("No Folder.xml file found ");
                MessageBox.Show(e.ToString());
                return false;
            }
        }

        public static bool GetWildCards()
        {
            try
            {
                using (FileStream fs = new FileStream(sWildcardDir, FileMode.Open))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(List<WildCard>));

                    WildCards = (List<WildCard>)xml.Deserialize(fs);
                    return true;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
                MessageBox.Show("No WildCard.xml file found");
                return false;
            }
        }

        public static bool GetAccounts()
        {
            try
            {
                using (FileStream fs = new FileStream(sDir, FileMode.Open))
                {
                    XmlSerializer xml = new XmlSerializer(typeof(List<EmailAccount>));

                    EmailAccounts = (List<EmailAccount>)xml.Deserialize(fs);
                    return true;
                }
            }
            catch
            {
                MessageBox.Show("No Email.xml file found");
                return false;
            }
        }

        [STAThread]
        private static void Main()
        {
            GetWildCards();
            GetAccounts();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MyForm1 = new Form1();
            Application.Run(MyForm1);
        }
    }
}