using App.Comments;

var type = typeof(Sentiment).Assembly.DefinedTypes.SingleOrDefault(t => t.Name == "EmojiExtensions");
Assert.IsNotNull(type);
Assert.IsTrue(type is { IsPublic: false }, "Extensions class wasn't correct shape (wasn't internal)");
Assert.IsTrue(type.GetMethod("GetEmojiForSentiment", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static) is { IsPublic: false } method, "Extension method wasn't correct shape (wasn't internal)");
Assert.AreEqual("😄", method.Invoke(null, [Sentiment.Happy])); // Cannot call the method directly, as it's internal, duh

try { 
    method.Invoke(null, [(Sentiment)10]);
}
catch (System.Reflection.TargetInvocationException ex) when (ex.InnerException is MissingEnumValueException)
{
    /* This is the expected path */ 
}
