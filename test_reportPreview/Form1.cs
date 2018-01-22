using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
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
            selectPath(out path);
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
            SQLiteDataAdapter cmd = new SQLiteDataAdapter(qry, config.get_cnn());
            DataTable dt = new DataTable();
            cmd.Fill(dt);
            return dt;
        }
        string getDateQry(string zStartDate, string zEndDate)
        {
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
            return qryDaysData;
        }
        string getMonthQry(string zStartDate, string zEndDate)
        {
            string qryMonthData = string.Format("select month, sum(receipt) as receipt, sum(inter_pay) as inter_pay, sum(exter_pay) as exter_pay, sum(salary) as salary, 0 as remain "
                + " from("
                + "   select strftime('%Y-%m', date) as month, 0 as receipt, sum(actually_spent) as inter_pay, 0 as exter_pay, 0 as salary"
                + "   from internal_payment where date between '{0} 00:00:00' and '{1} 00:00:00' group by month"
                + "   union"
                + "   select strftime('%Y-%m', date) as month, 0 as receipt, 0 as inter_pay, sum(spent) as exter_pay, 0 as salary"
                + "   from external_payment where date between '{0} 00:00:00' and '{1} 00:00:00' group by month"
                + "   union"
                + "   select strftime('%Y-%m', date) as month, 0 as receipt, 0 as inter_pay, 0 as exter_pay, sum(salary) as salary"
                + "   from salary where date between '{0} 00:00:00' and '{1} 00:00:00' group by month"
                + "   union"
                + "   select strftime('%Y-%m', date) as month, sum(amount) as receipt, 0 as inter_pay, 0 as exter_pay, 0 as salary"
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
        private void Form1_Load(object sender, EventArgs e)
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
                { "DataSet1", "select * from receipts limit 5"},
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
                //List<ReportParameter> rpParams = new List<ReportParameter>()
                //{
                //    new ReportParameter("nameTxt", "nguyen van a")
                //    //new ReportParameter("startDate", zStartDate),
                //    //new ReportParameter("endDate", zEndDate),
                //    //new ReportParameter( "type", "Ngày")
                //};
                //reportViewer1.LocalReport.SetParameters(rpParams);
                reportViewer1.LocalReport.Refresh();

                reportViewer1.SetDisplayMode (DisplayMode.PrintLayout);


                reportViewer1.ResetPageSettings();

                pg = reportViewer1.GetPageSettings();
                //pg.Landscape = true;
                reportViewer1.SetPageSettings(pg);

                this.reportViewer1.RefreshReport();
            }
        }
        private void Form1_Loadbak(object sender, EventArgs e)
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
                { "DataSet1", getDateQry(zStartDate, zEndDate)},
                { "DataSet2", getMonthQry(zStartDate, zEndDate)}
            };

            //if (dt != null && dt.Rows.Count > 0)
            {
                reportViewer1.ProcessingMode = ProcessingMode.Local;

                LocalReport localReport = reportViewer1.LocalReport;

                reportViewer1.LocalReport.ReportPath = @"..\..\receipts.rdlc";


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

                    dt.TableName = pair.Key;
                    ds.Tables.Add(dt);

                    reportViewer1.LocalReport.DataSources.Add(new ReportDataSource(pair.Key, dt));
                }

                List<ReportParameter> rpParams = new List<ReportParameter>()
                {
                    new ReportParameter("startDate", zStartDate),
                    new ReportParameter("endDate", zEndDate),
                    new ReportParameter( "type", "Ngày")
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
