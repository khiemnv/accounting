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
		private void crtMenu()
        {
            MainMenu mainMenu = new MainMenu();
            this.Menu = mainMenu;
            MenuItem miFile = new MenuItem("&File");
            MenuItem miHelp = new MenuItem("&Help");
            mainMenu.MenuItems.AddRange(new MenuItem[] { miFile, miHelp });

            MenuItem miClose = new MenuItem("&Close");
            miFile.MenuItems.Add(miClose);
            miClose.Click += MiClose_Click;

            MenuItem miAbout = new MenuItem("&About");
            miHelp.MenuItems.Add(miAbout);
            miAbout.Click += MiAbout_Click;
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
