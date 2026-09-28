#if NETFRAMEWORK

public class ImmutableVersionTests
{
    // work around https://github.com/orgs/VerifyTests/discussions/1366
    [Test]
    public async Task AssertVersion()
    {
        // TUnit pulls a newer System.Collections.Immutable into the test output, so assert
        // on the version the shipped library was compiled against.
        var reference = typeof(AllFiles).Assembly
            .GetReferencedAssemblies()
            .Single(_ => _.Name == "System.Collections.Immutable");
        await Assert.That(reference.Version).IsEqualTo(new Version(8, 0, 0, 0));
    }
}

#endif