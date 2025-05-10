namespace blucaste.Logger
{
    public static class TelnetLogger
    {
        // Altere isso para 'false' para desativar os logs
        public static bool IsLoggingEnabled { get; set; } = false;

        private static readonly string LogFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "config_log.txt"
        );

        public static void Log(string message)
        {
            if (!IsLoggingEnabled)
                return;

            File.AppendAllText(LogFilePath, $"{DateTime.Now}: {message}{Environment.NewLine}");
        }
    }
}
