CREATE TYPE [WordCloud].[WordHistogramType] AS TABLE
(
    [Word] NVARCHAR(50) NOT NULL , 
    [Date] DATE NOT NULL, 
    [StringCount] INT NOT NULL, 
    [WordCount] INT NOT NULL 
)
