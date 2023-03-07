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
}