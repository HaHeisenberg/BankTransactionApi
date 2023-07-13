using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BankTransactionApi.Entities
{
    public class TransactionEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public Guid SenderAccountId { get; set; }
        [Required]
        public Guid ReceiverAccountId { get; set; }
        [Required]
        public double Amount { get; set; }
        [Required]
        public DateTime SendingTime { get; set; }
        public bool Completed { get; set; }
    }
}
