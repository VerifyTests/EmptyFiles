public class BuildTargetsTests
{
    [Test]
    public async Task BuildCopiesEmptyFiles()
    {
        using var temp = new TempDirectory();

        var (nugetSource, packageVersion) = FindPackageInfo();
        WriteCsproj(temp, packageVersion);
        WriteNugetConfig(temp, nugetSource);

        var exitCode = await DotnetBuild(temp);
        That(exitCode, Is.EqualTo(0));

        var emptyFilesDir = Path.Combine(temp.Path, "bin", "Release", "net11.0", "EmptyFiles");
        That(Directory.Exists(emptyFilesDir), Is.True, "EmptyFiles directory should exist after build");
        That(Directory.GetFiles(emptyFilesDir, "*", SearchOption.AllDirectories), Is.Not.Empty, "EmptyFiles directory should contain files");
    }

    [Test]
    public async Task RecoverFromDeletedEmptyFilesDirectory()
    {
        using var temp = new TempDirectory();

        var (nugetSource, packageVersion) = FindPackageInfo();
        WriteCsproj(temp, packageVersion);
        WriteNugetConfig(temp, nugetSource);

        // First build
        var exitCode = await DotnetBuild(temp);
        That(exitCode, Is.EqualTo(0));

        var emptyFilesDir = Path.Combine(temp.Path, "bin", "Release", "net11.0", "EmptyFiles");
        That(Directory.Exists(emptyFilesDir), Is.True, "EmptyFiles directory should exist after first build");

        // Delete EmptyFiles directory from output (leave obj/ intact so marker file survives)
        Directory.Delete(emptyFilesDir, true);
        That(Directory.Exists(emptyFilesDir), Is.False);

        // Second build — should recover
        exitCode = await DotnetBuild(temp);
        That(exitCode, Is.EqualTo(0));

        That(Directory.Exists(emptyFilesDir), Is.True, "EmptyFiles directory should exist after second build");
        That(Directory.GetFiles(emptyFilesDir, "*", SearchOption.AllDirectories), Is.Not.Empty, "EmptyFiles directory should contain files after recovery");
    }

    static void WriteCsproj(TempDirectory temp, string version)
    {
        var csproj = $"""
            <Project Sdk="Microsoft.NET.Sdk">
              <PropertyGroup>
                <TargetFramework>net11.0</TargetFramework>
                <RestorePackagesPath>packages</RestorePackagesPath>
              </PropertyGroup>
              <ItemGroup>
                <PackageReference Include="EmptyFiles" Version="{version}" />
              </ItemGroup>
            </Project>
            """;
        File.WriteAllText(Path.Combine(temp.Path, "Project.csproj"), csproj);
    }

    static void WriteNugetConfig(TempDirectory temp, string nugetSource)
    {
        var config = $"""
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <packageSources>
                <clear />
                <add key="local" value="{nugetSource}" />
              </packageSources>
            </configuration>
            """;
        File.WriteAllText(Path.Combine(temp.Path, "nuget.config"), config);
    }

    static async Task<int> DotnetBuild(TempDirectory temp)
    {
        var startInfo = new ProcessStartInfo("dotnet", "build --configuration Release --disable-build-servers -nodeReuse:false /p:UseSharedCompilation=false")
        {
            WorkingDirectory = temp.Path,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var process = Process.Start(startInfo)!;
        var stdout = await process.StandardOutput.ReadToEndAsync();
        var stderr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        await TestContext.Out.WriteLineAsync("STDOUT:");
        await TestContext.Out.WriteLineAsync(stdout);
        await TestContext.Out.WriteLineAsync("STDERR:");
        await TestContext.Out.WriteLineAsync(stderr);

        return process.ExitCode;
    }

    static (string nugetSource, string packageVersion) FindPackageInfo()
    {
        var nugetSource = FindNugetSource();
        var packageVersion = FindPackageVersion(nugetSource);
        return (nugetSource, packageVersion);
    }

    static string FindNugetSource()
    {
        var nugetsDir = Path.GetFullPath(Path.Combine(ProjectFiles.SolutionDirectory, "..", "nugets"));
        if (Directory.Exists(nugetsDir))
        {
            return nugetsDir;
        }

        throw new InvalidOperationException($"Cannot find nugets directory at {nugetsDir}");
    }

    static string FindPackageVersion(string nugetSource)
    {
        var nupkg = Directory
            .GetFiles(nugetSource, "EmptyFiles.*.nupkg")
            .FirstOrDefault(_ => !Path.GetFileName(_).StartsWith("EmptyFiles.Tool")) ??
                    throw new InvalidOperationException($"Cannot find EmptyFiles nupkg in {nugetSource}");

        var fileName = Path.GetFileNameWithoutExtension(nupkg);
        return fileName["EmptyFiles.".Length..];
    }
}