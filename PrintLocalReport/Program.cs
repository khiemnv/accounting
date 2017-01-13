using System;
using System.IO;
using System.Data;
using System.Text;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Drawing;
using System.Data.SqlClient;
using System.Diagnostics;

public class Demo : IDisposable
{
    private int m_currentPageIndex;
    private IList<Stream> m_streams;

    string rc_name = "DataSet1";
    string view_name = "vReceipts";
    //string rdlc_path = @"..\..\receipts.rdlc";
    string rdlc_path = @"..\..\receipts.rdlc";
    string xml_path = @"..\..\receiptsData.xml";
    string ds_name = "DataSet1";

    class lBaseReport:IDisposable
    {
        public string m_rcName;     //data set
        public string m_viewName;   //data view
        public string m_rdlcPath;   //report template
#if crt_xml
        public string m_xmlPath;    //xml path
#endif
        public string m_pdfPath;    //print to pdf file
        public string m_dsName;     //data set name

        private SqlConnection m_cnn;
        private DataSet m_ds = new DataSet();
        public void init(SqlConnection cnn)
        {
            m_cnn = cnn;
        }
        private DataTable loadData()
        {
            string qry = string.Format("SELECT * FROM {0}", m_viewName);
            SqlDataAdapter cmd = new SqlDataAdapter(qry, m_cnn);

            // Create and fill a DataSet.
            m_ds.Clear();
            m_ds.DataSetName = m_dsName;
            cmd.Fill(m_ds);
            m_ds.Tables[0].TableName = m_viewName;
#if crt_xml
            m_ds.WriteXml(m_xmlPath);
#endif
            return m_ds.Tables[0];
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
        private void Export(LocalReport report)
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
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out warnings);
            foreach (Stream stream in m_streams) { 
                stream.Position = 0;
            }
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
        private void Print()
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

        private ReportParameter[] getReportParam()
        {

            // Create a report parameter for the sales order number
            ReportParameter rpParam = new ReportParameter();
            //detail 1: 0x01
            //detail 2: 0x02
            //detail 3: 0x04
            string qry = "select DISTINCT[year] from v_external_payment";
            SqlCommand command = new SqlCommand(qry, m_cnn);

            SqlDataReader reader = command.ExecuteReader();

            // Call Read before accessing data.
            int curYear = DateTime.Now.Year;
            int DetailFlags = 0;
            while (reader.Read())
            {
                string val = reader[0].ToString();
                switch (curYear - int.Parse(val))
                {
                    case 0:
                    case 1:
                        DetailFlags |= 1;
                        break;
                    case 2:
                    case 3:
                        DetailFlags |= 2;
                        break;
                    case 4:
                        DetailFlags |= 4;
                        break;
                }
            }
            // Call Close when done reading.
            reader.Close();

            Debug.WriteLine(string.Format("DetailFlags {0}", DetailFlags));
            rpParam.Name = "DetailFlags";
            string value = DetailFlags.ToString();
            rpParam.Values.Add(value);

            // Set the report parameters for the report
            return new ReportParameter[] { rpParam };
        }
        public void Run()
        {
            LocalReport report = new LocalReport();
            report.ReportPath = m_rdlcPath;
            DataTable dt = loadData();
            report.DataSources.Add(new ReportDataSource(m_rcName, dt));

            //add rp params
            ReportParameter[] rpParams = getReportParam();
            report.SetParameters(rpParams);

            report.Refresh();
#if true
            byte[] bytes = report.Render("PDF");
            FileStream fs = new FileStream(m_pdfPath, FileMode.OpenOrCreate);
            fs.Seek(0, SeekOrigin.Begin);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
#else
            Export(report);
            Print();
#endif
        }
        public void Dispose()
        {
            if (m_streams != null)
            {
                foreach (Stream stream in m_streams)
                    stream.Close();
                m_streams = null;
            }
        }
    }

