using LicenseIdentifiers;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Generator
{
    class Program
    {
        static void Main(string[] args)
        {
            Regex illegalCharacters = new Regex("[^a-zA-Z0-9_]");

            var raw = "{\"licenseListVersion\":\"a3d43ca\",\"licenses\":[{\"reference\":\"./ GPL - 2.0 - with - classpath - exception.json\",\"isDeprecatedLicenseId\":true,\"detailsUrl\":\"./ GPL - 2.0 - with - classpath - exception.html\",\"referenceNumber\":0,\"name\":\"GNU General Public License v2.0 w / Classpath exception\",\"licenseId\":\"GPL-2.0-with-classpath-exception\",\"seeAlso\":[\"https://www.gnu.org/software/classpath/license.html\"],\"isOsiApproved\":false},{\"reference\":\"./APSL-1.1.json\",\"isDeprecatedLicenseId\":false,\"detailsUrl\":\"./APSL-1.1.html\",\"referenceNumber\":1,\"name\":\"Apple Public Source License 1.1\",\"licenseId\":\"APSL-1.1\",\"seeAlso\":[\"http://www.opensource.apple.com/source/IOSerialFamily/IOSerialFamily-7/APPLE_LICENSE\"],\"isOsiApproved\":true},{\"reference\":\"./OLDAP-2.8.json\",\"isDeprecatedLicenseId\":false,\"detailsUrl\":\"./OLDAP-2.8.html\",\"referenceNumber\":2,\"name\":\"Open LDAP Public License v2.8\",\"licenseId\":\"OLDAP-2.8\",\"seeAlso\":[\"http://www.openldap.org/software/release/license.html\"],\"isOsiApproved\":true}]}";
            var data = JsonSerializer.Deserialize<LicensesList>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // GPL-2.0-with-classpath-exception -> GPL2_0WithClasspathException
            string FormatId(string id)
            {
                var textSegments = id
                    .Split('-')
                    .Select(s =>
                        s.First().ToString().ToUpper() + s.Substring(1))
                    .ToArray();

                var result = string.Join(string.Empty, textSegments).Replace('.', '_');

                return illegalCharacters.Replace(result, "");
            };

            using (StreamWriter outputFile = new StreamWriter("../../../../LicenseIdentifiers/LicenseIdentifier.cs"))
            {
                outputFile.Write(FileComponents.HEADER);

                foreach (var license in data.Licenses)
                {
                    outputFile.Write(string.Format(FileComponents.LICENSE_DEFINITION,
                        FormatId(license.LicenseId),
                        license.Reference,
                        license.IsDeprecatedLicenseId.ToString().ToLower(),
                        license.DetailsUrl,
                        license.ReferenceNumber,
                        license.Name,
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
