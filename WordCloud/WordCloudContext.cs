using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordCloudService
{
    public class WordCloudContext : DbContext
    {
        static WordCloudContext()
        {
            Database.SetInitializer(new NullDatabaseInitializer<WordCloudContext>());
        }
        public DbSet<StopWord> StopWords { get; set; }
    }
}
