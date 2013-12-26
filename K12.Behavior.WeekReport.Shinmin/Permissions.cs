using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Behavior.WeekReport.Shinmin
{
    /// <summary>
    /// 代表目前使用者的相關權限資訊。
    /// </summary>
    public static class Permissions
    {
        //毛毛蟲權限(對應於毛毛蟲內之權限字串)
        //JHSchool.Class.Report0060
        public static string 獎懲週報表 { get { return "K12.Behavior.Shinmin.7000"; } }
        public static bool 獎懲週報表權限
        {
            get { return FISCA.Permission.UserAcl.Current[獎懲週報表].Executable; }
        }


        //JHSchool.Class.Report0100
        public static string 缺曠週報表_依節次 { get { return "K12.Behavior.Shinmin.8000"; } }
        public static bool 缺曠週報表_依節次權限
        {
            get { return FISCA.Permission.UserAcl.Current[缺曠週報表_依節次].Executable; }
        }

        //JHSchool.Class.Report0110
        public static string 缺曠週報表_依假別 { get { return "K12.Behavior.Shinmin.9000"; } }

        public static bool 缺曠週報表_依假別權限
        {
            get { return FISCA.Permission.UserAcl.Current[缺曠週報表_依假別].Executable; }
        }
    }
}
