using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    public class LicensesList
    {
        public List<License> Licenses { get; set; }
    }

    public class License
    {
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
