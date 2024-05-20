using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Repository.Common
{
    public interface IEventPerformerRepository
    {
        Task AddEventPerformerAsync(IEventPerformerModel eventPerformer);
    }
}
