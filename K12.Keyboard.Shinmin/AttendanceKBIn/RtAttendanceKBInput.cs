using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using DevComponents.DotNetBar.Controls;
using FISCA.Presentation.Controls;
using K12.Data;
using FISCA.LogAgent;

namespace K12.Keyboard.Shinmin
{
    public partial class RtAttendanceKBInput : BaseForm
    {
        Dictionary<string, List<StudentRecord>> _ClassList = new Dictionary<string, List<StudentRecord>>();
        Dictionary<string, int> PeriodDic = new Dictionary<string, int>();
        AbsenceHelper _AbsenceHelper = new AbsenceHelper();
        PeriodHelper _PeriodHelper = new PeriodHelper();

        //List<string> SchoolYearList = new List<string>();

        Dictionary<string, char> AltKey = new Dictionary<string, char>();

        Dictionary<string, string> ClassNameDic = new Dictionary<string, string>();

        private PeriodDG _PeriodDG;

        #region 以常數定義每個欄位的名稱
        private const int ClassColumnIndex = 0;
        private const int SeatNoColumnIndex = 1;
        private const int StudentNumberColumnIndex = 2;
        private const int StudentNameColumnIndex = 3;
        private const int DateColumnIndex = 4;
        int SchoolYearIndex;
        int SemesterIndex;
        #endregion

        public RtAttendanceKBInput()
        {
            InitializeComponent();
        }

