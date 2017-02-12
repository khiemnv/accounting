//#define use_custom_cols
#define format_currency

using System.Windows.Forms;
using System;
using System.Drawing;
using System.Diagnostics;
using System.Data;

namespace test_binding
{
    public class myCustomCtrl : IDisposable
    {
        public DataGridView m_DGV;
        public Control m_ctrl { get { return getControl(); } }
        public bool m_bChanged = false;
        public int m_iRow;
        public int m_iCol;

        protected myCustomCtrl(DataGridView dgv)
        {
            m_DGV = dgv;
            //m_ctrl = ctrl;
        }

        public virtual void show(Rectangle rec)
        {
            m_ctrl.Location = rec.Location;
            m_ctrl.Size = rec.Size;
            m_ctrl.Visible = true;

        }
        public virtual void hide() { m_ctrl.Visible = false; }
        public virtual bool isChanged() { return m_bChanged; }
        public virtual string getValue() { return ""; }
        public virtual void setValue(string text) { }
        public virtual Control getControl() { return null; }
        public virtual void ctrl_ValueChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("ctrl_ValueChanged");
            m_bChanged = true;
            m_DGV.NotifyCurrentCellDirty(true);
        }

        internal void reLocation()
        {
            Rectangle rec = m_DGV.GetCellDisplayRectangle(m_iCol, m_iRow, true);
            m_ctrl.Size = rec.Size;
            m_ctrl.Location = rec.Location;
        }

