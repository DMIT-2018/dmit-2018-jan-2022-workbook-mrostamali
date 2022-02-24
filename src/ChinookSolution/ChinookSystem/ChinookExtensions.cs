﻿using System;
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
        public static void ChinookSystemBackendDependencies
            (this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            //Register the DBContext class in Chinook with the service collection
            services.AddDbContext<ChinookContext>(options);

            //Add any services that you create in the class library
            //   using the .AddTransient<T>(...)

            services.AddTransient<AboutServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                //Create an instance of the services and return the instance
                return new AboutServices(context);
            });

            services.AddTransient<GenreServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                //Create an instance of the Genre and return the instance
                return new GenreServices(context);
            });

            services.AddTransient<AlbumServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<ChinookContext>();
                //Create an instance of the required data and return the instance
                return new AlbumServices(context);
            });
        }
    }
}
