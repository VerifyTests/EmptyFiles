static class Size
{
    static string[] SizeSuffixes = { "bytes", "KB", "MB"};

    public static string Suffix(long value)
    {
        var mag = (int) Math.Log(value, 1024);

        var adjustedSize = (decimal) value / (1L << (mag * 10));

        if (Math.Round(adjustedSize, 1) >= 1000)
        {
            mag += 1;
            adjustedSize /= 1024;
        }

        return $"{adjustedSize:0.#} {SizeSuffixes[mag]}";
    }
}