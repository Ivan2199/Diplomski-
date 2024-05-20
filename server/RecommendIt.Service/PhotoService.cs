using GeoTagMap.Models;
using GeoTagMap.Models.Common;
using GeoTagMap.Repository.Common;
using GeoTagMap.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Service
{
    public class PhotoService : IPhotoService
    {
        private readonly IPhotoRepository _photoRepository;
        public PhotoService(IPhotoRepository photoRepository)
        {
            _photoRepository = photoRepository;
        }
        public async Task<List<IPhotoModel>> GetPhotosAsync()
        {
            return await _photoRepository.GetPhotosAsync();
        }
        public async Task<IPhotoModel> GetPhotoAsync(Guid id)
        {
            return await _photoRepository.GetPhotoAsync(id);
        }
        public async Task AddPhotoAsync(IPhotoModel photoModel)
        {
            photoModel.CreatedBy = GetUserId();
            photoModel.UpdatedBy = GetUserId();
            photoModel.UserId = GetUserId();
            await _photoRepository.AddPhotoAsync(photoModel);
        }
        public async Task UpdatePhotoAsync(Guid id, IPhotoModel photoData)
        {
            photoData.UpdatedBy = GetUserId();
            await _photoRepository.UpdatePhotoAsync(id, photoData);
        }
        public async Task DeletePhotoAsync(Guid id)
        {
            await _photoRepository.DeletePhotoAsync(id);
        }

        public Guid GetUserId()
        {
            var identity = ClaimsPrincipal.Current.Identity as ClaimsIdentity;
            return Guid.Parse(identity.FindFirst("userId")?.Value);
        }
    }
}
