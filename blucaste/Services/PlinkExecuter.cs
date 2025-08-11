using blucaste.Logger;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blucaste.Services
{
    public class PlinkExecutor
    {
        private readonly string _host;
        private readonly string _username;
        private readonly string _password;
        private readonly string _plinkPath;

        public PlinkExecutor(string host, string username, string password)
        {
            _host = host;
            _username = username;
            _password = password;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _plinkPath = Path.Combine(baseDir, "Resources", "Plink", "plink.exe");

            if (!File.Exists(_plinkPath))
            {
                TelnetLogger.Log("plink.exe não encontrado no caminho esperado: " + _plinkPath);
                throw new FileNotFoundException("Arquivo plink.exe não encontrado.", _plinkPath);
            }
        }

        public async Task ExecuteAsync(List<string> commands)
        {
            foreach (var command in commands)
            {
                try
                {
                    TelnetLogger.Log($"Executando com plink: {command}");

                    var startInfo = new ProcessStartInfo
                    {
                        FileName = _plinkPath,
                        Arguments = $"-ssh {_username}@{_host} -pw {_password} -batch -noagent -hostkey * \"{command}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using var process = Process.Start(startInfo);
                    string output = await process.StandardOutput.ReadToEndAsync();
                    string error = await process.StandardError.ReadToEndAsync();

                    process.WaitForExit();

                    TelnetLogger.Log($"Plink Output: {output}");
                    if (!string.IsNullOrWhiteSpace(error))
                        TelnetLogger.Log($"Plink Error: {error}");
                }
                catch (Exception ex)
                {
                    TelnetLogger.Log($"Erro ao executar comando via plink: {ex.Message}");
                }
            }
        }
    }

}
