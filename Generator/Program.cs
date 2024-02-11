using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;

namespace Generator
{
    class Program
    {
        private static readonly string LICENSES_OUTPUT_LOCATION = GetProjectFolderPath() + LicenseIdentifierFileComponents.NAME;
        private static readonly string NUSPEC_OUTPUT_LOCATION = GetProjectFolderPath() + "LicenseIdentifiers.nuspec";

        private static readonly HttpClient client = new();

        static async Task Main(string[] args)
        {
            if (!TryParseTagName(args, out var tag))
            {
                throw new ArgumentException("Please provide a valid tag name.");
            }

            var spdxJsonUrl = $"https://raw.githubusercontent.com/spdx/license-list-data/{tag}/json/licenses.json";

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "License-Identifiers");

            var licensesMetadata = await client.GetFromJsonAsync<LicensesList>(spdxJsonUrl);

            if (licensesMetadata == null)
            {
                throw new InvalidOperationException("Failed to retrieve licenses.");
            }

            GenerateLicensesClass(licensesMetadata.Licenses, LICENSES_OUTPUT_LOCATION);

            var version = GetVersion(tag);

            await UpdateNuspecFile(tag, version);
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

        private static async Task UpdateNuspecFile(string tag, string version)
        {
            var nuspecContents = NuspecGenerator.GenerateContent(version, tag);
            await File.WriteAllTextAsync(NUSPEC_OUTPUT_LOCATION, nuspecContents);
        }

        private static string GetVersion(string tag)
        {
            if (!tag.StartsWith('v'))
            {
                return tag;
            }

            return tag[1..];
        }

        public static string GetProjectFolderPath()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return new FileInfo(location.AbsolutePath).Directory.FullName + "/../../../../LicenseIdentifiers/";
        }

        private static bool TryParseTagName(string[] args, [NotNullWhen(true)] out string? tag)
        {
            if (args.Length < 1)
            {
                tag = null;
                return false;
            }

            tag = args[0];
            return !string.IsNullOrWhiteSpace(tag);
        }
    }
}
