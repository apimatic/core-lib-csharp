namespace APIMatic.Core.Test.Utilities.Date
{
    internal class StringReplacer
    {
        internal static string ReplaceBackSlashR(string value)
        {
            return value.Replace("/r", "");
        }
    }
}
