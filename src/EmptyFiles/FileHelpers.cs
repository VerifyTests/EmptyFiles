using System.IO;

static class FileHelpers
{
    public static string Extension(string path)
    {
        return Path.GetExtension(path).Substring(1);
    }
}