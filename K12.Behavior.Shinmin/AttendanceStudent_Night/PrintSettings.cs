using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace K12.Behavior.Shinmin.AttendanceStatistics_進校
{
    public partial class PrintSettings : BaseForm
    {
        GetConfigSetup_n config;

        public PrintSettings()
        {
            InitializeComponent();
        }

        private void PrintSettings_Load(object sender, EventArgs e)
        {
            //取得設定內容
            config = new GetConfigSetup_n();
            checkBoxX1.Checked = config.略過六日資料;
            foreach (string each in config.AbsenceDic.Keys)
            {
                ListViewItem Item = new ListViewItem();
                Item.Name = each;
                Item.Text = each;
                Item.Checked = config.AbsenceDic[each];
                lvAbsence.Items.Add(Item);
            }
            foreach (string each in config.PeriodDic.Keys)
            {
                ListViewItem Item = new ListViewItem();
                Item.Name = each;
                Item.Text = each;
                Item.Checked = config.PeriodDic[each];
                lvPeriod.Items.Add(Item);
            }
            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            config.略過六日資料 = checkBoxX1.Checked;
            config.AbsenceDic = GetAbsence();
            config.PeriodDic = GetPeriod();
            config.SaveConfigSetup();
            MsgBox.Show("儲存成功!!");
            this.Close();
        }

        private Dictionary<string, bool> GetAbsence()
        {
            Dictionary<string, bool> dic = new Dictionary<string, bool>();
            foreach (ListViewItem item in lvAbsence.Items)
            {
                dic.Add(item.Text, item.Checked);
            }

            return dic;
        }

        private Dictionary<string, bool> GetPeriod()
        {
            Dictionary<string, bool> dic = new Dictionary<string, bool>();
            foreach (ListViewItem item in lvPeriod.Items)
            {
                dic.Add(item.Text, item.Checked);
            }
            return dic;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cbAbsence_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvAbsence.Items)
            {
                item.Checked = cbAbsence.Checked;
            }
        }

        private void cbPeriod_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListViewItem item in lvPeriod.Items)
            {
                item.Checked = cbPeriod.Checked;
            }
        }
    }
}
