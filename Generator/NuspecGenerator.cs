using System.Xml.Schema;
using System.Xml.Serialization;

namespace Generator
{
    public class NuspecGenerator
    {
        public static void GenerateFile(string path, string version, string tag)
        {
            var package = new Package
            {
                Metadata = new Metadata
                {
                    Id = "LicenseIdentifiers",
                    Authors = "TobyAndToby",
                    Version = version,
                    Tags = "license licence SPDX OSS parse",
                    License = new LicenseXml
                    {
                        Type = "expression",
                        Text = "ISC"
                    },
                    Repository = new Repository
                    {
                        Url = "https://github.com/TobyAndToby/License-Identifiers",
                    },
                    ReleaseNotes = $"https://github.com/spdx/license-list-data/releases/tag/{tag}",
                    Description = $"SPDX-compliant license enum class. Validate and parse all verified SPDX license identifiers from SPDX version {version}."
                }
            };

            var xmlSerializer = new XmlSerializer(typeof(Package));
            var streamWriter = new StreamWriter(path);

            xmlSerializer.Serialize(streamWriter, package);
            streamWriter.Close();
        }
    }

    [XmlRoot("package")]
    public class Package
    {
        [XmlElement("metadata")]
        public Metadata Metadata { get; init; }
    }

    public class Metadata
    {
        [XmlElement("id")]
        public string Id { get; init; }

        [XmlElement("version")]
        public string Version { get; init; }

        [XmlElement("authors")]
        public string Authors { get; init; }

        [XmlElement("license")]
        public LicenseXml License { get; init; }

        [XmlElement("repository")]
        public Repository Repository { get; init; }

        [XmlElement("tags")]
        public string Tags { get; init; }

        [XmlElement("description")]
        public string Description { get; init; }

        [XmlElement("releaseNotes")]
        public string ReleaseNotes { get; init; }
    }

    public class LicenseXml
    {
        [XmlAttribute("type")]
        public string Type { get; init; }

        [XmlText]
        public string Text { get; init; }
    }

    public class Repository
    {
        [XmlAttribute("url")]
        public string Url { get; init; }
    }
}
