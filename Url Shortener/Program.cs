using Microsoft.EntityFrameworkCore;
using Url_Shortener;
using Url_Shortener.Entities;
using Url_Shortener.Models;
using Url_Shortener.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationContext>(o => o.UseSqlite("Data Source=Url.db"));

builder.Services.AddScoped<UrlShorteningServices>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseCors("AllowReactApp");
app.MapPost("api/shorten", async (
    ShortenUrlRequest request,
    UrlShorteningServices urlShorteningServices,
    ApplicationContext context,
    HttpContext httpContext) =>
{
    if (!Uri.TryCreate(request.Url, UriKind.Absolute, out _))
    {
        return Results.BadRequest("The spacified URL is invalid.");
    }

    var code = await urlShorteningServices.GenerateUniqueCode();

    var shortenedUrl = new ShortenedUrl()
    {
        Id = Guid.NewGuid(),
        LongUrl = request.Url,
        Code = code,
        CraeteOnUtc = DateTime.UtcNow,
        ShortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/api/{code}"
    };

    context.ShortenedUrls.Add(shortenedUrl);
    await context.SaveChangesAsync();

    return Results.Ok(shortenedUrl.ShortUrl);
});


app.MapGet("api/{code}", async (string code , ApplicationContext context) => 
{
    var shortenedUrl = await context.ShortenedUrls.FirstOrDefaultAsync(s => s.Code == code);

    if(shortenedUrl is null)
    {
        return Results.NotFound();
    }
    return Results.Redirect(shortenedUrl.LongUrl);
});

app.Run();

