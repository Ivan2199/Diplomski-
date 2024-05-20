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
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }
        public async Task<List<ICommentModel>> GetAllCommentsAsync()
        {
            return await _commentRepository.GetAllCommentsAsync();
        }
        public async Task<ICommentModel> GetCommentAsync(Guid id)
        {
            return await _commentRepository.GetCommentAsync(id);
        }
        public async Task AddCommentAsync(ICommentModel comment)
        {
            comment.UserId = GetUserId();
            comment.CreatedBy = GetUserId();
            comment.UpdatedBy = GetUserId();
            await _commentRepository.AddCommentAsync(comment);
        }
        public async Task UpdateCommentAsync(Guid id, ICommentModel comment)
        {
            comment.UpdatedBy = GetUserId();
            await _commentRepository.UpdateCommentAsync(id, comment);
        }
        public async Task DeleteCommentAsync(Guid id)
        {
            await _commentRepository.DeleteCommentAsync(id);
        }
        public Guid GetUserId()
        {
            var identity = ClaimsPrincipal.Current.Identity as ClaimsIdentity;
            return Guid.Parse(identity.FindFirst("userId")?.Value);
        }
    }
}
