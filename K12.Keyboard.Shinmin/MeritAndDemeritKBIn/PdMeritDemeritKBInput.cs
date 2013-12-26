﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar.Controls;
using System.Xml;
using FISCA.Presentation.Controls;
using K12.Data;
using FISCA.DSAUtil;
using FISCA.LogAgent;
using FISCA.UDT;

namespace K12.Keyboard.Shinmin
{
    //shinmin 新民
    public partial class PdMeritDemeritKBInput : BaseForm
    {
        Dictionary<string, List<StudentRecord>> _ClassList = new Dictionary<string, List<StudentRecord>>();

        AccessHelper _accessHelper = new AccessHelper();

        List<int> PeriodDic1 = new List<int>() { 6, 7, 8 }; //獎勵Cell
        List<int> PeriodDic2 = new List<int>() { 9, 10, 11 }; //懲戒Cell
        PdPeriodDG2 _PeriodDG;
        Dictionary<string, string> MeritDemeritList = new Dictionary<string, string>(); //獎懲代碼表

        Dictionary<string, string> ClassNameDic = new Dictionary<string, string>();

        #region 以常數定義每個欄位的名稱
        private const int TeacherPonit = 0; //註記
        private const int ClassColumnIndex = 1; //班級
        private const int SeatNoColumnIndex = 2; //座號
        private const int StudentNumberColumnIndex = 3; //學號
        private const int StudentNameColumnIndex = 4; //座號
        private const int DateColumnIndex = 5; //日期
        private const int MeritA = 6;
        private const int MeritB = 7;
        private const int MeritC = 8;
        private const int DemeritA = 9;
        private const int DemeritB = 10;
        private const int DemeritC = 11;
        private const int ReasonColumnIndex = 12; //事由
        private const int SchoolYearIndex = 13; //學年度
        private const int SemesterIndex = 14; //學期
        private const int DefInputDate = 15;  //輸入日期
        #endregion

        public PdMeritDemeritKBInput()
        {
            InitializeComponent();
        }

        private void PdMeritDemeritKBInput_Load(object sender, EventArgs e)
        {
            #region Load
            ClassNameDic = DataSort.GetClassNameDic();

            btnInputDate.Text = DateTime.Now.ToString("yyyyMMdd");
            //tbDateTime.Text = DateTime.Now.ToString("yyyyMMdd");

            int RowsAdd = Pddgv.Rows.Add();

            #region 學年度學期
            cbBoxItem1SchoolYear.Text = School.DefaultSchoolYear;
            cbBoxItem1Semester.Text = School.DefaultSemester;
            #endregion

            _PeriodDG = new PdPeriodDG2(this.Pddgv, PeriodDic1, PeriodDic2); //註冊事件

            #region 初始化資料
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += delegate
            {
                GetDReasonList(); //取得獎懲代碼表
                ReflashSchoolClass(); //建立學校班級清單            
            };

            bg.RunWorkerCompleted += delegate
            {
                Enabled = true;
                this.Text = "獎懲資料鍵盤化管理";
            };
            bg.RunWorkerAsync();
            Enabled = false;
            this.Text = "初始化中...";
            #endregion

            #endregion
        }

        #region Method

        private void GetDReasonList()
        {
            #region 取得並填入獎懲代碼表

            MeritDemeritList.Clear();
            DSResponse dsrsp = Config.GetDisciplineReasonList();
            foreach (XmlElement var in dsrsp.GetContent().GetElements("Reason"))
            {

                string type = var.GetAttribute("Type");
                string code = var.GetAttribute("Code");
                string desc = var.GetAttribute("Description");

                if (!MeritDemeritList.ContainsKey(code))
                {
                    MeritDemeritList.Add(code, desc);
                    string AddItems = code + " " + "(" + type + ")" + " " + desc;
                    //?
                    cbbReasonHotKey.Invoke(new AddItem(AddHotKey), AddItems);
                }
            }
            #endregion
        }

        //?
        private delegate void AddItem(string item);
        //?
        private void AddHotKey(string item)
        {
            cbbReasonHotKey.Items.Add(item);
        }

