using System;
using System.Linq;
using System.Threading;
using AspCoreCardGameEngine.Api.Config;
using AspCoreCardGameEngine.Api.Config.Extensions;
using AspCoreCardGameEngine.Api.Persistence;
using AspCoreCardGameEngine.Api.ServiceImplementations.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace AspCoreCardGameEngine.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var mysqlConfig = _configuration.GetSection("mysql").GetValidated<MysqlConfig>();
            services.AddDbContextPool<CardsDbContext>(
                options => options.UseMySql(
                    mysqlConfig.ConnectionString,
                    o => { o.ServerVersion(new Version(5, 7, 17), ServerType.MySql); }
                ));

            services.AddControllers();

            services.AddScoped<StartupHelper>();
            services.AddServiceImplementations(_configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IHostApplicationLifetime hostApplicationLifetime,
            StartupHelper startupHelper,
            ILogger<Startup> logger)
        {
            try
            {
                var appliedMigrations = startupHelper.ApplyMysqlSchema().ToArray();
                logger.LogInformation($"Applied migrations: {string.Join(", ", appliedMigrations)}");
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Error in startup Configure");
                Thread.Sleep(TimeSpan.FromSeconds(3)); // Sleep to flush logs and app telemetry
                hostApplicationLifetime.StopApplication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}