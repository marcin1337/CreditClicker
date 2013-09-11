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

namespace gmail
{
    public partial class EditWildcards : Form
    {
        public EditWildcards()
        {
            InitializeComponent();
            InitDataSource();
        }

        DataSet WildcardDataSet = new DataSet();
        string sWildcardDir = Environment.CurrentDirectory + @"/Wildcard.xml";

        private void InitDataSource()
        {
            try
            {

                WildcardDataSet.ReadXml(sWildcardDir);

                dataGridView1.AutoGenerateColumns = false;
                dataGridView1.DataSource = WildcardDataSet;

                dataGridView1.DataMember = "Wildcard";
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }
        private void SaveImportedWildCardsDataset()
        {
            File.Delete(sWildcardDir);
            WildcardDataSet.WriteXml(sWildcardDir, System.Data.XmlWriteMode.IgnoreSchema);
        }

        
        private void SaveButton_Click(object sender, EventArgs e)
        {                     
            try
            {
                 SaveImportedWildCardsDataset();
                 Program.GetWildCards();
            }
            catch(Exception ee )
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void ImportButton_Click(object sender, EventArgs e)
        {
            if (ImportWildCard.ShowDialog() == DialogResult.OK)
            {
                if (Path.GetExtension(ImportWildCard.FileName) != ".txt")
                {
                    MessageBox.Show("Program only accepts .txt for now, please refer to manual");
                }
                else
                {
                    List<WildCard> TempList = new List<WildCard>();


                        WildcardDataSet.Tables[0].Rows.Clear();
                    
                   
                    string line = "";

                    using (StreamReader sr = new StreamReader(ImportWildCard.FileName))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            WildCard TempWildCard = new WildCard();
                            
                            DataRow TempRow = WildcardDataSet.Tables[0].NewRow();
                      
                            TempWildCard.URL = line;
                            TempWildCard.Option = "DELAY_20";

                            TempRow[0] = TempWildCard.URL;
                            TempRow[1] = TempWildCard.Option;

                            WildcardDataSet.Tables[0].Rows.Add(TempRow);

                            TempList.Add(TempWildCard);                          
                        }
                    }

                }
                
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
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
}
