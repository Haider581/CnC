using CnC.Core.Cards;
using CnC.Core.Customers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Service
{
    public interface ICardServiceProvider
    {
        int NewCardRequest(Customer customer, CardRequest cardRequest);
        List<Card> GetCards(int clientId);
        Card GetCardInfo(string clientId, string cardId, string expiryDate, string dateOfBirth
            , int maxRows, string startDate, string endDate);
    }
}