    class lReceiptsReport:lBaseReport
    {
        public lReceiptsReport()
        {
            m_rcName = "DataSet1";
            m_viewName = "vReceipts";
            m_rdlcPath = @"..\..\receipts.rdlc";
            m_dsName = "DataSet1";
            m_pdfPath = @"..\..\report.pdf";
        }
    }
    class lInternalPaymentReport : lBaseReport
    {
        public lInternalPaymentReport()
        {
            m_rcName = "DataSet1";
            m_viewName = "v_internal_payment";
            m_rdlcPath = @"..\..\internal_payment.rdlc";
            m_dsName = "DataSet1";
            m_pdfPath = @"..\..\report.pdf";
        }
    }
    class lExternalPaymentReport : lBaseReport
    {
        public lExternalPaymentReport()
        {
            m_rcName = "DataSet1";
            m_viewName = "v_external_payment";
            m_rdlcPath = @"..\..\Report1.rdlc";
            m_dsName = "DataSet1";
            m_pdfPath = @"..\..\report.pdf";
        }
    }
#if false
    private DataTable LoadSalesData()
    {
        // Create a new DataSet and read sales data file 
        //    data.xml into the first DataTable.
        DataSet dataSet = new DataSet();
        dataSet.ReadXml(@"..\..\Data.xml");
        return dataSet.Tables[0];
    }
    private DataTable LoadReceiptsData()
    {
        // Create a new DataSet and read sales data file 
        //    data.xml into the first DataTable.
        PrintLocalReport.accountingDataSet ds = new PrintLocalReport.accountingDataSet();
        ds.vReceipts.WriteXml(xml_path);
        ds.WriteXml(xml_path);

        DataSet dataSet = new DataSet();
        dataSet.ReadXml(xml_path);
        return dataSet.Tables[0];
    }
#endif
    private DataTable loadOtherData() {
        //DataSet dataSet = new DataSet(@"..\..\vReceipts.xsd");

        string selectQuery = string.Format("SELECT * FROM {0}", view_name);

        // Establish the connection to the SQL database 
        string cnnStr = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=accounting;Integrated Security=True;Pooling=False";
        SqlConnection conn = new SqlConnection(cnnStr);
        conn.Open();

        // Connect to the SQL database using the above query to get all the data from table.
        SqlDataAdapter myCommand = new SqlDataAdapter(selectQuery, conn);

        // Create and fill a DataSet.
        DataSet ds = new DataSet();
        ds.DataSetName = ds_name;
        myCommand.Fill(ds);
        ds.Tables[0].TableName = view_name;
        //ds.WriteXml(xml_path);

        //DataSet ds2 = new DataSet();
        //ds2.ReadXml(xml_path);

        //return ds.Tables[0];
        return ds.Tables[0];
    }
    // Routine to provide to the report renderer, in order to
    //    save an image for each page of the report.
    private Stream CreateStream(string name,
      string fileNameExtension, Encoding encoding,
      string mimeType, bool willSeek)
    {
        Stream stream = new MemoryStream();
        m_streams.Add(stream);
        return stream;
    }
    // Export the given report as an EMF (Enhanced Metafile) file.
    private void Export(LocalReport report)
    {
#if true
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
        Warning[] warnings;
        m_streams = new List<Stream>();
        report.Render("Image", deviceInfo, CreateStream,
           out warnings);
        foreach (Stream stream in m_streams)
            stream.Position = 0;
#endif
    }
    // Handler for PrintPageEvents
    private void PrintPage(object sender, PrintPageEventArgs ev)
    {
        Metafile pageImage = new
           Metafile(m_streams[m_currentPageIndex]);

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

    private void Print()
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
    // Create a local report for Report.rdlc, load the data,
    //    export the report to an .emf file, and print it.
    private void Run()
    {
        LocalReport report = new LocalReport();
        bool isExist = System.IO.File.Exists(rdlc_path);
        report.ReportPath = rdlc_path;
        //report.DataSources.Add(new ReportDataSource("Sales", LoadSalesData()));
        report.DataSources.Add(new ReportDataSource(rc_name, loadOtherData()));
        report.Refresh();
#if false
        string pdfPath = @"..\..\report.pdf";
        byte[] bytes = report.Render("PDF");
        FileStream fs = new FileStream(pdfPath, FileMode.OpenOrCreate);
        fs.Seek(0, SeekOrigin.Begin);
        fs.Write(bytes, 0, bytes.Length);
        fs.Close();
#else
        Export(report);
        Print();
#endif
    }

    public void Dispose()
    {
        if (m_streams != null)
        {
            foreach (Stream stream in m_streams)
                stream.Close();
            m_streams = null;
        }
    }

    public static void Main(string[] args)
    {
        using (lExternalPaymentReport demo = new lExternalPaymentReport())
        {
            string cnnStr = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=accounting;Integrated Security=True;Pooling=False";
            SqlConnection conn = new SqlConnection(cnnStr);
            conn.Open();
            demo.init(conn);
            demo.Run();
        }
    }
}