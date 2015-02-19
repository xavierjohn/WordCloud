using EntityFrameworkExtras.EF6;

namespace WordCloudService
{
    [UserDefinedTableType("[WordCloud].[WordHistogramType]")]
    public class WordHistogram
    {
        [UserDefinedTableTypeColumn(1)]
        public string Word { get; set; }

        [UserDefinedTableTypeColumn(2)]
        public int StringCount { get; set; }

        [UserDefinedTableTypeColumn(3)]
        public int WordCount { get; set; }
    }
}
