﻿using EntityFrameworkExtras.EF6;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCloudService
{
    public class WordCloud
    {

        static public void AddString(long cloudKey, string str, DateTime? date = null)
        {
            char[] delimiters = new char[] { ' ', ',', '?', '!', '"', '\'', '.', '[', ']', '(', ')', '\n', '\r', ';' };

            if (date == null) date = DateTime.Now;
            var allWords = str.Split(delimiters, StringSplitOptions.RemoveEmptyEntries)
                .Where(r => r.Length > 2 && r.Length < 50 && r.All(char.IsLetter))
                .Select(r => r.ToLower())
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

        static public void Delete(long cloudKey)
        {
            using (var db = new WordCloudContext())
            {
                SqlParameter cloudKeyParam = new SqlParameter("@wordCloudKey", cloudKey);
                db.Database.ExecuteSqlCommand("[WordCloud].[DeleteWordCloud] @wordCloudKey", cloudKeyParam);
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
                db.Database.Connection.Open();
                var reader = db.Database.ExecuteReader(proc);
                var flatHistogram = ((IObjectContextAdapter)db)
                    .ObjectContext
                    .Translate<WordHistogramFlat>(reader).ToList();

                reader.NextResult();
                var flatCloudKey = ((IObjectContextAdapter)db)
                    .ObjectContext
                    .Translate<WordCloudKeyFlat>(reader).ToList();

                db.Database.Connection.Close();
                return flatHistogram
                    .Select(r => new WordHistogram
                    {
                        Word = r.Word,
                        WordCount = r.WordCount,
                        StringCount = r.StringCount,
                        CloudKeys = flatCloudKey.Where(w => w.Word == r.Word).Select(w => w.CloudKey).ToList()
                    })
                    .OrderByDescending(h => h.StringCount);
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

        [DebuggerDisplay("{Word}")]
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

        private class WordCloudKeyFlat
        {
            public string Word { get; set; }
            public long CloudKey { get; set; }
        }
    }
}
