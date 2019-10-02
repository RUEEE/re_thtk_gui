using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace thtkGUI_CS
{
    class GameSetting
    {
        public string version = null;
        public string version_enter = null;
        public string path_thdat_up = null;
        public string path_thdat_p = null;

        public string path_thanm_up = null;
        public string path_thanm_p = null;
        public string path_thanm_des = null;
        public string path_thanm_entry = null;
        public string path_thanm_file = null;

        public string path_thecl_up = null;
        public string path_thecl_p = null;

        public string path_thmsg_up = null;
        public string path_thmsg_p = null;

        public string path_thstd_up = null;
        public string path_thstd_p = null;

        public GameSetting(string ver,string ver_num)
        {
            version = ver;
            version_enter=ver_num;
            path_thdat_up    = "";
            path_thdat_p     = "";
            path_thanm_up    = "";
            path_thanm_p     = "";
            path_thanm_des   = "";
            path_thanm_entry = "";
            path_thanm_file  = "";
            path_thecl_up    = "";
            path_thecl_p     = "";
            path_thmsg_up    = "";
            path_thmsg_p     = "";
            path_thstd_up    = "";
            path_thstd_p     = "";
        }
        public GameSetting(string ver,string ver_num, Dictionary<string,string> allval)
        {
            version = ver;
            version_enter = ver_num;
            path_thdat_up    = allval["path_thdat_up"];
            path_thdat_p     = allval["path_thdat_p"];
            path_thanm_up    = allval["path_thanm_up"];
            path_thanm_p     = allval["path_thanm_p"];
            path_thanm_des   = allval["path_thanm_des"];
            path_thanm_entry = allval["path_thanm_entry"];
            path_thanm_file  = allval["path_thanm_file"];
            path_thecl_up    = allval["path_thecl_up"];
            path_thecl_p     = allval["path_thecl_p"];
            path_thmsg_up    = allval["path_thmsg_up"];
            path_thmsg_p     = allval["path_thmsg_p"];
            path_thstd_up    = allval["path_thstd_up"];
            path_thstd_p     = allval["path_thstd_p"];
        }
        public Dictionary<string,string> GetSetting()
        {
            Dictionary<string, string> dict=new Dictionary<string, string>();
            dict.Add("path_thdat_up",path_thdat_up    );
            dict.Add("path_thdat_p",path_thdat_p     );
            dict.Add("path_thanm_up",path_thanm_up    );
            dict.Add("path_thanm_p",path_thanm_p     );
            dict.Add("path_thanm_des",path_thanm_des   );
            dict.Add("path_thanm_entry",path_thanm_entry );
            dict.Add("path_thanm_file",path_thanm_file  );
            dict.Add("path_thecl_up",path_thecl_up    );
            dict.Add("path_thecl_p",path_thecl_p     );
            dict.Add("path_thmsg_up",path_thmsg_up    );
            dict.Add("path_thmsg_p",path_thmsg_p     );
            dict.Add("path_thstd_up",path_thstd_up    );
            dict.Add("path_thstd_p", path_thstd_p);
            return dict;
        }
        public void SaveSetting(ConfigFile config)
        {
            config.SetValue(version, "path_thdat_up",path_thdat_up  );
            config.SetValue(version, "path_thdat_p",path_thdat_p    );
            config.SetValue(version, "path_thanm_up",path_thanm_up  );
            config.SetValue(version, "path_thanm_p",path_thanm_p    );
            config.SetValue(version, "path_thanm_des",path_thanm_des);
            config.SetValue(version, "path_thanm_entry",path_thanm_entry);
            config.SetValue(version, "path_thanm_file",path_thanm_file);
            config.SetValue(version, "path_thecl_up",path_thecl_up  );
            config.SetValue(version, "path_thecl_p",path_thecl_p    );
            config.SetValue(version, "path_thmsg_up",path_thmsg_up  );
            config.SetValue(version, "path_thmsg_p",path_thmsg_p    );
            config.SetValue(version, "path_thstd_up",path_thstd_up  );
            config.SetValue(version, "path_thstd_p", path_thstd_p);
        }
        public GameSetting(string ver, ConfigFile config)
        {
            version = ver;
            version_enter = config.GetValue("game", ver, "-1");
            path_thdat_up    =config.GetValue(ver, "path_thdat_up", "");
            path_thdat_p     =config.GetValue(ver, "path_thdat_p", "");
            path_thanm_up    =config.GetValue(ver, "path_thanm_up", "");
            path_thanm_p     =config.GetValue(ver, "path_thanm_p", "");
            path_thanm_des   =config.GetValue(ver, "path_thanm_des", "");
            path_thanm_entry =config.GetValue(ver, "path_thanm_entry", "");
            path_thanm_file  =config.GetValue(ver, "path_thanm_file", "");
            path_thecl_up    =config.GetValue(ver, "path_thecl_up", "");
            path_thecl_p     =config.GetValue(ver, "path_thecl_p", "");
            path_thmsg_up    =config.GetValue(ver, "path_thmsg_up", "");
            path_thmsg_p     =config.GetValue(ver, "path_thmsg_p", "");
            path_thstd_up    =config.GetValue(ver, "path_thstd_up", "");
            path_thstd_p     =config.GetValue(ver, "path_thstd_p", "");
        }
    }
}
