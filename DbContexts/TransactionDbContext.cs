using System.Collections.Generic;
using BankTransactionApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BankTransactionApi.DbContexts
{
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext(DbContextOptions<TransactionDbContext> o) : base(o)
        {
        }

        public DbSet<TransactionEntity> Transactions => Set<TransactionEntity>();
    }
}
