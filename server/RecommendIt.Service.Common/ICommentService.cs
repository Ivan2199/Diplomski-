﻿using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Service.Common
{
    public interface ICommentService
    {
        Task<List<ICommentModel>> GetAllCommentsAsync();
        Task<ICommentModel> GetCommentAsync(Guid id);
        Task AddCommentAsync(ICommentModel comment);
        Task UpdateCommentAsync(Guid id, ICommentModel comment);
        Task DeleteCommentAsync(Guid id);
    }
}
