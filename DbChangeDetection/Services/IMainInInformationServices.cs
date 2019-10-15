using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;

namespace DbChangeDetection.Services
{
    public interface IMainInformationServices
    {
        Task<int> GetPreviousChangeId(CloudTable inforTable);
        Task UpdateChangeId(CloudTable inforTable, int curentChangeId);
    }
}