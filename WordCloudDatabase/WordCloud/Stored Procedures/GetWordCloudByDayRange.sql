CREATE PROCEDURE WordCloud.[GetWordCloudByDayRange]
    @CloudKey as INT,
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
  WHERE wh.CloudKey = @CloudKey and
  wh.[Date] between @FromDate and @ToDate
  group by wh.CloudKey, w.Word

RETURN 0
