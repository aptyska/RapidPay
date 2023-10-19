using System.Text;
using RapidPay.Models;

namespace RapidPay.Repositories
{
    public class CardRepository : ICardRepository
    {
        private CardDataStore _data;

        public CardRepository(CardDataStore data) 
        {
            _data = data;
        }

        public Card Create(string name, string zip)
        {
            Card temp = new Card()
            {
                cardNumber = getCardNumber(),
                name = name,
                zip = zip,
                balance = 0
            };

            _data.Cards.Add(temp);

            return temp;
        }

        private string getCardNumber()
        {
            if(_data.Cards.Count == 0)//If collection is empty, initialize with 15 digit int
                return "100000000000000";
            else
                return (_data.Cards.Max(card => Convert.ToInt64(card.cardNumber)) + 1).ToString();//Get highest card number and increment by 1 for unique new number
        }



        public Card GetCard(string cardNumber)
        {
            return _data.Cards.Find(card => card.cardNumber == cardNumber);
        }

        public async Task<Card> GetCardAsync(string cardNumber)
        {
            return await Task.Run(() => _data.Cards.Find(card => card.cardNumber == cardNumber));
        }
    }
}
