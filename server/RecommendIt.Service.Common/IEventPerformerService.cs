using GeoTagMap.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoTagMap.Service.Common
{
    public interface IEventPerformerService
    {
        Task AddEventPerformerAsync(IEventPerformerModel eventPerformer);
    }
}
