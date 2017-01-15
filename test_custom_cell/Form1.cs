using System;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data;
using System.Drawing;
using System.Diagnostics;

namespace CBV_KeToan
{
    public partial class Form1 : Form
    {
        SQLiteConnection m_dbConnection;
        DataSet dataSet;

        public Form1()
        {
            InitializeComponent();

            //open db
            string dbPath = "test.db";
            if (!System.IO.File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile("test.db");
            }

            m_dbConnection = new SQLiteConnection("Data Source=test.db; Version=3;");
            m_dbConnection.Open();

            string sql = "CREATE TABLE if not exists  external_payment("
                + "ID INTEGER PRIMARY KEY AUTOINCREMENT, date datetime,"
                + "name char(31), content char(255), group_name char(31),"
                + "spent real, note char(255));";
            sql += "CREATE TABLE if not exists  receipts("
                + "ID INTEGER PRIMARY KEY AUTOINCREMENT, date datetime, "
                + "name char(31), content char(255),"
                + "price INTEGER, note char(255) );";
            sql += "CREATE TABLE if not exists internal_payment("
                + "ID INTEGER PRIMARY KEY AUTOINCREMENT, date datetime, name char(31),"
                + "content char(255), group_name char(31),"
                + "advance_payment INTEGER, reimbursement INTEGER,"
                + "actually_spent INTEGER, note char(255));";
            sql += "CREATE TABLE if not exists  salary("
                + "ID INTEGER PRIMARY KEY AUTOINCREMENT, month INT8, date datetime,"
                + "name char(31), group_name char(31),"
                + "content char(255), salary INTEGER, note char(255));";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();


            //dtp.CloseUp += new EventHandler(dtp_CloseUp);
            //dtp.TextChanged += new EventHandler(dtp_OnTextChange);
            //dtp.Visible = false;

            content.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            content.AutoCompleteSource = AutoCompleteSource.HistoryList;


            //dGV_receipt.CellClick += new DataGridViewCellEventHandler(dGV_receipt_CellClick);
            dGV_receipt.CellMouseClick += new DataGridViewCellMouseEventHandler(dGV_receipt_CellMouseClick);
            dGV_receipt.CellMouseDoubleClick += new DataGridViewCellMouseEventHandler(dGV_receipt_CellMouseDoubleClick);
            dGV_receipt.CellBeginEdit += new DataGridViewCellCancelEventHandler(dGV_receipt_CellBeginEdit);
            dGV_receipt.CellEnter += new DataGridViewCellEventHandler(dGV_receipt_CellEnter);
            dGV_receipt.CellEndEdit += new DataGridViewCellEventHandler(dGV_receipt_CellEndEdit);

            btn_apply.Click += new EventHandler(btn_apply_Click);
        }

        class myHistory {
            int m_cursor = 0;
            int m_count = 0;
            int m_rowindex = 0;
            int m_colindex = 0;
            static int m_data_size = 8;
            public enum myCellEvent
            {
                enter,
                click,
                edit,
                doubleClick
            }
            myCellEvent[] m_data = new myCellEvent[m_data_size];
            public myHistory()
            {

            }
            public void addEvent(myCellEvent e, int rowindex, int colindex)
            {
                Debug.WriteLine("addEvent e {0} row {1} col {2}", e, rowindex, colindex);
                do
                {
                    if (rowindex != m_rowindex) { clear(); break; }
                    if (colindex != m_colindex) { clear(); break; }
                    if (e == m_data[m_cursor]) return;
                } while (false);

                m_rowindex = rowindex;
                m_colindex = colindex;

                if (m_count == 0)
                {
                    m_count = 1;
                    m_cursor = 0;
                    m_data[0] = e;
                }
                else
                {
                    m_count++;
                    m_cursor = (m_cursor + 1) % m_data_size;
                    m_data[m_cursor] = e;
                }

                printData();
            }
            private void printData()
            {
                Debug.WriteLine("history data {0}", m_count);
                int n = Math.Min(m_data_size, m_count);
                for (int i = 0; i < n; i++)
                {
                    int index = (i + m_cursor + m_data_size - n + 1) % m_data_size;
                    Debug.WriteLine("  {0} {1}", i, m_data[index]);
                }
            }
            public void clear()
            {
                m_count = 0;
            }
            private myCellEvent[] m_ptn = new myCellEvent[] {
                    myCellEvent.enter,
                    myCellEvent.click,
                    myCellEvent.doubleClick,
                    myCellEvent.edit,
            };
            public bool checkPartern()
            {
                Debug.WriteLine("checkPartern");
                do
                {
                    int n = m_ptn.Length;
                    if (m_count < n) break;
                    int i = 0;
                    int sum = 0;
                    for (; i<n;i++ )
                    {
                        int index = (i + m_cursor + m_data_size - n + 1) % m_data_size;
                        if (m_data[index] != m_ptn[i]) break;
                    }
                    if (i < n) break;
                    return true;
                } while (false);
                return false;
            }
        }

