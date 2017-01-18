using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test_autocomplete
{
    public partial class Form1 : Form
    {
        //private TextBox m_txt;
        myTextEdit m_txt;
        public Form1()
        {
            InitializeComponent();

            m_txt = new myTextEdit();
           

            m_txt.Size = new Size(100, 30);
            this.Controls.Add(m_txt);
            

            this.Load += new System.EventHandler(Form1_Load);

            Button btn = new Button();
            btn.AutoSize = true;
            btn.Text = "test btn";
            this.Controls.Add(btn);
            btn.Location = new Point(0, 40);
            btn.Click += Btn_Click;
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Debug.WriteLine("Btn_Click");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
#if false
            string dbPath = "test.db";
            if (!System.IO.File.Exists(dbPath))
            {
                SQLiteConnection.CreateFile(dbPath);
            }
            SQLiteConnection cnn = new SQLiteConnection(string.Format("Data Source={0};Version=3;", dbPath));
            cnn.Open();
            string qry = "select * from group_name";
            SQLiteCommand cmd = new SQLiteCommand(qry, cnn);
            SQLiteDataReader rd = cmd.ExecuteReader();
            AutoCompleteStringCollection col = new AutoCompleteStringCollection();
            while(rd.Read())
            {
                col.Add(rd[1].ToString());
            }
            rd.Close();
            m_txt.AutoCompleteCustomSource = col;
#endif
            m_txt.LoadData();
        }

        class myTextEdit
        {
            TextBox m_txt;
            DataSync m_data;
            public myTextEdit(){
                m_data = DataSync.createInstance();
                m_data.m_qry = "select * from group_name";

                m_txt = new TextBox();
                m_txt.AutoCompleteMode = AutoCompleteMode.Suggest;
                m_txt.AutoCompleteSource = AutoCompleteSource.CustomSource;
                m_txt.Validated += M_txt_Validated;
            }

            public static implicit operator TextBox(myTextEdit edt)
            {
                return edt.m_txt;
            }
            public Point Location
            {
                get
                {
                    return m_txt.Location;
                }
                set
                {
                    m_txt.Location = value;
                }
            }
            public Size Size
            {
                get
                {
                    return m_txt.Size;
                }
                set
                {
                    m_txt.Size = value;
                }
            }
            public void LoadData()
            {
                AutoCompleteStringCollection col = new AutoCompleteStringCollection();
                DataTable tbl = m_data.getData();
                foreach(DataRow row in tbl.Rows)
                {
                    col.Add(row[1].ToString());
                }
                m_txt.AutoCompleteCustomSource = col;
            }
            private void M_txt_LostFocus(object sender, EventArgs e)
            {
                Debug.WriteLine("M_txt_LostFocus:" + ((TextBox)sender).Text);
            }

            private void M_txt_Validated(object sender, EventArgs e)
            {
                Debug.WriteLine("M_txt_Validated:" + ((TextBox)sender).Text);
                string selectedValue = m_txt.Text;
                if (selectedValue != "" && !m_txt.AutoCompleteCustomSource.Contains(selectedValue))
                {
                    m_txt.AutoCompleteCustomSource.Add(selectedValue);
                    m_data.sync(selectedValue);
                }
            }

            private void M_txt_TextChanged(object sender, EventArgs e)
            {
                Debug.WriteLine("M_txt_TextChanged:" + ((TextBox)sender).Text);
            }
        }

        class DataSync
        {
            private SQLiteDataAdapter m_dataAdapter = new SQLiteDataAdapter();

            public string m_qry;
            private SQLiteConnection m_cnn;
            private DataTable m_dataTbl;

            DataSync() {
                string dbPath = "test.db";
                if (!System.IO.File.Exists(dbPath))
                {
                    SQLiteConnection.CreateFile(dbPath);
                }
                m_cnn = new SQLiteConnection(string.Format("Data Source={0};Version=3;", dbPath));
                m_cnn.Open();
            }

            public static DataSync createInstance()
            {
                return new DataSync();
            }


            DataSet ds = new DataSet();
            public DataTable getData()
            {
                m_dataAdapter.SelectCommand = new SQLiteCommand(m_qry, m_cnn);
                m_dataAdapter.Fill(ds);
                m_dataTbl = ds.Tables[0];
                return m_dataTbl;
            }

            public void sync(string newValue)
            {
                DataRow newRow = m_dataTbl.NewRow();
                newRow[1] = newValue;
                m_dataTbl.Rows.Add(newRow);
                sync();
            }
            public void sync()
            {
                using (SQLiteCommandBuilder builder = new SQLiteCommandBuilder(m_dataAdapter))
                {
                    //m_dataAdapter.DeleteCommand = builder.GetDeleteCommand(true);
                    m_dataAdapter.UpdateCommand = builder.GetUpdateCommand(true);
                    m_dataAdapter.Update(ds);
                }
            }
        }
    }
}
