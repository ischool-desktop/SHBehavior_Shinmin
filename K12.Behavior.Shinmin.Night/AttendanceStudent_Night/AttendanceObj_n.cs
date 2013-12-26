using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Behavior.Shinmin.Night
{
    class AttendanceObj_n
    {
        public List<AttendanceRecord> AttendanceList { get; set; }

        public AttendanceObj_n(bool 略過六日資料, List<string> StudentIDList, DateTime StartDate, DateTime EndDate)
        {
            List<StudentRecord> StudentRecordList = Student.SelectByIDs(StudentIDList);
            AttendanceList = Attendance.SelectByDate(StudentRecordList, StartDate, EndDate);

            if (略過六日資料)
            {
                #region 略過六日資料
                List<AttendanceRecord> RemoveAttendance = new List<AttendanceRecord>();
                //如果為星期六或星期日
                foreach (AttendanceRecord each in AttendanceList)
                {
                    if (each.OccurDate.DayOfWeek == DayOfWeek.Sunday || each.OccurDate.DayOfWeek == DayOfWeek.Saturday)
                    {
                        RemoveAttendance.Add(each);
                    }
                }
                //移除 六/日 資料
                foreach (AttendanceRecord each in RemoveAttendance)
                {
                    AttendanceList.Remove(each);
                }
                #endregion
            }
        }
    }
}
