
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManagementApi.Application.Repositories;
using TaskManagementApi.Application.Services;
using TaskManagementApi.Authorization;
using TaskManagementApi.Infrastructure.Persistence;
using TaskManagementApi.Infrastructure.Repositories;

namespace TaskManagementApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var issuer = builder.Configuration["Jwt:Issuer"]
                         ?? throw new InvalidOperationException("Jwt:Issuer is missing.");
            var audience = builder.Configuration["Jwt:Audience"]
                           ?? throw new InvalidOperationException("Jwt:Audience is missing.");
            var signingKey = builder.Configuration["Jwt:SigningKey"]
                             ?? throw new InvalidOperationException("Jwt:SigningKey is missing.");

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MapInboundClaims = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30),
                        NameClaimType = "sub",
                        RoleClaimType = "role"
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();

                options.AddPolicy("CanDeleteProjects", p => p.RequireRole("Admin"));
                options.AddPolicy("CanAssignTasks", p => p.RequireRole("Manager", "Admin"));
                options.AddPolicy("CanDeleteTasks", p => p.RequireRole("Manager", "Admin"));
                options.AddPolicy("CanUpdateTasks", p => p.Requirements.Add(new CanUpdateTaskRequirement()));
            });

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            //service registration
            builder.Services.AddSingleton<IAuthorizationHandler, CanUpdateTaskHandler>();

            builder.Services.AddScoped<ITaskService, TaskService>();
            builder.Services.AddScoped<ITaskRepository, EfTaskRepository>();

            builder.Services.AddScoped<IProjectService, ProjectService>();
            builder.Services.AddScoped<IProjectRepository, EfProjectRepository>();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbConnection")));

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }
    }
}
