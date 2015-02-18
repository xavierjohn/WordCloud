using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCloudService
{
    // Words per day.
    [Table("WordCloud.WordHistograms")]
    public class WordHistogram
    {
        [Key]
        public string Word { get; set; }
        public DateTime Date { get; set; }
        public int StringCount { get; set; }
        public int WordCount { get; set; }
    }
}
