using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;


using BankTransactionApi.Entities;
using BankTransactionApi.Models;
using BankTransactionApi.Services;
using BankProject.Shared;

namespace BankTransactionApi.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionRepository _transactionRepository;
        private IHttpClientFactory _httpClientFactory;
        private IMapper _mapper;
        private readonly int maxPageSize = 20;

        public TransactionController(
            IHttpClientFactory httpClientFactory,
            ITransactionRepository transactionRepository,
            IMapper mapper)
        {
            _httpClientFactory = httpClientFactory;
            _mapper = mapper ??
                      throw new ArgumentNullException(
                          nameof(mapper));

            _transactionRepository = transactionRepository ??
                              throw new ArgumentNullException(
                                  nameof(transactionRepository));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionDto>>>
            GetTransactions(
                string? searchQuery,
                int pageNumber = 1,
                int pageSize = 10)
        {
            if (pageSize > maxPageSize)
            {
                pageSize = maxPageSize;
            }

            var (transactionEntities, paginationMetadata) =
                await _transactionRepository.GetTransactionsAsync(
                    searchQuery,
                    pageNumber,
                    pageSize);

            Response.Headers.Add(
                "X-Pagination",
                JsonSerializer.Serialize(paginationMetadata));

            return Ok(
                _mapper.Map<IEnumerable<TransactionDto>>(transactionEntities));
        }

        [HttpGet(
            "{transactionId}",
            Name = "GetTransaction")]
        public async Task<ActionResult> GetTransaction(
            Guid transactionId)
        {
            var transactionEntity =
                await _transactionRepository.GetTransactionAsync(transactionId);

            if (transactionEntity == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<TransactionDto>(transactionEntity));
        }

        [HttpPost]
        public async Task<ActionResult<TransactionDto>> CreateTransaction(TransactionForCreationDto transaction)
        {
            var finalTransaction = _mapper.Map<TransactionEntity>(transaction);
            var httpClient = _httpClientFactory.CreateClient("AccountsAPI");

            var transactionForProcessing = _mapper.Map<TransactionForProcessingDto>(transaction);

            var response = await httpClient.PostAsJsonAsync("api/accounts/transfer", transactionForProcessing);

            if(!response.IsSuccessStatusCode) 
            {
                return BadRequest(response);
            }

            _transactionRepository.CreateTransaction(finalTransaction);
            await _transactionRepository.SaveChangesAsync();

            var createdTransactionToReturn = _mapper.Map<TransactionDto>(finalTransaction);

            return CreatedAtRoute(
                "GetTransaction",
                new { transactionId = finalTransaction.Id },
                createdTransactionToReturn);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteTransaction(Guid transactionId)
        {
            if (!await _transactionRepository.TransactionExistsAsync(transactionId))
            {
                return NotFound();
            }

            var transactionEntity = await _transactionRepository.GetTransactionAsync(transactionId);

            if (transactionEntity == null)
            {
                return NotFound();
            }

            _transactionRepository.DeleteTransaction(transactionEntity);
            await _transactionRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
