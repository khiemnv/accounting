#define use_sqlite

using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace test_reportPreview
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            reportViewer1.Padding = new Padding(5,5,5,5);
            Resize += Form1_Resize;
            reportViewer1.Resize += ReportViewer1_Resize;
            reportViewer1.ZoomChange += ReportViewer1_ZoomChange;

            //menu
            this.Menu = new MainMenu();
            var miFile = new MenuItem("File");
            var miSaveAs = new MenuItem("Save As...");
            miSaveAs.Click += MiSaveAs_Click;
            miFile.MenuItems.Add(miSaveAs);

            var miOption = new MenuItem("Option");
            var miAutoScale = new MenuItem("Auto Scale");
            miAutoScale.Click += MiAutoScale_Click;
            
            miOption.MenuItems.Add(miAutoScale);

            Menu.MenuItems.AddRange(new MenuItem[] {miFile, miOption });
        }

        bool mAutoScale = false;
        private void MiAutoScale_Click(object sender, EventArgs e)
        {
            mAutoScale = !mAutoScale;
            MenuItem mi = (MenuItem)sender;
            mi.Checked = mAutoScale;
        }

        class lMapExt
        {
            public string ext;
            public string format;
            public exportCallback cb;
        }

        delegate void exportCallback(LocalReport report, string pdfPath, string format);

        int selectPath(out string path)
        {
            int ret = 0;
            //select output path
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            path = "";
            saveFileDialog1.Filter = "pdf files (*.pdf)|*.pdf"
                + "|Excel files (*.xls;*.xlsx)|*.xls;*.xlsx"
                + "|Word files (*.doc;*.docx)|*.doc;*.docx"
                + "|Image Files (*.bmp;*.jpg;*.gif)|*.bmp;*.jpg;*.gif"
                + "|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            var map = new lMapExt[]
            {
                new lMapExt { ext = ".pdf" , format = "PDF", cb = Export2PDF },
                new lMapExt { ext = ".xls" , format = "Excel", cb = Export2 },
                new lMapExt { ext = ".xlsx" , format = "EXCELOPENXML", cb = Export2},
                new lMapExt { ext = ".doc" , format = "WORD", cb = Export2 },
                new lMapExt { ext = ".docx" , format = "WORDOPENXML", cb = Export2 },
                new lMapExt { ext = ".bmp" , format = "IMAGE", cb = Export2 },
                new lMapExt { ext = ".jpg" , format = "IMAGE", cb = Export2 },
                new lMapExt { ext = ".gif" , format = "IMAGE", cb = Export2 },
            };
            
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                path = saveFileDialog1.FileName;
                string ext = Path.GetExtension(path).ToLower();
                foreach (var i in map)
                {
                    if (i.ext == ext)
                    {
                        i.cb(reportViewer1.LocalReport, path, i.format);
                        break;
                    }
                }
            }
            saveFileDialog1.Dispose();
            return ret;
        }
        private void MiSaveAs_Click(object sender, EventArgs e)
        {
            string path;
            //"Excel" "EXCELOPENXML" "IMAGE" "PDF" "WORD" "WORDOPENXML"
            //selectPath(out path);
            List<ReportParameter> rpParams = new List<ReportParameter>()
                {
                    new ReportParameter("ReportParameter1", new string [] {"two", "three", "one",})
                //    new ReportParameter("nameTxt", "nguyen van a")
                //    //new ReportParameter("startDate", zStartDate),
                //    //new ReportParameter("endDate", zEndDate),
                //    //new ReportParameter( "type", "Ngày")
                };
            reportViewer1.LocalReport.SetParameters(rpParams);
            reportViewer1.LocalReport.Refresh();
            reportViewer1.RefreshReport();
#if false
            switch(mode)
            {
                case exportMode.pdf:
                    //export to PDF
                    path = "report.pdf";
                    Export2PDF(reportViewer1.LocalReport, path);
                    break;
                case exportMode.xls:
                    //export report to excel
                    path = "report.xls";
                    Export2Excel(reportViewer1.LocalReport, path);
                    break;
                case exportMode.doc:
                    //export to word
                    path = "report.doc";
                    Export2(reportViewer1.LocalReport, path, "WORD");
                    break;
                case exportMode.docx:
                    //export to word
                    path = "report.docx";
                    Export2(reportViewer1.LocalReport, path, "WORDOPENXML");
                    break;
                case exportMode.xlsx:
                    //export to word
                    path = "report.xlsx";
                    Export2(reportViewer1.LocalReport, path, "EXCELOPENXML");
                    break;
                case exportMode.jpeg:
                    //export to word
                    path = "report.jpeg";
                    Export2(reportViewer1.LocalReport, path, "IMAGE");
                    break;
            }
#endif
        }

        //gui udpate
        private void ReportViewer1_ZoomChange(object sender, ZoomChangeEventArgs e)
        {
            if (!mAutoScale) return;
            int reqW = getPrintPageW(e.ZoomPercent);
            sizeFormW(reqW);
        }

        int setPrintPageW(int reqW)
        {
            int pgW = getPrintPageWidth();
            if (pgW == -1) return -1;

            int percent = reqW * 100 / pgW;
            reportViewer1.ZoomPercent = percent;
            return reqW;
        }

        int getPrintPageW(int percent)
        {
            int pgW = getPrintPageWidth();
            int reqW = percent * pgW / 100;
            return reqW;
        }
        int sizeFormW(int reqW = -1)
        {
            if (reqW == -1)
            {
                reqW = Size.Width;
            }
            else
            {
                int reqH = this.Size.Height;
                Size = new Size(reqW, reqH);
            }
            return reqW;
        }

        private void ReportViewer1_Resize(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
        }

        int getPrintPageWidth()
        {
            var pg = reportViewer1.GetPageSettings();
            if (pg == null) return -1;
            int pgW = (int)pg.PrintableArea.Width;
            if (pg.Landscape)
            {
                pgW = (int)pg.PrintableArea.Height;
            }
            return pgW;
        }
        int getZoomPercent()
        {
            int percent = reportViewer1.ZoomPercent;
            return percent;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (!mAutoScale) return;
            int reqW = Size.Width;
            setPrintPageW(reqW);
        }

        protected DataTable loadData(string qry)
        {
#if use_sqlite
            SQLiteDataAdapter cmd = new SQLiteDataAdapter(qry, config.get_cnn());
#else
            var m_cnnStr = @"Data Source=.\SQLEXPRESS;Initial Catalog=accounting;Integrated Security=True;MultipleActiveResultSets=True";
            var m_cnn = new SqlConnection(m_cnnStr);
            m_cnn.Open();
            var cmd = new SqlDataAdapter(qry, m_cnn);
#endif

            DataTable dt = new DataTable();
            cmd.Fill(dt);
            return dt;
        }
        string getDateQry(string zStartDate, string zEndDate)
        {
            string qryDaysData = string.Format("select group_name, date, "
                + " sum(inter_pay1) as inter_pay1, sum(inter_pay2) as inter_pay2, sum(exter_pay) as exter_pay, sum(salary) as salary"
                + " from"
                + " (select group_name, date, advance_payment as inter_pay1, actually_spent as inter_pay2, 0 as exter_pay, 0 as salary"
                + " from internal_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
                + " union"
                + " select group_name, date, 0 as inter_pay1, 0 as inter_pay2,spent as exter_pay, 0 as salary"
                + " from external_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
                + " union"
                + " select group_name, date, 0 as inter_pay1,0 as inter_pay2, 0 as exter_pay, salary"
                + " from salary where date between '{0} 00:00:00' and '{1} 00:00:00')"
                + " group by group_name, date",
                zStartDate, zEndDate);
            string qryWeeksData = string.Format("select group_name, week as date, "
               + " sum(inter_pay1) as inter_pay1, sum(inter_pay2) as inter_pay2, sum(exter_pay) as exter_pay, sum(salary) as salary"
               + " from"
               + " (select group_name, strftime('%W-%Y', date) as week, advance_payment as inter_pay1, actually_spent as inter_pay2, 0 as exter_pay, 0 as salary"
               + " from internal_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
               + " union"
               + " select group_name, strftime('%W-%Y', date) as week, 0 as inter_pay1, 0 as inter_pay2,  spent as exter_pay, 0 as salary"
               + " from external_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
               + " union"
               + " select group_name, strftime('%W-%Y', date) as week, 0 as inter_pay1, 0 as inter_pay2, 0 as exter_pay, salary"
               + " from salary where date between '{0} 00:00:00' and '{1} 00:00:00')"
               + " group by group_name, week",
               zStartDate, zEndDate);
            string qryMonthsData = string.Format("select group_name, month as date,"
               + " sum(inter_pay1) as inter_pay1, sum(inter_pay2) as inter_pay2, sum(exter_pay) as exter_pay, sum(salary) as salary"
               + " from"
               + " (select group_name, strftime('%m-%Y', date) as month, advance_payment as inter_pay1, actually_spent as inter_pay2, 0 as exter_pay, 0 as salary"
               + " from internal_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
               + " union"
               + " select group_name, strftime('%m-%Y', date) as month, 0 as inter_pay1, 0 as inter_pay2, spent as exter_pay, 0 as salary"
               + " from external_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
               + " union"
               + " select group_name, strftime('%m-%Y', date) as month, 0 as inter_pay1, 0 as inter_pay2, 0 as exter_pay, salary"
               + " from salary where date between '{0} 00:00:00' and '{1} 00:00:00')"
               + " group by group_name, month",
               zStartDate, zEndDate);
            return qryMonthsData;
        }
        
        string getMonthQry(string zStartDate, string zEndDate)
        {
            string qryMonthData = string.Format("select month, sum(receipt) as receipt, sum(inter_pay1) as inter_pay1, sum(inter_pay2) as inter_pay2,sum(exter_pay) as exter_pay, sum(salary) as salary, 0 as remain "
                + " from("
                + "   select strftime('%Y-%m', date) as month, 0 as receipt, sum(advance_payment) as inter_pay1, sum(actually_spent) as inter_pay2, 0 as exter_pay, 0 as salary"
                + "   from internal_payment where date between '{0} 00:00:00' and '{1} 00:00:00' group by month"
                + "   union"
                + "   select strftime('%Y-%m', date) as month, 0 as receipt, 0 as inter_pay1, 0 as inter_pay2,sum(spent) as exter_pay, 0 as salary"
                + "   from external_payment where date between '{0} 00:00:00' and '{1} 00:00:00' group by month"
                + "   union"
                + "   select strftime('%Y-%m', date) as month, 0 as receipt, 0 as inter_pay1, 0 as inter_pay2,0 as exter_pay, sum(salary) as salary"
                + "   from salary where date between '{0} 00:00:00' and '{1} 00:00:00' group by month"
                + "   union"
                + "   select strftime('%Y-%m', date) as month, sum(amount) as receipt, 0 as inter_pay1, 0 as inter_pay2, 0 as exter_pay, 0 as salary"
                + "   from receipts where date between '{0} 00:00:00' and '{1} 00:00:00' group by month"
                + " ) group by month",
                zStartDate, zEndDate);
            return qryMonthData;

        }

