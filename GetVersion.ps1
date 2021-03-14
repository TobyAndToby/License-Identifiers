$spdxReleasesUrl = "https://api.github.com/repos/spdx/license-list-data/releases";
$spdxReleases = Invoke-RestMethod -Uri $spdxReleasesUrl -ContentType "application/json";
$latestSpdxRelease = $spdxReleases | Select-Object -First 1;
$latestSpdxReleaseVersion = $latestSpdxRelease.tag_name;

Rename-Item "../$latestSpdxReleaseVersion" "license-list-data";

if ($latestSpdxReleaseVersion[0] -eq 'v')
{
	$latestSpdxReleaseVersion = $latestSpdxReleaseVersion.Substring(1);
}

$nugetReleasesUrl = "https://api.nuget.org/v3-flatcontainer/LicenseIdentifiers/index.json";
$nugetReleases = Invoke-RestMethod -Uri $nugetReleasesUrl -ContentType "application/json";
$latestNugetReleaseVersion = $nugetReleases.versions | Select-Object -Last 1

Write-Host "Latest SPDX Release: $latestSpdxReleaseVersion";
Write-Host "Latest NuGet Release: $latestNugetReleaseVersion";

if ($latestSpdxReleaseVersion -eq $latestNugetReleaseVersion)
{
	Write-Error "Version $latestSpdxReleaseVersion already published";
}else
{
	Write-Host "##vso[task.setvariable variable=latestVersion]$latestSpdxReleaseVersion"
}
