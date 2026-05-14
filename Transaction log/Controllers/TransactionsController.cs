using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transaction_log.Model;

namespace Transaction_log.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionContext _context;

        public TransactionsController(TransactionContext ctx)
        {
            _context = ctx;
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            try
            {
                var transactions = await _context.Transactions.ToListAsync();
                return Ok(transactions); // 200 OK
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching transactions.");
            }
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            try
            {
                var transaction = await _context.Transactions.FindAsync(id);

                if (transaction == null)
                {
                    return NotFound("Transaction not found."); // 404 Not Found
                }

                return Ok(transaction); // 200 OK
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while fetching the transaction.");
            }
        }
        // POST: api/Transactions
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction log)
        {
            try
            { 
                // Example business conflict (dummy check)
                // Suppose we don't allow duplicate TransactionId manually inserted
                if (_context.Transactions.Any(t => t.TransactionId == log.TransactionId))
                {
                    return Conflict("Duplicate transaction ID not allowed."); // 409 Conflict
                }

                log.Timestamp = DateTime.Now;

                _context.Transactions.Add(log);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTransaction), new { id = log.TransactionId }, log); // 201 Created
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while creating the transaction."); // 500 Internal Server Error
            }
        }
    }
    
}
