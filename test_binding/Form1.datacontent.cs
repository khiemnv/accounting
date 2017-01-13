#define col_class

using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace test_binding
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// table info
        /// + fields
        /// + fileds type
        /// + alias
        /// </summary>
        class lTableInfo
        {
            //#define col_class
#if col_class
            public class lColInfo
            {
                public enum lColType
                {
                    text,
                    dateTime,
                    num
                };
                public string m_field;
                public string m_alias;
                public lColType m_type;
                public lColInfo(string field, string alias, lColType type)
                {
                    m_field = field;
                    m_alias = alias;
                    m_type = type;
                }
            };
            public lColInfo[] m_cols;
            public string m_tblName;
            public string m_tblAlias;
            public string m_crtQry;
#else
    public struct lColInfo
    {
        public enum lColType
        {
            text,
            dateTime,
            num
        };
        public string m_field;
        public string m_alias;
        public lColType m_type;
    };
    public virtual lColInfo[] getColsInfo() { return null; }
#endif
            public int getColIndex(string colName)
            {
                int i = 0;
                foreach (lColInfo col in m_cols)
                {
                    if (col.m_field == colName)
                    {
                        return i;
                    }
                    i++;
                }
                return -1;
            }
        }

        class lReceiptsTblInfo : lTableInfo
        {
#if col_class
            public lReceiptsTblInfo()
            {
                m_tblName = "receipts";
                m_tblAlias = "Bảng Thu";
                m_crtQry = "CREATE TABLE if not exists  receipts("
                + "ID INTEGER PRIMARY KEY AUTOINCREMENT,"
                + "date datetime,"
                + "receipt_number char(31),"
                + "name char(31),"
                + "content char(255),"
                + "price INTEGER,"
                + "note char(255)"
                + ")";
                m_cols = new lColInfo[] {
                   new lColInfo( "ID","ID", lColInfo.lColType.num),
                   new lColInfo( "date","Ngày Tháng", lColInfo.lColType.dateTime),
                   new lColInfo( "receipt_number","Mã Phiếu Thu", lColInfo.lColType.text),
                   new lColInfo( "name","Họ tên", lColInfo.lColType.text),
                   new lColInfo( "content","Nội dung", lColInfo.lColType.text),
                   new lColInfo( "price","Số tiền", lColInfo.lColType.num),
                   new lColInfo( "note","Ghi chú", lColInfo.lColType.text),
                };
            }
#else
    static lColInfo[] m_cols = new lColInfo[] {
               new lColInfo() {m_field = "ID", m_alias = "ID", m_type = lColInfo.lColType.num }
            };
    public override lColInfo[] getColsInfo()
    {
        return m_cols;
    }
#endif

        };
        class lInternalPaymentTblInfo : lTableInfo
        {
            public lInternalPaymentTblInfo()
            {
                m_tblName = "internal_payment";
                m_tblAlias = "Chi Nội Chúng";
                m_crtQry = "CREATE TABLE if not exists internal_payment("
                + "ID INTEGER PRIMARY KEY AUTOINCREMENT,"
                + "date datetime,"
                + "payment_number char(31),"
                + "name char(31),"
                + "content char(255),"
                + "group_name char(31),"
                + "advance_payment INTEGER,"
                + "reimbursement INTEGER,"
                + "actually_spent INTEGER,"
                + "note char(255)"
                + ")";
                m_cols = new lColInfo[] {
                   new lColInfo( "ID","ID", lColInfo.lColType.num),
                   new lColInfo( "date","Ngày Tháng", lColInfo.lColType.dateTime),
                   new lColInfo( "payment_number","Mã Phiếu Chi", lColInfo.lColType.text),
                   new lColInfo( "name","Họ Tên", lColInfo.lColType.text),
                   new lColInfo( "content","Nội dung", lColInfo.lColType.text),
                   new lColInfo( "group_name","Thuộc ban", lColInfo.lColType.text),
                   new lColInfo( "advance_payment","Tạm ứng", lColInfo.lColType.num),
                   new lColInfo( "reimbursement","Hoàn ứng", lColInfo.lColType.num),
                   new lColInfo( "actually_spent","Thực chi", lColInfo.lColType.num),
                   new lColInfo( "note","Ghi Chú", lColInfo.lColType.text),
                };
            }
        };
        class lExternalPaymentTblInfo : lTableInfo
        {
            public lExternalPaymentTblInfo()
            {
                m_tblName = "external_payment";
                m_tblAlias = "Chi Ngoại Chúng";
                m_crtQry = "CREATE TABLE if not exists external_payment("
                + "ID INTEGER PRIMARY KEY AUTOINCREMENT,"
                + "date datetime,"
                + "payment_number char(31),"
                + "name char(31),"
                + "content char(255),"
                + "group_name char(31),"
                + "spent INTEGER,"
                + "note char(255)"
                + ")";
                m_cols = new lColInfo[] {
                   new lColInfo( "ID","ID", lColInfo.lColType.num),
                   new lColInfo( "date","Ngày Tháng", lColInfo.lColType.dateTime),
                   new lColInfo( "payment_number","Mã Phiếu Chi", lColInfo.lColType.text),
                   new lColInfo( "name","Họ Tên", lColInfo.lColType.text),
                   new lColInfo( "content","Nội dung", lColInfo.lColType.text),
                   new lColInfo( "group_name","Thuộc ban", lColInfo.lColType.text),
                   new lColInfo( "spent","Số tiền", lColInfo.lColType.num),
                   new lColInfo( "note","Ghi Chú", lColInfo.lColType.text),
                };
            }
        };
        class lSalaryTblInfo : lTableInfo
        {
            public lSalaryTblInfo()
            {
                m_tblName = "salary";
                m_tblAlias = "Bảng Lương";
                m_crtQry = "CREATE TABLE if not exists salary("
                + "ID INTEGER PRIMARY KEY AUTOINCREMENT,"
                + "month INTEGER,"
                + "date datetime,"
                + "payment_number char(31),"
                + "name char(31),"
                + "group_name char(31),"
                + "content char(255),"
                + "salary INTEGER,"
                + "note char(255)"
                + ")";
                m_cols = new lColInfo[] {
                   new lColInfo( "ID","ID", lColInfo.lColType.num),
                   new lColInfo( "month","Tháng(1...12)", lColInfo.lColType.num),
                   new lColInfo( "date","Ngày Tháng", lColInfo.lColType.dateTime),
                   new lColInfo( "payment_number","Mã Phiếu Chi", lColInfo.lColType.text),
                   new lColInfo( "name","Họ Tên", lColInfo.lColType.text),
                   new lColInfo( "group_name","Thuộc ban", lColInfo.lColType.text),
                   new lColInfo( "content","Nội dung", lColInfo.lColType.text),
                   new lColInfo( "salary","Số tiền", lColInfo.lColType.num),
                   new lColInfo( "note","Ghi Chú", lColInfo.lColType.text),
                };
            }
        };


        /// <summary>
        /// data content
        /// + getdata()
        ///     return databinding
        /// + reload()
        /// + submit()
        /// </summary>
        class lDataContent
        {
#if use_sqlite
            private SQLiteConnection m_cnn;
#else
            private SqlConnection m_cnn;
#endif
            private string m_table { get { return m_tblInfo.m_tblName; } }
#if use_sqlite
            private SQLiteDataAdapter m_dataAdapter = new SQLiteDataAdapter();
#else
            private SqlDataAdapter m_dataAdapter = new SqlDataAdapter();
#endif
#if check_reload
            private bool m_canReload = false;
#endif

            public lTableInfo m_tblInfo;
            public BindingSource m_bindingSource = new BindingSource();

            public lDataContent(lTableInfo tblInfo) {
                m_tblInfo = tblInfo;
            }
#if use_sqlite
            public void init(SQLiteConnection cnn)
            {
                m_cnn = cnn;
                m_dataAdapter.SelectCommand = new SQLiteCommand(string.Format("select * from {0}", m_table), cnn);
            }
#else
            public void init(SqlConnection cnn)
            {
                m_cnn = cnn;
                m_dataAdapter.SelectCommand = new SqlCommand(string.Format("select * from {0}", m_table), cnn);
            }
#endif
            public void Search(List<string> exprs, List<lEntity> arr)
            {
                string sql = string.Format("select * from {0} ", m_table);

                if (exprs.Count > 0)
                {
                    sql += " where " + string.Join(" and ", exprs);
#if use_sqlite
                    SQLiteCommand selectCommand = new SQLiteCommand(sql, m_cnn);
#else
                    SqlCommand selectCommand = new SqlCommand(sql, m_cnn);
#endif
                    foreach (lEntity entity in arr)
                    {
                        selectCommand.Parameters.AddWithValue(entity.m_param, entity.m_value);
                    }
                    GetData(selectCommand);
                }
                else
                {
                    GetData(sql);
                }
            }
            public void GetData()
            {
                GetData(string.Format("select * from {0}", m_table));
            }
            public void GetData(string selectStr)
            {
#if use_sqlite
                SQLiteCommand selectCommand = new SQLiteCommand(selectStr, m_cnn);
#else
                SqlCommand selectCommand = new SqlCommand(selectStr, m_cnn);
#endif
                GetData(selectCommand);
            }
#if use_sqlite
            public void GetData(SQLiteCommand selectCommand)
#else
            public void GetData(SqlCommand selectCommand)
#endif
            {
                try
                {
                    m_dataAdapter.SelectCommand = selectCommand;

                    // Populate a new data table and bind it to the BindingSource.
                    DataTable table = new DataTable();
                    table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                    m_dataAdapter.Fill(table);
                    m_bindingSource.DataSource = table;
#if check_reload
                    m_canReload = true;
#endif
                }
#if use_sqlite
                catch (SQLiteException)
#else
                catch (SqlException)
#endif
                {
                    MessageBox.Show("To run this example, replace the value of the " +
                        "connectionString variable with a connection string that is " +
                        "valid for your system.");
                }
            }
            public void Reload()
            {
#if check_reload
                if (m_canReload) { 
                    GetData(m_dataAdapter.SelectCommand.CommandText);
                }
#else
                GetData(m_dataAdapter.SelectCommand);
#endif
            }
            public void Submit()
            {
#if use_sqlite
                using (SQLiteCommandBuilder builder = new SQLiteCommandBuilder(m_dataAdapter))
#else
                using (SqlCommandBuilder builder = new SqlCommandBuilder(m_dataAdapter))
#endif
                {
                    DataTable dt = (DataTable)m_bindingSource.DataSource;
                    if (dt != null) {
                        m_dataAdapter.UpdateCommand = builder.GetUpdateCommand();
                        m_dataAdapter.Update(dt);
                    }
                }
            }
        }

    }
}