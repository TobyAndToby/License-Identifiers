function Download-Release {
	param (
		[string]$url,
		[string]$destination
	);

	# create temp with zip extension (or Expand will complain)
	$tmp = New-TemporaryFile | Rename-Item -NewName { $_ -replace 'tmp$', 'zip' } -PassThru;
	#download
	Invoke-WebRequest -OutFile $tmp -Uri $url;
	#exract to same folder 
	$tmp | Expand-Archive -DestinationPath $destination -Force;
	# remove temporary file
	$tmp | Remove-Item;
}

# Get SPDX Latest Release Version
$spdxReleasesUrl = "https://api.github.com/repos/spdx/license-list-data/releases";
$spdxReleases = Invoke-RestMethod -Uri $spdxReleasesUrl -ContentType "application/json";
$latestSpdxRelease = $spdxReleases | Select-Object -First 1;
$latestSpdxReleaseVersion = $latestSpdxRelease.tag_name;

# Get SPDX Latest Release

$latestSpdxReleaseDataUrl = "https://github.com/spdx/license-list-data/archive/$latestSpdxReleaseVersion.zip";
Download-Release $latestSpdxReleaseDataUrl "../license-list-data-parent";

if ($latestSpdxReleaseVersion[0] -eq 'v')
{
	$latestSpdxReleaseVersion = $latestSpdxReleaseVersion.Substring(1);
}

Move-Item -Path "../license-list-data-parent/license-list-data-$latestSpdxReleaseVersion" -Destination "../license-list-data";

$nugetReleasesUrl = "https://api.nuget.org/v3-flatcontainer/LicenseIdentifiers/index.json";
$nugetReleases = Invoke-RestMethod -Uri $nugetReleasesUrl -ContentType "application/json";
$latestNugetReleaseVersion = $nugetReleases.versions | Select-Object -Last 1;

Write-Host "Latest SPDX Release: $latestSpdxReleaseVersion";
Write-Host "Latest NuGet Release: $latestNugetReleaseVersion";

if ($latestSpdxReleaseVersion -eq $latestNugetReleaseVersion)
{
	Write-Error "Version $latestSpdxReleaseVersion already published";
}else
{
	Write-Host "##vso[task.setvariable variable=latestVersion]$latestSpdxReleaseVersion";
}