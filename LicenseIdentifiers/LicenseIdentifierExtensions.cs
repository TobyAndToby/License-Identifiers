using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LicenseIdentifiers
{
    public partial class LicenseIdentifier
    {
        public static bool TryParse(string licenseId, out LicenseIdentifier license)
        {
            var identifierType = typeof(LicenseIdentifier);

            license = identifierType
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(f => f.FieldType == identifierType)
                .Select(f => f.GetValue(null))
                .Cast<LicenseIdentifier>()  
                .FirstOrDefault(f => f.LicenseId.ToLower() == licenseId.ToLower());

            return license != null;
        }
    }
}
