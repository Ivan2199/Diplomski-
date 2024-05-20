using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Service.Common
{
    public interface IStoryService
    {
        Task<List<IStoryModel>> GetAllStoriesAsync();
        Task<IStoryModel> GetStoryAsync(Guid id);
        Task AddStoryAsync(IStoryModel story);
        Task UpdateStoryAsync(Guid id, IStoryModel story);
        Task DeleteStoryAsync(Guid id);
    }
}
