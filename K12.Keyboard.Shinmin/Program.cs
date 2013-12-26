using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA;
using FISCA.Presentation;
using FISCA.Permission;

namespace K12.Keyboard.Shinmin
{
    public static class Program
    {
        [MainMethod()]
        public static void Main()
        {
            RibbonBarItem KBKeyIn = FISCA.Presentation.MotherForm.RibbonBarItems["學務作業", "鍵盤作業"];

            KBKeyIn["缺曠鍵盤(Shinmin)"].Enable = FISCA.Permission.UserAcl.Current["K12.Behavior.Shinmin.5000"].Executable;
            KBKeyIn["缺曠鍵盤(Shinmin)"].Image = Properties.Resources.polygon_64;
            KBKeyIn["缺曠鍵盤(Shinmin)"].Click += delegate
            {
                new RtAttendanceKBInput().ShowDialog();
            };


            KBKeyIn["獎懲鍵盤(Shinmin)"].Enable = FISCA.Permission.UserAcl.Current["K12.Behavior.Shinmin.6000"].Executable;
            KBKeyIn["獎懲鍵盤(Shinmin)"].Image = Properties.Resources.star_64;
            KBKeyIn["獎懲鍵盤(Shinmin)"].Click += delegate
            {
                if (Control.ModifierKeys == Keys.Shift)
                {
                    new TeacherPointCheck().ShowDialog();
                }
                else
                {
                    new PdMeritDemeritKBInput().ShowDialog();
                }
            };

            Catalog ribbon = RoleAclSource.Instance["新民客製功能"]["鍵盤化輸入"];
            ribbon.Add(new RibbonFeature("K12.Behavior.Shinmin.5000", "缺曠鍵盤(Shinmin)"));
            ribbon.Add(new RibbonFeature("K12.Behavior.Shinmin.6000", "獎懲鍵盤(Shinmin)"));
        }
    }
}
