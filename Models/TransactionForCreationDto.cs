using System.ComponentModel.DataAnnotations;

namespace BankTransactionApi.Models
{
    public class TransactionForCreationDto
    {
        [Required(ErrorMessage = "You should provide a sender")]
        public Guid SenderAccountId { get; set; }
        [Required(ErrorMessage = "You should provide a receiver")]
        public Guid ReceiverAccountId { get; set; }
        [Required(ErrorMessage = "You should provide the amount sent")]
        public double Amount { get; set; }
        public DateTime SendingTime { get; set; } = DateTime.UtcNow;
        public bool Completed { get; set; }
    }
}
