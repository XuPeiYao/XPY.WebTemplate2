using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XPY.WebTemplate2.Data;
using XPY.WebTemplate2.Infrastructure.EF;
using XPY.WebTemplate2.Infrastructure.Extensions;
using XPY.WebTemplate2.Services.Extensions;

namespace XPY.WebTemplate2
{
    /// <summary>
    /// 啟動類別
    /// </summary>
    public class Startup
    {
        public static IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 此方法在執行階段呼叫，使用此方法在服務容器加入新服務
        /// 需要了解更多設定相關資訊請瀏覽: <see cref="https://go.microsoft.com/fwlink/?LinkID=398940"/>
        /// </summary>
        /// <param name="services">服務容器</param>
        public void ConfigureServices(IServiceCollection services)
        {
            // 日誌紀錄器
            services.AddLogging();

            services.AddScoped<RequestCancelInterceptor>();
            services.AddDbContext<SampleDbContext>((sp, options) => {
                options.AddInterceptors(sp.GetService<RequestCancelInterceptor>());
            });

            // 支援DI取得HttpContext
            services.AddHttpContextAccessor();

            // 快取
            services.AddMemoryCache();

            // MiniProfiler支援
            services.AddMiniProfiler(config => {
                config.RouteBasePath = "/profiler";
            }).AddEntityFramework();

            // 加入NSwag
            services.AddNSwag(
                    title: Configuration.GetValue<string>("Swagger:Title"),
                    description: Configuration.GetValue<string>("Swagger:Description"));

            // JWT支援
            services.AddJwtAuthentication(
                issuer: Configuration.GetValue<string>("JWT:Issuer"),
                audience: Configuration.GetValue<string>("JWT:Audience"),
                secureKey: Configuration.GetValue<string>("JWT:SecureKey"));

            // 加入路由服務
            services.AddRouting();

            // 中文編碼問題修正
            services.AddSingleton(HtmlEncoder.Create(UnicodeRanges.All));

            // 加入MVC核心服務
            services.AddMvcCore()
                .AddAuthorization()
                .AddApiExplorer()
                .AddJsonOptions(options => {
                    // 列舉
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                })
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Latest);

            // 加入SPA
            services.AddSpaStaticFiles(config => {
                config.RootPath = "wwwroot";
            });

            // 加入服務
            services.AddServices();
        }

        /// <summary>
        /// 此方法在執行階段呼叫，使用此方法定義HTTP Request管線
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // 開發頁面設定
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMiniProfiler();
            }

            // 加入安全標頭
            app.UseSecurityHeaders(new HeaderPolicyCollection()
                .AddFrameOptionsDeny()
                .AddXssProtectionBlock()
                .AddContentTypeOptionsNoSniff()
                .AddReferrerPolicyStrictOriginWhenCrossOrigin()
                .RemoveServerHeader());

            // 當Request中斷則取消查詢
            app.Use(async (context, next) => {
                var interceptor = context.RequestServices.GetService<RequestCancelInterceptor>();
                context.RequestAborted.Register(() => {
                    foreach (var command in interceptor.Commands)
                    {
                        try
                        {
                            command.Cancel();
                        }
                        catch { }
                    }
                });
                await next();
            });


            // 轉發標頭
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.All
            });

            // 使用路由
            app.UseRouting();

            // 回應緩衝
            app.UseResponseBuffering();

            // 使用身分授權
            app.UseAuthorization();

            // 路由端點設定
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
            });

            // 加入SwaggerUI以及OpenAPI
            app.UseOpenApiAndSwaggerUi3();

            #region SPA設定
            // 使用靜態檔案
            app.UseStaticFiles();

            // 當找不到index.html時的處理
            app.Use(async (context, next) => {
                try
                {
                    await next();
                }
                catch
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;
                }
            });

            // 使用SPA
            app.UseSpa(config => { });
            #endregion
        }
    }
}
