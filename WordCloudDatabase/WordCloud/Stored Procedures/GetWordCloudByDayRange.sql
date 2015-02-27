CREATE PROCEDURE [WordCloud].[GetWordCloudByDayRange]
    @CloudKeys as [WordCloud].[CloudKeyType] readonly,
    @FromDate as Date,
    @ToDate as Date,
    @Limit as INT
AS

DECLARE @WordHistogram [WordCloud].[WordHistogramType];
  INSERT INTO @WordHistogram
  SELECT TOP (@Limit)
      [Word]
      ,sum([StringCount]) as StringCount
      ,sum([WordCount]) as WordCount
  FROM [WordCloud].[WordHistograms] wh
  INNER JOIN [WordCloud].[Words] w on wh.WordKey = w.[WordKey]
  INNER JOIN @CloudKeys ck on wh.CloudKey = ck.CloudKey
  WHERE wh.[Date] between @FromDate and @ToDate
  group by w.Word;

  SELECT * from @WordHistogram order by StringCount desc;

  SELECT w.Word, CloudKey FROM @WordHistogram as v_wh
  INNER JOIN WordCloud.Words as w on v_wh.Word = w.Word
  INNER JOIN WordCloud.WordHistograms as wh on w.[WordKey] = wh.WordKey
RETURN 0
GO
