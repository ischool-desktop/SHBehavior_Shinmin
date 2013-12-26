using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Behavior.Shinmin
{
    class Permissions
    {
        public static string 班級獎懲統計表 { get { return "K12.Behavior.Shinmin.1000"; } }

        public static bool 班級獎懲統計表權限
        {
            get { return FISCA.Permission.UserAcl.Current[班級獎懲統計表].Executable; }
        }

        public static string 班級缺曠統計表 { get { return "K12.Behavior.Shinmin.2000"; } }

        public static bool 班級缺曠統計表權限
        {
            get { return FISCA.Permission.UserAcl.Current[班級缺曠統計表].Executable; }
        }

        public static string 德行特殊表現名單 { get { return "K12.Behavior.Shinmin.3000"; } }

        public static bool 德行特殊表現名單權限
        {
            get { return FISCA.Permission.UserAcl.Current[德行特殊表現名單].Executable; }
        }

        public static string 獎勵快速登錄 { get { return "K12.Behavior.Shinmin.10000"; } }

        public static bool 獎勵快速登錄權限
        {
            get { return FISCA.Permission.UserAcl.Current[獎勵快速登錄].Executable; }
        }

        public static string 懲戒快速登錄 { get { return "K12.Behavior.Shinmin.11000"; } }

        public static bool 懲戒快速登錄權限
        {
            get { return FISCA.Permission.UserAcl.Current[懲戒快速登錄].Executable; }
        }

        public static string 匯出導師註記獎懲資料 { get { return "K12.Behavior.Shinmin.ExportDiscipline"; } }

        public static bool 匯出導師註記獎懲資料權限
        {
            get { return FISCA.Permission.UserAcl.Current[匯出導師註記獎懲資料].Executable; }
        }
    }
}
