using System.IO;

namespace Generator
{
    class Program
    {
        private const string LICENSES_URI = "../../../../../license-list-data/json/licenses.json";
        private const string OUTPUT_LOCATION = "../../../../LicenseIdentifiers/LicenseIdentifier.cs";

        static void Main(string[] args)
        {
            var data = Utils.ParseJson<LicensesList>(LICENSES_URI);
            GenerateLicensesClass(data, OUTPUT_LOCATION);
        }

        private static void GenerateLicensesClass(LicensesList data, string destination)
        {
            using (StreamWriter outputFile = new StreamWriter(destination))
            {
                outputFile.Write(FileComponents.HEADER);

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
}
