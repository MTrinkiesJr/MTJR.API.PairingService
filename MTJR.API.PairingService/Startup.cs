using System;
using System.Collections.Generic;
using System.IO;
using Markdig;
using Markdig.SyntaxHighlighting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using MTJR.API.PairingService.Authentication;
using MTJR.API.PairingService.Handler;
using Newtonsoft.Json.Converters;

namespace MTJR.API.PairingService
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
            services.AddControllers();
            var list = new List<PairingSession>();
            services.AddSingleton(list);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = GuidAuthenticationOptions.DefaultScheme;
            })
                .AddGuidAuthentication(options =>
            {
                options.Guid = Configuration["ApiGuid"];
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Samsung TV Pairing API",
                    Description = "Samsung TV pairing API to handle handshake events"
                });
                options.DescribeAllEnumsAsStrings();

            });

            services.AddMvc(options =>
            {
                options.AllowEmptyInputInBodyModelBinding = true;
            }).AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseSyntaxHighlighting()
                .Build();

            


            

            var html = "<link rel=\"stylesheet\" type=\"text/css\" href=\"stylesheet.css\">\n" + Markdown.ToHtml(File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "/readme.md"), pipeline);
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "wwwroot/index.html", html);


            PhysicalFileProvider fileProvider = new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"wwwroot"));
            var opt = new DefaultFilesOptions();
            opt.DefaultFileNames.Clear();
            opt.DefaultFileNames.Add("index.html");
            opt.FileProvider = fileProvider;

            app.UseDefaultFiles(opt);

            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = fileProvider,
                RequestPath = new PathString("")
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Samsung TV Pairing API");
                options.RoutePrefix = "swagger";
            });
            
        }
    }
}