        private myHistory m_history = new myHistory();
        private void dGV_receipt_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            Debug.WriteLine("dGV_receipt_CellEnter");
            m_history.addEvent(myHistory.myCellEvent.enter, e.RowIndex, e.ColumnIndex);
            Debug.WriteLine(string.Format(" m_history check {0}", m_history.checkPartern()));
        }

  
        private void btn_apply_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("btn_apply_Click");
#if true
                using (SQLiteCommandBuilder builder = new SQLiteCommandBuilder(m_dataAdapter))
#else
            using (SqlCommandBuilder builder = new SqlCommandBuilder(m_dataAdapter))
#endif
            {
                DataTable dt = (DataTable)m_bindingSource.DataSource;
                if (dt != null)
                {
                    m_dataAdapter.UpdateCommand = builder.GetUpdateCommand();
                    m_dataAdapter.Update(dt);
                }
            }
        }

        SQLiteDataAdapter m_dataAdapter;
        public BindingSource m_bindingSource = new BindingSource();
        private void btn_Search_Click(object sender, EventArgs e)
        {
            //get input keys
            DateTime startDate = dTP_startDate.Value;
            DateTime endDate = dTP_endDate.Value;

            dataSet = new DataSet();
            string sql = "select * from receipts;";
            m_dataAdapter = new SQLiteDataAdapter(sql, m_dbConnection);
            m_dataAdapter.Fill(dataSet);
            //DataTable table = new DataTable();
            //m_dataAdapter.Fill(table);
            m_bindingSource.DataSource = dataSet.Tables[0];
            //dGV_receipt.DataSource = dataSet.Tables[0].DefaultView;
            dGV_receipt.DataSource = m_bindingSource;

            dGV_receipt.Columns[1].CellTemplate = new CalendarCell();
        }

        private void btn_addNew_Click(object sender, EventArgs e)
        {
            string sql = "INSERT INTO receipts(date, name, content, price, note)"
                + "VALUES('2016-12-30', 'name 1', 'content 1', 200, 'note 1');";
            SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection);
            command.ExecuteNonQuery();
        }

        class myDtp:DateTimePicker
        {
            public DataSet dataSet;
            public DataGridView dataGridView;
            public bool valueChanged = false;
            int rowIndex;

            int m_icol;
            int m_irow;
            public myDtp(int icol, int irow)
            {
                Debug.WriteLine(string.Format("new dtp {0} {1}", icol, irow));
                m_icol = icol;
                m_irow = irow;
                Format = DateTimePickerFormat.Short;
            }
            protected override void OnValueChanged(EventArgs eventargs)
            {
                Debug.WriteLine("OnValueChanged enter");
                // Notify the DataGridView that the contents of the cell
                // have changed.
                Debug.WriteLine("+ dataSet.Tables[0].Rows.Count " + dataSet.Tables[0].Rows.Count);
                valueChanged = true;
                this.dataGridView.NotifyCurrentCellDirty(true);
                base.OnValueChanged(eventargs);
                Debug.WriteLine("+ dataGridView.Rows.Count " + dataGridView.Rows.Count);
                DataTable tb = dataSet.Tables[0];
                Debug.WriteLine("+ dataSet.Tables[0].Rows.Count " + dataSet.Tables[0].Rows.Count);
            }
        }
        class myDtp_new:DateTimePicker, IDataGridViewEditingControl
        {
            public DataSet dataSet;
            public DataGridView dataGridView;
            public bool valueChanged = false;
            int rowIndex;

            int m_icol;
            int m_irow;
            public myDtp_new(int icol,int irow)
            {
                Debug.WriteLine(string.Format("new dtp {0} {1}", icol, irow));
                m_icol = icol;
                m_irow = irow;
                Format = DateTimePickerFormat.Short;
            }

            public DataGridView EditingControlDataGridView
            {
                get
                {
                    return dataGridView;
                }

                set
                {
                    dataGridView = value;
                }
            }

            public object EditingControlFormattedValue
            {
                get
                {
                    return this.Value.ToShortDateString();
                }

                set
                {
                    if (value is String)
                    {
                        try
                        {
                            // This will throw an exception of the string is 
                            // null, empty, or not in the format of a date.
                            this.Value = DateTime.Parse((String)value);
                        }
                        catch
                        {
                            // In the case of an exception, just use the 
                            // default value so we're not left with a null
                            // value.
                            this.Value = DateTime.Now;
                        }
                    }
                }
            }

            public int EditingControlRowIndex
            {
                get
                {
                    return rowIndex;
                }
                set
                {
                    rowIndex = value;
                }
            }

            public bool EditingControlValueChanged
            {
                get
                {
                    return valueChanged;
                }

                set
                {
                    valueChanged = value;
                }
            }

            public Cursor EditingPanelCursor
            {
                get
                {
                    return base.Cursor;
                }
            }

            public bool RepositionEditingControlOnValueChange
            {
                get
                {
                    return false;
                }
            }

            public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
            {
                this.Font = dataGridViewCellStyle.Font;
                this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
                this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
            }

            public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
            {
                // Let the DateTimePicker handle the keys listed.
                switch (keyData & Keys.KeyCode)
                {
                    case Keys.Left:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Right:
                    case Keys.Home:
                    case Keys.End:
                    case Keys.PageDown:
                    case Keys.PageUp:
                        return true;
                    default:
                        return !dataGridViewWantsInputKey;
                }
            }

            public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
            {
                return EditingControlFormattedValue;
            }

            public void PrepareEditingControlForEdit(bool selectAll)
            {
                //throw new NotImplementedException();

            }

            protected override void OnValueChanged(EventArgs eventargs)
            {
                // Notify the DataGridView that the contents of the cell
                // have changed.
                valueChanged = true;
                this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
                base.OnValueChanged(eventargs);
                Debug.WriteLine("OnValueChanged 227 dataGridView.Rows.Count " + dataGridView.Rows.Count);
                //DataTable tb = dataSet.Tables[0];
                //DataRow row = null;
                //if ((tb.Rows.Count) == m_irow)
                //{
                //    row = dataSet.Tables[0].NewRow();
                //    dataSet.Tables[0].Rows.Add(row);
                //}else { 
                //    row = tb.Rows[m_irow];
                //}
                //row.ItemArray[m_icol] = Value.ToString("yyyy-MM-dd");
                //row.AcceptChanges();
                //dataSet.Tables[0].Rows[m_irow].[m_icol].Value = Value.ToString("yyyy-MM-dd");
            }
        }
        //myDtp dtp = new DateTimePicker();
        myDtp_new dtp = null;
        //private void dGV_receipt_CellClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    Debug.WriteLine(string.Format("dGV_receipt_CellClick {0} {1}", e.ColumnIndex, e.RowIndex));
        //    {
        //        if (e.ColumnIndex == 1)
        //        {
        //            //dGV_receipt.Controls.Remove(dtp);
        //            dGV_receipt.Controls.Add(dtp);
        //            dtp.EditingControlDataGridView = dGV_receipt;
        //            dGV_receipt.NotifyCurrentCellDirty(true);
        //            dtp.Format = DateTimePickerFormat.Short;
        //            Rectangle Rectangle = dGV_receipt.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

        //            dtp.Size = new Size(Rectangle.Width, Rectangle.Height);
        //            dtp.Location = new Point(Rectangle.X, Rectangle.Y);

        //            //dtp.Visible = true;
        //            dtp.Show();
        //            ActiveControl = dtp;
        //            //m_isEditing = true;
        //        }
        //    }
        //}
        //private bool m_isEditing = false;
        private void dGV_receipt_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            Debug.WriteLine("dGV_receipt_CellBeginEdit begin");
            Debug.WriteLine(string.Format("col row: {0} {1}", e.ColumnIndex, e.RowIndex));
            m_history.addEvent(myHistory.myCellEvent.edit, e.RowIndex, e.ColumnIndex);
            Debug.WriteLine(string.Format(" m_history check {0}", m_history.checkPartern()));
