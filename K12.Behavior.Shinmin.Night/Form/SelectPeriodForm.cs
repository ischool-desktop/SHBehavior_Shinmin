using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using FISCA.Presentation.Controls;
using K12.Data;
using K12.Data.Configuration;

namespace K12.Behavior.Shinmin.Night
{
    public partial class SelectPeriodForm : BaseForm
    {
        private BackgroundWorker _BGWPeriodList;
        private string _key;

        private List<string> periodList = new List<string>();

        public SelectPeriodForm(string key)
        {
            InitializeComponent();
            _key = key;
        }

        void _BGWPeriodList_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            foreach (string period in periodList)
            {
                ListViewItem item = new ListViewItem(period);
                listViewEx1.Items.Add(item);
            }

            #region Ū���C�L�]�w Preference

            XmlElement config = School.Configuration[_key].GetXml("XmlData", null);

            if (config == null) return;

            foreach (XmlElement period in config.SelectNodes("Period"))
            {
                string name = period.GetAttribute("Name");
                foreach (ListViewItem item in listViewEx1.Items)
                {
                    if (item.Text == name)
                        item.Checked = true;
                }
            }

            #endregion
        }

        void _BGWPeriodList_DoWork(object sender, DoWorkEventArgs e)
        {
            foreach (XmlElement var in Config.GetPeriodList().GetContent().GetElements("Period"))
            {
                if (!periodList.Contains(var.GetAttribute("Name")))
                    periodList.Add(var.GetAttribute("Name"));
            }
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            #region ��s�C�L�]�w Preference


            XmlElement config = K12.Data.School.Configuration[_key].GetXml("XmlData", null);
            
            if (config == null)
            {
                config = XmlHelper.LoadXml("<PeriodSetup/>");
            }

            config.RemoveAll();

            foreach (ListViewItem item in listViewEx1.Items)
            {
                XmlElement period = config.OwnerDocument.CreateElement("Period");
                period.SetAttribute("Name", item.Text);
                if (item.Checked == true)
                    config.AppendChild(period);
            }

            ConfigData cd = School.Configuration[_key];
            cd.SetXml("XmlData", config);
            cd.Save();

            #endregion

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SelectPeriodForm_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {

                _BGWPeriodList = new BackgroundWorker();
                _BGWPeriodList.DoWork += new DoWorkEventHandler(_BGWPeriodList_DoWork);
                _BGWPeriodList.RunWorkerCompleted += new RunWorkerCompletedEventHandler(_BGWPeriodList_RunWorkerCompleted);
                _BGWPeriodList.RunWorkerAsync();
                listViewEx1.Items.Clear();
            }
        }

        private void checkBoxX1_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem each in listViewEx1.Items)
            {
                each.Checked = checkBoxX1.Checked;
            }
        }
    }
}