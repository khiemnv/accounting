using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test_binding
{
    public partial class inputF : Form
    {
        public inputF()
        {
            InitializeComponent();

            initCtrls();

            Load += InputF_Load;
        }

        private void InputF_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected virtual lInputPanel CrtInputPanel()
        {
            return new lReceiptsInputPanel();
        }

        List<ReportParameter> crtParams()
        {
            //List<ReportParameter> rpParams = new List<ReportParameter>()
            //{
            //    new ReportParameter("amountTxts", m_inputPanel.m_amountTxs.ToArray())
            //};
            //return rpParams;
            return m_inputPanel.billRptParams;
        }

        protected virtual void LoadData()
        {
            m_inputPanel.LoadData();
            m_inputPanel.RefreshPreview += refreshPreview;
            {
                //after load data complete
                var dt = m_inputPanel.m_dataContent.m_dataTable;
                //dt.TableName = m_viewName;

                LocalReport report = reportViewer2.LocalReport;
                report.ReportPath = GetBill();
                report.DataSources.Add(new ReportDataSource("DataSet1", dt));
                report.SetParameters(crtParams());
                report.Refresh();

                reportViewer2.SetDisplayMode(DisplayMode.PrintLayout);
                reportViewer2.ResetPageSettings();
                reportViewer2.RefreshReport();
            }
        }

        private void refreshPreview(object sender, lInputPanel.PreviewEventArgs e)
        {
            reportViewer2.LocalReport.SetParameters(crtParams());
            //reportViewer2.LocalReport.Refresh();
            reportViewer2.RefreshReport();
        }

        protected lInputPanel m_inputPanel;
        protected virtual string GetBill()
        {
            return @"..\..\bill_receipts.rdlc";
        }

        protected virtual void initCtrls()
        {
            m_inputPanel = CrtInputPanel();
            m_inputPanel.initCtrls();
            tableLayoutPanel1.Controls.Add(m_inputPanel.m_tbl);
            tableLayoutPanel1.Dock = DockStyle.Fill;
        }
    }

    public class lReceiptsInputF : inputF
    {
        protected override void initCtrls()
        {
            this.Text = "Phiếu Thu";
            base.initCtrls();
        }
        protected override string GetBill()
        {
            return @"..\..\bill_receipts.rdlc";
        }
        protected override lInputPanel CrtInputPanel()
        {
            return new lReceiptsInputPanel();
        }
    }
    public class lExterPayInputF : inputF
    {
        protected override void initCtrls()
        {
            this.Text = "Phiếu Chi Ngoại";
            base.initCtrls();
        }
        protected override string GetBill()
        {
            return @"..\..\bill_exterpay.rdlc";
        }
        protected override lInputPanel CrtInputPanel()
        {
            return new lExterPayInputPanel();
        }
    }
    public class lInterPayInputF : inputF
    {
        protected override void initCtrls()
        {
            this.Text = "Phiếu Chi Nội";
            base.initCtrls();
        }
        protected override string GetBill()
        {
            return @"..\..\bill_interpay.rdlc";
        }
        protected override lInputPanel CrtInputPanel()
        {
            return new lInterPayInputPanel();
        }
    }
    public class lSalaryInputF : inputF
    {
        protected override void initCtrls()
        {
            this.Text = "Phiếu Chi Lương";
            base.initCtrls();
        }
        protected override string GetBill()
        {
            return @"..\..\bill_salary.rdlc";
        }
        protected override lInputPanel CrtInputPanel()
        {
            return new lSalaryInputPanel();
        }
    }
}
