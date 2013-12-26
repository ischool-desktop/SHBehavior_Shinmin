using FISCA;
using System.Collections.Generic;
using K12.Data;
using FISCA.Permission;

namespace K12.Behavior.WeekReport.Shinmin
{
    public static class Program
    {
        private static string MeritDemeritName = "獎懲週報表";
        private static string CountByPeriodName = "缺曠週報表 (依節次)";
        private static string CountByAbsenceName = "缺曠週報表 (依缺曠別)";

        [MainMethod()]
        public static void Main()
        {
            ClassFalse();

            K12.Presentation.NLDPanels.Class.SelectedSourceChanged += delegate
            {
                if (K12.Presentation.NLDPanels.Class.SelectedSource.Count <= 0)
                {
                    ClassFalse();
                }
                else
                {
                    K12.Presentation.NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["新民客制報表"][MeritDemeritName].Enable = Permissions.獎懲週報表權限 && K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0;
                    K12.Presentation.NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["新民客制報表"][CountByPeriodName].Enable = Permissions.缺曠週報表_依節次權限 && K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0;
                    K12.Presentation.NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["新民客制報表"][CountByAbsenceName].Enable = Permissions.缺曠週報表_依假別權限 && K12.Presentation.NLDPanels.Class.SelectedSource.Count > 0;
                }
            };

            //Class.Instance.RibbonBarItems["資料統計"]["報表"]["學務相關報表"]["獎懲週報表"].Enable = Permissions.獎懲週報表權限;
            K12.Presentation.NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["新民客制報表"][MeritDemeritName].Click += delegate
            {
                new K12.Behavior.WeekReport.Shinmin.獎懲週報表.Report().Print();
            };

            //Class.Instance.RibbonBarItems["資料統計"]["報表"]["學務相關報表"]["缺曠週報表(依節次)"].Enable = Permissions.缺曠週報表_依節次權限;
            K12.Presentation.NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["新民客制報表"][CountByPeriodName].Click += delegate
            {
                new K12.Behavior.WeekReport.Shinmin.缺曠週報表_依節次.Report().Print();
            };

            K12.Presentation.NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["新民客制報表"][CountByAbsenceName].Click += delegate
            {
                new K12.Behavior.WeekReport.Shinmin.缺曠週報表_依假別.Report().Print();
            };
            
            #region 註冊權限(目前依附ischool高中的xml檔案)
            Catalog ribbon = RoleAclSource.Instance["新民客製功能"]["班級報表"];
            ribbon.Add(new FISCA.Permission.RibbonFeature(Permissions.獎懲週報表, "獎懲週報表"));
            ribbon.Add(new FISCA.Permission.RibbonFeature(Permissions.缺曠週報表_依假別, "缺曠週報表_依假別"));
            ribbon.Add(new FISCA.Permission.RibbonFeature(Permissions.缺曠週報表_依節次, "缺曠週報表_依節次"));
            #endregion
        }

        private static void ClassFalse()
        {
            K12.Presentation.NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["新民客制報表"][MeritDemeritName].Enable = false;
            K12.Presentation.NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["新民客制報表"][CountByPeriodName].Enable = false;
            K12.Presentation.NLDPanels.Class.RibbonBarItems["資料統計"]["報表"]["新民客制報表"][CountByAbsenceName].Enable = false;
        }
    }
}
