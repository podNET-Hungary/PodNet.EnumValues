public class SpanishAttribute(string value) : ValueAttribute(value);
public class FrenchAttribute(string value) : ValueAttribute(value);

// Supply both 👇 "SpanishAttribute" and 👇 "FrenchAttribute" to the type
[Values<SpanishAttribute>, Values<FrenchAttribute>]

/*
  You could use multiple attribute lists as well, naturally:
  [Values<SpanishAttribute>]
  [Values<FrenchAttribute>]
  Also note that you can configure all instances differently
*/
public enum Greeting
{
    // 👇 Supply both translations 👇 to each value
    [Spanish("Hola"),          French("Salut")]   Hi,
    [Spanish("Buenos días"),   French("Bonjour")] GoodMorning,
    [Spanish("Buenas noches"), French("Bonsoir")] GoodEvening
}
