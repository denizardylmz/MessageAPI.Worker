using MessageWorker.Application.Features.Shifts;
using MessageWorker.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Application
{
    public static class ApplicaitonServiceRegistration
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<StartShiftHandler>();

            services.AddScoped<EndShiftHandler>();

            return services;
        }
    }
}
