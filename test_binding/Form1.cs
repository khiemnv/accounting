//#define DEBUG_DRAWING
#define use_sqlite

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
using System.Runtime.Serialization;

namespace test_binding
{
    public partial class Form1 : Form
    {
        static lContentProvider s_contentProvider;
        static lConfigMng s_config;

        private lDbSchema m_dbSchema {
            get { return s_config.m_dbSchema; }
        }
        List<lBasePanel> m_panels {
            get { return s_config.m_panels; }
        }

        private TabControl m_tabCtrl;

        public Form1()
        {
            InitializeComponent();

            //init config & load config
            s_config = lConfigMng.crtInstance();
            if (s_config.m_dbSchema == null) {
                s_config.m_dbSchema = new lSQLiteDbSchema();
                s_config.m_panels = new List<lBasePanel> {
                    new lReceiptsPanel(),
                    new lInterPaymentPanel(),
                    new lExternalPaymentPanel(),
                    new lSalaryPanel(),
                };
                s_config.UpdateConfig();
            }

            //init content provider
#if use_sqlite
            s_contentProvider = lSQLiteContentProvider.getInstance();
#else
            s_contentProvider = lSqlContentProvider.getInstance();
#endif

            //menu
            crtMenu();

            //tab control
            m_tabCtrl = new TabControl();
            Controls.Add(m_tabCtrl);
            m_tabCtrl.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            m_tabCtrl.Dock = DockStyle.Fill;

#if crtnew_panel
            List<lBasePanel> newPanels = new List<lBasePanel>();
            foreach(lBasePanel panel in m_panels)
            {
                lBasePanel newPanel = lBasePanel.crtPanel(panel);
                newPanels.Add(newPanel);
                TabPage newTab = crtTab(newPanel);
                m_tabCtrl.TabPages.Add(newTab);
            }
            m_panels = newPanels;
#else
            foreach (lBasePanel panel in m_panels)
            {
                panel.Restore();
                TabPage newTab = crtTab(panel);
                m_tabCtrl.TabPages.Add(newTab);
            }
#endif

            m_tabCtrl.SelectedIndex = 0;

            Load += new System.EventHandler(Form1_Load);
            Text = "CBV Kế Toán";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach(lBasePanel panel in m_panels)
            {
                panel.LoadData();
            }
        }

        [DataContract(Name ="Panel")]
        class lBasePanel
        {
            public lTableInfo m_tblInfo { get { return m_dataPanel.m_tblInfo; } }
            //public lDataContent m_data;
            [DataMember(Name ="dataPanel")]
            public lDataPanel m_dataPanel;
            [DataMember(Name = "searchPanel")]
            public lSearchPanel m_searchPanel;
            [DataMember(Name = "report")]
            public lBaseReport m_report;

            public TableLayoutPanel m_panel;
            public Button m_printBtn;

            protected lBasePanel() { }
            public static lBasePanel crtPanel(lBasePanel panel)
            {
                lDataPanel dataPanel = lDataPanel.crtDataPanel(panel.m_dataPanel);
                lSearchPanel searchPanel = lSearchPanel.crtSearchPanel(dataPanel, panel.m_searchPanel.m_searchCtrls);
                lBaseReport report = lBaseReport.crtReport(panel.m_report);
                lBasePanel newPanel = new lBasePanel() {
                    m_dataPanel = dataPanel, m_searchPanel = searchPanel, m_report = report
                };
                newPanel.init();
                return newPanel;
            }

            public void Restore()
            {
                m_searchPanel.m_dataPanel = m_dataPanel;
            }

            protected void init()
            {
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
                //create table layout & add controls to
                // +----------------+----------------+
                // |search panel    |          print |
                // |                |                |
                // +----------------+----------------+
                // |reload & save btn         sum    |
                // +----------------+----------------+
                // |data grid view                   |
                // |                                 |
                // +----------------+----------------+
                m_panel = new TableLayoutPanel();
                m_panel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                m_panel.Dock = DockStyle.Fill;
#if DEBUG_DRAWING
                m_panel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
#endif
                m_printBtn = new Button();
                m_printBtn.Text = "Print";
                m_printBtn.Click += new System.EventHandler(printBtn_Click);

                //add search panel to table layout
                m_searchPanel.initCtrls();
                m_panel.Controls.Add(m_searchPanel.m_tbl, 0, 0);

                //add print btn to table layout
                m_printBtn.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                m_panel.Controls.Add(m_printBtn, 1, 0);

                //add data panel ctrls to table layout
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

        [DataContract(Name ="InternalPaymentPanel")]
        class lInterPaymentPanel : lBasePanel
        {
            public lInterPaymentPanel()
            {
                m_dataPanel = new lInterPaymentDataPanel();
                m_searchPanel = new lInterPaymentSearchPanel(m_dataPanel);
                m_report = new lInternalPaymentReport();
                base.init();
            }
        }

        [DataContract(Name = "ReceiptsPanel")]
        class lReceiptsPanel : lBasePanel
        {
            public lReceiptsPanel()
            {
                m_dataPanel = new lReceiptsDataPanel();
                m_searchPanel = new lReceiptsSearchPanel(m_dataPanel);
                m_report = new lReceiptsReport();
            }
        }

        [DataContract(Name = "ExternalPaymentPanel")]
        class lExternalPaymentPanel : lBasePanel
        {
            public lExternalPaymentPanel()
            {
                m_dataPanel = new lExternalPaymentDataPanel();
                m_searchPanel = new lExternalPaymentSearchPanel(m_dataPanel);
                m_report = new lExternalPaymentReport();
                base.init();
            }
        }

        [DataContract(Name = "SalaryPanel")]
        class lSalaryPanel : lBasePanel
        {
            public lSalaryPanel()
            {
                m_dataPanel = new lSalaryDataPanel();
                m_searchPanel = new lSalarySearchPanel(m_dataPanel);
                m_report = new lSalaryReport();
                base.init();
            }
        }
        
        private TabPage crtTab(lBasePanel newPanel)
        {
            TabPage newTabPage = new TabPage();
            newPanel.initCtrls();
            newTabPage.Controls.Add(newPanel.m_panel);
            newTabPage.Text = newPanel.m_tblInfo.m_tblAlias;
            return newTabPage;
        }
        
        private void Form1_Resize(object sender, EventArgs e)
        {
        }
    }
}
