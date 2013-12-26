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
using K12.Data;

namespace K12.Behavior.Shinmin.Night
{
    public partial class AttendanceForm_n : BaseForm
    {
        BackgroundWorker BGW = new BackgroundWorker();

        DateTime _StartDate { get; set; }
        DateTime _EndDate { get; set; }

        public int 時間區間內總節數 { get; set; }

        List<string> _StudIDList { get; set; }

        public AttendanceForm_n(List<string> StudIDList)
        {
            InitializeComponent();

            _StudIDList = StudIDList;
        }

        private void AttendanceForm_n_Load(object sender, EventArgs e)
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
                FISCA.Presentation.MotherForm.SetStatusBarMessage("開始列印[進校學生缺席統計表]...");
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
            GetConfigSetup_n config = new GetConfigSetup_n();

            Dictionary<string, ClassDataObj> ClassObjDic = new Dictionary<string, ClassDataObj>();
            //設計一個班級容器 / 內含學生資料與統計

            List<string> ClassIDList = new List<string>();
            List<string> StudIDList = new List<string>();
            foreach (StudentRecord each in Student.SelectByIDs(_StudIDList))
            {
                if (string.IsNullOrEmpty(each.RefClassID))
                    continue; //沒有班級

                if (!ClassIDList.Contains(each.RefClassID))
                {
                    ClassIDList.Add(each.RefClassID);
                }

                StudIDList.Add(each.ID);
            }

            foreach (ClassRecord cr in Class.SelectByIDs(ClassIDList))
            {
                ClassDataObj obj = new ClassDataObj(cr, config.AbsenceList);

                if (!ClassObjDic.ContainsKey(cr.ID))
                {
                    ClassObjDic.Add(cr.ID, obj);
                }
            }

            //取得缺曠資料 & 移除六日資料
            AttendanceObj_n attendanceObj = new AttendanceObj_n(config.略過六日資料, StudIDList, _StartDate, _EndDate);

            foreach (AttendanceRecord attendance in attendanceObj.AttendanceList)
            {
                string classID = attendance.Student.RefClassID;

                if (ClassObjDic.ContainsKey(classID))
                {
                    if (ClassObjDic[classID].StudentDic.ContainsKey(attendance.RefStudentID))
                    {
                        //把物件名稱縮短...                   
                        Dictionary<string, int> dic = ClassObjDic[classID].StudentDic[attendance.RefStudentID].AbsenceDic;
                        foreach (AttendancePeriod attPeriod in attendance.PeriodDetail)
                        {
                            if (dic.ContainsKey(attPeriod.AbsenceType))
                            {
                                //統計資料
                                dic[attPeriod.AbsenceType]++;
                            }
                        }
                    }
                }
            }

            foreach (ClassDataObj classObj in ClassObjDic.Values)
            {

                //必須增加Class迴圈,並且針對每名學生進行資料填取
                foreach (StudentDataObj studObj in classObj.StudentDic.Values)
                {
                    studObj.Total();
                }
            }

            #region 舊資料

            Workbook book = new Workbook();
            book.Open(new MemoryStream(Properties.Resources.進校學生缺席統計表_範本), FileFormatType.Excel2003);

            //標題
            book.Worksheets[0].Cells.Merge(0, 0, 1, config.AbsenceList.Count + 4);
            book.Worksheets[0].Cells[0, 0].PutValue("台中市私立新民高中進修學校　學生缺席統計表");
            book.Worksheets[0].Cells[0, 0].Style.HorizontalAlignment = TextAlignmentType.Center; //置中
            //book.Worksheets[0].Cells[0, 0].Style.Font.Size = 20;
            //SetBorder(book, 0, 0, 1, config.AbsenceList.Count + 4);
            //SetBorder(book, 1, 0, 1, config.AbsenceList.Count + 4);

            //book.Worksheets[0].Cells.CreateRange(0, 0, 1, config.AbsenceList.Count + 4).SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);

            book.Worksheets[0].Cells.Merge(1, 0, 1, config.AbsenceList.Count + 4);
            book.Worksheets[0].Cells[1, 0].PutValue("日期區間：" + _StartDate.ToShortDateString() + "至" + _EndDate.ToShortDateString());
            book.Worksheets[0].Cells[1, 0].Style.HorizontalAlignment = TextAlignmentType.Right; //置右

            //book.Worksheets[0].Cells[2, 0].PutValue("班級");
            //book.Worksheets[0].Cells[2, 1].PutValue("座號");
            //book.Worksheets[0].Cells[2, 2].PutValue("姓名");

            //統計使用者選了幾個缺曠別
            Dictionary<string, int> AbsenceIndexDic = new Dictionary<string, int>();
            int AbsenceCount = 3;
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

            //AbsenceCount++;
            //book.Worksheets[0].Cells[2, AbsenceCount].PutValue("到課率");
            //AbsenceIndexDic.Add("到課率", AbsenceCount);

            int RowIndex = 3;
            foreach (ClassDataObj classObj in ClassObjDic.Values)
            {

                //必須增加Class迴圈,並且針對每名學生進行資料填取
                foreach (StudentDataObj studObj in classObj.StudentDic.Values)
                {
                    if (studObj.總缺席數 < config.缺席數)
                        continue;

                    //班級名稱
                    book.Worksheets[0].Cells[RowIndex, 0].PutValue(classObj._classRecord.Name);
                    //座號
                    book.Worksheets[0].Cells[RowIndex, 1].PutValue(studObj._stud.SeatNo.HasValue ? studObj._stud.SeatNo.Value.ToString() : "");
                    //姓名
                    book.Worksheets[0].Cells[RowIndex, 2].PutValue(studObj._stud.Name);

                    //缺曠課名稱
                    foreach (string each2 in studObj.AbsenceDic.Keys)
                    {
                        book.Worksheets[0].Cells[RowIndex, AbsenceIndexDic[each2]].PutValue(studObj.AbsenceDic[each2]);
                    }

                    book.Worksheets[0].Cells[RowIndex, AbsenceIndexDic["總缺席數"]].PutValue(studObj.總缺席數);
                    RowIndex++;

                }
            }

            book.Worksheets[0].Cells.Merge(RowIndex, 0, 1, config.AbsenceList.Count + 4);
            book.Worksheets[0].Cells[RowIndex, 0].PutValue("列印日期：" + DateTime.Today.ToShortDateString());

            for (int xpp = 0; xpp <= book.Worksheets[0].Cells.MaxDataRow - 1; xpp++)
            {
                SetBorder(book, xpp, 0, 1, config.AbsenceList.Count + 4);
            }
            for (int xpp = 0; xpp <= book.Worksheets[0].Cells.MaxDataColumn; xpp++)
            {
                SetBorder(book, 0, xpp, book.Worksheets[0].Cells.MaxDataRow, 1);
                //book.Worksheets[0].Cells.MaxColumn
            }
            //book.Worksheets[0].Cells.Ranges[0].SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Thick, Color.Black);

            //book.Worksheets[0].Cells.CreateRange(0, RowIndex, true).SetOutlineBorder
            //(BorderType.BottomBorder, CellBorderType.Thin, Color.Black);

            #endregion

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
                sd.FileName = "進校學生缺席統計表.xls";
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
            FISCA.Presentation.MotherForm.SetStatusBarMessage("[進校學生缺席統計表]列印完成。");
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
    }
}
