using Microsoft.AspNetCore.CookiePolicy;

namespace ChatApp.API.ServiceCollection;

public static class MiddlewareConfiguration
{
    public static IApplicationBuilder ConfigureMiddleware(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.DisplayRequestDuration();
            });
        }
        else
        {
            app.UseExceptionHandler("/error");
            app.UseHsts();
        }
            
        app.UseHttpsRedirection();
        app.UseRouting();

        //app.ConfigureCustomMiddleware();

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.Strict,
            HttpOnly = HttpOnlyPolicy.Always,
            Secure = CookieSecurePolicy.Always,
        });
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        return app;
    }
}