#if fasle
        private void Form1_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
        }
#endif
#if true
        BackgroundWorker m_worker;
        StatusStrip m_stsBar;
        ToolStripProgressBar m_progress;
        private void Form1_Load(object sender, EventArgs e)
        {
            //loadRptDay(sender, e);
        }
        void loadWkr()
        {
            m_progress = new ToolStripProgressBar();

            //m_stsBar = new StatusStrip();
            //this.Controls.Add(m_stsBar);
            statusStrip1.Items.Add(m_progress);

            m_worker = new BackgroundWorker();
            m_worker.WorkerReportsProgress = true;
            m_worker.ProgressChanged += M_worker_ProgressChanged;
            m_worker.DoWork += M_worker_DoWork;
            m_worker.RunWorkerAsync();
        }

        private void M_worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (int i = 0; i<100; i++)
            {
                Thread.Sleep(1000);
                m_worker.ReportProgress(i);
            }
        }

        private void M_worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            m_progress.Increment(e.ProgressPercentage);
        }

        private void loadRpt()
        {
            PageSettings pg;
            pg = new System.Drawing.Printing.PageSettings();
            pg.Landscape = true;
            pg.Margins.Top = 2;
            pg.Margins.Bottom = 2;
            pg.Margins.Left = 40;
            pg.Margins.Right = 2;
            PaperSize size = new PaperSize();
            size.RawKind = (int)PaperKind.A4;
            pg.PaperSize = size;
            //reportViewer1.SetPageSettings(pg);

            //DataTable dt = ReceiptsManager.GetAvowelsReportDT(< Parameters >);
            DateTime startDate = new DateTime(2018, 01, 28);
            DateTime endDate = new DateTime(2018, 02, 03);
            string zStartDate = startDate.ToString("yyyy-MM-dd");
            string zEndDate = endDate.ToString("yyyy-MM-dd");
#if !use_sqlite
            string qry = string.Format("select * from external_payment"
                + " where constr_org like N'%{0}%' and date between '{1} 00:00:00' and '{2} 00:00:00'"
                + " order by date",
                constrorg, zStartDate, zEndDate);
#else
            string qry = string.Format("select * from receipts"
                + " where date between '{0} 00:00:00' and '{1} 00:00:00'"
                + " order by date",
                zStartDate, zEndDate);
#endif
            var m_sqls = new Dictionary<string, string>
            {
                { "DataSet1", qry},
            };

            //if (dt != null && dt.Rows.Count > 0)
            {
                reportViewer1.ProcessingMode = ProcessingMode.Local;

                LocalReport localReport = reportViewer1.LocalReport;

                reportViewer1.LocalReport.ReportPath = @"..\..\Report1.rdlc";
                reportViewer1.LocalReport.DataSources.Clear();

                DataSet ds = new DataSet();
                foreach (var pair in m_sqls)
                {
                    DataTable dt;
                    dt = loadData(pair.Value);
                    //dt = new DataTable();

                    dt.TableName = pair.Key;
                    ds.Tables.Add(dt);
                    
                    reportViewer1.LocalReport.DataSources.Add(new ReportDataSource(pair.Key, dt));
                }
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource());
                List<ReportParameter> rpParams = new List<ReportParameter>()
                {
                    new ReportParameter("startDate", zStartDate),
                    new ReportParameter("endDate", zEndDate)
                };
                reportViewer1.LocalReport.SetParameters(rpParams);
                reportViewer1.LocalReport.Refresh();

                reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
                reportViewer1.ResetPageSettings();

                pg = reportViewer1.GetPageSettings();
                //pg.Landscape = true;
                reportViewer1.SetPageSettings(pg);

                this.reportViewer1.RefreshReport();
            }
        }
        void loadBill()
        {
            PageSettings pg;
            pg = new System.Drawing.Printing.PageSettings();
            pg.Landscape = true;
            pg.Margins.Top = 2;
            pg.Margins.Bottom = 2;
            pg.Margins.Left = 40;
            pg.Margins.Right = 2;
            PaperSize size = new PaperSize();
            size.RawKind = (int)PaperKind.A4;
            pg.PaperSize = size;
            //reportViewer1.SetPageSettings(pg);

            //DataTable dt = ReceiptsManager.GetAvowelsReportDT(< Parameters >);
            DateTime startDate = new DateTime(2018, 01, 28);
            DateTime endDate = new DateTime(2018, 01, 28);
            string zStartDate = startDate.ToString("yyyy-MM-dd");
            string zEndDate = endDate.ToString("yyyy-MM-dd");
            string constrorg = "don vi a";
#if !use_sqlite
            string qry = string.Format("select * from external_payment"
                + " where constr_org like N'%{0}%' and date between '{1} 00:00:00' and '{2} 00:00:00'"
                + " order by date",
                constrorg, zStartDate, zEndDate);
            qry = "select top 2 * from internal_payment";
#else
            string qry = string.Format("select * from internal_payment"
                + " where constr_org like '%{0}%' and date between '{1} 00:00:00' and '{2} 00:00:00'"
                + " order by date",
                constrorg, zStartDate, zEndDate);
            qry = "select * from internal_payment limit 2" ;
#endif
            var m_sqls = new Dictionary<string, string>
            {
                { "DataSet1", qry},
            };

            //if (dt != null && dt.Rows.Count > 0)
            {
                reportViewer1.ProcessingMode = ProcessingMode.Local;

                LocalReport localReport = reportViewer1.LocalReport;

                reportViewer1.LocalReport.ReportPath = @"..\..\bill_receipts.rdlc";
                reportViewer1.LocalReport.DataSources.Clear();

                DataSet ds = new DataSet();
                foreach (var pair in m_sqls)
                {
                    DataTable dt;
                    dt = loadData(pair.Value);
                    //dt = new DataTable();

                    dt.TableName = pair.Key;
                    ds.Tables.Add(dt);

                    DataTable rptDt = new DataTable();
                    rptDt.Columns.Add(new DataColumn("name"));
                    rptDt.Columns.Add(new DataColumn("addr"));
                    rptDt.Columns.Add(new DataColumn("date", typeof(DateTime)));
                    rptDt.Columns.Add(new DataColumn("num"));
                    rptDt.Columns.Add(new DataColumn("content"));
                    rptDt.Columns.Add(new DataColumn("note"));
                    rptDt.Columns.Add(new DataColumn("amount", typeof(Int64)));
                    rptDt.Columns.Add(new DataColumn("amountTxt"));

                    foreach (DataRow dr in dt.Rows)
                    {
                        var r = rptDt.NewRow();
                        r["name"] = dr["name"];
                        r["addr"] = dr["addr"];
                        r["date"] = dr["date"];
                        r["num"] = dr["payment_number"];
                        r["content"] = dr["content"];
                        r["note"] = dr["note"];
                        r["amount"] = dr["advance_payment"];
                        r["amountTxt"] = "";
                        rptDt.Rows.Add(r);
                    }

                    reportViewer1.LocalReport.DataSources.Add(new ReportDataSource(pair.Key, rptDt));
                }
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource());
                List<ReportParameter> rpParams = new List<ReportParameter>()
                {
                    //receipt = 1
                    //payment = 2
                    new ReportParameter("type", "1")
                };
                reportViewer1.LocalReport.SetParameters(rpParams);
                reportViewer1.LocalReport.Refresh();

                reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);


                reportViewer1.ResetPageSettings();

                pg = reportViewer1.GetPageSettings();
                //pg.Landscape = true;
                reportViewer1.SetPageSettings(pg);

                this.reportViewer1.RefreshReport();
            }
        }
        void load_bill()
        {
            PageSettings pg;
            pg = new System.Drawing.Printing.PageSettings();
            pg.Landscape = true;
            pg.Margins.Top = 2;
            pg.Margins.Bottom = 2;
            pg.Margins.Left = 40;
            pg.Margins.Right = 2;
            PaperSize size = new PaperSize();
            size.RawKind = (int)PaperKind.A4;
            pg.PaperSize = size;
            //reportViewer1.SetPageSettings(pg);

            //DataTable dt = ReceiptsManager.GetAvowelsReportDT(< Parameters >);
            DateTime startDate = new DateTime(2015, 01, 03);
            DateTime endDate = new DateTime(2015, 01, 06);
            string zStartDate = startDate.ToString("yyyy-MM-dd");
            string zEndDate = endDate.ToString("yyyy-MM-dd");

            var m_sqls = new Dictionary<string, string>
            {
                { "DataSet1", "select * from receipts limit 3"},
            };

            //if (dt != null && dt.Rows.Count > 0)
            {
                reportViewer1.ProcessingMode = ProcessingMode.Local;

                LocalReport localReport = reportViewer1.LocalReport;

                reportViewer1.LocalReport.ReportPath = @"..\..\bill_receipts.rdlc";


                //ReportDataSource RDS = new ReportDataSource();
                //RDS.Name = < RDLC Report Dataset Name >;
                //RDS.Value = dt;
                reportViewer1.LocalReport.DataSources.Clear();
                //reportViewer1.LocalReport.DataSources.Add(RDS);

                DataSet ds = new DataSet();
                foreach (var pair in m_sqls)
                {
                    DataTable dt;
                    dt = loadData(pair.Value);
                    //dt = new DataTable();

                    dt.TableName = pair.Key;
                    ds.Tables.Add(dt);

                    reportViewer1.LocalReport.DataSources.Add(new ReportDataSource(pair.Key, dt));
                }
                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource());
                List<ReportParameter> rpParams = new List<ReportParameter>()
                {
                    new ReportParameter("ReportParameter1", new string [] {"one","two", "three" })
                //    new ReportParameter("nameTxt", "nguyen van a")
                //    //new ReportParameter("startDate", zStartDate),
                //    //new ReportParameter("endDate", zEndDate),
                //    //new ReportParameter( "type", "Ngày")
                };
                reportViewer1.LocalReport.SetParameters(rpParams);
                reportViewer1.LocalReport.Refresh();

                reportViewer1.SetDisplayMode (DisplayMode.PrintLayout);


                reportViewer1.ResetPageSettings();

                pg = reportViewer1.GetPageSettings();
                //pg.Landscape = true;
                reportViewer1.SetPageSettings(pg);

                this.reportViewer1.RefreshReport();
            }
        }

        Int64 getPrevRm(DateTime date)
        {
            var zDate = date.ToString("yyyy-MM-dd");
           string sql = string.Format( "select * from( "
                + " select(select sum(amount) from receipts where date < '{0} 00:00:00') a1, "
                + " (select sum(spent)from external_payment where date < '{0} 00:00:00') b1,"
                + " (select sum(advance_payment) + sum(actually_spent) from internal_payment where date < '{0} 00:00:00') b2,"
                + " (select sum(salary) as d1 from salary where date < '{0} 00:00:00') b3)", zDate);
            DataTable dt = loadData(sql);
            var row = dt.Rows[0];
            Int64 sum = row[0] !=DBNull.Value? (Int64)row[0] : 0;
            sum -= row[1] != DBNull.Value ? (Int64)row[1] : 0;
            sum -= row[2] != DBNull.Value ? (Int64)row[2] : 0;
            sum -= row[3] != DBNull.Value ? (Int64)row[3] : 0;
            return sum;
        }
        private void loadRptDay(object sender, EventArgs e)
        {
            PageSettings pg;

            //DataTable dt = ReceiptsManager.GetAvowelsReportDT(< Parameters >);
            DateTime startDate = new DateTime(2018, 02, 07);
            DateTime endDate = new DateTime(2018, 02, 09);
            string zStartDate = startDate.ToString("yyyy-MM-dd");
            string zEndDate = endDate.ToString("yyyy-MM-dd");

            var m_sqls = new Dictionary<string, string>
            {
                { "DataSet1", getDateQry(zStartDate, zEndDate)},
                { "DataSet2", getMonthQry(zStartDate, zEndDate)}
            };
            
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            reportViewer1.LocalReport.ReportPath = @"..\..\rpt_days.rdlc";

            //ReportDataSource RDS = new ReportDataSource();
            //RDS.Name = < RDLC Report Dataset Name >;
            //RDS.Value = dt;
            reportViewer1.LocalReport.DataSources.Clear();
            //reportViewer1.LocalReport.DataSources.Add(RDS);

            DataSet ds = new DataSet();
            Int64 preRm = getPrevRm(startDate);
            Int64 curRm = preRm;
            foreach (var pair in m_sqls)
            {
                DataTable dt;
                dt = loadData(pair.Value);

                dt.TableName = pair.Key;
                ds.Tables.Add(dt);

                //refine data
                if (dt.TableName == "DataSet1")
                {
                    
                }
                if (dt.TableName == "DataSet2")
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        curRm = curRm + (Int64)row["receipt"]
                            - (Int64)row["inter_pay1"]
                            - (Int64)row["inter_pay2"]
                            - (Int64)row["exter_pay"]
                            - (Int64)row["salary"];
                        row["remain"] = curRm;
                    }
                }

                reportViewer1.LocalReport.DataSources.Add(new ReportDataSource(pair.Key, dt));
            }

            List<ReportParameter> rpParams = new List<ReportParameter>()
            {
                new ReportParameter("startDate", zStartDate),
                new ReportParameter("endDate", zEndDate),
                new ReportParameter( "type", "Ngày"),
                new ReportParameter( "prevRm", preRm.ToString()),
                new ReportParameter( "curRm", curRm.ToString()),
            };
            reportViewer1.LocalReport.SetParameters(rpParams);
            reportViewer1.LocalReport.Refresh();

            reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);


            reportViewer1.ResetPageSettings();

            pg = reportViewer1.GetPageSettings();
            pg.Landscape = true;
            reportViewer1.SetPageSettings(pg);

            this.reportViewer1.RefreshReport();
        }
