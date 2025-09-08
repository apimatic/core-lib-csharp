namespace APIMatic.Core.Test.Utilities.Date
{
    internal sealed class StringReplacer
    {
        internal static string ReplaceBackSlashR(string value)
        {
            return value.Replace("\r", "");
        }
    }
}
