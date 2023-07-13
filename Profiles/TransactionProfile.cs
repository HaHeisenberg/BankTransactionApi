using AutoMapper;
using BankProject.Shared;

namespace BankTransactionApi.Profiles
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<Entities.TransactionEntity, Models.TransactionDto>();
            CreateMap<Models.TransactionForCreationDto, Entities.TransactionEntity>();
            CreateMap<Models.TransactionForCreationDto, TransactionForProcessingDto>();
        }
    }
}
