namespace blucaste.Services
{
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    namespace NetworkServices.Telnet
    {
        public static class TelnetHelperAppService
        {
            public static async Task<string> ReadResponseAsync(StreamReader reader, int timeoutMs = 10000)
            {
                var sb = new StringBuilder();
                var buffer = new char[1024];
                var start = DateTime.Now;

                while ((DateTime.Now - start).TotalMilliseconds < timeoutMs)
                {
                    if (reader.Peek() > -1)
                    {
                        int charsRead = await reader.ReadAsync(buffer, 0, buffer.Length);
                        sb.Append(buffer, 0, charsRead);
                        string response = sb.ToString();
                        //
                        // Verifica prompts conhecidos e retorna imediatamente
                        if (response.Contains("login:", StringComparison.OrdinalIgnoreCase) ||
                            response.Contains("password:", StringComparison.OrdinalIgnoreCase) ||
                            response.Contains("WAP>") || response.Contains("#") || response.Contains(">"))
                        {
                            return response;
                        }
                    }
                    else
                    {
                        await Task.Delay(100);
                    }
                }

                return sb.ToString(); // timeout, retorna o que conseguiu ler
            }

        }
    }
}