using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace test_binding
{
    [DataContract(Name = "InputCtrl")]
    public class lInputCtrl : lSearchCtrl
    {
        protected new Label m_label;
        public lInputCtrl() { }
        public lInputCtrl(string fieldName, string alias, ctrlType type, Point pos, Size size)
            : base(fieldName, alias, type, pos, size)
        {
            m_label = lConfigMng.crtLabel();
            m_label.Text = alias + " : ";
            m_label.TextAlign = ContentAlignment.MiddleLeft;
            m_panel.BorderStyle = BorderStyle.None;
        }
        public virtual string Text { get; set; }
        public event EventHandler<string> EditingCompleted;
        protected virtual void onEditingCompleted()
        {
            if (EditingCompleted != null) { EditingCompleted(this, Text); }
        }
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        m_label.Dispose();
        //    }
        //    base.Dispose();
        //}
    }

    [DataContract(Name = "InputCtrlText")]
    public class lInputCtrlText : lInputCtrl
    {
        protected TextBox m_text;
        ComboBox m_combo;
        string m_value
        {
            get
            {
                if (m_text != null) return m_text.Text;
                else return m_combo.Text;
            }
        }
        public lInputCtrlText(string fieldName, string alias, ctrlType type, Point pos, Size size)
            : base(fieldName, alias, type, pos, size)
        {
            m_text = lConfigMng.crtTextBox();
            m_text.Width = 200;

            m_panel.Controls.AddRange(new Control[] { m_label, m_text });
        }
        public override void updateInsertParams(List<string> exprs, List<lSearchParam> srchParams)
        {
            exprs.Add(m_fieldName);
            srchParams.Add(
                new lSearchParam()
                {
                    key = string.Format("@{0}", m_fieldName),
                    val = m_value
                }
            );
        }
        lDataSync m_autoCompleteData;
        public override void LoadData()
        {
            if (m_colInfo != null && m_colInfo.m_lookupData != null)
            {
                m_text.Validated += M_text_Validated;
                m_autoCompleteData = m_colInfo.m_lookupData;
                AutoCompleteStringCollection col = m_autoCompleteData.m_colls;
                m_text.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                m_text.AutoCompleteSource = AutoCompleteSource.CustomSource;
                m_text.AutoCompleteCustomSource = col;
#if false
                m_combo = lConfigMng.crtComboBox();
                m_text.Hide();

                m_combo.Size = m_text.Size;

                m_panel.Controls.Remove(m_text);
                m_panel.Controls.Add(m_combo);

                DataTable tbl = m_colInfo.m_lookupData.m_dataSource;
                BindingSource bs = new BindingSource();
                bs.DataSource = tbl;
                m_combo.DataSource = bs;
                m_combo.DisplayMember = tbl.Columns[1].ColumnName;

                m_combo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                m_combo.AutoCompleteSource = AutoCompleteSource.CustomSource;
                AutoCompleteStringCollection col = m_colInfo.m_lookupData.m_colls;
                m_combo.AutoCompleteCustomSource = col;
                m_combo.Validated += M_combo_Validated;

                m_text.Dispose();
                m_text = null;
#endif
            }
        }

        private void M_text_Validated(object sender, EventArgs e)
        {
            TextBox edt = (TextBox)sender;
            Debug.WriteLine("M_text_Validated:" + edt.Text);
            string selectedValue = edt.Text;
            if (selectedValue != "")
            {
                m_autoCompleteData.Update(selectedValue);
            }
        }

        private void M_combo_Validated(object sender, EventArgs e)
        {
            string key = m_combo.Text;
            string val = m_colInfo.m_lookupData.find(key);
            if (val != null)
                m_combo.Text = val;
        }
        public override string Text
        {
            get
            {
                return m_value;
            }
            set
            {
                if (m_text != null)
                {
                    m_text.Text = value;
                }
            }

        }
    }
    [DataContract(Name = "InputCtrlDate")]
    public class lInputCtrlDate : lInputCtrl
    {
        private DateTimePicker m_date = new DateTimePicker();
        public lInputCtrlDate(string fieldName, string alias, ctrlType type, Point pos, Size size)
            : base(fieldName, alias, type, pos, size)
        {
#if fit_txt_size
            int w = lConfigMng.getWidth(lConfigMng.getDateFormat()) + 20;
#else
            int w = 100;
#endif
            m_date.Width = w;
            m_date.Format = DateTimePickerFormat.Custom;
            m_date.CustomFormat = lConfigMng.getDisplayDateFormat();
            m_date.Font = lConfigMng.getFont();

            m_panel.Controls.AddRange(new Control[] { m_label, m_date });
        }

        public override void updateInsertParams(List<string> exprs, List<lSearchParam> srchParams)
        {
            string zStartDate = m_date.Value.ToString(lConfigMng.getDateFormat());
            exprs.Add(m_fieldName);
            srchParams.Add(
                new lSearchParam()
                {
                    key = string.Format("@{0}", m_fieldName),
                    val = string.Format("{0} 00:00:00", zStartDate),
                    type = DbType.Date
                }
            );
        }
    }
    [DataContract(Name = "InputCtrlNum")]
    public class lInputCtrlNum : lInputCtrlText
    {
        public lInputCtrlNum(string fieldName, string alias, ctrlType type, Point pos, Size size)
            : base(fieldName, alias, type, pos, size)
        {
            m_text.KeyPress += onKeyPress;
        }
        private void onKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
    [DataContract(Name = "InputCtrlCurrency")]
    public class lInputCtrlCurrency : lInputCtrl
    {
        private TextBox m_val = lConfigMng.crtTextBox();
        //private Label m_lab = lConfigMng.crtLabel();
        public lInputCtrlCurrency(string fieldName, string alias, ctrlType type, Point pos, Size size)
            : base(fieldName, alias, type, pos, size)
        {
#if fit_txt_size
            int w = lConfigMng.getWidth("000,000,000,000");
#else
            int w = 100;
#endif
            m_val.Width = w;
            m_val.RightToLeft = RightToLeft.Yes;
            m_val.TextChanged += M_val_TextChanged;
            m_val.Validated += M_val_Validated;
            m_panel.Controls.AddRange(new Control[] { m_label, m_val });
        }

        private void M_val_Validated(object sender, EventArgs e)
        {
            onEditingCompleted();
        }

        private void M_val_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                (e.KeyChar != ','))
            {
                e.Handled = true;
            }

            //// only allow one decimal point
            //if ((e.KeyChar == ',') && ((sender as TextBox).Text.IndexOf(',') > -1))
            //{
            //    e.Handled = true;
            //}
        }

        //ToolTip tt = new ToolTip();
        private void M_val_TextChanged(object sender, EventArgs e)
        {
            string val;
            Int64 amount = 0;
            val = m_val.Text;
            //display in 000,000
            char[] buff = new char[64];
            Debug.Assert(val.Length < 48);
            int j = 63;
            for (int i = val.Length; i > 0; i--)
            {
                char ch = val[i - 1];
                if (ch >= '0' && ch <= '9')
                {
                    amount = amount * 10 + (ch - '0');
                    if (j % 4 == 0)
                    {
                        buff[j] = ',';
                        j--;
                    }
                    buff[j] = ch;
                    j--;
                }
            }
            val = new string(buff, j + 1, 63 - j);
            m_val.Text = val;
            m_val.Select(val.Length, 0);

            //update size
            int w = lConfigMng.getWidth(val);
            if (w > 100) m_val.Width = w;
#if display_amount_tooltip
            if (amount > 0)
            {
                tt.IsBalloon = true;
                tt.InitialDelay = 0;
                tt.ShowAlways = true;
                tt.SetToolTip(m_val, common.amountToTxt(amount));
            }
#endif
        }

        void getInputRange(out string val)
        {
            val = m_val.Text.Replace(",", "");
            if (val == "") val = "0";
        }

        public override string Text
        {
            get
            {
                return m_val.Text;
            }

            set
            {
                m_val.Text = value;
            }
        }
        public override void updateInsertParams(List<string> exprs, List<lSearchParam> srchParams)
        {
            string val;
            getInputRange(out val);
            srchParams.Add(
                new lSearchParam()
                {
                    key = "@" + m_fieldName,
                    val = val,
                    type = DbType.UInt64
                }
            );
            exprs.Add(m_fieldName);
        }
    }

    [DataContract(Name = "InputPanel")]
    public class lInputPanel
    {
        public lDataContent m_dataContent;
        public DataTable m_bills;

        //convert currency to text
        protected class rptAssist
        {
            public rptAssist(string paramName, string convField)
            {
                m_paramName = paramName;
                m_field = convField;
            }
            string m_paramName;
            string m_field;
            public List<ReportParameter> crtParams(DataTable dt)
            {
                List<string> salaryTxs = new List<string>();
                foreach (DataRow row in dt.Rows)
                {
                    if (row.RowState == DataRowState.Deleted)
                    {
                        continue;
                    }
                    //[TODO] db null
                    try
                    {
                        long amount = (long)row[m_field];
                        salaryTxs.Add(common.CurrencyToTxt(amount));
                    }
                    catch
                    {
                        Debug.Assert(false);
                        salaryTxs.Add("   ");
                    }
                }
                return new List<ReportParameter>()
                {
                    new ReportParameter(m_paramName, salaryTxs.ToArray())
                };
            }
        }
        protected rptAssist m_rptAsst;

        public virtual List<ReportParameter> billRptParams { get { return m_rptAsst.crtParams(m_bills); } }

        [DataMember(Name = "inputCtrls")]
        public List<lInputCtrl> m_inputsCtrls;

        public class PreviewEventArgs : EventArgs
        {
            public DataTable tbl;
        }
        public event EventHandler<PreviewEventArgs> RefreshPreview;
        protected string m_tblName;
        protected lTableInfo m_tblInfo { get { return appConfig.s_config.getTable(m_tblName); } }

        public TableLayoutPanel m_tbl;
        protected DataGridView m_dataGridView;
        public virtual void initCtrls()
        {
            //create input ctrls
            //List<lInputCtrl> inputCtrls = m_inputsCtrls;
            //m_inputsCtrls = new List<lInputCtrl>();
            //foreach (lInputCtrl ctrl in inputCtrls)
            //{
            //    m_inputsCtrls.Add(
            //        crtInputCtrl(
            //            m_tblInfo,
            //            ctrl.m_fieldName,
            //            ctrl.m_pos,
            //            ctrl.m_size,
            //            ctrl.m_mode
            //            )
            //        );
            //}

            //create table layout & add ctrls to
            //  +-------------------------+
            //  |search ctrl|             |
            //  +-------------------------+
            //  |search ctrl|             |
            //  +-------------------------+
            //  |    save btn|delete btn  |
            //  +-------------------------+
            //  +-------------------------+
            //  |    grid view            |
            //  +-------------------------+
            m_tbl = new TableLayoutPanel();
#if DEBUG_DRAWING
                m_tbl.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
#endif

            //add search ctrls to table layout
            int lastRow = 0;
            foreach (var ctrl in m_inputsCtrls)
            {
                m_tbl.Controls.Add(ctrl.m_panel, ctrl.m_pos.X, ctrl.m_pos.Y);
                m_tbl.SetColumnSpan(ctrl.m_panel, ctrl.m_size.Width);
                m_tbl.SetRowSpan(ctrl.m_panel, ctrl.m_size.Height);
                lastRow = Math.Max(lastRow, ctrl.m_pos.Y);
            }

            //add buttons
            Button m_addBtn = lConfigMng.crtButton();
            m_addBtn.Text = "Add";
            m_addBtn.Click += M_addBtn_Click;
            Button m_saveBtn = lConfigMng.crtButton();
            m_saveBtn.Text = "Save";
            m_saveBtn.Click += M_saveBtn_Click;
            Button m_editBtn = lConfigMng.crtButton();
            m_editBtn.Text = "Edit";
            m_editBtn.Click += M_editBtn_Click;
            Button m_clearBtn = lConfigMng.crtButton();
            m_clearBtn.Text = "Clear";
            m_clearBtn.Click += M_clearBtn_Click;
            FlowLayoutPanel tflow = new FlowLayoutPanel();
            tflow.FlowDirection = FlowDirection.LeftToRight;
            tflow.Controls.AddRange(new Control[] { m_addBtn, m_saveBtn, m_clearBtn });
            tflow.AutoSize = true;

            //  add buttons to last row
            m_tbl.Controls.Add(tflow, 0, ++lastRow);

            // add data grid view
            m_dataGridView = lConfigMng.crtDGV();
            m_dataGridView.EnableHeadersVisualStyles = false;
            m_dataGridView.Dock = DockStyle.Fill;
            m_dataGridView.CellClick += M_dataGridView_CellClick;

            m_tbl.Controls.Add(m_dataGridView, 0, ++lastRow);
            m_tbl.Dock = DockStyle.Fill;
            m_tbl.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
        }

        private void M_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var rows = m_dataGridView.SelectedRows;
            if (rows.Count > 0)
            {
                var row = rows[0];
                DataRow dr = ((DataRowView)row.DataBoundItem).Row;
                m_bills.Rows.Clear();
                m_bills.ImportRow(dr);

                if (RefreshPreview != null) { RefreshPreview(this, null); }
            }
        }

        private void Clear()
        {
            m_dataGridView.CancelEdit();
            m_dataContent.m_dataTable.Clear();

            //clean bills
            m_bills.Rows.Clear();
            if (RefreshPreview != null) { RefreshPreview(this, null); }
        }
        private void M_clearBtn_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void M_editBtn_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        bool bIncKeyReq;
        private void M_addBtn_Click(object sender, EventArgs e)
        {
            Add();
        }
        protected virtual void Add()
        {
            //check key is unique
            string key = m_keyCtrl.Text;
            if (!m_keyMng.IsUniqKey(key))
            {
                Debug.Assert(false);
                lConfigMng.showInputError("Mã này đã tồn tại");
                m_keyCtrl.Text = m_keyMng.IncKey();
                return;
            }

            List<string> exprs = new List<string>();
            List<lSearchParam> srchParams = new List<lSearchParam>();
            foreach (lSearchCtrl ctrl in m_inputsCtrls)
            {
                ctrl.updateInsertParams(exprs, srchParams);
            }

            //crt new row
            DataTable tbl = m_dataContent.m_dataTable;
            DataRow newRow = tbl.NewRow();
            for (int i = 0; i < exprs.Count; i++)
            {
                string field = exprs[i];
                var newValue = srchParams[i].val;
                newRow[field] = newValue;
            }
            tbl.Rows.Add(newRow);
            m_dataContent.Submit();

            //add new record to bills
            m_bills.Rows.Clear();
            m_bills.ImportRow(newRow);
            if (RefreshPreview != null) { RefreshPreview(this, null); }

            //inc key
            bIncKeyReq = true;
        }
        protected virtual lInputCtrl m_keyCtrl { get; }

        protected virtual keyMng m_keyMng { get; }
        protected virtual string InitKey()
        {
            return m_keyMng.InitKey();
        }
        protected virtual string IncKey()
        {
            return m_keyMng.IncKey();
        }
        private void M_saveBtn_Click(object sender, EventArgs e)
        {
            Save();

            //if (RefreshPreview != null)
            //{
            //    RefreshPreview(this, new PreviewEventArgs { tbl = m_dataContent.m_dataTable });
            //}
        }
        protected virtual void Save()
        {
            m_bills.Clear();
            if (RefreshPreview != null) { RefreshPreview(this, null); }

            m_dataContent.Submit();
        }
        protected virtual void InsertRec()
        {
            List<string> exprs = new List<string>();
            List<lSearchParam> srchParams = new List<lSearchParam>();
            foreach (lSearchCtrl ctrl in m_inputsCtrls)
            {
                ctrl.updateInsertParams(exprs, srchParams);
            }

            m_dataContent.AddRec(exprs, srchParams);
        }
        protected virtual void Dispose(bool disposing)
        {

        }

        public virtual void LoadData()
        {
            m_tblInfo.LoadData();
            //m_dataGridView.AutoGenerateColumns = false;
            //crtColumns();
            m_dataContent = appConfig.s_contentProvider.CreateDataContent(m_tblInfo.m_tblName);

            //init gridview
            m_dataGridView.DataSource = m_dataContent.m_bindingSource;
            DataTable tbl = (DataTable)m_dataContent.m_bindingSource.DataSource;
            if (tbl != null)
            {
                updateCols();
                m_dataGridView.AutoGenerateColumns = false;
            }

            m_keyCtrl.Text = InitKey();
            m_dataContent.UpdateTableCompleted += M_dataContent_UpdateTableCompleted;

            //load input ctrls data
            foreach (var ctrl in m_inputsCtrls)
            {
                ctrl.LoadData();
            }

            //init bill table
            m_bills = tbl.Copy();
        }

        private void M_dataContent_UpdateTableCompleted(object sender, lDataContent.FillTableCompletedEventArgs e)
        {
            //if add compeplete
            if (bIncKeyReq)
            {
                m_keyCtrl.Text = IncKey();
                bIncKeyReq = false;
            }
#if single_preview
            //if delete or add complete
            if (RefreshPreview != null)
            {
                RefreshPreview(this, new PreviewEventArgs { tbl = m_dataContent.m_dataTable });
            }
#endif
        }

        private void crtColumns()
        {
            int i = 0;
            foreach (var field in m_tblInfo.m_cols)
            {
#if !use_custom_cols
                i = m_dataGridView.Columns.Add(field.m_field, field.m_alias);
                var dgvcol = m_dataGridView.Columns[i];
#else
                    DataGridViewColumn dgvcol;
                    if (field.m_type == lTableInfo.lColInfo.lColType.dateTime)
                    {
                        dgvcol = new CalendarColumn();
                        dgvcol.SortMode = DataGridViewColumnSortMode.Automatic;
                    }
                    else if (field.m_lookupTbl != null)
                    {
                        var cmb = new DataGridViewComboBoxColumn();
                        DataTable tbl = field.m_lookupData.m_dataSource;
                        BindingSource bs = new BindingSource();
                        bs.DataSource = tbl;
                        cmb.DataSource = bs;
                        cmb.DisplayMember = tbl.Columns[1].ColumnName;
                        cmb.AutoComplete = true;
                        cmb.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                        cmb.FlatStyle = FlatStyle.Flat;
                        dgvcol = cmb;
                        dgvcol.SortMode = DataGridViewColumnSortMode.Automatic;
                    }
                    else
                    { 
                        dgvcol = new DataGridViewTextBoxColumn();
                    }
                    i = m_dataGridView.Columns.Add(dgvcol);
                    dgvcol.HeaderText = field.m_alias;
                    dgvcol.Name = field.m_field;
#endif //use_custom_cols
                dgvcol.DataPropertyName = field.m_field;
                switch (field.m_type)
                {
#if format_currency
                    case lTableInfo.lColInfo.lColType.currency:
                        dgvcol.DefaultCellStyle.Format = lConfigMng.getCurrencyFormat();
                        break;
#endif
                    case lTableInfo.lColInfo.lColType.dateTime:
                        dgvcol.DefaultCellStyle.Format = lConfigMng.getDisplayDateFormat();
                        break;
                }
            }
            m_dataGridView.Columns[0].Visible = false;
            //last columns
            var lastCol = m_dataGridView.Columns[i];
            lastCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            lastCol.FillWeight = 1;
#if set_column_order
            for (; i > 0; i--)
            {
                m_dataGridView.Columns[i].DisplayIndex = i - 1;
            }
#endif
        }

        private void updateCols()
        {
            m_dataGridView.Columns[0].Visible = false;
            lTableInfo tblInfo = m_tblInfo;
            int i = 1;
            for (; i < m_dataGridView.ColumnCount; i++)
            {
                //show hide columns
                if (tblInfo.m_cols[i].m_visible == false)
                {
                    m_dataGridView.Columns[i].Visible = false;
                    continue;
                }

                m_dataGridView.Columns[i].HeaderText = tblInfo.m_cols[i].m_alias;

#if header_blue
                //header color blue
                m_dataGridView.Columns[i].HeaderCell.Style.BackColor = Color.Blue;
                m_dataGridView.Columns[i].HeaderCell.Style.ForeColor = Color.White;
#endif

                switch (tblInfo.m_cols[i].m_type)
                {
                    case lTableInfo.lColInfo.lColType.currency:
                        m_dataGridView.Columns[i].DefaultCellStyle.Format = lConfigMng.getCurrencyFormat();
                        break;
                    case lTableInfo.lColInfo.lColType.dateTime:
                        m_dataGridView.Columns[i].DefaultCellStyle.Format = lConfigMng.getDisplayDateFormat();
                        break;
                }
#if false
                    m_dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    m_dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    m_dataGridView.Columns[i].FillWeight = 1;
#endif
#if set_column_order
                m_dataGridView.Columns[i].DisplayIndex = i - 1;
#endif
            }
            m_dataGridView.Columns[i - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            m_dataGridView.Columns[i - 1].FillWeight = 1;
        }
        public lInputCtrl crtInputCtrl(lTableInfo tblInfo, string colName, Point pos, Size size)
        {
            return crtInputCtrl(tblInfo, colName, pos, size, lSearchCtrl.SearchMode.match);
        }
        public lInputCtrl crtInputCtrl(lTableInfo tblInfo, string colName, Point pos, Size size, lSearchCtrl.SearchMode mode)
        {
            int iCol = tblInfo.getColIndex(colName);
            if (iCol != -1)
            {
                return crtInputCtrl(tblInfo, iCol, pos, size, mode);
            }
            return null;
        }
        public lInputCtrl crtInputCtrl(lTableInfo tblInfo, int iCol, Point pos, Size size)
        {
            return crtInputCtrl(tblInfo, iCol, pos, size, lSearchCtrl.SearchMode.match);
        }
        public lInputCtrl crtInputCtrl(lTableInfo tblInfo, int iCol, Point pos, Size size, lSearchCtrl.SearchMode mode)
        {
            lTableInfo.lColInfo col = tblInfo.m_cols[iCol];
            switch (col.m_type)
            {
                case lTableInfo.lColInfo.lColType.text:
                case lTableInfo.lColInfo.lColType.uniqueText:
                    lInputCtrlText textCtrl = new lInputCtrlText(col.m_field, col.m_alias, lSearchCtrl.ctrlType.text, pos, size);
                    textCtrl.m_mode = mode;
                    textCtrl.m_colInfo = col;
                    return textCtrl;
                case lTableInfo.lColInfo.lColType.dateTime:
                    lInputCtrlDate dateCtrl = new lInputCtrlDate(col.m_field, col.m_alias, lSearchCtrl.ctrlType.dateTime, pos, size);
                    return dateCtrl;
                case lTableInfo.lColInfo.lColType.num:
                    lInputCtrlNum numCtrl = new lInputCtrlNum(col.m_field, col.m_alias, lSearchCtrl.ctrlType.num, pos, size);
                    return numCtrl;
                case lTableInfo.lColInfo.lColType.currency:
                    lInputCtrlCurrency currencyCtrl = new lInputCtrlCurrency(col.m_field, col.m_alias, lSearchCtrl.ctrlType.currency, pos, size);
                    return currencyCtrl;
            }
            return null;
        }
    }
    public class keyMng
    {
        string m_preFix;
        string m_tblName;
        string m_keyField;
        string m_dateField;
        Regex m_reg;
        DateTime m_preDate;
        int m_preNo;
        public keyMng(string preFix, string tblName, string keyField, string dateField = "date")
        {
            m_preFix = preFix;
            m_tblName = tblName;
            m_keyField = keyField;
            m_dateField = dateField;
            //@"(PT\d{8})(\d{3})"
            m_reg = new Regex(@"(\d{8})(\d{3})");
        }
        private string genKey(DateTime date, int n)
        {
            string zKey = m_preFix + date.ToString("yyyyMMdd") + n.ToString("D3");
            m_preDate = date;
            m_preNo = n;
            return zKey;
        }
        public string InitKey()
        {
            DateTime curDate = DateTime.Now.Date;
            //PTyyyyMMdd001
            string zKey = genKey(curDate, 1);
            string zDate = curDate.ToString(lConfigMng.getDateFormat());
            string sql = string.Format("select {0} from {1} where {2} = '{3} 00:00:00' order by {0}",
                m_keyField, m_tblName, m_dateField, zDate);
            DataTable tbl = appConfig.s_contentProvider.GetData(sql);
            if (tbl.Rows.Count > 0)
            {
                int no = 1;
                int i = 1;
                for (; i <= tbl.Rows.Count; i++)
                {
                    string curKey = tbl.Rows[i - 1][0].ToString();
                    Match m = m_reg.Match(curKey);
                    no = int.Parse(m.Groups[2].Value);
                    if (i != no) break;
                }
                zKey = genKey(curDate, i);
            }
            if (!IsUniqKey(zKey))
            {
                Debug.Assert(false);    //invalid manual gen key
                zKey = IncKey();
            }
            return zKey;
        }
        public bool IsUniqKey(string val)
        {
            var bRet = true;
            string sql = string.Format("select id, {0} from {1} where {0} = '{2}'",
                m_keyField, m_tblName, val);
            var tbl = appConfig.s_contentProvider.GetData(sql);
            if (tbl.Rows.Count > 0)
            {
                Debug.WriteLine("{0} {1} not unique value {2}", this, "checkUniqKey() check unique", val);
                bRet = false;
            }
            return bRet;
        }
        public string IncKey()
        {
            //PTyyyyMMdd000
            string newKey = "";
            //string curKey = m_preKey;
            //Match m = m_reg.Match(curKey);
            //if (m.Success)
            {
                //int no = int.Parse(m.Groups[2].Value);
                int no = m_preNo + 1;
                for (; no < 999; no++)
                {
                    newKey = genKey(m_preDate, no);
                    if (IsUniqKey(newKey))
                    {
                        break;
                    }
                }
            }
            return newKey;
        }
    }

    [DataContract(Name = "ReceiptsInputPanel")]
    public class lReceiptsInputPanel : lInputPanel
    {
        protected override lInputCtrl m_keyCtrl { get { return m_inputsCtrls[0]; } }
        private keyMng m_key;
        protected override keyMng m_keyMng { get { return m_key; } }
        public lReceiptsInputPanel()
        {
            m_tblName = "receipts";

            m_inputsCtrls = new List<lInputCtrl> {
                crtInputCtrl(m_tblInfo, "receipt_number", new Point(0, 0), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "date"          , new Point(0, 1), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "name"          , new Point(0, 2), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "addr"          , new Point(0, 3), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "content"       , new Point(0, 4), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "note"          , new Point(0, 5), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "amount"        , new Point(0, 6), new Size(1, 1)),
            };
            m_key = new keyMng("PT", m_tblName, "receipt_number");

            m_rptAsst = new rptAssist("amountTxts", "amount");
        }
    }
    [DataContract(Name = "InterPayInputPanel")]
    public class lInterPayInputPanel : lInputPanel
    {
        protected override lInputCtrl m_keyCtrl { get { return m_inputsCtrls[0]; } }
        private keyMng m_key;
        protected override keyMng m_keyMng { get { return m_key; } }
        public lInterPayInputPanel()
        {
            m_tblName = "internal_payment";
#if has_advance
            lInputCtrl advance_payment = crtInputCtrl(m_tblInfo, "advance_payment", new Point(0, 6), new Size(1, 1));
            lInputCtrl reimbursement = crtInputCtrl(m_tblInfo, "reimbursement", new Point(0, 7), new Size(1, 1));
            lInputCtrl actually_spent = crtInputCtrl(m_tblInfo, "actually_spent", new Point(0, 8), new Size(1, 1));
#endif
            m_inputsCtrls = new List<lInputCtrl>
            {
                crtInputCtrl(m_tblInfo, "payment_number"    , new Point(0, 0), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "date"              , new Point(0, 1), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "name"              , new Point(0, 2), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "addr"              , new Point(0, 3), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "group_name"        , new Point(0, 4), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "content"           , new Point(0, 5), new Size(1, 1)),
#if has_advance
                advance_payment,
                reimbursement,
                actually_spent,
                crtInputCtrl(m_tblInfo, "note"              , new Point(0, 9), new Size(1, 1)),
#else
                crtInputCtrl(m_tblInfo, "advance_payment"   , new Point(0, 6), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "note"              , new Point(0, 7), new Size(1, 1)),
#endif
            };
#if has_advance
            reimbursement.EditingCompleted += (s, e) =>
            {
                long advance, reimbur;
                bool bRet = long.TryParse(advance_payment.Text.Replace(",", ""), out advance);
                bRet &= long.TryParse(reimbursement.Text.Replace(",", ""), out reimbur);
                if (bRet)
                {
                    Debug.Assert(advance >= reimbur);
                    actually_spent.Text = (advance - reimbur).ToString();
                }
            };
#endif
            m_key = new keyMng("PCN", m_tblName, "payment_number");

            m_rptAsst = new rptAssist("advanceTxts", "advance_payment" );
        }
        public override void initCtrls()
        {
            base.initCtrls();

            m_dataGridView.CellEndEdit += (s, e) =>
            {
                //if (e.ColumnIndex == )
                //{

                //}
            };
        }
    }
    [DataContract(Name = "ExterPayInputPanel")]
    public class lExterPayInputPanel : lInputPanel
    {
        protected override lInputCtrl m_keyCtrl { get { return m_inputsCtrls[0]; } }
        private keyMng m_key;
        protected override keyMng m_keyMng { get { return m_key; } }
        public lExterPayInputPanel()
        {
            m_tblName = "external_payment";
            m_inputsCtrls = new List<lInputCtrl>
            {
                crtInputCtrl(m_tblInfo, "payment_number", new Point(0, 0), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "date"          , new Point(0, 1), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "name"          , new Point(0, 2), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "addr"          , new Point(0, 3), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "content"       , new Point(0, 4), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "constr_org"    , new Point(0, 5), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "building"      , new Point(0, 6), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "group_name"    , new Point(0, 7), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "spent"         , new Point(0, 8), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "note"          , new Point(0, 9), new Size(1, 1)),
            };
            m_key = new keyMng("PCG", m_tblName, "payment_number");
            m_rptAsst = new rptAssist("spentTxts", "spent");
        }
    }
    [DataContract(Name = "SalaryInputPanel")]
    public class lSalaryInputPanel : lInputPanel
    {
        protected override lInputCtrl m_keyCtrl { get { return m_inputsCtrls[0]; } }
        private keyMng m_key;
        protected override keyMng m_keyMng { get { return m_key; } }
        public lSalaryInputPanel()
        {
            m_tblName = "salary";

            m_inputsCtrls = new List<lInputCtrl> {
                crtInputCtrl(m_tblInfo, "payment_number", new Point(0, 0), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "month"         , new Point(0, 1), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "date"          , new Point(0, 1), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "name"          , new Point(0, 2), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "addr"          , new Point(0, 3), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "group_name"    , new Point(0, 4), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "content"       , new Point(0, 5), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "salary"        , new Point(0, 6), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "note"          , new Point(0, 7), new Size(1, 1)),
            };
            m_key = new keyMng("PCL", m_tblName, "payment_number");
            m_rptAsst = new rptAssist("salaryTxts", "salary");
        }
    }
}