#if true
            if (e.ColumnIndex == 1) { 
                if (m_history.checkPartern()) { showDtp(e.ColumnIndex, e.RowIndex);}
            }
#endif
        }

        private void dGV_receipt_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Debug.WriteLine("dGV_receipt_CellEndEdit {0} {1}", e.ColumnIndex, e.RowIndex);
            if (e.ColumnIndex == 1) hideDtp();
        }
      //private bool m_showDTP = false;
        private void dGV_receipt_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Debug.WriteLine("dGV_receipt_CellMouseClick");
            m_history.addEvent(myHistory.myCellEvent.click, e.RowIndex, e.ColumnIndex);
            Debug.WriteLine(string.Format(" m_history check {0}", m_history.checkPartern()));
#if false
            if (e.ColumnIndex == 1) { 
                if (m_history.checkPartern())
                {
                    m_history.clear();
                    showDtp(e.ColumnIndex, e.RowIndex);
                }
                //else { hideDtp(); }
            }
#endif
        }

        private void dGV_receipt_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Debug.WriteLine("dGV_receipt_CellMouseDoubleClick");
            m_history.addEvent(myHistory.myCellEvent.doubleClick, e.RowIndex, e.ColumnIndex);

#if false
            //bool res =  m_history.checkPartern();
            bool res = true;
            if (e.ColumnIndex == 1 && res)
            { 
                showDtp(e.ColumnIndex, e.RowIndex);
            }
