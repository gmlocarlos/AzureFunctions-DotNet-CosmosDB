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
    public class UpdateCreditCard
    {
        private readonly ILogger<CreditCard> _logger;
        private readonly ICreditCardRepository _creditCardRepository; 
        public UpdateCreditCard(ICreditCardRepository creditCardRepository, ILogger<CreditCard> logger){
            _logger = logger;
            _creditCardRepository = creditCardRepository;
        }   

        [FunctionName("UpdateCreditCard")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "updateCreditCard/{id}")]
            HttpRequest req, string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request for update credit card");
             IActionResult result;
            try{
                var creditCard = await _creditCardRepository.GetCreditCard(id);
                if(creditCard == null){
                    result = new StatusCodeResult(StatusCodes.Status404NotFound);
                    _logger.LogInformation($"Credit card with id {id} doesn't exist");
                     Console.WriteLine($"Credit card with id {id} doesn't exist");
                }
                else{
                    using var r = new StreamReader(req.Body, Encoding.UTF8);
                    var incomingRequest = await r.ReadToEndAsync();
                    if(!string.IsNullOrEmpty(incomingRequest)){
                        var creditCardReq = JsonConvert.DeserializeObject<CreditCard>(incomingRequest);
                        var creditCardModel = new CreditCard{
                            Id = id,
                            CardHolder = creditCardReq.CardHolder,
                            CardNumber = creditCardReq.CardNumber,
                            ExpirationDate = creditCardReq.ExpirationDate,
                            CVV = creditCardReq.CVV
                        };
                        await _creditCardRepository.UpdateCreditCard(creditCardModel,id,creditCardModel.CardNumber);
                        result = new StatusCodeResult(StatusCodes.Status201Created);                        
                    }
                    else{
                        result = new StatusCodeResult(StatusCodes.Status400BadRequest);
                    }
                }
            }
            catch(Exception ex){
                _logger.LogError($"Internal Server Error. Exception: {ex.Message}");
                Console.WriteLine($"Internal Server Error. Exception: {ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            return result;
        }
    }
}
