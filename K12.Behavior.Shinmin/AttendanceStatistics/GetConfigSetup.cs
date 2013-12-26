using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace K12.Behavior.Shinmin.AttendanceStatistics
{
    class GetConfigSetup
    {
        //基本設定檔
        string SetupCode = "K12.Behavior.Shinmin.AttendanceStatistics.SetupCode";
        //缺曠設定檔
        string AbsenceCode = "K12.Behavior.Shinmin.AttendanceStatistics.AbsenceCode";
        //節次設定檔
        string PeriodCode = "K12.Behavior.Shinmin.AttendanceStatistics.PeriodCode";
        //設定
        Campus.Configuration.ConfigData cd;

        //列印缺曠別 (曠課/bool)
        public Dictionary<string, bool> AbsenceDic = new Dictionary<string, bool>();
        //節次類型 (七/bool)
        public Dictionary<string, bool> PeriodDic = new Dictionary<string, bool>();

        public List<string> AbsenceList = new List<string>();
        public List<string> PeriodList = new List<string>();

        bool _略過六日資料 = true;
        string Code1 = "略過六日資料";
        public bool 略過六日資料
        {
            get { return _略過六日資料; }
            set { _略過六日資料 = value; }
        }

        public GetConfigSetup()
        {
            //取得設定檔
            cd = Campus.Configuration.Config.User[SetupCode];

            //略過星期六,星期日資料
            if (!string.IsNullOrEmpty(cd[Code1]))
            {
                bool.TryParse(cd[Code1], out _略過六日資料);
            }

            //取得目前缺曠別
            cd = Campus.Configuration.Config.User[AbsenceCode];
            foreach(AbsenceMappingInfo each in AbsenceMapping.SelectAll())
            {
                if (!string.IsNullOrEmpty(cd[each.Name]))
                {
                    bool Absence = false; //預設為false
                    bool.TryParse(cd[each.Name], out Absence); //能轉換則填入Absence
                    AbsenceDic.Add(each.Name, Absence); //新增資料內容   
                    if (Absence)
                    {
                        AbsenceList.Add(each.Name);
                    }
                }
                else //不存在的缺曠別,預設為false
                {
                    AbsenceDic.Add(each.Name, false);
                }
            }

            //取得目前節次別
            cd = Campus.Configuration.Config.User[PeriodCode];
            foreach (PeriodMappingInfo each in PeriodMapping.SelectAll())
            {
                if (!string.IsNullOrEmpty(cd[each.Name]))
                {
                    bool Period = false; //預設為false
                    bool.TryParse(cd[each.Name], out Period); //能轉換則填入Absence
                    PeriodDic.Add(each.Name, Period); //新增資料內容  
                    if (Period)
                    {
                        PeriodList.Add(each.Name);
                    }
                }
                else //不存在的缺曠別,預設為false
                {
                    PeriodDic.Add(each.Name, false);
                }
            }

        }

        //儲存設定檔
        public void SaveConfigSetup()
        {
            cd = Campus.Configuration.Config.User[SetupCode];
            cd[Code1] = _略過六日資料.ToString();
            cd.Save();

            cd = Campus.Configuration.Config.User[AbsenceCode];
            foreach (string each in AbsenceDic.Keys)
            {
                cd[each] = AbsenceDic[each].ToString();
            }
            cd.Save();

            cd = Campus.Configuration.Config.User[PeriodCode];
            foreach (string each in PeriodDic.Keys)
            {
                cd[each] = PeriodDic[each].ToString();
            }
            cd.Save();
        }
    }
}
