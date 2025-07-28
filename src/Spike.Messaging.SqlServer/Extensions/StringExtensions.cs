using System.Text.RegularExpressions;

namespace Spike.Messaging.SqlServer.Extensions
{
    public static class StringExtensions
    {
        public static string ToCompactSql(this string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return string.Empty;

            var compact = Regex.Replace(sql, @"\s+", " ");

            return compact.Trim();
        }
    }
}
