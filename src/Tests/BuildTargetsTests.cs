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

        var emptyFilesDir = Path.Combine(temp.Path, "bin", "Release", "net10.0", "EmptyFiles");
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

        var emptyFilesDir = Path.Combine(temp.Path, "bin", "Release", "net10.0", "EmptyFiles");
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
                <TargetFramework>net10.0</TargetFramework>
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

        TestContext.Out.WriteLine("STDOUT:");
        TestContext.Out.WriteLine(stdout);
        TestContext.Out.WriteLine("STDERR:");
        TestContext.Out.WriteLine(stderr);

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
        var solutionDir = SolutionDirectoryFinder.Find();
        var nugetsDir = Path.GetFullPath(Path.Combine(solutionDir, "..", "nugets"));
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

sealed class TempDirectory : IDisposable
{
    public string Path { get; }

    public TempDirectory()
    {
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
        Directory.CreateDirectory(Path);
    }

    public void Dispose()
    {
        if (!Directory.Exists(Path))
        {
            return;
        }

        try
        {
            Directory.Delete(Path, true);
        }
        catch (IOException ex)
        {
            TestContext.Out.WriteLine($"Failed to delete temp directory: {Path}");
            TestContext.Out.WriteLine($"Exception: {ex.Message}");
            LogLockedFiles(Path);
            throw;
        }
    }

    static void LogLockedFiles(string directory)
    {
        TestContext.Out.WriteLine("Scanning for locked files...");

        foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
        {
            if (IsFileLocked(file))
            {
                TestContext.Out.WriteLine($"LOCKED: {file}");
                var processes = GetLockingProcesses(file);
                foreach (var proc in processes)
                {
                    TestContext.Out.WriteLine($"  Locked by: {proc}");
                }
            }
        }
    }

    static bool IsFileLocked(string filePath)
    {
        try
        {
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            return false;
        }
        catch (IOException)
        {
            return true;
        }
        catch (UnauthorizedAccessException)
        {
            return true;
        }
    }

    static List<string> GetLockingProcesses(string filePath)
    {
        var result = new List<string>();

        var res = RmStartSession(out var sessionHandle, 0, Guid.NewGuid().ToString());
        if (res != 0)
        {
            result.Add($"(Failed to start Restart Manager session: error {res})");
            return result;
        }

        try
        {
            string[] resources = [filePath];
            res = RmRegisterResources(sessionHandle, (uint)resources.Length, resources, 0, null, 0, null);
            if (res != 0)
            {
                result.Add($"(Failed to register resource: error {res})");
                return result;
            }

            uint procInfoNeeded = 0;
            uint procInfo = 0;
            uint rebootReasons = 0;

            res = RmGetList(sessionHandle, out procInfoNeeded, ref procInfo, null, ref rebootReasons);
            if (res == ERROR_MORE_DATA && procInfoNeeded > 0)
            {
                var processInfo = new RM_PROCESS_INFO[procInfoNeeded];
                procInfo = procInfoNeeded;

                res = RmGetList(sessionHandle, out procInfoNeeded, ref procInfo, processInfo, ref rebootReasons);
                if (res == 0)
                {
                    for (var i = 0; i < procInfo; i++)
                    {
                        try
                        {
                            var proc = Process.GetProcessById(processInfo[i].Process.dwProcessId);
                            result.Add($"PID {proc.Id}: {proc.ProcessName} ({proc.MainModule?.FileName ?? "unknown path"})");
                        }
                        catch
                        {
                            result.Add($"PID {processInfo[i].Process.dwProcessId}: {processInfo[i].strAppName} (process no longer running or inaccessible)");
                        }
                    }
                }
            }
            else if (res == 0 && procInfoNeeded == 0)
            {
                result.Add("(No processes found via Restart Manager - file may be locked by system)");
            }
        }
        finally
        {
            RmEndSession(sessionHandle);
        }

        if (result.Count == 0)
        {
            result.Add("(Unable to determine locking process)");
        }

        return result;
    }

    const int ERROR_MORE_DATA = 234;

    [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
    static extern int RmStartSession(out uint pSessionHandle, int dwSessionFlags, string strSessionKey);

    [DllImport("rstrtmgr.dll")]
    static extern int RmEndSession(uint pSessionHandle);

    [DllImport("rstrtmgr.dll", CharSet = CharSet.Unicode)]
    static extern int RmRegisterResources(uint pSessionHandle, uint nFiles, string[]? rgsFilenames, uint nApplications, RM_UNIQUE_PROCESS[]? rgApplications, uint nServices, string[]? rgsServiceNames);

    [DllImport("rstrtmgr.dll")]
    static extern int RmGetList(uint dwSessionHandle, out uint pnProcInfoNeeded, ref uint pnProcInfo, [In, Out] RM_PROCESS_INFO[]? rgAffectedApps, ref uint lpdwRebootReasons);

    [StructLayout(LayoutKind.Sequential)]
    struct RM_UNIQUE_PROCESS
    {
        public int dwProcessId;
        public System.Runtime.InteropServices.ComTypes.FILETIME ProcessStartTime;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct RM_PROCESS_INFO
    {
        public RM_UNIQUE_PROCESS Process;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string strAppName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string strServiceShortName;
        public int ApplicationType;
        public uint AppStatus;
        public uint TSSessionId;
        [MarshalAs(UnmanagedType.Bool)]
        public bool bRestartable;
    }
}
