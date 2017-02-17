using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Data.SQLite;

namespace PrintLocalReport
{

    public class ProgressDlg : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.m_descrTxt = new System.Windows.Forms.Label();
            this.m_cancelBtn = new System.Windows.Forms.Button();
            this.m_percentTxt = new System.Windows.Forms.Label();
            this.m_stepTxt = new System.Windows.Forms.Label();
            this.m_prg = new System.Windows.Forms.ProgressBar();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            //this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.m_descrTxt, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.m_cancelBtn, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.m_percentTxt, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_stepTxt, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.m_prg, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 42.85714F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 19.04762F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 14.28571F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 23.80952F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 131);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.m_descrTxt.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.m_descrTxt, 3);
            this.m_descrTxt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_descrTxt.Location = new System.Drawing.Point(4, 1);
            this.m_descrTxt.Name = "label1";
            this.m_descrTxt.Size = new System.Drawing.Size(276, 54);
            this.m_descrTxt.TabIndex = 0;
            this.m_descrTxt.Text = "descripton";
            this.m_descrTxt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            this.m_cancelBtn.Location = new System.Drawing.Point(216, 101);
            this.m_cancelBtn.Name = "button1";
            this.m_cancelBtn.Size = new System.Drawing.Size(64, 23);
            this.m_cancelBtn.TabIndex = 3;
            this.m_cancelBtn.Text = "Cancel";
            this.m_cancelBtn.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.m_percentTxt.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.m_percentTxt.AutoSize = true;
            this.m_percentTxt.Location = new System.Drawing.Point(14, 80);
            this.m_percentTxt.Name = "label3";
            this.m_percentTxt.Size = new System.Drawing.Size(44, 13);
            this.m_percentTxt.TabIndex = 4;
            this.m_percentTxt.Text = "Percent";
            // 
            // label4
            // 
            this.m_stepTxt.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.m_stepTxt.AutoSize = true;
            this.m_stepTxt.Location = new System.Drawing.Point(234, 80);
            this.m_stepTxt.Name = "label4";
            this.m_stepTxt.Size = new System.Drawing.Size(27, 13);
            this.m_stepTxt.TabIndex = 5;
            this.m_stepTxt.Text = "step";
            // 
            // progressBar1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.m_prg, 3);
            this.m_prg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.m_prg.Location = new System.Drawing.Point(4, 59);
            this.m_prg.Name = "progressBar1";
            this.m_prg.Size = new System.Drawing.Size(276, 17);
            this.m_prg.TabIndex = 6;
            // 
            // ProgressDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 131);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressDlg";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Form1";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label m_descrTxt;
        private System.Windows.Forms.Button m_cancelBtn;
        private System.Windows.Forms.Label m_percentTxt;
        private System.Windows.Forms.Label m_stepTxt;
        private System.Windows.Forms.ProgressBar m_prg;

        public object m_param;
        public Int64 m_endPos;
        public Int64 m_scale;
        public string m_descr;
        public myCursor m_cursor;
        public bool m_isCancel = false;
        int m_timeOut;
        enum state
        {
            init,
            completed,
            canceled,
            closed,
        };
        state m_state = state.init;
        Thread m_task;
        Int64 m_nStep;

#if false
        class myMutex
        {
            Mutex m_mutex;
            public myMutex()
            {
                m_mutex = new Mutex();
            }
            public void enter()
            {
                Debug.WriteLine("{0}.enter {1}", this, Thread.CurrentThread.ManagedThreadId);
                m_mutex.WaitOne();
                Debug.WriteLine("+ enter done");
            }
            public void leave()
            {
                Debug.WriteLine("{0}.leave {1}", this, Thread.CurrentThread.ManagedThreadId);
                m_mutex.ReleaseMutex();
                Debug.WriteLine("+ leave done");
            }
        }
#endif

        public ProgressDlg()
        {
            InitializeComponent();
#if enable_cancel
                m_cancelBtn.Click += M_cancelBtn_Click;
#else
            m_cancelBtn.Visible = false;
            this.ControlBox = false;
            //this.FormBorderStyle = FormBorderStyle.None;
#endif

            this.Load += ProgressDlg_Load;
            m_endPos = 1000;
            m_scale = 1;

            m_timeOut = 100;        //100 ms
        }

