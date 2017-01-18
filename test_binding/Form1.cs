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
        static lContentProvider s_contentProvider;

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
            s_contentProvider = lSqlContentProvider.getInstance();
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
            public lTableInfo m_tblInfo { get { return m_dataPanel.m_tblInfo; } }
            //public lDataContent m_data;
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

            public virtual void LoadData()
            {
                m_dataPanel.LoadData();
                m_searchPanel.LoadData();
            }
        }

        class lInterPaymentPanel : lBasePanel
        {
            public lInterPaymentPanel()
                : base()
            {
                m_dataPanel = new lInterPaymentDataPanel();
                m_searchPanel = new lInterPaymentSearchPanel(m_dataPanel);
                m_report = new lInternalPaymentReport();
            }
        }

        class lReceiptsPanel : lBasePanel
        {
            public lReceiptsPanel() : base()
            {
                m_dataPanel = new lReceiptsDataPanel();
                m_searchPanel = new lReceiptsSearchPanel(m_dataPanel);
                m_report = new lReceiptsReport();
            }
        }

        class lExternalPaymentPanel : lBasePanel
        {
            public lExternalPaymentPanel()
                : base()
            {
                m_dataPanel = new lExternalPaymentDataPanel();
                m_searchPanel = new lExternalPaymentSearchPanel(m_dataPanel);
                m_report = new lExternalPaymentReport();
            }
        }

        class lSalaryPanel : lBasePanel
        {
            public lSalaryPanel()
                : base()
            {
                m_dataPanel = new lSalaryDataPanel();
                m_searchPanel = new lSalarySearchPanel(m_dataPanel);
                m_report = new lSalaryReport();
            }
        }

        /// <summary>
        /// panels & create panel
        /// </summary>
        private lInterPaymentPanel m_interPaymentPanel;
        private TabPage crtInternalPaymentTab()
        {
            TabPage newTabPage = new TabPage();

            m_interPaymentPanel = new lInterPaymentPanel();
            m_interPaymentPanel.initCtrls();
            newTabPage.Controls.Add(m_interPaymentPanel.m_panel);
            newTabPage.Text = m_interPaymentPanel.m_tblInfo.m_tblAlias;
            return newTabPage;
        }

        private lReceiptsPanel m_receiptsPanel;
        private TabPage crtReceiptsTab()
        {
            TabPage newTabPage = new TabPage();

            m_receiptsPanel = new lReceiptsPanel();
            m_receiptsPanel.initCtrls();
            newTabPage.Controls.Add(m_receiptsPanel.m_panel);
            newTabPage.Text = m_receiptsPanel.m_tblInfo.m_tblAlias;
            return newTabPage;
        }

        private lExternalPaymentPanel m_externalPaymentPanel;
        private TabPage crtExternalPaymentTab()
        {
            TabPage newTabPage = new TabPage();

            m_externalPaymentPanel = new lExternalPaymentPanel();
            m_externalPaymentPanel.initCtrls();
            newTabPage.Controls.Add(m_externalPaymentPanel.m_panel);
            newTabPage.Text = m_externalPaymentPanel.m_tblInfo.m_tblAlias;
            return newTabPage;
        }

        private lSalaryPanel m_salaryPanel;
        private TabPage crtSalaryTab()
        {
            TabPage newTabPage = new TabPage();

            m_salaryPanel = new lSalaryPanel();
            m_salaryPanel.initCtrls();
            newTabPage.Controls.Add(m_salaryPanel.m_panel);
            newTabPage.Text = m_salaryPanel.m_tblInfo.m_tblAlias;
            return newTabPage;
        }

        private void Form1_Load(object sender, System.EventArgs e)
        {
            m_externalPaymentPanel.LoadData();
            m_interPaymentPanel.LoadData();
            m_receiptsPanel.LoadData();
            m_salaryPanel.LoadData();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
        }
    }
}
