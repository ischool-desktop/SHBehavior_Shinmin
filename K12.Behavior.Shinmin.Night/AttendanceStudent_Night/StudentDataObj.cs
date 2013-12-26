using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Behavior.Shinmin.Night
{
    class StudentDataObj
    {
        public StudentRecord _stud { get; set; }

        public Dictionary<string, int> AbsenceDic { get; set; }

        //就是把AbsenceDic相加啦...
        public int 總缺席數 { get; set; }

        /// <summary>
        /// 傳入學生物件與缺曠類別
        /// </summary>
        public StudentDataObj(StudentRecord stud, List<string> list)
        {
            _stud = stud;

            //建立基本模型
            AbsenceDic = new Dictionary<string, int>();
            foreach (string each in list)
            {
                AbsenceDic.Add(each, 0);
            }
        }

        //計算總缺席數
        public void Total()
        {
            foreach (string each2 in AbsenceDic.Keys)
            {
                總缺席數 += AbsenceDic[each2];
            }
        }




    }
}
