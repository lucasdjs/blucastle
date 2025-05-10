namespace blucaste.Scripts
{
    public static class ScriptConst
    {
        // === COMANDOS COMUNS AOS DOIS MODELOS ===
        public const string WebAccountSetUser = "prolinecmd webAccount set user";
        public const string WebPwdSetUser = "prolinecmd webpwd set user";
        public const string WpaKeySet24G = "prolinecmd wpakey set 12345678";
        public const string WpaKeySet5G = "prolinecmd wpakeyac set 12345678";
        public const string RestoreDefault = "prolinecmd restore default";

        // === COMANDOS ESPECÍFICOS - MODELO PRETO ===
        public static string SuperWebAccountSetAdmin(string usuario)
        {
            return $"prolinecmd superwebAccount set {usuario}";
        }

        public static string SuperWebPwdSetAdmin(string senha)
        {
            return $"prolinecmd superwebpwd set {senha}";
        }

        // === COMANDOS ESPECÍFICOS - MODELO BRANCO ===
        public static string WebSuperPwdSetAdmin(string senha)
        {
            return $"prolinecmd websuperpwd set {senha}";
        }

        public static string CountryCode(string codigo)
        {
            return $"prolinecmd countrycode set {codigo}";
        }

        // === COMANDOS DINÂMICOS - COMUM PARA OS DOIS MODELOS ===
        public static string SsidSet24G(string ssid)
        {
            return $"prolinecmd ssid set {ssid}";
        }

        public static string SsidSet5G(string ssid)
        {
            return $"prolinecmd ssidac set {ssid}";
        }

        // === SCRIPT COMPLETO - MODELO PRETO ===
        public static string GerarScriptModeloPreto(string usuarioAdmin, string senhaAdmin, string ssid24G, string ssid5G)
        {
            var comandos = new List<string>
            {
                {WebAccountSetUser},
                WebPwdSetUser,
                WpaKeySet24G,
                WpaKeySet5G,
                SuperWebAccountSetAdmin(usuarioAdmin),
                SuperWebPwdSetAdmin(senhaAdmin),
                SsidSet24G(ssid24G),
                SsidSet5G(ssid5G),
                RestoreDefault
            };

            return string.Join(Environment.NewLine, comandos);
        }

        // === SCRIPT COMPLETO - MODELO BRANCO ===
        public static string GerarScriptModeloBranco(string senhaAdmin, string ssid24G, string ssid5G, string countryCode)
        {
            var comandos = new List<string>
            {
                {WebAccountSetUser},
               {WebPwdSetUser},
                WpaKeySet24G,
                WpaKeySet5G,
                WebSuperPwdSetAdmin(senhaAdmin),
                SsidSet24G(ssid24G),
                SsidSet5G(ssid5G),
                CountryCode(countryCode),
                RestoreDefault
            };

            return string.Join(Environment.NewLine, comandos);
        }
    }
}