using System.Collections.Generic;
using SmartSchool.API.PlugIn;
using System;
using K12.Data;
using FISCA.UDT;

namespace K12.Behavior.Shinmin
{
    class ExportDiscipline : SmartSchool.API.PlugIn.Export.Exporter
    {
        //建構子
        public ExportDiscipline()
        {
            this.Image = null;
            this.Text = "匯出獎懲記錄(導師註記)";
        }

        public override void InitializeExport(SmartSchool.API.PlugIn.Export.ExportWizard wizard)
        {
            wizard.ExportableFields.AddRange("學年度", "學期", "日期", "大功", "小功", "嘉獎", "大過", "小過", "警告", "事由", "是否銷過", "銷過日期", "銷過事由", "登錄日期", "留校察看", "導師註記");

            wizard.ExportPackage += delegate(object sender, SmartSchool.API.PlugIn.Export.ExportPackageEventArgs e)
            {

                //依學生編號+獎懲編號取得導師註記資料
                //可節省許多網路傳輸的資源

                List<StudentRecord> students = Student.SelectByIDs(e.List);

                #region 收集資料(DicMerit)
                Dictionary<string, List<DisciplineRecord>> DicDiscipline = new Dictionary<string, List<DisciplineRecord>>();

                //取得學生所有獎懲資料
                List<DisciplineRecord> ListDiscipline = Discipline.SelectByStudentIDs(e.List);

                //ListDiscipline.Sort(SortDate);
                List<string> MeritIDList = new List<string>();
                List<string> DemeritIDList = new List<string>();

                foreach (DisciplineRecord disRecord in ListDiscipline)
                {
                    if (!DicDiscipline.ContainsKey(disRecord.RefStudentID))
                    {
                        DicDiscipline.Add(disRecord.RefStudentID, new List<DisciplineRecord>());
                    }
                    DicDiscipline[disRecord.RefStudentID].Add(disRecord);

                    //取得導師註記資料
                    if (disRecord.MeritFlag == "0")
                    {
                        DemeritIDList.Add(disRecord.ID);
                    }
                    else if (disRecord.MeritFlag == "1")
                    {
                        MeritIDList.Add(disRecord.ID);
                    }
                }


                AccessHelper _accessHelper = new AccessHelper();

                //懲戒
                List<string> DemeritPointIDList = new List<string>();
                if (DemeritIDList.Count > 0)
                {
                    List<string> test = new List<string>();
                    foreach (string each in DemeritIDList)
                    {
                        test.Add(string.Format("'{0}'", each));
                    }
                    string test1 = "DemeritID in(" + string.Join(",", test.ToArray()) + ")";
                    foreach (TeacherSetDemerit each in _accessHelper.Select<TeacherSetDemerit>(test1))
                    {
                        if (!DemeritPointIDList.Contains(each.DemeritID))
                        {
                            DemeritPointIDList.Add(each.DemeritID);
                        }
                    }
                }

                //獎勵
                List<string> MeritPointIDList = new List<string>();
                if (MeritIDList.Count > 0)
                {
                    List<string> test = new List<string>();
                    foreach (string each in MeritIDList)
                    {
                        test.Add(string.Format("'{0}'", each));
                    }
                    string test2 = "MeritID in(" + string.Join(",", test.ToArray()) + ")";

                    foreach (TeacherSetMerit each in _accessHelper.Select<TeacherSetMerit>(test2))
                    {
                        if (!MeritPointIDList.Contains(each.MeritID))
                        {
                            MeritPointIDList.Add(each.MeritID);
                        }
                    }
                }
                #endregion

                students.Sort(SortStudent);

                foreach (StudentRecord stud in students)
                {
                    if (DicDiscipline.ContainsKey(stud.ID))
                    {

                        DicDiscipline[stud.ID].Sort(SortDate);

                        foreach (DisciplineRecord JHR in DicDiscipline[stud.ID])
                        {
                            string Note = "";
                            if (JHR.MeritFlag == "0")
                            {
                                if (DemeritPointIDList.Contains(JHR.ID))
                                {
                                    Note = "是";
                                }
                            }
                            else if (JHR.MeritFlag == "1")
                            {
                                if (MeritPointIDList.Contains(JHR.ID))
                                {
                                    Note = "是";
                                }
                            }



                            string OccurdateString = JHR.OccurDate.ToShortDateString();

                            string ClearDateString = "";
                            if (JHR.ClearDate.HasValue)
                            {
                                ClearDateString = JHR.ClearDate.Value.ToShortDateString();
                            }

                            string RegisterDateString = "";
                            if (JHR.RegisterDate.HasValue)
                            {
                                RegisterDateString = JHR.RegisterDate.Value.ToShortDateString();
                            }

                            RowData row = new RowData();
                            row.ID = stud.ID;
                            foreach (string field in e.ExportFields)
                            {
                                if (wizard.ExportableFields.Contains(field))
                                {
                                    switch (field)
                                    {
                                        case "學年度": row.Add(field, "" + JHR.SchoolYear.ToString()); break;
                                        case "學期": row.Add(field, "" + JHR.Semester.ToString()); break;
                                        case "日期": row.Add(field, "" + OccurdateString); break;
                                        case "大功": row.Add(field, JHR.MeritA != null ? "" + JHR.MeritA.ToString() : "0"); break;
                                        case "小功": row.Add(field, JHR.MeritB != null ? "" + JHR.MeritB.ToString() : "0"); break;
                                        case "嘉獎": row.Add(field, JHR.MeritC != null ? "" + JHR.MeritC.ToString() : "0"); break;
                                        case "大過": row.Add(field, JHR.DemeritA != null ? "" + JHR.DemeritA.ToString() : "0"); break;
                                        case "小過": row.Add(field, JHR.DemeritB != null ? "" + JHR.DemeritB.ToString() : "0"); break;
                                        case "警告": row.Add(field, JHR.DemeritC != null ? "" + JHR.DemeritC.ToString() : "0"); break;
                                        case "事由": row.Add(field, "" + JHR.Reason); break;
                                        case "是否銷過": row.Add(field, "" + JHR.Cleared); break;
                                        case "銷過日期": row.Add(field, "" + ClearDateString); break;
                                        case "銷過事由": row.Add(field, "" + JHR.ClearReason); break;
                                        case "登錄日期": row.Add(field, "" + RegisterDateString); break;
                                        case "留校察看": row.Add(field, "" + JHR.MeritFlag == "2" ? "是" : ""); break;
                                        case "導師註記": row.Add(field, "" + Note); break;
                                    }
                                }
                            }
                            e.Items.Add(row);
                        }
                    }
                }
            };
        }

        private int SortStudent(StudentRecord x, StudentRecord y)
        {

            string xx1 = x.Class != null ? x.Class.Name : "";
            string xx2 = x.SeatNo.HasValue ? x.SeatNo.Value.ToString().PadLeft(3, '0') : "000";
            string xx3 = xx1 + xx2;

            string yy1 = y.Class != null ? y.Class.Name : "";
            string yy2 = y.SeatNo.HasValue ? y.SeatNo.Value.ToString().PadLeft(3, '0') : "000";
            string yy3 = yy1 + yy2;

            return xx3.CompareTo(yy3);
        }

        private int SortDate(DisciplineRecord x, DisciplineRecord y)
        {
            return x.OccurDate.CompareTo(y.OccurDate);
        }
    }
}
