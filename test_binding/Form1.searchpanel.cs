//#define DEBUG_DRAWING
#define use_cmd_params

using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System;
using System.Data;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace test_binding
{
    public partial class Form1 : Form
    {

        class lEntity
        {
            public lEntity(string param, string value)
            {
                this.m_param = param;
                this.m_value = value;
            }
            public string m_param;
            public string m_value;
        };

        [DataContract(Name ="SearchCtrl")]
        class lSearchCtrl:IDisposable
        {
            public enum ctrlType
            {
                text,
                dateTime,
                num,
                currency
            };
            public lTableInfo.lColInfo m_colInfo;
            [DataMember(Name = "field", EmitDefaultValue = false)]
            public string m_fieldName;
            public string m_alias;
            public ctrlType m_type;
            [DataMember(Name = "pos", EmitDefaultValue = false)]
            public Point m_pos;
            [DataMember(Name = "size", EmitDefaultValue = false)]
            public Size m_size;

            [DataContract(Name = "SeachMode")]
            public enum SearchMode
            {
                [EnumMember]
                like,
                [EnumMember]
                match
            };

            [DataMember(Name = "mode", EmitDefaultValue = false)]
            public SearchMode m_mode = SearchMode.like;

            public FlowLayoutPanel m_panel = new FlowLayoutPanel();
            public CheckBox m_label = new CheckBox();

            public lSearchCtrl() { }
            public lSearchCtrl(string fieldName, string alias, ctrlType type, Point pos, Size size)
            {
                m_fieldName = fieldName;
                m_alias = alias;
                m_type = type;
                m_pos = pos;
                m_size = size;

                m_label.Text = alias;
                m_label.Width = 100;
                m_label.TextAlign = ContentAlignment.MiddleLeft;
                m_panel.AutoSize = true;
#if true
                m_panel.BorderStyle = BorderStyle.FixedSingle;
#endif
            }

            public virtual void updateSearchParams(List<string> exprs, Dictionary<string,string> srchParams){}
            public virtual string getSearchParams() { return null; }
            public virtual void LoadData(){}
            protected virtual void valueChanged(object sender, EventArgs e)
            {
                m_label.Checked = true;
            }

            public virtual void Dispose()
            {
                m_panel.Dispose();
                m_label.Dispose();
            }
        };

        [DataContract(Name = "SearchCtrlText")]
        class lSearchCtrlText : lSearchCtrl
        {
            protected TextBox m_text;
            ComboBox m_combo;
            string m_value {
                get {
                    if (m_text != null) return m_text.Text;
                    else return m_combo.Text;
                }
            }
            public lSearchCtrlText(string fieldName, string alias, ctrlType type, Point pos, Size size)
                : base(fieldName, alias, type, pos, size)
            {
                m_text = new TextBox();
                m_text.Width = 200;
                m_text.TextChanged += valueChanged;
                m_panel.Controls.AddRange(new Control[] { m_label, m_text });
            }

            public override string getSearchParams()
            {
                string srchParam = null;
                if (m_label.Checked)
                {
                    if (m_mode == SearchMode.like)
                        srchParam = string.Format("({0} like '%{1}%')", m_fieldName, m_value);
                    else
                        srchParam = string.Format("({0} = '{1}')", m_fieldName, m_value);
                }
                return srchParam;
            }
            public override void updateSearchParams(List<string> exprs, Dictionary<string, string> srchParams)
            {
                if (m_label.Checked)
                {
                    if (m_mode == SearchMode.like)
                    {
                        exprs.Add(string.Format("({0} like @{0})", m_fieldName));
                        srchParams.Add(string.Format("@{0}", m_fieldName), string.Format("%{0}%", m_value));
                    }
                    else
                    {
                        exprs.Add(string.Format("({0}=@{0})", m_fieldName));
                        srchParams.Add(string.Format("@{0}", m_fieldName), m_value);
                    }
                }
            }
            public override void LoadData()
            {
                if (m_colInfo!= null && m_colInfo.m_lookupData != null)
                {
                    m_combo = new ComboBox();
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

                    m_combo.Click += valueChanged;

                    m_text.Dispose();
                    m_text = null;
                }
            }
            public override void Dispose()
            {
                if (m_text != null) m_text.Dispose();
                if (m_combo != null) m_combo.Dispose();
                base.Dispose();
            }
        }
        [DataContract(Name = "SearchCtrlDate")]
        class lSearchCtrlDate : lSearchCtrl
        {
            private DateTimePicker m_startdate = new DateTimePicker();
            private DateTimePicker m_enddate = new DateTimePicker();
            private CheckBox m_to = new CheckBox();
            public lSearchCtrlDate(string fieldName, string alias, ctrlType type, Point pos, Size size)
                : base(fieldName, alias, type, pos, size)
            {
                m_to.Text = "to";
                m_to.AutoSize = true;

                m_startdate.Width = 80;
                m_startdate.Format = DateTimePickerFormat.Custom;
                m_startdate.CustomFormat = "yyyy-MM-dd";
                m_enddate.Width = 80;
                m_enddate.Format = DateTimePickerFormat.Custom;
                m_enddate.CustomFormat = "yyyy-MM-dd";
                FlowLayoutPanel datePanel = new FlowLayoutPanel();
                datePanel.BorderStyle = BorderStyle.FixedSingle;
                datePanel.Dock = DockStyle.Top;
                datePanel.AutoSize = true;
                datePanel.Controls.AddRange(new Control[] { m_startdate, m_to, m_enddate });

                m_panel.Controls.AddRange(new Control[] { m_label, datePanel });

                m_startdate.TextChanged += valueChanged;
                m_enddate.TextChanged += M_enddate_TextChanged;
            }

            private void M_enddate_TextChanged(object sender, EventArgs e)
            {
                m_to.Checked = true;
            }

            public override string getSearchParams()
            {
                string srchParams = null;
                if (m_label.Checked)
                {
                    if (m_to.Checked)
                        srchParams = string.Format("({0} between '{1}  00:00:00' and '{2} 00:00:00')", m_fieldName, m_startdate.Text, m_enddate.Text);
                    else
                        srchParams = string.Format("({0}='{1} 00:00:00')", m_fieldName, m_startdate.Text);
                }
                return srchParams;
            }
            public override void updateSearchParams(List<string> exprs, Dictionary<string,string> srchParams)
            {
                if (m_label.Checked)
                {
                    if (m_to.Checked)
                    {
                        exprs.Add("(date between @startDate and @endDate)");
                        srchParams.Add("@startDate", string.Format("{0} 00:00:00", m_startdate.Text));
                        srchParams.Add("@endDate", string.Format("{0} 00:00:00", m_enddate.Text));
                    }
                    else
                    {
                        exprs.Add("(date=@startDate)");
                        srchParams.Add("@startDate", string.Format("{0} 00:00:00", m_startdate.Text));
                    }
                }
            }
        }
        [DataContract(Name = "SearchCtrlNum")]
        class lSearchCtrlNum : lSearchCtrlText
        {
            public lSearchCtrlNum(string fieldName, string alias, ctrlType type, Point pos, Size size)
                : base(fieldName, alias, type, pos, size)
            {
                m_mode = SearchMode.match;
            }
        }
        [DataContract(Name = "SearchCtrlCurrency")]
        class lSearchCtrlCurrency : lSearchCtrlText
        {
            public lSearchCtrlCurrency(string fieldName, string alias, ctrlType type, Point pos, Size size)
                : base(fieldName, alias, type, pos, size)
            {
                m_mode = SearchMode.match;
            }
        }

        /// <summary>
        /// search panel
        /// + search ctrl
        /// + search btn
        /// + getWhereQry
        /// </summary>
        [DataContract(Name ="SearchPanel")]
        class lSearchPanel:IDisposable
        {
            public lDataPanel m_dataPanel;
            public lTableInfo m_tblInfo { get { return m_dataPanel.m_tblInfo; } }

            public TableLayoutPanel m_tbl;
            public Button m_searchBtn;

            [DataMember(Name ="searchCtrls")]
            public List<lSearchCtrl> m_searchCtrls;

            protected lSearchPanel() { }

            public static lSearchPanel crtSearchPanel(lDataPanel dataPanel, List<lSearchCtrl> searchCtrls)
            {
                lSearchPanel newPanel = new lSearchPanel();
                newPanel.init(dataPanel, searchCtrls);
                return newPanel;
            }

            protected void init(lDataPanel dataPanel, List<lSearchCtrl> searchCtrls)
            {
                m_dataPanel = dataPanel;
                m_searchCtrls = searchCtrls;
            }

            public virtual void initCtrls()
            {
                //crt search btn
                m_searchBtn = new Button();
                m_searchBtn.Text = "Search";
                m_searchBtn.Click += new System.EventHandler(searchButton_Click);

                //create search ctrls
                List<lSearchCtrl> searchCtrls = m_searchCtrls;
                m_searchCtrls = new List<lSearchCtrl>();
                foreach (lSearchCtrl ctrl in searchCtrls) { 
                    m_searchCtrls.Add(
                        crtSearchCtrl(
                            m_tblInfo,
                            ctrl.m_fieldName,
                            ctrl.m_pos,
                            ctrl.m_size,
                            ctrl.m_mode
                            )
                        );
                }

                //create table layout & add ctrls to
                //  +-------------------------+
                //  |search ctrl|             |
                //  +-------------------------+
                //  |search ctrl|             |
                //  +-------------------------+
                //  |       search btn        |
                //  +-------------------------+
                m_tbl = new TableLayoutPanel();
                m_tbl.AutoSize = true;
#if DEBUG_DRAWING
                m_tbl.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
#endif

                //add search ctrls to table layout
                int lastRow = 0;
                foreach (lSearchCtrl searchCtrl in m_searchCtrls)
                {
                    m_tbl.Controls.Add(searchCtrl.m_panel, searchCtrl.m_pos.X, searchCtrl.m_pos.Y);
                    m_tbl.SetColumnSpan(searchCtrl.m_panel, searchCtrl.m_size.Width);
                    m_tbl.SetRowSpan(searchCtrl.m_panel, searchCtrl.m_size.Height);
                    lastRow = Math.Max(lastRow, searchCtrl.m_pos.Y);
                }

                //  add search button to last row
                m_tbl.Controls.Add(m_searchBtn, 0, lastRow + 1);
                m_searchBtn.Anchor = AnchorStyles.None;
            }

            private void searchButton_Click(object sender, System.EventArgs e)
            {
#if use_cmd_params
                List<string> exprs = new List<string>();
                Dictionary<string, string> srchParams = new Dictionary<string, string>();
                foreach (lSearchCtrl searchCtrl in m_searchCtrls)
                {
                    searchCtrl.updateSearchParams(exprs, srchParams);
                }
                m_dataPanel.search(exprs, srchParams);
#else
                string where = null;
                List<string> exprs = new List<string> ();
                foreach (lSearchCtrl searchCtrl in m_searchCtrls)
                {
                    string expr = searchCtrl.getSearchParams();
                    if (expr != null)
                        exprs.Add(expr);
                }
                if (exprs.Count > 0)
                {
                    where = string.Join(" and ", exprs);
                }
                m_dataPanel.search(where);
#endif
            }

            public virtual void LoadData()
            {
                foreach(lSearchCtrl ctrl in m_searchCtrls)
                {
                    ctrl.LoadData();
                }
            }

            public lSearchCtrl crtSearchCtrl(lTableInfo tblInfo, string colName, Point pos, Size size)
            {
                return crtSearchCtrl(tblInfo, colName, pos, size, lSearchCtrl.SearchMode.match);
            }
            public lSearchCtrl crtSearchCtrl(lTableInfo tblInfo, string colName, Point pos, Size size, lSearchCtrl.SearchMode mode)
            {
                int iCol = tblInfo.getColIndex(colName);
                if (iCol != -1)
                {
                    return crtSearchCtrl(tblInfo, iCol, pos, size, mode);
                }
                return null;
            }
            public lSearchCtrl crtSearchCtrl(lTableInfo tblInfo, int iCol, Point pos, Size size)
            {
                return crtSearchCtrl(tblInfo, iCol, pos, size, lSearchCtrl.SearchMode.match);
            }
            public lSearchCtrl crtSearchCtrl(lTableInfo tblInfo, int iCol, Point pos, Size size, lSearchCtrl.SearchMode mode)
            {
                lTableInfo.lColInfo col = tblInfo.m_cols[iCol];
                switch (col.m_type)
                {
                    case lTableInfo.lColInfo.lColType.text:
                        lSearchCtrlText textCtrl = new lSearchCtrlText(col.m_field, col.m_alias, lSearchCtrl.ctrlType.text, pos, size);
                        textCtrl.m_mode = mode;
                        textCtrl.m_colInfo = col;
                        return textCtrl;
                    case lTableInfo.lColInfo.lColType.dateTime:
                        lSearchCtrlDate dateCtrl = new lSearchCtrlDate(col.m_field, col.m_alias, lSearchCtrl.ctrlType.dateTime, pos, size);
                        return dateCtrl;
                    case lTableInfo.lColInfo.lColType.num:
                        lSearchCtrlNum numCtrl = new lSearchCtrlNum(col.m_field, col.m_alias, lSearchCtrl.ctrlType.num, pos, size);
                        return numCtrl;
                    case lTableInfo.lColInfo.lColType.currency:
                        lSearchCtrlCurrency currencyCtrl = new lSearchCtrlCurrency(col.m_field, col.m_alias, lSearchCtrl.ctrlType.currency, pos, size);
                        return currencyCtrl;
                }
                return null;
            }

            public void Dispose()
            {
                m_tbl.Dispose();
                m_searchBtn.Dispose();
                foreach (var ctrl in m_searchCtrls)
                {
                    ctrl.Dispose();
                }
            }
        }

        [DataContract(Name = "ReceiptsSearchPanel")]
        class lReceiptsSearchPanel : lSearchPanel
        {
            public lReceiptsSearchPanel(lDataPanel dataPanel)
            {
                m_dataPanel = dataPanel;
                m_searchCtrls = new List<lSearchCtrl> {
                    crtSearchCtrl(m_tblInfo, "date", new Point(0, 0), new Size(1, 1)),
                    crtSearchCtrl(m_tblInfo, "receipt_number", new Point(0, 1), new Size(1, 1), lSearchCtrl.SearchMode.match),
                    crtSearchCtrl(m_tblInfo, "name", new Point(1, 0), new Size(1, 1), lSearchCtrl.SearchMode.like),
                    crtSearchCtrl(m_tblInfo, "content", new Point(1, 1), new Size(1, 1), lSearchCtrl.SearchMode.match),
                };
            }
        }

        [DataContract(Name = "InterPaymentSearchPanel")]
        class lInterPaymentSearchPanel : lSearchPanel
        {
            public lInterPaymentSearchPanel(lDataPanel dataPanel){
                m_dataPanel = dataPanel;
                m_searchCtrls = new List<lSearchCtrl> {
                    crtSearchCtrl(m_tblInfo, "date", new Point(0, 0), new Size(1, 1)),
                    crtSearchCtrl(m_tblInfo, "payment_number", new Point(0, 1), new Size(1, 1)),
                    crtSearchCtrl(m_tblInfo, "name", new Point(1, 0), new Size(1, 1), lSearchCtrl.SearchMode.match),
                    crtSearchCtrl(m_tblInfo, "group_name", new Point(1, 1), new Size(1, 1), lSearchCtrl.SearchMode.match),
                };
            }
        }

        [DataContract(Name = "ExternalPaymentSearchPanel")]
        class lExternalPaymentSearchPanel : lSearchPanel
        {
            public lExternalPaymentSearchPanel(lDataPanel dataPanel) {
                m_dataPanel = dataPanel;
                m_searchCtrls = new List<lSearchCtrl> {
                    crtSearchCtrl(m_tblInfo, "date", new Point(0, 0), new Size(1, 1)),
                    crtSearchCtrl(m_tblInfo, "payment_number", new Point(0, 1), new Size(1, 1), lSearchCtrl.SearchMode.match),
                    crtSearchCtrl(m_tblInfo, "name", new Point(1, 0), new Size(1, 1), lSearchCtrl.SearchMode.like),
                    crtSearchCtrl(m_tblInfo, "group_name", new Point(1, 1), new Size(1, 1), lSearchCtrl.SearchMode.match),
                    crtSearchCtrl(m_tblInfo, "content", new Point(1, 2), new Size(1, 1), lSearchCtrl.SearchMode.like),
                };
            }
        }

        [DataContract(Name = "SalarySearchPanel")]
        class lSalarySearchPanel : lSearchPanel
        {
            public lSalarySearchPanel(lDataPanel dataPanel)
            {
                m_dataPanel = dataPanel;
                m_searchCtrls = new List<lSearchCtrl> {
                    crtSearchCtrl(m_tblInfo, "month", new Point(0, 0), new Size(1, 1)),
                    crtSearchCtrl(m_tblInfo, "date", new Point(0, 1), new Size(1, 1)),
                    crtSearchCtrl(m_tblInfo, "payment_number", new Point(0, 2), new Size(1, 1), lSearchCtrl.SearchMode.match),
                    crtSearchCtrl(m_tblInfo, "name", new Point(1, 0), new Size(1, 1), lSearchCtrl.SearchMode.like),
                    crtSearchCtrl(m_tblInfo, "group_name", new Point(1, 1), new Size(1, 1), lSearchCtrl.SearchMode.match),
                    crtSearchCtrl(m_tblInfo, "content", new Point(1, 2), new Size(1, 1), lSearchCtrl.SearchMode.like),
                };
            }
        }

    }
}