        private void ReflashSchoolClass()
        {
            #region 建立班級資料

            _ClassList = new Dictionary<string, List<StudentRecord>>();
            _ClassList.Clear();

            List<ClassRecord> Classes = Class.SelectAll();
            List<StudentRecord> Students = Student.SelectAll();

            foreach (StudentRecord eachstudent in Students)
            {
                if (eachstudent.Status == StudentRecord.StudentStatus.畢業或離校)
                    continue;
                if (eachstudent.Status == StudentRecord.StudentStatus.刪除)
                    continue;
                if (eachstudent.Status == StudentRecord.StudentStatus.休學)
                    continue;

                if (!string.IsNullOrEmpty(eachstudent.RefClassID))
                {
                    if (!_ClassList.ContainsKey(eachstudent.Class.Name))
                        _ClassList.Add(eachstudent.Class.Name, new List<StudentRecord>());

                    _ClassList[eachstudent.Class.Name].Add(eachstudent);
                }
            }
            #endregion
        }

        private bool IsDateTime(string date)
        {
            #region 時間錯誤判斷
            if (date == "")
            {
                return false;
            }

            if (date.Length == 4)
            {
                string[] bb = DateTime.Now.ToShortDateString().Split('/');
                date = date.Insert(0, bb[0]);
            }
            else if (date.Length != 8)
            {
                return false;
            }

            date = date.Insert(4, "/");
            date = date.Insert(7, "/");

            DateTime try_value;
            if (DateTime.TryParse(date, out try_value))
            {
                return true;
            }
            return false;
            #endregion
        }

        private void TextInsertError(ErrorProvider errorProvider, TextBoxX Text, string ErrorInfo)
        {
            #region 設定Text錯誤訊息之用
            Text.SelectAll();
            errorProvider.SetError(Text, ErrorInfo);
            errorProvider.SetIconPadding(Text, -20);
            #endregion
        }

        private DateTime DateInsertSlash(string TimeString)
        {
            #region 將8碼之時間,插入"\"符號
            string InsertSlash = TimeString.Insert(4, "/");
            InsertSlash = InsertSlash.Insert(7, "/");
            return DateTimeHelper.ParseDirect(InsertSlash);
            #endregion
        }

        private void SetReadOnlyAndColor(DataGridViewRow NeRow)
        {
            #region 儲存成功鎖定本行內容
            NeRow.ReadOnly = true;
            foreach (DataGridViewCell NeCell in NeRow.Cells)
            {
                NeCell.Style.BackColor = Color.LightSkyBlue;
            }
            #endregion
        }

        private bool CheckRow(DataGridViewRow _row)
        {
            #region 檢查ROW資料是否正確
            Dictionary<DataGridViewCell, bool> dic = new Dictionary<DataGridViewCell, bool>();


            if (_row.Tag is StudentRecord) //本CELL已找查出學生
            {
                foreach (DataGridViewCell each in _row.Cells) //檢查每一個CELL
                {
                    //略過獎懲範圍(5,6,7,8,9,10)
                    if (!PeriodDic1.Contains(each.ColumnIndex) &&
                        !PeriodDic2.Contains(each.ColumnIndex) &&
                        each.ColumnIndex != TeacherPonit &&
                        each.ColumnIndex != StudentNumberColumnIndex)
                    {
                        if ("" + each.Value == string.Empty) //是否為空值
                        {
                            dic.Add(each, false);
                        }
                    }
                }
            }
            else
            {
                return true; //true為資料錯誤
            }
            return dic.ContainsValue(false);
            #endregion
        }

        private int AddNewRowSetColor()
        {
            #region 新增一行,並且預設顏色
            int addNewRowIndex = Pddgv.Rows.Add();

            foreach (DataGridViewCell cell in Pddgv.Rows[addNewRowIndex].Cells)
            {
                if (PeriodDic1.Contains(cell.OwningColumn.Index) || PeriodDic2.Contains(cell.OwningColumn.Index))
                {
                    cell.Style.BackColor = Color.White;
                }
            }
            return addNewRowIndex;
            #endregion
        }

