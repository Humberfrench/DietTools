namespace Dietcode.Core.Lib.Rest
{
    using System.ComponentModel;

    public enum EnumApiRest
    {
        [Description("Autenticação Basic")]
        Basic = 1,

        [Description("Autenticação Bearer")]
        Bearer = 2,

        [Description("API Key via Header")]
        XApiKey = 3,

        [Description("Sem Autenticação")]
        None = 4
    }
}

