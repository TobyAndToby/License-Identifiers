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

            var allLicenses = identifierType
                .GetFields(BindingFlags.Static | BindingFlags.Public)
                .Where(t => t.FieldType == identifierType);

            foreach (var licenseField in allLicenses)
            {
                var value = (LicenseIdentifier) licenseField.GetValue(null);

                if (value.LicenseId.ToLower() == licenseId.ToLower())
                {
                    license = value;
                    return true;
                }
            }

            license = null;
            return false;
        }
    }
}
