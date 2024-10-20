using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace thtkGUI_CS
{
    class THTK_Commander
    {
        private static string AddArg(string former,bool is_not_add_space,string latter)
        {
            if(is_not_add_space)
            {
                return former + latter;
            }else{
                return former + " " + latter;
            }
        }
        public static string CastCommand(string cmd, Dictionary<string, string> def)
        {
            string ret = "";
            string[] str = cmd.Split(' ');
            bool is_not_add_space = false;
            foreach (string token in str)
            {
                string s = token;
                if (s.Length==0) continue;
                is_not_add_space = false;
                if (s[0]=='$')
                {
                    is_not_add_space = true;
                    s = s.Remove(0, 1);
                    if (s.Length == 0) continue;
                }
                if (s[0] == '@')
                {
                    if (s.Length == 1)
                    {
                        ret=AddArg(ret, is_not_add_space, s);
                        continue;
                    }
                    if (s[1] == '@')
                        if (def.ContainsKey(s.Remove(0, 2)))
                            ret = AddArg(ret, is_not_add_space, "\"" + def[s.Remove(0, 2)] + "\"");
                        else
                            ret = AddArg(ret, is_not_add_space, s);
                    else
                        if (def.ContainsKey(s.Remove(0, 1)))
                        ret = AddArg(ret, is_not_add_space, def[s.Remove(0, 1)]);
                    else
                        ret = AddArg(ret, is_not_add_space, s);
                }
                else
                {
                    ret = ret + " " + s;
                }
            }
            return ret;
        }
        public delegate void LogFunc(string str);
        public static string DoCommand(string program_path,string command, string workingDc,LogFunc log)
        {
            string opt2 = "";
            string output = "";
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = program_path;
            p.StartInfo.Arguments = command;
            p.StartInfo.UseShellExecute = false;        
            p.StartInfo.RedirectStandardInput = true;   
            p.StartInfo.RedirectStandardOutput = true;  
            p.StartInfo.RedirectStandardError = true;
            if(workingDc!=null && workingDc!="")
            {
                p.StartInfo.WorkingDirectory = workingDc;
            }
            if (!Directory.Exists(workingDc))
            {
                Directory.CreateDirectory(workingDc);
            }
            p.Start();
            opt2 = p.StandardOutput.ReadToEnd();
            output = p.StandardError.ReadToEnd();

            if (output!=null && output!="")
                log(output);
            p.WaitForExit();
            p.Close();
            return opt2;
        }
        public static string getWorkDir(string workDir,Dictionary<string,string> def)
        {
            if (workDir.Length == 0)
                return workDir;
            if(workDir[0]=='@')
            {
                if (workDir.Length == 1)
                    return workDir;
                if(workDir[1]=='@')
                {
                    if (def.ContainsKey(workDir.Remove(0, 2)))
                        return "\"" + def[workDir.Remove(0, 2)] + "\"";
                    else
                        return workDir;
                }
                if (def.ContainsKey(workDir.Remove(0, 1)))
                    return def[workDir.Remove(0, 1)];
                else
                    return workDir;
            }
            return workDir;
        }
    }
}