        #region dispose
        // Dispose() calls Dispose(true)  
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        // NOTE: Leave out the finalizer altogether if this class doesn't   
        // own unmanaged resources itself, but leave the other methods  
        // exactly as they are.   
        ~myCustomCtrl()
        {
            // Finalizer calls Dispose(false)  
            Dispose(false);
        }
        // The bulk of the clean-up code is implemented in Dispose(bool)  
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            // free native resources if there are any.  
            m_ctrl.Dispose();
        }
        #endregion
    }
    public class myComboBox : myCustomCtrl
    {
        private ComboBox m_combo;

        //data table has single column
        public myComboBox(DataGridView dgv, lDataSync data)
            : base(dgv)
        {
            m_combo = new ComboBox();
            m_combo.DataSource = data.m_bindingSrc;
            DataTable tbl = (DataTable)data.m_bindingSrc.DataSource;
            m_combo.DisplayMember = tbl.Columns[1].ColumnName;
            m_combo.SelectedValueChanged += ctrl_ValueChanged;
        }
        public override Control getControl()
        {
            return m_combo;
        }
        public override string getValue()
        {
            Debug.WriteLine("getValue: " + m_combo.Text);
            return m_combo.Text;
        }

        public override void setValue(string text)
        {
            Debug.WriteLine("setValue: " + text);
            m_combo.Text = text;
        }
    }
    public class myDateTimePicker : myCustomCtrl
    {
        public DateTimePicker m_dtp;

        public myDateTimePicker(DataGridView dgv)
            : base(dgv)
        {
            m_dtp = new DateTimePicker();
            m_dtp.Format = DateTimePickerFormat.Custom;
            m_dtp.CustomFormat = "yyyy-MM-dd";
            m_dtp.ValueChanged += ctrl_ValueChanged;
        }
        public override Control getControl()
        {
            return m_dtp;
        }
        public override string getValue()
        {
            return m_dtp.Value.ToString("yyyy-MM-dd");
        }
        public override void setValue(string text)
        {
            DateTime dt;
            if (DateTime.TryParse(text, out dt))
            {
                m_dtp.Value = dt;
            }
        }
    }

    public class lCustomDGV : DataGridView
    {
        protected lTableInfo m_tblInfo;
        myCustomCtrl m_customCtrl;
        DataRow m_newRow;
        DataTable m_dataTable;

        public lCustomDGV(lTableInfo tblInfo)
        {
            m_tblInfo = tblInfo;
            DataTable dt = appConfig.s_contentProvider.CreateDataContent(m_tblInfo.m_tblName).m_dataTable;
            dt.TableNewRow += Dt_TableNewRow;
            m_dataTable = dt;
        }

        private void Dt_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            m_newRow = e.Row;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.Control)
                switch (e.KeyCode)
                {
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

        private void pasteClipboard()
        {
            Debug.WriteLine("{0}.pasteClipboard {1}", this, Clipboard.GetText());
            string inTxt = Clipboard.GetText();
            var lines = inTxt.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

            int baseRow = CurrentCell.RowIndex;
            int baseCol = CurrentCell.ColumnIndex;
            DataTable tbl = m_dataTable;
            int iRow = baseRow;
            foreach (var line in lines)
            {
                bool bNewRow = false;
                var fields = line.Split(new char[] { '\t', ';' });
                DataRow row;
                if (iRow < tbl.Rows.Count)
                {
                    row = tbl.Rows[iRow];
                }
                else
                {
                    row = (m_newRow != null) ? m_newRow : tbl.NewRow();
                    m_newRow = null;
                    bNewRow = true;
                }
                int iCol = baseCol;
                foreach (var field in fields)
                {
                    //m_dataGridView[iCol, iRow].Value = field;
                    switch (m_tblInfo.m_cols[iCol].m_type)
                    {
#if format_currency
                        case lTableInfo.lColInfo.lColType.currency:
                            row[iCol] = field.Replace(",", "");
                            break;
#endif
                        default:
                            row[iCol] = field;
                            break;
                    }
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
            Clipboard.SetDataObject(GetClipboardContent());
        }

        protected override void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
        {
            //base.OnDataError(displayErrorDialogIfNoHandler, e);
            //do nothing
        }
#if !use_custom_cols
        protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
        {
            base.OnCellEndEdit(e);
            Debug.WriteLine("OnCellEndEdit");
            hideCustomCtrl();
            //update selected value
            lDataSync data = m_tblInfo.m_cols[e.ColumnIndex].m_lookupData;
            if (data != null && CurrentCell.Value != null)
            {
                string key = CurrentCell.Value.ToString();
                string val = data.find(key);
                if (val != null)
                    CurrentCell.Value = val;
            }
        }
        protected override void OnCellDoubleClick(DataGridViewCellEventArgs e)
        {
            base.OnCellDoubleClick(e);
            Debug.WriteLine("OnCellClick");
            if (e.ColumnIndex != -1) showCustomCtrl(e.ColumnIndex, e.RowIndex);
        }

        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);
            if (m_customCtrl != null)
            {
                m_customCtrl.reLocation();
            }
        }

        lDataSync m_autoCompleteData;

        protected override void OnEditingControlShowing(DataGridViewEditingControlShowingEventArgs e)
        {
            base.OnEditingControlShowing(e);
            Debug.WriteLine("OnEditingControlShowing");
            do
            {
                if (m_customCtrl != null) break;

                m_autoCompleteData = m_tblInfo.m_cols[CurrentCell.ColumnIndex].m_lookupData;
                if (m_autoCompleteData == null) break;

                AutoCompleteStringCollection col = m_autoCompleteData.m_colls;
                DataGridViewTextBoxEditingControl edt = (DataGridViewTextBoxEditingControl)e.Control;
                edt.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                edt.AutoCompleteSource = AutoCompleteSource.CustomSource;
                edt.AutoCompleteCustomSource = col;

                edt.Validated += Edt_Validated;
            } while (false);
        }

        private void Edt_Validated(object sender, EventArgs e)
        {
            TextBox edt = (TextBox)sender;
            Debug.WriteLine("Edt_Validated:" + edt.Text);
            string selectedValue = edt.Text;
            if (selectedValue != "")
            {
                m_autoCompleteData.Update(selectedValue);
            }

            m_autoCompleteData = null;
            edt.Validated -= Edt_Validated;
        }

        public virtual void showCustomCtrl(int col, int row)
        {
            Debug.WriteLine("showDtp");
            if (m_tblInfo.m_cols[col].m_type == lTableInfo.lColInfo.lColType.dateTime)
            {
                m_customCtrl = new myDateTimePicker(this);
            }
            else if (m_tblInfo.m_cols[col].m_lookupData != null)
            {
                //m_customCtrl = new myComboBox(this, m_tblInfo.m_cols[col].m_lookupData);
            }
            if (m_customCtrl != null)
            {
                m_customCtrl.m_iRow = row;
                m_customCtrl.m_iCol = col;
                this.Controls.Add(m_customCtrl.getControl());
                if (CurrentCell.Value != null)
                {
                    m_customCtrl.setValue(this.CurrentCell.Value.ToString());
                }
                Rectangle rec = this.GetCellDisplayRectangle(col, row, true);
                m_customCtrl.show(rec);

                //ActiveControl = m_dtp;
                this.BeginEdit(true);
            }
        }
        public virtual void hideCustomCtrl()
        {
            if (m_customCtrl != null)
            {
                Debug.WriteLine("hideDtp");
                m_customCtrl.hide();

                if (m_customCtrl.isChanged())
                {
                    this.CurrentCell.Value = m_customCtrl.getValue();
                }

                this.Controls.Remove(m_customCtrl.getControl());
                m_customCtrl.Dispose();
                m_customCtrl = null;
            }
        }
#endif
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            m_dataTable.TableNewRow -= Dt_TableNewRow;
            if (m_customCtrl != null) m_customCtrl.Dispose();
        }
    }
    public class lInterPaymentDGV : lCustomDGV
    {
        public lInterPaymentDGV(lTableInfo tblInfo) : base(tblInfo)
        {
        }
        protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
        {
            base.OnCellEndEdit(e);
            if (m_tblInfo.m_cols[e.ColumnIndex].m_field == "reimbursement")
            {
                Int64 advance = (Int64)Rows[e.RowIndex].Cells["advance_payment"].Value;
                Int64 remain = (Int64)Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                this.Rows[e.RowIndex].Cells["actually_spent"].Value = advance - remain;
            }
        }
    }
    public class lSalaryDGV : lCustomDGV
    {
        public lSalaryDGV(lTableInfo tblInfo) : base(tblInfo)
        {
        }
        protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
        {
            base.OnCellEndEdit(e);
            if (m_tblInfo.m_cols[e.ColumnIndex].m_field == "date")
            {
                var val = Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (val != DBNull.Value) { 
                DateTime cur = (DateTime)val;
                if (Rows[e.RowIndex].Cells["month"].Value == DBNull.Value)
                    this.Rows[e.RowIndex].Cells["month"].Value = cur.Month;
                }
            }
        }
    }
}