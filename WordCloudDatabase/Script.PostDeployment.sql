/*
Post-Deployment Script Template                            
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.        
 Use SQLCMD syntax to include a file in the post-deployment script.            
 Example:      :r .\myfile.sql                                
 Use SQLCMD syntax to reference a variable in the post-deployment script.        
 Example:      :setvar TableName MyTable                            
               SELECT * FROM [$(TableName)]                    
--------------------------------------------------------------------------------------
*/
declare @words table 
(
	[Word] NVARCHAR(50) NOT NULL
)
SET NOCOUNT OFF;

insert into @words 
values
('for'),
('was'),
('what');

merge [WordCloud].[StopWords] as target
using (select * from @words) as source
on target.word = source.word
when not matched then
insert values (source.word);