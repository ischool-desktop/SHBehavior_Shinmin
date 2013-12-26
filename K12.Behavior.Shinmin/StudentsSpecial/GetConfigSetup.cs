using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA.DSAUtil;
using System.Windows.Forms;

namespace K12.Behavior.Shinmin.StudentsSpecial
{
    class GetConfigSetup
    {
        //由此取得設定檔

        public int MeritAtoB { get; set; }
        public int MeritBtoC { get; set; }
        public int DemeritAtoB { get; set; }
        public int DemeritBtoC { get; set; }

        public GetConfigSetup()
        {
            DSResponse dsrsp = Config.GetMDReduce();
            if (!dsrsp.HasContent)
            {
                FISCA.Presentation.Controls.MsgBox.Show("取得對照表失敗 : " + dsrsp.GetFault().Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MeritAtoB = 3;
                MeritBtoC = 3;
                DemeritAtoB = 3;
                DemeritBtoC = 3;
            }
            else
            {
                DSXmlHelper helper = dsrsp.GetContent();
                MeritAtoB = int.Parse(helper.GetText("Merit/AB"));
                MeritBtoC = int.Parse(helper.GetText("Merit/BC"));
                DemeritAtoB = int.Parse(helper.GetText("Demerit/AB"));
                DemeritBtoC = int.Parse(helper.GetText("Demerit/BC"));
            }
        }
    }
}
