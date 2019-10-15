using Microsoft.WindowsAzure.Storage.Table;

namespace DbChangeDetection.Models
{
    public class MainInformationTableEntity : TableEntity
    {
        public int Count { get; set; }
    }
}
