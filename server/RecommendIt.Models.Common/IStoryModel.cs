using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models.Common
{
    public interface IStoryModel
    {
        Guid Id { get; set; }
        string Text { get; set; }
        DateTime? DateTime { get; set; }
        Guid LocationId { get; set; }
        Guid UserId { get; set; }

        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
        Guid CreatedBy { get; set; }
        Guid UpdatedBy { get; set; }
        bool? IsActive { get; set; }

        ILocationModel Location { get; set; }
        List<ICommentModel> Comment { get; set; }
        IUserModel User { get; set; }
        List<IPhotoModel> Photos { get; set; }
    }
}
