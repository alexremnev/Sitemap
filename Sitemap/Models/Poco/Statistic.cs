using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sitemap.Models.Poco
{
    [Table("Statistic")]
    public class Statistic
    {
        [Key]
        public string PageUrl { get; set; }
        
        [ForeignKey("PageUrl")]
        public virtual IList<History> HistoryResults { get; set; }

        public double BestTime { get; set; }

        public double WorstTime { get; set; }
        
    }
}