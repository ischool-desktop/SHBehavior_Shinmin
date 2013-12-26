using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.UDT;

namespace K12.Behavior.Shinmin
{
    public class TeacherSetMerit : ActiveRecord
    {
        /// <summary>
        /// 獎勵系統ID
        /// </summary>
        [Field(Field = "MeritID", Indexed = true)]
        public string MeritID { get; set; }

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
