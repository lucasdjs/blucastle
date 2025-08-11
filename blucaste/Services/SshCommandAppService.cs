using Renci.SshNet;
using blucaste.Logger;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace blucaste.Services {
    public class SshCommandAppService : IDisposable {
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;

        private SshClient? _client;

        public SshCommandAppService (string host, int port, string username, string password) {
            _host = host;
            _port = port;
            _username = username;
            _password = password;
        }

        public async Task<bool> TryConnectAsync () {
            for (int tentativa = 1; tentativa <= 2; tentativa++) {
                try {
                    if (_client != null && _client.IsConnected)
                        return true;

                    _client = new SshClient(_host, _port, _username, _password);
                    _client.ConnectionInfo.Timeout = TimeSpan.FromSeconds(8);

                    // Conectar de forma assíncrona
                    await Task.Run(() => _client.Connect());

                    if (_client.IsConnected) {
                        TelnetLogger.Log($"Conectado via SSH em {_host}:{_port} como {_username}.");
                        return true;
                    } else {
                        TelnetLogger.Log($"Falha ao conectar via SSH em {_host}:{_port} (tentativa {tentativa}).");
                    }
                } catch (Exception ex) {
                    TelnetLogger.Log($"Erro conexão (tentativa {tentativa}): {ex.Message}");
                    if (tentativa < 2) {
                        await Task.Delay(1000);
                    }
                }
            }
            return false;
        }

        public async Task ExecuteCommandsAsync (List<string> commands) {
            if (_client == null || !_client.IsConnected) {
                TelnetLogger.Log("Cliente SSH não conectado. Impossível executar comandos.");
                CustomMessageBox.ShowMessage( "Erro: conexão SSH não está ativa.", false);
                return;
            }

            try {
                foreach (var command in commands) {
                    TelnetLogger.Log($"Enviando comando SSH: {command}");

                    // Como RunCommand não é async, rodamos em Task.Run para não travar UI
                    using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20))) {
                        var resultado = await Task.Run(() => _client.RunCommand(command), cts.Token);

                        string output = resultado.Result?.Trim() ?? "";

                    TelnetLogger.Log($"Resposta do comando: {output}");

                    // Você pode analisar output para detectar erros ou sucesso
                    if (output.Contains("Concluido", StringComparison.OrdinalIgnoreCase)) {
                        TelnetLogger.Log("Comando indicou conclusão com sucesso.");
                    }

                    // Pequeno delay opcional para não sobrecarregar
                    await Task.Delay(300);
                }
                TelnetLogger.Log("Todos comandos SSH executados com sucesso.");
                }
            } catch (OperationCanceledException) {
                TelnetLogger.Log($"Timeout ao executar comando SSH");
            } catch (Exception ex) {

                TelnetLogger.Log($"Erro na execução dos comandos SSH: {ex.Message}");
                CustomMessageBox.ShowMessage( $"Erro ao executar comandos SSH: {ex.Message}", false);
            }
        }

        public async Task<string> ExecuteSingleCommandAsync (string command) {
            if (_client == null || !_client.IsConnected) {
                TelnetLogger.Log("Cliente SSH não conectado. Impossível executar comando.");
                return "Erro: conexão SSH não está ativa.";
            }

            try {
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20))) {
                    var resultado = await Task.Run(() => _client.RunCommand(command), cts.Token);
                    return resultado.Result?.Trim() ?? "";
                }
            } catch (OperationCanceledException) {
                TelnetLogger.Log($"Timeout ao executar comando SSH: {command}");
                return $"Erro: Timeout no comando";
            } catch (Exception ex) {
                TelnetLogger.Log($"Erro ao executar comando SSH: {ex.Message}");
                return $"Erro: {ex.Message}";
            }
        }

        public void Dispose () {
            if (_client != null) {
                try {
                    if (_client.IsConnected) {
                        // Disconnect assíncrono para não travar
                        Task.Run(() => {
                            try { _client.Disconnect(); } catch { }
                        });
                    }
                } catch { }

                try {
                    _client.Dispose();
                } catch { }

                _client = null;
            }
        }
    }
}