        void cancel()
        {
#if enable_cancel
            m_isCancel = true;
#endif
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (m_state == state.init)
            {
                cancel();
                e.Cancel = true;
            }
        }

        private void M_cancelBtn_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("{0}.M_cancelBtn_Click cur thread {1}", this, Thread.CurrentThread.ManagedThreadId);
            cancel();
        }

        private void ProgressDlg_Load(object sender, EventArgs e)
        {
            this.Text = m_descr;
            if (m_cursor == null)
                m_prg.Style = ProgressBarStyle.Marquee;
            start();
        }

        void start()
        {
            m_nStep = m_endPos / m_scale;
            m_prg.Maximum = (int)m_nStep;
            m_prg.Value = 0;
            m_task = new Thread(new ThreadStart(() =>
            {
                for (int i = 1; ; i++)
                {
                    Int64 curPos = m_cursor.getPos();
                    if (curPos == m_endPos)
                    {
                        m_state = state.completed;
                        break;
                    }
                    if (m_isCancel)
                    {
                        m_state = state.canceled;
                        break;
                    }
                    incPrgCallback((int)(curPos / m_scale), i / 10);
                    Thread.Sleep(m_timeOut);
                    Debug.WriteLine("{0}.m_task cur thread {1} elapsed {2} s",
                        this,
                        Thread.CurrentThread.ManagedThreadId,
                        i / 10);
                }
                closeDlgCallback();
            }
            ));
            m_task.Start();
        }

        delegate void lCloseDlg();
        void closeDlgCallback()
        {
            Debug.WriteLine("{0}.closeDlgCallback cur thread {1}", this,
                Thread.CurrentThread.ManagedThreadId);
            if (this.m_prg.InvokeRequired)
            {
                Debug.WriteLine("+ invoked req");
                //call it self in form thread
                lCloseDlg d = new lCloseDlg(this.closeDlgCallback);
                this.Invoke(d);
            }
            else
            {
                Debug.WriteLine("+ close");
                this.Close();
            }
        }

        delegate void lIncProgress(int iStep, int elapsed);
        void incPrgCallback(int iStep, int elapsed)
        {
            Debug.WriteLine("{0}.incPrgCallback cur thread {1}", this, Thread.CurrentThread.ManagedThreadId);
            if (this.m_prg.InvokeRequired)
            {
                Debug.WriteLine("+ invoke req");
                //call it self in form thread
                lIncProgress d = new lIncProgress(this.incPrgCallback);
                this.Invoke(d, new object[] { iStep, elapsed });
            }
            else
            {
                Debug.WriteLine("+ method body {0} {1}", iStep, elapsed);
                this.m_prg.Value = iStep;
                double percent = (double)iStep / m_nStep;
                m_percentTxt.Text = percent.ToString("0.00%");
                m_stepTxt.Text = string.Format("{0}/{1}", iStep, m_nStep);
                m_descrTxt.Text = string.Format("{0} \nElapsed {1} s", m_descr, elapsed);
            }
        }
    }


    public interface myCursor
    {
        Int64 getPos();
    }

    [DataContract(Name = "Report")]
    public class lBaseReport : IDisposable, myCursor
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
        public string m_pdfPath;    //print to pdf file

        public Dictionary<string, string> m_sqls;

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

        protected virtual DataTable loadData(string sql)
        {
            throw new NotImplementedException();
            //string qry = string.Format("SELECT * FROM {0}", m_viewName);
            //DataTable dt = appConfig.s_contentProvider.GetData(qry);
            //return dt;
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
            foreach (Stream stream in m_streams)
            {
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

        public virtual List<ReportParameter> getReportParam()
        {
            throw new NotImplementedException();
        }

        private delegate void voidCaller();

        long m_iWork;
        public long getPos()
        {
            return m_iWork;
        }

        public void Run()
        {
            ProgressDlg prg = new ProgressDlg();
            var d = new voidCaller(() => {
                //display wait msg
                prg.m_descr = "Load view data ...";

                LocalReport report = new LocalReport();

                //long time work
                DataSet ds = new DataSet();
                int step = 50 / m_sqls.Count;
                foreach (var pair in m_sqls)
                {
                    DataTable dt;
                    dt = loadData(pair.Value);
                    //after load data complete
                    m_iWork += step;

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
                prg.m_descr = "Exporting ...";

                //long time work
                Export(report);
                m_iWork = 100;

                ds.Clear();
                ds.Dispose();
                report.Dispose();
            });

            var t = d.BeginInvoke(null, null);
            m_iWork = 0;
            prg.m_endPos = 100;
            prg.m_cursor = this;
            prg.m_param = t;
            prg.ShowDialog();
            prg.Dispose();

            //print
#if false
            byte[] bytes = report.Render("PDF");
            FileStream fs = new FileStream(m_pdfPath, FileMode.Create);
            fs.Seek(0, SeekOrigin.Begin);
            fs.Write(bytes, 0, bytes.Length);
            fs.Close();
#else
            Print();
#endif
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
    public class lReportDlg: Form
    {
        #region gui
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.paymentRadio = new System.Windows.Forms.RadioButton();
            this.buildingRadio = new System.Windows.Forms.RadioButton();
            this.remainRadio = new System.Windows.Forms.RadioButton();
            this.yearRadio = new System.Windows.Forms.RadioButton();
            this.startDate = new System.Windows.Forms.DateTimePicker();
            this.endDate = new System.Windows.Forms.DateTimePicker();
            this.paymentRptType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buildingCmb = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.printBtn = new System.Windows.Forms.Button();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // paymentRadio
            // 
            this.paymentRadio.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.paymentRadio.AutoSize = true;
            this.paymentRadio.Checked = true;
            this.paymentRadio.Location = new System.Drawing.Point(3, 29);
            this.paymentRadio.Name = "paymentRadio";
            this.paymentRadio.Size = new System.Drawing.Size(82, 17);
            this.paymentRadio.TabIndex = 0;
            this.paymentRadio.TabStop = true;
            this.paymentRadio.Text = "Báo cáo chi";
            this.paymentRadio.UseVisualStyleBackColor = true;
            // 
            // buildingRadio
            // 
            this.buildingRadio.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.buildingRadio.AutoSize = true;
            this.buildingRadio.Location = new System.Drawing.Point(3, 54);
            this.buildingRadio.Name = "buildingRadio";
            this.buildingRadio.Size = new System.Drawing.Size(73, 17);
            this.buildingRadio.TabIndex = 1;
            this.buildingRadio.TabStop = true;
            this.buildingRadio.Text = "Công trình";
            this.buildingRadio.UseVisualStyleBackColor = true;
            // 
            // remainRadio
            // 
            this.remainRadio.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.remainRadio.AutoSize = true;
            this.remainRadio.Location = new System.Drawing.Point(3, 79);
            this.remainRadio.Name = "remainRadio";
            this.remainRadio.Size = new System.Drawing.Size(63, 17);
            this.remainRadio.TabIndex = 2;
            this.remainRadio.TabStop = true;
            this.remainRadio.Text = "Kiểm kê";
            this.remainRadio.UseVisualStyleBackColor = true;
            // 
            // yearRadio
            // 
            this.yearRadio.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.yearRadio.AutoSize = true;
            this.yearRadio.Location = new System.Drawing.Point(3, 104);
            this.yearRadio.Name = "yearRadio";
            this.yearRadio.Size = new System.Drawing.Size(88, 17);
            this.yearRadio.TabIndex = 3;
            this.yearRadio.TabStop = true;
            this.yearRadio.Text = "Báo cáo năm";
            this.yearRadio.UseVisualStyleBackColor = true;
            // 
            // startDate
            // 
            this.startDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.startDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.startDate.Location = new System.Drawing.Point(197, 3);
            this.startDate.Name = "startDate";
            this.startDate.Size = new System.Drawing.Size(94, 20);
            this.startDate.TabIndex = 4;
            // 
            // endDate
            // 
            this.endDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.endDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.endDate.Location = new System.Drawing.Point(97, 3);
            this.endDate.Name = "endDate";
            this.endDate.Size = new System.Drawing.Size(94, 20);
            this.endDate.TabIndex = 5;
            // 
            // paymentRptType
            // 
            this.paymentRptType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableLayoutPanel1.SetColumnSpan(this.paymentRptType, 2);
            this.paymentRptType.FormattingEnabled = true;
            this.paymentRptType.Location = new System.Drawing.Point(97, 28);
            this.paymentRptType.Name = "paymentRptType";
            this.paymentRptType.Size = new System.Drawing.Size(194, 21);
            this.paymentRptType.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(66, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Ngày Tháng";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // buildingCmb
            // 
            this.buildingCmb.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableLayoutPanel1.SetColumnSpan(this.buildingCmb, 2);
            this.buildingCmb.FormattingEnabled = true;
            this.buildingCmb.Location = new System.Drawing.Point(97, 53);
            this.buildingCmb.Name = "buildingCmb";
            this.buildingCmb.Size = new System.Drawing.Size(194, 21);
            this.buildingCmb.TabIndex = 10;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.buildingCmb, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.startDate, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.endDate, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.yearRadio, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.paymentRadio, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.remainRadio, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.paymentRptType, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.buildingRadio, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.printBtn, 2, 5);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(295, 154);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // printBtn
            // 
            this.printBtn.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.printBtn.Location = new System.Drawing.Point(207, 128);
            this.printBtn.Name = "printBtn";
            this.printBtn.Size = new System.Drawing.Size(75, 23);
            this.printBtn.TabIndex = 11;
            this.printBtn.Text = "Print";
            this.printBtn.UseVisualStyleBackColor = true;
            // 
            // lReportDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(295, 154);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "lReportDlg";
            this.Text = "Report";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton paymentRadio;
        private System.Windows.Forms.RadioButton buildingRadio;
        private System.Windows.Forms.RadioButton remainRadio;
        private System.Windows.Forms.RadioButton yearRadio;
        private System.Windows.Forms.DateTimePicker startDate;
        private System.Windows.Forms.DateTimePicker endDate;
        private System.Windows.Forms.ComboBox paymentRptType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ComboBox buildingCmb;
        private System.Windows.Forms.Button printBtn;
        #endregion

        enum receiptsRptType
        {
            byDays,
            byWeek,
            byMonth,
            byYear,
        }
        Dictionary<receiptsRptType, string> m_receiptRptTypes;

        public lReportDlg()
        {
            InitializeComponent();

            startDate.CustomFormat = "yyyy-MM-dd";
            endDate.CustomFormat = "yyyy-MM-dd";

            m_receiptRptTypes = new Dictionary<receiptsRptType, string> {
                {receiptsRptType.byDays, "Báo cáo theo ngày" },
                {receiptsRptType.byWeek, "Báo cáo theo tuần" },
                {receiptsRptType.byMonth, "Báo cáo theo tháng" },
                {receiptsRptType.byYear, "Báo cáo theo năm" },
            };
            foreach (var val in m_receiptRptTypes.Values) { 
                paymentRptType.Items.Add(val);
            }
            paymentRptType.SelectedIndex = 0;

            printBtn.Click += PrintBtn_Click;
            Load += LReportDlg_Load;
        }

        private void PrintBtn_Click(object sender, EventArgs e)
        {
            lBaseReport rpt = null;
            if (paymentRadio.Checked)
            {
                switch (paymentRptType.SelectedIndex)
                {
                    case (int)receiptsRptType.byDays:
                        rpt = new lDaysReport(startDate.Value, endDate.Value);
                        break;
                    case (int)receiptsRptType.byWeek:
                        rpt = new lWeekReport(startDate.Value, endDate.Value);
                        break;
                    case (int)receiptsRptType.byMonth:
                        rpt = new lMonthReport(startDate.Value, endDate.Value);
                        break;
                }
            }
            if (rpt != null) { 
                rpt.Run();
                rpt.Dispose();
            }
        }

        private void LReportDlg_Load(object sender, EventArgs e)
        {
            //
        }
    }

    public class config {
        static SQLiteConnection m_cnn;
        public static SQLiteConnection get_cnn()
        {
            string dbPath = @"..\..\..\test_binding\appData.db";
            if (m_cnn == null) { 
                m_cnn = new SQLiteConnection(string.Format("Data Source={0};Version=3;", dbPath));
                m_cnn.Open();
            }
            return m_cnn;
        }
    }

    public class lDaysReport : lBaseReport
    {
        protected SQLiteConnection m_cnn;
        List<ReportParameter> m_rptParams;
        protected virtual string getDateQry(string zStartDate, string zEndDate)
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

        protected string getMonthQry(string zStartDate, string zEndDate)
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
        protected virtual string getType()
        {
            return "Ngày";
        }
        public lDaysReport(DateTime endDate, DateTime startDate)
        {
            string zStartDate = startDate.ToString("yyyy-MM-dd");
            string zEndDate = endDate.ToString("yyyy-MM-dd");
            m_rptParams = new List<ReportParameter>()
            {
                new ReportParameter("startDate",zStartDate),
                new ReportParameter("endDate",zEndDate),
                new ReportParameter( "type", getType())
            };

            m_sqls = new Dictionary<string, string>
            {
                { "DataSet1", getDateQry(zStartDate, zEndDate)},
                { "DataSet2", getMonthQry(zStartDate, zEndDate)}
            };

            m_rdlcPath = @"..\..\receipts.rdlc";
        }
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing) { 
                m_sqls.Clear();
            }
        }
        protected override DataTable loadData(string qry)
        {
            SQLiteDataAdapter cmd = new SQLiteDataAdapter(qry, config.get_cnn());
            DataTable dt = new DataTable();
            cmd.Fill(dt);
            return dt;
        }
        public override List<ReportParameter> getReportParam()
        {
            return m_rptParams;
        }
    }

    public class lWeekReport : lDaysReport
    {
        protected override string getDateQry(string zStartDate, string zEndDate)
        {
            string qryWeeksData = string.Format("select group_name, week as date, '' as name,"
               + " sum(inter_pay) as inter_pay, sum(exter_pay) as exter_pay, sum(salary) as salary"
               + " from"
               + " (select group_name, strftime('%Y-%W', date) as week, actually_spent as inter_pay, 0 as exter_pay, 0 as salary"
               + " from internal_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
               + " union"
               + " select group_name, strftime('%Y-%W', date) as week, 0 as inter_pay, spent as exter_pay, 0 as salary"
               + " from external_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
               + " union"
               + " select group_name, strftime('%Y-%W', date) as week, 0 as inter_pay, 0 as exter_pay, salary"
               + " from salary where date between '{0} 00:00:00' and '{1} 00:00:00')"
               + " group by group_name, week",
               zStartDate, zEndDate);
            return qryWeeksData;
        }
        protected override string getType()
        {
            return "Tuần";
        }
        public lWeekReport(DateTime endDate, DateTime startDate) : base(endDate, startDate)
        {
        }
    }
    public class lMonthReport : lDaysReport
    {
        protected override string getDateQry(string zStartDate, string zEndDate)
        {
            string qryMonthsData = string.Format("select group_name, month as date, '' as name,"
               + " sum(inter_pay) as inter_pay, sum(exter_pay) as exter_pay, sum(salary) as salary"
               + " from"
               + " (select group_name, strftime('%Y-%m', date) as month, actually_spent as inter_pay, 0 as exter_pay, 0 as salary"
               + " from internal_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
               + " union"
               + " select group_name, strftime('%Y-%m', date) as month, 0 as inter_pay, spent as exter_pay, 0 as salary"
               + " from external_payment where date between '{0} 00:00:00' and '{1} 00:00:00'"
               + " union"
               + " select group_name, strftime('%Y-%m', date) as month, 0 as inter_pay, 0 as exter_pay, salary"
               + " from salary where date between '{0} 00:00:00' and '{1} 00:00:00')"
               + " group by group_name, month",
               zStartDate, zEndDate);
            return qryMonthsData;
        }
        protected override string getType()
        {
            return "Tháng";
        }
        public lMonthReport(DateTime endDate, DateTime startDate) : base(endDate, startDate)
        {
        }
    }
}
