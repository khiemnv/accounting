//#define DEBUG_DRAWING
#define use_custom_dgv

using System.Windows.Forms;
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Data;

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
            public FlowLayoutPanel m_reloadPanel;
            public FlowLayoutPanel m_sumPanel;

            public Button m_reloadBtn;
            public Button m_submitBtn;
            public Label m_status;
            public Label m_sumLabel;
            public TextBox m_sumTxt;

            public DataGridView m_dataGridView;

            public lDataContent m_dataContent;

            [DataMember(Name = "tblInfo")]
            public lTableInfo m_tblInfo;
            [DataMember(Name = "countOn")]
            public string m_countOn = "";

            protected lDataPanel() { }
            /// <summary>
            /// create new instance
            /// </summary>
            /// <param name="dataPanel"></param>
            /// <returns></returns>
            public static lDataPanel crtDataPanel(lDataPanel dataPanel)
            {
                lDataPanel newDataPanel = new lDataPanel();
                newDataPanel.init(dataPanel);
                return newDataPanel;
            }

            private void init(lDataPanel dataPanel)
            {
                m_countOn = dataPanel.m_countOn;
                m_tblInfo = dataPanel.m_tblInfo;
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
#else
                m_dataGridView = new DataGridView();
                m_dataGridView.CellClick += M_dataGridView_CellClick;
                m_dataGridView.CellEndEdit += M_dataGridView_CellEndEdit;
                m_dataGridView.Scroll += M_dataGridView_Scroll;
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
            public void search(string where)
            {
                //m_dataContent.GetData(qry);
                m_dataContent.Search(where);
                update();
            }
#if use_cmd_params
            public void search(List<string> exprs, Dictionary<string,string> srchParams)
            {
                //m_dataContent.GetData(qry);
                m_dataContent.Search(exprs, srchParams);
                update();
            }
#endif
            private void reloadButton_Click(object sender, System.EventArgs e)
            {
                m_dataContent.Reload();
                update();
                m_status.Text = "Reloading completed!";
            }
            private void submitButton_Click(object sender, System.EventArgs e)
            {
                m_dataContent.Submit();
                m_status.Text = "Saving completed!";
            }

            public virtual Int64 getSum()
            {
                Int64 sum = 0;
                int iCol = m_tblInfo.getColIndex(m_countOn);
                for (int i = 0; i < (m_dataGridView.RowCount - 1); i++)
                {
                    sum += Int64.Parse(m_dataGridView[iCol, i].Value.ToString());
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
                m_dataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.Silver;
                m_dataGridView.EnableHeadersVisualStyles = false;
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
                if (m_dataGridView.ColumnCount > 0) {update();}
            }
        }

        [DataContract(Name = "InterPaymentDataPanel")]
        class lInterPaymentDataPanel : lDataPanel
        {
            public lInterPaymentDataPanel()                
            {
                m_tblInfo = new lInternalPaymentTblInfo();
                m_countOn = "actually_spent";
            }
        }

        [DataContract(Name = "ReceiptsDataPanel")]
        class lReceiptsDataPanel : lDataPanel
        {
            public lReceiptsDataPanel()
            {
                m_tblInfo = new lReceiptsTblInfo();
                m_countOn = "amount";
            }
        }

        [DataContract(Name = "ExterPaymentDataPanel")]
        class lExternalPaymentDataPanel : lDataPanel
        {
            public lExternalPaymentDataPanel()                
            {
                m_tblInfo = new lExternalPaymentTblInfo();
                m_countOn = "spent";
            }
        }

        [DataContract(Name = "SalaryDataPanel")]
        class lSalaryDataPanel : lDataPanel
        {
            public lSalaryDataPanel()
            {
                m_tblInfo = new lSalaryTblInfo();
                m_countOn = "salary";
            }
        }

        class lGroupNameDataPanel : lDataPanel
        {
            public lGroupNameDataPanel()
            {
                m_tblInfo = new lGroupNameTblInfo();
            }
            public override Int64 getSum()
            {
                BindingSource bs = (BindingSource)m_dataGridView.DataSource;
                DataTable tbl = (DataTable)bs.DataSource;
                return tbl.Rows.Count;
            }
        }
        class lReceiptsContentDataPanel : lGroupNameDataPanel
        {
            public lReceiptsContentDataPanel()
            {
                m_tblInfo = new lReceiptsContentTblInfo();
            }
        }
    }
}