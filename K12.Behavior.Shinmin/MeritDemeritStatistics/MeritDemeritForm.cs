using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using Aspose.Cells;
using System.IO;

namespace K12.Behavior.Shinmin.MeritDemeritStatistics
{
    public partial class MeritDemeritForm : FISCA.Presentation.Controls.BaseForm
    {
        BackgroundWorker BGW = new BackgroundWorker();

        DateTime _StartDate { get; set; }
        DateTime _EndDate { get; set; }
        bool _IsByOccurDate { get; set; }

        public MeritDemeritForm(bool OpenSetup)
        {
            InitializeComponent();
            linkPrintSetup.Visible = OpenSetup;
        }

        private void MeritDemeritForm_Load(object sender, EventArgs e)
        {
            //如果沒有設定檔,會預設設定檔
            dtStartDate.Value = DateTime.Today;
            dtEndDate.Value = DateTime.Today;

            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (!BGW.IsBusy)
            {
                _StartDate = dtStartDate.Value;
                _EndDate = dtEndDate.Value;
                _IsByOccurDate = rbByOccurDate.Checked;
                btnPrint.Enabled = false;
                linkPrintSetup.Enabled = false;
                FISCA.Presentation.MotherForm.SetStatusBarMessage("開始列印[班級獎懲統計表]...");
                BGW.RunWorkerAsync();
            }
            else
            {
                MsgBox.Show("功能忙碌中,請稍後再試!!");
            }

        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            //取得最新設定檔
            GetConfigSetup config = new GetConfigSetup();
            //收集資料
            ClassRobot classRobot = new ClassRobot();
            //取得獎懲資料
            MeritDemeritObj meritDemeritObj = new MeritDemeritObj(config.略過班導師註記, _IsByOccurDate, classRobot.StudentIDList, _StartDate, _EndDate);
            //統計獎懲資料各班總計
            classRobot.SetMeritDemeritInClassObj(meritDemeritObj);

            //依設定檔統計資料
            classRobot.SumOfAllTheInformation(config);

            Workbook template = new Workbook();

            template.Open(new MemoryStream(Properties.Resources.班級獎懲統計表_範本), FileFormatType.Excel2003);
            Range prototypeRow = template.Worksheets[0].Cells.CreateRange(3, 1, false);
            
            Workbook book = new Workbook();
            book.Open(new MemoryStream(Properties.Resources.班級獎懲統計表_範本));

            book.Worksheets[0].Cells[1, 0].PutValue("日期區間：" + _StartDate.ToShortDateString() + "至" + _EndDate.ToShortDateString());
            //列印資料
            int ClassIndex = 3;

            foreach (ClassDataObj each in classRobot.ClassDataObjDic.Values)
            {
                book.Worksheets[0].Cells.CreateRange(ClassIndex, 1, false).Copy(prototypeRow);

                book.Worksheets[0].Cells[ClassIndex, 0].PutValue(each._classRecord.Name);
                book.Worksheets[0].Cells[ClassIndex, 1].PutValue(each._大功);
                book.Worksheets[0].Cells[ClassIndex, 2].PutValue(each._小功);
                book.Worksheets[0].Cells[ClassIndex, 3].PutValue(each._嘉獎);
                book.Worksheets[0].Cells[ClassIndex, 4].PutValue(each._大過);
                book.Worksheets[0].Cells[ClassIndex, 5].PutValue(each._小過);
                book.Worksheets[0].Cells[ClassIndex, 6].PutValue(each._警告);
                book.Worksheets[0].Cells[ClassIndex, 7].PutValue(each._總分);
                ClassIndex++;
            }
            book.Worksheets[0].Cells.Merge(ClassIndex, 0, 1, 10);
            book.Worksheets[0].Cells[ClassIndex, 0].PutValue("列印日期：" + DateTime.Today.ToShortDateString());
            e.Result = book;          
            
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Workbook book = (Workbook)e.Result;
                //儲存資料
                SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = "班級獎懲統計表.xls";
                sd.Filter = "Excel檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        book.Save(sd.FileName, FileFormatType.Excel2003);
                        System.Diagnostics.Process.Start(sd.FileName);

                    }
                    catch
                    {
                        FISCA.Presentation.MotherForm.SetStatusBarMessage("指定路徑無法存取。");
                        MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnPrint.Enabled = true;
                        linkPrintSetup.Enabled = true;
                        return;
                    }
                } 
            }
            else
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("資料列印時發生錯誤!!");
                MsgBox.Show("資料列印時發生錯誤!!" + e.Error.Message);
                btnPrint.Enabled = true;
                linkPrintSetup.Enabled = true;
                return;
            }
            FISCA.Presentation.MotherForm.SetStatusBarMessage("[班級獎懲統計表]列印完成。");
            btnPrint.Enabled = true;
            linkPrintSetup.Enabled = true;
        }

        private void linkPrintSetup_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PlusTheStandardDeduction pts = new PlusTheStandardDeduction();
            pts.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }        
    }
}
