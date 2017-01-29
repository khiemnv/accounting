using System.Windows.Forms;
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Data;

namespace test_binding
{
    public partial class Form1 : Form
    {
        class lEditDlg : Form
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
                m_dataPanel.m_dataContent.Reload();
                base.OnClosed(e);
            }
        }

        class lGroupNameEditDlg : lEditDlg
        {
            public lGroupNameEditDlg()
                :base(new lGroupNameDataPanel())
            {
            }
        }
        class lReceiptsContentEditDlg : lEditDlg
        {
            public lReceiptsContentEditDlg()
                : base(new lReceiptsContentDataPanel())
            {
            }
        }
    }
}
