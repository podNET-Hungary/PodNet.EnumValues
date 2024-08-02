Assert.AreEqual("Green", Sentiment.Happy.GetValue());
Assert.AreEqual("Yellow", Sentiment.Neutral.GetValue()); 
Assert.AreEqual("Red", Sentiment.Sad.GetValue());
Assert.AreEqual("4", ((Sentiment)4).GetValue());
Assert.AreEqual("Green", new Comment { Sentiment = Sentiment.Happy }.SentimentValue);