#endif

        protected void Export2PDF(LocalReport report, string pdfPath, string format)
        {
            string deviceInfo =
              @"<DeviceInfo>
                    <OutputFormat>EMF</OutputFormat>
                    <PageWidth>8.5in</PageWidth>
                    <PageHeight>11in</PageHeight>
                    <MarginTop>0.25in</MarginTop>
                    <MarginLeft>0.25in</MarginLeft>
                    <MarginRight>0.25in</MarginRight>
                    <MarginBottom>0.25in</MarginBottom>
                </DeviceInfo>";

            {
                byte[] bytes = report.Render("PDF", deviceInfo);
                FileStream fs = new FileStream(pdfPath, FileMode.Create);
                fs.Seek(0, SeekOrigin.Begin);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }
        }
        protected void Export2Excel(LocalReport report, string pdfPath, string format)
        {
            byte[] bytes = report.Render("Excel");
            FileStream fs = new FileStream(pdfPath, FileMode.Create);
            fs.Seek(0, SeekOrigin.Begin);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }
        protected void Export2(LocalReport report, string pdfPath, string format)
        {
            byte[] bytes = report.Render(format);
            FileStream fs = new FileStream(pdfPath, FileMode.Create);
            fs.Seek(0, SeekOrigin.Begin);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }
    }
    public class config
    {
        static SQLiteConnection m_cnn;
        public static SQLiteConnection get_cnn()
        {
            string dbPath = @"..\..\..\test_binding\appData.db";
            if (m_cnn == null)
            {
                m_cnn = new SQLiteConnection(string.Format("Data Source={0};Version=3;", dbPath));
                m_cnn.Open();
            }
            return m_cnn;
        }
    }

}
