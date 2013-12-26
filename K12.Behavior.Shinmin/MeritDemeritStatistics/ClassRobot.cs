using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Behavior.Shinmin.MeritDemeritStatistics
{
    //班級物件
    //1.取得班級學生
    class ClassRobot
    {
        public List<string> StudentIDList;

        //班級ID / 班級資料清單物件
        public Dictionary<string, ClassDataObj> ClassDataObjDic = new Dictionary<string, ClassDataObj>();

        //取得班級學生
        public ClassRobot()
        {
            GetClassRecord GCR = new GetClassRecord();
            StudentIDList = GCR.StudentIDList;

            #region 班級資料清單
            foreach (ClassRecord each in GCR.SortClasslist)
            {
                if (!ClassDataObjDic.ContainsKey(each.ID))
                {
                    ClassDataObjDic.Add(each.ID, new ClassDataObj(each));
                }
            }
            #endregion

            
        }

        //統計各班獎勵資料
        public void SetMeritDemeritInClassObj(MeritDemeritObj obj)
        {
            #region 統計各班獎勵資料
            foreach (MeritRecord mr in obj.MeritList)
            {
                if (!string.IsNullOrEmpty(mr.Student.RefClassID)) //有班級ID
                {
                    if (ClassDataObjDic.ContainsKey(mr.Student.RefClassID))
                    {
                        ClassDataObjDic[mr.Student.RefClassID]._大功 += mr.MeritA.HasValue ? mr.MeritA.Value : 0;
                        ClassDataObjDic[mr.Student.RefClassID]._小功 += mr.MeritB.HasValue ? mr.MeritB.Value : 0;
                        ClassDataObjDic[mr.Student.RefClassID]._嘉獎 += mr.MeritC.HasValue ? mr.MeritC.Value : 0;
                    }
                }
            }
            #endregion

            #region 統計各班 懲戒/留查 資料
            foreach (DemeritRecord mr in obj.DemeritList)
            {
                if (!string.IsNullOrEmpty(mr.Student.RefClassID)) //有班級ID
                {
                    if (ClassDataObjDic.ContainsKey(mr.Student.RefClassID))
                    {
                        if (mr.MeritFlag == "2") //留查資料
                        {
                            ClassDataObjDic[mr.Student.RefClassID]._留查++;
                        }
                        else if (mr.Cleared != "是") //未銷過資料
                        {
                            ClassDataObjDic[mr.Student.RefClassID]._大過 += mr.DemeritA.HasValue ? mr.DemeritA.Value : 0;
                            ClassDataObjDic[mr.Student.RefClassID]._小過 += mr.DemeritB.HasValue ? mr.DemeritB.Value : 0;
                            ClassDataObjDic[mr.Student.RefClassID]._警告 += mr.DemeritC.HasValue ? mr.DemeritC.Value : 0;
                        }
                        else //銷過資料
                        {
                        }
                    }
                }
            }
            #endregion
        }

        //開始統計各班分數
        public void SumOfAllTheInformation(GetConfigSetup config)
        {
            foreach (ClassDataObj each in ClassDataObjDic.Values)
            {
                int 統計大功 = each._大功 * config.大功;
                int 統計小功 = each._小功 * config.小功;
                int 統計嘉獎 = each._嘉獎 * config.嘉獎;
                int 統計大過 = each._大過 * config.大過;
                int 統計小過 = each._小過 * config.小過;
                int 統計警告 = each._警告 * config.警告;

                each._總分 = config.班級基本分 + 統計大功 + 統計小功 + 統計嘉獎 + 統計大過 + 統計小過 + 統計警告;
                //如果啟用限制,則判斷分數是否超過100
                //就設定為100
                if (config.啟用總分100分限制)
                {
                    if (each._總分 > 100)
                    {
                        each._總分 = 100;
                    }
                }
            }


        }
    }

    class ClassDataObj
    {
        public ClassDataObj(ClassRecord each)
        {
            _ClassID = each.ID;
            _classRecord = each;
        }
        public string _ClassID { get; set; }
        public ClassRecord _classRecord { get; set; }

        public int _大功 { get; set; }
        public int _小功 { get; set; }
        public int _嘉獎 { get; set; }
        public int _大過 { get; set; }
        public int _小過 { get; set; }
        public int _警告 { get; set; }

        public int _留查 { get; set; }

        public int _公告退學 { get; set; }
        public int _總分 { get; set; } //嘉獎+1、警告-1、小功+3、小過-3、大功+9、大過-9
    }
}
