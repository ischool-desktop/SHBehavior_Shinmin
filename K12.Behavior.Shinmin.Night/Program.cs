using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using FISCA.Permission;
using FISCA.Presentation;
using K12.Data;

namespace K12.Behavior.Shinmin.Night
{
    public static class Program
    {
        private static string CountByAbsenceNameNight = "進校缺曠週報表(依節次)";

        [MainMethod()]
        public static void Main()
        {
            ClassFalse();

            //學生報表
            RibbonBarButton rbItemStudent_n = K12.Presentation.NLDPanels.Student.RibbonBarItems["資料統計"]["報表"];
            rbItemStudent_n["新民客制報表"]["進校學生缺席統計表"].Enable = Permissions.進校學生缺席統計表權限;
            rbItemStudent_n["新民客制報表"]["進校學生缺席統計表"].Click += delegate
            {

                AttendanceForm_n att = new AttendanceForm_n(K12.Presentation.NLDPanels.Student.SelectedSource);
                att.ShowDialog();
            };

            //班級報表
            RibbonBarItem rbItemClass = MotherForm.RibbonBarItems["班級", "資料統計"];
            rbItemClass["報表"]["新民客制報表"]["進校班級學生缺席統計表"].Enable = Permissions.進校班級學生缺席統計表權限;
            rbItemClass["報表"]["新民客制報表"]["進校班級學生缺席統計表"].Click += delegate
            {
                List<StudentRecord> studList = new List<StudentRecord>();
                foreach (StudentRecord stud in K12.Data.Student.SelectByClassIDs(K12.Presentation.NLDPanels.Class.SelectedSource))
                {
                    if (stud.StatusStr == "一般" || stud.StatusStr == "延修")
                    {
                        studList.Add(stud);
                    }
                }
                AttendanceForm_n att = new AttendanceForm_n(studList.Select(x => x.ID).ToList());
                att.ShowDialog();
            };

            K12.Presentation.NLDPanels.Class.SelectedSourceChanged += delegate
            {
                if (K12.Presentation.NLDPanels.Class.SelectedSource.Count <= 0)
                {
                    ClassFalse();
                }
                else
                {
                    K12.Presentation.NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["新民客制報表"][CountByAbsenceNameNight].Enable = Permissions.缺曠週報表_依節次_進校權限 && K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0;
                }
            };

            K12.Presentation.NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["新民客制報表"][CountByAbsenceNameNight].Click += delegate
            {
                new Report().Print();
            };

            #region 註冊權限(目前依附ischool高中的xml檔案)

            Catalog ribbon = RoleAclSource.Instance["新民客製功能"]["班級報表"];
            ribbon.Add(new FISCA.Permission.RibbonFeature(Permissions.缺曠週報表_依節次_進校, CountByAbsenceNameNight));

            ribbon = RoleAclSource.Instance["新民客製功能"]["學生報表"];
            ribbon.Add(new FISCA.Permission.RibbonFeature(Permissions.進校學生缺席統計表, "進校學生缺席統計表"));

            ribbon = RoleAclSource.Instance["新民客製功能"]["班級報表"];
            ribbon.Add(new FISCA.Permission.RibbonFeature(Permissions.進校班級學生缺席統計表, "進校班級學生缺席統計表"));
            #endregion
        }

        private static void ClassFalse()
        {
            K12.Presentation.NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["新民客制報表"][CountByAbsenceNameNight].Enable = false;
        }


    }
}
