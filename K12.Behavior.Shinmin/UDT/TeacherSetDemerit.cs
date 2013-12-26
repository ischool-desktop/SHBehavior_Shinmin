using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace K12.Behavior.Shinmin
{
    public class TeacherSetDemerit : ActiveRecord
    {
        /// <summary>
        /// 懲戒系統ID
        /// </summary>
        [Field(Field = "DemeritID", Indexed = true)]
        public string DemeritID { get; set; }

        /// <summary>
        /// 資訊擁有者,學生ID
        /// </summary>
        [Field(Field = "StudentID", Indexed = false)]
        public string StudentID { get; set; }

        /// <summary>
        /// 是否為老師註記
        /// </summary>
        [Field(Field = "IsTeacherNote", Indexed = false)]
        public bool IsTeacherNote { get; set; }

        /// <summary>
        /// 學年度
        /// </summary>
        [Field(Field = "SchoolYear", Indexed = false)]
        public int SchoolYear { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        [Field(Field = "Semester", Indexed = false)]
        public int Semester { get; set; }
    }
}
