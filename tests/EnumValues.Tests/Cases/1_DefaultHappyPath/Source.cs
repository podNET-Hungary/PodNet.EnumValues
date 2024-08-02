[Values<ValueAttribute>]
public enum Sentiment
{
    [Value("Green")]  Happy,
    [Value("Yellow")] Neutral,
    [Value("Red")]    Sad
}

public class Comment
{
    public Sentiment Sentiment { get; set; }
    public string SentimentValue => Sentiment.GetValue();
}