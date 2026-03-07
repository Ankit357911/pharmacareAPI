using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pharmacareAPI.Models
{
    public enum PeriodType
    {
        Weekly,
        Monthly,
        Yearly,
        Overall
    }

    public class Earning
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public PeriodType PeriodType { get; set; }

        [Required]
        public DateTime PeriodStart { get; set; }

        [Required]
        public DateTime PeriodEnd { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Investment { get; set; } = 0.00m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Earnings { get; set; } = 0.00m;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Profit { get; set; } = 0.00m;
    }
}