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
        static char[] _Delimiters = new char[] { ' ', ',', '?', '!', '"', '\'', '.' };

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
                        WordHistograms = allWords.Select(r => new WordHistogramType() { Word = r.Word.ToLower(), StringCount = 1, WordCount = r.WordCount }).ToList()
                    };
                    db.Database.ExecuteStoredProcedure(proc);
                }
            }
        }

        static public IEnumerable<WordHistogram> GetWordCloudByDayRange(long cloudKey, DateTime fromDate, DateTime toDate, int limit)
        {
            return GetWordCloudByDayRange(new[] { cloudKey }, fromDate, toDate, limit);
        }

        static public IEnumerable<WordHistogram> GetWordCloudByDayRange(IEnumerable<long> cloudKeys, DateTime fromDate, DateTime toDate, int limit)
        {
            using (var db = new WordCloudContext())
            {
                var proc = new GetWordCloudByDayRangeProcedure()
                {
                    CloudKeys = cloudKeys.Select(r => new CloudKeyType() { CloudKey = r }).ToList(),
                    FromDate = fromDate,
                    ToDate = toDate,
                    Limit = limit
                };
                var flatHistogram = db.Database.ExecuteStoredProcedure<WordHistogramFlat>(proc);
                return flatHistogram
                    .GroupBy(r => new { r.Word, r.StringCount, r.WordCount }, r => r.CloudKey)
                    .Select(g => new WordHistogram { Word = g.Key.Word, WordCount = g.Key.WordCount, StringCount = g.Key.StringCount, CloudKeys = g });
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
            public List<WordHistogramType> WordHistograms { get; set; }
        }

        [UserDefinedTableType("[WordCloud].[CloudKeyType]")]
        private class CloudKeyType
        {
            [UserDefinedTableTypeColumn(1)]
            public long CloudKey { get; set; }
        }

        [StoredProcedure("[WordCloud].[GetWordCloudByDayRange]")]
        private class GetWordCloudByDayRangeProcedure
        {
            [StoredProcedureParameter(SqlDbType.Udt)]
            public List<CloudKeyType> CloudKeys { get; set; }

            [StoredProcedureParameter(SqlDbType.Date)]
            public DateTime FromDate { get; set; }

            [StoredProcedureParameter(SqlDbType.Date)]
            public DateTime ToDate { get; set; }

            [StoredProcedureParameter(SqlDbType.Int)]
            public int Limit { get; set; }
        }

        [UserDefinedTableType("[WordCloud].[WordHistogramType]")]
        private class WordHistogramType
        {
            [UserDefinedTableTypeColumn(1)]
            public string Word { get; set; }

            [UserDefinedTableTypeColumn(2)]
            public int StringCount { get; set; }

            [UserDefinedTableTypeColumn(3)]
            public int WordCount { get; set; }
        }

        private class WordHistogramFlat
        {
            public long CloudKey { get; set; }
            public string Word { get; set; }
            public int StringCount { get; set; }
            public int WordCount { get; set; }
        }
    }
}
