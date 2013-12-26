using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Behavior.Shinmin.StudentsSpecial
{
    class MeritDemeritObj
    {
        public List<MeritRecord> MeritList { get; set; }

        public List<DemeritRecord> DemeritList { get; set; }

        //學生清單 / 學年度 / 學期
        public MeritDemeritObj(List<string> StudentIDList, int SchoolYear, int Semester)
        {
            //取得獎懲資料
            MeritList = Merit.SelectBySchoolYearAndSemester(StudentIDList, SchoolYear, Semester);
            DemeritList = Demerit.SelectBySchoolYearAndSemester(StudentIDList, SchoolYear, Semester);
        }

        //學生清單 / 學年度 / 學期
        public MeritDemeritObj(List<string> StudentIDList)
        {
            //取得獎懲資料
            MeritList = Merit.SelectByStudentIDs(StudentIDList);
            DemeritList = Demerit.SelectByStudentIDs(StudentIDList);
        }

        ///// <summary>
        ///// 換算公式方法
        ///// </summary>
        //public void ConversionFormula(K12.Data.DisciplineRecord DiscRecord)
        //{
            //銷過也要統計??
            //DiscRecord.
            //功過相抵結果需要換算為最大值
            //需要參考畢業預警報表內容算法
        //}
    }
}
