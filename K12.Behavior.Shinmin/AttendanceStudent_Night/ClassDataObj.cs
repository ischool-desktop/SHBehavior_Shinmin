using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Behavior.Shinmin.AttendanceStatistics_進校
{
    class ClassDataObj
    {

        //增加一層學生資料

        
        public string _ClassID { get; set; }
        public ClassRecord _classRecord { get; set; }
        public Dictionary<string, int> AbsenceDic { get; set; }

        //就是把AbsenceDic相加啦...
        public int 總缺席數 { get; set; }

        public int 班級學生人數 { get; set; }

        public double 到課率 { get; set; }

        public ClassDataObj(ClassRecord classRecord, List<string> list)
        {
            _ClassID = classRecord.ID;
            _classRecord = classRecord;

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
