﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


#region Additional Namespaces
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ChinookSystem.DAL;
using ChinookSystem.BLL;
#endregion

namespace ChinookSystem
{
    public static class ChinookExtensions
    {
        public static void ChinookSystemBackendDependencies(this IServiceCollection services,
            Action<DbContextOptionsBuilder> options)
        {
            //register the DbContext class in Chinook with the service collection
            services.AddDbContext<ChinookContext>(options);

            //add any services that you create in the class library
            //  using .AddTransient<T>(...)
            services.AddTransient<AboutServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                //create an instance of the service and return the instance
                return new AboutServices(context);
            });
            services.AddTransient<GenreServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                //create an instance of the service and return the instance
                return new GenreServices(context);
            });
            services.AddTransient<AlbumServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                //create an instance of the service and return the instance
                return new AlbumServices(context);
            });
            services.AddTransient<ArtistServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                //create an instance of the service and return the instance
                return new ArtistServices(context);
            });
            services.AddTransient<TrackServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                //create an instance of the service and return the instance
                return new TrackServices(context);
            });
            services.AddTransient<PlaylistTrackServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                //create an instance of the service and return the instance
                return new PlaylistTrackServices(context);
            });

        }
    }
}
