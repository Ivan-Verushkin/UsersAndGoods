using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Model;

namespace ControleService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddMassTransit(config => {

                config.AddConsumer<ProductConsumer>();

                config.UsingRabbitMq((ctx, cfg) => {

                    cfg.Host(Configuration["entryCredetials"]);

                    cfg.ReceiveEndpoint(Configuration["theFirstQueue"], c => {

                        c.ConfigureConsumer<ProductConsumer>(ctx);
                    });
                });
            });

            services.AddMassTransitHostedService();

            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            // set database context
            services.AddDbContext<ProductContextCopy>(options => options.UseSqlServer(connectionString));

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
