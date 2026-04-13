namespace EmptyFiles;

public class EmptyFile
{
    public string Path { get; }
    public DateTime LastWriteTime { get; }
    public Category Category { get; }
    public string Extension { get; }
    internal string ResourceName { get; }

    internal static EmptyFile Build(string file, Category category, string resourceName)
    {
        var writeTime = File.GetLastWriteTime(file);
        var extension = System.IO.Path.GetExtension(file);
        return new(file, writeTime, category, extension, resourceName);
    }

    public EmptyFile(string path, in DateTime lastWriteTime, in Category category)
        : this(path, lastWriteTime, category, System.IO.Path.GetExtension(path), $"EmptyFiles.{category.ToString().ToLowerInvariant()}.empty{System.IO.Path.GetExtension(path)}")
    {
    }

    EmptyFile(string path, in DateTime lastWriteTime, in Category category, string extension, string resourceName)
    {
        Guard.AgainstNullOrEmpty(path);
        Path = path;
        LastWriteTime = lastWriteTime;
        Category = category;
        Extension = extension;
        ResourceName = resourceName;
    }

    public Stream OpenRead()
    {
        var assembly = typeof(EmptyFile).Assembly;
        return assembly.GetManifestResourceStream(ResourceName) ??
               throw new($"Embedded resource not found: {ResourceName}");
    }
}
