using Newtonsoft.Json;

namespace Serverless.FunctionsDemo.Model
{
    public class CreditCard{

        [JsonProperty(PropertyName = "id")]

        public string Id {get; set;}

        [JsonProperty(PropertyName = "cardHolder")]
        public string CardHolder {get; set;}

        [JsonProperty(PropertyName = "cardNumber")]
        public string CardNumber {get; set;}

        [JsonProperty(PropertyName = "expirationDate")]
        public string ExpirationDate {get; set;}

        public string CVV {get; set;}
    }
}