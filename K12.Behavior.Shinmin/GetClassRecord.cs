using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace K12.Behavior.Shinmin
{
    public class GetClassRecord
    {
        /// <summary>
        /// 所選擇的班級ID清單
        /// </summary>
        public List<string> ClassIDList { get; set; }

        /// <summary>
        /// 班級清單的所有學生Record
        /// </summary>
        public List<StudentRecord> StudentRecordList { get; set; }

        /// <summary>
        /// 班級清單的所有學生ID
        /// </summary>
        public List<string> StudentIDList { get; set; }

        /// <summary>
        /// 排序過的班級Record
        /// </summary>
        public List<ClassRecord> SortClasslist { get; set; }

        /// <summary>
        /// 班級清單的一般狀態學生Record
        /// </summary>
        public List<StudentRecord> StudentInINEB1 { get; set; }

        /// <summary>
        /// 班級清單的一般狀態學生ID
        /// </summary>
        public List<string> StudentInINEB2 { get; set; }

        public GetClassRecord()
        {
            //取得班級清單
            ClassIDList = K12.Presentation.NLDPanels.Class.SelectedSource;

            StudentInINEB1 = new List<StudentRecord>();

            #region 取得狀態非(刪除&畢業或離校)的學生
            List<StudentRecord> StudentRecordList = new List<StudentRecord>();
            foreach (StudentRecord each in Student.SelectByClassIDs(ClassIDList))
            {
                //篩選狀態
                if (each.Status != StudentRecord.StudentStatus.刪除 && each.Status != StudentRecord.StudentStatus.畢業或離校)
                {
                    if (!StudentRecordList.Contains(each))
                    {
                        StudentRecordList.Add(each);
                    }
                }

                if (each.Status == StudentRecord.StudentStatus.一般 || each.Status == StudentRecord.StudentStatus.延修)
                {
                    if (!StudentInINEB1.Contains(each))
                    {
                        StudentInINEB1.Add(each);
                    }
                }
            }
            StudentIDList = StudentRecordList.Select(x => x.ID).ToList();

            StudentInINEB2 = StudentInINEB1.Select(x => x.ID).ToList();
            #endregion

            //排序
            SortClasslist = SortClassIndex.K12Data_ClassRecord(Class.SelectByIDs(ClassIDList));
        }
    }
}
