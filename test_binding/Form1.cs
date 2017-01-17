//#define DEBUG_DRAWING

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data.SqlClient;

namespace test_binding
{

    public partial class Form1 : Form
    {
        private lContentProvider m_contentProvider;
        private TabControl tabControl1;
        private TabPage tabPage1;   //receipts
        private TabPage tabPage2;   //internal payment
        private TabPage tabPage3;   //external payment
        private TabPage tabPage4;   //salary
        
        public Form1()
        {
            InitializeComponent();
#if use_sqlite
            m_contentProvider = lSQLiteContentProvider.getInstance();
#else
            m_contentProvider = lSqlContentProvider.getInstance();
#endif

            //tab control
            this.tabControl1 = new TabControl();
            this.Controls.Add(tabControl1);
            this.tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            this.tabControl1.Dock = DockStyle.Fill;

            //tab1
            this.tabPage1 = crtReceiptsTab();
            this.tabControl1.TabPages.Add(tabPage1);

            ///////////////////tab2
            this.tabPage2 = crtInternalPaymentTab();
            this.tabControl1.TabPages.Add(tabPage2);

            //tab3
            this.tabPage3 = crtExternalPaymentTab();
            this.tabControl1.TabPages.Add(tabPage3);

            //tab4
            this.tabPage4 = crtSalaryTab();
            this.tabControl1.TabPages.Add(tabPage4);

            this.tabControl1.SelectedTab = tabPage1;

            this.Load += new System.EventHandler(Form1_Load);
            this.Text = "CBV Kế Toán";
        }

        class lBasePanel
        {
            public lTableInfo m_tblInfo;
            public lDataContent m_data;
            public lDataPanel m_dataPanel;
            public lSearchPanel m_searchPanel;
            public lBaseReport m_report;

            public TableLayoutPanel m_panel;
            public Button m_printBtn;

            public lBasePanel()
            {
                m_panel = new TableLayoutPanel();
                m_panel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                m_panel.Dock = DockStyle.Fill;
#if DEBUG_DRAWING
                m_panel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
#endif
                m_printBtn = new Button();
                m_printBtn.Text = "Print";
                m_printBtn.Click += new System.EventHandler(printBtn_Click);
            }

            private void printBtn_Click(object sender, EventArgs e)
            {
                if (m_report != null) {
                    m_report.Run();
                    m_report.Dispose();
                }
            }

            public virtual void initCtrls()
            {
                m_searchPanel.initCtrls();
                m_panel.Controls.Add(m_searchPanel.m_tbl, 0, 0);

                m_printBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                m_panel.Controls.Add(m_printBtn, 1, 0);

                m_dataPanel.initCtrls();
                m_panel.Controls.Add(m_dataPanel.m_reloadPanel, 0, 1);
                m_panel.Controls.Add(m_dataPanel.m_sumPanel, 1, 1);
                m_panel.Controls.Add(m_dataPanel.m_dataGridView, 0, 2);
                m_panel.SetColumnSpan(m_dataPanel.m_dataGridView, 2);
            }
        }

        class lInterPaymentPanel : lBasePanel
        {
            public lInterPaymentPanel(lContentProvider cp)
                : base()
            {
                m_tblInfo = new lInternalPaymentTblInfo();
                m_data = cp.createDataContent(m_tblInfo);
                m_dataPanel = new lInterPaymentDataPanel(m_data);
                m_searchPanel = new lInterPaymentSearchPanel(m_dataPanel);
                m_report = new lInternalPaymentReport(cp);
            }
        }

        class lReceiptsPanel : lBasePanel
        {
            public lReceiptsPanel(lContentProvider cp) : base()
            {
                m_tblInfo = new lReceiptsTblInfo();
                m_data = cp.createDataContent(m_tblInfo);
                m_dataPanel = new lReceiptsDataPanel(m_data);
                m_searchPanel = new lReceiptsSearchPanel(m_dataPanel);
                m_report = new lReceiptsReport(cp);
            }
        }

        class lExternalPaymentPanel : lBasePanel
        {
            public lExternalPaymentPanel(lContentProvider cp)
                : base()
            {
                m_tblInfo = new lExternalPaymentTblInfo();
                m_data = cp.createDataContent(m_tblInfo);
                m_dataPanel = new lExternalPaymentDataPanel(m_data);
                m_searchPanel = new lExternalPaymentSearchPanel(m_dataPanel);
                m_report = new lExternalPaymentReport(cp);
            }
        }

        class lSalaryPanel : lBasePanel
        {
            public lSalaryPanel(lContentProvider cp)
                : base()
            {
                m_tblInfo = new lSalaryTblInfo();
                m_data = cp.createDataContent(m_tblInfo);
                m_dataPanel = new lSalaryDataPanel(m_data);
                m_searchPanel = new lSalarySearchPanel(m_dataPanel);
                m_report = new lSalaryReport(cp);
            }
        }

        /// <summary>
        /// panels & create panel
        /// </summary>
        private lInterPaymentPanel m_interPaymentPanel;
        private TabPage crtInternalPaymentTab()
        {
            TabPage newTabPage = new TabPage();

            m_interPaymentPanel = new lInterPaymentPanel(m_contentProvider);
            m_interPaymentPanel.initCtrls();
            newTabPage.Controls.Add(m_interPaymentPanel.m_panel);
            newTabPage.Text = m_interPaymentPanel.m_tblInfo.m_tblAlias;
            return newTabPage;
        }

        private lReceiptsPanel m_receiptsPanel;
        private TabPage crtReceiptsTab()
        {
            TabPage newTabPage = new TabPage();

            m_receiptsPanel = new lReceiptsPanel(m_contentProvider);
            m_receiptsPanel.initCtrls();
            newTabPage.Controls.Add(m_receiptsPanel.m_panel);
            newTabPage.Text = m_receiptsPanel.m_tblInfo.m_tblAlias;
            return newTabPage;
        }

        private lExternalPaymentPanel m_externalPaymentPanel;
        private TabPage crtExternalPaymentTab()
        {
            TabPage newTabPage = new TabPage();

            m_externalPaymentPanel = new lExternalPaymentPanel(m_contentProvider);
            m_externalPaymentPanel.initCtrls();
            newTabPage.Controls.Add(m_externalPaymentPanel.m_panel);
            newTabPage.Text = m_externalPaymentPanel.m_tblInfo.m_tblAlias;
            return newTabPage;
        }

        private lSalaryPanel m_salaryPanel;
        private TabPage crtSalaryTab()
        {
            TabPage newTabPage = new TabPage();

            m_salaryPanel = new lSalaryPanel(m_contentProvider);
            m_salaryPanel.initCtrls();
            newTabPage.Controls.Add(m_salaryPanel.m_panel);
            newTabPage.Text = m_salaryPanel.m_tblInfo.m_tblAlias;
            return newTabPage;
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
        }
    }
}
