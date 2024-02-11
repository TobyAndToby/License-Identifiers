namespace Generator
{
    public class NuspecGenerator
    {
        public static string GenerateContent(string version, string tag)
        {
            return @$"<?xml version=""1.0"" encoding=""utf-8""?>
<package>
  <metadata>
    <id>LicenseIdentifiers</id>
    <version>{version}</version>
    <authors>TobyAndToby</authors>
    <requireLicenseAcceptance>true</requireLicenseAcceptance>
    <license type=""expression"">ISC</license>
    <projectUrl>http://github.com/TobyAndToby/License-Identifiers</projectUrl>
    <description>SPDX-compliant license enum class. Validate and parse all verified SPDX license identifiers from SPDX version {version}.</description>
    <releaseNotes>https://github.com/spdx/license-list-data/releases/tag/{tag}</releaseNotes>
    <tags>license,licence,SPDX,OSS,parse</tags>
  </metadata>
</package>";
        }
    }
}
