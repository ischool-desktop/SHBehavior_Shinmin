using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using FISCA.DSAUtil;
using FISCA.Presentation.Controls;

namespace K12.Keyboard.Shinmin
{
    class PeriodHelper
    {
        ///// <summary>
        ///// 名稱,熱鍵
        ///// </summary>
        //public Dictionary<string, string> _GetPeriodCodes = new Dictionary<string, string>();

        public Dictionary<int, string> _GetPeriodDic = new Dictionary<int, string>();
        ///// <summary>
        ///// 熱鍵,名稱
        ///// </summary>
        //public Dictionary<string, string> _GetCodesPeriod = new Dictionary<string, string>();
        /// <summary>
        /// 名稱,類別
        /// </summary>
        public Dictionary<string, string> _GetPeriodType = new Dictionary<string, string>();

        /// <summary>
        /// 建立具有各種節次字典的物件
        /// </summary>
        public PeriodHelper()
        {
            DSResponse dsrsp_2 = Config.GetPeriodList();
            DSXmlHelper helper_2 = dsrsp_2.GetContent();
            foreach (XmlElement element in helper_2.GetElements("Period"))
            {
                PeriodInfo info = new PeriodInfo(element);
                if (CheckPeriod(info)) //檢查
                {
                    if (!_GetPeriodType.ContainsKey(info.Name))
                    {
                        _GetPeriodType.Add(info.Name, info.Type);   //名稱,類別
                    }
                    if (!_GetPeriodDic.ContainsKey(info.Sort)) //順序,名稱
                    {
                        _GetPeriodDic.Add(info.Sort, info.Name);
                    }
                }
                else
                {
                    MsgBox.Show("節次熱鍵有誤,請確認節次熱鍵");
                }
            }
        }

        private bool CheckPeriod(PeriodInfo Peroi)
        {
            #region 檢查每日節次是否有值
            if (Peroi.Name == string.Empty || Peroi.Type == string.Empty)
            {
                return false;
            }
            return true;
            #endregion
        }

        ///// <summary>
        ///// 傳入string,確認是否為Code熱鍵
        ///// </summary>
        ///// <returns></returns>
        //public bool NameKey(char Key)
        //{
        //    return _GetCodesPeriod.ContainsKey("" + Key);
        //}
    }
}
