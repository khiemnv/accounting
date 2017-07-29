#define use_custom_mi

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace test_binding
{
    public partial class Form1 : Form
    {
        private MenuItem crtMenuItem(string text)
        {
#if use_custom_mi
            return new myMenuItem(text.Replace("&", ""));
#else
            return new MenuItem(text);
#endif
        }
		private void crtMenu()
        {
            MainMenu mainMenu = new MainMenu();
            this.Menu = mainMenu;

            //File
            //  Close
            MenuItem miFile = crtMenuItem("&File");
            MenuItem miClose = crtMenuItem("&Close");
            miFile.MenuItems.Add(miClose);
            miClose.Click += MiClose_Click;

            //Help
            //  About
            MenuItem miHelp = crtMenuItem("&Help");
            MenuItem miAbout = crtMenuItem("&About");
            miHelp.MenuItems.Add(miAbout);
            miAbout.Click += MiAbout_Click;

            //Edit
            //  GroupName
            //  ReceiptsContent
            //  Building
            MenuItem miEdit = crtMenuItem("&Edit");

            MenuItem miEditGroupName = crtMenuItem("Danh Sách Các Ban");
            miEditGroupName.Click += MiEditGroupName_Click;
            miEdit.MenuItems.Add(miEditGroupName);

            MenuItem miEditReceiptsContent = crtMenuItem("Nguồn Thu");
            miEditReceiptsContent.Click += MiEditReceiptsContent_Click;
            miEdit.MenuItems.Add(miEditReceiptsContent);

            MenuItem miBuilding = crtMenuItem("Công Trình");
            miBuilding.Click += MiBuilding_Click;
            miEdit.MenuItems.Add(miBuilding);

            //Report
            MenuItem miReport = crtMenuItem("&Report");
            miReport.Click += MiReport_Click;

            mainMenu.MenuItems.AddRange(new MenuItem[] { miFile, miEdit, miReport, miHelp });
        }

        private void MiReport_Click(object sender, EventArgs e)
        {
            lReportDlg dlg = new lReportDlg();
            dlg.ShowDialog();
            dlg.Dispose();
        }

        private void MiBuilding_Click(object sender, EventArgs e)
        {
            lEditDlg edtDlg = new lBuildingEditDlg();
            edtDlg.ShowDialog();
            edtDlg.Dispose();
        }

        private void MiEditReceiptsContent_Click(object sender, EventArgs e)
        {
            lEditDlg edtDlg = new lReceiptsContentEditDlg();
            edtDlg.ShowDialog();
            edtDlg.Dispose();
        }

        private void MiEditGroupName_Click(object sender, EventArgs e)
        {
            lEditDlg edtDlg = new lGroupNameEditDlg();
            edtDlg.ShowDialog();
            edtDlg.Dispose();
        }

        private void MiClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
                aboutDlg.Size = new Size(150, 120);
                aboutDlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                aboutDlg.MinimizeBox = false;
                aboutDlg.MaximizeBox = false;

                TableLayoutPanel tblPanel = new TableLayoutPanel();
                tblPanel.ColumnCount = 1;
                tblPanel.Dock = DockStyle.Fill;

                Label txt = new Label();
                txt.Text = "About this app";
                //txt.AutoSize = true;
                txt.Dock = DockStyle.Fill;
                //txt.Dock = DockStyle.Fill;

                Button btn = new Button();
                btn.Text = "OK";
                btn.TextAlign = ContentAlignment.MiddleCenter;
                btn.Anchor = AnchorStyles.None;
                btn.AutoSize = true;
                btn.Click += Btn_Click;
                aboutDlg.AcceptButton = btn;

                tblPanel.Controls.Add(txt, 0, 0);
                tblPanel.Controls.Add(btn, 0, 1);

                aboutDlg.Controls.Add(tblPanel);
            }

            private void Btn_Click(object sender, EventArgs e)
            {
                Close();
            }
        }
        private void MiAbout_Click(object sender, EventArgs e)
        {
#if false
            var ret = MessageBox.Show("About this app");
#else
            lAboutDlg aboutDlg = new lAboutDlg();
            DialogResult ret = aboutDlg.ShowDialog();
            aboutDlg.Dispose();
#endif
        }
    }
}
