using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace thtkGUI_CS
{
    public partial class Help : Form
    {
        public Help()
        {
            InitializeComponent();
        }

        private void Help_Load(object sender, EventArgs e)
        {
            try
            {
                StreamReader fs = new StreamReader("config/help.txt");
                hp.Text = fs.ReadToEnd();
            }catch (Exception ex){
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
