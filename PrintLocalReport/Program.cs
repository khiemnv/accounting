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
        using (Demo demo = new Demo())
        {
            demo.Run();
        }
    }
}