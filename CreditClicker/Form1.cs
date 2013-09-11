using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

//using AE.Net.Mail;
//using AE.Net.Mail.Imap;

namespace gmail
{
    public partial class Form1 : Form
    {
        public BackgroundWorker BackgroundScanner = new BackgroundWorker();

        private delegate void LabelDelagate(string folder);

        private delegate void TickDelegate(object source, ElapsedEventArgs e);

        private delegate void FormDelegate();

        private AutoResetEvent evt = new AutoResetEvent(false);
        private System.Windows.Forms.Timer WebsiteTimer = new System.Windows.Forms.Timer();

        private int iWebsiteDelay = 0;

        private bool Pause;

        public Form1()
        {
            InitializeComponent();

            Pause = false;

            webBrowser1.ScriptErrorsSuppressed = true;

            WebsiteTimer.Enabled = true;
            WebsiteTimer.Tick += WebsiteTimerFinished;
            WebsiteTimer.Stop();

            BackgroundScanner.WorkerReportsProgress = true;
            BackgroundScanner.WorkerSupportsCancellation = true;

            BackgroundScanner.DoWork += new DoWorkEventHandler(BackgroundScanner_DoWork);
            BackgroundScanner.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BackgroundScanner_RunWorkerCompleted);
        }

        private void ChangeCurrentFolderLabel(string folder)
        {
            if (FolderLabel.InvokeRequired)
            {
                LabelDelagate d = new LabelDelagate(ChangeCurrentFolderLabel);
                this.Invoke(d, new Object[] { folder });
            }
            else
                FolderLabel.Text = folder;
        }

        private void ChangeCurLabel(string s)
        {
            if (CurEmail.InvokeRequired)
            {
                LabelDelagate d = new LabelDelagate(ChangeCurLabel);
                this.Invoke(d, new Object[] { s });
            }
            else
                CurEmail.Text = s;
        }

        private int ParseDelay(string s)
        {
            // DELAY_X
            string sNumber = s.Substring(6, s.Length - 6);

            return Int32.Parse(sNumber);
        }

        private void WebsiteTimerFinished(object sender, EventArgs e)
        {
            WebsiteTimer.Stop();
            evt.Set();
        }

        private void ChangeMaxLabel(string s)
        {
            if (this.MaxEmail.InvokeRequired)
            {
                LabelDelagate d = new LabelDelagate(ChangeMaxLabel);
                this.Invoke(d, new Object[] { s });
            }
            else
                MaxEmail.Text = s;
        }

        private void FocusWindow()
        {
            if (this.InvokeRequired)
            {
                FormDelegate d = new FormDelegate(FocusWindow);
                this.Invoke(d, new Object[] { });
            }
            else
                this.Activate();
        }

        private void ParseWildcardOption(string sOption, string sWebsite)
        {
            if ((sOption == "CAPTCHA") || (sOption == "IMGCLICK"))
            {
                this.FocusWindow();

                ViewBrowser(sWebsite);
            }
            else
            {
                iWebsiteDelay = ParseDelay(sOption);
                ViewBrowser(sWebsite);
            }
        }

        private void Continue()
        {
            WebsiteTimer.Stop();

            Pause = false;
            evt.Set();
        }

