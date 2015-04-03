CREATE PROCEDURE [WordCloud].[DeleteWordCloud]
    @CloudKey as BIGINT
AS
    DELETE FROM [WordCloud].WordHistograms WHERE CloudKey = @CloudKey;
RETURN 0
