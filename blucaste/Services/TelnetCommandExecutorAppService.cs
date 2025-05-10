using blucaste.Logger;
using blucaste.Services.NetworkServices.Telnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blucaste.Services
{
    public class TelnetCommandExecutor
    {
        public async Task<bool> ExecuteCommandsAsync(StreamWriter writer, StreamReader reader, List<string> commands)
        {
            foreach (var command in commands)
            {
                await writer.WriteLineAsync(command);
                TelnetLogger.Log($"Enviado comando: {command}");

                var response = await TelnetHelperAppService.ReadResponseAsync(reader);

                if (response.Contains("Error") || response.Contains("incorrect") || response.Contains("failed"))
                {
                    TelnetLogger.Log($"Erro ao executar comando: {command}. Resposta: {response}");
                    return false;
                }

                TelnetLogger.Log($"Resposta do comando '{command}': {response}");
            }
            return true;
        }
    }
}
