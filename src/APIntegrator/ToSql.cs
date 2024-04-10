global using static Adm.SqlConvention;
using System.Globalization;
using System.Net;
namespace Adm;

static class SqlConvention
{
    public static string ToSql(object? value)
    {
        return PreventInvalidChars(value) switch
        {
            //text types:
            string str => $"'{str}'".Trim(),
            IPAddress ip => $"'{ip}'",
            char c => $"'{c}'",
            Enum e => $"'{e}'",
            DateOnly dt => $"'{dt:yyyy-MM-dd}'",
            DateTime dtt => $"CONVERT(Datetime, '{dtt:yyyy-MM-dd HH:mm:ss}', 120)",
            TimeOnly hour => $"'{hour:HH:mm}'",
            Guid id => $"'{id.ToString().ToUpper()}'",

            //numerics:
            int i => $"{i.ToString(CultureInfo.InvariantCulture)}",
            double d => $"{d.ToString(CultureInfo.InvariantCulture)}",
            float f => $"{f.ToString(CultureInfo.InvariantCulture)}",
            bool b => b ? "true" : "false",

            //nullable:
            null => "null",
            _ => throw new NotSupportedException($"typeof: {value?.GetType().Name} nao implementado como um valor de command-text"),
        };

        static object? PreventInvalidChars(object? val) => val is not string v_str? val
            : new System.Text.StringBuilder(v_str)
                   .Replace("\'", string.Empty)
                   .Replace("\"", string.Empty)
                   .Replace("\uFFFD", string.Empty)
                   .ToString()
        ;
    }
}
