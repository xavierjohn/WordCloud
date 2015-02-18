using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCloudService
{
    [Table("WordCloud.StopWords")]
    public class StopWord
    {
        [Key]
        public string Word { get; set; }
    }
}
