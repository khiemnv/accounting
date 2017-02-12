using System.Windows.Forms;
using System;
using System.Drawing;

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
}
