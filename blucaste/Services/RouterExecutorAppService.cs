using System.Net.NetworkInformation;
using blucaste.Gateway;
using blucaste.Logger;
using blucaste.Scripts;
using NetworkServices.Telnet;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Renci.SshNet;

namespace blucaste.Services {
    public class RouterExecutorAppService {
        private bool IsInterfaceEnabled (string interfaceName) {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();

            foreach (var ni in interfaces) {
                bool nomeBate = ni.Name.IndexOf(interfaceName, StringComparison.OrdinalIgnoreCase) >= 0;
                bool descricaoBate = ni.Description.IndexOf(interfaceName, StringComparison.OrdinalIgnoreCase) >= 0;

                if ((nomeBate || descricaoBate) && ni.OperationalStatus == OperationalStatus.Up) {
                    TelnetLogger.Log($"Interface encontrada: {ni.Name} - Status: {ni.OperationalStatus} - Tipo: {ni.NetworkInterfaceType}");
                    return true;
                }
            }

            TelnetLogger.Log($"Nenhuma interface ativa encontrada com nome contendo '{interfaceName}'.");
            return false;
        }


        private async Task<string> ObterIpDoRoteador (string interfaceName) {
            var gatewayNetwork = new GatewayNetwork();
            string gateway = null;

            if (!IsInterfaceEnabled(interfaceName)) {
                TelnetLogger.Log($"Interface {interfaceName} está desativada.");
                return null;
            }

            try {
                // Timeout de 3 segundos para cada tentativa
                using (var cts = new CancellationTokenSource(3000)) {
                    gateway = await Task.Run(() => gatewayNetwork.GetGatewayIp(interfaceName), cts.Token);
                }
            } catch (OperationCanceledException) {
                TelnetLogger.Log($"Timeout ao buscar gateway em {interfaceName}");
            }

            if (string.IsNullOrWhiteSpace(gateway)) {
                try {
                    using (var cts = new CancellationTokenSource(3000)) {
                        gateway = await Task.Run(() => gatewayNetwork.GetGatewayIp("ethernet"), cts.Token);
                    }
                } catch (OperationCanceledException) {
                    TelnetLogger.Log("Timeout ao buscar gateway ethernet");
                }
            }

            if (string.IsNullOrWhiteSpace(gateway)) {
                try {
                    using (var cts = new CancellationTokenSource(3000)) {
                        gateway = await Task.Run(() => gatewayNetwork.GetGatewayIp("wi-fi"), cts.Token);
                    }
                } catch (OperationCanceledException) {
                    TelnetLogger.Log("Timeout ao buscar gateway wi-fi");
                }
            }

            if (string.IsNullOrWhiteSpace(gateway)) {
                TelnetLogger.Log("Nenhum gateway encontrado - sem roteador conectado.");
                return null;
            }

            // Ping mais rápido - 300ms apenas
            using var ping = new Ping();
            try {
                var reply = await ping.SendPingAsync(gateway, 300);
                TelnetLogger.Log($"Ping para {gateway}: {reply.Status}");

                if (reply.Status != IPStatus.Success) {
                    TelnetLogger.Log($"Gateway {gateway} não responde - roteador não encontrado.");
                    return null;
                }

                return gateway;
            } catch (Exception ex) {
                TelnetLogger.Log($"Erro no ping para {gateway}: {ex.Message}");
                return null;
            }
        }

        public async Task ExecuteAsync (string interfaceName, string selectedRouter, string user, string password, string wifi24, string wifi5G) {

            TelnetLogger.Log("Início da execução do botão.");
            wifi24 = wifi24.Replace(" ", "_");
            wifi5G = wifi5G.Replace(" ", "_");
            var gatewayNetWork = new GatewayNetwork();
            var routerIp = await ObterIpDoRoteador(interfaceName);
            if (string.IsNullOrEmpty(routerIp)) {
                TelnetLogger.Log("Nenhum IP identificado. Encerrando operação.");
                CustomMessageBox.ShowMessage("Roteador não encontrado.", false);
                return;
            } else {

                if (string.IsNullOrEmpty(routerIp)) {
                    TelnetLogger.Log("Nenhum IP identificado. Encerrando operação.");
                    CustomMessageBox.ShowMessage("Roteador não encontrado.", false);
                    return;
                } else {
                    TelnetLogger.Log($"IP identificado: {routerIp}");
                }

            }

            if (string.IsNullOrEmpty(routerIp)) {
                CustomMessageBox.ShowMessage( "Roteador não encontrado.", false);
                return;
            }
            var telnetService = new TelnetCommandAppService(routerIp, 23, "marcelo", "firmware");
            string scriptGeneration;
            if (selectedRouter == "preta") {
                scriptGeneration = ScriptConst.GerarScriptModeloPreto(user, password, wifi24, wifi5G);
                var script = scriptGeneration
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .ToList();
                await telnetService.ExecuteCommandsAsync(script);

            } else {
                TelnetLogger.Log($"Roteador selecionado: {selectedRouter.ToUpper()}. Preparando para conexão SSH.");
                var sshService = new SshCommandAppService(routerIp, 22, "telecomadmin", "ADM1N1str@t0rUM");

                if (!await sshService.TryConnectAsync()) {
                    TelnetLogger.Log("Falha ao conectar via SSH. Verifique IP e credenciais. [FALHA]");
                    CustomMessageBox.ShowMessage("Erro de Conexão, tente novamente!", false);
                    return;
                }
                TelnetLogger.Log("Conexão SSH bem-sucedida. [SUCESSO]");
                scriptGeneration = ScriptConst.GerarScriptModeloBranco(password, wifi24, wifi5G, "BR");
                var script = scriptGeneration
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .ToList();

                foreach (var comando in script) {
                    TelnetLogger.Log($"Enviando comando via SSH: {comando}");
                    try {
                        string resultado = await sshService.ExecuteSingleCommandAsync(comando);
                        if (!string.IsNullOrWhiteSpace(resultado))
                            TelnetLogger.Log($"Resposta de '{comando}': {resultado}");
                        else
                            TelnetLogger.Log($"Sem resposta de '{comando}'. Pode indicar execução silenciosa.");
                    } catch (Exception ex) {
                        TelnetLogger.Log($"Erro ao executar '{comando}': {ex.Message}");
                    }
                }
                sshService.Dispose();
                TelnetLogger.Log("Execução SSH de comandos (modelo branco) concluída.");
                if (script.Any(cmd => cmd.Contains("restore default"))) {
                    CustomMessageBox.ShowMessage("Concluído, reiniciando!", true);
                } else {
                    CustomMessageBox.ShowMessage("Operação concluída.", true);
                }
            }
        }
    }
}