using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Serverless.FunctionsDemo.Model;
using Serverless.FunctionsDemo.Contract;

namespace Serverless.FunctionsDemo.Repository
{
    public class CreditCardRepository: ICreditCardRepository{

        private CosmosClient cosmosClient;
        private Database database;
        private Container container;
        private string databaseId = string.Empty;
        private string containerId = string.Empty;   
        private IConfiguration _iconfig; 

        public CreditCardRepository(IConfiguration iconfig){
            _iconfig = iconfig;
            string connectionString = iconfig["CosmosDBConnection"];
            databaseId = "DemoDB";
            containerId = "CreditCards";
            cosmosClient = new CosmosClient(connectionString, new CosmosClientOptions(){
                ConnectionMode = ConnectionMode.Gateway
            });
            CreateDatabaseAsync().Wait();
            CreateContainerAsync().Wait();
        }

        private async Task CreateDatabaseAsync(){
            this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
        }

        private async Task CreateContainerAsync(){
            this.container = await this.database.CreateContainerIfNotExistsAsync(containerId,"/cardNumber");
        }

        public async Task<List<CreditCard>> GetCreditCards(){
            var query = "SELECT * FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(query);
            FeedIterator<CreditCard> queryIterator = container.GetItemQueryIterator<CreditCard>(queryDefinition);
            List<CreditCard> result = new List<CreditCard>();
            while(queryIterator.HasMoreResults){
                FeedResponse<CreditCard> resultSet = await queryIterator.ReadNextAsync();
                foreach(CreditCard creditCard in resultSet){
                    result.Add(creditCard);
                }
            }
            return result;
        }

        public async Task<CreditCard> GetCreditCard(string id){
            try{
                var query = string.Format("SELECT * FROM c WHERE c.id = '{0}'", id);
                QueryDefinition queryDefinition = new QueryDefinition(query);
                FeedIterator<CreditCard> queryIterator = container.GetItemQueryIterator<CreditCard>(queryDefinition);
                CreditCard result = new CreditCard();
                while(queryIterator.HasMoreResults){
                    FeedResponse<CreditCard> resultSet = await queryIterator.ReadNextAsync();
                    foreach(CreditCard creditCard in resultSet){
                        result.Id = creditCard.Id;
                        result.CardHolder = creditCard.CardHolder;
                        result.CardNumber = creditCard.CardNumber;
                        result.ExpirationDate = creditCard.ExpirationDate;
                        result.CVV = creditCard.CVV;
                    }
                }
                return result;
            }
            catch(CosmosException ex){
                throw new System.Exception(string.Format( "Error occurs: {0}", ex.Message));
            }
        }
        public async Task CreateCreditCard(CreditCard creditCard){
            try{
                ItemResponse<CreditCard> itemResponse = await container.ReadItemAsync<CreditCard>(creditCard.Id, new PartitionKey(creditCard.CardNumber));
            }
            catch(CosmosException ex) when(ex.StatusCode == System.Net.HttpStatusCode.NotFound){
                await container.CreateItemAsync(creditCard,new PartitionKey(creditCard.CardNumber));
            }
        }
        public async Task UpdateCreditCard(CreditCard creditCard, string id, string partitionKey){
            ItemResponse<CreditCard> response = await container.ReadItemAsync<CreditCard>(id, new PartitionKey(partitionKey));
            var result = response.Resource;
            result.Id = creditCard.Id;
            result.CardHolder = creditCard.CardHolder;
            result.CardNumber = creditCard.CardNumber;
            result.ExpirationDate = creditCard.ExpirationDate;
            result.CVV = creditCard.CVV;
            await this.container.ReplaceItemAsync<CreditCard>(result,result.Id, new PartitionKey(result.CardNumber));
        }
        public async Task DeleteCreditCard(CreditCard creditCard){
            var partitionKeyValue = creditCard.CardNumber;
            var id = creditCard.Id;
            await this.container.DeleteItemAsync<CreditCard>(id, new PartitionKey(partitionKeyValue));
        }
    }   
}