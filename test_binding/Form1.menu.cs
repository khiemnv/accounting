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

        class myCmdDlg : CommonDialog
        {
            public override void Reset()
            {
                throw new NotImplementedException();
            }

            protected override bool RunDialog(IntPtr hwndOwner)
            {
                //throw new NotImplementedException();
                return false;
            }
        }
        private void MiAbout_Click(object sender, EventArgs e)
        {
#if true
            var ret = MessageBox.Show("About this app");
#else
            //show about dlg
            Form aboutDlg = new Form();
            aboutDlg.Location = this.Location;
            aboutDlg.Size = new Size(400, 300);
            aboutDlg.FormBorderStyle = FormBorderStyle.FixedSingle;

            Label txt = new Label();
            txt.Text = "About this app";
            txt.Dock = DockStyle.Fill;
            aboutDlg.Controls.Add(txt);

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            DialogResult ret = aboutDlg.ShowDialog(this);
            aboutDlg.Dispose();
#endif
        }
    }
}
