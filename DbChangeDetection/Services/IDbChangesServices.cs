using System.Threading.Tasks;

namespace DbChangeDetection.Services
{
    public interface IDbChangesServices
    {
        Task<string> GetChangeDiaryNetData(int PreviousChangeId);
        Task<int> GetCurentDbChangeId();
    }
}