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
    public partial class ANM_Entry_choose : Form
    {
        public string entry="";
        public string str_anm_desFile = null;
        public ANM_Entry_choose()
        {
            InitializeComponent();
        }
        private void ANM_Entry_choose_Load(object sender, EventArgs e)
        {
            try
            {
                Form_main owner = (Owner as Form_main);
                btn_OK.Text = owner.language.GetValue("entrySelect", "OK", "OK");
                btn_Cancel.Text = owner.language.GetValue("entrySelect", "Cancel", "Cancel");
                this.Text = owner.language.GetValue("entrySelect", "choose_entry", "choose entry");
                StreamReader fs = new StreamReader(str_anm_desFile);
                string ln=fs.ReadLine();
                string flag = owner.config_command.GetValue(owner.ver_thtk, "thanm_entry_flag", "Name:");
                this.BackgroundImage = owner.BackgroundImage;
                this.Opacity = owner.Opacity;
                while (ln!=null)
                {
                    int ind= ln.IndexOf(flag);
                    if(ind>=0)
                    {
                        list_entry.Items.Add(ln.Substring(ind+flag.Length).Trim(' '));
                    }
                    ln = fs.ReadLine();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }
        private void Btn_OK_Click(object sender, EventArgs e)
        {
            Choose_exit();
        }
        private void Choose_exit()
        {
            if (list_entry.Items.Count > 0 && list_entry.SelectedIndex >= 0)
            {
                entry = list_entry.SelectedItem.ToString();
            }
            Close();
        }
        private void Btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void List_entry_DoubleClick(object sender, EventArgs e)
        {
            Choose_exit();
        }

        private void List_entry_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
