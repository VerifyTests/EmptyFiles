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

    public static void AgainstEmpty(CharSpan value, [CallerArgumentExpression("value")] string argumentName = "")
    {
        if (value.Length == 0)
        {
            throw new ArgumentNullException(argumentName);
        }
    }
    public static void ValidExtension(CharSpan extension, [CallerArgumentExpression("extension")] string argumentName = "")
    {
        if (extension.Length == 0)
        {
            throw new ArgumentNullException(argumentName);
        }
        if (extension[0] != '.')
        {
            throw new ArgumentNullException(argumentName, $"Extension must begin with a period. Value: {extension}");
        }
    }
    public static void ValidExtension(string extension, [CallerArgumentExpression("extension")] string argumentName = "")
    {
        if (extension.Length == 0)
        {
            throw new ArgumentNullException(argumentName);
        }
        if (extension[0] != '.')
        {
            throw new ArgumentNullException(argumentName, $"Extension must begin with a period. Value: {extension}");
        }
    }
}