        private void RtAttendanceKBInput_Load(object sender, EventArgs e)
        {
            #region Load

            int RowsAdd = AddNewRowSetColor();

            ClassNameDic = DataSort.GetClassNameDic();

            #region 學年度學期
            cbBoxItem1SchoolYear.Text = School.DefaultSchoolYear;
            cbBoxItem1Semester.Text = School.DefaultSemester;
            #endregion

            //dgv.Columns.Add

            ReflashAbsence(); //假別熱鍵

            ReflashPeriod(); //動態填入節次

            _PeriodDG = new PeriodDG(this.dgv, PeriodDic, _AbsenceHelper.GetListHotKeyAbbreviation());

            #region 初始化資料
            BackgroundWorker bg = new BackgroundWorker();
            bg.DoWork += delegate
            {
                ReflashSchoolClass(); //建立學校班級清單         
            };

            bg.RunWorkerCompleted += delegate
            {
                Enabled = true;
                this.Text = "缺曠資料鍵盤化管理";
            };
            bg.RunWorkerAsync();
            Enabled = false;
            this.Text = "初始化中...";
            #endregion

            #endregion
        }

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
                txtItem1DateTime.Focus();
            }
            #endregion
        }

        private void txtItem1DateTime_KeyUp(object sender, KeyEventArgs e)
        {
            #region 日期
            if (e.KeyCode == Keys.Enter)
            {
                if (IsDateTime(txtItem1DateTime.Text))
                {
                    if (txtItem1DateTime.Text.Length == 4)
                    {
                        string[] bb = DateTime.Now.ToShortDateString().Split('/');
                        txtItem1DateTime.Text = txtItem1DateTime.Text.Insert(0, bb[0]);
                    }
                    errorProvider1.Clear();
                    btnItem1Insert_Click(null, null);
                    dgv.Focus();
                }
                else if (txtItem1DateTime.Text == string.Empty)
                {
                    errorProvider1.Clear();
                    btnItem1Insert_Click(null, null);
                    dgv.Focus();
                }
                else
                {
                    TextInsertError(txtItem1DateTime, "請輸入正確 日期格式");
                }
            }
            #endregion
        }

        private void btnItem1Insert_Click(object sender, EventArgs e)
        {
            #region 新增
            int InsertRow;

            if (!CheckSchoolYearSemeset())
            {
                MsgBox.Show("學年度/學期 資料錯誤");
                return;
            }

            if (dgv.Rows.Count <= 0)
            {
                //如果沒有新行
                InsertRow = AddNewRowSetColor();
            }
            else
            {
                //如果新行內容有值,就再Add新Row
                InsertRow = dgv.Rows.Count - 1;
                foreach (DataGridViewCell each in dgv.Rows[InsertRow].Cells)
                {
                    if ("" + each.Value != string.Empty)
                    {
                        InsertRow = AddNewRowSetColor();
                        break;
                    }
                }
            }

            dgv.Rows[InsertRow].Cells[SchoolYearIndex].Value = cbBoxItem1SchoolYear.Text;
            dgv.Rows[InsertRow].Cells[SemesterIndex].Value = cbBoxItem1Semester.Text;
            dgv.Rows[InsertRow].Cells[DateColumnIndex].Value = txtItem1DateTime.Text;

            //if (_AbsenceHelper.AbbreviationExists(txtItem1Absence.Text))
            //{
            //    foreach (int each in PeriodDic.Values)
            //    {
            //        dgv.Rows[InsertRow].Cells[each].Value = txtItem1Absence.Text;
            //    }
            //}

            dgv.Rows[InsertRow].Cells[0].Selected = true; 
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

        private void InsertRow()
        {
            #region 插入新Rows
            int InsertRow = AddNewRowSetColor();

            if (IsDateTime(txtItem1DateTime.Text)) //如果時間欄位有值
            {
                dgv.Rows[InsertRow].Cells[DateColumnIndex].Value = txtItem1DateTime.Text;
            }

            dgv.Rows[InsertRow].Cells[0].Selected = true; 
            #endregion
        }

        private void dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            #region CellEndEdit
            CellHelper cell = new CellHelper(dgv.CurrentCell);
            cell.SetError(""); //重置錯誤訊息

            if (cell.GetCellIndex() == ClassColumnIndex)
            {
                #region 班級
                cell.SetupNumCell("123"); //重設座號,學號,姓名
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
                cell.SetupNumCell("23"); //重設
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
                            //cell.SetCellStyle("23", Color.Blue, false); //設定填入值的顏色
                            break;
                        }
                    }

                    if (!(cell.GetRowTag() is StudentRecord))
                    {
                        cell.SetError("查無此學生");
                        return;
                    }

                    if (cell.GetNumCellValue(DateColumnIndex) != string.Empty) //如果日期是有值
                    {
                        string Date = cell.GetNumCellValue(DateColumnIndex); //將日期欄位的值取出

                        if (!IsDateTime(Date)) //是否日期格式
                        {
                            cell.SetNumError(DateColumnIndex, "日期格式錯誤");
                            return;
                        }

                        StudentRecord SR = (StudentRecord)cell.GetRowTag(); //取得ROW的TAG

                        foreach (AttendanceRecord each in Attendance.SelectByStudents(new List<StudentRecord> { SR })) //取得該生的缺曠資料
                        {
                            if (DateTime.Parse(each.OccurDate.ToString()) == DateTime.Parse(DateInsertSlash(Date))) //如果日期相同
                            {
                                cell.SetRowTag(each); //記住缺曠

                                foreach (AttendancePeriod AttPerLv1 in each.PeriodDetail) //填入該日期缺曠資料詳細內容
                                {
                                    if (PeriodDic.ContainsKey(AttPerLv1.Period))
                                    {
                                        cell.SetNumCellValue(PeriodDic[AttPerLv1.Period], _AbsenceHelper.GetAbbreviationByName(AttPerLv1.AbsenceType));
                                    }
                                }
                            }
                        }
                        cell.SetNumCellValue(SchoolYearIndex, cbBoxItem1SchoolYear.Text);
                        cell.SetNumCellValue(SemesterIndex, cbBoxItem1Semester.Text);
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

                StudentRecord SR = null;

                if (cell.GetRowTag()  == null) //沒有內容
                {
                    cell.SetNumError(0, "請輸入正確班級");
                    cell.SetNumError(1, "請輸入正確座號");
                    return;
                }
                else if (cell.GetRowTag() is StudentRecord)
                {
                    SR = (StudentRecord)cell.GetRowTag(); //取得ROW的TAG
                }
                else if (cell.GetRowTag() is AttendanceRecord) //表示前筆有找到資料
                {
                    AttendanceRecord AR = (AttendanceRecord)cell.GetRowTag();
                    SR = Student.SelectByID(AR.RefStudentID); //透過AttendanceRecord取得學生Record
                    cell.SetRowTag(SR);
                }

                foreach (int Num in PeriodDic.Values) //清空缺曠欄位資料
                {
                    cell.ClearIntNumCellValue(Num);
                }

                string Date = cell.GetValue();

                if (IsDateTime(Date)) //是否日期格式
                {
                    if (Date.Length == 4)
                    {
                        string[] bb = DateTime.Now.ToShortDateString().Split('/');
                        Date = Date.Insert(0, bb[0]);
                        cell.SetValue(Date);
                    }

                    foreach (AttendanceRecord each in Attendance.SelectByStudents(new List<StudentRecord> { SR })) //取得該生的缺曠資料
                    {
                        if (DateTime.Parse(each.OccurDate.ToString()) == DateTime.Parse(DateInsertSlash(Date))) //如果日期相同
                        {
                            cell.SetRowTag(each); //記住此缺曠記錄

                            foreach (AttendancePeriod AttPerLv1 in each.PeriodDetail) //填入該日期缺曠資料詳細內容
                            {
                                if (PeriodDic.ContainsKey(AttPerLv1.Period))
                                {
                                    cell.SetNumCellValue(PeriodDic[AttPerLv1.Period], _AbsenceHelper.GetAbbreviationByName(AttPerLv1.AbsenceType));
                                }
                            }
                        }
                    }
                    #region 如果重新輸入日期,而沒有此筆缺曠記錄,則重置Row

                    //if (CheckAttendanceDate)
                    //{
                    //    if ((cell.GetRowTag() is AttendanceRecord)) //是缺曠記錄
                    //    {
                    //        AttendanceRecord xAtt = (AttendanceRecord)cell.GetRowTag();
                    //        StudentRecord xSR = Student.GetByID(xAtt.RefStudentID);
                    //        cell.SetRowTag(xSR);
                    //        foreach (int eachInt in PeriodDic.Values)
                    //        {
                    //            cell.SetNumCellValue(eachInt, "");
                    //        }
                    //    }
                    //} 
                    #endregion

                    cell.SetNumCellValue(SchoolYearIndex, cbBoxItem1SchoolYear.Text);
                    cell.SetNumCellValue(SemesterIndex, cbBoxItem1Semester.Text);
                }
                else
                {
                    cell.SetNumError(DateColumnIndex, "日期格式錯誤");
                }
                #endregion
            }
            //else if (cell.GetCellIndex() == SchoolYearIndex - 1)
            //{
            //    _RowSave(cell.GetRow());
            //}

            #endregion
        }

        private void _RowSave(DataGridViewRow _row)
        {
            #region 儲存
            //錯誤就離開
            if (_row.Tag == null)
                return;

            if (CheckRowError(_row))
            {
                MsgBox.Show("資料內容有誤,請檢查內容");
                return;
            }

            #region 將Row指定範圍取代為縮寫
            foreach (DataGridViewCell _cell in _row.Cells)
            {
                if (PeriodDic.ContainsValue(_cell.OwningColumn.Index))
                {
                    if (_AbsenceHelper.HotKeyExists("" + _cell.Value)) //如果是熱鍵內容
                    {
                        _cell.Value = _AbsenceHelper.GetAbbreviationByHotKey("" + _cell.Value); //將資料取代為縮寫
                    }
                    else if (_AbsenceHelper.AbbreviationExists("" + _cell.Value))
                    {
                        //維持原縮寫內容
                    }
                    else //其他狀況一率設定為空字串
                    {
                        _cell.Value = string.Empty;
                    }
                }
            }
            #endregion

            if (_row.Tag is StudentRecord)
            {
                #region 新增
                StudentRecord SR = (StudentRecord)_row.Tag;

                AttendanceRecord AR = new AttendanceRecord();

                AR.RefStudentID = SR.ID; //取得ID
                AR.SchoolYear = int.Parse("" + _row.Cells[SchoolYearIndex].Value); //學年度
                AR.Semester = int.Parse("" + _row.Cells[SemesterIndex].Value); //學期

                if (IsDateTime("" + _row.Cells[DateColumnIndex].Value))
                {
                    AR.OccurDate = DateTime.Parse(DateInsertSlash("" + _row.Cells[DateColumnIndex].Value));
                }
                else
                {
                    MsgBox.Show("日期格式有誤,儲存失敗");
                    return;
                }

                foreach (DataGridViewCell _cell in _row.Cells)
                {
                    if (PeriodDic.ContainsValue(_cell.OwningColumn.Index))
                    {
                        string stringValue = "" + _cell.Value;
                        if (stringValue.Trim() != "")
                        {
                            AttendancePeriod period = new AttendancePeriod();
                            period.AbsenceType = _AbsenceHelper.GetNameByAbbreviation("" + _cell.Value);
                            period.Period = _cell.OwningColumn.HeaderText;

                            AR.PeriodDetail.Add(period);
                        }
                    }
                }

                if (AR.PeriodDetail.Count != 0)
                {
                    try
                    {
                        Attendance.Insert(AR);
                        SetReadOnlyAndColor(_row);
                    }
                    catch
                    {
                        MsgBox.Show("新增缺曠記錄,發生錯誤");
                        return;
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("詳細資料：");
                    sb.AppendLine("學生「" + SR.Name + "」");
                    sb.AppendLine("日期「" + AR.OccurDate.ToShortDateString() + "」");
                    ApplicationLog.Log("學務系統.缺曠鍵盤登錄", "新增缺曠資料", "student", SR.ID, "由「缺曠鍵盤登錄」功能，新增一筆缺曠資料。\n" + sb.ToString());
                }
                else
                {
                    //SetReadOnlyAndColor(_row);
                    MsgBox.Show("未輸入資料!新增資料失敗!");
                    return;
                }
                #endregion

            }
            else if (_row.Tag is AttendanceRecord)
            {
                #region 更新
                AttendanceRecord Att = (AttendanceRecord)_row.Tag;
                Att.PeriodDetail.Clear();

                //AttendancePeriod
                //Period    <節次>
                //AbsenceType   <假別>

                //將節次欄位組成PeriodDetail內容
                foreach (DataGridViewCell _cell in _row.Cells)
                {
                    if (PeriodDic.ContainsValue(_cell.OwningColumn.Index))
                    {

                        string stringValue = "" + _cell.Value;

                        if (stringValue.Trim() != "")
                        {

                            AttendancePeriod period = new AttendancePeriod();
                            period.AbsenceType = _AbsenceHelper.GetNameByAbbreviation("" + _cell.Value);
                            period.Period = _cell.OwningColumn.HeaderText;

                            Att.PeriodDetail.Add(period);
                        }
                    }
                }

                //如果是空的,就刪除
                if (Att.PeriodDetail.Count == 0)
                {
                    try
                    {
                        Attendance.Delete(Att.ID);
                        SetReadOnlyAndColor(_row);
                    }
                    catch
                    {
                        MsgBox.Show("刪除缺曠時,發生錯誤");
                        return;
                    }

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("詳細資料：");
                    sb.AppendLine("學生「" + Att.Student.Name + "」");
                    sb.AppendLine("日期「" + Att.OccurDate.ToShortDateString() + "」");
                    ApplicationLog.Log("學務系統.缺曠鍵盤登錄", "刪除缺曠資料", "student", Att.Student.ID, "由「缺曠鍵盤登錄」功能，刪除一筆缺曠資料。\n" + sb.ToString());
                }
                else
                {
                    try
                    {
                        Attendance.Update(Att);
                        SetReadOnlyAndColor(_row);
                    }
                    catch
                    {
                        MsgBox.Show("更新缺曠記錄,發生錯誤");
                        return;
                    }
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("詳細資料：");
                    sb.AppendLine("學生「" + Att.Student.Name + "」");
                    sb.AppendLine("日期「" + Att.OccurDate.ToShortDateString() + "」");
                    ApplicationLog.Log("學務系統.缺曠鍵盤登錄", "修改缺曠資料", "student", Att.Student.ID, "由「缺曠鍵盤登錄」功能，修改一筆缺曠資料。\n" + sb.ToString());
                }
                #endregion
            }
            else if (_row.Tag is List<StudentRecord>)
            {
                #region 班級批次新增








                #endregion
            }

            InsertRow();

            #endregion
        }

        #region Method

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

        private int AddNewRowSetColor()
        {
            #region 新增一行,並且預設顏色
            int addNewRowIndex = dgv.Rows.Add();

            foreach (DataGridViewCell cell in dgv.Rows[addNewRowIndex].Cells)
            {
                if (PeriodDic.ContainsValue(cell.OwningColumn.Index))
                {
                    cell.Style.BackColor = Color.White;
                }
            }
            return addNewRowIndex; 
            #endregion
        }

        private void TextInsertError(TextBoxX Text, string ErrorInfo)
        {
            #region 設定Text錯誤訊息之用
            Text.SelectAll();
            errorProvider1.SetError(Text, ErrorInfo);
            errorProvider1.SetIconPadding(Text, -20);
            #endregion
        }

        private void ReflashSchoolClass()
        {
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

            #region 全校學生資訊(Key班級名稱,value學生List)

            //Dictionary<string, List<StudentRecord>> _ClassList = new Dictionary<string, List<StudentRecord>>();

            //int t = Environment.TickCount;

            //foreach (ClassRecord var in Class.GetAll())
            //{
            //    _ClassList.Add(var.Name, new List<StudentRecord>());

            //    foreach (StudentRecord stud in var.Students)
            //    {
            //        _ClassList[var.Name].Add(stud);
            //    }
            //}


            //List<JHSchool.Data.ClassRecord> Classes = JHSchool.Data.Class.GetAll();
            //List<JHSchool.Data.StudentRecord> Students = JHSchool.Data.Student.GetAll();

            //foreach (JHSchool.Data.StudentRecord Student in Students)
            //{
            //    if (!_ClassList.ContainsKey(Student.Class.Name))
            //        _ClassList.Add(Student.Class.Name, new List<JHSchool.Data.StudentRecord>());

            //    _ClassList[Student.Class.Name].Add(Student);
            //}

            //MsgBox.Show("" + (Environment.TickCount - t));


            //List<StudentRecord> stus = Student.GetAll(); //取得全部學生
            //foreach (ClassRecord var in Class.GetAll()) //取得全部班級
            //{
            //    _ClassList.Add(var.Name, new List<StudentRecord>()); //建立以班級名稱為key的字典
            //}

            //foreach (StudentRecord stu in stus)
            //{
            //    if (null == stu.Class) //沒有班級不列入
            //        continue;

            //    foreach (string each in _ClassList.Keys) //取得班級名稱
            //    {
            //        if (stu.Class.Name == each) //如果名稱相符則加入
            //        {
            //            _ClassList[each].Add(stu);
            //            break;
            //        }
            //    }
            //}
            #endregion
        }

        private void ReflashAbsence()
        {
            #region 假別熱鍵表
            cpAbsence.Controls.Clear();
            Dictionary<string, string> List1 = _AbsenceHelper.GetListNameHotKey();
            Dictionary<string, string> List2 = _AbsenceHelper.GetListNameAbbreviation();

            foreach (string each in List1.Keys)
            {
                DevComponents.DotNetBar.LabelX newLabel = new DevComponents.DotNetBar.LabelX();
                newLabel.Text = each + "=" + List2[each] + "=" + List1[each];

                if (List1[each].Length == 1 && !AltKey.ContainsKey(List1[each]))
                {
                    AltKey.Add(List1[each], char.Parse(List1[each]));
                }

                newLabel.Size = new Size(90, 20);
                cpAbsence.Controls.Add(newLabel);
            }
            #endregion
        }

        public bool CheckRowError(DataGridViewRow _row)
        {
            #region 儲存前檢查Row
            Dictionary<DataGridViewCell, bool> dic = new Dictionary<DataGridViewCell, bool>();

            foreach (DataGridViewCell _cell in _row.Cells)
            {
                if (PeriodDic.ContainsValue(_cell.ColumnIndex))
                    continue;

                if (_cell.ColumnIndex == StudentNumberColumnIndex)
                    continue;

                //如果內容有空值
                if ("" + _cell.Value == string.Empty)
                {
                    dic.Add(_cell, false);
                }

                //如果內容有錯誤訊息
                if ("" + _cell.ErrorText != string.Empty)
                {
                    dic.Add(_cell, false);
                }
            }

            return dic.ContainsValue(false);
            #endregion
        }

        private void ReflashPeriod()
        {
            #region 動態填入節次欄位
            _PeriodHelper = new PeriodHelper();
            Dictionary<int, string> Pe = _PeriodHelper._GetPeriodDic;
            PeriodDic.Clear();
            List<int> list = new List<int>();
            foreach (int str in _PeriodHelper._GetPeriodDic.Keys)
            {
                list.Add(str);
            }
            list.Sort();
            int insert = 5;
            foreach (int each in list)
            {
                PeriodDic.Add(Pe[each], insert);
                DataGridViewColumn PeriodDataGridView = new DataGridViewColumn(dgv.Rows[0].Cells[0]);
                PeriodDataGridView.Width = 23;
                PeriodDataGridView.HeaderText = Pe[each];
                dgv.Columns.Insert(insert, PeriodDataGridView);
                insert++;
            }
            SchoolYearIndex = insert;
            SemesterIndex = insert + 1;
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

        private string DateInsertSlash(string TimeString)
        {
            #region 將8碼之時間,插入"\"符號
            string InsertSlash = TimeString.Insert(4, "/");
            InsertSlash = InsertSlash.Insert(7, "/");
            return InsertSlash;
            #endregion
        }

        #endregion

        #region 畫面按鈕

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        private void dgv_KeyUp(object sender, KeyEventArgs e)
        {
            #region 快速填滿熱鍵

            if (dgv.Rows.Count <= 0)
                return;

            if (dgv.CurrentRow.ReadOnly == true)
                return;

            Keys key = (e.KeyData & Keys.KeyCode);

            #region 如果是按Alt+S
            if (key == Keys.S && e.Alt) //Alt+Enter
            {
                _RowSave(dgv.CurrentRow);
                return;
            } 
            #endregion

            #region 如果是按下Alt+A
            if (key == Keys.A && e.Alt)
            {
                foreach (DataGridViewCell cell in dgv.CurrentRow.Cells)
                {
                    if (PeriodDic.ContainsValue(cell.OwningColumn.Index))
                    {
                        cell.Value = "";
                    }
                }
                return;
            } 
            #endregion

            #region 如果是按Alt+熱鍵
            bool xyz = false;
            string NowCellHotKey = "";
            foreach (string each in AltKey.Keys)
            {
                if (AltKey[each] == e.KeyValue)
                {
                    xyz = true;
                    NowCellHotKey = each;
                    break;
                }
            }

            if (xyz && e.Alt)
            {
                foreach (DataGridViewCell _cell in dgv.CurrentRow.Cells)
                {
                    if (PeriodDic.ContainsValue(_cell.OwningColumn.Index))
                    {
                        _cell.Value = _AbsenceHelper.GetAbbreviationByHotKey(NowCellHotKey);
                    }
                }
            }  
            #endregion

            #region 如果是按下Enter
            if (key == Keys.Enter)
            {
                if (dgv.CurrentCell.ColumnIndex == SemesterIndex)
                {
                    if (dgv.CurrentRow.Index == dgv.Rows.Count - 1)
                    {
                        _RowSave(dgv.CurrentRow);
                        //int addrow = AddNewRowSetColor();
                        dgv.CurrentCell = dgv.Rows[dgv.Rows.Count - 1].Cells[0];
                    }
                }
            }
            #endregion

            #endregion
        }

        private void btnSetClassNameCode_Click(object sender, EventArgs e)
        {
            SetClassCode cc = new SetClassCode();
            cc.ShowDialog();

            ClassNameDic = DataSort.GetClassNameDic();
        }
    }
}
