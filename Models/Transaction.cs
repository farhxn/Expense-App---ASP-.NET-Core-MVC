using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_App.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        //Foriegn Key
        [Range(1,int.MaxValue,ErrorMessage ="Please Select Category")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        
        [Range(1,int.MaxValue,ErrorMessage ="Amount Should be greater than 0")]
         public int Amount { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string? Note { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
        [NotMapped]
        public string CategoryTitleWithIcon { get {
                return Category == null ? "" :Category.TitleWithIcon;
            } 
        }
        
        [NotMapped]
        public string? FormatedAmount { get {
                return ((Category == null || Category.Type == "Expense")  ? "- " : "+ ")+Amount.ToString("C0");
            } 
        }

    }
}
