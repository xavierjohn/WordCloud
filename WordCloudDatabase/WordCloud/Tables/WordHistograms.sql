﻿CREATE TABLE [WordCloud].[WordHistograms]
(
    [Word] NVARCHAR(50) NOT NULL , 
    [Date] DATE NOT NULL, 
    [StringCount] INT NOT NULL, 
    [WordCount] INT NOT NULL, 
    CONSTRAINT [PK_WordHistograms] PRIMARY KEY ([Word], [Date]) 
)
