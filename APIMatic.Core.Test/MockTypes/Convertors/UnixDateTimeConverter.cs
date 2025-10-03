using APIMatic.Core.Utilities.Date;

namespace APIMatic.Core.Test.MockTypes.Convertors
{
    /// <summary>
    /// Extends from DateTimeConverterBase, uses unix DateTime format.
    /// </summary>
    #pragma warning disable S2094
    public class UnixDateTimeConverter : CoreUnixDateTimeConverter { }
    #pragma warning restore S2094
}
