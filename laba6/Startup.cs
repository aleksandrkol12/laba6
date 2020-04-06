using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace laba6
{
    public class Startup
    {
        StudentController studentController = new StudentController();

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationContext>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello world!");
                });

                endpoints.MapPost("/students", async context =>
                {
                    await context.Response.WriteAsync(await studentController.AddStudentAsync(context));
                });

                endpoints.MapGet("/students", async context =>
                {
                    await context.Response.WriteAsync(await studentController.GetStudentsAsync());
                });

                endpoints.MapGet("/students/{id}", async context =>
                {
                    await context.Response.WriteAsync(await studentController.GetStudentAsync(context));
                });

                endpoints.MapDelete("/students/{id}", async context =>
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    await context.Response.WriteAsync(await studentController.DeleteStudentAsync(context));
                });

                endpoints.MapPut("/students/{id}", async context =>
                {
                    context.Response.ContentType = "text/html; charset=utf-8";
                    await context.Response.WriteAsync(await studentController.PutStudentAsync(context));
                });
            });
        }
    }
}