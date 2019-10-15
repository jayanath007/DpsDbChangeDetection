using DbChangeDetection.Models;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace DbChangeDetection.Services
{
    public class MainInformationServices : IMainInformationServices
    {


        public async Task UpdateChangeId(CloudTable inforTable, int curentChangeId)
        {
            var entity = new MainInformationTableEntity { PartitionKey = "MainInformation", RowKey = "DbChangeRow", Count = curentChangeId };
            TableOperation insertOrMergeOperation = TableOperation.InsertOrMerge(entity);
            await inforTable.ExecuteAsync(insertOrMergeOperation);
        }

        public async Task<int> GetPreviousChangeId(CloudTable inforTable)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<MainInformationTableEntity>("MainInformation", "DbChangeRow");
            TableResult result = await inforTable.ExecuteAsync(retrieveOperation);
            MainInformationTableEntity inforEntity = result.Result as MainInformationTableEntity;
            int previousChangeId = 0;
            if (inforEntity != null)
            {
                previousChangeId = inforEntity.Count;
            }

            return previousChangeId;
        }

    }
}
