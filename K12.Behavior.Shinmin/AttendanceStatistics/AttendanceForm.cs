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

namespace K12.Behavior.Shinmin.AttendanceStatistics
{
    public partial class AttendanceForm : BaseForm
    {
        BackgroundWorker BGW = new BackgroundWorker();

        DateTime _StartDate { get; set; }
        DateTime _EndDate { get; set; }

        public int 時間區間內總節數 { get; set; }

        public AttendanceForm()
        {
            InitializeComponent();
        }

        private void AttendanceForm_Load(object sender, EventArgs e)
        {
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
                btnPrint.Enabled = false;
                linkPrintSetup.Enabled = false;
                FISCA.Presentation.MotherForm.SetStatusBarMessage("開始列印[班級缺曠統計表]...");
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

            //計算時間區間總時數
            時間區間內總節數 = GetDateCount(config);

            //取得班級資料
            ClassRobot classRobot = new ClassRobot(config);
            //取得缺曠資料
            AttendanceObj attendanceObj = new AttendanceObj(config.略過六日資料, classRobot.StudentIDList, _StartDate, _EndDate);
            //開始統計缺曠資料
            classRobot.SetAttendanceInClassObj(attendanceObj);

            //依設定檔統計資料
            classRobot.SumOfAllTheInformation(時間區間內總節數);

            Workbook book = new Workbook();
            book.Open(new MemoryStream(Properties.Resources.班級缺曠統計表_範本), FileFormatType.Excel2003);
            book.Worksheets[0].Cells.Merge(0, 0, 1, config.AbsenceList.Count + 3);
            book.Worksheets[0].Cells[0, 0].PutValue("台中市私立新民高中　班級缺曠統計表");
            book.Worksheets[0].Cells[0, 0].Style.HorizontalAlignment = TextAlignmentType.Center; //置中
            //book.Worksheets[0].Cells[0, 0].Style.Font.Size = 20;
            //SetBorder(book, 0, 0, 1, config.AbsenceDic.Count + 3);

            book.Worksheets[0].Cells.CreateRange(0, 0, 1, config.AbsenceList.Count + 3).SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);

            book.Worksheets[0].Cells.Merge(1, 0, 1, config.AbsenceList.Count + 3);
            book.Worksheets[0].Cells[1, 0].PutValue("日期區間：" + _StartDate.ToShortDateString() + "至" + _EndDate.ToShortDateString());
            book.Worksheets[0].Cells[1, 0].Style.HorizontalAlignment = TextAlignmentType.Right; //置右

            //book.Worksheets[0].Cells[2, 0].PutValue("班級");

            Dictionary<string, int> AbsenceIndexDic = new Dictionary<string, int>();
            int AbsenceCount = 1;
            foreach (string each in config.AbsenceDic.Keys)
            {
                if (config.AbsenceDic[each]) //是列印內容之一則..
                {
                    book.Worksheets[0].Cells[2, AbsenceCount].PutValue(each);
                    AbsenceIndexDic.Add(each, AbsenceCount);
                    AbsenceCount++;
                }
            }
            book.Worksheets[0].Cells[2, AbsenceCount].PutValue("總缺席數");
            AbsenceIndexDic.Add("總缺席數", AbsenceCount);
            AbsenceCount++;
            book.Worksheets[0].Cells[2, AbsenceCount].PutValue("到課率");
            AbsenceIndexDic.Add("到課率", AbsenceCount);

            int RowIndex = 3;
            foreach (ClassDataObj each1 in classRobot.ClassDataObjDic.Values)
            {
                //班級名稱
                book.Worksheets[0].Cells[RowIndex, 0].PutValue(each1._classRecord.Name);
                //缺曠課名稱
                foreach (string each2 in each1.AbsenceDic.Keys)
                {
                    book.Worksheets[0].Cells[RowIndex, AbsenceIndexDic[each2]].PutValue(each1.AbsenceDic[each2]);
                }

                book.Worksheets[0].Cells[RowIndex, AbsenceIndexDic["總缺席數"]].PutValue(each1.總缺席數);
                book.Worksheets[0].Cells[RowIndex, AbsenceIndexDic["到課率"]].PutValue(each1.到課率);
                RowIndex++;
            }

