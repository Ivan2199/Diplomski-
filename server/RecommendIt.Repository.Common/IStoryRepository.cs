using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Repository.Common
{
    public interface IStoryRepository
    {
        Task<List<IStoryModel>> GetAllStoriesAsync();
        Task<IStoryModel> GetStoryAsync(Guid id);
        Task AddStoryAsync(IStoryModel story);
        Task UpdateStoryAsync(Guid id, IStoryModel story);
        Task DeleteStoryAsync(Guid id);
    }
}
