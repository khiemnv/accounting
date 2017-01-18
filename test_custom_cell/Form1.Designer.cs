using System;
using System.Windows.Forms;

namespace CBV_KeToan
{
    partial class Form1
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
            this.Receipts = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dGV_receipt = new System.Windows.Forms.DataGridView();
            this.btn_apply = new System.Windows.Forms.Button();
            this.btn_addNew = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_Search = new System.Windows.Forms.Button();
            this.tB_receipt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lable_to = new System.Windows.Forms.Label();
            this.dTP_endDate = new System.Windows.Forms.DateTimePicker();
            this.dTP_startDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.content_cmb = new System.Windows.Forms.ComboBox();
            this.content_txt = new System.Windows.Forms.TextBox();
            this.Receipts.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGV_receipt)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Receipts
            // 
            this.Receipts.Controls.Add(this.tabPage1);
            this.Receipts.Controls.Add(this.tabPage2);
            this.Receipts.Location = new System.Drawing.Point(12, 12);
            this.Receipts.Name = "Receipts";
            this.Receipts.SelectedIndex = 0;
            this.Receipts.Size = new System.Drawing.Size(683, 402);
            this.Receipts.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.content_txt);
            this.tabPage1.Controls.Add(this.dGV_receipt);
            this.tabPage1.Controls.Add(this.btn_apply);
            this.tabPage1.Controls.Add(this.btn_addNew);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(675, 376);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dGV_receipt
            // 
            this.dGV_receipt.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGV_receipt.Location = new System.Drawing.Point(6, 181);
            this.dGV_receipt.Name = "dGV_receipt";
            this.dGV_receipt.Size = new System.Drawing.Size(663, 189);
            this.dGV_receipt.TabIndex = 4;
            // 
            // btn_apply
            // 
            this.btn_apply.Location = new System.Drawing.Point(93, 150);
            this.btn_apply.Name = "btn_apply";
            this.btn_apply.Size = new System.Drawing.Size(99, 23);
            this.btn_apply.TabIndex = 3;
            this.btn_apply.Text = "Apply Changes";
            this.btn_apply.UseVisualStyleBackColor = true;
            // 
            // btn_addNew
            // 
            this.btn_addNew.Location = new System.Drawing.Point(6, 150);
            this.btn_addNew.Name = "btn_addNew";
            this.btn_addNew.Size = new System.Drawing.Size(81, 25);
            this.btn_addNew.TabIndex = 2;
            this.btn_addNew.Text = "Adding New";
            this.btn_addNew.UseVisualStyleBackColor = true;
            this.btn_addNew.Click += new System.EventHandler(this.btn_addNew_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.content_cmb);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btn_Search);
            this.groupBox1.Controls.Add(this.tB_receipt);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.lable_to);
            this.groupBox1.Controls.Add(this.dTP_endDate);
            this.groupBox1.Controls.Add(this.dTP_startDate);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(260, 123);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "noi dung";
            // 
            // btn_Search
            // 
            this.btn_Search.Location = new System.Drawing.Point(72, 96);
            this.btn_Search.Name = "btn_Search";
            this.btn_Search.Size = new System.Drawing.Size(76, 26);
            this.btn_Search.TabIndex = 6;
            this.btn_Search.Text = "Search";
            this.btn_Search.UseVisualStyleBackColor = true;
            this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
            // 
            // tB_receipt
            // 
            this.tB_receipt.Location = new System.Drawing.Point(82, 48);
            this.tB_receipt.Name = "tB_receipt";
            this.tB_receipt.Size = new System.Drawing.Size(144, 20);
            this.tB_receipt.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Phieu Chi";
            // 
            // lable_to
            // 
            this.lable_to.AutoSize = true;
            this.lable_to.Location = new System.Drawing.Point(133, 19);
            this.lable_to.Name = "lable_to";
            this.lable_to.Size = new System.Drawing.Size(16, 13);
            this.lable_to.TabIndex = 3;
            this.lable_to.Text = "to";
            // 
            // dTP_endDate
            // 
            this.dTP_endDate.Location = new System.Drawing.Point(155, 17);
            this.dTP_endDate.Name = "dTP_endDate";
            this.dTP_endDate.Size = new System.Drawing.Size(72, 20);
            this.dTP_endDate.TabIndex = 2;
            // 
            // dTP_startDate
            // 
            this.dTP_startDate.Location = new System.Drawing.Point(55, 17);
            this.dTP_startDate.Name = "dTP_startDate";
            this.dTP_startDate.Size = new System.Drawing.Size(70, 20);
            this.dTP_startDate.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ngay";
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(675, 376);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // content_cmb
            // 
            this.content_cmb.FormattingEnabled = true;
            this.content_cmb.Location = new System.Drawing.Point(81, 73);
            this.content_cmb.Name = "content_cmb";
            this.content_cmb.Size = new System.Drawing.Size(146, 21);
            this.content_cmb.TabIndex = 9;
            // 
            // content_txt
            // 
            this.content_txt.Location = new System.Drawing.Point(283, 80);
            this.content_txt.Name = "content_txt";
            this.content_txt.Size = new System.Drawing.Size(136, 20);
            this.content_txt.TabIndex = 5;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(707, 426);
            this.Controls.Add(this.Receipts);
            this.Name = "Form1";
            this.Text = "CBV_KeToan";
            this.Receipts.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dGV_receipt)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }
    

        #endregion

        private System.Windows.Forms.TabControl Receipts;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btn_apply;
        private System.Windows.Forms.Button btn_addNew;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_Search;
        private System.Windows.Forms.TextBox tB_receipt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lable_to;
        private System.Windows.Forms.DateTimePicker dTP_endDate;
        private System.Windows.Forms.DateTimePicker dTP_startDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.DataGridView dGV_receipt;
        private System.Windows.Forms.Label label3;
        private ComboBox content_cmb;
        private TextBox content_txt;
    }
}

