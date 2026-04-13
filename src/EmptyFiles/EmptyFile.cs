namespace EmptyFiles;

public class EmptyFile
{
    public string Path
    {
        get
        {
            EnsureExtracted();
            return path;
        }
    }

    public DateTime LastWriteTime
    {
        get
        {
            EnsureExtracted();
            return lastWriteTime;
        }
    }

    public Category Category { get; }
    public string Extension { get; }
    internal string ResourceName { get; }

    string path;
    DateTime lastWriteTime;
    bool extracted;
    readonly object sync = new();

    internal static EmptyFile Embedded(string targetPath, Category category, string extension, string resourceName) =>
        new(targetPath, category, extension, resourceName, extracted: false, lastWriteTime: default);

    EmptyFile(string path, Category category, string extension, string resourceName, bool extracted, DateTime lastWriteTime)
    {
        Guard.AgainstNullOrEmpty(path);
        this.path = path;
        Category = category;
        Extension = extension;
        ResourceName = resourceName;
        this.extracted = extracted;
        this.lastWriteTime = lastWriteTime;
    }

    public EmptyFile(string path, in DateTime lastWriteTime, in Category category)
        : this(
            path,
            category,
            System.IO.Path.GetExtension(path),
            $"EmptyFiles.{category.ToString().ToLowerInvariant()}.empty{System.IO.Path.GetExtension(path)}",
            extracted: true,
            lastWriteTime: lastWriteTime)
    {
    }

    void EnsureExtracted()
    {
        if (extracted)
        {
            return;
        }

        lock (sync)
        {
            if (extracted)
            {
                return;
            }

            var directory = System.IO.Path.GetDirectoryName(path)!;
            Directory.CreateDirectory(directory);
            var assembly = typeof(EmptyFile).Assembly;
            using var resource = assembly.GetManifestResourceStream(ResourceName) ??
                                 throw new($"Embedded resource not found: {ResourceName}");
            if (!File.Exists(path) || new FileInfo(path).Length != resource.Length)
            {
                using var fileStream = File.Create(path);
                resource.CopyTo(fileStream);
            }

            lastWriteTime = File.GetLastWriteTime(path);
            extracted = true;
        }
    }

    public Stream OpenRead()
    {
        var assembly = typeof(EmptyFile).Assembly;
        return assembly.GetManifestResourceStream(ResourceName) ??
               throw new($"Embedded resource not found: {ResourceName}");
    }
}
