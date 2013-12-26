using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;
using K12.Data.Configuration;
using Campus.Report;

namespace K12.Behavior.Shinmin.MeritDemeritStatistics
{
    //設定檔物件
    class GetConfigSetup
    {
        //設定檔代碼
        string SetupCode = "K12.Behavior.Shinmin.MeritDemeritStatistics";
        //設定檔
        Campus.Configuration.ConfigData cd;

        #region 預設設定檔
        int _班級基本分 = 80;
        string Code1 = "班級基本分";
        public int 班級基本分
        {
            get { return _班級基本分; }
            set { _班級基本分 = value; }
        }

        bool _啟用總分100分限制 = true;
        string Code2 = "啟用總分100分限制";
        public bool 啟用總分100分限制
        {
            get { return _啟用總分100分限制; }
            set { _啟用總分100分限制 = value; }
        }

        bool _略過班導師註記 = true;
        string Code3 = "略過班導師註記";
        public bool 略過班導師註記
        {
            get { return _略過班導師註記; }
            set { _略過班導師註記 = value; }
        }

        int _大功 = 9;
        string MeritA = "大功";
        public int 大功
        {
            get { return _大功; }
            set { _大功 = value; }
        }

        int _小功 = 3;
        string MeritB = "小功";
        public int 小功
        {
            get { return _小功; }
            set { _小功 = value; }
        }

        int _嘉獎 = 1;
        string MeritC = "嘉獎";
        public int 嘉獎
        {
            get { return _嘉獎; }
            set { _嘉獎 = value; }
        }

        int _大過 = -9;
        string DemeritA = "大過";
        public int 大過
        {
            get { return _大過; }
            set { _大過 = value; }
        }

        int _小過 = -3;
        string DemeritB = "小過";
        public int 小過
        {
            get { return _小過; }
            set { _小過 = value; }
        }

        int _警告 = -1;
        string DemeritC = "警告";
        public int 警告
        {
            get { return _警告; }
            set { _警告 = value; }
        }
        #endregion

        public GetConfigSetup()
        {
            //取得設定檔
            cd = Campus.Configuration.Config.User[SetupCode];

            #region 讀取設定檔
            if (!string.IsNullOrEmpty(cd[Code1]))
            {
                int.TryParse(cd[Code1], out _班級基本分);
            }

            //啟用總分100分限制
            if (!string.IsNullOrEmpty(cd[Code2]))
            {
                bool.TryParse(cd[Code2], out _啟用總分100分限制);
            }

            //略過班導師註記
            if (!string.IsNullOrEmpty(cd[Code3]))
            {
                bool.TryParse(cd[Code3], out _略過班導師註記);
            }

            //加扣分標準
            if (!string.IsNullOrEmpty(cd[MeritA]))
            {
                int.TryParse(cd[MeritA], out _大功);
            }

            if (!string.IsNullOrEmpty(cd[MeritB]))
            {
                int.TryParse(cd[MeritB], out _小功);
            }

            if (!string.IsNullOrEmpty(cd[MeritC]))
            {
                int.TryParse(cd[MeritC], out _嘉獎);
            }

            if (!string.IsNullOrEmpty(cd[DemeritA]))
            {
                int.TryParse(cd[DemeritA], out _大過);
            }

            if (!string.IsNullOrEmpty(cd[DemeritB]))
            {
                int.TryParse(cd[DemeritB], out _小過);
            }

            if (!string.IsNullOrEmpty(cd[DemeritC]))
            {
                int.TryParse(cd[DemeritC], out _警告);
            }
            #endregion
        }

        //儲存設定檔
        public void SaveConfigSetup()
        {            
            cd = Campus.Configuration.Config.User[SetupCode];
            cd[Code1] = _班級基本分.ToString();
            cd[Code2] = _啟用總分100分限制.ToString();
            cd[Code3] = _略過班導師註記.ToString();
            cd[MeritA] = _大功.ToString();
            cd[MeritB] = _小功.ToString();
            cd[MeritC] = _嘉獎.ToString();
            cd[DemeritA] = _大過.ToString();
            cd[DemeritB] = _小過.ToString();
            cd[DemeritC] = _警告.ToString();
            cd.Save();
        }
    }
}
