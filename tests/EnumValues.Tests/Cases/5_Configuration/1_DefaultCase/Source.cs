namespace App.Comments;

public class EmojiAttribute(string value) : ValueAttribute(value);

[Values<EmojiAttribute>(                    // 👈 All parameters are optional
    Accessibility = Accessibility.Internal, // 👈 Defaults to the enum's visibility, but can be public or internal
    Namespace = "App.Helpers",              // 👈 Defaults to the enum's namespace (would be "App.Comments" here), overriding this implies you'll need to import the namespace yourself at the usage sites
    ClassName = "EmojiExtensions",          // 👈 The default is "{EnumName}ValueExtensions"
    MethodName = "GetEmojiForSentiment",    // 👈 The default is "Get{TValue}", in this case would be "GetEmoji"
    MissingValueHandling = MissingValueHandling.EmptyString, // 👈 Default is to throw when you forget to add [Emoji] to a member (also warns if a value is missing when this is set to throw)
    UndefinedValueHandling = UndefinedValueHandling.ThrowMissingValueException // 👈 Default is "RawValueToString", which would return the raw int value if it wasn't Happy (0), Neutral (1) or Sad (2)
)]
public enum Sentiment
{
    [Emoji("😄")]  Happy,
    [Emoji("🙄")] Neutral,
    [Emoji("😔")]    Sad
}