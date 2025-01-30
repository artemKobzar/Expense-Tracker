using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int? IconId { get; set; }
        public Icon? Icon { get; set; }
        public string Type { get; set; } = "Expense";
        [JsonIgnore]
        public ICollection<Transaction>? Transactions { get; set; }
        [NotMapped]
        public string? TitleWithIcon
        {
            get
            {
                return this.Icon?.Logo + " " + this.Title;
            }
        }
    }
}
