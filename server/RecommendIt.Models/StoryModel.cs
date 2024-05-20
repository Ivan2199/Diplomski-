using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Models
{
    public class StoryModel : IStoryModel
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime? DateTime { get; set; }
        public Guid LocationId { get; set; }
        public Guid UserId { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
        public bool? IsActive { get; set; }

        public ILocationModel Location { get; set; }
        public List<ICommentModel> Comment { get; set; }
        public IUserModel User { get; set; }
        public List<IPhotoModel> Photos { get; set; }
    }
}
