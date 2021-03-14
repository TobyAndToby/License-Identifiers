using System;
using System.IO;
using System.Reflection;

namespace Generator
{
    class Program
    {
        private const string OUTPUT_LOCATION = "../../../../LicenseIdentifiers/" + FileComponents.NAME;
        private static readonly string licensesUri = GetExecutingDirectoryName() + "/../../../../../license-list-data/json/licenses.json";

        static void Main(string[] args)
        {
            var data = Utils.ParseJson<LicensesList>(licensesUri);
            GenerateLicensesClass(data, OUTPUT_LOCATION);
        }

        public static string GetExecutingDirectoryName()
        {
            var location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            return new FileInfo(location.AbsolutePath).Directory.FullName;
        }

        private static void GenerateLicensesClass(LicensesList data, string destination)
        {
            using (StreamWriter outputFile = new StreamWriter(destination))
            {
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
}
