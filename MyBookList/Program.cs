using Microsoft.EntityFrameworkCore;
using MyBookList.Core.Entities;
using MyBookList.Core.Interfaces;
using MyBookList.Infrastructure.Data;
using MyBookList.Infrastructure.Services;
using MyBookList.Services;

namespace MyBookList;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                               throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                options.SignIn.RequireConfirmedAccount = false;

                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddRazorPages();
        
        builder.Services.AddTransient<ICommentService, CommentService>();
        builder.Services.AddTransient<IThumbnailService, ThumbnailService>();
        builder.Services.AddTransient<IBookService, BookService>();
        builder.Services.AddTransient<IRatingService, RatingService>();
        builder.Services.AddTransient<IAuthorService, AuthorService>();
        builder.Services.AddTransient<ISubjectService, SubjectService>();

        builder.Logging.AddConsole(config => config.TimestampFormat = "[dd/MM/yy HH:mm:ss:fff] ");

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<OpenLibrarySeeder>>();
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var configuration = services.GetRequiredService<IConfiguration>();

                context.Database.EnsureCreated();
                OpenLibrarySeeder.Seed(context, configuration, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}