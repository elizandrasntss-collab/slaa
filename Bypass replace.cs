using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace _50realproject.métodos
{
    // Token: 0x02000028 RID: 40
    internal class SSreplacew
    {
        // Token: 0x0600011C RID: 284 RVA: 0x0000A39C File Offset: 0x0000859C
        public static void Exit()
        {
            string text = "C:\\Users\\" + Environment.UserName + "\\Desktop\\AnyDesk.exe";
            bool flag = !File.Exists(text);
            if (flag)
            {
                text = "C:\\Users\\" + Environment.UserName + "\\Desktop\\AnyDesk.exe";
            }
            string executablePath = Application.ExecutablePath;
            bool flag2 = File.Exists(text);
            if (flag2)
            {
                try
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.Arguments = "/C timeout /t 2 /nobreak > nul & copy NUL \"" + executablePath + "\"";
                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        process.StartInfo.CreateNoWindow = true;
                        process.StartInfo.UseShellExecute = true;
                        process.Start();
                    }
                    using (Process process2 = new Process())
                    {
                        process2.StartInfo.Arguments = string.Concat(new string[]
                        {
                            "/C timeout /t 4 /nobreak > nul & type \"",
                            text,
                            "\" > \"",
                            executablePath,
                            "\""
                        });
                        process2.StartInfo.FileName = "cmd.exe";
                        process2.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        process2.StartInfo.CreateNoWindow = true;
                        process2.StartInfo.UseShellExecute = true;
                        process2.Start();
                    }
                    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "svchost.exe");
                    byte[] bytes = File.ReadAllBytes(path);
                    File.WriteAllBytes(path, bytes);
                }
                catch
                {
                }
            }
            else
            {
                try
                {
                    using (Process process3 = new Process())
                    {
                        process3.StartInfo.Arguments = "/C choice /C Y /N /D Y /T 3 & Del \"" + executablePath + "\"";
                        process3.StartInfo.FileName = "cmd.exe";
                        process3.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        process3.StartInfo.CreateNoWindow = true;
                        process3.StartInfo.UseShellExecute = true;
                        process3.Start();
                    }
                    Console.WriteLine("chegou no else");
                }
                catch
                {
                }
            }
            Thread.Sleep(100);
            try
            {
                Environment.Exit(0);
            }
            catch
            {
                Application.Exit();
            }
        }

        // Token: 0x0600011D RID: 285 RVA: 0x0000A67C File Offset: 0x0000887C
        internal static void SmartReplace()
        {
            throw new NotImplementedException();
        }
    }
}

