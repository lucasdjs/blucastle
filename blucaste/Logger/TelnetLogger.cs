using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace blucaste.Logger
{
    public static class TelnetLogger
    {
        private static readonly string BlucastleFolderName = "Blucastle";
        private static readonly string LogFileName = "config_log.txt";

        private static string _logFilePath;
        private static StreamWriter _logWriter;
        private static readonly object _lock = new object();

        public static bool IsInitialized { get; private set; } = false;

        // Novo controle para ativar/desativar logs
        public static bool IsLoggingEnabled { get; set; } = true;

        public static void Initialize()
        {
            if (IsInitialized || !IsLoggingEnabled)
                return;

            try
            {
                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string blucastlePath = Path.Combine(documentsPath, BlucastleFolderName);

                if (!Directory.Exists(blucastlePath))
                    Directory.CreateDirectory(blucastlePath);

                _logFilePath = Path.Combine(blucastlePath, LogFileName);

                FileStream fileStream = new FileStream(_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                _logWriter = new StreamWriter(fileStream, Encoding.UTF8)
                {
                    AutoFlush = true
                };

                _logWriter.WriteLine($"--- LOG INICIADO: {DateTime.Now:dd/MM/yyyy HH:mm:ss} ---");
                IsInitialized = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TelnetLogger] ERRO CRÍTICO ao inicializar: {ex.Message}");
                IsInitialized = false;
            }
        }

        public static void Log(string message)
        {
            if (!IsLoggingEnabled)
                return;

            if (!IsInitialized)
                Initialize();

            string timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string fullMessage = $"{timestamp}: {message}";

            lock (_lock)
            {
                try
                {
                    if (_logWriter != null)
                    {
                        _logWriter.WriteLine(fullMessage);
                    }
                    else
                    {
                        Console.WriteLine($"[FALHA NO LOGGER] {fullMessage}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[TelnetLogger] ERRO ao escrever log: {ex.Message}");
                }
            }

            Console.WriteLine(fullMessage); // Exibe sempre no console (para debug interno)
        }

        public static async Task LogAsync(string message)
        {
            if (!IsLoggingEnabled)
                return;

            if (!IsInitialized)
                Initialize();

            string timestamp = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            string fullMessage = $"{timestamp}: {message}";

            try
            {
                if (_logWriter != null)
                {
                    await _logWriter.WriteLineAsync(fullMessage);
                    await _logWriter.FlushAsync();
                }
                else
                {
                    Console.WriteLine($"[FALHA NO LOGGER] {fullMessage}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TelnetLogger] ERRO ao escrever log async: {ex.Message}");
            }

            Console.WriteLine(fullMessage);
        }

        public static void Close()
        {
            if (!IsLoggingEnabled)
                return;

            lock (_lock)
            {
                try
                {
                    _logWriter?.Flush();
                    _logWriter?.Close();
                    _logWriter = null;
                    IsInitialized = false;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[TelnetLogger] ERRO ao fechar log: {ex.Message}");
                }
            }
        }
    }
}
