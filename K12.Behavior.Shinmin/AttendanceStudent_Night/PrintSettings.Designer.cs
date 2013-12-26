namespace K12.Behavior.Shinmin.AttendanceStatistics_進校
{
    partial class PrintSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.lvPeriod = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.checkBoxX1 = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.lvAbsence = new DevComponents.DotNetBar.Controls.ListViewEx();
            this.cbAbsence = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.cbPeriod = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.gpPeriod = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.gpAbsence = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.gpPeriod.SuspendLayout();
            this.gpAbsence.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.AutoSize = true;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(339, 334);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 25);
            this.btnExit.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnExit.TabIndex = 6;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.AutoSize = true;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(258, 334);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 25);
            this.btnSave.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "儲存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lvPeriod
            // 
            // 
            // 
            // 
            this.lvPeriod.Border.Class = "ListViewBorder";
            this.lvPeriod.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lvPeriod.CheckBoxes = true;
            this.lvPeriod.Location = new System.Drawing.Point(6, 5);
            this.lvPeriod.Name = "lvPeriod";
            this.lvPeriod.Size = new System.Drawing.Size(172, 240);
            this.lvPeriod.TabIndex = 2;
            this.lvPeriod.UseCompatibleStateImageBehavior = false;
            this.lvPeriod.View = System.Windows.Forms.View.List;
            // 
            // checkBoxX1
            // 
            this.checkBoxX1.AutoSize = true;
            this.checkBoxX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.checkBoxX1.BackgroundStyle.Class = "";
            this.checkBoxX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.checkBoxX1.Location = new System.Drawing.Point(17, 338);
            this.checkBoxX1.Name = "checkBoxX1";
            this.checkBoxX1.Size = new System.Drawing.Size(164, 21);
            this.checkBoxX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.checkBoxX1.TabIndex = 4;
            this.checkBoxX1.Text = "略過星期六,星期日資料";
            // 
            // lvAbsence
            // 
            // 
            // 
            // 
            this.lvAbsence.Border.Class = "ListViewBorder";
            this.lvAbsence.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lvAbsence.CheckBoxes = true;
            this.lvAbsence.Location = new System.Drawing.Point(11, 5);
            this.lvAbsence.Name = "lvAbsence";
            this.lvAbsence.Size = new System.Drawing.Size(172, 240);
            this.lvAbsence.TabIndex = 3;
            this.lvAbsence.UseCompatibleStateImageBehavior = false;
            this.lvAbsence.View = System.Windows.Forms.View.List;
            // 
            // cbAbsence
            // 
            this.cbAbsence.AutoSize = true;
            this.cbAbsence.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.cbAbsence.BackgroundStyle.Class = "";
            this.cbAbsence.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbAbsence.Location = new System.Drawing.Point(11, 251);
            this.cbAbsence.Name = "cbAbsence";
            this.cbAbsence.Size = new System.Drawing.Size(54, 21);
            this.cbAbsence.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbAbsence.TabIndex = 7;
            this.cbAbsence.Text = "全選";
            this.cbAbsence.CheckedChanged += new System.EventHandler(this.cbAbsence_CheckedChanged);
            // 
            // cbPeriod
            // 
            this.cbPeriod.AutoSize = true;
            this.cbPeriod.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.cbPeriod.BackgroundStyle.Class = "";
            this.cbPeriod.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbPeriod.Location = new System.Drawing.Point(6, 251);
            this.cbPeriod.Name = "cbPeriod";
            this.cbPeriod.Size = new System.Drawing.Size(54, 21);
            this.cbPeriod.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbPeriod.TabIndex = 8;
            this.cbPeriod.Text = "全選";
            this.cbPeriod.CheckedChanged += new System.EventHandler(this.cbPeriod_CheckedChanged);
            // 
            // gpPeriod
            // 
            this.gpPeriod.BackColor = System.Drawing.Color.Transparent;
            this.gpPeriod.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpPeriod.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpPeriod.Controls.Add(this.cbPeriod);
            this.gpPeriod.Controls.Add(this.lvPeriod);
            this.gpPeriod.Location = new System.Drawing.Point(223, 12);
            this.gpPeriod.Name = "gpPeriod";
            this.gpPeriod.Size = new System.Drawing.Size(191, 304);
            // 
            // 
            // 
            this.gpPeriod.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpPeriod.Style.BackColorGradientAngle = 90;
            this.gpPeriod.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpPeriod.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpPeriod.Style.BorderBottomWidth = 1;
            this.gpPeriod.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpPeriod.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpPeriod.Style.BorderLeftWidth = 1;
            this.gpPeriod.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpPeriod.Style.BorderRightWidth = 1;
            this.gpPeriod.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpPeriod.Style.BorderTopWidth = 1;
            this.gpPeriod.Style.Class = "";
            this.gpPeriod.Style.CornerDiameter = 4;
            this.gpPeriod.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpPeriod.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpPeriod.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.gpPeriod.StyleMouseDown.Class = "";
            this.gpPeriod.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.gpPeriod.StyleMouseOver.Class = "";
            this.gpPeriod.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.gpPeriod.TabIndex = 9;
            this.gpPeriod.Text = "統計節次設定：";
            // 
            // gpAbsence
            // 
            this.gpAbsence.BackColor = System.Drawing.Color.Transparent;
            this.gpAbsence.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpAbsence.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpAbsence.Controls.Add(this.lvAbsence);
            this.gpAbsence.Controls.Add(this.cbAbsence);
            this.gpAbsence.Location = new System.Drawing.Point(17, 12);
            this.gpAbsence.Name = "gpAbsence";
            this.gpAbsence.Size = new System.Drawing.Size(200, 304);
            // 
            // 
            // 
            this.gpAbsence.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpAbsence.Style.BackColorGradientAngle = 90;
            this.gpAbsence.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpAbsence.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpAbsence.Style.BorderBottomWidth = 1;
            this.gpAbsence.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpAbsence.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpAbsence.Style.BorderLeftWidth = 1;
            this.gpAbsence.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpAbsence.Style.BorderRightWidth = 1;
            this.gpAbsence.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpAbsence.Style.BorderTopWidth = 1;
            this.gpAbsence.Style.Class = "";
            this.gpAbsence.Style.CornerDiameter = 4;
            this.gpAbsence.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpAbsence.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpAbsence.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            // 
            // 
            // 
            this.gpAbsence.StyleMouseDown.Class = "";
            this.gpAbsence.StyleMouseDown.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            // 
            // 
            // 
            this.gpAbsence.StyleMouseOver.Class = "";
            this.gpAbsence.StyleMouseOver.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.gpAbsence.TabIndex = 10;
            this.gpAbsence.Text = "列印缺曠設定：";
            // 
            // PrintSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(442, 373);
            this.Controls.Add(this.gpAbsence);
            this.Controls.Add(this.gpPeriod);
            this.Controls.Add(this.checkBoxX1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSave);
            this.Name = "PrintSettings";
            this.Text = "列印設定";
            this.Load += new System.EventHandler(this.PrintSettings_Load);
            this.gpPeriod.ResumeLayout(false);
            this.gpPeriod.PerformLayout();
            this.gpAbsence.ResumeLayout(false);
            this.gpAbsence.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.Controls.ListViewEx lvPeriod;
        private DevComponents.DotNetBar.Controls.CheckBoxX checkBoxX1;
        private DevComponents.DotNetBar.Controls.ListViewEx lvAbsence;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbAbsence;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbPeriod;
        private DevComponents.DotNetBar.Controls.GroupPanel gpPeriod;
        private DevComponents.DotNetBar.Controls.GroupPanel gpAbsence;
    }
}