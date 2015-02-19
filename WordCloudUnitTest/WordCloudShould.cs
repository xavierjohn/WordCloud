using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WordCloudService;
using System.Linq;
using System.Data.Entity.Migrations;

namespace WordCloudUnitTest
{
    [TestClass]
    public class WordCloudShould
    {
        [TestInitialize]
        public void CleanDb()
        {
            using (var db = new WordCloudContext())
            {
                db.Database.ExecuteSqlCommand("truncate table wordcloud.wordhistograms");
                db.Database.ExecuteSqlCommand("truncate table wordcloud.stopwords");
                db.StopWords.AddOrUpdate(new StopWord() { Word = "for" });
                db.StopWords.AddOrUpdate(new StopWord() { Word = "was" });
                db.SaveChanges();
            }
        }


        [TestMethod]
        public void HaveTheCorrectWordCount()
        {
            WordCloud.AddString(1, "How are you?");
            WordCloud.AddString(1, "How are you? Where do you come from?");
            WordCloud.AddString(2, "What was your score?");
            var words = WordCloud.GetWordCloudByDayRange(1, DateTime.Now, DateTime.Now, 100).ToDictionary(r => r.Word);
            Assert.AreEqual(2, words["how"].WordCount);
            Assert.AreEqual(1, words["where"].WordCount);
            Assert.AreEqual(3, words["you"].WordCount);
            Assert.AreEqual(2, words["you"].StringCount);

            // Should not have items from CloudKey 2
            Assert.IsFalse(words.ContainsKey("what"));
            words = WordCloud.GetWordCloudByDayRange(2, DateTime.Now, DateTime.Now, 100).ToDictionary(r => r.Word);
            Assert.AreEqual(1, words["what"].WordCount);
            Assert.AreEqual(1, words["what"].StringCount);
        }

        [TestMethod]
        public void WorkWithDifferentDateRange()
        {
            var startDate = new DateTime(2015, 2, 1);
            var endDate = new DateTime(2015, 2, 2);
            var today = DateTime.Now;
            WordCloud.AddString(1, "How are you?", startDate);
            WordCloud.AddString(1, "How are you? Where do you come from?", endDate);
            var words = WordCloud.GetWordCloudByDayRange(1, startDate, startDate, 100).ToDictionary(r => r.Word);
            Assert.AreEqual(1, words["how"].WordCount);

            words = WordCloud.GetWordCloudByDayRange(1, startDate, endDate, 100).ToDictionary(r => r.Word);
            Assert.AreEqual(2, words["how"].WordCount);

            words = WordCloud.GetWordCloudByDayRange(1, endDate, endDate, 100).ToDictionary(r => r.Word);
            Assert.AreEqual(1, words["how"].WordCount);

            words = WordCloud.GetWordCloudByDayRange(1, today, today, 100).ToDictionary(r => r.Word);
            Assert.IsFalse(words.ContainsKey("how"));
        }
    }
}
