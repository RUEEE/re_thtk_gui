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
    public partial class Form_main : Form
    {
        private int is_Errror = 0;
        public Image form_background = null;
        public ConfigFile config_main = null;
        public ConfigFile config_language = null;
        public ConfigFile config_command = null;
        public ConfigFile config_game = null;
        public ConfigFile language = null;//当前选中的language的配置
        public string ver_thtk = null;//如thtk11
        private Dictionary<string, string> langs = null;
        private Dictionary<string, string> games = null;
        //private GameSetting game_setting;//存储各种路径
        private bool is_use_ECLMAP = true;
        private bool is_use_ANMMAP = false;
        private bool is_use_STDMAP = false;
        private bool is_use_MSGMAP = false;
        private string ecl_suffix = null;
        private string anm_suffix = null;
        private string std_suffix = null;
        private string msg_suffix = null;
        private string ecl_suffix_file = "txt|*.txt|allFile|*.*";
        private string anm_suffix_file = "txt|*.txt|allFile|*.*";
        private string std_suffix_file = "txt|*.txt|allFile|*.*";
        private string msg_suffix_file = "txt|*.txt|allFile|*.*";
        private string now_game = null;//如 TH06东方红魔乡
        private string now_game_enter = null;//如15,165
        string language_selected = null;
        public Form_main()
        {
            InitializeComponent();
        }
        private void Form_main_Load(object sender, EventArgs e)
        {
            ConfigFile config = new ConfigFile("config/all_game.ini");
            try
            {
                config_main = new ConfigFile("config/main_config.ini");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                is_Errror = 1;
                this.Close();
                return;
            }
            //加载mainconfig
            Init_Settings();
            Init_Language();
            Init_Game();
            Init_Command();
            Init_FormPos();
        }
        private void Init_Game()
        {
            try
            {
                config_game = new ConfigFile("config/all_game.ini");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                is_Errror = 1;
                this.Close();
                return;
            }
            games = config_game.GetKeys("game");
            if (games == null)
            {
                MessageBox.Show("未检测到config/games.ini中的[game]节,请重试", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                is_Errror = 1;
                this.Close();
                return;
            }
            string now_game_buf = config_main.GetValue("main", "game", "-1");
            foreach (var game in games)
            {
                var l = new ToolStripMenuItem(game.Key);
                l.Click += Game_ChooseBox_Click;
                if (game.Key == now_game_buf)
                    l.Checked = true;
                else l.Checked = false;
                GameToolStripMenuItem.DropDownItems.Add(l);
            }

            Change_game(now_game_buf);
            now_game = now_game_buf;
        }
        private void Init_Command()
        {
            try
            {
                string buf;
                buf = config_main.GetValue("main", "thtk_version", "thtk11");
                ver_thtk = buf;
                config_command = new ConfigFile("config/command.ini");
                var all_thtk_ver = config_command.GetSections();
                foreach (var thtk in all_thtk_ver)
                {
                    var l = new ToolStripMenuItem(thtk);
                    l.Click += THTK_ChooseBox_Click;
                    if (thtk == buf)
                        l.Checked = true;
                    else l.Checked = false;
                    thtkVersionToolStripMenuItem.DropDownItems.Add(l);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                is_Errror = 1;
                this.Close();
                return;
            }
            ver_thtk = config_main.GetValue("main", "thtk_version", "-1");
            if (ver_thtk == "-1")
            {
                MessageBox.Show("unknown thtk ver", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                is_Errror = 1;
                this.Close();
                return;
            }
        }
        private void Init_Settings()
        {
            string buf;
            try
            {
                ecl_suffix = config_main.GetValue("saveFile", "eclFile_suffix", ".decl");
                anm_suffix = config_main.GetValue("saveFile", "anmDescription_file_suffix", ".ddes");
                std_suffix = config_main.GetValue("saveFile", "stdFile_suffix", ".dstd");
                msg_suffix = config_main.GetValue("saveFile", "msgFile_suffix", ".dmsg");
                if (ecl_suffix != ".txt" && ecl_suffix != "")
                    ecl_suffix_file = "ecl|*" + ecl_suffix + ";*.txt|allFile|*.*";
                if (anm_suffix != ".txt" && anm_suffix != "")
                    anm_suffix_file = "des|*" + anm_suffix + ";*.txt|allFile|*.*";
                if (std_suffix != ".txt" && std_suffix != "")
                    std_suffix_file = "std|*" + std_suffix + ";*.txt|allFile|*.*";
                if (msg_suffix != ".txt" && msg_suffix != "")
                    msg_suffix_file = "msg|*" + msg_suffix + ";*.txt|allFile|*.*";
                buf = config_main.GetValue("main", "checkBox_is_ed", "false");
                if (Convert.ToBoolean(buf))
                {
                    cbox_ed.Checked = true;
                }
                buf = config_main.GetValue("main", "enable_anm_map", "false");
                if (Convert.ToBoolean(buf))
                {
                    ANMMAPToolStripMenuItem.Checked = true; is_use_ANMMAP = true;
                }
                else
                {
                    ANMMAPToolStripMenuItem.Checked = false; is_use_ANMMAP = false;
                }
                buf = config_main.GetValue("main", "enable_ecl_map", "true");
                if (Convert.ToBoolean(buf))
                {
                    ECLMAPToolStripMenuItem.Checked = true; is_use_ECLMAP = true;
                }
                else
                {
                    ECLMAPToolStripMenuItem.Checked = false; is_use_ECLMAP = false;
                }
                buf = config_main.GetValue("main", "enable_std_map", "false");
                if (Convert.ToBoolean(buf))
                {
                    STDMAPToolStripMenuItem.Checked = true; is_use_STDMAP = true;
                }
                else
                {
                    STDMAPToolStripMenuItem.Checked = false; is_use_STDMAP = false;
                }
                buf = config_main.GetValue("main", "enable_msg_map", "false");
                if (Convert.ToBoolean(buf))
                {
                    MSGMAPToolStripMenuItem.Checked = true; is_use_MSGMAP = true;
                }
                else
                {
                    MSGMAPToolStripMenuItem.Checked = false; is_use_MSGMAP = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("加载设置错误," + ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                is_Errror = 1;
                this.Close();
                return;
            }
        }
        private void Init_Language()
        {
            try
            {
                config_language = new ConfigFile("config/language/all_language.ini");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                is_Errror = 1;
                this.Close();
                return;
            }//加载language
            langs = config_language.GetKeys("language");
            if (langs == null)
            {
                MessageBox.Show("未检测到config/language/all_language.ini中的[language]节,请重试", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                is_Errror = 1;
                this.Close();
                return;
            }
            string now_language = config_main.GetValue("main", "language", "en_us");
            foreach (var lang in langs)
            {
                var l = new ToolStripMenuItem(lang.Key);
                l.Click += Language_ChooseBox_Click;
                if (lang.Key == now_language)
                    l.Checked = true;
                else l.Checked = false;
                LanguageToolStripMenuItem.DropDownItems.Add(l);
            }
            Change_language(now_language);
            //修改语言完毕
        }
        private void Init_FormPos()
        {
            string buf, buf2;
            buf = config_main.GetValue("form_set", "height", "500");
            this.Height = Convert.ToInt32(buf);
            buf = config_main.GetValue("form_set", "width", "800");
            this.Width = Convert.ToInt32(buf);
            buf = config_main.GetValue("form_set", "loc_x", "0");
            buf2 = config_main.GetValue("form_set", "loc_y", "0");
            if (Convert.ToInt32(buf) >= 0 && Convert.ToInt32(buf2) >= 0)
                this.Location = new Point(Convert.ToInt32(buf), Convert.ToInt32(buf2));
            buf = config_main.GetValue("form_set", "icon", "0");
            if (buf != "default")
            {
                try
                {
                    this.Icon = new Icon(@"config\" + buf);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    is_Errror = 1;
                    this.Close();
                    return;
                }
            }
            buf = config_main.GetValue("form_set", "background", "0");
            if (buf != "default")
            {
                try
                {
                    form_background = Image.FromFile(@"config\" + buf);
                    this.BackgroundImage = form_background;
                    var alpha = Convert.ToInt32(config_main.GetValue("form_set", "alpha", "120"));
                    var argb_folder = Color.FromArgb(alpha, Folder_thdat.BackColor.R, Folder_thdat.BackColor.G, Folder_thdat.BackColor.B);
                    var argb_lbl = Color.FromArgb(0, lbl_archive.BackColor.R, lbl_archive.BackColor.G, lbl_archive.BackColor.B);
                    Folder_thdat.BackColor = argb_folder;
                    Folder_thanm.BackColor = argb_folder;
                    Folder_thecl.BackColor = argb_folder;
                    Folder_thstd.BackColor = argb_folder;
                    Folder_thmsg.BackColor = argb_folder;
                    lbl_archive.BackColor = argb_lbl;
                    lbl_archive2.BackColor = argb_lbl;
                    lbl_archive3.BackColor = argb_lbl;
                    lbl_archive4.BackColor = argb_lbl;
                    lbl_archive5.BackColor = argb_lbl;
                    lbl_cap.BackColor = argb_lbl;
                    lbl_des.BackColor = argb_lbl;
                    lbl_entry.BackColor = argb_lbl;
                    lbl_entry_file.BackColor = argb_lbl;
                    lbl_file.BackColor = argb_lbl;
                    lbl_file2.BackColor = argb_lbl;
                    lbl_file3.BackColor = argb_lbl;
                    lbl_folder.BackColor = argb_lbl;
                    lbl_folder2.BackColor = argb_lbl;
                    lbl_dat_list.BackColor = argb_lbl;
                    Menu_main.BackColor = argb_folder;
                    if (this.MinimumSize.Width * form_background.Height / form_background.Width > this.MinimumSize.Height)
                    {
                        this.MinimumSize =
                        new Size(this.MinimumSize.Width, this.MinimumSize.Width * form_background.Height / form_background.Width);
                    }
                    else
                    {
                        this.MinimumSize =
                        new Size(this.MinimumSize.Height * form_background.Width / form_background.Height, this.MinimumSize.Height);
                    }
                    Height = Width * form_background.Height / form_background.Width;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    is_Errror = 1;
                    this.Close();
                    return;
                }
            }
            buf = config_main.GetValue("form_set", "alpha_bk", "255");
            this.Opacity = (float)(Convert.ToDouble(buf) / 255.0);
        }
        private void Change_language(string lang_selected)
        {
            string lang_file;
            language_selected = lang_selected;
            if (!langs.ContainsKey(lang_selected))
            {
                MessageBox.Show("找不到对应语言", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            lang_file = langs[lang_selected];
            try
            {
                language = new ConfigFile("config/language/" + lang_file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show("修改语言失败", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            GameToolStripMenuItem.Text = language.GetValue("menu", "Game", "Game");
            SettingsToolStripMenuItem.Text = language.GetValue("menu", "Settings", "Settings");
            HelpToolStripMenuItem.Text = language.GetValue("menu", "Help", "Help");
            LanguageToolStripMenuItem.Text = language.GetValue("menu", "Language", "Language");
            AboutToolStripMenuItem.Text = language.GetValue("menu", "About", "About");
            HelpToolStripMenuItem1.Text = language.GetValue("menu", "Help1", "Help");
            MapToolStripMenuItem.Text = language.GetValue("menu", "Maps", "Maps");
            thtkVersionToolStripMenuItem.Text = language.GetValue("menu", "THTK_version", "THTK version");
            ANMMAPToolStripMenuItem.Text = language.GetValue("menu", "ANM_MAP", "ANM_MAP");
            ECLMAPToolStripMenuItem.Text = language.GetValue("menu", "ECL_MAP", "ECL_MAP");
            STDMAPToolStripMenuItem.Text = language.GetValue("menu", "STD_MAP", "STD_MAP");
            MSGMAPToolStripMenuItem.Text = language.GetValue("menu", "MSG_MAP", "MSG_MAP");
            //dat
            Folder_thdat.Text = language.GetValue("thdat", "thdat", "thdat");
            lbl_folder.Text = language.GetValue("thdat", "folder", "folder");
            lbl_archive.Text = language.GetValue("thdat", "archive", "archive");
            lbl_dat_list.Text = language.GetValue("thdat", "list", "list");
            btn_thdat_pack.Text = language.GetValue("thdat", "pack", "pack");
            btn_thdat_unpack.Text = language.GetValue("thdat", "unpack", "unpack");
            btn_thdat_getList.Text = language.GetValue("thdat", "get_file_list", "get file list");
            //anm
            Folder_thanm.Text = language.GetValue("thanm", "thanm", "thanm");
            lbl_folder2.Text = language.GetValue("thanm", "folder", "folder");
            lbl_archive2.Text = language.GetValue("thanm", "archive", "archive");
            lbl_des.Text = language.GetValue("thanm", "desFile", "desFile");
            btn_thanm_pack.Text = language.GetValue("thanm", "pack", "pack");
            btn_thanm_unpack.Text = language.GetValue("thanm", "unpack", "unpack");
            btn_thanm_getDes.Text = language.GetValue("thanm", "get_des", "get description");
            lbl_entry.Text = language.GetValue("thanm", "entry", "entry");
            lbl_entry_file.Text = language.GetValue("thanm", "entry_file", "file");
            btn_thanm_rep.Text = language.GetValue("thanm", "replace", "replace");
            //ecl
            Folder_thecl.Text = language.GetValue("thecl", "thecl", "thecl");
            lbl_file.Text = language.GetValue("thecl", "file", "file");
            lbl_archive3.Text = language.GetValue("thecl", "archive", "archive");
            btn_thecl_pack.Text = language.GetValue("thecl", "pack", "pack");
            btn_thecl_unpack.Text = language.GetValue("thecl", "unpack", "unpack");
            //msg
            Folder_thmsg.Text = language.GetValue("thmsg", "thmsg", "thmsg");
            lbl_file2.Text = language.GetValue("thmsg", "file", "file");
            lbl_archive4.Text = language.GetValue("thmsg", "archive", "archive");
            btn_thmsg_pack.Text = language.GetValue("thmsg", "pack", "pack");
            btn_thmsg_unpack.Text = language.GetValue("thmsg", "unpack", "unpack");
            cbox_ed.Text = language.GetValue("thmsg", "is_ed", "is ed");
            //std
            Folder_thstd.Text = language.GetValue("thstd", "thstd", "thstd");
            lbl_file3.Text = language.GetValue("thstd", "file", "file");
            lbl_archive5.Text = language.GetValue("thstd", "archive", "archive");
            btn_thstd_pack.Text = language.GetValue("thstd", "pack", "pack");
            btn_thstd_unpack.Text = language.GetValue("thstd", "unpack", "unpack");
            //error
        }
        private void Language_ChooseBox_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                Change_language((sender as ToolStripMenuItem).Text);
            }
            for (int i = 0; i < LanguageToolStripMenuItem.DropDownItems.Count; i++)
            {
                ((LanguageToolStripMenuItem.DropDownItems[i]) as ToolStripMenuItem).Checked = false;
            }
            (sender as ToolStripMenuItem).Checked = true;
        }
        private void Change_game(string nwGame)
        {
            try
            {
                if (now_game != null)
                {
                    Save_game(now_game);
                }
                if (nwGame == "-1")
                {
                    MessageBox.Show("未找到游戏", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    is_Errror = 1;
                    this.Close();
                    return;
                }
                lbl_cap.Text = nwGame;
                txt_thdat_list.Text = config_game.GetValue(nwGame, "path_thdat_list", "");
                txt_thdat_folder.Text = config_game.GetValue(nwGame, "path_thdat_up", "");
                txt_thdat_archive.Text = config_game.GetValue(nwGame, "path_thdat_p", "");
                txt_thanm_folder.Text = config_game.GetValue(nwGame, "path_thanm_up", "");
                txt_thanm_archive.Text = config_game.GetValue(nwGame, "path_thanm_p", "");
                txt_thanm_des.Text = config_game.GetValue(nwGame, "path_thanm_des", "");
                cmb_entry.Text = config_game.GetValue(nwGame, "path_thanm_entry", "");
                txt_thanm_entry_file.Text = config_game.GetValue(nwGame, "path_thanm_file", "");
                txt_thecl_archive.Text = config_game.GetValue(nwGame, "path_thecl_p", "");
                txt_thecl_file.Text = config_game.GetValue(nwGame, "path_thecl_up", "");
                txt_thmsg_archive.Text = config_game.GetValue(nwGame, "path_thmsg_p", "");
                txt_thmsg_file.Text = config_game.GetValue(nwGame, "path_thmsg_up", "");
                txt_thstd_archive.Text = config_game.GetValue(nwGame, "path_thstd_p", "");
                txt_thstd_file.Text = config_game.GetValue(nwGame, "path_thstd_up", "");
                now_game = nwGame;
                now_game_enter = config_game.GetValue("game", nwGame, "-1");
                if (now_game_enter == "-1")
                {
                    MessageBox.Show("不明游戏版本,请确认config", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void Save_game(string nwGame)
        {
            config_game.SetValue(nwGame, "path_thdat_up", txt_thdat_folder.Text);
            config_game.SetValue(nwGame, "path_thdat_p", txt_thdat_archive.Text);
            config_game.SetValue(nwGame, "path_thdat_list", txt_thdat_list.Text);

            config_game.SetValue(nwGame, "path_thanm_up", txt_thanm_folder.Text);
            config_game.SetValue(nwGame, "path_thanm_p", txt_thanm_archive.Text);
            config_game.SetValue(nwGame, "path_thanm_des", txt_thanm_des.Text);
            config_game.SetValue(nwGame, "path_thanm_entry", cmb_entry.Text);
            config_game.SetValue(nwGame, "path_thanm_file", txt_thanm_entry_file.Text);

            config_game.SetValue(nwGame, "path_thecl_p", txt_thecl_archive.Text);
            config_game.SetValue(nwGame, "path_thecl_up", txt_thecl_file.Text);

            config_game.SetValue(nwGame, "path_thmsg_p", txt_thmsg_archive.Text);
            config_game.SetValue(nwGame, "path_thmsg_up", txt_thmsg_file.Text);

            config_game.SetValue(nwGame, "path_thstd_p", txt_thstd_archive.Text);
            config_game.SetValue(nwGame, "path_thstd_up", txt_thstd_file.Text);
            config_game.Save();
        }
        private void THTK_ChooseBox_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ver_thtk = (sender as ToolStripMenuItem).Text;
            }
            for (int i = 0; i < thtkVersionToolStripMenuItem.DropDownItems.Count; i++)
            {
                ((thtkVersionToolStripMenuItem.DropDownItems[i]) as ToolStripMenuItem).Checked = false;
            }
            (sender as ToolStripMenuItem).Checked = true;
        }
        private void Game_ChooseBox_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                Change_game((sender as ToolStripMenuItem).Text);
            }
            for (int i = 0; i < GameToolStripMenuItem.DropDownItems.Count; i++)
            {
                ((GameToolStripMenuItem.DropDownItems[i]) as ToolStripMenuItem).Checked = false;
            }
            (sender as ToolStripMenuItem).Checked = true;
        }
        private void Form_main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (is_Errror != 1)
            {
                try
                {
                    //修改参数
                    if (form_background != null)
                    {
                        Height = Width * form_background.Height / form_background.Width;
                    }
                    config_main.SetValue("main", "checkBox_is_ed", this.cbox_ed.Checked.ToString());
                    config_main.SetValue("main", "language", this.language_selected);
                    config_main.SetValue("main", "enable_anm_map", this.is_use_ANMMAP.ToString());
                    config_main.SetValue("main", "enable_ecl_map", this.is_use_ECLMAP.ToString());
                    config_main.SetValue("main", "enable_std_map", this.is_use_STDMAP.ToString());
                    config_main.SetValue("main", "enable_msg_map", this.is_use_MSGMAP.ToString());
                    config_main.SetValue("form_set", "height", this.Height.ToString());
                    config_main.SetValue("form_set", "width", this.Width.ToString());
                    config_main.SetValue("form_set", "loc_x", this.Location.X.ToString());
                    config_main.SetValue("form_set", "loc_y", this.Location.Y.ToString());
                    config_main.SetValue("main", "thtk_version", ver_thtk);
                    config_main.SetValue("main", "game", now_game);
                    config_main.Save();
                    config_language.Save();
                    Save_game(now_game);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("保存失败," + ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void Lay_Form()
        {
            int wd = Width;
            int folder_X = wd - 40;
            int button_file_X = wd - 70;
            int txt_X = wd - 135;
            //dat
            Folder_thdat.Width = folder_X;
            btn_thdat_folder_file.Left = button_file_X;
            btn_thdat_archive_file.Left = button_file_X;
            txt_thdat_folder.Width = txt_X;
            txt_thdat_archive.Width = txt_X;
            btn_thdat_list_file.Left = button_file_X;
            txt_thdat_list.Width = txt_X;
            //anm
            Folder_thanm.Width = folder_X;
            btn_thanm_folder_file.Left = button_file_X;
            btn_thanm_archive_file.Left = button_file_X;
            btn_thanm_des_file.Left = button_file_X;
            btn_thanm_entry_file_file.Left = button_file_X;
            txt_thanm_folder.Width = txt_X;
            txt_thanm_archive.Width = txt_X;
            txt_thanm_des.Width = txt_X;
            cmb_entry.Width = txt_X;
            txt_thanm_entry_file.Width = txt_X;
            //ecl
            Folder_thecl.Width = folder_X;
            btn_thecl_file_file.Left = button_file_X;
            btn_thecl_archive_file.Left = button_file_X;
            txt_thecl_file.Width = txt_X;
            txt_thecl_archive.Width = txt_X;
            //msg
            Folder_thmsg.Width = folder_X;
            btn_thmsg_file_file.Left = button_file_X;
            btn_thmsg_archive_file.Left = button_file_X;
            txt_thmsg_file.Width = txt_X;
            txt_thmsg_archive.Width = txt_X;
            //std
            Folder_thstd.Width = folder_X;
            btn_thstd_file_file.Left = button_file_X;
            btn_thstd_archive_file.Left = button_file_X;
            txt_thstd_file.Width = txt_X;
            txt_thstd_archive.Width = txt_X;
        }
        private void Form_main_Resize(object sender, EventArgs e)
        {
            if (form_background != null)
            {
                Height = Width * form_background.Height / form_background.Width;
            }
            Lay_Form();
        }
        private void ANMMAPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (sender as ToolStripMenuItem).Checked = !(sender as ToolStripMenuItem).Checked;
            is_use_ANMMAP = (sender as ToolStripMenuItem).Checked;
        }
        private void ECLMAPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (sender as ToolStripMenuItem).Checked = !(sender as ToolStripMenuItem).Checked;
            is_use_ECLMAP = (sender as ToolStripMenuItem).Checked;
        }
        private void MSGMAPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (sender as ToolStripMenuItem).Checked = !(sender as ToolStripMenuItem).Checked;
            is_use_MSGMAP = (sender as ToolStripMenuItem).Checked;
        }
        private void STDMAPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (sender as ToolStripMenuItem).Checked = !(sender as ToolStripMenuItem).Checked;
            is_use_STDMAP = (sender as ToolStripMenuItem).Checked;
        }
        //ecl
        private void Btn_thecl_file_file_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog { Filter = ecl_suffix_file };
            file.ShowDialog();
            if (file.FileName == "")
                return;
            txt_thecl_file.Text = file.FileName;
        }
        private void Btn_thecl_archive_file_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog { Filter = "ecl|*.ecl|allFile|*.*" };
            try
            {
                string path = txt_thecl_archive.Text;
                if (File.Exists(path))
                    if (Directory.Exists(System.IO.Path.GetDirectoryName(path)))
                        file.InitialDirectory = System.IO.Path.GetDirectoryName(path);
                if (Directory.Exists(path))
                    file.InitialDirectory = path;
                file.ShowDialog();
                if (file.FileName == "")
                    return;
                txt_thecl_archive.Text = file.FileName;
                txt_thecl_file.Text =
             System.IO.Path.GetDirectoryName(file.FileName) + @"\ECL\" +
             System.IO.Path.GetFileNameWithoutExtension(file.FileName) + ecl_suffix;
            }
            catch { }

        }
        private void Btn_thecl_unpack_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(txt_thecl_file.Text)))//若文件夹不存在则新建文件夹   
                {
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(txt_thecl_file.Text)); //新建文件夹   
                }
                FileStream fs = new FileStream(txt_thecl_file.Text, FileMode.Create);
                fs.Close();
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("gameVer", now_game_enter);
                args.Add("aim", txt_thecl_file.Text);
                args.Add("source", txt_thecl_archive.Text);
                args.Add("this", System.IO.Directory.GetCurrentDirectory());
                string workDir = THTK_Commander.getWorkDir(config_command.GetValue(ver_thtk, "thecl_workDir", "this"), args);
                string cmd_uncsted = null;
                if (is_use_ECLMAP)
                {
                    args.Add("map", System.IO.Directory.GetCurrentDirectory() + @"\MAP\ECL_MAP_" + now_game_enter + ".txt");
                    cmd_uncsted = config_command.GetValue(ver_thtk, "thecl_unpack_map", "-1");
                }
                else
                    cmd_uncsted = config_command.GetValue(ver_thtk, "thecl_unpack", "-1");
                if (cmd_uncsted == "-1")
                {
                    MessageBox.Show("unknown command", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string cmd = THTK_Commander.CastCommand(
                cmd_uncsted, args);
                THTK_Commander.DoCommand(
                    System.IO.Directory.GetCurrentDirectory() + @"\thtk\" + ver_thtk + @"\thecl.exe", cmd, workDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void Btn_thecl_pack_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("gameVer", now_game_enter);
                args.Add("aim", txt_thecl_archive.Text);
                args.Add("source", txt_thecl_file.Text);
                args.Add("this", System.IO.Directory.GetCurrentDirectory());
                string workDir = THTK_Commander.getWorkDir(config_command.GetValue(ver_thtk, "thecl_workDir", "this"), args);
                string cmd_uncsted = null;
                if (is_use_ECLMAP)
                {
                    args.Add("map", System.IO.Directory.GetCurrentDirectory() + @"\MAP\ECL_MAP_" + now_game_enter + ".txt");
                    cmd_uncsted = config_command.GetValue(ver_thtk, "thecl_pack_map", "-1");
                }
                else
                    cmd_uncsted = config_command.GetValue(ver_thtk, "thecl_pack", "-1");
                if (cmd_uncsted == "-1")
                {
                    MessageBox.Show("unknown command", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string cmd = THTK_Commander.CastCommand(
                cmd_uncsted, args);
                THTK_Commander.DoCommand(
                    System.IO.Directory.GetCurrentDirectory() + @"\thtk\" + ver_thtk + @"\thecl.exe", cmd, workDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        //anm1
        private void Btn_thanm_unpack_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("gameVer", now_game_enter);
                args.Add("aim", txt_thanm_folder.Text);
                args.Add("source", txt_thanm_archive.Text);
                args.Add("des", txt_thanm_des.Text);
                args.Add("this", System.IO.Directory.GetCurrentDirectory());

                string workDir = THTK_Commander.getWorkDir(config_command.GetValue(ver_thtk, "thanm_workDir", "this"), args);
                string cmd_uncsted = null;
                if (is_use_ANMMAP)
                {
                    args.Add("map", System.IO.Directory.GetCurrentDirectory() + @"\MAP\ANM_MAP_" + now_game_enter + ".txt");
                    cmd_uncsted = config_command.GetValue(ver_thtk, "thanm_unpack_map", "-1");
                }
                else
                    cmd_uncsted = config_command.GetValue(ver_thtk, "thanm_unpack", "-1");
                if (cmd_uncsted == "-1")
                {
                    MessageBox.Show("unknown command", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string cmd = THTK_Commander.CastCommand(
                cmd_uncsted, args);
                THTK_Commander.DoCommand(
                    System.IO.Directory.GetCurrentDirectory() + @"\thtk\" + ver_thtk + @"\thanm.exe", cmd, workDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void Btn_thanm_archive_file_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog file = new OpenFileDialog { Filter = "anm|*.anm|allFile|*.*" };
                string path = txt_thanm_archive.Text;
                if (File.Exists(path))
                    if (Directory.Exists(System.IO.Path.GetDirectoryName(path)))
                        file.InitialDirectory = System.IO.Path.GetDirectoryName(path);
                if (Directory.Exists(path))
                    file.InitialDirectory = path;
                file.ShowDialog();
                if (file.FileName == "")
                    return;
                txt_thanm_archive.Text = file.FileName;
                txt_thanm_folder.Text =
               System.IO.Path.GetDirectoryName(file.FileName) + @"\ANM";
                txt_thanm_des.Text =
               System.IO.Path.GetDirectoryName(file.FileName) + @"\ANM\" +
               System.IO.Path.GetFileNameWithoutExtension(file.FileName) + anm_suffix;
            }
            catch { }
        }
        private void Btn_thanm_folder_file_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog file = new FolderBrowserDialog();
            file.ShowDialog(this);
            if (file.DirectoryPath == "" || file.DirectoryPath == null)
                return;
            txt_thanm_folder.Text = file.DirectoryPath;
        }
        private void Btn_thanm_getDes_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("gameVer", now_game_enter);
                args.Add("aim", txt_thanm_folder.Text);
                args.Add("source", txt_thanm_archive.Text);
                args.Add("des", txt_thanm_des.Text);
                args.Add("this", System.IO.Directory.GetCurrentDirectory());
                string workDir = THTK_Commander.getWorkDir(config_command.GetValue(ver_thtk, "thanm_workDir", "this"), args);
                string cmd_uncsted = null;
                if (is_use_ANMMAP)
                {
                    args.Add("map", System.IO.Directory.GetCurrentDirectory() + @"\MAP\ANM_MAP_" + now_game_enter + ".txt");
                    cmd_uncsted = config_command.GetValue(ver_thtk, "thanm_des_map", "-1");
                }
                else
                    cmd_uncsted = config_command.GetValue(ver_thtk, "thanm_des", "-1");
                if (cmd_uncsted == "-1")
                {
                    MessageBox.Show("unknown command", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string cmd = THTK_Commander.CastCommand(
                cmd_uncsted, args);
                string desFile = THTK_Commander.DoCommand(
                    System.IO.Directory.GetCurrentDirectory() + @"\thtk\" + ver_thtk + @"\thanm.exe", cmd, workDir);
                StreamWriter fs = new StreamWriter(txt_thanm_des.Text);
                fs.Write(desFile);
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void Btn_thanm_pack_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("gameVer", now_game_enter);
                args.Add("aim", txt_thanm_folder.Text);
                args.Add("source", txt_thanm_archive.Text);
                args.Add("des", txt_thanm_des.Text);
                args.Add("this", System.IO.Directory.GetCurrentDirectory());

                string workDir = THTK_Commander.getWorkDir(config_command.GetValue(ver_thtk, "thanm_workDir", "this"), args);
                string cmd_uncsted = null;
                if (is_use_ANMMAP)
                {
                    args.Add("map", System.IO.Directory.GetCurrentDirectory() + @"\MAP\ANM_MAP_" + now_game_enter + ".txt");
                    cmd_uncsted = config_command.GetValue(ver_thtk, "thanm_pack_map", "-1");
                }
                else
                    cmd_uncsted = config_command.GetValue(ver_thtk, "thanm_pack", "-1");
                if (cmd_uncsted == "-1")
                {
                    MessageBox.Show("unknown command", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string cmd = THTK_Commander.CastCommand(
                cmd_uncsted, args);
                THTK_Commander.DoCommand(
                    System.IO.Directory.GetCurrentDirectory() + @"\thtk\" + ver_thtk + @"\thanm.exe", cmd, workDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        //msg
        private void Btn_thmsg_file_file_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog { Filter = msg_suffix_file };
            file.ShowDialog();
            if (file.FileName == "")
                return;
            txt_thmsg_file.Text = file.FileName;
        }
        private void Btn_thmsg_archive_file_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog { Filter = "msg|*.msg|allFile|*.*" };
            try
            {
                string path = txt_thmsg_archive.Text;
                if (File.Exists(path))
                    if (Directory.Exists(System.IO.Path.GetDirectoryName(path)))
                        file.InitialDirectory = System.IO.Path.GetDirectoryName(path);
                if (Directory.Exists(path))
                    file.InitialDirectory = path;
                file.ShowDialog();
                if (file.FileName == "")
                    return;
                txt_thmsg_archive.Text = file.FileName;
                txt_thmsg_file.Text =
             System.IO.Path.GetDirectoryName(file.FileName) + @"\MSG\" +
             System.IO.Path.GetFileNameWithoutExtension(file.FileName) + msg_suffix;
            }
            catch { }
        }
        private void Btn_thmsg_unpack_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(txt_thmsg_file.Text)))//若文件夹不存在则新建文件夹   
                {
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(txt_thmsg_file.Text)); //新建文件夹   
                }
                FileStream fs = new FileStream(txt_thmsg_file.Text, FileMode.Create);
                fs.Close();
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("gameVer", now_game_enter);
                args.Add("aim", txt_thmsg_file.Text);
                args.Add("source", txt_thmsg_archive.Text);
                args.Add("this", System.IO.Directory.GetCurrentDirectory());

                string workDir = THTK_Commander.getWorkDir(config_command.GetValue(ver_thtk, "thmsg_workDir", "this"), args);
                string cmd_uncsted = null;
                if (is_use_MSGMAP)
                {
                    args.Add("map", System.IO.Directory.GetCurrentDirectory() + @"\MAP\MSG_MAP_" + now_game_enter + ".txt");
                    if(cbox_ed.Checked)
                    {
                        cmd_uncsted = config_command.GetValue(ver_thtk, "thmsg_unpack_map_ed", "-1");
                    }
                    else
                    {
                        cmd_uncsted = config_command.GetValue(ver_thtk, "thmsg_unpack_map", "-1");
                    }
                }
                else
                {
                    if (cbox_ed.Checked)
                    {
                        cmd_uncsted = config_command.GetValue(ver_thtk, "thmsg_unpack_ed", "-1");
                    }
                    else
                    {
                        cmd_uncsted = config_command.GetValue(ver_thtk, "thmsg_unpack", "-1");
                    }
                }
                if (cmd_uncsted == "-1")
                {
                    MessageBox.Show("unknown command", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string cmd = THTK_Commander.CastCommand(
                cmd_uncsted, args);
                THTK_Commander.DoCommand(
                    System.IO.Directory.GetCurrentDirectory() + @"\thtk\" + ver_thtk + @"\thmsg.exe", cmd, workDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void Btn_thmsg_pack_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("gameVer", now_game_enter);
                args.Add("aim", txt_thmsg_archive.Text);
                args.Add("source", txt_thmsg_file.Text);
                args.Add("this", System.IO.Directory.GetCurrentDirectory());

                string workDir = THTK_Commander.getWorkDir(config_command.GetValue(ver_thtk, "thmsg_workDir", "this"), args);
                string cmd_uncsted = null;
                if (is_use_MSGMAP)
                {

                    args.Add("map", System.IO.Directory.GetCurrentDirectory() + @"\MAP\MSG_MAP_" + now_game_enter + ".txt");
                    if (cbox_ed.Checked)
                    {
                        cmd_uncsted = config_command.GetValue(ver_thtk, "thmsg_pack_map_ed", "-1");
                    }
                    else
                    {
                        cmd_uncsted = config_command.GetValue(ver_thtk, "thmsg_pack_map", "-1");
                    }
                }
                else
                {
                    if (cbox_ed.Checked)
                    {
                        cmd_uncsted = config_command.GetValue(ver_thtk, "thmsg_pack_ed", "-1");
                    }
                    else
                    {
                        cmd_uncsted = config_command.GetValue(ver_thtk, "thmsg_pack", "-1");
                    }
                }
                if (cmd_uncsted == "-1")
                {
                    MessageBox.Show("unknown command", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string cmd = THTK_Commander.CastCommand(
                cmd_uncsted, args);
                THTK_Commander.DoCommand(
                    System.IO.Directory.GetCurrentDirectory() + @"\thtk\" + ver_thtk + @"\thmsg.exe", cmd, workDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        //std
        private void Btn_thstd_file_file_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog { Filter = std_suffix_file };
            file.ShowDialog();
            if (file.FileName == "")
                return;
            txt_thstd_file.Text = file.FileName;
        }
        private void Btn_thstd_archive_file_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog { Filter = "std|*.std|allFile|*.*" };
            try
            {
                string path = txt_thstd_archive.Text;
                if (File.Exists(path))
                    if (Directory.Exists(System.IO.Path.GetDirectoryName(path)))
                        file.InitialDirectory = System.IO.Path.GetDirectoryName(path);
                if (Directory.Exists(path))
                    file.InitialDirectory = path;
                file.ShowDialog();
                if (file.FileName == "")
                    return;
                txt_thstd_archive.Text = file.FileName;
                txt_thstd_file.Text =
             System.IO.Path.GetDirectoryName(file.FileName) + @"\STD\" +
             System.IO.Path.GetFileNameWithoutExtension(file.FileName) + std_suffix;
            }
            catch { }
        }
        private void Btn_thstd_unpack_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(txt_thstd_file.Text)))//若文件夹不存在则新建文件夹   
                {
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(txt_thstd_file.Text)); //新建文件夹   
                }
                FileStream fs = new FileStream(txt_thstd_file.Text, FileMode.Create);
                fs.Close();
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("gameVer", now_game_enter);
                args.Add("aim", txt_thstd_file.Text);
                args.Add("source", txt_thstd_archive.Text);
                args.Add("this", System.IO.Directory.GetCurrentDirectory());

                string workDir = THTK_Commander.getWorkDir(config_command.GetValue(ver_thtk, "thstd_workDir", "this"), args);
                string cmd_uncsted = null;
                if (is_use_STDMAP)
                {
                    args.Add("map", System.IO.Directory.GetCurrentDirectory() + @"\MAP\STD_MAP_" + now_game_enter + ".txt");
                    cmd_uncsted = config_command.GetValue(ver_thtk, "thstd_unpack_map", "-1");
                }
                else
                    cmd_uncsted = config_command.GetValue(ver_thtk, "thstd_unpack", "-1");
                if (cmd_uncsted == "-1")
                {
                    MessageBox.Show("unknown command", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string cmd = THTK_Commander.CastCommand(
                cmd_uncsted, args);
                THTK_Commander.DoCommand(
                    System.IO.Directory.GetCurrentDirectory() + @"\thtk\" + ver_thtk + @"\thstd.exe", cmd, workDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void Btn_thstd_pack_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("gameVer", now_game_enter);
                args.Add("aim", txt_thstd_archive.Text);
                args.Add("source", txt_thstd_file.Text);
                args.Add("this", System.IO.Directory.GetCurrentDirectory());

                string workDir = THTK_Commander.getWorkDir(config_command.GetValue(ver_thtk, "thstd_workDir", "this"), args);
                string cmd_uncsted = null;
                if (is_use_STDMAP)
                {
                    args.Add("map", System.IO.Directory.GetCurrentDirectory() + @"\MAP\STD_MAP_" + now_game_enter + ".txt");
                    cmd_uncsted = config_command.GetValue(ver_thtk, "thstd_pack_map", "-1");
                }
                else
                    cmd_uncsted = config_command.GetValue(ver_thtk, "thstd_pack", "-1");
                if (cmd_uncsted == "-1")
                {
                    MessageBox.Show("unknown command", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string cmd = THTK_Commander.CastCommand(
                cmd_uncsted, args);
                THTK_Commander.DoCommand(
                    System.IO.Directory.GetCurrentDirectory() + @"\thtk\" + ver_thtk + @"\thstd.exe", cmd, workDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        //dat
        private void Btn_thdat_folder_file_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog file = new FolderBrowserDialog();
            file.ShowDialog(this);
            if (file.DirectoryPath == "" || file.DirectoryPath == null)
                return;
            txt_thdat_folder.Text = file.DirectoryPath;
        }
        private void Btn_thdat_archive_file_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog file = new OpenFileDialog { Filter = "dat|*.dat|allFile|*.*" };
                string path = txt_thdat_archive.Text;
                if (File.Exists(path))
                    if (Directory.Exists(System.IO.Path.GetDirectoryName(path)))
                        file.InitialDirectory = System.IO.Path.GetDirectoryName(path);
                if (Directory.Exists(path))
                    file.InitialDirectory = path;
                file.ShowDialog();
                if (file.FileName == "")
                    return;
                txt_thdat_archive.Text = file.FileName;

                txt_thdat_folder.Text =
                System.IO.Path.GetDirectoryName(file.FileName) + @"\data";
                txt_thdat_list.Text =
                System.IO.Path.GetDirectoryName(file.FileName) + @"\data\" +
                System.IO.Path.GetFileNameWithoutExtension(file.FileName) + "_list.txt";
                txt_thanm_archive.Text =
                System.IO.Path.GetDirectoryName(file.FileName) + @"\data\";
                txt_thecl_archive.Text =
                System.IO.Path.GetDirectoryName(file.FileName) + @"\data\";
                txt_thstd_archive.Text =
                System.IO.Path.GetDirectoryName(file.FileName) + @"\data\";
                txt_thmsg_archive.Text =
                System.IO.Path.GetDirectoryName(file.FileName) + @"\data\";
            }
            catch { }
        }
        private void Btn_thdat_list_file_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog { Filter = "txt|*.txt|allFile|*.*" };
            file.ShowDialog();
            if (file.FileName == "")
                return;
            txt_thdat_list.Text = file.FileName;
        }
        private void Btn_thdat_unpack_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("gameVer", now_game_enter);
                args.Add("aim", txt_thdat_folder.Text);
                args.Add("source", txt_thdat_archive.Text);
                args.Add("list", txt_thdat_list.Text);
                args.Add("this", System.IO.Directory.GetCurrentDirectory());

                string workDir = THTK_Commander.getWorkDir(config_command.GetValue(ver_thtk, "thdat_workDir", "this"), args);
                string cmd_uncsted = null;
                cmd_uncsted = config_command.GetValue(ver_thtk, "thdat_unpack", "-1");
                if (cmd_uncsted == "-1")
                {
                    MessageBox.Show("unknown command", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string cmd = THTK_Commander.CastCommand(
                cmd_uncsted, args);
                THTK_Commander.DoCommand(
                    System.IO.Directory.GetCurrentDirectory() + @"\thtk\" + ver_thtk + @"\thdat.exe", cmd, workDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void Btn_thdat_getList_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> args = new Dictionary<string, string>();
                args.Add("gameVer", now_game_enter);
                args.Add("aim", txt_thdat_folder.Text);
                args.Add("source", txt_thdat_archive.Text);
                args.Add("list", txt_thdat_list.Text);
                args.Add("this", System.IO.Directory.GetCurrentDirectory());

                string workDir = THTK_Commander.getWorkDir(config_command.GetValue(ver_thtk, "thdat_workDir", "this"), args);
                string cmd_uncsted = null;
                cmd_uncsted = config_command.GetValue(ver_thtk, "thdat_list", "-1");
                if (cmd_uncsted == "-1")
                {
                    MessageBox.Show("unknown command", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string cmd = THTK_Commander.CastCommand(
                cmd_uncsted, args);
                string desFile = THTK_Commander.DoCommand(
                    System.IO.Directory.GetCurrentDirectory() + @"\thtk\" + ver_thtk + @"\thdat.exe", cmd, workDir);
                StreamWriter fs = new StreamWriter(txt_thdat_list.Text);
                fs.Write(desFile);
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void Btn_thdat_pack_Click(object sender, EventArgs e)
        {
            try
            {
                //string names="";

                string line;
                int jmp_line = Convert.ToInt32(config_command.GetValue(ver_thtk, "thdat_allFile_jmpLine", "1"));
                int token_num = Convert.ToInt32(config_command.GetValue(ver_thtk, "thdat_allFile_token", "0"));
                Dictionary<string, string> args = new Dictionary<string, string>
                {
                    { "gameVer", now_game_enter },
                    { "aim", txt_thdat_folder.Text },
                    { "source", txt_thdat_archive.Text },
                    { "list", txt_thdat_list.Text },
                    { "this", System.IO.Directory.GetCurrentDirectory() }
                };
                string workDir = THTK_Commander.getWorkDir(config_command.GetValue(ver_thtk, "thanm_workDir", "this"), args);
                StreamReader fs = new StreamReader(txt_thdat_list.Text);
                for (int i = 0; i < jmp_line; i++)
                    line = fs.ReadLine();
                string names_path = "";
                while ((line = fs.ReadLine()) != null)
                {
                    //names = names + "\"" + (line.Split(' ')[token_num]) + "\" ";
                    names_path = names_path + "\"" + workDir + @"\" + (line.Split(' ')[token_num]) + "\" ";
                }
                fs.Close();
                //args.Add("allFile", names);
                args.Add("allFile_path", names_path);
                string cmd_uncsted = null;
                cmd_uncsted = config_command.GetValue(ver_thtk, "thdat_pack", "-1");
                if (cmd_uncsted == "-1")
                {
                    MessageBox.Show("unknown command", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string cmd = THTK_Commander.CastCommand(
                cmd_uncsted, args);
                THTK_Commander.DoCommand(
                    System.IO.Directory.GetCurrentDirectory() + @"\thtk\" + ver_thtk + @"\thdat.exe", cmd, workDir);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void Btn_thanm_entry_file_file_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog { Filter = "png|*.png|allFile|*.*" };
            string path = txt_thanm_entry_file.Text;
            if (File.Exists(path))
                if (Directory.Exists(System.IO.Path.GetDirectoryName(path)))
                    file.InitialDirectory = System.IO.Path.GetDirectoryName(path);
            file.ShowDialog();
            if (file.FileName == "")
                return;
            txt_thanm_entry_file.Text = file.FileName;
        }
        private void Btn_thanm_rep_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, string> args = new Dictionary<string, string>
                {
                    { "gameVer", now_game_enter },
                    { "aim", txt_thanm_folder.Text },
                    { "source", txt_thanm_archive.Text },
                    { "des", txt_thanm_des.Text },
                    { "entry",cmb_entry.Text },
                    { "file", txt_thanm_entry_file.Text },
                    { "this", System.IO.Directory.GetCurrentDirectory() }
                };
                string workDir = THTK_Commander.getWorkDir(config_command.GetValue(ver_thtk, "thanm_workDir", "this"), args);
                string cmd_uncsted = null;
                cmd_uncsted = config_command.GetValue(ver_thtk, "thanm_rep", "-1");
                if (cmd_uncsted == "-1")
                {
                    MessageBox.Show("unknown command", "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string cmd = THTK_Commander.CastCommand(
                cmd_uncsted, args);
                THTK_Commander.DoCommand(
                    System.IO.Directory.GetCurrentDirectory() + @"\thtk\" + ver_thtk + @"\thanm.exe", cmd, workDir);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "From thtk_gui", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }
        private void Btn_thanm_des_file_Click(object sender, EventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog { Filter = anm_suffix_file };
            file.ShowDialog();
            if (file.FileName == "")
                return;
            txt_thanm_des.Text = file.FileName;
        }

        private void HelpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Help hp = new Help();
            hp.Show();
        }
        private void Cmb_entry_Click(object sender, EventArgs e)
        {
            (sender as ComboBox).Items.Clear();
            try
            {
                StreamReader fs = new StreamReader(txt_thanm_des.Text);
                string ln = fs.ReadLine();
                string flag = config_command.GetValue(ver_thtk, "thanm_entry_flag", "Name:");
                while (ln != null)
                {
                    int ind = ln.IndexOf(flag);
                    if (ind >= 0)
                    {
                        (sender as ComboBox).Items.Add(ln.Substring(ind + flag.Length).Trim(' '));
                    }
                    ln = fs.ReadLine();
                }
                fs.Close();
            }
            catch(Exception ex)
            {

            }
        }
     }
}

