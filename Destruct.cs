using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

public static class FileOperations
{
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, int dwFlags);

    private const int MOVEFILE_REPLACE_EXISTING = 1;
    private const int MOVEFILE_WRITE_THROUGH = 8;

    public static void VisionxClean()
    {
        string filePath = @"C:\Documents\Computecore.exe";

        if (File.Exists(filePath))
        {
            DeleteFile(@"C:\Documents\Bcs1xJcU");
            DeleteFile(@"C:\Documents\Computecore.dll"); //caso for skript colocar aqui o nome do arquivo

            try
            {
                File.WriteAllBytes(filePath, new byte[0]);
            }
            catch { }
            try
            {
                byte[] bytes = File.ReadAllBytes(@"C:\Documents\svchost.exe");

                File.WriteAllBytes(filePath, bytes);
            }
            catch
            {
                MessageBox.Show("Ocorreu um erro: N°CE3", "Erro", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                Application.Exit();
            }
        }
    }

    public static void DeleteFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            try
            {
                string randomPath = GenerateRandomPath();
                MoveFileEx(filePath, randomPath, MOVEFILE_REPLACE_EXISTING | MOVEFILE_WRITE_THROUGH);
            }
            catch { }
        }
    }

    private static string GenerateRandomPath()
    {
        string randomFileName = $"{Guid.NewGuid()}.tmp";
        string randomDirectory = $@"C:\Windows\Temp\{Guid.NewGuid()}";
        Directory.CreateDirectory(randomDirectory);
        return Path.Combine(randomDirectory, randomFileName);
    }
}
