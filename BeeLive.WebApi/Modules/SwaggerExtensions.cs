namespace BeeLive.WebApi.Modules
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddBeeLiveSwagger(this IServiceCollection service)
        {
            service.AddSwaggerGen(c =>
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = "BEELIVE SERVER API",
                    Description = "REST API of BEELIVE BACKEND"
                }));
            return service;
        }

        public static IApplicationBuilder UseBeeLiveSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "REST API of BEELIVE");
                c.RoutePrefix = "api/swagger";
            });
            return app;
        }
    }
}
