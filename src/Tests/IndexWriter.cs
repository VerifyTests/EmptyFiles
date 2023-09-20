using EmptyFiles;

public class IndexWriter
{
    static List<KeyValuePair<string, EmptyFile>> files = null!;

    [ModuleInitializer]
    public static void Init() =>
        files = AllFiles.Files.OrderBy(_ => _.Key).ToList();

    [Test]
    public void CreateIndex() =>
        InnerCreateIndex();

    static void InnerCreateIndex([CallerFilePath] string filePath = "")
    {
        var rootDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(filePath)!, @"..\..\"));
        var indexPath = Path.Combine(rootDirectory, "index");
        Directory.CreateDirectory(indexPath);
        foreach (var toDelete in Directory.EnumerateFiles(indexPath))
        {
            File.Delete(toDelete);
        }

        foreach (var (key, value) in files)
        {
            File.Copy(value.Path, Path.Combine(indexPath, $"empty.{key}"));
        }
    }
}