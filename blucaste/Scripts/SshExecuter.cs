
using Renci.SshNet;

namespace blucaste.Scripts {
    public class SshExecuter {
        private readonly string _host;
        private readonly string _username;
        private readonly string _password;
        public SshExecuter (string host, string username, string password) {
            _host = host;
            _username = username;
            _password = password;
        }

        public void ExecutarScript (string script) {
            using var client = new SshClient(_host, _username, _password);
            client.Connect();

            var comandos = script.Split(Environment.NewLine);

            foreach (var comando in comandos) {
                if (!string.IsNullOrWhiteSpace(comando)) {
                    Console.WriteLine($"Enviando: {comando}");
                    var resultado = client.RunCommand(comando);
                    Console.WriteLine(resultado.Result);
                }
            }

            client.Disconnect();
        }
        public string ExecutarComandoSsh (string ip, string usuario, string senha, string comando) {
            using (var client = new SshClient(ip, usuario, senha)) {
                client.Connect();
                if (!client.IsConnected)
                    return "Falha na conexão SSH.";

                var result = client.RunCommand(comando);
                client.Disconnect();
                return result.Result.Trim(); // Limpa espaços e quebras de linha
            }
        }
        public async Task<string> ExecutarComandoSshAsync (string comando) {
            return await Task.Run(() =>
            {
                using var client = new SshClient(_host, _username, _password);
                client.Connect();
                if (!client.IsConnected)
                    return "Falha na conexão SSH.";

                var result = client.RunCommand(comando);
                client.Disconnect();

                return result.Result.Trim();
            });
        }


    }

}
