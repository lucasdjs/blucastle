using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace blucaste.Services
{
    public class UserPresetManager
    {
        private readonly string _userPresetFolderPath;
        private readonly string _userPresetFilePath; // Este arquivo "Usuários.txt" não está sendo usado para presets
        private readonly string _presetsDetailsFilePath;

        public UserPresetManager()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            _userPresetFolderPath = Path.Combine(documentsPath, "Blu-Castle");
            // O arquivo Usuários.txt não parece ser relevante para presets de roteador aqui.
            // _userPresetFilePath = Path.Combine(_userPresetFolderPath, "Usuários.txt");
            _presetsDetailsFilePath = Path.Combine(_userPresetFolderPath, "PresetsDetalhes.txt");

            // Garante que a pasta e o arquivo de presets existam na inicialização
            CreateOrUpdatePresetsDetailsFile();
        }

        /// <summary>
        /// Cria a pasta e o arquivo de presets com os valores padrão se não existirem.
        /// </summary>
        public void CreateOrUpdatePresetsDetailsFile()
        {
            try
            {
                if (!Directory.Exists(_userPresetFolderPath))
                {
                    Directory.CreateDirectory(_userPresetFolderPath);
                }

                // Cria o arquivo somente se ele não existir
                if (!File.Exists(_presetsDetailsFilePath))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("[Preset_Preta]");
                    sb.AppendLine("Usuario=admin");
                    sb.AppendLine("Senha=admin");
                    sb.AppendLine("Wifi24=Blucaste_2.4G"); // Valores padrão para Wi-Fi
                    sb.AppendLine("Wifi5G=Blucaste_5G");
                    sb.AppendLine(); // Linha em branco para separação
                    sb.AppendLine("[Preset_Branca]");
                    sb.AppendLine("Usuario=L1vt1m4eng");
                    sb.AppendLine("Senha=admin");
                    sb.AppendLine("Wifi24=Blucaste_2.4G"); // Valores padrão para Wi-Fi
                    sb.AppendLine("Wifi5G=Blucaste_5G");

                    File.WriteAllText(_presetsDetailsFilePath, sb.ToString());
                }
            }
            catch (Exception ex)
            {
                // Considere usar seu TelnetLogger aqui
                // blucaste.Logger.TelnetLogger.Log($"Erro ao criar/atualizar PresetsDetalhes.txt: {ex.Message}");
                Console.WriteLine($"Erro ao criar/atualizar PresetsDetalhes.txt: {ex.Message}"); // Para debug
            }
        }

        /// <summary>
        /// Carrega os dados de um preset específico do arquivo.
        /// </summary>
        /// <param name="presetName">O nome do preset (ex: "Preta", "Branca").</param>
        /// <returns>Um dicionário com os dados do preset, ou null se não encontrado.</returns>
        public Dictionary<string, string> LoadPreset(string presetName)
        {
            if (!File.Exists(_presetsDetailsFilePath))
            {
                // O arquivo não existe, talvez deva ser criado com valores padrão
                CreateOrUpdatePresetsDetailsFile();
                if (!File.Exists(_presetsDetailsFilePath)) return null; // Se ainda assim não existir, retorna null
            }

            Dictionary<string, string> presetData = new Dictionary<string, string>();
            bool inTargetPreset = false;

            try
            {
                var lines = File.ReadAllLines(_presetsDetailsFilePath);
                foreach (var line in lines)
                {
                    string trimmedLine = line.Trim();

                    if (string.IsNullOrWhiteSpace(trimmedLine))
                    {
                        if (inTargetPreset) break; // Termina a leitura se encontrar uma linha em branco após o preset
                        continue;
                    }

                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                    {
                        string sectionName = trimmedLine.Substring(1, trimmedLine.Length - 2);
                        if (sectionName.Equals($"Preset_{presetName}", StringComparison.OrdinalIgnoreCase))
                        {
                            inTargetPreset = true;
                        }
                        else if (inTargetPreset)
                        {
                            break; // Já encontramos o preset e passamos para o próximo
                        }
                        continue;
                    }

                    if (inTargetPreset)
                    {
                        var parts = trimmedLine.Split(new char[] { '=' }, 2);
                        if (parts.Length == 2)
                        {
                            presetData[parts[0].Trim()] = parts[1].Trim();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // blucaste.Logger.TelnetLogger.Log($"Erro ao carregar preset '{presetName}': {ex.Message}");
                Console.WriteLine($"Erro ao carregar preset '{presetName}': {ex.Message}"); // Para debug
                return null;
            }

            return presetData.Any() ? presetData : null;
        }

        /// <summary>
        /// Salva (atualiza) os dados de um preset específico no arquivo.
        /// Se o preset não existir, ele será adicionado (mas o formato espera que ele já exista).
        /// </summary>
        /// <param name="presetName">O nome do preset (ex: "Preta", "Branca").</param>
        /// <param name="user">O novo nome de usuário.</param>
        /// <param name="password">A nova senha.</param>
        /// <param name="wifi24">O novo SSID do Wi-Fi 2.4G.</param>
        /// <param name="wifi5g">O novo SSID do Wi-Fi 5G.</param>
        public void SavePreset(string presetName, string user, string password, string wifi24, string wifi5g)
        {
            if (!File.Exists(_presetsDetailsFilePath))
            {
                CreateOrUpdatePresetsDetailsFile(); // Garante que o arquivo exista
            }

            try
            {
                List<string> lines = File.ReadAllLines(_presetsDetailsFilePath).ToList();
                StringBuilder newFileContent = new StringBuilder();
                bool presetFound = false;
                bool inTargetPreset = false;

                for (int i = 0; i < lines.Count; i++)
                {
                    string trimmedLine = lines[i].Trim();

                    if (trimmedLine.StartsWith("[") && trimmedLine.EndsWith("]"))
                    {
                        string sectionName = trimmedLine.Substring(1, trimmedLine.Length - 2);
                        if (sectionName.Equals($"Preset_{presetName}", StringComparison.OrdinalIgnoreCase))
                        {
                            inTargetPreset = true;
                            presetFound = true;
                            newFileContent.AppendLine($"[{sectionName}]"); // Adiciona o cabeçalho do preset
                            newFileContent.AppendLine($"Usuario={user}");
                            newFileContent.AppendLine($"Senha={password}");
                            newFileContent.AppendLine($"Wifi24={wifi24}");
                            newFileContent.AppendLine($"Wifi5G={wifi5g}");
                            // Pula as linhas antigas deste preset
                            while (i + 1 < lines.Count && !lines[i + 1].Trim().StartsWith("[") && !string.IsNullOrWhiteSpace(lines[i + 1].Trim()))
                            {
                                i++; // Avança o índice para pular as linhas antigas do preset
                            }
                            if (i + 1 < lines.Count && string.IsNullOrWhiteSpace(lines[i + 1].Trim()))
                            {
                                newFileContent.AppendLine(); // Adiciona linha em branco se houver no original
                                i++;
                            }
                        }
                        else
                        {
                            inTargetPreset = false;
                            newFileContent.AppendLine(lines[i]); // Adiciona outras seções como estão
                        }
                    }
                    else if (!inTargetPreset)
                    {
                        newFileContent.AppendLine(lines[i]); // Adiciona linhas que não fazem parte do preset-alvo
                    }
                }

                // Se o preset não foi encontrado, adicione-o ao final (menos ideal para organização, mas funciona)
                if (!presetFound)
                {
                    newFileContent.AppendLine();
                    newFileContent.AppendLine($"[Preset_{presetName}]");
                    newFileContent.AppendLine($"Usuario={user}");
                    newFileContent.AppendLine($"Senha={password}");
                    newFileContent.AppendLine($"Wifi24={wifi24}");
                    newFileContent.AppendLine($"Wifi5G={wifi5g}");
                }

                File.WriteAllText(_presetsDetailsFilePath, newFileContent.ToString());
            }
            catch (Exception ex)
            {
                // blucaste.Logger.TelnetLogger.Log($"Erro ao salvar preset '{presetName}': {ex.Message}");
                Console.WriteLine($"Erro ao salvar preset '{presetName}': {ex.Message}"); // Para debug
            }
        }
    }
}