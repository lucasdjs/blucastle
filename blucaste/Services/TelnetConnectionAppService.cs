using System.Net.Sockets;
using System.Text;

namespace NetworkServices.Telnet
{
    public class TelnetConnectionAppService : IDisposable
    {
        private TcpClient _tcpClient;
        private NetworkStream _networkStream;
        private StreamReader _reader;
        private StreamWriter _writer;

        public StreamWriter Writer => _writer;
        public StreamReader Reader => _reader;

        public async Task ConnectAsync(string host, int port)
        {
            _tcpClient = new TcpClient(host, port);
            _networkStream = _tcpClient.GetStream();
            _reader = new StreamReader(_networkStream, Encoding.ASCII);
            _writer = new StreamWriter(_networkStream, Encoding.ASCII) { AutoFlush = true };
        }

        public void Dispose()
        {
            _writer?.Dispose();
            _reader?.Dispose();
            _networkStream?.Dispose();
            _tcpClient?.Close();
        }
    }
}