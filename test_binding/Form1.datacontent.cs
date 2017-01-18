#define col_class

using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using System;
using System.Data.SQLite;

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
                public string m_lookupTbl;
                public lDataSync m_lookupData;
                public lColType m_type;
                public lColInfo(string field, string alias, lColType type, string lookupTbl)
                {
                    m_lookupTbl = lookupTbl;
                    m_field = field;
                    m_alias = alias;
                    m_type = type;
                }
                public lColInfo(string field, string alias, lColType type)
                {
                    m_lookupTbl = null;
                    m_field = field;
                    m_alias = alias;
                    m_type = type;
                }
            };
            public lColInfo[] m_cols;
            public string m_tblName;
            public string m_tblAlias;
            public string m_crtQry;
            public virtual void LoadData()
            {
                foreach (lColInfo colInfo in m_cols)
                {
                    if (colInfo.m_lookupTbl != null)
                    {
                        colInfo.m_lookupData = s_contentProvider.CreateDataSync(colInfo.m_lookupTbl);
                        colInfo.m_lookupData.LoadData();
                    }
                }
            }
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
                + "content text,"
                + "price INTEGER,"
                + "note text"
                + ")";
                m_cols = new lColInfo[] {
                   new lColInfo( "ID","ID", lColInfo.lColType.num),
                   new lColInfo( "date","Ngày Tháng", lColInfo.lColType.dateTime),
                   new lColInfo( "receipt_number","Mã Phiếu Thu", lColInfo.lColType.text),
                   new lColInfo( "name","Họ tên", lColInfo.lColType.text),
                   new lColInfo( "content","Nội dung", lColInfo.lColType.text, "receipts_content"),
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
                + "content text,"
                + "group_name char(31),"
                + "advance_payment INTEGER,"
                + "reimbursement INTEGER,"
                + "actually_spent INTEGER,"
                + "note text"
                + ")";
                m_cols = new lColInfo[] {
                   new lColInfo( "ID","ID", lColInfo.lColType.num),
                   new lColInfo( "date","Ngày Tháng", lColInfo.lColType.dateTime),
                   new lColInfo( "payment_number","Mã Phiếu Chi", lColInfo.lColType.text),
                   new lColInfo( "name","Họ Tên", lColInfo.lColType.text),
                   new lColInfo( "content","Nội dung", lColInfo.lColType.text),
                   new lColInfo( "group_name","Thuộc ban", lColInfo.lColType.text, "group_name"),
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
                + "content text,"
                + "group_name char(31),"
                + "spent INTEGER,"
                + "note text"
                + ")";
                m_cols = new lColInfo[] {
                   new lColInfo( "ID","ID", lColInfo.lColType.num),
                   new lColInfo( "date","Ngày Tháng", lColInfo.lColType.dateTime),
                   new lColInfo( "payment_number","Mã Phiếu Chi", lColInfo.lColType.text),
                   new lColInfo( "name","Họ Tên", lColInfo.lColType.text),
                   new lColInfo( "content","Nội dung", lColInfo.lColType.text),
                   new lColInfo( "group_name","Thuộc ban", lColInfo.lColType.text, "group_name"),
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
                + "content text,"
                + "salary INTEGER,"
                + "note text"
                + ")";
                m_cols = new lColInfo[] {
                   new lColInfo( "ID","ID", lColInfo.lColType.num),
                   new lColInfo( "month","Tháng(1...12)", lColInfo.lColType.num),
                   new lColInfo( "date","Ngày Tháng", lColInfo.lColType.dateTime),
                   new lColInfo( "payment_number","Mã Phiếu Chi", lColInfo.lColType.text),
                   new lColInfo( "name","Họ Tên", lColInfo.lColType.text),
                   new lColInfo( "group_name","Thuộc ban", lColInfo.lColType.text, "group_name"),
                   new lColInfo( "content","Nội dung", lColInfo.lColType.text),
                   new lColInfo( "salary","Số tiền", lColInfo.lColType.num),
                   new lColInfo( "note","Ghi Chú", lColInfo.lColType.text),
                };
            }
        };

        interface lContentProvider
        {
            lDataContent CreateDataContent(string tblName);
            lDataSync CreateDataSync(string tblName);
            DataTable GetData(string qry);
        }
        class lSqlContentProvider : lContentProvider
        {
            static lSqlContentProvider m_instance;
            public static lContentProvider getInstance()
            {
                if (m_instance == null)
                {
                    m_instance = new lSqlContentProvider();
                }
                return m_instance;
            }

            lSqlContentProvider()
            {
                string[] lines = System.IO.File.ReadAllLines(@"..\..\config.txt");
                //string cnnStr = "Data Source=DESKTOP-GOEF1DS\\SQLEXPRESS;Initial Catalog=accounting;Integrated Security=true";
                string cnnStr = lines[0];
                m_cnn = new SqlConnection(cnnStr);
                m_cnn.Open();
            }

            private SqlConnection m_cnn;

            public lDataContent CreateDataContent(string tblName)
            {
                lSqlDataContent data = new lSqlDataContent(tblName, m_cnn);
                return data;
            }

            public lDataSync CreateDataSync(string tblName)
            {
                lDataContent dataContent = CreateDataContent(tblName);
                lDataSync dataSync = new lDataSync(dataContent);
                return dataSync;
            }

            public DataTable GetData(string qry)
            {
                SqlDataAdapter dataAdapter = new SqlDataAdapter();
                dataAdapter.SelectCommand = new SqlCommand(qry, m_cnn);
                // Populate a new data table and bind it to the BindingSource.
                DataTable table = new DataTable();
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                dataAdapter.Fill(table);
                return table;
            }
        }
        class lSQLiteContentProvider : lContentProvider
        {
            static lSQLiteContentProvider m_instance;
            public static lContentProvider getInstance()
            {
                if (m_instance == null) { m_instance = new lSQLiteContentProvider(); }
                return m_instance;
            }

            lSQLiteContentProvider()
            {
                string dbPath = "test.db";
                if (!System.IO.File.Exists(dbPath))
                {
                    SQLiteConnection.CreateFile(dbPath);
                }
                m_cnn = new SQLiteConnection(string.Format("Data Source={0};Version=3;", dbPath));
                m_cnn.Open();
            }

            private SQLiteConnection m_cnn;

            public lDataContent CreateDataContent(string tblName)
            {
                lSQLiteDataContent dataContent = new lSQLiteDataContent(tblName, m_cnn);
                return dataContent;
            }

            public lDataSync CreateDataSync(string tblName)
            {
                lSQLiteDataContent dataContent = new lSQLiteDataContent(tblName, m_cnn);
                lDataSync dataSync = new lDataSync(dataContent);
                return dataSync;
            }

            public DataTable GetData(string qry)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// data content
        /// + getdata()
        ///     return databinding
        /// + reload()
        /// + submit()
        /// </summary>
        class lDataContent
        {
            public BindingSource m_bindingSource;
            protected string m_table;
            protected lDataContent()
            {
                m_bindingSource = new BindingSource();
            }
            public virtual void Search(List<string> exprs, List<lEntity> arr) { }
            public virtual void Reload() { }
            public virtual void Submit() { }
        }
        class lSQLiteDataContent : lDataContent
        {
            private SQLiteConnection m_cnn;
            private SQLiteDataAdapter m_dataAdapter;

            public lSQLiteDataContent(string tblName, SQLiteConnection cnn)
                : base()
            {
                m_table = tblName;
                m_cnn = cnn;
                m_dataAdapter = new SQLiteDataAdapter();
                m_dataAdapter.SelectCommand = new SQLiteCommand(string.Format("select * form {0}", tblName), cnn);
            }

            public override void Search(List<string> exprs, List<lEntity> arr)
            {
                string sql = string.Format("select * from {0} ", m_table);

                if (exprs.Count > 0)
                {
                    sql += " where " + string.Join(" and ", exprs);
                    SQLiteCommand selectCommand = new SQLiteCommand(sql, m_cnn);
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
            public override void Reload()
            {
                GetData(m_dataAdapter.SelectCommand);
            }
            public override void Submit()
            {
                using (SQLiteCommandBuilder builder = new SQLiteCommandBuilder(m_dataAdapter))
                {
                    DataTable dt = (DataTable)m_bindingSource.DataSource;
                    if (dt != null)
                    {
                        m_dataAdapter.UpdateCommand = builder.GetUpdateCommand();
                        m_dataAdapter.Update(dt);
                    }
                }
            }
            private void GetData(string selectStr)
            {
                SQLiteCommand selectCommand = new SQLiteCommand(selectStr, m_cnn);
                GetData(selectCommand);
            }
            private void GetData(SQLiteCommand selectCommand)
            {
                m_dataAdapter.SelectCommand = selectCommand;
                // Populate a new data table and bind it to the BindingSource.
                DataTable table = new DataTable();
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                m_dataAdapter.Fill(table);
                m_bindingSource.DataSource = table;
            }
        }
        class lSqlDataContent : lDataContent
        {
            private SqlConnection m_cnn;
            private SqlDataAdapter m_dataAdapter;

            public lSqlDataContent(string tblName, SqlConnection cnn)
                : base()
            {
                m_table = tblName;
                m_cnn = cnn;
                m_dataAdapter = new SqlDataAdapter();
                m_dataAdapter.SelectCommand = new SqlCommand(string.Format("select * from {0}", tblName), cnn);
            }
            public override void Search(List<string> exprs, List<lEntity> arr)
            {
                string sql = string.Format("select * from {0} ", m_table);

                if (exprs.Count > 0)
                {
                    sql += " where " + string.Join(" and ", exprs);
                    SqlCommand selectCommand = new SqlCommand(sql, m_cnn);
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
            public override void Reload()
            {
                GetData(m_dataAdapter.SelectCommand);
            }
            public override void Submit()
            {
                using (SqlCommandBuilder builder = new SqlCommandBuilder(m_dataAdapter))
                {
                    DataTable dt = (DataTable)m_bindingSource.DataSource;
                    if (dt != null)
                    {
                        m_dataAdapter.UpdateCommand = builder.GetUpdateCommand();
                        m_dataAdapter.Update(dt);
                    }
                }
            }
            private void GetData(string selectStr)
            {
                SqlCommand selectCommand = new SqlCommand(selectStr, m_cnn);
                GetData(selectCommand);
            }
            private void GetData(SqlCommand selectCommand)
            {
                m_dataAdapter.SelectCommand = selectCommand;
                // Populate a new data table and bind it to the BindingSource.
                DataTable table = new DataTable();
                table.Locale = System.Globalization.CultureInfo.InvariantCulture;
                m_dataAdapter.Fill(table);
                m_bindingSource.DataSource = table;
            }
        }

        class lDataSync
        {
            private lDataContent m_data;
            public lDataSync(lDataContent data)
            {
                m_data = data;
            }
            public void LoadData()
            {
                m_data.Reload();
            }
            public DataTable m_dataSource
            {
                get { return (DataTable)m_data.m_bindingSource.DataSource; }
            }
            public void Add(string newValue)
            {
                //single col tables
                DataRow newRow = m_dataSource.NewRow();
                newRow[1] = newValue;
                m_dataSource.Rows.Add(newRow);
                m_data.Submit();
            }
        }
    }
}