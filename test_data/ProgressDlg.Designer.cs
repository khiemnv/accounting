namespace test_data
{
    partial class ProgressDlg
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_descrTxt = new System.Windows.Forms.Label();
            this.m_cancelBtn = new System.Windows.Forms.Button();
            this.m_percentTxt = new System.Windows.Forms.Label();
            this.m_stepTxt = new System.Windows.Forms.Label();
            this.m_prg = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            //this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.m_descrTxt, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_cancelBtn, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.m_percentTxt, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_stepTxt, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_prg, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42.85714F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.04762F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.80952F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 131);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.m_descrTxt.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.m_descrTxt, 3);
            this.m_descrTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_descrTxt.Location = new System.Drawing.Point(4, 1);
            this.m_descrTxt.Name = "label1";
            this.m_descrTxt.Size = new System.Drawing.Size(276, 54);
            this.m_descrTxt.TabIndex = 0;
            this.m_descrTxt.Text = "descripton";
            this.m_descrTxt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            this.m_cancelBtn.Location = new System.Drawing.Point(216, 101);
            this.m_cancelBtn.Name = "button1";
            this.m_cancelBtn.Size = new System.Drawing.Size(64, 23);
            this.m_cancelBtn.TabIndex = 3;
            this.m_cancelBtn.Text = "Cancel";
            this.m_cancelBtn.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.m_percentTxt.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.m_percentTxt.AutoSize = true;
            this.m_percentTxt.Location = new System.Drawing.Point(14, 80);
            this.m_percentTxt.Name = "label3";
            this.m_percentTxt.Size = new System.Drawing.Size(44, 13);
            this.m_percentTxt.TabIndex = 4;
            this.m_percentTxt.Text = "Percent";
            // 
            // label4
            // 
            this.m_stepTxt.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.m_stepTxt.AutoSize = true;
            this.m_stepTxt.Location = new System.Drawing.Point(234, 80);
            this.m_stepTxt.Name = "label4";
            this.m_stepTxt.Size = new System.Drawing.Size(27, 13);
            this.m_stepTxt.TabIndex = 5;
            this.m_stepTxt.Text = "step";
            // 
            // progressBar1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.m_prg, 3);
            this.m_prg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_prg.Location = new System.Drawing.Point(4, 59);
            this.m_prg.Name = "progressBar1";
            this.m_prg.Size = new System.Drawing.Size(276, 17);
            this.m_prg.TabIndex = 6;
            // 
            // ProgressDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 131);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Form1";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label m_descrTxt;
        private System.Windows.Forms.Button m_cancelBtn;
        private System.Windows.Forms.Label m_percentTxt;
        private System.Windows.Forms.Label m_stepTxt;
        private System.Windows.Forms.ProgressBar m_prg;
    }
}