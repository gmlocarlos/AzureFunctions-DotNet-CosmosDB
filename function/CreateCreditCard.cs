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
    public class CreateCreditCard
    {
        private readonly ILogger<CreditCard> _logger;
        private readonly ICreditCardRepository _creditCardRepository; 
        public CreateCreditCard(ICreditCardRepository creditCardRepository, ILogger<CreditCard> logger){
            _logger = logger;
            _creditCardRepository = creditCardRepository;
        }

        [FunctionName("AddCreditCard")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "createCreditCard")] HttpRequest req)
        {
            IActionResult result;
            try{
                using var read = new StreamReader(req.Body, Encoding.UTF8);
                var incomingReq = await read.ReadToEndAsync();
                if(!string.IsNullOrEmpty(incomingReq)){
                    var creditCardReq = JsonConvert.DeserializeObject<CreditCard>(incomingReq);
                    var creditCard = new CreditCard{
                        Id = Guid.NewGuid().ToString(),
                        CardHolder = creditCardReq.CardHolder,
                        CardNumber = creditCardReq.CardNumber,
                        ExpirationDate = creditCardReq.ExpirationDate,
                        CVV = creditCardReq.CVV
                    };
                    await _creditCardRepository.CreateCreditCard(creditCard);
                    result = new StatusCodeResult(StatusCodes.Status201Created);
                }
                else{
                     result = new StatusCodeResult(StatusCodes.Status400BadRequest);
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
