using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cortex_Bypass.Codes
{
    internal class Cpd
    {
        public static void ClearRegexString()
        {
            string system32path = @"C:\Documents";
            string dllPattern = @"\d{10}\.dll";

            string[] system32files = Directory.GetFiles(system32path);
            foreach (string file in system32files)
            {
                string fileName = Path.GetFileName(file);

                if (Regex.IsMatch(fileName, dllPattern))
                {
                    try
                    {
                        Console.WriteLine("Excluindo arquivo: " + fileName);
                        FileOperations.DeleteFile(file);
                    }
                    catch
                    {
                        MessageBox.Show("Ocorreu um erro: N°CE4", "Erro", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    }

                }
            }
        }
        public static void Prefetch(string folderPath, string prefix)
        {
            if (!Directory.Exists(folderPath)) return;
            string[] files = Directory.GetFiles(folderPath, $"{prefix}*");
            foreach (string file in files)
            {
                try
                {
                    FileOperations.DeleteFile(file);
                }
                catch { }
            }
        }
        public static void DeleteAll()
        {
            string prefetchFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\Prefetch";
            Prefetch(prefetchFolderPath, "cmd.exe");
            Prefetch(prefetchFolderPath, "TIMEOUT");
            Prefetch(prefetchFolderPath, "CONHOST");
            Prefetch(prefetchFolderPath, "FSUTIL");
            Prefetch(prefetchFolderPath, "MKDIR");
            Prefetch(prefetchFolderPath, "ATTRIB");
            Prefetch(prefetchFolderPath, "WINDOWSUPDATE");
            Prefetch(prefetchFolderPath, "MICROSOFTEDGE");
            Prefetch(prefetchFolderPath, "WindowsUpdate.20240222.213902.838.1444");
            Prefetch(prefetchFolderPath, "MicrosoftEdge_X64_121.0.2277.106_121.0.2277.98");
            Prefetch($"C:\\Users\\{Environment.UserName}\\AppData\\Local\\CrashDumps", "microsoft");
            Prefetch($"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Microsoft\\CLR_v4.0\\UsageLogs", "microsoft");
            Prefetch($"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Microsoft\\CLR_v4.0_32\\UsageLogs", "microsoft");
            Prefetch($"C:\\Users\\{Environment.UserName}\\AppData\\Local\\CrashDumps", "notepad");
            Prefetch($"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Microsoft\\CLR_v4.0\\UsageLogs", "notepad");
            Prefetch($"C:\\Users\\{Environment.UserName}\\AppData\\Local\\Microsoft\\CLR_v4.0_32\\UsageLogs", "notepad");
        }
    }
}
