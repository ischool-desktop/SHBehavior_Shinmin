using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Behavior.Shinmin.MeritDemeritStatistics
{
    //獎懲統計物件
    class MeritDemeritObj
    {
        public List<MeritRecord> MeritList { get; set; }

        public List<DemeritRecord> DemeritList { get; set; }
        //1.取得資料
        //2.每個班級的加總統計值
        public MeritDemeritObj(bool 略過班導師註記, bool IsByOccurDate, List<string> StudentIDList, DateTime StartDate, DateTime EndDate)
        {
            #region 依 學生ID /開始日期 /結束日期 取得獎懲資料
            //依登錄日期還是發生日期
            if (IsByOccurDate)
            {
                MeritList = Merit.SelectByOccurDate(StudentIDList, StartDate, EndDate);
                DemeritList = Demerit.SelectByOccurDate(StudentIDList, StartDate, EndDate);
            }
            else
            {
                MeritList = Merit.SelectByRegisterDate(StudentIDList, StartDate, EndDate);
                DemeritList = Demerit.SelectByRegisterDate(StudentIDList, StartDate, EndDate);
            }
            #endregion

            #region 略過班導師註記
            //取得所有具有註記之獎懲資料ID
            //如果為True,就要將資料移除
            if (略過班導師註記) 
            {
                List<MeritRecord> RemoveMerit = new List<MeritRecord>();
                foreach (MeritRecord merit in TeacherNote.GetTeacherNoteMeritList(StudentIDList))
                {
                    foreach (MeritRecord each in MeritList)
                    {
                        if (each.ID == merit.ID)
                        {
                            RemoveMerit.Add(each);
                        }
                    }
                }
                foreach (MeritRecord each in RemoveMerit)
                {
                    MeritList.Remove(each);
                }
                List<DemeritRecord> RemoveDemerit = new List<DemeritRecord>();
                foreach (DemeritRecord demerit in TeacherNote.GetTeacherNoteDemeritList(StudentIDList))
                {
                    foreach (DemeritRecord each in DemeritList)
                    {
                        if (each.ID == demerit.ID)
                        {
                            RemoveDemerit.Add(each);
                        }
                    }
                }
                foreach (DemeritRecord each in RemoveDemerit)
                {
                    DemeritList.Remove(each);
                }
            } 
            #endregion
        }
    }
}
