using RapidPay.Models;

namespace RapidPay.Repositories
{
    public interface ICardRepository
    {
        Card Create(string name, string zip);
        Card GetCard(string cardNumber);
        Task<Card> GetCardAsync(string cardNumber);


    }
}
