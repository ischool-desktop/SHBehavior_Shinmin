using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using K12.Data;
using Aspose.Cells;
using System.IO;

namespace K12.Behavior.Shinmin.StudentsSpecial
{
    public partial class StudentsSpecialForm : BaseForm
    {
        BackgroundWorker BGW = new BackgroundWorker();

        int _SchoolYear { get; set; }
        int _Semester { get; set; }
        bool _SelectByTotle { get; set; }

        public StudentsSpecialForm()
        {
            InitializeComponent();
        }

        private void StudentsSpecial_Load(object sender, EventArgs e)
        {
            intSchoolYear.Value = int.Parse(School.DefaultSchoolYear);
            intSemester.Value = int.Parse(School.DefaultSemester);

            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (!BGW.IsBusy)
            {
                _SelectByTotle = radioButton1.Checked;
                _SchoolYear = intSchoolYear.Value;
                _Semester = intSemester.Value;
                btnPrint.Enabled = false;
                linkPrintSetup.Enabled = false;
                BGW.RunWorkerAsync();
            }
            else
            {
                MsgBox.Show("功能忙碌中,請稍後再試!!");
            }
        }


        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            //取得設定檔
            GetConfigSetup configSetup = new GetConfigSetup();

            //取得班級資料
            StudentRobot classRobot = new StudentRobot();
            //取得獎懲資料
            MeritDemeritObj mdOBJ;
            if (_SelectByTotle)
            {
                //取得學生所有獎懲記錄進行統計
                mdOBJ = new MeritDemeritObj(classRobot.StudentIDList);
            }
            else
            {
                //依學生各學期進行統計
                mdOBJ = new MeritDemeritObj(classRobot.StudentIDList, _SchoolYear, _Semester);
            }
            //統計資料
            classRobot.SetMeritDemeritInClassObj(mdOBJ);

            //開始依設定檔統計資料 / 換算資料
            classRobot.SumOfAll(configSetup);

            Workbook template = new Workbook();
            template.Open(new MemoryStream(Properties.Resources.功過相抵名單_範本));
            Range prototypeRow = template.Worksheets[0].Cells.CreateRange(3, 1, false);

            //開始列印資料
            Workbook book = new Workbook();
            book.Open(new MemoryStream(Properties.Resources.功過相抵名單_範本));

            string Title = "";
            if (_SelectByTotle)
            {
                Title = "新民高級中學　功過相抵滿三大過學生";
            }
            else
            {
                Title = "新民高級中學　" + _SchoolYear + "學年度第" + _Semester + "學期　功過相抵滿三大過學生";
            }

            book.Worksheets[0].Cells[0, 0].PutValue(Title); //標頭

            int Count = 1;
            int RowIndex = 3;
            foreach (StudentDateObj each in classRobot.StudentDateObjDic.Values)
            {
                book.Worksheets[0].Cells.CreateRange(RowIndex, 1, false).Copy(prototypeRow);

                book.Worksheets[0].Cells[RowIndex, 0].PutValue(Count); //序號

                if (!string.IsNullOrEmpty(each._StudentRecord.RefClassID)) //班級
                {
                    if (classRobot.ClassRecordDic.ContainsKey(each._StudentRecord.RefClassID))
                    {
                        book.Worksheets[0].Cells[RowIndex, 1].PutValue(classRobot.ClassRecordDic[each._StudentRecord.RefClassID].Name);
                    }
                }                
                book.Worksheets[0].Cells[RowIndex, 2].PutValue(each._StudentRecord.SeatNo.HasValue ? each._StudentRecord.SeatNo.Value.ToString() : ""); //座號
                book.Worksheets[0].Cells[RowIndex, 3].PutValue(each._StudentRecord.Name);
                book.Worksheets[0].Cells[RowIndex, 4].PutValue(each._大功);
                book.Worksheets[0].Cells[RowIndex, 5].PutValue(each._小功);
                book.Worksheets[0].Cells[RowIndex, 6].PutValue(each._嘉獎);

                book.Worksheets[0].Cells[RowIndex, 7].PutValue(each._大過);
                book.Worksheets[0].Cells[RowIndex, 8].PutValue(each._小過);
                book.Worksheets[0].Cells[RowIndex, 9].PutValue(each._警告);

                book.Worksheets[0].Cells[RowIndex, 10].PutValue(each.功過相抵_大過);
                book.Worksheets[0].Cells[RowIndex, 11].PutValue(each.功過相抵_小過);
                book.Worksheets[0].Cells[RowIndex, 12].PutValue(each.功過相抵_警告);

                RowIndex++; //行++
                Count++; //項次++
            }


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
                sd.FileName = "特殊學生表現名單.xls";
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

            FISCA.Presentation.MotherForm.SetStatusBarMessage("[特殊學生表現名單]列印完成。");
            btnPrint.Enabled = true;
            linkPrintSetup.Enabled = true;
        }

        private void linkPrintSetup_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ReduceForm rf = new ReduceForm();
            rf.ShowDialog();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            intSchoolYear.Enabled = !radioButton1.Checked;
            intSemester.Enabled = !radioButton1.Checked;
            txtSchoolYear.Enabled = !radioButton1.Checked;
            txtSemester.Enabled = !radioButton1.Checked;
        }
    }
}
