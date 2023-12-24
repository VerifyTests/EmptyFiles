static class AssemblyLocation
{
    static AssemblyLocation()
    {
        var assembly = typeof(AssemblyLocation).Assembly;
#if NET5_0_OR_GREATER
        Path = assembly.Location;
#else
        var uri = new UriBuilder(assembly.CodeBase);
        Path = Uri.UnescapeDataString(uri.Path);
#endif

        Directory = System.IO.Path.GetDirectoryName(Path)!;
    }

    public static string Path;

    public static string Directory;
}