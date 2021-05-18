using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Api_Macoratti.Context;
using Api_Macoratti.Services;
using Api_Macoratti.Models;
using Microsoft.AspNetCore.Http;
using Api_Macoratti.Filters;
using Api_Macoratti.Extensions;
using Api_Macoratti.Logging;

namespace Api_Macoratti
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ApiLoggingFilter>();
            services.AddDbContext<AppDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            services.AddTransient<IMeuServico, MeuServico>();

            services.AddControllers().AddNewtonsoftJson(options => {
                options.SerializerSettings.ReferenceLoopHandling = 
                Newtonsoft.Json.ReferenceLoopHandling.Ignore; 
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            loggerFactory.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
            {
                LogLevel = LogLevel.Information
            }));
            // adiciona middleware de tratamento de erros
            app.ConfigureExceptionHandler();

            // adiciona o middleware para redirecionar para https
            app.UseHttpsRedirection();

            // adiciona o middleware de roteamento
            app.UseRouting();

            // adiciona o middleware que habilita a autorização
            app.UseAuthorization();

            // adiciona o middleware que executa o endpoint do request atual
            app.UseEndpoints(endpoints =>
            {
                // adiciona os endpoints para as actions dos controladores sem especificar rotas
                endpoints.MapControllers();
            });

            // middleware personalizado
            app.Run(async (context) => {
                await context.Response.WriteAsync("Middleware final");
            });
        }
    }
}
