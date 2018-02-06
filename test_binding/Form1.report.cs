//#define show_report_progress
#define use_sqlite

//using Microsoft.Reporting.WebForms;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace test_binding
{
    [DataContract(Name = "Report")]
    public class lBaseReport : ICursor, IDisposable
    {
        [DataMember(Name = "rcName")]
        public string m_rcName;     //data set
        [DataMember(Name = "viewName")]
        public string m_viewName;   //data view
        [DataMember(Name = "rdlcPath")]
        public string m_rdlcPath;   //report template
        [DataMember(Name = "m_dsName")]
        public string m_dsName;     //data set name
#if crt_xml
        public string m_xmlPath;    //xml path
#endif
        public string m_pdfPath = "lastReport.pdf";    //print to pdf file
        public static string s_dateFormat = lConfigMng.getDisplayDateFormat();

        protected lBaseReport()
        {
        }

        static public lBaseReport crtReport(lBaseReport m_report)
        {
            lBaseReport newRpt = new lBaseReport();
            newRpt.m_rcName = m_report.m_rcName;
            newRpt.m_viewName = m_report.m_viewName;
            newRpt.m_rdlcPath = m_report.m_rdlcPath;
            newRpt.m_dsName = m_report.m_dsName;
            newRpt.m_pdfPath = "report.pdf";
            return newRpt;
        }

        protected DataTable loadData(string qry)
        {
            DataTable dt = appConfig.s_contentProvider.GetData(qry);
            return dt;
        }

        protected virtual DataTable loadData()
        {
            string qry = string.Format("SELECT * FROM {0}", m_viewName);
            DataTable dt = appConfig.s_contentProvider.GetData(qry);
            return dt;
        }
        protected virtual void releaseData(DataTable dt)
        {
            dt.Dispose();
        }

        private List<Stream> m_streams;
        private Stream CreateStream(string name,
          string fileNameExtension, Encoding encoding,
          string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        protected void Export(LocalReport report)
        {
            mPrintMode.cb(report, m_pdfPath, mPrintMode.format);
#if false
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
            if (appConfig.s_config.m_printToPdf)
            {
                byte[] bytes = report.Render("PDF", deviceInfo);
                FileStream fs = new FileStream(m_pdfPath, FileMode.Create);
                fs.Seek(0, SeekOrigin.Begin);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }
            else
            {
                Warning[] warnings;
                m_streams = new List<Stream>();
                report.Render("Image", deviceInfo, CreateStream, out warnings);
                foreach (Stream stream in m_streams)
                {
                    stream.Position = 0;
                }
            }
#endif
        }

        int m_currentPageIndex;
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new Metafile(m_streams[m_currentPageIndex]);

            // Adjust rectangular area with printer margins.
            Rectangle adjustedRect = new Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            m_currentPageIndex++;
            ev.HasMorePages = (m_currentPageIndex < m_streams.Count);
        }

        protected void Print()
        {
            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                m_currentPageIndex = 0;
                printDoc.Print();
            }
        }

        public virtual List<ReportParameter> getReportParam()
        {
            List<ReportParameter> rpParams = new List<ReportParameter>();
            ReportParameter rpParam = new ReportParameter();
            rpParams.Add(rpParam);
            rpParam.Name = "details";

            string qry = string.Format("select DISTINCT[year] from {0} order by [year] desc", m_viewName);
            DataTable dt = appConfig.s_contentProvider.GetData(qry);
            for (int i = 0; i < 5; i++)
            {
                string val = i < dt.Rows.Count ? dt.Rows[i][0].ToString() : "0";
                Debug.WriteLine(string.Format("details({0}) {1}", i, val));
                rpParam.Values.Add(val);
            }

            // Set the report parameters for the report
            return rpParams;
        }

        protected delegate void voidCaller();

#region cursor
        protected long m_iWork;
        protected string m_statusMsg;
        public long getPos()
        {
            return m_iWork;
        }
        public void setPos(long pos)
        {
            m_iWork = pos;
        }
        public void setStatus(string msg)
        {
            m_statusMsg = msg;
        }
        public string getStatus()
        {
            return m_statusMsg;
        }
#endregion
        //render to streams
        protected virtual void prepare()
        {
#if show_report_progress
            try
#endif
            {
                //display wait msg
                setStatus("Load view data ...");

                //long time work
                DataTable dt = loadData();
                setPos(50);

                //after load data complete
                dt.TableName = m_viewName;

                LocalReport report = new LocalReport();
                report.ReportPath = m_rdlcPath;
                report.DataSources.Add(new ReportDataSource(m_rcName, dt));

                //add report params
                List<ReportParameter> rpParams = getReportParam();
                report.SetParameters(rpParams);

                report.Refresh();

                //display wait msg
                setStatus("Exporting ...");

                //long time work
                Export(report);
                setPos(100);

                releaseData(dt);
                report.Dispose();
            }
#if show_report_progress
            catch (Exception e)
            {
                Debug.WriteLine("{0}\n  {1}", e.Message, e.InnerException.Message);
                setPos(100);
            }
#endif
        }

        //select where to save report file.pdf
        bool selectPdfPath()
        {
            bool ret = false;
            //select output path
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "pdf files (*.pdf)|*.pdf|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.RestoreDirectory = true;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                m_pdfPath = saveFileDialog1.FileName;
                ret = true;
            }
            saveFileDialog1.Dispose();
            return ret;
        }

        // prepare = fill + clean
        public virtual void Fill(LocalReport report)
        {
            //long time work
            DataTable dt = loadData();

            //after load data complete
            dt.TableName = m_viewName;

            //LocalReport report = new LocalReport();
            report.ReportPath = m_rdlcPath;
            report.DataSources.Add(new ReportDataSource(m_rcName, dt));

            //add report params
            List<ReportParameter> rpParams = getReportParam();
            report.SetParameters(rpParams);

            report.Refresh();
        }
        public virtual void Clean()
        {
            //releaseData(dt);
        }

        //select path to print
        class lMapExt
        {
            public string ext;
            public string format;
            public exportCallback cb;
        }

        delegate void exportCallback(LocalReport report, string pdfPath, string format);
        lMapExt mPrintMode;

        bool selectPath(out string path)
        {
            bool ret = false;
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
                        mPrintMode = i;
                        ret = true;
                        break;
                    }
                }
            }
            saveFileDialog1.Dispose();
            return ret;
        }

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
        protected void Export2(LocalReport report, string pdfPath, string format)
        {
            byte[] bytes = report.Render(format);
            FileStream fs = new FileStream(pdfPath, FileMode.Create);
            fs.Seek(0, SeekOrigin.Begin);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }

        public virtual void Run()
        {
            if (appConfig.s_config.m_printToPdf)
            {
                bool ret = selectPath(out m_pdfPath);
                if (!ret) return;
            }

#if show_report_progress
            ProgressDlg prg = new ProgressDlg();
            var d = new voidCaller(prepare);

            var t = d.BeginInvoke(null, null);
            m_iWork = 0;
            prg.m_endPos = 100;
            prg.m_cursor = this;
            prg.m_param = t;
            prg.ShowDialog();
            prg.Dispose();
#else
            prepare();
#endif
            //print
            if (!appConfig.s_config.m_printToPdf)
            {
                Print();
            }
        }
