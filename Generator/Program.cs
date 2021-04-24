using Generator.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO.Compression;

namespace Generator
{
    class Program
    {
        private const string SPDX_RELEASES_URI = "https://api.github.com/repos/spdx/license-list-data/releases";

        private const string OUTPUT_LOCATION = "../../../../LicenseIdentifiers/" + FileComponents.NAME;

        static async Task Main(string[] args)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "License-Identifiers");

            var spdxReleases = await client.GetStringAsync(SPDX_RELEASES_URI);
            var releases = Utils.ParseJson<List<Releases>>(spdxReleases);

            if (releases == null || !releases.Any())
            {
                throw new Exception("No SPDX releases found.");
            }

            var licensesUri = $"https://github.com/spdx/license-list-data/archive/{releases.FirstOrDefault().TagName}.zip";
            var spdxLicenses = await client.GetAsync(licensesUri);

            var zipName = spdxLicenses.Content.Headers.ContentDisposition.FileName;
            var folderName = Path.GetFileNameWithoutExtension(zipName);

            var spdxLicensesContent = Utils.GetFileFromZip(await spdxLicenses.Content.ReadAsStreamAsync(), $"{folderName}/json/licenses.json");

            var licenses = Utils.ParseJson<LicensesList>(spdxLicensesContent);
            GenerateLicensesClass(licenses, OUTPUT_LOCATION);
        }

        private static void GenerateLicensesClass(LicensesList data, string destination)
        {
            using var outputFile = new StreamWriter(destination);

            outputFile.Write(FileComponents.HEADER);
            outputFile.Write(FileComponents.USINGS);

            foreach (var license in data.Licenses)
            {
                outputFile.Write(
                    string.Format(FileComponents.LICENSE_DEFINITION,
                        Utils.TransformId(license.LicenseId),
                        license.Reference,
                        license.IsDeprecatedLicenseId.ToString().ToLower(),
                        license.DetailsUrl,
                        license.ReferenceNumber,
                        license.Name.Replace("\"", "\\\""),
                        license.LicenseId,
                        Utils.TransformArrayValues(license.SeeAlso),
                        license.IsOsiApproved.ToString().ToLower()));
            }

            outputFile.Write(FileComponents.LICENSE_CONSTRUCTOR);
            outputFile.Write(FileComponents.LICENSE_PROPERTIES);
            outputFile.Write(FileComponents.FOOTER);
        }
    }
}