            book.Worksheets[0].Cells.Merge(RowIndex, 0, 1, config.AbsenceList.Count + 3);
            book.Worksheets[0].Cells[RowIndex, 0].PutValue("列印日期：" + DateTime.Today.ToShortDateString());

            for (int xpp = 1; xpp <= book.Worksheets[0].Cells.MaxDataRow - 1; xpp++)
            {
                SetBorder(book, xpp, 0, 1, config.AbsenceList.Count + 3);
            }
            for (int xpp = 1; xpp <= book.Worksheets[0].Cells.MaxDataColumn; xpp++)
            {
                SetBorder(book, 0, xpp, book.Worksheets[0].Cells.MaxDataRow, 1); //book.Worksheets[0].Cells.MaxColumn
            }
            //book.Worksheets[0].Cells.Ranges[0].SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Thick, Color.Black);

            //book.Worksheets[0].Cells.CreateRange(0, RowIndex, true).SetOutlineBorder
            //(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);

            e.Result = book;

        }

        void SetBorder(Workbook book, int 起始Row, int 起始Column, int 幾行, int 幾列)
        {
            book.Worksheets[0].Cells.CreateRange(起始Row, 起始Column, 幾行, 幾列).SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);
            book.Worksheets[0].Cells.CreateRange(起始Row, 起始Column, 幾行, 幾列).SetOutlineBorder(BorderType.LeftBorder, CellBorderType.Thin, Color.Black);
            book.Worksheets[0].Cells.CreateRange(起始Row, 起始Column, 幾行, 幾列).SetOutlineBorder(BorderType.RightBorder, CellBorderType.Thin, Color.Black);
            book.Worksheets[0].Cells.CreateRange(起始Row, 起始Column, 幾行, 幾列).SetOutlineBorder(BorderType.TopBorder, CellBorderType.Thin, Color.Black);
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                Workbook book = (Workbook)e.Result;
                //儲存資料
                SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();
                sd.Title = "另存新檔";
                sd.FileName = "班級缺曠統計表.xls";
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
            FISCA.Presentation.MotherForm.SetStatusBarMessage("[班級缺曠統計表]列印完成。");
            btnPrint.Enabled = true;
            linkPrintSetup.Enabled = true;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkPrintSetup_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            PrintSettings ps = new PrintSettings();
            ps.ShowDialog();
        }

        /// <summary>
        /// 計算總節次
        /// </summary>
        private int GetDateCount(GetConfigSetup config)
        {
            int jj = 0;
            int ag = 0;
            //算出共幾節次
            foreach (string each in config.PeriodDic.Keys)
            {
                if (config.PeriodDic[each])
                {
                    ag++;
                }
            }

            if (config.略過六日資料) //略過六日資料為True
            {
                TimeSpan ss = _EndDate - _StartDate;
                int count = 0;
                for (int x = 0; x <= ss.Days; x++)
                {
                    DateTime dtx = _StartDate.AddDays(x);
                    if (dtx.DayOfWeek != DayOfWeek.Saturday && dtx.DayOfWeek != DayOfWeek.Sunday)
                    {
                        count++;
                    }
                }
                jj = count * ag;
            }
            else //統計所有日期
            {
                TimeSpan ts = _EndDate - _StartDate;
                jj = (ts.Days + 1) * ag;
            }
            return jj;
        }

        //private DateTime GetWeekFirstDay(DateTime inputDate)
        //{
        //    switch (inputDate.DayOfWeek)
        //    {
        //        case DayOfWeek.Monday:
        //            return inputDate;
        //        case DayOfWeek.Tuesday:
        //            return inputDate.AddDays(-1);
        //        case DayOfWeek.Wednesday:
        //            return inputDate.AddDays(-2);
        //        case DayOfWeek.Thursday:
        //            return inputDate.AddDays(-3);
        //        case DayOfWeek.Friday:
        //            return inputDate.AddDays(-4);
        //        case DayOfWeek.Saturday:
        //            return inputDate.AddDays(-5);
        //        default:
        //            return inputDate.AddDays(-6);
        //    }
        //}
    }
}