        private void InsertRow()
        {
            #region 插入新Rows
            int InsertRow = AddNewRowSetColor();

            if (IsDateTime(tbDateTime.Text) || tbDateTime.Text == string.Empty) //如果時間欄位有值,並且是正確的
            {
                Pddgv.Rows[InsertRow].Cells[DateColumnIndex].Value = tbDateTime.Text;
                Pddgv.Rows[InsertRow].Cells[SchoolYearIndex].Value = cbBoxItem1SchoolYear.Text;
                Pddgv.Rows[InsertRow].Cells[SemesterIndex].Value = cbBoxItem1Semester.Text;
                Pddgv.Rows[InsertRow].Cells[ReasonColumnIndex].Value = tbReason.Text;
                Pddgv.Rows[InsertRow].Cells[MeritA].Value = textBoxX1.Text;
                Pddgv.Rows[InsertRow].Cells[MeritB].Value = textBoxX2.Text;
                Pddgv.Rows[InsertRow].Cells[MeritC].Value = textBoxX3.Text;
                Pddgv.Rows[InsertRow].Cells[DemeritA].Value = textBoxX4.Text;
                Pddgv.Rows[InsertRow].Cells[DemeritB].Value = textBoxX5.Text;
                Pddgv.Rows[InsertRow].Cells[DemeritC].Value = textBoxX6.Text;
                if (IsDateTime(tbDateTime.Text) || tbDateTime.Text == string.Empty)
                {
                    Pddgv.Rows[InsertRow].Cells[DefInputDate].Value = btnInputDate.Text;
                }
                else
                {
                    FISCA.Presentation.Controls.MsgBox.Show("登錄日期內容輸入錯誤");
                    return;
                }
            }
            else
            {
                FISCA.Presentation.Controls.MsgBox.Show("獎懲日期內容輸入錯誤");
                return;
            }

            Pddgv.Rows[InsertRow].Cells[0].Selected = true;
            #endregion
        }

        #endregion

        #region 畫面按紐

        private void btnCancel_Click(object sender, EventArgs e)
        {
            #region 關閉
            this.Close();
            #endregion
        }

        #endregion

        #region 批次輸入

        private void cbBoxItem1SchoolYear_KeyUp(object sender, KeyEventArgs e)
        {
            #region 學年度
            if (e.KeyCode == Keys.Enter)
            {
                cbBoxItem1Semester.Focus();
            }
            #endregion
        }

        private void cbBoxItem1Semester_KeyUp(object sender, KeyEventArgs e)
        {
            #region 學期
            if (e.KeyCode == Keys.Enter)
            {
                tbDateTime.Focus();
            }
            #endregion
        }

        private void tbDateTime_KeyUp(object sender, KeyEventArgs e)
        {
            #region 日期
            if (e.KeyCode == Keys.Enter)
            {
                if (IsDateTime(tbDateTime.Text))
                {
                    if (tbDateTime.Text.Length == 4)
                    {
                        string[] bb = DateTime.Now.ToShortDateString().Split('/');
                        tbDateTime.Text = tbDateTime.Text.Insert(0, bb[0]);
                    }
                    errorProvider1.Clear();
                    cbbReasonHotKey.Focus();
                }
                else if (tbDateTime.Text == string.Empty)
                {
                    errorProvider1.Clear();
                    cbbReasonHotKey.Focus();
                }
                else
                {
                    TextInsertError(errorProvider1, tbDateTime, "請輸入正確 日期格式");
                }
            }
            #endregion
        }

        private void cbbReasonHotKey_KeyUp_1(object sender, KeyEventArgs e)
        {
            #region 事由代碼
            if (e.KeyCode == Keys.Enter)
            {
                if (cbbReasonHotKey.Text != string.Empty)
                {
                    string[] _Reason = cbbReasonHotKey.Text.Split(' ');
                    if (_Reason.Length == 3)
                    {
                        tbReason.Text = "" + _Reason.GetValue(2);
                    }
                    else
                    {
                        cbbReasonHotKey.Text = "";
                        tbReason.Text = "";
                    }
                }
                tbReason.Focus();
            }
            #endregion
        }

        private void tbReason_KeyUp(object sender, KeyEventArgs e)
        {
            #region 事由
            if (e.KeyCode == Keys.Enter)
            {
                btnItem1Insert_Click(null, null);
                Pddgv.Focus();
            }
            #endregion
        }

