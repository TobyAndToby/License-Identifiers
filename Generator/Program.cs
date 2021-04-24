﻿using Generator.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;

namespace Generator
{
    class Program
    {
        private const string SPDX_RELEASES_URI = "https://api.github.com/repos/spdx/license-list-data/releases";
        private static readonly string LICENSES_OUTPUT_LOCATION = GetExecutingDirectoryName() + "/../../../../LicenseIdentifiers/" + LicenseIdentifierFileComponents.NAME;
        private static readonly string NUSPEC_OUTPUT_LOCATION = GetExecutingDirectoryName() + "/../../../../LicenseIdentifiers/LicenseIdentifiers.nuspec";

        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "License-Identifiers");

            var spdxReleaseMetadata = await GetLatestSpdxReleaseMetadata();
            var spdxRelease = await GetSpdxRelease(spdxReleaseMetadata.TagName);
            GenerateLicensesClass(spdxRelease.Licenses, LICENSES_OUTPUT_LOCATION);

            var versionOverride = args.Length > 0 ? args[0] : null;
            await UpdateNuspecFile(spdxReleaseMetadata, versionOverride);

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

        private static async Task UpdateNuspecFile(Release releaseMetadata, string versionOverride = null)
        {
            var version = string.IsNullOrEmpty(versionOverride) ? releaseMetadata.TagName : versionOverride;
            if (version.StartsWith('v'))
            {
                version = version[1..];
            }

            var nuspecContents = NuspecGenerator.GenerateContent(version, releaseMetadata.ReleaseDescription);
            await File.WriteAllTextAsync(NUSPEC_OUTPUT_LOCATION, nuspecContents);
        }

        public static string GetExecutingDirectoryName()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return new FileInfo(location.AbsolutePath).Directory.FullName;
        }
    }
}
