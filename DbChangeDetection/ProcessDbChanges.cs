using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace DbChangeDetection
{
    public static class ProcessDbChanges
    {
        [FunctionName("ProcessDbChanges")]
        public static void Run([QueueTrigger("db-change-items")]string dbChangeItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {dbChangeItem}");
        }
    }
}
