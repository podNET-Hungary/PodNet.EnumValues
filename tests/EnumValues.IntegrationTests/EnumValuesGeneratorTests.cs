namespace PodNet.EnumValues.IntegrationTests;

// These tests wouldn't even *compile* correctly if the generated method wasn't found.
// However, the E2E functionality should work as described here. Most other scenarios should be covered by the normal test project.
[TestClass]
public class EnumValuesGeneratorTests
{
    [TestMethod]
    public void SmokeTest()
    {
        Assert.AreEqual("Green", Sentiment.Happy.GetValue());
    }
}