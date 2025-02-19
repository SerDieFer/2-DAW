using System.Threading.Tasks;
using Vestigio.Models.DTOs;

namespace Vestigio.Services
{
    public interface IFilterService
    {
        Task<FilterResult> GetFilteredResultsAsync(FilterRequest request);
    }
}