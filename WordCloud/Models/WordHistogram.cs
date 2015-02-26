using System.Collections.Generic;

namespace WordCloudService
{
    public class WordHistogram
    {
        public string Word { get; set; }
        public int StringCount { get; set; }
        public int WordCount { get; set; }
        public IEnumerable<long> CloudKeys { get; set; }
    }
}
