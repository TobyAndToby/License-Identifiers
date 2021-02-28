using System.Collections.Generic;

namespace LicenseIdentifiers
{
    public class LicenseIdentifier
    {
		public static readonly LicenseIdentifier GPL2_0WithClasspathException = new LicenseIdentifier("./ GPL - 2.0 - with - classpath - exception.json", true, "./ GPL - 2.0 - with - classpath - exception.html", 0, "GNU General Public License v2.0 w / Classpath exception", "GPL-2.0-with-classpath-exception", new List<string> { "https://www.gnu.org/software/classpath/license.html" }, false);
		public static readonly LicenseIdentifier APSL1_1 = new LicenseIdentifier("./APSL-1.1.json", false, "./APSL-1.1.html", 1, "Apple Public Source License 1.1", "APSL-1.1", new List<string> { "http://www.opensource.apple.com/source/IOSerialFamily/IOSerialFamily-7/APPLE_LICENSE" }, true);
		public static readonly LicenseIdentifier OLDAP2_8 = new LicenseIdentifier("./OLDAP-2.8.json", false, "./OLDAP-2.8.html", 2, "Open LDAP Public License v2.8", "OLDAP-2.8", new List<string> { "http://www.openldap.org/software/release/license.html" }, true);

        private LicenseIdentifier(string reference, bool isDeprecatedLicenseId, string detailsUrl, int referenceNumber, string name, string licenseId, List<string> seeAlso, bool isOsiApproved)
        {
            Reference = reference;
            IsDeprecatedLicenseId = isDeprecatedLicenseId;
            DetailsUrl = detailsUrl;
            ReferenceNumber = referenceNumber;
            Name = name;
            LicenseId = licenseId;
            SeeAlso = seeAlso;
            IsOsiApproved = isOsiApproved;
        }

        public string Reference { get; set; }
        public bool IsDeprecatedLicenseId { get; set; }
        public string DetailsUrl { get; set; }
        public int ReferenceNumber { get; set; }
        public string Name { get; set; }
        public string LicenseId { get; set; }
        public List<string> SeeAlso { get; set; }
        public bool IsOsiApproved { get; set; }
    }
}
