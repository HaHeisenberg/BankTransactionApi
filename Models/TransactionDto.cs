using System.ComponentModel.DataAnnotations;

namespace BankTransactionApi.Models
{
    public class TransactionDto
    {
        public Guid Id { get; set; }
        public Guid SenderAccountId { get; set; }
        public Guid ReceiverAccountId { get; set; }
        public double Amount { get; set; }
        public DateTime SendingTime { get; set; }
        public bool Completed { get; set; }
    }
}
