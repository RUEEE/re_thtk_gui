using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;
namespace thtkGUI_CS
{
    class ReadConfig
    {
        /*[DllImport("kernel32", CharSet = CharSet.Unicode)]
        public static extern int GetPrivateProfileString(string section, string key, string defVal, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32", CharSet = CharSet.Unicode)]*/
        [DllImport("kernel32")] public static extern bool WritePrivateProfileString(byte[] section, byte[] key, byte[] val, string filePath);
        [DllImport("kernel32")] public static extern int GetPrivateProfileString(byte[] section, byte[] key, byte[] def, byte[] retVal, int size, string filePath);
        
    }
    public class ConfigFile
    {
        public bool Save()
        {
            try
            {
                StreamWriter fs = new StreamWriter(file_path);
                foreach (var iter in Map)
                {
                    fs.Write($"[{iter.Key}]\n");
                    foreach (var it2 in iter.Value)
                    {
                        fs.Write($"{it2.Key}={it2.Value}\n");
                    }
                    fs.Write("\n");
                }
                fs.Flush();
                fs.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
        private string file_path;
        private string File_path { get=>file_path; set=>file_path=value; }
        private StreamReader file;
        private StreamReader File { get => file; set => file = value; }
        private Dictionary<string, Dictionary<string, string>> map;
        private Dictionary<string, Dictionary<string, string>> Map { get => map; set => map = value; }
        public Dictionary<string,string> GetKeys(string Sec)
        {
            if (Map.ContainsKey(Sec))
            {
                return Map[Sec];
            }
            return null;
        }
        public bool Is_have_key(string Sec,string Key)
        {
            if (Map.ContainsKey(Sec))
            {
                if (Map[Sec].ContainsKey(Key))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public string GetValue(string Sec,string Key,string defaultStr="null")
        {
            if (Map.ContainsKey(Sec))
            {
                if (Map[Sec].ContainsKey(Key))
                {
                    return Map[Sec][Key];
                }
                else
                {
                    return defaultStr;
                }
            }
            return defaultStr;
        }
        public bool SetValue(string sec, string key, string val)
        {
            if (Map.ContainsKey(sec))
            {
                if (Map[sec].ContainsKey(key))
                {
                    Map[sec][key] = val;
                    return true;
                }else{
                    Map[sec].Add(key, val);
                    return false;
                }
            }
            else
            {
                Map.Add(sec,new Dictionary<string, string>());
                Map[sec].Add(key, val);
                return false;
            }
        }
        public List<string> GetSections()
        {
            return map.Keys.ToList();
        }
        public ConfigFile(string file_open)
        {
            File_path = file_open;
            File = new StreamReader(file_open);
            Map = new Dictionary<string, Dictionary<string, string>>();
            string line = File.ReadLine();
            string nowSec = null;
            Regex reg_sec = new Regex(@"^\[(\S+)\]$");
            Regex reg_key_val = new Regex(@"^([^=]+)\=([^=]*)$");
            Regex reg_space = new Regex(@"^\s*$");
            Regex reg_comment = new Regex(@"^#");
            Match match;
            while (line!=null)
            {
                match = reg_comment.Match(line);
                if (match.Success)
                {
                    line = File.ReadLine();
                    continue;
                } 
                match = reg_sec.Match(line);
                if (match.Success)
                {//找到小节
                    nowSec = match.Result("$1");
                    try
                    {
                        Map.Add(nowSec, new Dictionary<string, string>());
                    }
                    catch (ArgumentException)
                    {
                        throw new Exception("配置文件中存在多个相同节");
                    }
                }
                else
                {//找到键值
                    match = reg_key_val.Match(line);
                    if (match.Success)
                    {
                        string val, key;
                        if (nowSec == null)
                        {
                            throw new Exception("在节之前发现键值");
                        }
                        val = match.Result("$1");
                        key = match.Result("$2");
                        try
                        {
                            Map[nowSec].Add(val, key);
                        }
                        catch (ArgumentException)
                        {
                            throw new Exception("配置文件中存在多个相同键");
                        }
                    }
                    else
                    {//是注释
                        match = reg_space.Match(line);
                        if (!match.Success)
                        {
                            throw new Exception("配置文件中存在非空行非键值");
                        }
                    }
                }
                line = File.ReadLine();
            }
            file.Close();
        }
    }
}
