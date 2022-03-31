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
    public class GetCreditCard
    {
        private readonly ILogger<CreditCard> _logger;
        private readonly ICreditCardRepository _creditCardRepository; 
        public GetCreditCard(ICreditCardRepository creditCardRepository, ILogger<CreditCard> logger){
            _logger = logger;
            _creditCardRepository = creditCardRepository;
        }

        [FunctionName("GetCreditCard")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getCreditCardById/{id}")] HttpRequest req, string id)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request for get credit card");
            IActionResult result;
            try{
                var creditCard = await _creditCardRepository.GetCreditCard(id);
                if(creditCard == null){
                    result = new StatusCodeResult(StatusCodes.Status404NotFound);
                    _logger.LogInformation($"Credit card with id {id} doesn't exist");
                }
                result = new OkObjectResult(creditCard);
            }
            catch(Exception ex){
                _logger.LogError($"Internal server error. Exception throw: ${ex.Message}");
                result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
            return result;

            /*
            log.LogInformation("C# HTTP trigger function processed a request.");
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
