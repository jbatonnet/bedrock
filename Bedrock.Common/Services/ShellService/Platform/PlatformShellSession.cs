using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Diagnostics;
using System.IO;

namespace Bedrock.Common
{
    public class PlatformShellEnvironment : VariableList<string>
    {
        public PlatformShellEnvironment(EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {
            foreach (DictionaryEntry variable in Environment.GetEnvironmentVariables(target))
                Values.Add(variable.Key.ToString(), variable.Value.ToString());

            Values.Add("A", "1234");
            Values.Add("B", "5678");
        }
    }

    public class PlatformShellSession : ShellSession
    {
        public override IVariableList Variables => new PlatformShellEnvironment(EnvironmentVariableTarget.Process);

        private Process shellProcess;
        private string identifier;
        private string directory = Environment.CurrentDirectory;

        public PlatformShellSession()
        {
            shellProcess = new Process();
            identifier = string.Format("_{0}_{1}", GetType().Name, GetHashCode());

            // Set up the process
            shellProcess.StartInfo.CreateNoWindow = true;
            shellProcess.StartInfo.UseShellExecute = false;
            shellProcess.StartInfo.WorkingDirectory = directory;
            shellProcess.StartInfo.RedirectStandardOutput = true;
            shellProcess.StartInfo.RedirectStandardError = true;
            shellProcess.StartInfo.RedirectStandardInput = true;

            // Setup platform specific stuff
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                shellProcess.StartInfo.FileName = "sh";
            }
            else if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                shellProcess.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(850);
                shellProcess.StartInfo.StandardErrorEncoding = Encoding.GetEncoding(850);

                shellProcess.StartInfo.FileName = "cmd";
            }
            else
                throw new PlatformNotSupportedException();

            shellProcess.Start();

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                string line = null;

                while (line != "")
                    line = shellProcess.StandardOutput.ReadLine();

                // Change shell prompt to detect command end
                shellProcess.StandardInput.WriteLine("prompt " + identifier);
                shellProcess.StandardInput.WriteLine();
                shellProcess.StandardInput.Flush();

                while (line != identifier)
                    line = shellProcess.StandardOutput.ReadLine();
            }
        }

        public override bool Run(string command)
        {
            // TODO: Write environment

            shellProcess.StandardInput.WriteLine(command);
            shellProcess.StandardInput.WriteLine();
            shellProcess.StandardInput.Flush();

            shellProcess.StandardOutput.ReadLine();

            while (true)
            {
                string line = shellProcess.StandardOutput.ReadLine();

                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                {
                    if (line == identifier) // End of command output
                        break;
                }

                OnOutput(line);
            }

            // TODO: Read environment

            return true;
        }
    }
}
