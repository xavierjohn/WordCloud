CREATE PROCEDURE [WordCloud].[GetWordCloudByDayRange2]
    @CloudKeys as [WordCloud].[CloudKeyType] readonly,
    @FromDate as Date,
    @ToDate as Date,
    @Limit as INT
AS
  SELECT TOP (@Limit) 
      [Word]
      ,sum([StringCount]) as StringCount
      ,sum([WordCount]) as WordCount
  FROM [WordCloud].[WordHistograms] wh
  INNER JOIN [WordCloud].[Words] w on wh.WordKey = w.[Key]
  INNER JOIN @CloudKeys ck on wh.CloudKey = ck.CloudKey
  WHERE wh.[Date] between @FromDate and @ToDate
  group by wh.CloudKey, w.Word
  order by StringCount desc
RETURN 0
GO


