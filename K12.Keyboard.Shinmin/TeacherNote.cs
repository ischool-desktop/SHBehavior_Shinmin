using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;
using K12.Data;

namespace K12.Keyboard.Shinmin
{
    static public class TeacherNote
    {
        static AccessHelper _accessHelper = new AccessHelper();

        /// <summary>
        /// 學生ID / 註記物件
        /// </summary>
        static public Dictionary<string, TeacherSetMerit> MeritDic = new Dictionary<string, TeacherSetMerit>();
        /// <summary>
        /// 學生ID / 註記物件
        /// </summary>
        static public Dictionary<string, TeacherSetDemerit> DemeritDic = new Dictionary<string, TeacherSetDemerit>();

        /// <summary>
        /// 取得具有"導師註記"之獎勵資料
        /// </summary>
        static public List<MeritRecord> GetTeacherNoteMeritList()
        {
            List<string> StudentIDList = new List<string>();
            MeritDic = new Dictionary<string, TeacherSetMerit>();
            List<TeacherSetMerit> MeritPointList = _accessHelper.Select<TeacherSetMerit>();
            foreach (TeacherSetMerit dpl in MeritPointList)
            {
                if (dpl.IsTeacherNote)
                {
                    if (!MeritDic.ContainsKey(dpl.MeritID))
                    {
                        MeritDic.Add(dpl.MeritID, dpl);
                    }
                    if (!StudentIDList.Contains(dpl.StudentID))
                    {
                        StudentIDList.Add(dpl.StudentID);
                    }
                }
            }

            List<MeritRecord> MeritList = new List<MeritRecord>();
            foreach (MeritRecord dem in Merit.SelectByStudentIDs(StudentIDList))
            {
                if (MeritDic.ContainsKey(dem.ID))
                {
                    MeritList.Add(dem);
                }
            }
            return MeritList;

        }

        /// <summary>
        /// 取得具有"導師註記"之懲戒資料
        /// </summary>
        static public List<DemeritRecord> GetTeacherNoteDemeritList()
        {
            List<string> StudentIDList = new List<string>();
            DemeritDic = new Dictionary<string, TeacherSetDemerit>(); 
            List<TeacherSetDemerit> DemeritPointList = _accessHelper.Select<TeacherSetDemerit>();
            foreach (TeacherSetDemerit dpl in DemeritPointList)
            {
                if (dpl.IsTeacherNote)
                {
                    if (!DemeritDic.ContainsKey(dpl.DemeritID))
                    {
                        DemeritDic.Add(dpl.DemeritID, dpl);
                    }

                    if (!StudentIDList.Contains(dpl.StudentID))
                    {
                        StudentIDList.Add(dpl.StudentID);
                    }
                }
            }

            List<DemeritRecord> DemeritList = new List<DemeritRecord>();
            foreach (DemeritRecord dem in Demerit.SelectByStudentIDs(StudentIDList))
            {
                if (DemeritDic.ContainsKey(dem.ID))
                {
                    DemeritList.Add(dem);
                }
            }
            return DemeritList;
        }
    }
}
