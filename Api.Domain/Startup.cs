using System.Collections.Immutable;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
using Api_Macoratti.Repository;
using AutoMapper;
using Api_Macoratti.DTOs.Mappings;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNet.OData.Extensions;

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
            // cors
            services.AddCors(opt => {
                opt.AddPolicy("PermitirApiRequest", 
                    builder =>
                    builder.WithOrigins("http://apirequest.io")
                    .WithMethods("GET"));

                // para consumir a API
                opt.AddPolicy("EnableCors", builder => {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod().Build();
                });
            });

            var mappingConfig = new MapperConfiguration(mc => 
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ApiLoggingFilter>();
            services.AddDbContext<AppDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            // identity
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            // JWT
            // adiciona o manipulador de autenticação e define o 
            // esquema de autenticação usado: Bearer
            // valida o emissor, a audiencia e a chave
            // usando a chave secreta valida a assinatura
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = Configuration["TokenConfiguration:Audience"],
                    ValidIssuer = Configuration["TokenConfiguration:Issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                });
            
            // versionamento
            services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new HeaderApiVersionReader("x-api-version");
            });

            services.AddTransient<IMeuServico, MeuServico>();

            // swagger
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "APICatalogo",
                    Description = "Catálogo de Produtos e Categorias",
                    TermsOfService = new Uri("https://thais.net/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "thais",
                        Email = "thais@gmail.com",
                        Url = new Uri("https://thais.net"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Usar sobre LICX",
                        Url = new Uri("https://thais.net/license"),
                    }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; 
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Copiar 'bearer ' + token'",
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey
                    });

                // var security = new Dictionary<string, IEnumerable<string>>
                // {
                //     {"Bearer", new string[] {}},
                // };

                // c.AddSecurityRequirement(security);

                // register
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                });
                
            });

            // swagger opção 2
            // services.AddSwaggerGen(c => {
            //     c.SwaggerDoc("v1", new OpenApiInfo { Title = "CatalogoAPI", Version = "v1"});

            //     var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; 
            //     var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //     c.IncludeXmlComments(xmlPath);
            // });

            services.AddControllers().AddNewtonsoftJson(options => {
                options.SerializerSettings.ReferenceLoopHandling = 
                Newtonsoft.Json.ReferenceLoopHandling.Ignore; 
            });

            // OData
            // services.AddOData();

            // services.AddMvc().AddMvcOptions(options => options.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_3_0);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // logger
            // loggerFactory.AddProvider(new CustomLoggerProvider(new CustomLoggerProviderConfiguration
            // {
            //     LogLevel = LogLevel.Information
            // }));
            // adiciona middleware de tratamento de erros
            app.ConfigureExceptionHandler();

            // adiciona o middleware para redirecionar para https
            app.UseHttpsRedirection();

            // adiciona o middleware de roteamento
            app.UseRouting();

            // adiciona o middleware de autenticação
            app.UseAuthentication();

            // adiciona o middleware que habilita a autorização
            app.UseAuthorization();

            // cors
            // app.UseCors(opt => opt
            //     .WithOrigins("http://apirequest.io")
            //     .WithMethods("GET")
            //     );
            app.UseCors();

            // para consumo da API
            app.UseCors("EnableCors");

            // swagger
            app.UseSwagger();

            // swaggerUI
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Produtos e Categorias");
            });

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

            // OData -> não funciona
            // app.UseMvc(options => 
            // {
            //     options.EnableDependencyInjection();
            //     options.Expand().Select().Count().OrderBy().Filter();
            // });
        }
    }
}
