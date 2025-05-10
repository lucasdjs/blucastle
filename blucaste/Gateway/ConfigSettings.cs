using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace blucaste.Gateway
{
    public class ConfigSettings
    {
        /// <summary>
        /// Retorna um identificador SHA256 da máquina.
        /// </summary>
        public static string GetCpuAndMotherboardHash()
        {
            try
            {
                string motherboardId = GetWmiProperty("Win32_BaseBoard", "SerialNumber");
                string cpuId = GetWmiProperty("Win32_Processor", "ProcessorId");

                if (string.IsNullOrWhiteSpace(motherboardId) || string.IsNullOrWhiteSpace(cpuId))
                    throw new Exception("Não foi possível obter o ID da placa-mãe ou do processador.");

                string combined = motherboardId + cpuId;
                return GenerateSHA256Hash(combined);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao gerar hash SHA256: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Retorna um GUID baseado nos primeiros 16 bytes do hash SHA256.
        /// </summary>
        public static Guid GetCpuAndMotherboardGuid()
        {
            string hash = GetCpuAndMotherboardHash();
            if (hash == null) return Guid.Empty;

            // Converte os primeiros 16 bytes do hash SHA256 para um Guid
            byte[] hashBytes = Encoding.UTF8.GetBytes(hash);
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] fullHash = sha256.ComputeHash(hashBytes);
                byte[] guidBytes = new byte[16];
                Array.Copy(fullHash, guidBytes, 16);
                return new Guid(guidBytes);
            }
        }

        private static string GetWmiProperty(string wmiClass, string wmiProperty)
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher($"SELECT {wmiProperty} FROM {wmiClass}"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        return obj[wmiProperty]?.ToString() ?? string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao acessar WMI ({wmiClass}.{wmiProperty}): {ex.Message}");
            }
            return string.Empty;
        }

        private static string GenerateSHA256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                var sb = new StringBuilder();
                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }
    }
}
