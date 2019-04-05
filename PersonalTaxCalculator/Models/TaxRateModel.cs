using System.ComponentModel.DataAnnotations;

namespace PersonalTaxCalculator.Models
{
    public class TaxRateModel
    {
        public int BandId { get; set; }

        [Display(Name = "Band Start")]
        public string BandStart { get; set; }

        [Display(Name = "Band Finish")]
        public string BandFinish { get; set; }

        [Display(Name = "Tax Rate %")]
        public decimal TaxRatePercent { get; set; }

        [Display(Name = "Tax Collected")]
        public decimal TaxCollected { get; set; } = 0;        
        public string TaxCollectedForDisplay { get; set; } = "0.00";

    }
}