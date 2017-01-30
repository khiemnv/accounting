using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_data
{
    class Program
    {
        static void Main(string[] args)
        {
            string dbPath = @"E:\tmp\accounting\test_binding\appData.db";
            SQLiteConnection cnn = new SQLiteConnection(string.Format("Data Source={0};Version=3;", dbPath));
            cnn.Open();

            var cmd = new SQLiteCommand();
            cmd.Connection = cnn;
            var transaction  = cnn.BeginTransaction();
            for (int i = 0; i < 1000*1000; i++)
            {
                var rec = createReceiptsRec();
                cmd.CommandText = string.Format("insert into receipts(date, receipt_number, name, content, amount, note) "
                    + "values('{0}','{1}','{2}','{3}','{4}','{5}')", rec[0], rec[1], rec[2], rec[3], rec[4], rec[5]);
                var ret = cmd.ExecuteNonQuery();
            }
            transaction.Commit();
        }

        static string[] createReceiptsRec()
        {
            int nFields = 6;
            var rec = new string[nFields];
            rec[0] = genDate();
            rec[1] = genRcptNum();
            rec[2] = genName();
            rec[3] = genContent();
            rec[4] = genAmount();
            rec[5] = getNote();
            return rec;
        }

        static int noteNum = 0;
        static private string getNote()
        {
            return "ghi chú số " + noteNum++;
        }

        static private string genAmount()
        {
            return (rnd.Next() % Math.Pow(10, 6)).ToString() + "000";
        }

        static string[] contents = new string[] { "Sư Phụ giao", "Đổ hòm công đức", "Nguồn thu khác" };
        static private string genContent()
        {
            return contents[rnd.Next() % contents.Length];
        }

        static string[] first = new string[] { "Nguyễn", "Đào", "Ngô", "Phạm" };
        static string[] mid = new string[] {"Thị", "Văn" };
        static string[] last = new string[] {"Ngọc", "Ninh", "Minh", "An", "Minh Anh", "Bảo Châu" };
        static private string genName()
        {
            return string.Format("{0} {1} {2}",
                first[rnd.Next() % first.Length],
                mid[rnd.Next() % mid.Length],
                last[rnd.Next() % last.Length]
                );
        }

        static int m_baseNum = 0;
        static private string genRcptNum()
        {
            return string.Format("rcpt_num_{0}", m_baseNum++.ToString("000000"));
        }

        static Random rnd = new Random(Environment.TickCount);
        static int startYear = 2012;
        static int[] days = new int[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31, 30, 31 };
        static private string genDate()
        {
            int m = rnd.Next() % 12;
            int y = rnd.Next() % 5 + startYear;
            int d = rnd.Next() % days[m] + 1;
            return string.Format("{0}-{1}-{2} 00:00:00", y, (m+1).ToString("00"), d.ToString("00"));
        }
    }
}