        private void btnItem1Insert_Click(object sender, EventArgs e)
        {
            #region 新增
            int InsertRow;

            //資料錯誤檢查
            if (!CheckSchoolYearSemeset())
            {
                MsgBox.Show("學年度/學期 資料錯誤");
                return;
            }

            if (Pddgv.Rows.Count <= 0)
            {
                //如果沒有新行
                InsertRow = Pddgv.Rows.Add();
            }
            else
            {
                //如果新行內容有值,就再Add新Row
                InsertRow = Pddgv.Rows.Count - 1;
                foreach (DataGridViewCell each in Pddgv.Rows[InsertRow].Cells)
                {
                    if ("" + each.Value != string.Empty)
                    {
                        InsertRow = Pddgv.Rows.Add();
                        break;
                    }
                }
            }

            Pddgv.Rows[InsertRow].Cells[SchoolYearIndex].Value = cbBoxItem1SchoolYear.Text;
            Pddgv.Rows[InsertRow].Cells[SemesterIndex].Value = cbBoxItem1Semester.Text;
            Pddgv.Rows[InsertRow].Cells[DateColumnIndex].Value = tbDateTime.Text;
            Pddgv.Rows[InsertRow].Cells[ReasonColumnIndex].Value = tbReason.Text;
            Pddgv.Rows[InsertRow].Cells[DefInputDate].Value = btnInputDate.Text;
            Pddgv.Rows[InsertRow].Cells[MeritA].Value = textBoxX1.Text;
            Pddgv.Rows[InsertRow].Cells[MeritB].Value = textBoxX2.Text;
            Pddgv.Rows[InsertRow].Cells[MeritC].Value = textBoxX3.Text;
            Pddgv.Rows[InsertRow].Cells[DemeritA].Value = textBoxX4.Text;
            Pddgv.Rows[InsertRow].Cells[DemeritB].Value = textBoxX5.Text;
            Pddgv.Rows[InsertRow].Cells[DemeritC].Value = textBoxX6.Text;

            Pddgv.Rows[InsertRow].Cells[0].Selected = true;
            #endregion
        }

        private bool CheckSchoolYearSemeset()
        {
            #region 資料錯誤檢查
            int check;
            if (!int.TryParse(cbBoxItem1SchoolYear.Text, out check))
            {
                return false;
            }

            if (cbBoxItem1Semester.Text == "1" || cbBoxItem1Semester.Text == "2")
            {

            }
            else
            {
                return false;
            }

            return true;
            #endregion
        }

        #endregion

        private void Pddgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            #region CellEndEdit

            PdCellHelper cell = new PdCellHelper(Pddgv.CurrentCell);
            cell.SetError(""); //重置錯誤訊息
            if (cell.GetCellIndex() == TeacherPonit)
            {
                if (cell.GetValue() != string.Empty)
                {
                    cell.SetValue("V");
                }
                else
                {

                }
            }
            else if (cell.GetCellIndex() == ClassColumnIndex)
            {
                #region 班級
                cell.SetupNumCell("234"); //重設座號,學號,姓名
                cell.SetRowTag(null);

                string cellGetValue = "";
                if (ClassNameDic.ContainsKey(cell.GetValue())) //取得班級名稱代碼
                {
                    cell.SetValue(cellGetValue = ClassNameDic[cell.GetValue()]);
                }
                else
                {
                    cellGetValue = cell.GetValue();
                }

                if (_ClassList.ContainsKey(cellGetValue)) //是否有此班級
                {
                    cell.SetRowTag(_ClassList[cellGetValue]);
                    cell.SetError("");
                }
                else
                {
                    cell.SetError("系統內查無此班級");
                }

                #endregion
            }
            else if (cell.GetCellIndex() == SeatNoColumnIndex)
            {
                #region 座號
                cell.SetupNumCell("34"); //重設
                cell.SetRowTag(null);

                if (_ClassList.ContainsKey(cell.GetNumCellValue(ClassColumnIndex))) //是否已填入班級
                {
                    foreach (StudentRecord stud in _ClassList[cell.GetNumCellValue(ClassColumnIndex)]) //直接依班級查詢
                    {
                        if (stud.SeatNo.ToString() == cell.GetValue()) //如果座號相同
                        {
                            cell.SetRowTag(stud);                        //記住學生
                            cell.SetNumCellValue(StudentNumberColumnIndex, stud.StudentNumber); //填入學號
                            cell.SetNumCellValue(StudentNameColumnIndex, stud.Name);          //填入姓名
                            break;
                        }
                    }

                    if (!(cell.GetRowTag() is StudentRecord))
                    {
                        cell.SetError("查無此學生");
                        return;
                    }

                }
                else
                {
                    cell.SetError("您必須先輸入班級");
                    cell.SetNumError(ClassColumnIndex, "您必須先輸入班級");
                }
                #endregion
            }
            else if (cell.GetCellIndex() == DateColumnIndex)
            {
                #region 日期
                string Date = cell.GetValue();

                if (IsDateTime(Date)) //是否日期格式
                {
                    if (Date.Length == 4)
                    {
                        string[] bb = DateTime.Now.ToShortDateString().Split('/');
                        Date = Date.Insert(0, bb[0]);
                        cell.SetValue(Date);
                    }

                    cell.SetNumCellValue(SchoolYearIndex, cbBoxItem1SchoolYear.Text);
                    cell.SetNumCellValue(SemesterIndex, cbBoxItem1Semester.Text);
                    cell.SetNumCellValue(DefInputDate, btnInputDate.Text);
                }
                else
                {
                    cell.SetNumError(DateColumnIndex, "日期格式錯誤");
                }
                #endregion
            }
            else if (cell.GetCellIndex() == SchoolYearIndex)
            {
                //if (RowNowSave)
                //{
                //    if (CheckRow(Pddgv.CurrentRow))
                //    {
                //        MsgBox.Show("請確認學生資料是否輸入完整");
                //        return;
                //    }
                //    else
                //    {
                //        _RowSave(cell.GetRow());
                //        RowNowSave = false;
                //    }
                //}
            }
            #endregion
        }

