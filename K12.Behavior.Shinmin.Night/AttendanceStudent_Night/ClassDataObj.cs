using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Behavior.Shinmin.Night
{
    class ClassDataObj
    {
        public string _ClassID { get; set; }
        public ClassRecord _classRecord { get; set; }

        public int 班級學生人數 { get; set; }

        //排序用
        public List<StudentDataObj> StudentList { get; set; }

        public Dictionary<string, StudentDataObj> StudentDic { get; set; }

        public ClassDataObj(ClassRecord classRecord, List<string> list)
        {
            _ClassID = classRecord.ID;
            _classRecord = classRecord;

            StudentList = new List<StudentDataObj>();
            StudentDic = new Dictionary<string, StudentDataObj>();

            foreach (StudentRecord stud in Student.SelectByClassID(_ClassID))
            {
                if (stud.StatusStr == "一般" || stud.StatusStr == "延修")
                {
                    if (!StudentDic.ContainsKey(stud.ID))
                    {
                        StudentDataObj studObj = new StudentDataObj(stud, list);
                        StudentDic.Add(stud.ID, studObj);

                        //Sort
                        StudentList.Add(studObj);
                    }
                }
            }

            StudentList.Sort(SortStudentDate);
        }

        private int SortStudentDate(StudentDataObj aobj1, StudentDataObj bobj2)
        {
            string AOBJ_1 = aobj1._stud.Class.Name.PadLeft(10, '0');
            AOBJ_1 += aobj1._stud.SeatNo.HasValue ? aobj1._stud.SeatNo.Value.ToString().PadLeft(3, '0') : "000";
            AOBJ_1 += aobj1._stud.Name.PadLeft(10, '0');

            string BOBJ_1 = bobj2._stud.Class.Name.PadLeft(10, '0');
            BOBJ_1 += bobj2._stud.SeatNo.HasValue ? bobj2._stud.SeatNo.Value.ToString().PadLeft(3, '0') : "000";
            BOBJ_1 += bobj2._stud.Name.PadLeft(10, '0');
            
            return AOBJ_1.CompareTo(BOBJ_1);
        }
    }
}
