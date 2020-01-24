using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace AspCoreCardGameEngine.Api.Config.Extensions
{
    public static class StartupSwaggerExtensions
    {
        public static void AddCustomSwagger(this IServiceCollection services, string pageTitle)
        {
            services.AddSwaggerGen(c =>
            {
                c.CustomSchemaIds(t => t.FullName);

                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = pageTitle,
                    Version = "v1"
                });

                // This could be used if we later add authentication
                // c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                // {
                //     Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                //     Name = "Authorization",
                //     In = ParameterLocation.Header,
                //     Type = SecuritySchemeType.ApiKey
                // });
                //
                // c.AddSecurityRequirement(new OpenApiSecurityRequirement
                // {
                //     {
                //         new OpenApiSecurityScheme
                //         {
                //             Reference = new OpenApiReference
                //             {
                //                 Type = ReferenceType.SecurityScheme,
                //                 Id = "Bearer"
                //             }
                //         },
                //         new string[] { }
                //     }
                // });
            });
        }

        public static void UseCustomSwagger(this IApplicationBuilder app, string specName)
        {
            app.UseSwagger(c =>
            {
                //https://github.com/domaindrivendev/Swashbuckle.AspNetCore#change-the-path-for-swagger-json-endpoints
                c.RouteTemplate = "api/swagger/{documentName}/swagger.json";
            });

            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api/swagger-ui";
                c.DefaultModelsExpandDepth(0);
                c.SwaggerEndpoint("/api/swagger/v1/swagger.json", specName);
            });
        }
    }
}
