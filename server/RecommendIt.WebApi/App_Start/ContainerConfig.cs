using Autofac;
using GeoTagMap.Repository;
using GeoTagMap.Repository.Common;
using GeoTagMap.Service;
using GeoTagMap.Service.Common;
using GeoTagMap.WebApi.Controllers;
using System.Net.Http;

namespace GeoTagMap.WebApi
{
    public static class ContainerConfig
    {
        public static IContainer ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            //builder.RegisterType<controller name>();
            builder.RegisterType<UserController>();
            builder.RegisterType<UserRepository>().As<IUserRepository>();
            builder.RegisterType<UserService>().As<IUserService>();

            builder.RegisterType<RoleController>();
            builder.RegisterType<RoleService>().As<IRoleService>();
            builder.RegisterType<RoleRepository>().As<IRoleRepository>();

            builder.RegisterType<EventController>();
            builder.RegisterType<EventService>().As<IEventService>();
            builder.RegisterType<EventRepository>().As<IEventRepository>();

            builder.RegisterType<CommentController>();
            builder.RegisterType<CommentService>().As<ICommentService>();
            builder.RegisterType<CommentRepository>().As<ICommentRepository>();

            builder.RegisterType<StoryController>();
            builder.RegisterType<StoryService>().As<IStoryService>();
            builder.RegisterType<StoryRepository>().As<IStoryRepository>();

            builder.RegisterType<TouristSiteController>();
            builder.RegisterType<TouristSiteService>().As<ITouristSiteService>();
            builder.RegisterType<TouristSiteRepository>().As<ITouristSiteRepository>();

            builder.RegisterType<PerformerController>();
            builder.RegisterType<PerformerService>().As<IPerformerService>();
            builder.RegisterType<PerformerRepository>().As<IPerformerRepository>();

            builder.RegisterType<TicketInformationController>();
            builder.RegisterType<TicketInformationService>().As<ITicketInformationService>();
            builder.RegisterType<TicketInformationRepository>().As<ITicketInformationRepository>();

            builder.RegisterType<LocationController>();
            builder.RegisterType<LocationService>().As<ILocationService>();
            builder.RegisterType<LocationRepository>().As<ILocationRepository>();

            builder.RegisterType<GeoLocationController>();
            builder.RegisterType<GeoLocationService>().As<IGeoLocationService>();
            builder.RegisterType<GeoLocationRepository>().As<IGeoLoctionRepositrory>();

            builder.RegisterType<CategoryController>();
            builder.RegisterType<CategoryService>().As<ICategoryService>();
            builder.RegisterType<CategoryRepository>().As<ICategoryRepository>();

            builder.RegisterType<PhotoController>();
            builder.RegisterType<PhotoService>().As<IPhotoService>();
            builder.RegisterType<PhotoRepository>().As<IPhotoRepository>();

            builder.RegisterType<TouristSiteCategoryService>().As<ITouristSiteCategoryService>();
            builder.RegisterType<TouristSiteCategoryRepository>().As<ITouristSiteCategoryRepository>();

            builder.RegisterType<EventPerformerService>().As<IEventPerformerService>();
            builder.RegisterType<EventPerformerRepository>().As<IEventPerformerRepository>();

            builder.RegisterType<ApiEventDataInsertController>();
            builder.RegisterType<ApiTouristSiteDataInsertController>();

            builder.Register(c => new HttpClient()).SingleInstance();


            var container = builder.Build();

            return container;
        }
    }
}