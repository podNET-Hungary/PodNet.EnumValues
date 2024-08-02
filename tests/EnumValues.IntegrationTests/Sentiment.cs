namespace PodNet.EnumValues.IntegrationTests;

[Values<ValueAttribute>]
public enum Sentiment
{
    [Value("Green")] Happy,
    [Value("Yellow")] Neutral,
    [Value("Red")] Sad
}