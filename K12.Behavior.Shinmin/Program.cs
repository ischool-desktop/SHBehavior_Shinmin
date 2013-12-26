using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using FISCA.Presentation;
using FISCA.Permission;
using FISCA.Presentation.Controls;
using K12.Behavior.Shinmin.MeritDemeritStatistics;
using System.Windows.Forms;
using K12.Behavior.Shinmin.AttendanceStatistics;
using K12.Behavior.Shinmin.StudentsSpecial;
using K12.Data;

namespace K12.Behavior.Shinmin
{
    public static class Program
    {
        [MainMethod()]
        public static void Main()
        {
            //if (Control.ModifierKeys == Keys.Shift)
            //{
            //}

            RibbonBarButton rbItemExport = K12.Presentation.NLDPanels.Student.RibbonBarItems["資料統計"]["匯出"];
            rbItemExport["學務相關匯出"]["匯出獎懲記錄(導師註記)"].Enable = Permissions.匯出導師註記獎懲資料權限;
            rbItemExport["學務相關匯出"]["匯出獎懲記錄(導師註記)"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new ExportDiscipline();
                ExportStudentV2 wizard = new ExportStudentV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };

            RibbonBarItem rbItemClass = MotherForm.RibbonBarItems["班級", "資料統計"];

            //導師註記報表
            rbItemClass["報表"]["新民客制報表"]["班級獎懲統計表"].Enable = Permissions.班級獎懲統計表權限;
            rbItemClass["報表"]["新民客制報表"]["班級獎懲統計表"].Click += delegate
            {
                MeritDemeritForm mdf = new MeritDemeritForm(true);
                mdf.ShowDialog();
            };

            rbItemClass["報表"]["新民客制報表"]["班級缺曠統計表"].Enable = Permissions.班級缺曠統計表權限;
            rbItemClass["報表"]["新民客制報表"]["班級缺曠統計表"].Click += delegate
            {
                AttendanceForm att = new AttendanceForm();
                att.ShowDialog();
            };

            rbItemClass["報表"]["新民客制報表"]["德行特殊表現名單"].Enable = Permissions.德行特殊表現名單權限;
            rbItemClass["報表"]["新民客制報表"]["德行特殊表現名單"].Click += delegate
            {
                StudentsSpecialForm ssf = new StudentsSpecialForm();
                ssf.ShowDialog();
            };

            RibbonBarItem rbItemStudent = MotherForm.RibbonBarItems["學生", "新民客制功能"];

            rbItemStudent["獎勵快速登錄(導師註記)"].Enable = Permissions.獎勵快速登錄權限;
            rbItemStudent["獎勵快速登錄(導師註記)"].Click += delegate
            {
                MutiMeritDemerit editor = new MutiMeritDemerit("獎勵");
                editor.ShowDialog();
            };

            rbItemStudent["懲戒快速登錄(導師註記)"].Enable = Permissions.懲戒快速登錄權限;
            rbItemStudent["懲戒快速登錄(導師註記)"].Click += delegate
            {
                MutiMeritDemerit editor = new MutiMeritDemerit("懲戒");
                editor.ShowDialog();
            };

            Catalog ribbon1 = RoleAclSource.Instance["新民客製功能"]["學生功能"];
            ribbon1.Add(new FISCA.Permission.RibbonFeature(Permissions.獎勵快速登錄, "獎勵快速登錄(導師註記)"));
            ribbon1.Add(new FISCA.Permission.RibbonFeature(Permissions.懲戒快速登錄, "懲戒快速登錄(導師註記)"));
            ribbon1.Add(new FISCA.Permission.RibbonFeature(Permissions.匯出導師註記獎懲資料, "匯出獎懲記錄(導師註記)"));

            ribbon1 = RoleAclSource.Instance["新民客製功能"]["班級報表"];
            ribbon1.Add(new FISCA.Permission.RibbonFeature(Permissions.班級獎懲統計表, "班級獎懲統計表"));
            ribbon1.Add(new FISCA.Permission.RibbonFeature(Permissions.班級缺曠統計表, "班級缺曠統計表"));
            ribbon1.Add(new FISCA.Permission.RibbonFeature(Permissions.德行特殊表現名單, "德行特殊表現名單"));
        
        }
    }
}
