using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serverless.FunctionsDemo.Model;
using Serverless.FunctionsDemo.Contract;

namespace Serverless.FunctionsDemo.Function
{
    public class GetCreditCards
    {
        private readonly ILogger<CreditCard> _logger;
        private readonly ICreditCardRepository _creditCardRepository; 
        public GetCreditCards(ICreditCardRepository creditCardRepository, ILogger<CreditCard> logger){
            _logger = logger;
            _creditCardRepository = creditCardRepository;
        }

        [FunctionName("GetCreditCards")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getCreditCards")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request for get credit cards");
            IActionResult result;
            try{
                var creditCards = await _creditCardRepository.GetCreditCards();
                if(creditCards == null){
                    result = new StatusCodeResult(StatusCodes.Status404NotFound);
                    _logger.LogInformation($"Credit cards not found");
                }
                result = new OkObjectResult(creditCards);
            }
            catch(Exception ex){
                _logger.LogError($"Internal server error. Exception throw: ${ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            return result;            

            /*
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            string name = req.Query["name"];
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;
            string responseMessage = string.IsNullOrEmpty(name)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {name}. This HTTP triggered function executed successfully.";
            return new OkObjectResult(responseMessage);
            */
        }
    }
}
