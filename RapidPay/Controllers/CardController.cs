using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RapidPay.Core.Services.Interfaces;
using RapidPay.Models;
using RapidPay.Repositories;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RapidPay.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CardController : ControllerBase
    {

        private readonly ILogger<CardController> _logger;
        private readonly IUFEService _ufe;
        private readonly ICardRepository _cardRepository;
        private static readonly object _balanceLock = new object();


        public CardController(ILogger<CardController> logger, IUFEService ufe, ICardRepository cardRepository)
        {
            _logger = logger;
            _ufe = ufe;
            _cardRepository = cardRepository;
        }

        [HttpGet("{cardNumber}",Name = "GetBalance")]
        public async Task<ActionResult<decimal>> GetBalance(string cardNumber)
        {
            Card card = await _cardRepository.GetCardAsync(cardNumber);

            if(card == null)
                return NotFound();

            return Ok(card.balance);
        }

        [HttpGet("createcard",Name = "CreateCard")]
        public  ActionResult<Card> CreateCard(string name, string zip)
        {
            return Ok(_cardRepository.Create(name, zip));//create card with passed in name/zip
        }

        [HttpGet("createcardforuser", Name = "CreateCardForUser")]
        public ActionResult<Card> CreateCard()
        {
            return Ok(_cardRepository.Create(User.Claims.FirstOrDefault(c => c.Type == "first_name")?.Value + " " + User.Claims.FirstOrDefault(c => c.Type == "last_name")?.Value
                , User.Claims.FirstOrDefault(c => c.Type == "zip")?.Value));//create card with name and zip of current user
        }

        [HttpGet("pay", Name = "Pay")]
        public async Task<ActionResult<decimal>> Pay(string cardnumber, decimal amount)
        {
            Card card = await _cardRepository.GetCardAsync(cardnumber);

            if(card == null)
                return NotFound();

            lock(_balanceLock)
            {
                card.balance += _ufe.CalculateFee() + amount;//lock balance amount change for thread safety
            }
                     
            return Ok(card.balance);
        }

    }
}