#region dispose
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~lBaseReport()
        {
            // Finalizer calls Dispose(false)  
            Dispose(false);
        }
        // The bulk of the clean-up code is implemented in Dispose(bool)  
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_streams != null)
                {
                    foreach (Stream stream in m_streams)
                        stream.Close();
                    m_streams = null;
                }
            }
        }
#endregion
    }

    [DataContract(Name = "ReceiptsReport")]
    public class lReceiptsReport : lBaseReport
    {
        public lReceiptsReport()
        {
            m_rcName = "DataSet1";
            m_viewName = "v_receipts";
            m_rdlcPath = @"..\..\rpt_receipts.rdlc";
            m_dsName = "DataSet1";
            m_pdfPath = @"..\..\report.pdf";
        }
    }
    [DataContract(Name = "InternalPaymentReport")]
    public class lInternalPaymentReport : lBaseReport
    {
        public lInternalPaymentReport()
        {
            m_rcName = "DataSet1";
            m_viewName = "v_internal_payment";
            m_rdlcPath = @"..\..\rpt_interpayment.rdlc";
            m_dsName = "DataSet1";
            m_pdfPath = @"..\..\report.pdf";
        }
    }
    [DataContract(Name = "ExternalPaymentReport")]
    public class lExternalPaymentReport : lBaseReport
    {
        public lExternalPaymentReport()
        {
            m_rcName = "DataSet1";
            m_viewName = "v_external_payment";
            m_rdlcPath = @"..\..\rpt_exterpayment.rdlc";
            m_dsName = "DataSet1";
            m_pdfPath = @"..\..\report.pdf";
        }
    }
    [DataContract(Name = "SalaryReport")]
    public class lSalaryReport : lBaseReport
    {
        public lSalaryReport()
        {
            m_rcName = "DataSet1";
            m_viewName = "v_salary";
            m_rdlcPath = @"..\..\rpt_salary.rdlc";
            m_dsName = "DataSet1";
            m_pdfPath = @"..\..\report.pdf";
        }
    }


    public class lDaysReport : lBaseReport
    {
        protected Dictionary<string, string> m_sqls;
        protected DateTime m_startDate;
        protected DateTime m_endDate;

        List<ReportParameter> m_rptParams;

        protected virtual string getDateQry(string zStartDate, string zEndDate)
        {
            Debug.Assert(lConfigMng.checkDateString(zStartDate), "invalid date format");
            Debug.Assert(lConfigMng.checkDateString(zEndDate), "invalid date format");
#if use_sqlite
            string qryDaysData = string.Format("select group_name, date, name,"
                + " sum(inter_pay) as inter_pay, sum(exter_pay) as exter_pay, sum(salary) as salary"
                + " from"
                + " (select group_name, date, name, actually_spent as inter_pay, 0 as exter_pay, 0 as salary"
                + " from internal_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
                + " union"
                + " select group_name, date, name, 0 as inter_pay, spent as exter_pay, 0 as salary"
                + " from external_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
                + " union"
                + " select group_name, date, name, 0 as inter_pay, 0 as exter_pay, salary"
                + " from salary where date between '{0} 00:00:00' and '{1} 00:00:00')"
                + " group by group_name, date, name",
                zStartDate, zEndDate);
#else
            string qryDaysData = string.Format("select t1.group_name, t1.date, t1.name,"
                + " sum(t1.inter_pay) as inter_pay, sum(t1.exter_pay) as exter_pay, sum(t1.salary) as salary"
                + " from "
                + " (select group_name, date, name, actually_spent as inter_pay, 0 as exter_pay, 0 as salary"
                + " from internal_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
                + " union"
                + " select group_name, date, name, 0 as inter_pay, spent as exter_pay, 0 as salary"
                + " from external_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
                + " union"
                + " select group_name, date, name, 0 as inter_pay, 0 as exter_pay, salary"
                + " from salary where date between '{0} 00:00:00' and '{1} 00:00:00')"
                + " as t1"
                + " group by group_name, date, name",
                zStartDate, zEndDate);
#endif
            return qryDaysData;
        }

        protected string getMonthQry(string zStartDate, string zEndDate)
        {
            Debug.Assert(lConfigMng.checkDateString(zStartDate), "invalid date format");
            Debug.Assert(lConfigMng.checkDateString(zEndDate), "invalid date format");
#if use_sqlite
            string qryMonthData = string.Format("select month, sum(receipt) as receipt, sum(inter_pay) as inter_pay, sum(exter_pay) as exter_pay, sum(salary) as salary, 0 as remain "
                + " from("
                + "   select strftime('%m-%Y', date) as month, 0 as receipt, sum(actually_spent) as inter_pay, 0 as exter_pay, 0 as salary"
                + "   from internal_payment where date between '{0} 00:00:00' and '{1} 00:00:00' group by month"
                + "   union"
                + "   select strftime('%m-%Y', date) as month, 0 as receipt, 0 as inter_pay, sum(spent) as exter_pay, 0 as salary"
                + "   from external_payment where date between '{0} 00:00:00' and '{1} 00:00:00' group by month"
                + "   union"
                + "   select strftime('%m-%Y', date) as month, 0 as receipt, 0 as inter_pay, 0 as exter_pay, sum(salary) as salary"
                + "   from salary where date between '{0} 00:00:00' and '{1} 00:00:00' group by month"
                + "   union"
                + "   select strftime('%m-%Y', date) as month, sum(amount) as receipt, 0 as inter_pay, 0 as exter_pay, 0 as salary"
                + "   from receipts where date between '{0} 00:00:00' and '{1} 00:00:00' group by month"
                + " ) group by month",
                zStartDate, zEndDate);
#else
            string qryMonthData = string.Format(" select t1.month, sum(t1.receipt) as receipt, sum(t1.inter_pay) as inter_pay, sum(t1.exter_pay) as exter_pay, sum(t1.salary) as salary, 0 as remain "
            + " from( "
            + "   select right(CONVERT(VARCHAR(10), date, 105),7) as month, 0 as receipt, sum(actually_spent) as inter_pay, 0 as exter_pay, 0 as salary "
            + "   from internal_payment where date between '{0} 00:00:00' and '{1} 00:00:00' group by right(CONVERT(VARCHAR(10), date, 105),7) "
            + "   union "
            + "   select right(CONVERT(VARCHAR(10), date, 105),7) as month, 0 as receipt, 0 as inter_pay, sum(spent) as exter_pay, 0 as salary "
            + "   from external_payment where date between '{0} 00:00:00' and '{1} 00:00:00' group by right(CONVERT(VARCHAR(10), date, 105),7) "
            + "   union "
            + "   select right(CONVERT(VARCHAR(10), date, 105),7) as month, 0 as receipt, 0 as inter_pay, 0 as exter_pay, sum(salary) as salary "
            + "   from salary where date between '{0} 00:00:00' and '{1} 00:00:00' group by right(CONVERT(VARCHAR(10), date, 105),7) "
            + "   union "
            + "   select right(CONVERT(VARCHAR(10), date, 105),7) as month, sum(amount) as receipt, 0 as inter_pay, 0 as exter_pay, 0 as salary "
            + "   from receipts where date between '{0} 00:00:00' and '{1} 00:00:00' group by right(CONVERT(VARCHAR(10), date, 105),7) "
            + " ) as t1 group by month ",
                zStartDate, zEndDate);
#endif
            return qryMonthData;
        }
        protected virtual string getType()
        {
            return "Ngày";
        }
        public lDaysReport(DateTime startDate, DateTime endDate)
        {
            m_startDate = startDate;
            m_endDate = endDate;
            m_rptParams = new List<ReportParameter>()
            {
                new ReportParameter("startDate", startDate.ToString(lConfigMng.getDisplayDateFormat())),
                new ReportParameter("endDate", endDate.ToString(lConfigMng.getDisplayDateFormat())),
                new ReportParameter( "type", getType())
            };

            string zStartDate = startDate.ToString(lConfigMng.getDateFormat());
            string zEndDate = endDate.ToString(lConfigMng.getDateFormat());
            m_sqls = new Dictionary<string, string>
            {
                { "DataSet1", getDateQry(zStartDate, zEndDate)},
                { "DataSet2", getMonthQry(zStartDate, zEndDate)}
            };

            m_rdlcPath = @"..\..\rpt_days.rdlc";
        }

        protected override void prepare()
        {
            //display wait msg
            setStatus("Loading data ...");

            LocalReport report = new LocalReport();

            //long time work
            DataSet ds = new DataSet();
            int step = 50 / m_sqls.Count;
            Int64 pos = 0;
            foreach (var pair in m_sqls)
            {
                DataTable dt = getData(pair.Key);
                //after load data complete
                pos += step;
                setPos(pos);

                dt.TableName = pair.Key;
                ds.Tables.Add(dt);

                report.ReportPath = m_rdlcPath;
                report.DataSources.Add(new ReportDataSource(pair.Key, dt));
            }

            //add report params
            List<ReportParameter> rpParams = getReportParam();
            report.SetParameters(rpParams);

            report.Refresh();

            //display wait msg
            setStatus("Exporting ...");

            //long time work
            Export(report);
            setPos(100);

            ds.Clear();
            ds.Dispose();
            report.Dispose();
        }

        public override void Fill(LocalReport report)
        {
            //LocalReport report = new LocalReport();

            //long time work
            DataSet ds = new DataSet();
            foreach (var pair in m_sqls)
            {
                DataTable dt = getData(pair.Key);

                //after load data complete
                dt.TableName = pair.Key;
                onLoadDataComplete(dt);

                //add to ds
                ds.Tables.Add(dt);

                report.ReportPath = m_rdlcPath;
                report.DataSources.Add(new ReportDataSource(pair.Key, dt));
            }

            //add report params
            List<ReportParameter> rpParams = getReportParam();
            report.SetParameters(rpParams);

            report.Refresh();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_sqls.Clear();
            }
            base.Dispose(disposing);
        }

        public override List<ReportParameter> getReportParam()
        {
            return m_rptParams;
        }

        protected virtual DataTable getData(string key)
        {
            return loadData(m_sqls[key]);
        }

        protected virtual void onLoadDataComplete(DataTable dt)
        {
            //modify qry data
        }
    }
    public class lSqlDaysReport : lDaysReport
    {
        public lSqlDaysReport(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
            m_sqls["DataSet1"] = "rpt_days";
            m_sqls["DataSet2"] = "sta_month";
        }
        protected override DataTable getData(string key)
        {
            DataTable dt = new DataTable();
            var cmd = new SqlCommand();
            cmd.CommandText = m_sqls[key];
            cmd.Connection = (SqlConnection)appConfig.s_contentProvider.GetCnn();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@startDate", m_startDate));
            cmd.Parameters.Add(new SqlParameter("@endDate", m_endDate));

            var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(dt);
            return dt;
        }
    }
    public class lWeekReport : lDaysReport
    {
        protected override string getDateQry(string zStartDate, string zEndDate)
        {
            Debug.Assert(lConfigMng.checkDateString(zStartDate), "invalid date format");
            Debug.Assert(lConfigMng.checkDateString(zEndDate), "invalid date format");
#if use_sqlite
            string qryWeeksData = string.Format("select group_name, week as date, '' as name,"
               + " sum(inter_pay) as inter_pay, sum(exter_pay) as exter_pay, sum(salary) as salary"
               + " from"
               + " (select group_name, strftime('%W-%Y', date) as week, actually_spent as inter_pay, 0 as exter_pay, 0 as salary"
               + " from internal_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
               + " union"
               + " select group_name, strftime('%W-%Y', date) as week, 0 as inter_pay, spent as exter_pay, 0 as salary"
               + " from external_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
               + " union"
               + " select group_name, strftime('%W-%Y', date) as week, 0 as inter_pay, 0 as exter_pay, salary"
               + " from salary where date between '{0} 00:00:00' and '{1} 00:00:00')"
               + " group by group_name, week",
               zStartDate, zEndDate);
#else
            string qryWeeksData = string.Format(" select group_name, week as date, '' as name,"
            + " sum(inter_pay) as inter_pay, sum(exter_pay) as exter_pay, sum(salary) as salary"
            + " from"
            + " (select group_name, (cast(DATEPART(ww,date) as char(2))+'-'+cast(DATEPART(yyyy,date) as CHAR(4))) as week, actually_spent as inter_pay, 0 as exter_pay, 0 as salary"
            + " from internal_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
            + " union"
            + " select group_name, (cast(DATEPART(ww,date) as char(2))+'-'+cast(DATEPART(yyyy,date) as CHAR(4))) as week, 0 as inter_pay, spent as exter_pay, 0 as salary"
            + " from external_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
            + " union"
            + " select group_name, (cast(DATEPART(ww,date) as char(2))+'-'+cast(DATEPART(yyyy,date) as CHAR(4))) as week, 0 as inter_pay, 0 as exter_pay, salary"
            + " from salary where date between '{0} 00:00:00' and '{1} 00:00:00') as t1"
            + " group by group_name, week",
               zStartDate, zEndDate);
#endif //use_sqlite
            return qryWeeksData;
        }
        protected override string getType()
        {
            return "Tuần";
        }
        public lWeekReport(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
#if false
    public class lSqlWeekReport : lWeekReport
    {
        public lSqlWeekReport(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
        protected override string getType()
        {
            return "Tuần";
        }
    }
#endif
    public class lMonthReport : lDaysReport
    {
        protected override string getDateQry(string zStartDate, string zEndDate)
        {
            Debug.Assert(lConfigMng.checkDateString(zStartDate), "invalid date format");
            Debug.Assert(lConfigMng.checkDateString(zEndDate), "invalid date format");
#if use_sqlite
            string qryMonthsData = string.Format("select group_name, month as date, '' as name,"
               + " sum(inter_pay) as inter_pay, sum(exter_pay) as exter_pay, sum(salary) as salary"
               + " from"
               + " (select group_name, strftime('%m-%Y', date) as month, actually_spent as inter_pay, 0 as exter_pay, 0 as salary"
               + " from internal_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
               + " union"
               + " select group_name, strftime('%m-%Y', date) as month, 0 as inter_pay, spent as exter_pay, 0 as salary"
               + " from external_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
               + " union"
               + " select group_name, strftime('%m-%Y', date) as month, 0 as inter_pay, 0 as exter_pay, salary"
               + " from salary where date between '{0} 00:00:00' and '{1} 00:00:00')"
               + " group by group_name, month",
               zStartDate, zEndDate);
#else
            string qryMonthsData = string.Format(" select group_name, month as date, '' as name,"
                + " sum(inter_pay) as inter_pay, sum(exter_pay) as exter_pay, sum(salary) as salary"
                + " from"
                + " (select group_name, right(CONVERT(VARCHAR(10), date, 105),7) as month, actually_spent as inter_pay, 0 as exter_pay, 0 as salary"
                + " from internal_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
                + " union"
                + " select group_name, right(CONVERT(VARCHAR(10), date, 105),7) as month, 0 as inter_pay, spent as exter_pay, 0 as salary"
                + " from external_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
                + " union"
                + " select group_name, right(CONVERT(VARCHAR(10), date, 105),7) as month, 0 as inter_pay, 0 as exter_pay, salary"
                + " from salary where date between '{0} 00:00:00' and '{1} 00:00:00')"
                + " as t1 group by group_name, month",
               zStartDate, zEndDate);
#endif
            return qryMonthsData;
        }
        protected override string getType()
        {
            return "Tháng";
        }
        public lMonthReport(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
    }
#if false
    public class lSqlMonthReport : lMonthReport
    {
        public lSqlMonthReport(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
        }
        protected override string getType()
        {
            return "Tháng";
        }
    }
#endif
    public class lBuildingReport : lDaysReport
    {
        public string m_buildingName;
        List<ReportParameter> m_rptParams;
        public lBuildingReport(string building, DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
            string zStartDate = startDate.ToString(lConfigMng.getDisplayDateFormat());
            string zEndDate = endDate.ToString(lConfigMng.getDisplayDateFormat());
            m_buildingName = building;
            m_rptParams = new List<ReportParameter>()
            {
                new ReportParameter("startDate",zStartDate),
                new ReportParameter("endDate",zEndDate),
                new ReportParameter("buildingName", m_buildingName)
            };
            zStartDate = startDate.ToString(lConfigMng.getDateFormat());
            zEndDate = endDate.ToString(lConfigMng.getDateFormat());
#if use_sqlite
            string qry = string.Format("select * from external_payment"
                + " where building like '%{0}%' and date between '{1} 00:00:00' and '{2} 00:00:00'"
                + " order by date",
                building, zStartDate, zEndDate);
#else
            string qry = string.Format("select * from external_payment"
                + " where building like N'%{0}%' and date between '{1} 00:00:00' and '{2} 00:00:00'"
                + " order by date",
                building, zStartDate, zEndDate);
#endif
            m_sqls = new Dictionary<string, string>
            {
                { "DataSet1", qry }
            };
            m_rdlcPath = @"..\..\rpt_building.rdlc";
        }
        public override List<ReportParameter> getReportParam()
        {
            return m_rptParams;
        }
    }

    public class lConstrorgReport : lDaysReport
    {
        public string m_constrorg;
        List<ReportParameter> m_rptParams;
        public lConstrorgReport(string constrorg, DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
            string zStartDate = startDate.ToString(lConfigMng.getDisplayDateFormat());
            string zEndDate = endDate.ToString(lConfigMng.getDisplayDateFormat());
            m_constrorg = constrorg;
            m_rptParams = new List<ReportParameter>()
            {
                new ReportParameter("startDate",zStartDate),
                new ReportParameter("endDate",zEndDate),
                new ReportParameter("constrorg", m_constrorg)
            };
            zStartDate = startDate.ToString(lConfigMng.getDateFormat());
            zEndDate = endDate.ToString(lConfigMng.getDateFormat());
#if use_sqlite
            string qry = string.Format("select * from external_payment"
                + " where constr_org like '%{0}%' and date between '{1} 00:00:00' and '{2} 00:00:00'"
                + " order by date",
                constrorg, zStartDate, zEndDate);
#else
            string qry = string.Format("select * from external_payment"
                + " where constr_org like N'%{0}%' and date between '{1} 00:00:00' and '{2} 00:00:00'"
                + " order by date",
                constrorg, zStartDate, zEndDate);
#endif
            m_sqls = new Dictionary<string, string>
            {
                { "DataSet1", qry }
            };
            m_rdlcPath = @"..\..\rpt_constrorg.rdlc";
        }
        public override List<ReportParameter> getReportParam()
        {
            return m_rptParams;
        }
    }

    public class lDaysumReport : lDaysReport
    {
        List<ReportParameter> m_rptParams;
        public lDaysumReport(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
            string zStartDate = startDate.ToString(lConfigMng.getDisplayDateFormat());
            string zEndDate = endDate.ToString(lConfigMng.getDisplayDateFormat());
            m_rptParams = new List<ReportParameter>()
            {
                new ReportParameter("startDate",zStartDate),
                new ReportParameter("endDate",zEndDate),
            };

            zStartDate = startDate.ToString(lConfigMng.getDateFormat());
            zEndDate = endDate.ToString(lConfigMng.getDateFormat());
#if use_sqlite
            string qry = string.Format("select * from v_day_sum"
                + " where date between '{0} 00:00:00' and '{1} 00:00:00'"
                + " order by date",
                zStartDate, zEndDate);
#else
            string qry = string.Format("select * from v_day_sum"
                + " where date between '{0} 00:00:00' and '{1} 00:00:00'"
                + " order by date",
                zStartDate, zEndDate);
#endif
            m_sqls = new Dictionary<string, string>
            {
                { "DataSet1", qry }
            };
            m_rdlcPath = @"..\..\rpt_daysum.rdlc";
        }
        protected override void onLoadDataComplete(DataTable dt)
        {
            Int64 inc = 0;
            foreach (DataRow row in dt.Rows)
            {
                inc += (Int64)row["sum"];
                row["sum"] = inc;
            }
            m_rptParams.Add(new ReportParameter("remain", inc.ToString()));
        }
        public override List<ReportParameter> getReportParam()
        {
            return m_rptParams;
        }
    }

    public class lReceiptsDays : lDaysReport
    {
        List<ReportParameter> m_rptParams;
        public lReceiptsDays(DateTime startDate, DateTime endDate) : base(startDate, endDate)
        {
            string zStartDate = startDate.ToString(lConfigMng.getDisplayDateFormat());
            string zEndDate = endDate.ToString(lConfigMng.getDisplayDateFormat());
            m_rptParams = new List<ReportParameter>()
            {
                new ReportParameter("startDate",zStartDate),
                new ReportParameter("endDate",zEndDate),
            };

            zStartDate = startDate.ToString(lConfigMng.getDateFormat());
            zEndDate = endDate.ToString(lConfigMng.getDateFormat());
            string qry = string.Format("select * from receipts"
                + " where date between '{0} 00:00:00' and '{1} 00:00:00'"
                + " order by date",
                zStartDate, zEndDate);

            m_sqls = new Dictionary<string, string>
            {
                { "DataSet1", qry }
            };
            m_rdlcPath = @"..\..\rpt_dayreceipts.rdlc";
        }
        public override List<ReportParameter> getReportParam()
        {
            return m_rptParams;
        }
    }

    public class lCurReceiptsReport : lBaseReport
    {
        public lCurReceiptsReport()
        {
            m_rdlcPath = @"..\..\c_receipts.rdlc";
            m_viewName = "receipts";
            m_rcName = "DataSet1";
        }
        public override List<ReportParameter> getReportParam()
        {
            return new List<ReportParameter>();
        }
        protected override DataTable loadData()
        {
            lDataContent dc = appConfig.s_contentProvider.CreateDataContent(m_viewName);
            //dc.Load();    //if current is no data
            return dc.m_dataTable;
        }
        protected override void releaseData(DataTable dt)
        {
            //do nothing
        }
    }

    public class lCurInterPaymentReport : lCurReceiptsReport
    {
        public lCurInterPaymentReport()
        {
            m_rdlcPath = @"..\..\c_interpayment.rdlc";
            m_viewName = "internal_payment";
        }
    }

    public class lCurExterPaymentReport : lCurReceiptsReport
    {
        public lCurExterPaymentReport()
        {
            m_rdlcPath = @"..\..\c_exterpayment.rdlc";
            m_viewName = "external_payment";
        }
    }

    public class lCurSalaryReport : lCurReceiptsReport
    {
        public lCurSalaryReport()
        {
            m_rdlcPath = @"..\..\c_salary.rdlc";
            m_viewName = "salary";
        }
    }

    public class fileExporter
    {
        class lMapExt
        {
            public string ext;
            public string format;
            public exportCallback cb;
        }

        delegate void exportCallback(LocalReport report, string pdfPath, string format);
        lMapExt mPrintMode;
        string mPath = "";
        public bool selectPath(out string path)
        {
            bool ret = false;
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
                new lMapExt { ext = ".pdf" , format = "PDF", cb = Export2 },
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
                        mPrintMode = i;
                        mPath = path;
                        ret = true;
                        break;
                    }
                }
            }
            saveFileDialog1.Dispose();
            return ret;
        }

        protected void Export2(LocalReport report, string pdfPath, string format)
        {
            byte[] bytes = report.Render(format);
            FileStream fs = new FileStream(pdfPath, FileMode.Create);
            fs.Seek(0, SeekOrigin.Begin);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
        }

        public void export(LocalReport report)
        {
            string path;
            if (mPath == "")
            {
                selectPath(out path);
            }
            if (mPath != "")
            {
                mPrintMode.cb(report, mPath, mPrintMode.format);
            }
        }
    }
}