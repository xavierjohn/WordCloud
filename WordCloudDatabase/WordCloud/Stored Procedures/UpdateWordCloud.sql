CREATE PROCEDURE [WordCloud].[UpdateWordCloud]
    @CloudKey as INT,
    @Date as Date,
    @WordHistograms as [WordCloud].WordHistogramType readonly
AS
SET NOCOUNT OFF;

-- Filter out the stop words
DECLARE @UnStoppedWords [WordCloud].WordHistogramType;
INSERT INTO @UnStoppedWords ([Word], [StringCount], [WordCount]) 
SELECT [Word], [StringCount], [WordCount] from @WordHistograms as wh where wh.[Word] not in 
        (select Word from [WordCloud].[StopWords]);

--Add new words to create WordKeys
INSERT INTO WordCloud.Words
SELECT Word FROM @UnStoppedWords as wh where wh.[Word] not in 
        (select Word from [WordCloud].[StopWords]
		 union all 
		 select [Word] from WordCloud.Words
		); 

MERGE [WordCloud].WordHistograms as target
using (
select @Date as [Date], @CloudKey as CloudKey, w.[WordKey], usw.[StringCount], usw.[WordCount]
from @UnStoppedWords as usw 
inner join WordCloud.Words as w on usw.[Word] = w.[Word])
as source
on target.[Date] = source.[Date] and target.CloudKey = source.CloudKey and target.[WordKey] = source.[WordKey]

when matched then
    update set 
    StringCount = target.StringCount + source.StringCount,
    WordCount = target.WordCount + source.WordCount

when not matched then
	insert ([Date],[CloudKey], [WordKey], [StringCount], [WordCount])
	values  (source.[Date],source.[CloudKey], source.[WordKey], source.[StringCount], source.[WordCount]);
