using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.UDT;
using K12.Data;
using FISCA.Presentation.Controls;

namespace K12.Keyboard.Shinmin
{
    public partial class TeacherPointCheck : FISCA.Presentation.Controls.BaseForm
    {
        //DAL未提供依編號取得資料
        AccessHelper _accessHelper = new AccessHelper();

        public TeacherPointCheck()
        {
            InitializeComponent();

            K12.Presentation.NLDPanels.Student.TempSourceChanged += new EventHandler(Student_TempSourceChanged);

            labelX1.Text = "學生待處理：" + K12.Presentation.NLDPanels.Student.TempSource.Count;
        }

        private void TeacherPointCheck_Load(object sender, EventArgs e)
        {
            GetMeritList();

            GetDemeritList();

        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            PassWord pw = new PassWord();
            DialogResult dr = pw.ShowDialog();
            if (dr == DialogResult.Yes)
            {
                //註記物件
                List<TeacherSetMerit> list1 = new List<TeacherSetMerit>();
                List<TeacherSetDemerit> list2 = new List<TeacherSetDemerit>();
                //獎懲物件
                List<MeritRecord> list3 = new List<MeritRecord>();
                List<DemeritRecord> list4 = new List<DemeritRecord>();
                //Row
                List<DataGridViewRow> DelRow = new List<DataGridViewRow>();

                //透過選擇的Row進行註記資料刪除
                foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
                {
                    if (row.IsNewRow)
                        continue;

                    if (row.Cells[0].Tag is TeacherSetMerit)
                    {
                        list1.Add((TeacherSetMerit)row.Cells[0].Tag);
                    }
                    else if (row.Cells[0].Tag is TeacherSetDemerit)
                    {
                        list2.Add((TeacherSetDemerit)row.Cells[0].Tag);
                    }

                    if (row.Cells[1].Tag is MeritRecord)
                    {
                        list3.Add((MeritRecord)row.Cells[1].Tag);
                    }
                    else if (row.Cells[1].Tag is DemeritRecord)
                    {
                        list4.Add((DemeritRecord)row.Cells[1].Tag);
                    }

                    DelRow.Add(row);
                }

                _accessHelper.DeletedValues(list1.ToArray());
                _accessHelper.DeletedValues(list2.ToArray());

                if (checkBoxX1.Checked)
                {
                    Merit.Delete(list3);
                    Demerit.Delete(list4);
                }

                foreach (DataGridViewRow row in DelRow)
                {
                    dataGridViewX1.Rows.Remove(row);
                }
            }
            else
            {
                MsgBox.Show("輸入錯誤");
            }
        }

        /// <summary>
        /// 取得有註記之獎勵資料
        /// </summary>
        private void GetMeritList()
        {
            foreach (MeritRecord dem in TeacherNote.GetTeacherNoteMeritList())
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);
                row.Cells[0].Value = "V";
                row.Cells[1].Value = "獎勵";
                row.Cells[2].Value = dem.Student.Name;
                row.Cells[3].Value = "" + dem.SchoolYear;
                row.Cells[4].Value = "" + dem.Semester;
                row.Cells[5].Value = dem.OccurDate.ToShortDateString();
                row.Cells[6].Value = "大功(" + dem.MeritA + ")小功(" + dem.MeritB + ")嘉獎(" + dem.MeritC + ")";
                row.Cells[7].Value = dem.Reason;
                row.Cells[8].Value = dem.RegisterDate.HasValue ? dem.RegisterDate.Value.ToShortDateString() : "";

                row.Cells[0].Tag = TeacherNote.MeritDic[dem.ID]; //TeacherSetMerit
                row.Cells[1].Tag = dem; //獎勵物件

