using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FISCA.DSAUtil;
using System.Xml;
using DevComponents.DotNetBar.Controls;
using DevComponents.DotNetBar;
using FISCA.LogAgent;
using K12.Data.Utility;
using FISCA.Presentation.Controls;

namespace K12.Behavior.Shinmin.StudentsSpecial
{
    public partial class ReduceForm : FISCA.Presentation.Controls.BaseForm
    {
        StringBuilder sb = new StringBuilder();

        public ReduceForm()
        {
            InitializeComponent();
        }

        private void ReduceForm_Load(object sender, EventArgs e)
        {
            DSResponse dsrsp = Config.GetMDReduce();
            if (!dsrsp.HasContent)
            {
                FISCA.Presentation.Controls.MsgBox.Show("���o��Ӫ��� : " + dsrsp.GetFault().Message, "���~", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DSXmlHelper helper = dsrsp.GetContent();
            txtMAB.Text = helper.GetText("Merit/AB");
            txtMBC.Text = helper.GetText("Merit/BC");
            txtDAB.Text = helper.GetText("Demerit/AB");
            txtDBC.Text = helper.GetText("Demerit/BC");

            sb.AppendLine("�u�\�L�����v�w�Q�ק�C");
            sb.AppendLine("�ק�e�G");
            sb.AppendLine("�u1�j�\�v����u" + txtMAB.Text + "�p�\�v");
            sb.AppendLine("�u1�p�\�v����u" + txtMBC.Text + "�ż��v");
            sb.AppendLine("�u1�j�L�v����u" + txtDAB.Text + "�p�L�v");
            sb.AppendLine("�u1�p�L�v����u" + txtDBC.Text + "�ż��v");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!IsValid()) return;
            //���g��촫���
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("Reduce");
            doc.AppendChild(root);

            XmlElement element = doc.CreateElement("Merit");
            root.AppendChild(element);
            XmlElement ab = doc.CreateElement("AB");
            element.AppendChild(ab);
            ab.InnerText = txtMAB.Text;
            XmlElement bc = doc.CreateElement("BC");
            element.AppendChild(bc);
            bc.InnerText = txtMBC.Text;

            element = doc.CreateElement("Demerit");
            root.AppendChild(element);
            ab = doc.CreateElement("AB");
            element.AppendChild(ab);
            ab.InnerText = txtDAB.Text;
            bc = doc.CreateElement("BC");
            element.AppendChild(bc);
            bc.InnerText = txtDBC.Text;

            sb.AppendLine("�ק��G");
            sb.AppendLine("�u1�j�\�v����u" + txtMAB.Text + "�p�\�v");
            sb.AppendLine("�u1�p�\�v����u" + txtMBC.Text + "�ż��v");
            sb.AppendLine("�u1�j�L�v����u" + txtDAB.Text + "�p�L�v");
            sb.AppendLine("�u1�p�L�v����u" + txtDBC.Text + "�ż��v");

            try
            {
                DSXmlHelper helper = new DSXmlHelper("Lists");
                helper.AddElement("List");
                helper.AddElement("List", "Content");
                helper.AddElement("List/Content", doc.DocumentElement);
                helper.AddElement("List", "Condition");
                helper.AddElement("List/Condition", "Name", "���g��촫���");
                Config.Update(new DSRequest(helper));
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show("�x�s���� : " + ex.Message, "���~", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ApplicationLog.Log("�ǰȨt��.�\�L����޲z", "�ק�\�L����", sb.ToString());
            MsgBox.Show("�x�s���\!", "����", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private bool IsValid()
        {
            error.Clear();
            error.Tag = true;
            ValidInt(txtMAB, lblMAB);
            ValidInt(txtMBC, lblMBC);
            ValidInt(txtDAB, lblDAB);
            ValidInt(txtDBC, lblDBC);
            return bool.Parse(error.Tag.ToString());
        }

        private void ValidInt(TextBoxX txt, LabelX lbl)
        {
            int i;
            if (!int.TryParse(txt.Text, out i))
            {
                error.Tag = false;
                error.SetError(lbl, "�������Ʀr");
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}