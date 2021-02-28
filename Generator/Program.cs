using LicenseIdentifiers;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            Regex illegalCharacters = new Regex("[^a-zA-Z0-9_]");

            const string URL = "../../../../../license-list-data/json/licenses.json";

            var raw = File.ReadAllText(URL);
            var data = JsonSerializer.Deserialize<LicensesList>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            string FormatIdToVariableName(string id)
            {
                var textSegments = id
                    .Split('-')
                    .Select(s =>
                    {
                        var first = s.First();

                        if (first >= '0' && first <= '9')
                        {
                            return "_" + s;
                        }

                        return char.ToUpper(first) + s.Substring(1);
                    })
                    .ToArray();

                var result = string.Join(string.Empty, textSegments).Replace('.', '_').Replace("+", "Plus");

                return illegalCharacters.Replace(result, "");
            };

            using (StreamWriter outputFile = new StreamWriter("../../../../LicenseIdentifiers/LicenseIdentifier.cs"))
            {
                outputFile.Write(FileComponents.HEADER);

                foreach (var license in data.Licenses)
                {
                    outputFile.Write(string.Format(FileComponents.LICENSE_DEFINITION,
                        FormatIdToVariableName(license.LicenseId),
                        license.Reference,
                        license.IsDeprecatedLicenseId.ToString().ToLower(),
                        license.DetailsUrl,
                        license.ReferenceNumber,
                        license.Name.Replace("\"", "\\\""),
                        license.LicenseId,
                        string.Join(',', license.SeeAlso.Select(value => $"\"{value}\"")),
                        license.IsOsiApproved.ToString().ToLower()));
                }

                outputFile.Write(FileComponents.LICENSE_CONSTRUCTOR);
                outputFile.Write(FileComponents.LICENSE_PROPERTIES);
                outputFile.Write(FileComponents.FOOTER);
            }
        }
    }
}
