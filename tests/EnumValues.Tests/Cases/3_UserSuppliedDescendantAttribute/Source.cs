public class ColorAttribute(string value) : ValueAttribute(value);

[Values<ColorAttribute>] // 👈 "ColorAttribute" instead of "ValueAttribute"
public enum Sentiment
{
    [Color("Green")]  Happy,   // 👈 \
    [Color("Yellow")] Neutral, // 👈  | [Color] instead of [Value]
    [Color("Red")]    Sad      // 👈 /
}
