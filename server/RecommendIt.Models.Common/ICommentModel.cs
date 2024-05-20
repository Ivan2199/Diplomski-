using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models.Common
{
    public interface ICommentModel
    {
        Guid Id { get; set; }
        string Text { get; set; }
        bool? IsActive { get; set; }
        Guid UserId { get; set; }
        Guid? EventId { get; set; }
        Guid? TouristSiteId { get; set; }
        Guid? StoryId { get; set; }

        DateTime DateCreated { get; set; }
        DateTime DateUpdated { get; set; }
        Guid CreatedBy { get; set; }
        Guid UpdatedBy { get; set; }

        IEventModel Event { get; set; }
        IStoryModel Story { get; set; }
        ITouristSitesModel Site { get; set; }
        IUserModel User { get; set; }
    }
}
