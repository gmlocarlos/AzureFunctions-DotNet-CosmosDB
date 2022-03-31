using System.Collections.Generic;
using System.Threading.Tasks;
using Serverless.FunctionsDemo.Model;

namespace Serverless.FunctionsDemo.Contract
{
    public interface ICreditCardRepository{

        Task<List<CreditCard>> GetCreditCards();
        Task<CreditCard> GetCreditCard(string id);
        Task CreateCreditCard(CreditCard creditCard);
        Task UpdateCreditCard(CreditCard creditCard, string id, string partitionKey);
        Task DeleteCreditCard(CreditCard creditCard);
    }
}