                dataGridViewX1.Rows.Add(row);
            }
        }

        /// <summary>
        /// 取得有註記之懲戒資料
        /// </summary>
        private void GetDemeritList()
        {
            foreach (DemeritRecord dem in TeacherNote.GetTeacherNoteDemeritList())
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);
                row.Cells[0].Value = "V";
                row.Cells[1].Value = "懲戒";
                row.Cells[2].Value = dem.Student.Name;
                row.Cells[3].Value = "" + dem.SchoolYear;
                row.Cells[4].Value = "" + dem.Semester;
                row.Cells[5].Value = dem.OccurDate.ToShortDateString();
                row.Cells[6].Value = "大過(" + dem.DemeritA + ")小過(" + dem.DemeritB + ")警告(" + dem.DemeritC + ")";
                row.Cells[7].Value = dem.Reason;
                row.Cells[8].Value = dem.RegisterDate.HasValue ? dem.RegisterDate.Value.ToShortDateString() : "";

                row.Cells[0].Tag = TeacherNote.DemeritDic[dem.ID]; //TeacherSetDemerit
                row.Cells[1].Tag = dem; //懲戒物件

                dataGridViewX1.Rows.Add(row);
            }
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void 將選取學生加入學生待處理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> StudentIDList = new List<string>();
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                if (row.IsNewRow)
                    continue;

                if (row.Cells[1].Tag is MeritRecord)
                {
                    MeritRecord mr = (MeritRecord)row.Cells[1].Tag;
                    if (!StudentIDList.Contains(mr.RefStudentID))
                    {
                        StudentIDList.Add(mr.RefStudentID);
                    }
                }
                else if (row.Cells[1].Tag is DemeritRecord)
                {
                    DemeritRecord dmr = (DemeritRecord)row.Cells[1].Tag;
                    if (!StudentIDList.Contains(dmr.RefStudentID))
                    {
                        StudentIDList.Add(dmr.RefStudentID);
                    }
                }
            }
            K12.Presentation.NLDPanels.Student.AddToTemp(StudentIDList);
        }

        void Student_TempSourceChanged(object sender, EventArgs e)
        {
            labelX1.Text = "學生待處理：" + K12.Presentation.NLDPanels.Student.TempSource.Count;
        }

        private void 清空學生待處理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            K12.Presentation.NLDPanels.Student.RemoveFromTemp(K12.Presentation.NLDPanels.Student.TempSource);
        }

        private void 刪除所選註記資料ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PassWord pw = new PassWord();
            DialogResult dr = pw.ShowDialog();
            if (dr == DialogResult.Yes)
            {
                //註記物件
                List<TeacherSetMerit> list1 = new List<TeacherSetMerit>();
                List<TeacherSetDemerit> list2 = new List<TeacherSetDemerit>();
                //獎懲物件
                List<MeritRecord> list3 = new List<MeritRecord>();
                List<DemeritRecord> list4 = new List<DemeritRecord>();
                //Row
                List<DataGridViewRow> DelRow = new List<DataGridViewRow>();

                //透過選擇的Row進行註記資料刪除
                foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
                {
                    if (row.IsNewRow)
                        continue;

                    if (row.Cells[0].Tag is TeacherSetMerit)
                    {
                        list1.Add((TeacherSetMerit)row.Cells[0].Tag);
                    }
                    else if (row.Cells[0].Tag is TeacherSetDemerit)
                    {
                        list2.Add((TeacherSetDemerit)row.Cells[0].Tag);
                    }

                    if (row.Cells[1].Tag is MeritRecord)
                    {
                        list3.Add((MeritRecord)row.Cells[1].Tag);
                    }
                    else if (row.Cells[1].Tag is DemeritRecord)
                    {
                        list4.Add((DemeritRecord)row.Cells[1].Tag);
                    }

                    DelRow.Add(row);
                }

                _accessHelper.DeletedValues(list1.ToArray());
                _accessHelper.DeletedValues(list2.ToArray());

                if (checkBoxX1.Checked)
                {
                    Merit.Delete(list3);
                    Demerit.Delete(list4);
                }

                foreach (DataGridViewRow row in DelRow)
                {
                    dataGridViewX1.Rows.Remove(row);
                }
            }
            else
            {
                MsgBox.Show("輸入錯誤");
            }
        }

        private void buttonX2_Click_1(object sender, EventArgs e)
        {
            #region 匯出
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            DataGridViewExport export = new DataGridViewExport(dataGridViewX1);
            export.Save(saveFileDialog1.FileName);

            if (new CompleteForm().ShowDialog() == DialogResult.Yes)
                System.Diagnostics.Process.Start(saveFileDialog1.FileName);
            #endregion
        }
    }
}
