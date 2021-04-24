using Generator.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Generator
{
    class Program
    {
        private const string SPDX_RELEASES_URI = "https://api.github.com/repos/spdx/license-list-data/releases";
        private const string LICENSES_OUTPUT_LOCATION = "../../../../LicenseIdentifiers/" + LicenseIdentifierFileComponents.NAME;
        private const string NUSPEC_OUTPUT_LOCATION = "../../../../LicenseIdentifiers/LicenseIdentifiers.nuspec";

        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "License-Identifiers");

            var spdxReleaseMetadata = await GetLatestSpdxReleaseMetadata();
            var spdxRelease = await GetSpdxRelease(spdxReleaseMetadata.TagName);
            GenerateLicensesClass(spdxRelease.Licenses, LICENSES_OUTPUT_LOCATION);
            
            await UpdateNuspecFile(spdxReleaseMetadata);

            Console.WriteLine("Done!");
        }

        private static void GenerateLicensesClass(List<License> licenses, string destination)
        {
            using var outputFile = new StreamWriter(destination);

            outputFile.Write(LicenseIdentifierFileComponents.HEADER);
            outputFile.Write(LicenseIdentifierFileComponents.USINGS);

            foreach (var license in licenses)
            {
                outputFile.Write(
                    string.Format(LicenseIdentifierFileComponents.LICENSE_DEFINITION,
                        Utils.SanitiseLicenseId(license.LicenseId),
                        license.Reference,
                        license.IsDeprecatedLicenseId.ToString().ToLower(),
                        license.DetailsUrl,
                        license.ReferenceNumber,
                        license.Name.Replace("\"", "\\\""),
                        license.LicenseId,
                        Utils.SpreadArrayValues(license.SeeAlso),
                        license.IsOsiApproved.ToString().ToLower()));
            }

            outputFile.Write(LicenseIdentifierFileComponents.LICENSE_CONSTRUCTOR);
            outputFile.Write(LicenseIdentifierFileComponents.LICENSE_PROPERTIES);
            outputFile.Write(LicenseIdentifierFileComponents.FOOTER);
        }

        private static async Task<Release> GetLatestSpdxReleaseMetadata()
        {
            var spdxReleases = await client.GetStringAsync(SPDX_RELEASES_URI);
            var releases = Utils.ParseJson<List<Release>>(spdxReleases);

            if (releases == null || !releases.Any())
            {
                throw new Exception("No SPDX releases found.");
            }

            return releases.FirstOrDefault();
        }

        private static async Task<LicensesList> GetSpdxRelease(string releaseTagName)
        {
            var licensesUri = $"https://github.com/spdx/license-list-data/archive/{releaseTagName}.zip";
            var spdxLicenses = await client.GetAsync(licensesUri);

            var zipName = spdxLicenses.Content.Headers.ContentDisposition.FileName;
            var folderName = Path.GetFileNameWithoutExtension(zipName);

            var spdxLicensesContent = Utils.GetFileContentsFromZip(
                await spdxLicenses.Content.ReadAsStreamAsync(),
                $"{folderName}/json/licenses.json");

            return Utils.ParseJson<LicensesList>(spdxLicensesContent);
        }

        private static async Task UpdateNuspecFile(Release releaseMetadata)
        {
            var version = releaseMetadata.TagName;
            if (version.StartsWith('v'))
            {
                version = version[1..];
            }

            var nuspecContents = NuspecGenerator.GenerateContent(version, releaseMetadata.ReleaseDescription);
            await File.WriteAllTextAsync(NUSPEC_OUTPUT_LOCATION, nuspecContents);
        }
    }
}