#endif
        }

        private void showDtp(int col, int row)
        {
            Debug.WriteLine("showDtp");
            //dGV_receipt.Controls.Remove(dtp);
            dtp = new myDtp_new(col, row);
            dGV_receipt.Controls.Add(dtp);
            dtp.dataSet = dataSet;
            dtp.dataGridView = dGV_receipt;
            dtp.Format = DateTimePickerFormat.Short;
            string v = dGV_receipt.CurrentCell.Value.ToString();
            Debug.WriteLine("  dGV_receipt.CurrentCell.Value " + v);
            if (v != "")
            {
                DateTime result;
                if (DateTime.TryParse(v, out result)) {
                    dtp.Value = result;
                }
            }
            else
            {
                Debug.WriteLine("dGV_receipt.Rows.Count " + dGV_receipt.Rows.Count);
            }
            Rectangle Rectangle = dGV_receipt.GetCellDisplayRectangle(col, row, true);

            dtp.Size = new Size(Rectangle.Width, Rectangle.Height);
            dtp.Location = new Point(Rectangle.X, Rectangle.Y);

            dtp.Show();
            ActiveControl = dtp;
            //m_isEditing = true;
        }
        private void hideDtp()
        {
            if (dtp != null)
            {
                Debug.WriteLine("hideDtp");
                dtp.Hide();
                if (dtp.valueChanged)
                {
                    dGV_receipt.CurrentCell.Value = dtp.Value.ToString("yyyy-MM-dd");
                }
                dGV_receipt.Controls.Remove(dtp);
                Debug.WriteLine("dGV_receipt.Rows.Count " + dGV_receipt.Rows.Count);
                dtp = null;
            }
        }
        private void dtp_OnTextChange(object sender, EventArgs e)
        {
            Debug.WriteLine(string.Format("dtp_OnTextChange {0}", dtp.Value.ToString("yyyy-MM-dd")));
            dGV_receipt.CurrentCell.Value = dtp.Value.ToString("yyyy-MM-dd");
            //dtp.Visible = false;
        }
        void dtp_CloseUp(object sender, EventArgs e)
        {
            Debug.WriteLine("dtp_CloseUp");
            //if (m_isEditing)
            //{
            //    dtp.Hide();
            //    dGV_receipt.Controls.Remove(dtp);
            //    m_isEditing = false;
            //}
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {

            switch (keyData & Keys.KeyCode)
            {
                case Keys.Enter:
                case Keys.Tab:
                    this.dGV_receipt.Focus();
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);

        }

    }
}
