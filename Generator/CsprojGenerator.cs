using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    public class CsprojGenerator
    {
        public static string GenerateContent(string version)
        {
            return @$"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Version>{version}</Version>
  </PropertyGroup>
</Project>";
        }
    }
}
