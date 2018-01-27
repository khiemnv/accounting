﻿using System;
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
    public class lInputCtrl : lSearchCtrl
    {
        protected new Label m_label;
        public lInputCtrl(string fieldName, string alias, ctrlType type, Point pos, Size size)
            : base(fieldName, alias, type, pos, size)
        {
            m_label = lConfigMng.crtLabel();
            m_label.Text = alias + " : ";
            m_label.TextAlign = ContentAlignment.MiddleLeft;
            m_panel.BorderStyle = BorderStyle.None;
        }
        public virtual string Text { get; set; }
    }

    [DataContract(Name = "lInputCtrlText")]
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
        public override void LoadData()
        {
            if (m_colInfo != null && m_colInfo.m_lookupData != null)
            {
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
            //m_val.KeyPress += textBox1_KeyPress;

            //m_lab.AutoSize = true;

            //FlowLayoutPanel group = new FlowLayoutPanel();
            //group.FlowDirection = FlowDirection.LeftToRight;
            //group.AutoSize = true;

            //group.Controls.AddRange(new Control[] { m_label, m_val});
            m_panel.Controls.AddRange(new Control[] { m_label, m_val });
            //m_panel.FlowDirection = FlowDirection.TopDown;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
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

    public class lInputPanel
    {
        public lDataContent m_dataContent;
        public virtual List<ReportParameter> billRptParams { get; }
        public List<lInputCtrl> m_inputsCtrls;

        public class PreviewEventArgs : EventArgs
        {
            public DataTable tbl;
        }
        static Dictionary<string, EventHandler<PreviewEventArgs>> m_dict = new Dictionary<string, EventHandler<PreviewEventArgs>>();
        private EventHandler<PreviewEventArgs> mRefreshPreview;
        public event EventHandler<PreviewEventArgs> RefreshPreview
        {
            add
            {
                addEvent("RefreshPreview",ref mRefreshPreview, value);
            }
            remove
            {
            }
        }
        private void addEvent(string zType, ref EventHandler<PreviewEventArgs> handler, EventHandler<PreviewEventArgs> value)
        {
            string key = zType + value.Target.ToString();
            if (!m_dict.ContainsKey(key))
            {
                m_dict.Add(key, value);
                handler += value;
            }
            else
            {
                handler -= m_dict[key];
                m_dict[key] = value;
                handler += value;
            }
        }

        protected string m_tblName;
        protected lTableInfo m_tblInfo { get { return appConfig.s_config.getTable(m_tblName); } }

        public TableLayoutPanel m_tbl;
        DataGridView m_dataGridView;
        public virtual void initCtrls()
        {
            //create input ctrls

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
            foreach (lSearchCtrl searchCtrl in m_inputsCtrls)
            {
                m_tbl.Controls.Add(searchCtrl.m_panel, searchCtrl.m_pos.X, searchCtrl.m_pos.Y);
                m_tbl.SetColumnSpan(searchCtrl.m_panel, searchCtrl.m_size.Width);
                m_tbl.SetRowSpan(searchCtrl.m_panel, searchCtrl.m_size.Height);
                lastRow = Math.Max(lastRow, searchCtrl.m_pos.Y);
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

            m_tbl.Controls.Add(m_dataGridView, 0, ++lastRow);
            m_tbl.Dock = DockStyle.Fill;
            m_tbl.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
        }

        private void M_clearBtn_Click(object sender, EventArgs e)
        {
            m_dataGridView.CancelEdit();
            m_dataContent.m_dataTable.Clear();
            if (mRefreshPreview != null) { mRefreshPreview(this, null); }
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

            //sync
            //if (RefreshPreview != null)
            //{
            //    RefreshPreview(this, new PreviewEventArgs { tbl = tbl });
            //}
            bIncKeyReq = true;
        }
        protected virtual lInputCtrl m_keyCtrl{get;}

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
            m_dataGridView.DataSource = m_dataContent.m_bindingSource;
            DataTable tbl = (DataTable)m_dataContent.m_bindingSource.DataSource;
            if (tbl != null)
            {
                updateCols();
                m_dataGridView.AutoGenerateColumns = false;
            }

            m_keyCtrl.Text = InitKey();
            m_dataContent.UpdateTableCompleted += M_dataContent_UpdateTableCompleted;
        }

        private void M_dataContent_UpdateTableCompleted(object sender, lDataContent.FillTableCompletedEventArgs e)
        {
            //if add compeplete
            if (bIncKeyReq)
            {
                m_keyCtrl.Text = IncKey();
                bIncKeyReq = false;
            }
            //if delete or add complete
            if (mRefreshPreview != null)
                mRefreshPreview(this, new PreviewEventArgs { tbl = m_dataContent.m_dataTable });
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
        string m_preKey;
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
            string zKey = m_preFix +  date.ToString("yyyyMMdd") + n.ToString("D3");
            m_preKey = zKey;
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
                int no = 0;
                int i = 0;
                for (; i < tbl.Rows.Count && i == no; i++)
                {
                    string curKey = tbl.Rows[i][0].ToString();
                    Match m = m_reg.Match(curKey);
                    no = int.Parse(m.Groups[2].Value);
                }
                zKey = genKey(curDate, 1);
            }
            return zKey;
        }
        bool checkUniqKey(string val)
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
            string curKey = m_preKey;
            Match m = m_reg.Match(curKey);
            if (m.Success)
            {
                int no = int.Parse(m.Groups[2].Value);
                for (no++; no < 999; no++)
                {
                    newKey = m.Groups[1].Value + no.ToString("D3");
                    if (checkUniqKey(newKey))
                    {
                        break;
                    }
                }
            }
            return newKey;
        }
    }
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
                crtInputCtrl(m_tblInfo, "date", new Point(0, 1), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "name", new Point(0, 2), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "addr", new Point(0, 3), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "content", new Point(0, 4), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "note", new Point(0, 5), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "amount", new Point(0, 6), new Size(1, 1)),
            };
            m_key = new keyMng("PT", m_tblName, "receipt_number");
        }
        public override List<ReportParameter> billRptParams
        {
            get
            {
                List<string> amountTxs = new List<string>();
                foreach (DataRow row in m_dataContent.m_dataTable.Rows)
                {
                    if (row.RowState == DataRowState.Deleted) continue;
                    //[TODO] db null
                    try
                    {
                        long amount = (long)row["amount"];
                        amountTxs.Add(common.CurrencyToTxt(amount));
                    }
                    catch
                    {
                        Debug.Assert(false);
                        amountTxs.Add("   ");
                    }
                }
                return new List<ReportParameter>()
                {
                    new ReportParameter("amountTxts", amountTxs.ToArray())
                };
            }
        }
    }
    public class lInterPayInputPanel : lInputPanel
    {
        private keyMng m_key;
        protected override keyMng m_keyMng { get { return m_key; } }
        public lInterPayInputPanel()
        {
            m_tblName = "internal_payment";
            m_inputsCtrls = new List<lInputCtrl>
            {
                crtInputCtrl(m_tblInfo, "date", new Point(0, 0), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "payment_number", new Point(0, 1), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "name", new Point(1, 0), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "group_name", new Point(1, 1), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "advance_payment", new Point(0, 2), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "reimbursement", new Point(1, 2), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "content", new Point(0, 2), new Size(1, 1)),
            };
            m_key = new keyMng("PCN", m_tblName, "payment_number");
        }
    }
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
                crtInputCtrl(m_tblInfo, "payment_number", new Point(0, 1), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "date", new Point(0, 0), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "name", new Point(0, 2), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "addr", new Point(0, 3), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "group_name", new Point(0, 4), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "building", new Point(0, 5), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "content", new Point(0, 6), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "note", new Point(0, 7), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "spent", new Point(0, 8), new Size(1, 1)),
            };
            m_key = new keyMng("PCG", m_tblName, "payment_number");
        }
        public override List<ReportParameter> billRptParams
        {
            get
            {
                List<string> spentTxs = new List<string>();
                foreach (DataRow row in m_dataContent.m_dataTable.Rows)
                {
                    if (row.RowState == DataRowState.Deleted) continue;
                    //[TODO] db null
                    try
                    {
                        long amount = (long)row["spent"];
                        spentTxs.Add(common.CurrencyToTxt(amount));
                    }
                    catch
                    {
                        Debug.Assert(false);
                        spentTxs.Add("   ");
                    }
                }
                return new List<ReportParameter>()
                {
                    new ReportParameter("spentTxts", spentTxs.ToArray())
                };
            }
        }
    }
    public class lSalaryInputPanel : lInputPanel
    {
        protected override lInputCtrl m_keyCtrl { get { return m_inputsCtrls[0]; } }
        private keyMng m_key;
        protected override keyMng m_keyMng { get { return m_key; } }
        public lSalaryInputPanel()
        {
            m_tblName = "receipts";

            m_inputsCtrls = new List<lInputCtrl> {
                crtInputCtrl(m_tblInfo, "receipt_number", new Point(0, 0), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "date", new Point(0, 1), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "name", new Point(0, 2), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "addr", new Point(0, 3), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "content", new Point(0, 4), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "note", new Point(0, 5), new Size(1, 1)),
                crtInputCtrl(m_tblInfo, "amount", new Point(0, 6), new Size(1, 1)),
            };
            m_key = new keyMng("PT", m_tblName, "receipt_number");
        }
    }

}
