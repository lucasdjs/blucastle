using blucaste.Data;
using blucaste.Gateway;
using blucaste.Logger;
using blucaste.Scripts;
using NetworkServices.Telnet;

namespace blucaste.Services
{
    public class RouterExecutorAppService
    {
        public async Task ExecuteAsync(string interfaceName, string selectedRouter, string user, string password, string wifi24, string wifi5G)
        {
            TelnetLogger.Log("Início da execução do botão.");

            wifi24 = wifi24.Replace(" ", "_");
            wifi5G = wifi5G.Replace(" ", "_");

            var gatewayNetWork = new GatewayNetwork();


            var routerIp = await Task.Run(() => gatewayNetWork.GetGatewayIp(interfaceName));

            TelnetLogger.Log($"Ip identificado: {routerIp}");

            if (string.IsNullOrEmpty(routerIp)) {

                CustomMessageBox.ShowMessage("Dispositivo não encontrado.", false);
                return;
            }

            var telnetService = new TelnetCommandAppService(routerIp, 23, "marcelo", "firmware");

            string scriptGeneration;

            if (selectedRouter == "preta")
            {
                scriptGeneration = ScriptConst.GerarScriptModeloPreto(user, password, wifi24, wifi5G);
            }
            else
            {
                scriptGeneration = ScriptConst.GerarScriptModeloBranco(password, wifi24, wifi5G, "999");
            }

            var script = scriptGeneration
                .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(line => line.Trim())
                .ToList();

            await telnetService.ExecuteCommandsAsync(script);
        }
    }
}
