#define update_dgv
//#define manual_crt_col

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test_data
{
    public partial class Program
    {
        class myElapsed:IDisposable
        {
            int m_begin;
            string m_msg = "";
            public myElapsed(string msg)
            {
                m_msg = msg;
                m_begin = Environment.TickCount;
            }
            public myElapsed()
            {
                m_begin = Environment.TickCount;
            }

            public void Dispose()
            {
                Debug.WriteLine("[{0}] elapsed {1}", m_msg, Environment.TickCount - m_begin);
            }
        }
        class lDataDlg : Form
        {
            public FlowLayoutPanel m_reloadPanel;
            public FlowLayoutPanel m_sumPanel;

            public Button m_reloadBtn;
            public Button m_submitBtn;
            public Label m_status;
            public Label m_sumLabel;
            public TextBox m_sumTxt;

            public DataGridView m_dataGridView;
            BindingSource m_bs;
            DataTable m_tbl;
            SQLiteDataAdapter m_adapter;
            Thread m_thread;
            SQLiteConnection m_cnn;
            SQLiteCommand m_cmd;

            public lDataDlg()
            {
                InitializeComponent();

                initCtrls();
            }

            private void InitializeComponent()
            {
                Form form = this;
                form.Location = new Point(0, 0);
                form.Size = new Size(800, 600);
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MinimizeBox = false;
                form.MaximizeBox = false;
                
                this.Load += LDataDlg_Load;
            }
            public virtual void initCtrls()
            {
                m_reloadPanel = new FlowLayoutPanel();
                m_sumPanel = new FlowLayoutPanel();

                m_reloadBtn = new Button();
                m_submitBtn = new Button();
                m_status = new Label();
                m_sumLabel = new Label();
                m_sumTxt = new TextBox();

                m_reloadBtn.Text = "Reload";
                m_submitBtn.Text = "Save";
                m_status.AutoSize = true;
                m_status.TextAlign = ContentAlignment.MiddleLeft;
                m_status.Dock = DockStyle.Fill;

                m_reloadBtn.Click += new System.EventHandler(reloadButton_Click);
                m_submitBtn.Click += new System.EventHandler(submitButton_Click);

                m_sumLabel.Text = "Sum";

#if use_custom_dgv
                m_dataGridView = m_tblInfo.m_tblName == "internal_payment" ?
                    new lInterPaymentDGV(m_tblInfo): new lCustomDGV(m_tblInfo);
                m_dataGridView.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
                m_dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.Silver;
                m_dataGridView.EnableHeadersVisualStyles = false;
                m_dataGridView.AutoGenerateColumns = false;
                foreach(var col in m_tblInfo.m_cols) {
                    //var dgvcol = new DataGridViewColumn();
                    //dgvcol.DataPropertyName = col.m_field;
                    //dgvcol.HeaderText = col.m_alias;
                    //dgvcol.Name = col.m_field;
                    //m_dataGridView.Columns.Add(dgvcol);
                    int i = m_dataGridView.Columns.Add(col.m_field, col.m_alias);
                    m_dataGridView.Columns[i].DataPropertyName = col.m_field;
                }
#else
                m_dataGridView = new DataGridView();
#endif

                //reload panel with reload and save buttons
                m_reloadPanel.AutoSize = true;
                m_reloadPanel.AutoSizeMode = AutoSizeMode.GrowOnly;
                m_reloadPanel.Dock = DockStyle.Left;
#if DEBUG_DRAWING
                m_reloadPanel.BorderStyle = BorderStyle.FixedSingle;
#endif

                m_sumPanel.AutoSize = true;
                m_sumPanel.AutoSizeMode = AutoSizeMode.GrowOnly;
                m_sumPanel.Dock = DockStyle.Right;
#if DEBUG_DRAWING
                m_sumPanel.BorderStyle = BorderStyle.FixedSingle;
#endif

                m_reloadPanel.Controls.AddRange(new Control[] { m_reloadBtn, m_submitBtn, m_status });

                //sum panel with label and text ctrls
                m_sumPanel.Controls.AddRange(new Control[] { m_sumLabel, m_sumTxt });

                m_sumLabel.Anchor = AnchorStyles.Right;
                m_sumLabel.TextAlign = ContentAlignment.MiddleRight;
                m_sumLabel.AutoSize = true;

                m_sumTxt.Width = 100;

                m_dataGridView.Anchor = AnchorStyles.Top & AnchorStyles.Left;
                m_dataGridView.Dock = DockStyle.Fill;
                
                // +----------------+----------------+
                // |reload & save btn         sum    |
                // +----------------+----------------+
                // |data grid view                   |
                // |                                 |
                // +----------------+----------------+
                var m_panel = new TableLayoutPanel();
                m_panel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                m_panel.Dock = DockStyle.Fill;
#if DEBUG_DRAWING
                m_panel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
#endif
                //add data panel ctrls to table layout
                m_panel.Controls.Add(m_reloadPanel, 0, 1);
                m_panel.Controls.Add(m_sumPanel, 1, 1);
                m_panel.Controls.Add(m_dataGridView, 0, 2);
                m_panel.SetColumnSpan(m_dataGridView, 2);

                Controls.Add(m_panel);

                m_dataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
                m_dataGridView.KeyDown += M_dataGridView_KeyDown;
                //m_dataGridView.CellValueNeeded += M_dataGridView_CellValueNeeded;
            }

            private void M_dataGridView_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.Control)
                {
                    switch (e.KeyCode) { 
                        case Keys.C:
                            //copy to clip board
                            copyClipboard();
                        break;
                    case Keys.V:
                            //paste data
                            pasteClipboard();
                        break;
                    }
                }
            }
            DataRow m_newrow;
            private void pasteClipboard()
            {
                Debug.WriteLine("{0}.pasteClipboard {1}", this, Clipboard.GetText());
                string inTxt = Clipboard.GetText();
                var lines = inTxt.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

                int baseRow = m_dataGridView.CurrentCell.RowIndex;
                int baseCol = m_dataGridView.CurrentCell.ColumnIndex;
                DataTable tbl = m_tbl;
                int iRow = baseRow;
                foreach (var line in lines)
                {
                    bool bNewRow = false;
                    var fields = line.Split(new char[] { '\t', ';', ',' });
                    DataRow row;
                    if (iRow < tbl.Rows.Count)
                    {
                        row = tbl.Rows[iRow];
                    }
                    else
                    {
                        if (m_newrow != null)
                        {
                            row = m_newrow;
                        } else { 
                            row = tbl.NewRow();
                        }
                        m_newrow = null;
                        bNewRow = true;
                    }
                    int iCol = baseCol;
                    foreach (var field in fields)
                    {
                        //m_dataGridView[iCol, iRow].Value = field;
                        row[iCol] = field;
                        iCol++;
                    }
                    Debug.WriteLine("before tbl add row");
                    if (bNewRow) tbl.Rows.Add(row);
                    Debug.WriteLine("after tbl add row");
                    iRow++;
                }
                //m_dataGridView.CurrentCell = m_dataGridView[baseCol, baseRow];
                Debug.WriteLine("{0}.pasteClipboard end", this);
            }

            private void copyClipboard()
            {
                Clipboard.SetDataObject(m_dataGridView.GetClipboardContent());
                Debug.WriteLine("{0}.copyClipboard {1}", this, Clipboard.GetText());
            }

            private void submitButton_Click(object sender, EventArgs e)
            {
                using (SQLiteCommandBuilder builder = new SQLiteCommandBuilder(m_adapter))
                {
                    DataTable dt = m_tbl;
                    if (dt != null)
                    {
                        m_adapter.UpdateCommand = builder.GetUpdateCommand();
                        m_adapter.Update(dt);
                    }
                }
            }

            private void reloadButton_Click(object sender, EventArgs e)
            {
                int begin = Environment.TickCount;
#if true
                //m_dataGridView.VirtualMode = true;
                //m_dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                //m_dataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                m_tbl.Clear();
                using (myElapsed t = new myElapsed("fill data"))
                {
                    m_adapter.Fill(m_tbl);
                }
                int elapsed = Environment.TickCount - begin;
                m_status.Text = string.Format("Loading completed ({0} ms)", elapsed);
#if update_dgv
                update();
#endif
#else //
                m_tbl.Clear();
                //using (myElapsed t = new myElapsed("fill data"))
                //{
                //    m_adapter.Fill(m_tbl);
                //}
                //m_thread = new Thread(new ThreadStart(threadRun));
                //m_thread.Start();
                //m_status.Text = m_thread.ThreadState.ToString();
                threadRun();
#endif
            }
            public void threadRun()
            {
                Debug.WriteLine("threadRun");
                using (myElapsed t = new myElapsed("fill data"))
                {
                    //m_adapter.Fill(m_tbl);
                    SQLiteDataReader rd = m_cmd.ExecuteReader();
                    m_tbl.Columns.Add("ID", typeof(Int64));
                    m_tbl.Columns.Add("date", typeof(DateTime));
                    m_tbl.Columns.Add("receipt_number", typeof(string));
                    m_tbl.Columns.Add("name", typeof(string));
                    m_tbl.Columns.Add("content", typeof(string));
                    m_tbl.Columns.Add("amount", typeof(Int64));
                    m_tbl.Columns.Add("note", typeof(string));
                    while (rd.Read())
                    {
                        m_tbl.Rows.Add(rd[0], rd[1], rd[2], rd[3],rd[4],rd[5], rd[6]);
                    }
                    rd.Close();
                }
            }
            private void M_dataGridView_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
            {
                //BindingSource bs = (BindingSource)m_dataGridView.DataSource;
                //DataTable dt = (DataTable)bs.DataSource;
                DataTable dt = m_tbl;
                if (e.RowIndex >= dt.Rows.Count)
                    return;

                if (e.ColumnIndex >= dt.Columns.Count)
                    return;

                e.Value = dt.Rows[e.RowIndex][e.ColumnIndex];
            }

            private void update()
            {
                // Resize the DataGridView columns to fit the newly loaded content.
                //dataGridView1.AutoResizeColumns(
                //    DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
#if false
                m_dataGridView.Columns[0].Visible = false;
                lTableInfo tblInfo = m_tblInfo;
                for (int i = 1; i < m_dataGridView.ColumnCount; i++)
                {
                    m_dataGridView.Columns[i].HeaderText = tblInfo.m_cols[i].m_alias;

                    switch (tblInfo.m_cols[i].m_type)
                    {
                        case lTableInfo.lColInfo.lColType.currency:
                            m_dataGridView.Columns[i].DefaultCellStyle.Format = "#0,0";
                            break;
                        case lTableInfo.lColInfo.lColType.dateTime:
                            m_dataGridView.Columns[i].DefaultCellStyle.Format = "yyyy-MM-dd";
                            break;
                    }
                    //m_dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    //m_dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    //m_dataGridView.Columns[i].FillWeight = 1;
                }

#endif
                using (var t = new myElapsed("update time"))
                {
                    m_dataGridView.Columns[0].Visible = false;
                    //for (int i = 1; i < m_dataGridView.ColumnCount; i++)
                    //{
                    //    m_dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    //    m_dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    //    m_dataGridView.Columns[i].FillWeight = 1;
                    //}
                    m_dataGridView.Columns[6].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    m_dataGridView.Columns[6].FillWeight = 1;
                    m_dataGridView.Columns[5].DefaultCellStyle.Format = "#0,0";
                    m_dataGridView.Columns[1].DefaultCellStyle.Format = "yyyy-MM-dd";
                    Int64 sum = getSum();

                    m_sumTxt.Text = sum.ToString("#0,0");
                }
            }
            public virtual Int64 getSum()
            {
                Int64 sum = 0;
                int iCol = 5;
                for (int i = 0; i < (m_tbl.Rows.Count - 1); i++)
                {
                    Int64 val = 0;
                    if (Int64.TryParse(m_tbl.Rows[i][iCol].ToString(), out val))
                        sum += val;
                }
                return sum;
            }
            private void LDataDlg_Load(object sender, EventArgs e)
            {
                m_cnn = new SQLiteConnection(string.Format("Data Source={0};Version=3;", dbPath));
                m_cnn.Open();

                m_cmd = new SQLiteCommand();
                m_cmd.Connection = m_cnn;
                m_cmd.CommandText = "select * from receipts where rowid > ((select max(rowid) from receipts) - 10)";   //~1p
                m_adapter = new SQLiteDataAdapter(m_cmd);

                m_bs = new BindingSource();
                m_tbl = new DataTable();
                m_tbl.TableNewRow += M_tbl_TableNewRow;
                m_tbl.RowDeleted += M_tbl_RowDeleted;
                m_bs.DataSource = m_tbl;
                m_dataGridView.DataSource = m_bs;

#if manual_crt_col
                int i;
                i = m_dataGridView.Columns.Add("ID", "ID");
                m_dataGridView.Columns[i].DataPropertyName = "ID";

                //m_dataGridView.Columns.Add("date", "ngay");
                DataGridViewColumn col = new CalendarColumn();
                i = m_dataGridView.Columns.Add(col);
                col.HeaderText = "ngay thang";
                col.Name = "date";
                col.DataPropertyName = "date";
                col.SortMode = DataGridViewColumnSortMode.Automatic;
                col.DefaultCellStyle.Format = "yyyy/MM/dd";

                i = m_dataGridView.Columns.Add("receipt_number", "ma so");
                m_dataGridView.Columns[i].DataPropertyName = "receipt_number";
                

                i = m_dataGridView.Columns.Add("name", "ho ten");
                m_dataGridView.Columns[i].DataPropertyName = "name";

                DataGridViewComboBoxColumn cmbCol = new DataGridViewComboBoxColumn();
                cmbCol.HeaderText = "noi dung chi";
                cmbCol.Name = "content";
                cmbCol.DataPropertyName = "content";
                cmbCol.DataSource = new List<string>() {"su phu giao", "do hom cong duc", "other" };
                //cmbCol.Items.AddRange(new List<string>() {"su phu giao", "do hom cong duc", "other" });
                cmbCol.AutoComplete = true;
                cmbCol.SortMode = DataGridViewColumnSortMode.Automatic;
                i = m_dataGridView.Columns.Add(cmbCol);
                

                i = m_dataGridView.Columns.Add("amount", "so tien");
                m_dataGridView.Columns[i].DataPropertyName = "amount";

                i = m_dataGridView.Columns.Add("note", "ghi chu");
                m_dataGridView.Columns[i].DataPropertyName = "note";

                m_dataGridView.AutoGenerateColumns = false;

                m_dataGridView.DataError += M_dataGridView_DataError;
#else
                m_dataGridView.AutoGenerateColumns = true;
#endif
                m_dataGridView.UserAddedRow += M_dataGridView_UserAddedRow;
                m_dataGridView.RowsAdded += M_dataGridView_RowsAdded;
            }

            private void M_dataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
            {
                Debug.WriteLine("{0} rowadded {1}", this, e.RowIndex);
                if (e.RowIndex > m_tbl.Rows.Count)
                {
                    DataGridView dgv = (DataGridView)sender;
                }
            }

            private void M_dataGridView_UserAddedRow(object sender, DataGridViewRowEventArgs e)
            {
                Debug.WriteLine("{0} useraddedrow {1}", this, e.Row);
            }

            private void M_tbl_RowDeleted(object sender, DataRowChangeEventArgs e)
            {
                Debug.WriteLine("{0}.onRowDelete {1}", e.Row[0]);
            }

            private void M_tbl_TableNewRow(object sender, DataTableNewRowEventArgs e)
            {
                Debug.WriteLine("{0}.onNewRow", this);
                //DataTable tbl = (DataTable)sender;
                //tbl.Rows.Add(e.Row);
                m_newrow = e.Row;
            }

            private void M_dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
            {
                //MessageBox.Show("invalid value", "dgv data error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        [STAThread]
        static void Main(string[] args)
        {
            //crtDict();
            //gen_data();
            lDataDlg dlg = new lDataDlg();
            dlg.ShowDialog();
        }

        private static void crtDict()
        {
            string val = contents[1];
            string lower = val.ToLower();
            Encoding ascii = new ASCIIEncoding();
            Encoding unicode = new UnicodeEncoding();
            Byte[] encodedBytes = ascii.GetBytes(val);
            string v2 = ascii.GetString(encodedBytes);
            string v3 = unicode.GetString(encodedBytes);
            //Convert.()
        }

        static void load_data()
        {

        }
        static string dbPath = @"..\..\..\test_binding\appData.db";
        static void gen_data()
        {
            SQLiteConnection cnn = new SQLiteConnection(string.Format("Data Source={0};Version=3;", dbPath));
            cnn.Open();

            var cmd = new SQLiteCommand();
            cmd.Connection = cnn;
            var transaction  = cnn.BeginTransaction();
            for (int i = 0; i < 1000*1000; i++)
            {
                var rec = createReceiptsRec();
                cmd.CommandText = string.Format("insert into receipts(date, receipt_number, name, content, amount, note) "
                    + "values('{0}','{1}','{2}','{3}','{4}','{5}')", rec[0], rec[1], rec[2], rec[3], rec[4], rec[5]);
                var ret = cmd.ExecuteNonQuery();
            }
            transaction.Commit();
        }

        static string[] createReceiptsRec()
        {
            int nFields = 6;
            var rec = new string[nFields];
            rec[0] = genDate();
            rec[1] = genRcptNum();
            rec[2] = genName();
            rec[3] = genContent();
            rec[4] = genAmount();
            rec[5] = getNote();
            return rec;
        }

        static int noteNum = 0;
        static private string getNote()
        {
            return "ghi chú số " + noteNum++;
        }

        static private string genAmount()
        {
            return (rnd.Next() % Math.Pow(10, 6)).ToString() + "000";
        }

        static string[] contents = new string[] { "Sư Phụ giao", "Đổ hòm công đức", "Nguồn thu khác" };
        static private string genContent()
        {
            return contents[rnd.Next() % contents.Length];
        }

        static string[] first = new string[] { "Nguyễn", "Đào", "Ngô", "Phạm" };
        static string[] mid = new string[] {"Thị", "Văn" };
        static string[] last = new string[] {"Ngọc", "Ninh", "Minh", "An", "Minh Anh", "Bảo Châu" };
        static private string genName()
        {
            return string.Format("{0} {1} {2}",
                first[rnd.Next() % first.Length],
                mid[rnd.Next() % mid.Length],
                last[rnd.Next() % last.Length]
                );
        }

        static int m_baseNum = 0;
        static private string genRcptNum()
        {
            return string.Format("rcpt_num_{0}", m_baseNum++.ToString("000000"));
        }

        static Random rnd = new Random(Environment.TickCount);
        static int startYear = 2012;
        static int[] days = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31, 30, 31 };
        static private string genDate()
        {
            int m = rnd.Next() % 12;
            int y = rnd.Next() % 5 + startYear;
            int d = rnd.Next() % days[m] + 1;
            return string.Format("{0}-{1}-{2} 00:00:00", y, (m+1).ToString("00"), d.ToString("00"));
        }
    }
}
