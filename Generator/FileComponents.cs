using System;
using System.Collections.Generic;
using System.Text;

namespace Generator
{
    public static class FileComponents
    {   
        public const string HEADER = @"using System.Collections.Generic;

namespace LicenseIdentifiers
{
    public class LicenseIdentifier
    {";

        public static readonly string LICENSE_DEFINITION = Environment.NewLine + "\t\tpublic static readonly LicenseIdentifier {0} = new LicenseIdentifier(\"{1}\", {2}, \"{3}\", \"{4}\", \"{5}\", \"{6}\", new List<string> {{ {7} }}, {8});";

        public const string LICENSE_CONSTRUCTOR = @"

        private LicenseIdentifier(string reference, bool isDeprecatedLicenseId, string detailsUrl, string referenceNumber, string name, string licenseId, List<string> seeAlso, bool isOsiApproved)
        {
            Reference = reference;
            IsDeprecatedLicenseId = isDeprecatedLicenseId;
            DetailsUrl = detailsUrl;
            ReferenceNumber = referenceNumber;
            Name = name;
            LicenseId = licenseId;
            SeeAlso = seeAlso;
            IsOsiApproved = isOsiApproved;
        }";

        public const string LICENSE_PROPERTIES = @"

        public string Reference { get; set; }
        public bool IsDeprecatedLicenseId { get; set; }
        public string DetailsUrl { get; set; }
        public string ReferenceNumber { get; set; }
        public string Name { get; set; }
        public string LicenseId { get; set; }
        public List<string> SeeAlso { get; set; }
        public bool IsOsiApproved { get; set; }";

        public const string FOOTER = @"
    }
}
";
    }
}
