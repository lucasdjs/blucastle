using Renci.SshNet; // Necessário para SSH.NET
using System;
using System.Collections.Generic; // Embora não usado diretamente nesta classe, pode ser útil
using System.IO;
using System.Text;
using System.Threading.Tasks;
using blucaste.Logger; // Para usar seu logger

// MUDANÇA DE NAMESPACE: Recomenda-se usar um namespace mais específico para serviços SSH.
// Certifique-se de que o arquivo 'SshConnectionAppService.cs' esteja localizado em uma pasta
// que corresponda a este namespace (ex: SeuProjeto/NetworkServices/SSH/SshConnectionAppService.cs)
namespace NetworkServices.SSH
{
    public class SshConnectionAppService : IDisposable
    {
        private SshClient _sshClient;
        private ShellStream _shellStream;
        private StreamReader _reader;
        private StreamWriter _writer;

        // Propriedades para acessar o reader e writer (se necessário para login ou leitura de prompts complexos)
        public StreamReader Reader => _reader;
        public StreamWriter Writer => _writer;


        /// <param name="host">O endereço IP ou hostname do dispositivo.</param>
        /// <param name="port">A porta SSH (geralmente 22).</param>
        /// <param name="username">O nome de usuário para login SSH.</param>
        /// <param name="password">A senha para login SSH.</param>
        /// <returns>Uma Task que representa a operação de conexão e login.</returns>
        /// <exception cref="Exception">Lançada em caso de falha de conexão ou autenticação.</exception>
        public async Task ConnectAndLoginAsync(string host, int port, string username, string password)
        {
            TelnetLogger.Log($"Iniciando conexão SSH com {host}:{port} para o usuário {username}...");
            try
            {
                // Inicializa o cliente SSH com as credenciais fornecidas
                _sshClient = new SshClient(host, port, username, password);
                _sshClient.ConnectionInfo.Timeout = TimeSpan.FromSeconds(30); // Define um timeout para a conexão

                // Tenta conectar de forma assíncrona para não bloquear a thread principal
                await Task.Run(() => _sshClient.Connect());

                if (_sshClient.IsConnected)
                {
                    TelnetLogger.Log($"Conexão SSH estabelecida com sucesso com {host}.");

                    _shellStream = _sshClient.CreateShellStream("xterm", 80, 24, 800, 600, 1024);

                    _reader = new StreamReader(_shellStream, Encoding.UTF8);
                    _writer = new StreamWriter(_shellStream, Encoding.UTF8) { AutoFlush = true }; // AutoFlush garante que os comandos sejam enviados imediatamente

                    await ReadUntilPromptAsync();
                }
                else
                {
                    throw new Exception($"Falha ao conectar SSH a {host}:{port}. Verifique o IP e a porta.");
                }
            }
            catch (Renci.SshNet.Common.SshConnectionException sshEx)
            {
                TelnetLogger.Log($"Erro de conexão SSH: {sshEx.Message}");
                throw new Exception($"Erro de conexão SSH: {sshEx.Message}", sshEx);
            }
            catch (Renci.SshNet.Common.SshAuthenticationException authEx)
            {
                TelnetLogger.Log($"Erro de autenticação SSH: {authEx.Message}");
                throw new Exception($"Erro de autenticação SSH. Verifique usuário e senha. Detalhes: {authEx.Message}", authEx);
            }
            catch (Exception ex)
            {
                TelnetLogger.Log($"Erro geral na conexão SSH: {ex.Message}");
                throw new Exception($"Erro inesperado na conexão SSH. Detalhes: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Executa um único comando SSH no shell aberto e retorna sua saída.
        /// </summary>
        /// <param name="command">O comando a ser executado.</param>
        /// <returns>A string contendo a saída do comando.</returns>
        /// <exception cref="InvalidOperationException">Lançada se a conexão SSH não estiver ativa.</exception>
        public async Task<string> ExecuteCommandAsync(string command)
        {
            // Verifica se o cliente SSH e o shellstream estão conectados e prontos para uso
            if (_sshClient == null || !_sshClient.IsConnected || _shellStream == null)
            {
                throw new InvalidOperationException("Conexão SSH não está ativa ou shell não está pronto.");
            }

            TelnetLogger.Log($"Enviando comando SSH: {command}");
            await _writer.WriteLineAsync(command); // Envia o comando seguido de uma nova linha
            await _writer.FlushAsync(); // Garante que o comando seja enviado imediatamente

            // Lê a saída do comando até encontrar um prompt conhecido ou atingir um timeout
            string output = await ReadUntilPromptAsync(TimeSpan.FromSeconds(10)); // Espera até 10 segundos pela resposta
            TelnetLogger.Log($"Saída SSH para '{command}':\n{output}");
            return output;
        }

        /// <summary>
        /// Lê o ShellStream até encontrar um prompt comum ou até que um timeout seja atingido.
        /// Essa função é crucial para a interação com o shell, pois a saída dos comandos
        /// e os prompts podem não chegar de uma vez.
        /// </summary>
        /// <param name="timeout">Opcional. O tempo máximo para esperar por uma resposta/prompt.</param>
        /// <returns>A string contendo todo o texto lido do stream.</returns>
        private async Task<string> ReadUntilPromptAsync(TimeSpan? timeout = null)
        {
            var output = new StringBuilder();
            var startTime = DateTime.Now;
            var buffer = new char[1024]; // Buffer para ler chunks de dados

            // Define o timeout padrão se não for fornecido
            if (!timeout.HasValue)
            {
                timeout = TimeSpan.FromSeconds(5); // Timeout padrão para leitura de prompt
            }

            while (true)
            {
                // Verifica se há dados disponíveis para leitura no stream
                if (_shellStream.DataAvailable)
                {
                    int bytesRead = await _reader.ReadAsync(buffer, 0, buffer.Length);
                    output.Append(buffer, 0, bytesRead);

                    string currentOutput = output.ToString();
                    if (currentOutput.EndsWith("# ") || currentOutput.EndsWith("$ ") ||
                        currentOutput.EndsWith("> ") || currentOutput.EndsWith("\n\r\n") || // Nova linha dupla (comum após prompts)
                        currentOutput.EndsWith("\r\n#") || currentOutput.EndsWith("\r\n$") ||
                        currentOutput.EndsWith("\r\n>"))
                    {
                        break;
                    }
                }
                else if ((DateTime.Now - startTime) > timeout.Value)
                {
                    TelnetLogger.Log($"Timeout ao esperar pelo prompt SSH. Conteúdo lido até agora: '{output.ToString()}'");
                    break;
                }
                await Task.Delay(50);
            }
            return output.ToString();
        }

        public void Dispose()
        {
            
            _writer?.Dispose();
            _reader?.Dispose();
            _shellStream?.Dispose();

            if (_sshClient != null && _sshClient.IsConnected)
            {
                TelnetLogger.Log("Desconectando cliente SSH.");
                _sshClient.Disconnect();
            }
            _sshClient?.Dispose(); 
            GC.SuppressFinalize(this);
        }
    }
}