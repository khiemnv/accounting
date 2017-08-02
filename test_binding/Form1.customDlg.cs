#define use_sqlite

using System.Windows.Forms;
using System;
using System.Drawing;
using Microsoft.Reporting.WinForms;
using System.Collections.Generic;

namespace test_binding
{
    public class lEditDlg : Form
    {
        lDataPanel m_dataPanel;
        TableLayoutPanel m_tblPanel;
        protected lEditDlg(lDataPanel dataPanel)
        {
            m_dataPanel = dataPanel;

            InitializeComponent();
            m_dataPanel.initCtrls();

            m_tblPanel = new TableLayoutPanel();
            m_tblPanel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            m_tblPanel.Dock = DockStyle.Fill;
            m_tblPanel.Controls.Add(m_dataPanel.m_reloadPanel, 0, 1);
            m_tblPanel.Controls.Add(m_dataPanel.m_sumPanel, 1, 1);
            m_tblPanel.Controls.Add(m_dataPanel.m_dataGridView, 0, 2);
            m_tblPanel.SetColumnSpan(m_dataPanel.m_dataGridView, 2);

            Controls.Add(m_tblPanel);
        }
        private void InitializeComponent()
        {
            Form form = this;
            form.Location = new Point(0, 0);
            form.Size = new Size(500, 300);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            m_dataPanel.LoadData();
        }
        protected override void OnClosed(EventArgs e)
        {
            m_dataPanel.m_dataContent.m_dataTable.RejectChanges();
            base.OnClosed(e);
        }
    }

    public class lGroupNameEditDlg : lEditDlg
    {
        public lGroupNameEditDlg()
            : base(new lGroupNameDataPanel())
        {
        }
    }
    public class lReceiptsContentEditDlg : lEditDlg
    {
        public lReceiptsContentEditDlg()
            : base(new lReceiptsContentDataPanel())
        {
        }
    }
    public class lBuildingEditDlg : lEditDlg
    {
        public lBuildingEditDlg()
            : base(new lBuildingDataPanel())
        {
        }
    }

