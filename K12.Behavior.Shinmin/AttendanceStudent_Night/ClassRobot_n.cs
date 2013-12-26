using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Behavior.Shinmin.AttendanceStatistics_進校
{
    class ClassRobot_n
    {
        //學生人數
        public List<string> StudentIDList { get; set; }
        //班級ID / 班級資料清單物件
        public Dictionary<string, ClassDataObj> ClassDataObjDic = new Dictionary<string, ClassDataObj>();

        /// <summary>
        /// 取得班級學生與建立資料物件
        /// </summary>
        public ClassRobot_n(GetConfigSetup_n config, List<string> StudIDList)
        {
            //取得缺曠類型單位,如果
            List<string> list = new List<string>();
            foreach (string each in config.AbsenceDic.Keys)
            {
                //如果Absence為True,才加入清單
                if (config.AbsenceDic[each])
                {
                    list.Add(each);
                }
            }

            //GetClassRecord GCR = new GetClassRecord();
            StudentIDList = StudIDList;
            List<string> classList = new List<string>();
            foreach (StudentRecord stud in Student.SelectByIDs(StudentIDList))
            {
                if (string.IsNullOrEmpty(stud.RefClassID))
                    continue;

                if (!classList.Contains(stud.RefClassID))
                    classList.Add(stud.RefClassID);

            }

            List<ClassRecord> SortClasslist = SortClassIndex.K12Data_ClassRecord(Class.SelectByIDs(classList));
            #region 班級資料清單
            foreach (ClassRecord each in SortClasslist)
            {
                if (!ClassDataObjDic.ContainsKey(each.ID))
                {
                    ClassDataObjDic.Add(each.ID, new ClassDataObj(each, list));
                }
            }
            #endregion
        }

        /// <summary>
        /// 統計各班缺曠資料
        /// </summary>
        public void SetAttendanceInClassObj(AttendanceObj_n obj)
        {
            foreach (AttendanceRecord attendance in obj.AttendanceList)
            {
                string classID = attendance.Student.RefClassID;

                if (string.IsNullOrEmpty(classID)) //有班級ID
                    continue;

                if (ClassDataObjDic.ContainsKey(classID))
                {
                    foreach (AttendancePeriod attPeriod in attendance.PeriodDetail)
                    {
                        if (ClassDataObjDic[classID].AbsenceDic.ContainsKey(attPeriod.AbsenceType))
                        {
                            ClassDataObjDic[classID].AbsenceDic[attPeriod.AbsenceType]++;
                        }
                    }
                }
            }

        }

        /// <summary>
        /// 計算總缺席數到課率
        /// </summary>
        internal void SumOfAllTheInformation(int 時間區間內總節數)
        {
            //計算各班人數
            foreach (StudentRecord student in Student.SelectByIDs(StudentIDList))
            {
                if (!string.IsNullOrEmpty(student.RefClassID))
                {
                    ClassDataObjDic[student.RefClassID].班級學生人數++;
                }
            }

            foreach (string each1 in ClassDataObjDic.Keys)
            {
                ClassDataObjDic[each1].Total();

                if (ClassDataObjDic[each1].班級學生人數 != 0)
                {
                    int 班級學生人數 = ClassDataObjDic[each1].班級學生人數;
                    int 班級缺課數 = ClassDataObjDic[each1].總缺席數;
                    double x = (時間區間內總節數 * 班級學生人數) - 班級缺課數;
                    double y = 班級學生人數 * 時間區間內總節數;
                    double z = (x / y) * 100;
                    ClassDataObjDic[each1].到課率 = Math.Round(z, 2, MidpointRounding.AwayFromZero);

                }
            }

            //時間區間內總節數

            //班級學生人數

            //總缺席數/班級學生人數*時間區間內總節數

            //到課率=(總節數*班級人數)-班級缺課數/(班級人數*總節數)*100%(取到小數第二位)
        }
    }
}
