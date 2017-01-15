#define showDtpOnClick
//#define use_newDtp

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
            dGV_receipt.CellMouseDoubleClick += dGV_receipt_CellMouseDoubleClick;
            dGV_receipt.CellBeginEdit += dGV_receipt_CellBeginEdit;
            dGV_receipt.CellEnter += dGV_receipt_CellEnter;
            dGV_receipt.CellEndEdit += dGV_receipt_CellEndEdit;

            dGV_receipt.EditingControlShowing += DGV_receipt_EditingControlShowing;

#if showDtpOnClick
            dGV_receipt.CellClick += DGV_receipt_CellClick;
            dGV_receipt.CurrentCellChanged += DGV_receipt_CurrentCellChanged;
#endif

            btn_apply.Click += new EventHandler(btn_apply_Click);
        }

#if showDtpOnClick
        private void DGV_receipt_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            m_history.addEvent(myHistory.myCellEvent.click, e.RowIndex, e.ColumnIndex);
            if (e.ColumnIndex == 1)
            {
                showDtp(e.ColumnIndex, e.RowIndex);
            }
        }

        int m_preCellCol = 0;
        int m_preCellRow= 0;
        private void DGV_receipt_CurrentCellChanged(object sender, EventArgs e)
        {
            m_history.addEvent(myHistory.myCellEvent.cellChanged);
            //if (m_preCellCol == 1)
            //{
            //    hideDtp();
            //}
            if (dGV_receipt.CurrentCell != null) {
                m_preCellCol = dGV_receipt.CurrentCell.ColumnIndex;
                m_preCellRow = dGV_receipt.CurrentCell.RowIndex;
            }
        }
#endif


        private myHistory m_history = new myHistory();
        private void dGV_receipt_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            m_history.addEvent(myHistory.myCellEvent.enter, e.RowIndex, e.ColumnIndex);
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

            //dGV_receipt.Columns[1].CellTemplate = new CalendarCell();
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

            int m_icol;
            int m_irow;
            public myDtp(int icol, int irow)
            {
                Debug.WriteLine(string.Format("new dtp {0} {1}", icol, irow));
                m_icol = icol;
                m_irow = irow;
                Format = DateTimePickerFormat.Short;
            }
            //protected override void OnCloseUp(EventArgs e)
            //{
            //    dataGridView.Controls.Remove(this);
            //    base.OnCloseUp(e);
            //}
            protected override void OnValueChanged(EventArgs eventargs)
            {
                Debug.WriteLine("OnValueChanged enter");
                // Notify the DataGridView that the contents of the cell
                // have changed.
                Debug.WriteLine("+ dataSet.Tables[0].Rows.Count " + dataSet.Tables[0].Rows.Count);
                valueChanged = true;

                //dataGridView.CurrentCell.Value = Value.ToString("yyyy-MM-dd");
                //Debug.WriteLine("+ dataGridView.Rows.Count " + dataGridView.Rows.Count);
                //DataTable tb = dataSet.Tables[0];
                //Debug.WriteLine("+ dataSet.Tables[0].Rows.Count " + dataSet.Tables[0].Rows.Count);
                base.OnValueChanged(eventargs);
                dataGridView.NotifyCurrentCellDirty(true);
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
                Debug.WriteLine("OnValueChanged dataGridView.Rows.Count " + dataGridView.Rows.Count);
                // Notify the DataGridView that the contents of the cell
                // have changed.
                valueChanged = true;
                this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
                base.OnValueChanged(eventargs);
            }

        }
        //myDtp dtp = new DateTimePicker();
#if use_newDtp
        myDtp_new dtp = null;
#else
        myDtp dtp = null;
#endif
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
            m_history.addEvent(myHistory.myCellEvent.beginEdit, e.RowIndex, e.ColumnIndex);
#if false
            if (e.ColumnIndex == 1) { 
                if (m_history.checkPartern()) { showDtp(e.ColumnIndex, e.RowIndex);}
            }
#endif
        }

        private void dGV_receipt_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            m_history.addEvent(myHistory.myCellEvent.endEdit, e.RowIndex, e.ColumnIndex);
            if (e.ColumnIndex == 1)
            {
                hideDtp();
            }
        }
        //private bool m_showDTP = false;
        //DateTimePicker m_dtp;
        private void dGV_receipt_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            m_history.addEvent(myHistory.myCellEvent.click, e.RowIndex, e.ColumnIndex);
