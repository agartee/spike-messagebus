﻿using System.Text.RegularExpressions;

namespace Spike.SqlServer.Extensions
{
    public static class StringExtensions
    {
        private static readonly Regex trimmer = new Regex(@"\s\s+");

        public static string TrimExtraWhitespace(this string str)
        {
            return trimmer.Replace(str, " ");
        }
    }
}
