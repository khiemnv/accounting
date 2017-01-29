using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test_dlg
{
    class Program
    {
        class lGroupNameEditDlg : Form
        {
            public lGroupNameEditDlg()
            {
                InitializeComponent();
            }

            void pickColor(out Color color)
            {
                ColorDialog dlg = new ColorDialog();
                var ret = dlg.ShowDialog();
                color = dlg.Color;
                dlg.Dispose();
            }

            private void InitializeComponent()
            {
                Form form = this;
                form.Location = new Point(0, 0);
                form.Size = new Size(400, 300);
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MinimizeBox = false;
                form.MaximizeBox = false;

                DataGridView dgv = new DataGridView();
                Color color;
                pickColor(out color);
                //dgv.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle() { BackColor = Color.Blue };
                dgv.ColumnHeadersDefaultCellStyle.BackColor = color;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font(dgv.ColumnHeadersDefaultCellStyle.Font, FontStyle.Bold);
                form.Controls.Add(dgv);
                dgv.AutoSize = true;
                dgv.Dock = DockStyle.Fill;

                DataTable table = new DataTable();
                table.Columns.Add("Dosage", typeof(int));
                table.Columns.Add("Drug", typeof(string));
                table.Columns.Add("Patient", typeof(string));
                table.Columns.Add("Date", typeof(DateTime));

                table.Rows.Add(25, "Indocin", "David", DateTime.Now);
                table.Rows.Add(50, "Enebrel", "Sam", DateTime.Now);
                table.Rows.Add(10, "Hydralazine", "Christoff", DateTime.Now);
                table.Rows.Add(21, "Combivent", "Janet", DateTime.Now);
                table.Rows.Add(100, "Dilantin", "Melanie", DateTime.Now);

                dgv.DataSource = table;
            }
        }
        class lAboutDlg : Form
        {
            public lAboutDlg()
            {
                InitializeComponent();
            }

            private void InitializeComponent()
            {
                //show about dlg
                Form aboutDlg = this;
                aboutDlg.Location = new Point(0, 0);
                aboutDlg.Size = new Size(200, 150);
                aboutDlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                aboutDlg.MinimizeBox = false;
                aboutDlg.MaximizeBox = false;

                Label txt = new Label();
                txt.Text = "About this app";
                txt.TextAlign = ContentAlignment.MiddleCenter;
                txt.Height = 80;
                txt.Dock = DockStyle.Fill;

                Button btn = new Button();
                btn.Text = "OK";
                btn.TextAlign = ContentAlignment.MiddleCenter;
                btn.Anchor = AnchorStyles.None;
                btn.AutoSize = true;
                btn.Click += Btn_Click;
                aboutDlg.AcceptButton = btn;

                //FlowLayoutPanel panel = new FlowLayoutPanel();
                //panel.FlowDirection = FlowDirection.TopDown;
                //panel.BorderStyle = BorderStyle.FixedSingle;
                //panel.Dock = DockStyle.Fill;
                //panel.Controls.Add(txt);
                //panel.Controls.Add(btn);

                TableLayoutPanel panel = new TableLayoutPanel();
                panel.ColumnCount = 1;
                panel.RowCount = 3;
                panel.Dock = DockStyle.Fill;
                panel.Controls.Add(txt, 0, 0);
                //tblPanel.SetRowSpan(txt, 2);
                panel.Controls.Add(btn, 0, 1);
                panel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;

                aboutDlg.Controls.Add(panel);
            }

            private void Btn_Click(object sender, EventArgs e)
            {
                Close();
            }
        }
        static void Main(string[] args)
        {
            Form aboutDlg = new lGroupNameEditDlg();
            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            DialogResult ret = aboutDlg.ShowDialog();
            aboutDlg.Dispose();
        }
    }
}