    public class lReportDlg : Form
    {
        #region gui
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
            this.paymentRadio = new System.Windows.Forms.RadioButton();
            this.buildingRadio = new System.Windows.Forms.RadioButton();
            this.remainRadio = new System.Windows.Forms.RadioButton();
            this.yearRadio = new System.Windows.Forms.RadioButton();
            this.startDate = new System.Windows.Forms.DateTimePicker();
            this.endDate = new System.Windows.Forms.DateTimePicker();
            this.paymentRptType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buildingCmb = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.printBtn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // paymentRadio
            // 
            this.paymentRadio.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.paymentRadio.AutoSize = true;
            this.paymentRadio.Checked = true;
            //this.paymentRadio.Location = new System.Drawing.Point(3, 29);
            this.paymentRadio.Name = "paymentRadio";
            //this.paymentRadio.Size = new System.Drawing.Size(82, 17);
            this.paymentRadio.TabIndex = 0;
            this.paymentRadio.TabStop = true;
            this.paymentRadio.Text = "Báo cáo chi";
            this.paymentRadio.UseVisualStyleBackColor = true;
            // 
            // buildingRadio
            // 
            this.buildingRadio.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buildingRadio.AutoSize = true;
            //this.buildingRadio.Location = new System.Drawing.Point(3, 54);
            this.buildingRadio.Name = "buildingRadio";
            //this.buildingRadio.Size = new System.Drawing.Size(73, 17);
            this.buildingRadio.TabIndex = 1;
            this.buildingRadio.TabStop = true;
            this.buildingRadio.Text = "Công trình";
            this.buildingRadio.UseVisualStyleBackColor = true;
            // 
            // remainRadio
            // 
            this.remainRadio.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.remainRadio.AutoSize = true;
            //this.remainRadio.Location = new System.Drawing.Point(3, 79);
            this.remainRadio.Name = "remainRadio";
            //this.remainRadio.Size = new System.Drawing.Size(63, 17);
            this.remainRadio.TabIndex = 2;
            this.remainRadio.TabStop = true;
            this.remainRadio.Text = "Kiểm kê";
            this.remainRadio.UseVisualStyleBackColor = true;
            // 
            // yearRadio
            // 
            this.yearRadio.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.yearRadio.AutoSize = true;
            //this.yearRadio.Location = new System.Drawing.Point(3, 104);
            this.yearRadio.Name = "yearRadio";
            //this.yearRadio.Size = new System.Drawing.Size(88, 17);
            this.yearRadio.TabIndex = 3;
            this.yearRadio.TabStop = true;
            this.yearRadio.Text = "Báo cáo năm";
            this.yearRadio.UseVisualStyleBackColor = true;
            // 
            // startDate
            // 
            this.startDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.startDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            //this.startDate.Location = new System.Drawing.Point(197, 3);
            this.startDate.Name = "startDate";
            //this.startDate.Size = new System.Drawing.Size(94, 20);
            this.startDate.TabIndex = 4;
            // 
            // endDate
            // 
            this.endDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.endDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            //this.endDate.Location = new System.Drawing.Point(97, 3);
            this.endDate.Name = "endDate";
            //this.endDate.Size = new System.Drawing.Size(94, 20);
            this.endDate.TabIndex = 5;
            // 
            // paymentRptType
            // 
            //this.paymentRptType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableLayoutPanel1.SetColumnSpan(this.paymentRptType, 2);
            paymentRptType.Dock = DockStyle.Fill;
            this.paymentRptType.FormattingEnabled = true;
            //this.paymentRptType.Location = new System.Drawing.Point(97, 28);
            this.paymentRptType.Name = "paymentRptType";
            //this.paymentRptType.Size = new System.Drawing.Size(194, 21);
            this.paymentRptType.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            //this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            //this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Ngày Tháng";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buildingCmb
            // 
            //this.buildingCmb.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableLayoutPanel1.SetColumnSpan(this.buildingCmb, 2);
            this.buildingCmb.FormattingEnabled = true;
            buildingCmb.Dock = DockStyle.Fill;
            //this.buildingCmb.Location = new System.Drawing.Point(97, 53);
            this.buildingCmb.Name = "buildingCmb";
            //this.buildingCmb.Size = new System.Drawing.Size(194, 21);
            this.buildingCmb.TabIndex = 10;
            // 
            // printBtn
            // 
            this.printBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            //this.printBtn.Location = new System.Drawing.Point(207, 128);
            this.printBtn.Name = "printBtn";
            //this.printBtn.Size = new System.Drawing.Size(75, 23);
            this.printBtn.TabIndex = 11;
            this.printBtn.Text = "Print";
            this.printBtn.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buildingCmb, 1, 2);
            //tableLayoutPanel1.SetColumnSpan(buildingCmb, 2);
            this.tableLayoutPanel1.Controls.Add(this.startDate, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.endDate, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.yearRadio, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.paymentRadio, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.remainRadio, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.paymentRptType, 1, 1);
            //tableLayoutPanel1.SetColumnSpan(paymentRptType, 2);
            this.tableLayoutPanel1.Controls.Add(this.buildingRadio, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.printBtn, 2, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            //this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            //this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            //this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            //this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            //this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            //this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(SizeType.AutoSize));
            //this.tableLayoutPanel1.Size = new System.Drawing.Size(295, 154);
            this.tableLayoutPanel1.TabIndex = 11;
            //tableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            //tableLayoutPanel1.AutoSize = true;
            // 
            // lReportDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 200);
            //this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "lReportDlg";
            this.Text = "Report";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton paymentRadio;
        private System.Windows.Forms.RadioButton buildingRadio;
        private System.Windows.Forms.RadioButton remainRadio;
        private System.Windows.Forms.RadioButton yearRadio;
        private System.Windows.Forms.DateTimePicker startDate;
        private System.Windows.Forms.DateTimePicker endDate;
        private System.Windows.Forms.ComboBox paymentRptType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox buildingCmb;
        private System.Windows.Forms.Button printBtn;
        #endregion

        enum receiptsRptType
        {
            byDays,
            byWeek,
            byMonth,
            byYear,
        }
        Dictionary<receiptsRptType, string> m_receiptRptTypes;

        public lReportDlg()
        {
            InitializeComponent();

            startDate.CustomFormat = lConfigMng.getDateFormat();
            endDate.CustomFormat = lConfigMng.getDateFormat();

            m_receiptRptTypes = new Dictionary<receiptsRptType, string> {
                {receiptsRptType.byDays, "Báo cáo theo ngày" },
                {receiptsRptType.byWeek, "Báo cáo theo tuần" },
                {receiptsRptType.byMonth, "Báo cáo theo tháng" },
            };
            foreach (var val in m_receiptRptTypes.Values)
            {
                paymentRptType.Items.Add(val);
            }
            paymentRptType.SelectedIndex = 0;

            printBtn.Click += PrintBtn_Click;
            Load += LReportDlg_Load;

            //bao cao kiem ke & bao cao nam
            remainRadio.Hide();
            yearRadio.Hide();

            //set font
            foreach (Control crt in Controls)
            {
                crt.Font = lConfigMng.getFont();
            }
        }

        private void PrintBtn_Click(object sender, EventArgs e)
        {
            lBaseReport rpt = null;
            if (paymentRadio.Checked)
            {
                switch (paymentRptType.SelectedIndex)
                {
                    case (int)receiptsRptType.byDays:
#if use_sqlite
                        rpt = new lDaysReport(startDate.Value, endDate.Value);
#else
                        rpt = new lSqlDaysReport(startDate.Value, endDate.Value);
#endif
                        break;
                    case (int)receiptsRptType.byWeek:
#if use_sqlite
                        rpt = new lWeekReport(startDate.Value, endDate.Value);
#else
                        rpt = new lSqlWeekReport(startDate.Value, endDate.Value);
#endif
                        break;
                    case (int)receiptsRptType.byMonth:
#if use_sqlite
                        rpt = new lMonthReport(startDate.Value, endDate.Value);
#else
                        rpt = new lSqlMonthReport(startDate.Value, endDate.Value);
#endif
                        break;
                }
            }
            else if (buildingRadio.Checked)
            {
                rpt = new lBuildingReport(buildingCmb.Text, startDate.Value, endDate.Value);
            }
            if (rpt != null)
            {
                rpt.Run();
                rpt.Dispose();
            }
            Close();
        }

        private void LReportDlg_Load(object sender, EventArgs e)
        {
            //load data for building combo box
            BindingSource bs = new BindingSource();
            lDataContent dc = appConfig.s_contentProvider.CreateDataContent("building");
            bs.DataSource = dc.m_dataTable;
            buildingCmb.DataSource = bs;
            buildingCmb.DisplayMember = dc.m_dataTable.Columns[1].ColumnName;
        }
    }
}
