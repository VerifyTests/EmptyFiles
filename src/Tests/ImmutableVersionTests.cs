#if NETFRAMEWORK

public class ImmutableVersionTests
{
    // work around https://github.com/orgs/VerifyTests/discussions/1366
    [Test]
    public void AssertVersion()
    {
        var assemblyName = typeof(ImmutableDictionary).Assembly.GetName();
        AreEqual(new Version(8, 0, 0, 0), assemblyName.Version);
    }
}

#endif