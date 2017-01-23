//#define DEBUG_DRAWING
#define use_custom_dgv

using System.Windows.Forms;
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace test_binding
{
    public partial class Form1 : Form
    {

        /// <summary>
        /// Data Panel
        /// + sum
        /// + data grid
        /// + update()
        ///     cal sum
        ///     update data grid - auto
        /// </summary>
        [DataContract(Name = "DataPanel")]
        class lDataPanel
        {
            //public TableLayoutPanel m_tbl = new TableLayoutPanel();
            public FlowLayoutPanel m_reloadPanel = new FlowLayoutPanel();
            public FlowLayoutPanel m_sumPanel = new FlowLayoutPanel();

            public Button m_reloadBtn = new Button();
            public Button m_submitBtn = new Button();
            public Label m_sumLabel = new Label();
            public TextBox m_sumTxt = new TextBox();

            public DataGridView m_dataGridView;

            lDataContent m_dataContent;

            [DataMember(Name = "tblInfo")]
            public lTableInfo m_tblInfo;
            [DataMember(Name = "countOn")]
            public string m_countOn = "";

            public lDataPanel() { }
            public lDataPanel(lTableInfo tblInfo)
            {
                m_tblInfo = tblInfo;

                m_reloadBtn.Text = "Reload";
                m_submitBtn.Text = "Save";

                m_reloadBtn.Click += new System.EventHandler(reloadButton_Click);
                m_submitBtn.Click += new System.EventHandler(submitButton_Click);

                m_sumLabel.Text = "Sum";

#if use_custom_dgv
                m_dataGridView = new myDataGridView(m_tblInfo);
#else
                m_dataGridView = new DataGridView();
                m_dataGridView.CellClick += M_dataGridView_CellClick;
                m_dataGridView.CellEndEdit += M_dataGridView_CellEndEdit;
                m_dataGridView.Scroll += M_dataGridView_Scroll;
#endif
            }
#if !use_custom_dgv
            private myCustomCtrl m_customCtrl;
            private void M_dataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
            {
                Debug.WriteLine("OnCellEndEdit");
                hideCustomCtrl();
            }
            private void M_dataGridView_Scroll(object sender, ScrollEventArgs e)
            {
                if (m_customCtrl != null)
                {
                    m_customCtrl.reLocation();
                }
            }
            private void M_dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
            {
                Debug.WriteLine("OnCellClick");
                showCustomCtrl(e.ColumnIndex, e.RowIndex);
            }
            private void showCustomCtrl(int col, int row)
            {
                Debug.WriteLine("showDtp");
                if (m_tblInfo.m_cols[col].m_type == lTableInfo.lColInfo.lColType.dateTime)
                {
                    m_customCtrl = new myDateTimePicker(m_dataGridView);
                }
                else if (m_tblInfo.m_cols[col].m_lookupData != null)
                {
                    m_customCtrl = new myComboBox(m_dataGridView, m_tblInfo.m_cols[col].m_lookupData.m_dataSource);
                }
                if (m_customCtrl != null)
                {
                    m_customCtrl.m_iRow = row;
                    m_customCtrl.m_iCol = col;
                    m_dataGridView.Controls.Add(m_customCtrl.getControl());
                    m_customCtrl.setValue(m_dataGridView.CurrentCell.Value.ToString());
                    Rectangle rec = m_dataGridView.GetCellDisplayRectangle(col, row, true);
                    m_customCtrl.show(rec);

                    //ActiveControl = m_dtp;
                    m_dataGridView.BeginEdit(true);
                }
            }
            private void hideCustomCtrl()
            {
                if (m_customCtrl != null)
                {
                    Debug.WriteLine("hideDtp");
                    m_customCtrl.hide();

                    if (m_customCtrl.isChanged())
                    {
                        m_dataGridView.CurrentCell.Value = m_customCtrl.getValue();
                    }

                    m_dataGridView.Controls.Remove(m_customCtrl.getControl());
                    m_customCtrl = null;
                }
            }
#endif
            public void search(List<string> exprs, List<lEntity> arr)
            {
                //m_dataContent.GetData(qry);
                m_dataContent.Search(exprs, arr);
                update();
            }
            private void reloadButton_Click(object sender, System.EventArgs e)
            {
                m_dataContent.Reload();
                update();
            }
            private void submitButton_Click(object sender, System.EventArgs e)
            {
                m_dataContent.Submit();
            }

            public virtual void initCtrls()
            {
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

                m_reloadPanel.Controls.AddRange(new Control[] { m_reloadBtn, m_submitBtn });
                m_sumPanel.Controls.AddRange(new Control[] { m_sumLabel, m_sumTxt});

                m_sumLabel.Anchor = AnchorStyles.Right;
                m_sumLabel.TextAlign = ContentAlignment.MiddleRight;
                m_sumLabel.AutoSize = true;
                
                m_sumTxt.Width = 100;

                m_dataGridView.Anchor = AnchorStyles.Top & AnchorStyles.Left;
                //m_dataGridView.AutoSize = true;
                m_dataGridView.Dock = DockStyle.Fill;
            }
#if false
            public void createCols(List<string> fields, List<string> headers)
            {
                m_dataGridView.AutoGenerateColumns = false;
                for (int i = 0; i < fields.Count; i++) { 
                    m_dataGridView.Columns.Add(fields[i], headers[i]);
                }
                m_dataGridView.Columns[0].Visible = false;
            }
#endif
            public virtual Int64 getSum()
            {
                Int64 sum = 0;
                for (int i = 0; i < (m_dataGridView.RowCount - 1); i++)
                {
                    sum += Int64.Parse(m_dataGridView[4, i].Value.ToString());
                }
                return sum;
            }
            private void update()
            {
                // Resize the DataGridView columns to fit the newly loaded content.
                //dataGridView1.AutoResizeColumns(
                //    DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader);
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
                    m_dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    m_dataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                    m_dataGridView.Columns[i].FillWeight = 1;
                }

                Int64 sum = getSum();
                m_sumTxt.Text = sum.ToString("#0,0");
            }
            public virtual void LoadData()
            {
                m_tblInfo.LoadData();
                m_dataContent = s_contentProvider.CreateDataContent(m_tblInfo.m_tblName);
                m_dataGridView.DataSource = m_dataContent.m_bindingSource;
            }
        }

        [DataContract(Name = "InterPaymentDataPanel")]
        class lInterPaymentDataPanel : lDataPanel
        {
            const int advance_payment_col = 6;
            const int reimbursement_col = 7;
            const int actually_spent_col = 8;
            private Int64 advance_payment_sum;
            private Int64 reimbursement_sum;
            private Int64 actually_spent_sum;

            public lInterPaymentDataPanel()
                    : base(new lInternalPaymentTblInfo())
            {
            }

            public override Int64 getSum()
            {
                advance_payment_sum = 0;
                reimbursement_sum = 0;
                actually_spent_sum = 0;
                for (int i = 0; i < (m_dataGridView.RowCount - 1); i++)
                {
                    advance_payment_sum += Int64.Parse(m_dataGridView[advance_payment_col, i].Value.ToString());
                    reimbursement_sum += Int64.Parse(m_dataGridView[reimbursement_col, i].Value.ToString());
                    actually_spent_sum += Int64.Parse(m_dataGridView[actually_spent_col, i].Value.ToString());
                }
                return actually_spent_sum;
            }
        }

        [DataContract(Name = "ReceiptsDataPanel")]
        class lReceiptsDataPanel : lDataPanel
        {
            const int price_col = 5;
            private Int64 price_sum;

            public lReceiptsDataPanel()
                    : base(new lReceiptsTblInfo())
            {
            }

            public override Int64 getSum()
            {
                price_sum = 0;
                for (int i = 0; i < (m_dataGridView.RowCount - 1); i++)
                {
                    price_sum += Int64.Parse(m_dataGridView[price_col, i].Value.ToString());
                }
                return price_sum;
            }
        }

        [DataContract(Name = "ExterPaymentDataPanel")]
        class lExternalPaymentDataPanel : lDataPanel
        {
            const int spent_col = 6;
            private Int64 spent_sum;

            public lExternalPaymentDataPanel()
                    : base(new lExternalPaymentTblInfo())
            {
            }

            public override Int64 getSum()
            {
                spent_sum = 0;
                for (int i = 0; i < (m_dataGridView.RowCount - 1); i++)
                {
                    spent_sum += Int64.Parse(m_dataGridView[spent_col, i].Value.ToString());
                }
                return spent_sum;
            }
        }

        [DataContract(Name = "SalaryDataPanel")]
        class lSalaryDataPanel : lDataPanel
        {
            const int salary_col = 7;
            private Int64 spent_sum;

            public lSalaryDataPanel()
                    : base(new lSalaryTblInfo())
            {
#if false
                //update data grid view hdr
                List<string> headers = new List<string>(
                    new string[] { "ID", "month",
                        "date", "payment_number",
                        "name", "group_name",
                        "content", "salary", "note" }
                    );
                List<string> fields = new List<string>(
                    new string[] {"ID","Thang",
                        "Ngay thang","Ma Phieu",
                        "Ho ten", "Ban",
                        "Noi dung", "So tien", "Ghi Chu"}
                    );

                createCols(fields, headers);
#endif
            }

            public override Int64 getSum()
            {
                spent_sum = 0;
                for (int i = 0; i < (m_dataGridView.RowCount - 1); i++)
                {
                    spent_sum += Int64.Parse(m_dataGridView[salary_col, i].Value.ToString());
                }
                return spent_sum;
            }
        }
    }
}