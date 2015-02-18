using EntityFrameworkExtras.EF6;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCloudService
{
    public class WordCloud
    {
        static char[] _Delimiters = new char[] { ' ', ',', '?', '!', '"','\'', '.' };

        static public void AddString(string str)
        {
            var allWords = str.Split(_Delimiters, StringSplitOptions.RemoveEmptyEntries)
                .Where(r => r.Length > 2)
                .GroupBy(r => r)
                .Select(g => new { Word = g.Key, WordCount = g.Count() });

            if (allWords.Count() > 0)
            {
                using (var db = new WordCloudContext())
                {
                    var proc = new UpdateWordCloudProcedure()
                    {
                        WordHistograms = allWords.Select(r => new WordHistogram() { Word = r.Word, Date = DateTimeOffset.Now, StringCount = 1, WordCount = r.WordCount } ).ToList()
                    };
                    db.Database.ExecuteStoredProcedure(proc);

                }
            }
        }

        [UserDefinedTableType("[WordCloud].[WordHistogramType]")]
        public class WordHistogram
        {
            [UserDefinedTableTypeColumn(1)]
            public string Word { get; set; }

            [UserDefinedTableTypeColumn(2)]
            public DateTimeOffset Date { get; set; }

            [UserDefinedTableTypeColumn(3)]
            public int StringCount { get; set; }

            [UserDefinedTableTypeColumn(4)]
            public int WordCount { get; set; }
        }

        [StoredProcedure("[WordCloud].[UpdateWordCloud]")]
        public class UpdateWordCloudProcedure
        {
            [StoredProcedureParameter(SqlDbType.Udt)]
            public List<WordHistogram> WordHistograms { get; set; }
        }

    }
}
