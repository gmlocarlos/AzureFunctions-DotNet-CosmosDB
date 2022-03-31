using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Serverless.FunctionsDemo.Model;
using Serverless.FunctionsDemo.Contract;

namespace Serverless.FunctionsDemo.Function
{
    public class DeleteCreditCard
    {
        private readonly ILogger<CreditCard> _logger;
        private readonly ICreditCardRepository _creditCardRepository; 
        public DeleteCreditCard(ICreditCardRepository creditCardRepository, ILogger<CreditCard> logger){
            _logger = logger;
            _creditCardRepository = creditCardRepository;
        }   

        [FunctionName("DeleteCreditCard")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "deleteCreditCard/{id}")] HttpRequest req, string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request for delete credit card");
            IActionResult result;
            try{
                var creditCard = await _creditCardRepository.GetCreditCard(id);
                if(creditCard == null){
                    result = new StatusCodeResult(StatusCodes.Status404NotFound);
                    _logger.LogInformation($"Credit card with id {id} doesn't exist");
                }
                else{
                    await _creditCardRepository.DeleteCreditCard(creditCard);
                    result = new StatusCodeResult(StatusCodes.Status204NoContent);
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal server error. Exception throw: ${ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
