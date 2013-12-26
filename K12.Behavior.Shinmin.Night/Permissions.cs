using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace K12.Behavior.Shinmin.Night
{
    /// <summary>
    /// 代表目前使用者的相關權限資訊。
    /// </summary>
    public static class Permissions
    {
        public static string 缺曠週報表_依節次_進校 { get { return "K12.Behavior.Shinmin.8000.Night"; } }
        public static bool 缺曠週報表_依節次_進校權限
        {
            get { return FISCA.Permission.UserAcl.Current[缺曠週報表_依節次_進校].Executable; }
        }

        public static string 進校學生缺席統計表 { get { return "K12.Behavior.Shinmin.2000.Night.Student"; } }

        public static bool 進校學生缺席統計表權限
        {
            get { return FISCA.Permission.UserAcl.Current[進校學生缺席統計表].Executable; }
        }

        public static string 進校班級學生缺席統計表 { get { return "K12.Behavior.Shinmin.2000.Night.Class"; } }

        public static bool 進校班級學生缺席統計表權限
        {
            get { return FISCA.Permission.UserAcl.Current[進校班級學生缺席統計表].Executable; }
        }
    }
}
