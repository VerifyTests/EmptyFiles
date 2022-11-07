static class AssemblyLocation
{
    static AssemblyLocation()
    {
        var assembly = typeof(AssemblyLocation).Assembly;
#if NET5_0_OR_GREATER
        var path = assembly.Location;
#else
        var uri = new UriBuilder(assembly.CodeBase);
        var path = Uri.UnescapeDataString(uri.Path);
#endif

        CurrentDirectory = Path.GetDirectoryName(path)!;
    }

    public static string CurrentDirectory;
}