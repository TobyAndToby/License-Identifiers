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
        /// Given a JSON file location, read and parse into specified generic type. Logs exceptions.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileLocation"></param>
        /// <returns></returns>
        public static T ParseJson<T>(string raw) where T : new()
        {
            var data = new T();

            try
            {
                data = JsonSerializer.Deserialize<T>(raw, JsonOptions);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return data;
        }

        /// <summary>
        /// Takes a license ID and formats it to be a valid C# variable name.
        /// <code>gpl-1.0+ -> GPL_1_0_PLUS</code>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string TransformId(string id)
        {
            var formattedId = id
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
        public static string TransformArrayValues(IEnumerable<string> source)
        {
            return string.Join(',', source.Select(value => $"\"{value}\""));
        }

        public static string GetFileFromZip(Stream data, string targetFilePath)
        {
            using var zipArchive = new ZipArchive(data, ZipArchiveMode.Read);
            
            foreach (var entry in zipArchive.Entries)
            {
                if (entry.FullName != targetFilePath)
                {
                    continue;
                }

                using var stream = entry.Open();
                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);

                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            return null;
        }
    }
}
