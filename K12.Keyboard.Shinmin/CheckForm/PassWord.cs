using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;

namespace K12.Keyboard.Shinmin
{
    public partial class PassWord : BaseForm
    {
        public PassWord()
        {
            InitializeComponent();

            textBoxX1.Focus();
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            if (textBoxX1.Text == "Shinmin-Admin")
            {
                this.DialogResult = DialogResult.Yes;
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }
    }
}
