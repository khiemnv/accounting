﻿//#define DEBUG_DRAWING

using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System;
using System.Data;
using System.Runtime.Serialization;

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
        class lSearchCtrl
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
                m_panel.AutoSize = true;
                //m_panel.Dock = DockStyle.Fill;
#if DEBUG_DRAWING
                m_panel.BorderStyle = BorderStyle.FixedSingle;
#endif
            }

            //public virtual bool isChecked() { return false; } 
            //public virtual List<lEntity> getExprParams() { return null; }
            //public virtual string getExpr() { return null; }
            //ref: search btn click
            public virtual void updateSearchParams(List<string> exprs, List<lEntity> arr)
            {
                //if (isChecked()) {
                //    string expr = getExpr();
                //    exprs.Add(getExpr());
                //    List<lEntity> exprParams = getExprParams();
                //    foreach (lEntity param in exprParams) arr.Add(param);
                //}
            }

            public virtual void LoadData()
            {
            }
        };

        [DataContract(Name = "SearchCtrlText")]
        class lSearchCtrlText : lSearchCtrl
        {
            protected TextBox m_text = new TextBox();
            public lSearchCtrlText(string fieldName, string alias, ctrlType type, Point pos, Size size)
                : base(fieldName, alias, type, pos, size)
            {
                m_text.Width = 200;
                m_panel.Controls.AddRange(new Control[] { m_label, m_text });
            }
            public override void updateSearchParams(List<string> exprs, List<lEntity> arr)
            {
                if (m_label.Checked)
                {
                    if (m_mode == SearchMode.like)
                    {
                        exprs.Add(string.Format("({0} like @{0})", m_fieldName));
                        arr.Add(new lEntity(string.Format("@{0}", m_fieldName), string.Format("%{0}%", m_text.Text)));
                    }
                    else
                    {
                        exprs.Add(string.Format("({0}=@{0})", m_fieldName));
                        arr.Add(new lEntity(string.Format("@{0}", m_fieldName), m_text.Text));
                    }
                }
            }
            public override void LoadData()
            {
                if (m_colInfo!= null && m_colInfo.m_lookupData != null)
                {
                    m_text.AutoCompleteMode = AutoCompleteMode.Suggest;
                    m_text.AutoCompleteSource = AutoCompleteSource.CustomSource;

                    AutoCompleteStringCollection col = new AutoCompleteStringCollection();
                    DataTable tbl = m_colInfo.m_lookupData.m_dataSource;
                    foreach (DataRow row in tbl.Rows)
                    {
                        col.Add(row[1].ToString());
                    }
                    m_text.AutoCompleteCustomSource = col;
                }
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
            }
            public override void updateSearchParams(List<string> exprs, List<lEntity> arr)
            {
                if (m_label.Checked)
                {
                    if (m_to.Checked)
                    {
                        exprs.Add("(date between @startDate and @endDate)");
                        arr.Add(new lEntity("@startDate", string.Format("{0} 00:00:00", m_startdate.Text)));
                        arr.Add(new lEntity("@endDate", string.Format("{0} 00:00:00", m_enddate.Text)));
                    }
                    else
                    {
                        exprs.Add("(date=@startDate)");
                        arr.Add(new lEntity("@startDate", string.Format("{0} 00:00:00", m_startdate.Text)));
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
        class lSearchPanel
        {
            public lDataPanel m_dataPanel;
            public lTableInfo m_tblInfo { get { return m_dataPanel.m_tblInfo; } }

            public TableLayoutPanel m_tbl;
            public Button m_searchBtn;

            [DataMember(Name ="searchCtrls")]
            public List<lSearchCtrl> m_searchCtrls;

            protected lSearchPanel() { }

            protected void init(lDataPanel dataPanel, List<lSearchCtrl> searchCtrls)
            {
                m_tbl = new TableLayoutPanel();
                m_searchBtn = new Button();

                m_dataPanel = dataPanel;
                m_searchCtrls = searchCtrls;

                m_searchBtn.Text = "Search";
                m_searchBtn.Click += new System.EventHandler(searchButton_Click);
            }

            public static lSearchPanel crtSearchPanel(lDataPanel dataPanel, List<lSearchCtrl> searchCtrls)
            {
                lSearchPanel newPanel = new lSearchPanel();
                newPanel.init(dataPanel, searchCtrls);
                return newPanel;
            }

            public lSearchCtrl crtSearchCtrl(lTableInfo tblInfo, string colName, Point pos, Size size)
            {
                return crtSearchCtrl(tblInfo, colName, pos, size, true);
            }
            public lSearchCtrl crtSearchCtrl(lTableInfo tblInfo, string colName, Point pos, Size size, bool bMath)
            {
                int iCol = tblInfo.getColIndex(colName);
                if (iCol != -1)
                {
                    return crtSearchCtrl(tblInfo, iCol, pos, size, bMath);
                }
                return null;
            }
            public lSearchCtrl crtSearchCtrl(lTableInfo tblInfo, int iCol, Point pos, Size size)
            {
                return crtSearchCtrl(tblInfo, iCol, pos, size, true);
            }
            public lSearchCtrl crtSearchCtrl(lTableInfo tblInfo, int iCol, Point pos, Size size, bool bMath)
            {
                lTableInfo.lColInfo col = tblInfo.m_cols[iCol];
                switch (col.m_type)
                {
                    case lTableInfo.lColInfo.lColType.text:
                        lSearchCtrlText textCtrl = new lSearchCtrlText(col.m_field, col.m_alias, lSearchCtrl.ctrlType.text, pos, size);
                        if (bMath) textCtrl.m_mode = lSearchCtrlText.SearchMode.match;
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

            public virtual void initCtrls()
            {
                List<lSearchCtrl> searchCtrls = m_searchCtrls;
                m_searchCtrls = new List<lSearchCtrl>();
                foreach (lSearchCtrl ctrl in searchCtrls) { 
                    m_searchCtrls.Add(
                        crtSearchCtrl(
                            m_tblInfo,
                            ctrl.m_fieldName,
                            ctrl.m_pos,
                            ctrl.m_size,
                            ctrl.m_mode == lSearchCtrl.SearchMode.match
                            )
                        );
                }

                m_tbl.AutoSize = true;
#if DEBUG_DRAWING
                m_tbl.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
#endif
                int lastRow = 0;
                foreach (lSearchCtrl searchCtrl in m_searchCtrls)
                {
                    m_tbl.Controls.Add(searchCtrl.m_panel, searchCtrl.m_pos.X, searchCtrl.m_pos.Y);
                    m_tbl.SetColumnSpan(searchCtrl.m_panel, searchCtrl.m_size.Width);
                    m_tbl.SetRowSpan(searchCtrl.m_panel, searchCtrl.m_size.Height);
                    lastRow = Math.Max(lastRow, searchCtrl.m_pos.Y);
                }

                m_tbl.Controls.Add(m_searchBtn, 1, lastRow + 1);
                m_searchBtn.Anchor = AnchorStyles.None;
            }

            private void searchButton_Click(object sender, System.EventArgs e)
            {
                List<string> exprs = new List<string>();
                List<lEntity> arr = new List<lEntity>();
                foreach (lSearchCtrl searchCtrl in m_searchCtrls)
                {
                    searchCtrl.updateSearchParams(exprs, arr);
                }
                m_dataPanel.search(exprs, arr);
            }

            public virtual void LoadData()
            {
                foreach(lSearchCtrl ctrl in m_searchCtrls)
                {
                    ctrl.LoadData();
                }
            }
        }

        [DataContract(Name = "InterPaymentSearchPanel")]
        class lInterPaymentSearchPanel : lSearchPanel
        {
            public lInterPaymentSearchPanel(lDataPanel dataPanel){
                m_dataPanel = dataPanel;
                m_searchCtrls = new List<lSearchCtrl> {
                    crtSearchCtrl(m_tblInfo, "date", new Point(1, 1), new Size(1, 1)),
                    crtSearchCtrl(m_tblInfo, "payment_number", new Point(1, 2), new Size(1, 1)),
                    crtSearchCtrl(m_tblInfo, "name", new Point(1, 3), new Size(1, 1), true),
                    crtSearchCtrl(m_tblInfo, "group_name", new Point(1, 4), new Size(1, 1), true),
                };
            }
        }

        [DataContract(Name = "ReceiptsSearchPanel")]
        class lReceiptsSearchPanel : lSearchPanel
        {
            public lReceiptsSearchPanel(lDataPanel dataPanel)
            {
                m_dataPanel = dataPanel;
                m_searchCtrls = new List<lSearchCtrl> {
                    crtSearchCtrl(m_tblInfo, "date", new Point(1, 1), new Size(1, 1)),
                    crtSearchCtrl(m_tblInfo, "receipt_number", new Point(1, 2), new Size(1, 1), true),
                    crtSearchCtrl(m_tblInfo, "name", new Point(1, 3), new Size(1, 1), false),
                    crtSearchCtrl(m_tblInfo, "content", new Point(1, 4), new Size(1, 1), false),
                };
            }
        }

        [DataContract(Name = "ExternalPaymentSearchPanel")]
        class lExternalPaymentSearchPanel : lSearchPanel
        {
            public lExternalPaymentSearchPanel(lDataPanel dataPanel) {
                m_dataPanel = dataPanel;
                m_searchCtrls = new List<lSearchCtrl> {
                    crtSearchCtrl(m_tblInfo, "date", new Point(1, 1), new Size(1, 1)),
                    crtSearchCtrl(m_tblInfo, "payment_number", new Point(1, 2), new Size(1, 1), true),
                    crtSearchCtrl(m_tblInfo, "name", new Point(1, 3), new Size(1, 1), true),
                    crtSearchCtrl(m_tblInfo, "group_name", new Point(1, 4), new Size(1, 1), true),
                    crtSearchCtrl(m_tblInfo, "content", new Point(1, 5), new Size(1, 1)),
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
                    crtSearchCtrl(m_tblInfo, "month", new Point(1, 0), new Size(1, 1)),
                    crtSearchCtrl(m_tblInfo, "date", new Point(1, 1), new Size(1, 1)),
                    crtSearchCtrl(m_tblInfo, "payment_number", new Point(1, 2), new Size(1, 1), true),
                    crtSearchCtrl(m_tblInfo, "name", new Point(1, 3), new Size(1, 1), true),
                    crtSearchCtrl(m_tblInfo, "group_name", new Point(1, 4), new Size(1, 1), true),
                    crtSearchCtrl(m_tblInfo, "content", new Point(1, 5), new Size(1, 1)),
                };
            }
        }

    }
}