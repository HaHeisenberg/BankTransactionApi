using BankTransactionApi.DbContexts;
using BankTransactionApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankTransactionApi.Services
{
    public interface ITransactionRepository
    {
        Task<(IEnumerable<TransactionEntity>, PaginationMetadata)> GetTransactionsAsync(
            string? searchQuery,
            int pageNumber,
            int pageSize);
        Task<TransactionEntity?> GetTransactionAsync(Guid transactionId);
        void CreateTransaction(TransactionEntity transaction);
        Task<bool> TransactionExistsAsync(Guid transactionId);
        Task<bool> SaveChangesAsync();
        void DeleteTransaction(TransactionEntity transaction);
    }
    public class TransactionRepository : ITransactionRepository
    {
        private TransactionDbContext _context;
        public TransactionRepository(TransactionDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<(IEnumerable<TransactionEntity>, PaginationMetadata)>
            GetTransactionsAsync(
                string? searchQuery,
                int pageNumber,
                int pageSize)
        {
            var collection =
                _context.Transactions as IQueryable<TransactionEntity>;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Where(
                    t => t.GetType()
                        .GetProperties()
                        .Any(
                            p => p.GetValue(t)
                                .ToString()
                                .Contains(searchQuery)));
            }

            var totalItemCount = await collection.CountAsync();

            var paginationMetadata = new PaginationMetadata(
                totalItemCount,
                pageSize,
                pageNumber);

            var collectionToReturn = await collection
                .OrderBy(c => c.SendingTime)
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetadata);
        }

        public async Task<TransactionEntity?> GetTransactionAsync(Guid transactionId)
        {
            return await _context.Transactions
                .Where(t => t.Id.Equals(transactionId))
                .FirstOrDefaultAsync();
        }

        public void CreateTransaction(TransactionEntity transaction)
        {
            _context.Transactions.Add(transaction);
        }

        public void DeleteTransaction(TransactionEntity transaction)
        {
            _context.Transactions.Remove(transaction);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public async Task<bool> TransactionExistsAsync(Guid transactionId)
        {
            return await _context.Transactions.AnyAsync(
                t => t.Id.Equals(transactionId));
        }
    }
}
