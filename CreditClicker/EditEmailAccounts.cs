using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.IO;
using System.Diagnostics;

namespace gmail
{
    public partial class EditEmailAccounts : Form
    {
        string sDir = Environment.CurrentDirectory + @"/Email.xml";
        
        public EditEmailAccounts()
        {
            InitializeComponent();

            try
            {       
                EmailAccountDataSet.ReadXml(sDir);

                dataGridView1.AutoGenerateColumns = false;
                dataGridView1.DataSource = EmailAccountDataSet;

                dataGridView1.DataMember = "EmailAccount";
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {         
                EmailAccountDataSet.WriteXml(sDir, System.Data.XmlWriteMode.IgnoreSchema);
                
                Program.GetAccounts();
                Program.SaveGmailtxt();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }
        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                {
                    try
                    {
                        dataGridView1.Rows.Remove(row);
                    }
                    catch
                    {
                    }
                }
            }
        }

    }
}
