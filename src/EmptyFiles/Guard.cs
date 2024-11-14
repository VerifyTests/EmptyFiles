static class Guard
{
    public static void FileExists(string path, [CallerArgumentExpression("path")] string argumentName = "")
    {
        AgainstNullOrEmpty(argumentName, path);
        if (!File.Exists(path))
        {
            throw new ArgumentException($"File not found. Path: {path}");
        }
    }

    public static void AgainstNullOrEmpty(string value, [CallerArgumentExpression("value")] string argumentName = "")
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentNullException(argumentName);
        }
    }

    public static string ValidExtension(string extension, [CallerArgumentExpression(nameof(extension))] string argumentName = "")
    {
        if (extension.Length == 0)
        {
            throw new ArgumentNullException(argumentName);
        }

        if (extension.StartsWith('.'))
        {
            return extension;
        }

        return $".{extension}";
    }
}