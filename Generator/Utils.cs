using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Generator
{
    public static class Utils
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        private static readonly Regex IllegalCharacters = new Regex("[^a-zA-Z0-9_]");

        /// <summary>
        /// Takes a license ID and formats it to be a valid C# variable name.
        /// <code>gpl-1.0+ -> GPL_1_0_PLUS</code>
        /// </summary>
        /// <param name="licenseId"></param>
        /// <returns></returns>
        public static string SanitiseLicenseId(string licenseId)
        {
            var formattedId = licenseId
                .Replace('-', '_')
                .Replace('.', '_')
                .Replace("+", "_PLUS")
                .ToUpper();

            if (char.IsDigit(formattedId.First()))
                formattedId = "_" + formattedId;

            return IllegalCharacters.Replace(formattedId, string.Empty);
        }

        /// <summary>
        /// Takes a list of strings and formats them for insertion into an anonymous C# array.
        /// <code>["cat","dog","frog"] -> '"cat", "dog", "frog"'</code>
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string SpreadArrayValues(IEnumerable<string> source)
        {
            return string.Join(',', source.Select(value => $"\"{value}\""));
        }
    }
}
