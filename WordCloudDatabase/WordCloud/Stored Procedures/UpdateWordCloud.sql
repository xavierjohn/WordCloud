CREATE PROCEDURE [WordCloud].[UpdateWordCloud]
    @WordHistograms as [WordCloud].WordHistogramType readonly
AS

SET NOCOUNT OFF;
MERGE [WordCloud].WordHistograms as target
using (select * from @WordHistograms as wh where wh.[Word] not in 
        (select Word from [WordCloud].[StopWords])) as source
on target.[Word] = source.[Word] and target.[Date] = source.[Date]
when matched then
    update set 
    StringCount = target.StringCount + source.StringCount,
    WordCount = target.WordCount + source.WordCount

when not matched then
	insert ([Word], [Date], [StringCount], [WordCount])
	values  ( source.[Word], source.[Date], source.[StringCount], source.[WordCount]);