        private void StartScan()
        {
            Gmail myGmail = new Gmail();

            string buffer = "";

            int iNumEmails = 0;

            Program.Folders.Clear();

            for (int j = 0; j < Program.EmailAccounts.Count; j++)
            {
                ImapClient ic = new ImapClient("imap.gmail.com", Program.EmailAccounts[j].Login, Program.EmailAccounts[j].Password, ImapClient.AuthMethods.Login, 993, true);

                Mailbox[] ma = ic.ListMailboxes("", "*");

                foreach (Mailbox ms in ma)
                {
                    if (ms.Name.StartsWith("[Gmail]"))
                        continue;

                    Folder TempObject2 = new Folder();

                    TempObject2.FolderName = ms.Name;

                    Program.Folders.Add(TempObject2);
                }

                Folder TempObject3 = new Folder();

                TempObject3.FolderName = "[Gmail]/Spam";

                Program.Folders.Add(TempObject3);

                for (int i = 0; i < Program.Folders.Count; i++)
                {
                    string sFolder = Program.Folders[i].FolderName;

                    iNumEmails = ic.GetMessageCount(sFolder);

                    if (iNumEmails == 0)
                        continue;

                    ChangeCurrentFolderLabel(sFolder);

                    ic.SelectMailbox(sFolder);

                    for (int k = 1; k < iNumEmails; k++)
                    {
                        if (Pause)
                            evt.WaitOne();

                        ChangeCurLabel((k + 1).ToString());
                        ChangeMaxLabel(iNumEmails.ToString());

                        MailMessage m = null;

                        myGmail.iEmail = k;
                        try
                        {
                            m = ic.GetMessage(k, false, true);

                            if ((buffer = myGmail.CheckEmails(sFolder, m.Body)) != "None")
                            {
                                ParseWildcardOption(myGmail.CurWildcardOption, buffer);

                                evt.WaitOne();
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }
        }

        private void BackgroundScanner_DoWork(object sender, DoWorkEventArgs e)
        {
            StartScan();
            MessageBox.Show("Done!");
        }

        private void BackgroundScanner_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        public void ViewBrowser(string sUrl)
        {
            try
            {
                Uri URL = new Uri(sUrl);
                webBrowser1.Url = URL;
            }
            catch
            {
                FocusWindow();
            }
        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            label2.Text = "Currently Checking:";

            if (BackgroundScanner.IsBusy)
                return;
            else
                BackgroundScanner.RunWorkerAsync();
        }

        private void bContinue_Click(object sender, EventArgs e)
        {
            Continue();
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.AbsolutePath != (sender as WebBrowser).Url.AbsolutePath)
            {
                WebsiteTimer.Interval = 60 * 1000;
                WebsiteTimer.Start();
                return;
            }
            if (iWebsiteDelay != 0)
            {
                WebsiteTimer.Interval = iWebsiteDelay * 1000;
                WebsiteTimer.Start();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (BackgroundScanner.IsBusy && !Pause)
            {
                MessageBox.Show("Editing is not possible when scanning");
            }
            else
            {
                EditWildcards myEditWildCards = new EditWildcards();
                myEditWildCards.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Pause = true;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (BackgroundScanner.IsBusy)
            {
                MessageBox.Show("Editing is not possible when scanning");
            }
            else
            {
                EditEmailAccounts myEditEmailAccounts = new EditEmailAccounts();
                myEditEmailAccounts.Show();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("www.CreditClicker.com");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("www.bee4.biz/?ref=20336");
        }

        private void FolderLabel_Click(object sender, EventArgs e)
        {
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Warning this is a very time intensive function ( 5 - 10 minutes )");
            Gmail myGmail = new Gmail();

            int iNumEmails = 0;

            Program.Folders.Clear();

            for (int j = 0; j < Program.EmailAccounts.Count; j++)
            {
                ImapClient ic = new ImapClient("imap.gmail.com", Program.EmailAccounts[j].Login, Program.EmailAccounts[j].Password, ImapClient.AuthMethods.Login, 993, true);

                Mailbox[] ma = ic.ListMailboxes("", "*");

                foreach (Mailbox ms in ma)
                {
                    if (ms.Name.StartsWith("[Gmail]"))
                        continue;

                    Folder TempObject2 = new Folder();

                    TempObject2.FolderName = ms.Name;

                    Program.Folders.Add(TempObject2);
                }

                Folder TempObject3 = new Folder();

                TempObject3.FolderName = "[Gmail]/Spam";

                Program.Folders.Add(TempObject3);

                for (int i = 0; i < Program.Folders.Count; i++)
                {
                    string sFolder = Program.Folders[i].FolderName;

                    iNumEmails = ic.GetMessageCount(sFolder);

                    if (iNumEmails == 0)
                        continue;

                    ChangeCurrentFolderLabel(sFolder);

                    ic.SelectMailbox(sFolder);

                    try
                    {
                        ic.DeleteMessage(1);
                    }
                    catch
                    {
                    }
                }
            }
            MessageBox.Show("All Done cleaning Emails");
        }
    }
}