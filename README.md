# WordCloud
Word Cloud backend that supports multiple word cloud over date ranges.

It keeps tracks of total words and the number of strings the word appeared.

Word Cloud can be created by providing a CloudKey and a string.

Example:
```csharp
    WordCloud.AddString(1, "How are you?");
    WordCloud.AddString(1, "How are you? Where do you come from?");
    WordCloud.AddString(2, "What was your score?");
    var words = WordCloud.GetWordCloudByDayRange(1, DateTime.Now, DateTime.Now, 100).ToDictionary(r => r.Word);
    Assert.AreEqual(2, words["how"].WordCount);
    Assert.AreEqual(1, words["where"].WordCount);
    Assert.AreEqual(3, words["you"].WordCount);
    Assert.AreEqual(2, words["you"].StringCount);
```

Example of getting word cloud in different date range.

```csharp
    var startDate = new DateTime(2015, 2, 1);
    var endDate = new DateTime(2015, 2, 2);
    var today = DateTime.Now;
    WordCloud.AddString(1, "How are you?", startDate);
    WordCloud.AddString(1, "How are you? Where do you come from?", endDate);

    words = WordCloud.GetWordCloudByDayRange(1, startDate, endDate, 100).ToDictionary(r => r.Word);
    Assert.AreEqual(2, words["how"].WordCount);

    words = WordCloud.GetWordCloudByDayRange(1, endDate, endDate, 100).ToDictionary(r => r.Word);
    Assert.AreEqual(1, words["how"].WordCount);

```
