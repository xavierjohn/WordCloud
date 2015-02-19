using EntityFrameworkExtras.EF6;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCloudService
{
    public class WordCloud
    {
        static char[] _Delimiters = new char[] { ' ', ',', '?', '!', '"','\'', '.' };

        static public void AddString(long cloudKey, string str, DateTime? date = null)
        {
            if (date == null) date = DateTime.Now;
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
                        CloudKey = cloudKey,
                        Date = date.Value,
                        WordHistograms = allWords.Select(r => new WordHistogram() { Word = r.Word.ToLower(), StringCount = 1, WordCount = r.WordCount }).ToList()
                    };
                    db.Database.ExecuteStoredProcedure(proc);

                }
            }
        }

        static public IList<WordHistogram> GetWordCloudByDayRange(long cloudKey, DateTime fromDate, DateTime toDate, int limit) 
        {
                using (var db = new WordCloudContext())
                {
                    return db.Database.SqlQuery<WordHistogram>("[WordCloud].[GetWordCloudByDayRange] @CloudKey, @FromDate, @ToDate, @Limit",
                        new SqlParameter("CloudKey", cloudKey),
                        new SqlParameter("FromDate", fromDate),
                        new SqlParameter("ToDate", toDate),
                        new SqlParameter("Limit", limit)
                        ).ToList();
                }
        }

        [StoredProcedure("[WordCloud].[UpdateWordCloud]")]
        private class UpdateWordCloudProcedure
        {
            [StoredProcedureParameter(SqlDbType.BigInt)]
            public long CloudKey { get; set; }

            [StoredProcedureParameter(SqlDbType.Date)]
            public DateTime Date { get; set; }

            [StoredProcedureParameter(SqlDbType.Udt)]
            public List<WordHistogram> WordHistograms { get; set; }
        }

        [StoredProcedure("[WordCloud].[GetWordCloudByDayRange]")]
        private class GetWordCloudByDayRangeProcedure
        {
            [StoredProcedureParameter(SqlDbType.BigInt)]
            public long CloudKey { get; set; }

            [StoredProcedureParameter(SqlDbType.Date)]
            public DateTime FromDate { get; set; }

            [StoredProcedureParameter(SqlDbType.Date)]
            public DateTime ToDate { get; set; }

            [StoredProcedureParameter(SqlDbType.Int)]
            public int Limit { get; set; }

            [StoredProcedureParameter(SqlDbType.Udt, Direction = ParameterDirection.Output)]
            public List<WordHistogram> WordHistograms { get; set; }
        }
    }
}
