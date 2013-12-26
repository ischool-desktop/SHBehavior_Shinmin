using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Behavior.Shinmin.StudentsSpecial
{
    class StudentRobot
    {
        public List<string> StudentIDList { get; set; }

        //班級ID / 班級資料清單物件
        public Dictionary<string, StudentDateObj> StudentDateObjDic = new Dictionary<string, StudentDateObj>();

        public Dictionary<string, ClassRecord> ClassRecordDic = new Dictionary<string, ClassRecord>();
        //取得班級學生
        public StudentRobot()
        {
            GetClassRecord GCR = new GetClassRecord();
            StudentIDList = GCR.StudentInINEB2;
            foreach (ClassRecord each in GCR.SortClasslist)
            {
                if(!ClassRecordDic.ContainsKey(each.ID))
                {
                    ClassRecordDic.Add(each.ID, each);
                }
            }
        }

        //統計學生的獎懲資料
        public void SetMeritDemeritInClassObj(MeritDemeritObj obj)
        {
            #region 掃瞄獎勵資料
            foreach (MeritRecord each in obj.MeritList)
            {
                if (!StudentDateObjDic.ContainsKey(each.RefStudentID))
                {
                    StudentDateObjDic.Add(each.RefStudentID, new StudentDateObj(each.Student));
                }

                StudentDateObjDic[each.RefStudentID].MeritList.Add(each);
                StudentDateObjDic[each.RefStudentID]._大功 += each.MeritA.HasValue ? each.MeritA.Value : 0;
                StudentDateObjDic[each.RefStudentID]._小功 += each.MeritB.HasValue ? each.MeritB.Value : 0;
                StudentDateObjDic[each.RefStudentID]._嘉獎 += each.MeritC.HasValue ? each.MeritC.Value : 0;
            } 
            #endregion

            #region 掃瞄懲戒資料
            foreach (DemeritRecord each in obj.DemeritList)
            {
                if (each.Cleared == "是") //如果已銷過就排除
                    continue;

                if (!StudentDateObjDic.ContainsKey(each.RefStudentID))
                {
                    StudentDateObjDic.Add(each.RefStudentID, new StudentDateObj(each.Student));
                }

                StudentDateObjDic[each.RefStudentID].DemeritList.Add(each);
                StudentDateObjDic[each.RefStudentID]._大過 += each.DemeritA.HasValue ? each.DemeritA.Value : 0;
                StudentDateObjDic[each.RefStudentID]._小過 += each.DemeritB.HasValue ? each.DemeritB.Value : 0;
                StudentDateObjDic[each.RefStudentID]._警告 += each.DemeritC.HasValue ? each.DemeritC.Value : 0;
            } 
            #endregion
        }

        //統計值
        public void SumOfAll(GetConfigSetup configSetup)
        {
            //進行 獎/懲 加總,並換算為大過隻數

            List<string> list = new List<string>();

            foreach (string each in StudentDateObjDic.Keys)
            {
                //1.先換算為最低隻數
                int 嘉獎隻數 = 0;
                嘉獎隻數 += StudentDateObjDic[each]._大功 * configSetup.MeritAtoB * configSetup.MeritBtoC;
                嘉獎隻數 += StudentDateObjDic[each]._小功 * configSetup.MeritBtoC;
                嘉獎隻數 += StudentDateObjDic[each]._嘉獎;
                int 警告支數 = 0;
                警告支數 += StudentDateObjDic[each]._大過 * configSetup.DemeritAtoB * configSetup.DemeritBtoC;
                警告支數 += StudentDateObjDic[each]._小過 * configSetup.DemeritBtoC;
                警告支數 += StudentDateObjDic[each]._警告;
                
                //2.再相減
                int 加總隻數 = 嘉獎隻數 - 警告支數;

                //3.再換算為大過隻數
                if (加總隻數 >= 0) //如果加總後大於0,則不判斷與換算
                {
                    list.Add(each);
                    continue;
                }

                加總隻數 = 加總隻數 * -1;

                //警告換算為小過隻數
                int a1 = 加總隻數 / configSetup.DemeritBtoC;

                //餘數為"警告"隻數
                int a2 = 加總隻數 % configSetup.DemeritBtoC;

                //小過換算為"大過"隻數
                int b1 = a1 / configSetup.DemeritAtoB;

                //餘數為"小過"隻數
                int b2 = a1 % configSetup.DemeritAtoB;

                StudentDateObjDic[each].功過相抵_大過 = b1;
                StudentDateObjDic[each].功過相抵_小過 = b2;
                StudentDateObjDic[each].功過相抵_警告 = a2;

                //小於3的會被移除
                if (StudentDateObjDic[each].功過相抵_大過 < 3)
                {
                    list.Add(each);
                    continue;
                }
                else
                {

                }
            }
            //將未滿3大過之學生移除
            foreach (string each in list)
            {
                if (StudentDateObjDic.ContainsKey(each))
                {
                    StudentDateObjDic.Remove(each);
                }
            }
        }
    }

    class StudentDateObj
    {
        public StudentDateObj(StudentRecord student)
        {
            _StudentID = student.ID;
            _StudentRecord = student;
            MeritList = new List<MeritRecord>();
            DemeritList  =new List<DemeritRecord>();
        }
        public string _StudentID { get; set; }
        public StudentRecord _StudentRecord { get; set; }
        public List<MeritRecord> MeritList { get; set; }
        public List<DemeritRecord> DemeritList { get; set; }

        public int _大功 { get; set; }
        public int _小功 { get; set; }
        public int _嘉獎 { get; set; }

        public int _大過 { get; set; }
        public int _小過 { get; set; }
        public int _警告 { get; set; }

        public int 功過相抵_大過 { get; set; }
        public int 功過相抵_小過 { get; set; }
        public int 功過相抵_警告 { get; set; }
    }   
}
