using DbChangeDetection.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace DbChangeDetection
{
    public class ChangeDection
    {

        private readonly IDbChangesServices _dbServices;
        private readonly IMainInformationServices _informationServices;

        public ChangeDection(IDbChangesServices dbServices,
            IMainInformationServices informationServices)
        {
            _dbServices = dbServices;
            _informationServices = informationServices;
        }

        [FunctionName("ChangeDection")]
        public async Task Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer,
        [Queue("db-change-items", Connection = "AzureWebJobsStorage")] IAsyncCollector<string> queueCollector,
        [Table("InforTable", Connection = "AzureWebJobsStorage")] CloudTable inforTable,
        ILogger log)
        {

            log.LogInformation($"C# Timer trigger function executed start at: {DateTime.Now}");

            int previousChangeId = await _informationServices.GetPreviousChangeId(inforTable);
            int curentChangeId = await _dbServices.GetCurentDbChangeId();

            if (previousChangeId != curentChangeId)
            {
                var value = await _dbServices.GetChangeDiaryNetData(previousChangeId);
                await queueCollector.AddAsync(value);

                await _informationServices.UpdateChangeId(inforTable, curentChangeId);
                log.LogInformation("Changes : " + value);
            }
            else
            {
                log.LogInformation("no Changes");
            }


        }







    }
}