//#if showDtpOnClick
//            if (e.ColumnIndex == 1)
//            {
//                showDtp(e.ColumnIndex, e.RowIndex);
//            }
//#endif
#if false
            if (e.ColumnIndex == 1) { 
                //if (m_history.checkPartern())
                {
                    //m_history.clear();

                    if (dtp != null)
                    {
                        Debug.WriteLine("hideDtp");
                        dtp.Hide();
                        dGV_receipt.Controls.Remove(dtp);
                        dtp = null;
                    }
                    showDtp(e.ColumnIndex, e.RowIndex);
                }
                //else { hideDtp(); }
            }
#endif
#if false
            if (e.ColumnIndex == 1)
            {
                //Initialized a new DateTimePicker Control  
                m_dtp = new DateTimePicker();

                //Adding DateTimePicker control into DataGridView   
                dGV_receipt.Controls.Add(m_dtp);

                // Setting the format (i.e. 2014-10-10)  
                m_dtp.Format = DateTimePickerFormat.Short;

                // It returns the retangular area that represents the Display area for a cell  
                Rectangle oRectangle = dGV_receipt.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);

                //Setting area for DateTimePicker Control  
                m_dtp.Size = new Size(oRectangle.Width, oRectangle.Height);

                // Setting Location  
                m_dtp.Location = new Point(oRectangle.X, oRectangle.Y);

                // An event attached to dateTimePicker Control which is fired when DateTimeControl is closed  
                m_dtp.CloseUp += new EventHandler(dtp_CloseUp);

                // An event attached to dateTimePicker Control which is fired when any date is selected  
                m_dtp.TextChanged += new EventHandler(dtp_OnTextChange);

                // Now make it visible  
                m_dtp.Visible = true;
            }
#endif
        }

        private void dGV_receipt_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
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
#if use_newDtp
            dtp = new myDtp_new(col, row);
#else
            dtp = new myDtp(col, row);
#endif
            dGV_receipt.Controls.Add(dtp);
            dtp.dataSet = dataSet;
            dtp.dataGridView = dGV_receipt;
            dtp.Format = DateTimePickerFormat.Short;
            string v = dGV_receipt.CurrentCell.Value.ToString();
            Debug.WriteLine(string.Format("+ dGV_receipt.CurrentCell.Value '{0}'", v));
            if (v != "")
            {
                DateTime result;
                if (DateTime.TryParse(v, out result)) {
                    dtp.Value = result;
                }
            }
            else
            {
                Debug.WriteLine(string.Format("+ dGV_receipt.Rows.Count '{0}'", dGV_receipt.Rows.Count));
            }
            Rectangle Rectangle = dGV_receipt.GetCellDisplayRectangle(col, row, true);
            dtp.Size = new Size(Rectangle.Width, Rectangle.Height);
            dtp.Location = new Point(Rectangle.X, Rectangle.Y);

            dtp.Show();
            ActiveControl = dtp;
            dGV_receipt.BeginEdit(true);
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
                Debug.WriteLine("+ dGV_receipt.Rows.Count " + dGV_receipt.Rows.Count);
                dtp = null;
            }
        }
        private void dtp_OnTextChange(object sender, EventArgs e)
        {
            Debug.WriteLine(string.Format("dtp_OnTextChange {0}", dtp.Value.ToString("yyyy-MM-dd")));
            dGV_receipt.CurrentCell.Value = dtp.Value.ToString("yyyy-MM-dd");
            dGV_receipt.NotifyCurrentCellDirty(true);
            //dtp.Visible = false;
        }

        void dtp_CloseUp(object sender, EventArgs e)
        {
            Debug.WriteLine("dtp_CloseUp");
            dtp.Visible = false;
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

        private void DGV_receipt_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            m_history.addEvent(myHistory.myCellEvent.editShowing);
            if (dtp != null) { 
                e.Control.Hide();
                //e.Control.Enabled = false;
            }
            //tb.KeyPress += new KeyPressEventHandler(dataGridViewTextBox_KeyPress);

            //e.Control.KeyPress += new KeyPressEventHandler(dataGridViewTextBox_KeyPress);
            //hideDtp();
        }


        private void dataGridViewTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.WriteLine("dataGridViewTextBox_KeyPress {0}", e.KeyChar);
            ////when i press enter,bellow code never run?
            //if (e.KeyChar == (char)Keys.Enter)
            //{
            //    MessageBox.Show("You press Enter");
            //}
        }

    }
}
