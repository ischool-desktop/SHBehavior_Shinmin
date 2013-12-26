using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace K12.Behavior.Shinmin.MeritDemeritStatistics
{
    public partial class PlusTheStandardDeduction : FISCA.Presentation.Controls.BaseForm
    {

        GetConfigSetup config;

        public PlusTheStandardDeduction()
        {
            InitializeComponent();
        }

        private void PlusTheStandardDeduction_Load(object sender, EventArgs e)
        {
            //取得設定內容
            config = new GetConfigSetup();
            //初始化設定檔內容於畫面
            intClassBase.Value = config.班級基本分;
            cbMax100.Checked = config.啟用總分100分限制;
            cbIsTeacher.Checked = config.略過班導師註記;
            intMeritA.Value = config.大功;
            intMeritB.Value = config.小功;
            intMeritC.Value = config.嘉獎;
            intDemeritA.Value = config.大過;
            intDemeritB.Value = config.小過;
            intDemeritC.Value = config.警告;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            config.班級基本分 = intClassBase.Value;
            config.啟用總分100分限制 = cbMax100.Checked;
            config.略過班導師註記 = cbIsTeacher.Checked;
            config.大功 = intMeritA.Value;
            config.小功 = intMeritB.Value;
            config.嘉獎 = intMeritC.Value;
            config.大過 = intDemeritA.Value;
            config.小過 = intDemeritB.Value;
            config.警告 = intDemeritC.Value;
            config.SaveConfigSetup();
            MsgBox.Show("儲存成功!!");
            this.Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
