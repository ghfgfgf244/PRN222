using BusinessObjects;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Services;

namespace Web_
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppointmentsDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyStockDB")));

            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IAppointmentServices, AppointmentServices>();
            builder.Services.AddScoped<IDoctorServices, DoctorServices>();
            builder.Services.AddScoped<IPatientServices, PatientServices>();
            builder.Services.AddScoped<IUserServices, UserServices>();
            builder.Services.AddScoped<IExternalIntegrationService, ExternalIntegrationService>();
            builder.Services.AddScoped<IAccountServices, AccountServices>();

            // Thêm dịch vụ xác thực Cookie + Google
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                //muốn người dùng truy cập trực tiếp đến trang đăng nhập
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;//GoogleDefaults.AuthenticationScheme;
            })
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login"; // Đường dẫn trang đăng nhập
                    options.AccessDeniedPath = "/Account/AccessDenied"; // Trang lỗi truy cập
                })
                .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
                {
                    IConfigurationSection googleAuthNSection = builder.Configuration.GetSection("Authentication:Google");

                    // Thiết lập ClientID và ClientSecret để truy cập API google
                    options.ClientId = googleAuthNSection["ClientId"];
                    options.ClientSecret = googleAuthNSection["ClientSecret"];
                    options.CallbackPath = "/Account/signin-google"; // Đường callback mặc định
                });

            // Cấu hình Authorization
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireClaim("Role", "Admin"));
                options.AddPolicy("DoctorOnly", policy => policy.RequireClaim("Role", "Doctor"));
                options.AddPolicy("PatientOnly", policy => policy.RequireClaim("Role", "Patient"));
            });


            builder.Services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(30);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            });


            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // Hỗ trợ upload file tối đa 10MB
            });
            // Add services to the container.
            builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
            {
                options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
            }).AddMvcOptions(options =>
            {
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
                    _ => "Trường này không được để trống.");
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage(); // ← thêm dòng này nếu thiếu
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();

            app.UseAuthentication(); // <--- QUAN TRỌNG
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
