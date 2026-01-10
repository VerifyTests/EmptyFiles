public static class Program
{
    public static int Main()
    {
        try
        {
            // Test APIs that don't require the EmptyFiles directory to exist
            TestFileExtensions();
            TestCategoryEnum();

            Console.WriteLine("All EmptyFiles AOT tests passed!");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Test failed: {ex}");
            return 1;
        }
    }

    static void TestFileExtensions()
    {
        // Test FileExtensions class - this doesn't require any files
        var isTxt = FileExtensions.IsTextExtension(".txt");
        Console.WriteLine($"Is .txt a text extension: {isTxt}");
        if (!isTxt)
        {
            throw new(".txt should be a text extension");
        }

        var isBin = FileExtensions.IsTextExtension(".bin");
        Console.WriteLine($"Is .bin a text extension: {isBin}");
        if (isBin)
        {
            throw new(".bin should not be a text extension");
        }

        var isCs = FileExtensions.IsTextExtension(".cs");
        Console.WriteLine($"Is .cs a text extension: {isCs}");
        if (!isCs)
        {
            throw new(".cs should be a text extension");
        }

        var isPng = FileExtensions.IsTextExtension(".png");
        Console.WriteLine($"Is .png a text extension: {isPng}");
        if (isPng)
        {
            throw new(".png should not be a text extension");
        }

        // Test adding custom extensions
        FileExtensions.AddTextExtension(".mytext");
        var isMyText = FileExtensions.IsTextExtension(".mytext");
        Console.WriteLine($"Is .mytext a text extension after adding: {isMyText}");
        if (!isMyText)
        {
            throw new(".mytext should be a text extension after adding");
        }
    }

    static void TestCategoryEnum()
    {
        // Test Category enum - verify AOT handles enum correctly
        var categories = Enum.GetValues<Category>();
        Console.WriteLine($"Category enum values count: {categories.Length}");

        foreach (var category in categories)
        {
            Console.WriteLine($"  Category: {category}");
        }

        // Verify all expected categories exist
        if (!Enum.IsDefined(Category.Archive))
        {
            throw new("Archive category should be defined");
        }

        if (!Enum.IsDefined(Category.Document))
        {
            throw new("Document category should be defined");
        }

        if (!Enum.IsDefined(Category.Image))
        {
            throw new("Image category should be defined");
        }
    }
}
