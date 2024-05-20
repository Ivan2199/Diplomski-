using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Repository.Common
{
    public interface IPhotoRepository
    {
        Task<List<IPhotoModel>> GetPhotosAsync();
        Task<IPhotoModel> GetPhotoAsync(Guid id);
        Task AddPhotoAsync(IPhotoModel photoModel);
        Task UpdatePhotoAsync(Guid id, IPhotoModel photoData);
        Task DeletePhotoAsync(Guid id);

    }
}
