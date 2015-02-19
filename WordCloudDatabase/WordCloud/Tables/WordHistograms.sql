CREATE TABLE [WordCloud].[WordHistograms]
(
    [Date] DATE NOT NULL,
    [CloudKey] BIGINT NOT NULL,
    [WordKey] BIGINT NOT NULL , 
    [StringCount] INT NOT NULL, 
    [WordCount] INT NOT NULL, 
    CONSTRAINT [PK_WordHistograms] PRIMARY KEY ([Date], [CloudKey], [WordKey]) 
)
