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
        class myHistory
        {
            int m_cursor = 0;
            int m_count = 0;
            int m_rowindex = 0;
            int m_colindex = 0;
            static int m_data_size = 8;
            public enum myCellEvent
            {
                enter,
                click,
                mouseClick,
                beginEdit,
                endEdit,
                doubleClick,
                editShowing,
                cellChanged,
            }
            myCellEvent[] m_data = new myCellEvent[m_data_size];
            public myHistory()
            {

            }
            public void addEvent(myCellEvent e)
            {
                addEvent(e, m_rowindex, m_colindex);
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
                    myCellEvent.beginEdit,
            };
            public bool checkPartern()
            {
                Debug.WriteLine("checkPartern");
                do
                {
                    int n = m_ptn.Length;
                    if (m_count < n) break;
                    int i = 0;
                    for (; i < n; i++)
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
    }
}