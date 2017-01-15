using System.Windows.Forms;
using System.Collections.Generic;
using System;
using System.Drawing;
using System.Diagnostics;

namespace test_binding
{
    public partial class Form1 : Form
    {
        class myDateTimePicker : DateTimePicker
        {
            public DataGridView m_DGV;
            public bool valueChanged = false;

            public myDateTimePicker(DataGridView dgv)
            {
                m_DGV = dgv;
                Format = DateTimePickerFormat.Short;
            }
            protected override void OnValueChanged(EventArgs eventargs)
            {
                valueChanged = true;
                base.OnValueChanged(eventargs);
                m_DGV.NotifyCurrentCellDirty(true);
            }
        }

        class myDataGridView:DataGridView
        {
            lTableInfo m_tblInfo;
            myDateTimePicker m_dtp;

            public myDataGridView(lTableInfo tblInfo)
            {
                m_tblInfo = tblInfo;
            }

            protected override void OnCellEndEdit(DataGridViewCellEventArgs e)
            {
                Debug.WriteLine("OnCellEndEdit");
                hideDtp();
                base.OnCellEndEdit(e);
            }
            protected override void OnCellClick(DataGridViewCellEventArgs e)
            {
                Debug.WriteLine("OnCellClick");
                switch(m_tblInfo.m_cols[e.ColumnIndex].m_type)
                {
                    case lTableInfo.lColInfo.lColType.dateTime:
                        showDtp(e.ColumnIndex, e.RowIndex);
                        break;
                }
                base.OnCellClick(e);
            }

            private void showDtp(int col, int row)
            {
                Debug.WriteLine("showDtp");
                m_dtp = new myDateTimePicker(this);
                this.Controls.Add(m_dtp);

                m_dtp.Format = DateTimePickerFormat.Short;
                if (CurrentCell.Value.GetType() == typeof(DateTime))
                {
                    m_dtp.Value = (DateTime)CurrentCell.Value;
                }

                Rectangle rec = this.GetCellDisplayRectangle(col, row, true);
                m_dtp.Size = rec.Size;
                m_dtp.Location = rec.Location;

                m_dtp.Show();
                //ActiveControl = m_dtp;
                this.BeginEdit(true);
            }
            private void hideDtp()
            {
                if (m_dtp != null)
                {
                    Debug.WriteLine("hideDtp");
                    m_dtp.Hide();

                    if (m_dtp.valueChanged)
                    {
                        this.CurrentCell.Value = m_dtp.Value.ToString("yyyy-MM-dd");
                    }

                    this.Controls.Remove(m_dtp);
                    m_dtp = null;
                }
            }
        }
    }
}