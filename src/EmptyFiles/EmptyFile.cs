namespace EmptyFiles;

public class EmptyFile
{
    public string Path
    {
        get
        {
            EnsureExtracted();
            return path!;
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

    string? path;
    DateTime lastWriteTime;
    bool extracted;
    readonly object sync = new();

    internal static EmptyFile Embedded(Category category, string extension, string resourceName) =>
        new(category, extension, resourceName);

    EmptyFile(Category category, string extension, string resourceName)
    {
        Category = category;
        Extension = extension;
        ResourceName = resourceName;
    }

    public EmptyFile(string path, in DateTime lastWriteTime, in Category category)
    {
        Guard.AgainstNullOrEmpty(path);
        this.path = path;
        this.lastWriteTime = lastWriteTime;
        Category = category;
        Extension = System.IO.Path.GetExtension(path);
        ResourceName = $"EmptyFiles.{category.ToString().ToLowerInvariant()}.empty{Extension}";
        extracted = true;
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

            var categoryName = Category.ToString().ToLowerInvariant();
            var directory = System.IO.Path.Combine(AllFiles.ExtractDirectory, categoryName);
            Directory.CreateDirectory(directory);
            var target = System.IO.Path.Combine(directory, $"empty{Extension}");
            var assembly = typeof(EmptyFile).Assembly;
            using var resource = assembly.GetManifestResourceStream(ResourceName) ??
                                 throw new($"Embedded resource not found: {ResourceName}");
            if (!File.Exists(target) || new FileInfo(target).Length != resource.Length)
            {
                using var fileStream = File.Create(target);
                resource.CopyTo(fileStream);
            }

            path = target;
            lastWriteTime = File.GetLastWriteTime(target);
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