        private void Pddgv_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            #region 當事由編輯過,進行儲存處理(Pddgv_CellEnter會發生於SchoolYearIndex欄位)
            if (Pddgv.CurrentCell.ColumnIndex == ReasonColumnIndex)
            {
                Pddgv.EndEdit();
                if (MeritDemeritList.ContainsKey("" + Pddgv.CurrentCell.Value))
                {
                    Pddgv.CurrentCell.Value = MeritDemeritList["" + Pddgv.CurrentCell.Value];
                }
                Pddgv.BeginEdit(false);
            }
            //else if (Pddgv.CurrentCell.ColumnIndex == TeacherPonit)
            //{
            //    if (Pddgv.CurrentCell.Value != null)
            //    {
            //        Pddgv.EndEdit();
            //    }
            //}
            #endregion
        }

        private void Pddgv_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            #region 如果事由編輯過(學年度收到焦點),呼叫儲存_RowSave方法

            //if (e.ColumnIndex == SchoolYearIndex)
            //{
            //    if (!RowNowSave)
            //    {
            //        RowNowSave = true; //Pddgv_CellEnter會發生於SchoolYearIndex欄位
            //    }
            //}
            #endregion
        }

        private bool _RowSave(DataGridViewRow _row)
        {
            #region 儲存

            string CellMerit = "" + _row.Cells[MeritA].Value;
            CellMerit += "" + _row.Cells[MeritB].Value;
            CellMerit += "" + _row.Cells[MeritC].Value;

            string CellDemerit = "" + _row.Cells[DemeritA].Value;
            CellDemerit += "" + _row.Cells[DemeritB].Value;
            CellDemerit += "" + _row.Cells[DemeritC].Value;

            if (!string.IsNullOrEmpty(CellMerit) && !string.IsNullOrEmpty(CellDemerit))
            {
                MsgBox.Show("獎懲隻數資料不可同時輸入\n系統無法判斷本資料狀態!!\n請修正資料後重新進行儲存動作");
                return false;
            }

            if (CellMerit != string.Empty) //當獎懲欄位不是空的
            {
                #region 獎勵儲存
                MeritRecord InsertMerit = new MeritRecord();
                StudentRecord SR = (StudentRecord)_row.Tag;
                InsertMerit.RefStudentID = SR.ID; //學生ID

                #region 獎勵
                if ("" + _row.Cells[MeritA].Value == string.Empty)
                {
                    _row.Cells[MeritA].Value = 0;
                }
                InsertMerit.MeritA = Int.ParseAllowNull("" + _row.Cells[MeritA].Value); //大功

                if ("" + _row.Cells[MeritB].Value == string.Empty)
                {
                    _row.Cells[MeritB].Value = 0;
                }
                InsertMerit.MeritB = Int.ParseAllowNull("" + _row.Cells[MeritB].Value); //小功

                if ("" + _row.Cells[MeritC].Value == string.Empty)
                {
                    _row.Cells[MeritC].Value = 0;
                }
                InsertMerit.MeritC = Int.ParseAllowNull("" + _row.Cells[MeritC].Value); //嘉獎 
                #endregion

                //InsertMerit.MeritFlag = "1"; //MeritFlag=0 銷過,  MeritFlag=1 記功,   MeritFlag=2 記過
                InsertMerit.OccurDate = DateInsertSlash("" + _row.Cells[DateColumnIndex].Value); //日期
                InsertMerit.Reason = "" + _row.Cells[ReasonColumnIndex].Value; //事由
                InsertMerit.SchoolYear = int.Parse("" + _row.Cells[SchoolYearIndex].Value); //學年度
                InsertMerit.Semester = int.Parse("" + _row.Cells[SemesterIndex].Value); //學期
                InsertMerit.RegisterDate = DateInsertSlash("" + _row.Cells[DefInputDate].Value);
                
                try
                {
                    string MeritID = Merit.Insert(InsertMerit);
                    if ("" + _row.Cells[TeacherPonit].Value == "V")
                    {
                        TeacherSetMerit TSM = new TeacherSetMerit();
                        TSM.MeritID = MeritID;
                        TSM.IsTeacherNote = true;
                        TSM.StudentID = InsertMerit.RefStudentID;
                        TSM.SchoolYear = InsertMerit.SchoolYear;
                        TSM.Semester = InsertMerit.Semester;
                        TSM.Save();
                    }
                    SetReadOnlyAndColor(_row);

                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("儲存獎勵資料,發生錯誤");
                    return false;
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("詳細資料：");
                sb.AppendLine("學生「" + SR.Name + "」");
                sb.AppendLine("日期「" + InsertMerit.OccurDate.ToShortDateString() + "」");
                sb.AppendLine("獎勵內容「大功：" + InsertMerit.MeritA + " 小功：" + InsertMerit.MeritB + " 嘉獎：" + InsertMerit.MeritC + "」");
                sb.AppendLine("獎勵事由「" + InsertMerit.Reason + "」");
                ApplicationLog.Log("學務系統.獎懲鍵盤登錄", "新增獎勵資料", "student", SR.ID, "由「獎懲鍵盤登錄」功能，新增一筆獎勵資料。\n" + sb.ToString());

                #endregion
            }
            else if (CellDemerit != string.Empty) //當懲戒欄位不是空的
            {
                #region 懲戒儲存
                DemeritRecord InsertDemerit = new DemeritRecord();
                StudentRecord SR = (StudentRecord)_row.Tag;
                InsertDemerit.RefStudentID = SR.ID;

                #region 懲戒
                if ("" + _row.Cells[DemeritA].Value == string.Empty)
                {
                    _row.Cells[DemeritA].Value = 0;
                }
                InsertDemerit.DemeritA = Int.ParseAllowNull("" + _row.Cells[DemeritA].Value);

                if ("" + _row.Cells[DemeritB].Value == string.Empty)
                {
                    _row.Cells[DemeritB].Value = 0;
                }
                InsertDemerit.DemeritB = Int.ParseAllowNull("" + _row.Cells[DemeritB].Value);

                if ("" + _row.Cells[DemeritC].Value == string.Empty)
                {
                    _row.Cells[DemeritC].Value = 0;
                }
                InsertDemerit.DemeritC = Int.ParseAllowNull("" + _row.Cells[DemeritC].Value);
                #endregion

                //InsertDemerit.MeritFlag = "0";
                InsertDemerit.OccurDate = DateInsertSlash("" + _row.Cells[DateColumnIndex].Value);
                InsertDemerit.Reason = "" + _row.Cells[ReasonColumnIndex].Value;
                InsertDemerit.SchoolYear = int.Parse("" + _row.Cells[SchoolYearIndex].Value);
                InsertDemerit.Semester = int.Parse("" + _row.Cells[SemesterIndex].Value);
                InsertDemerit.RegisterDate = DateInsertSlash("" + _row.Cells[DefInputDate].Value);
                try
                {
                    string DemeritID = Demerit.Insert(InsertDemerit);
                    if ("" + _row.Cells[TeacherPonit].Value == "V")
                    {
                        TeacherSetDemerit TSD = new TeacherSetDemerit();
                        TSD.DemeritID = DemeritID;
                        TSD.IsTeacherNote = true;
                        TSD.StudentID = InsertDemerit.RefStudentID;
                        TSD.SchoolYear = InsertDemerit.SchoolYear;
                        TSD.Semester = InsertDemerit.Semester;
                        TSD.Save();
                    }
                    SetReadOnlyAndColor(_row);
                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("儲存懲戒資料,發生錯誤");
                    return false;
                }
                #endregion
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("詳細資料：");
                sb.AppendLine("學生「" + SR.Name + "」");
                sb.AppendLine("日期「" + InsertDemerit.OccurDate.ToShortDateString() + "」");
                sb.AppendLine("懲戒內容「大過：" + InsertDemerit.DemeritA + " 小過：" + InsertDemerit.DemeritB + " 警告：" + InsertDemerit.DemeritC + "」");
                sb.AppendLine("懲戒事由「" + InsertDemerit.Reason + "」");
                ApplicationLog.Log("學務系統.獎懲鍵盤化輸入", "新增懲戒資料", "student", SR.ID, "由「獎懲鍵盤登錄」功能，新增一筆懲戒資料。\n" + sb.ToString());
            }
            else
            {
                FISCA.Presentation.Controls.MsgBox.Show("獎懲欄位皆無內容,請檢查資料是否正確");
                return false;
            }

            return true;

            #endregion
        }

        private void Pddgv_KeyUp(object sender, KeyEventArgs e)
        {
            if (Pddgv.Rows.Count <= 0) //必須有Rows
                return;

            if (Pddgv.CurrentRow.ReadOnly == true) //必須選擇Row,不可為已儲存資料
                return;

            Keys key = (e.KeyData & Keys.KeyCode);

            #region 如果是按Alt+S
            if (key == Keys.S && e.Alt) //Alt+Enter
            {
                if (!CheckRow(Pddgv.CurrentRow))
                {
                    if (_RowSave(Pddgv.CurrentRow))
                    {
                        InsertRow();
                        Pddgv.CurrentCell = Pddgv.Rows[Pddgv.Rows.Count - 1].Cells[0];
                    }
                    return;
                }
            }
            #endregion

            #region 如果是按下Enter
            if (key == Keys.Enter)
            {
                //登錄日期欄位
                if (Pddgv.CurrentCell.ColumnIndex == DefInputDate)
                {
                    //是否停在最後一欄
                    if (Pddgv.CurrentRow.Index == Pddgv.Rows.Count - 1)
                    {
                        //Row資料是否正確
                        if (!CheckRow(Pddgv.CurrentRow))
                        {
                            //資料是否儲存成功
                            if (_RowSave(Pddgv.CurrentRow))
                            {
                                InsertRow();
                                Pddgv.CurrentCell = Pddgv.Rows[Pddgv.Rows.Count - 1].Cells[0];
                                return;
                            }
                        }
                    }
                }
            }
            #endregion
        }

        private void btnSetClassNameCode_Click(object sender, EventArgs e)
        {
            SetClassCode cc = new SetClassCode();
            cc.ShowDialog();

            ClassNameDic = DataSort.GetClassNameDic();
        }

        private void btnInputDate_KeyUp(object sender, KeyEventArgs e)
        {
            #region 日期
            if (e.KeyCode == Keys.Enter)
            {
                if (IsDateTime(btnInputDate.Text))
                {
                    if (btnInputDate.Text.Length == 4)
                    {
                        string[] bb = DateTime.Now.ToShortDateString().Split('/');
                        btnInputDate.Text = btnInputDate.Text.Insert(0, bb[0]);
                    }
                    errorProvider2.Clear();
                }
                else if (btnInputDate.Text == string.Empty)
                {
                    errorProvider2.Clear();
                }
                else
                {
                    TextInsertError(errorProvider2, btnInputDate, "請輸入正確 日期格式");
                }
            }
            #endregion
        }
    }
}
