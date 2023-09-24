using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Url_Shortener;
using Url_Shortener.Entities;
using Url_Shortener.Models;
using Url_Shortener.Services;
using Url_Shortener.Validation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationContext>(o => o.UseSqlite("Data Source=Url.db"));

builder.Services.AddScoped<UrlShorteningServices>();
builder.Services.AddScoped<IValidator<RegisterRequest> , RegisterRequestValidator>();
builder.Services.AddScoped<IValidator<LoginRequest> , LoginRequestValidator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddAuthentication().AddJwtBearer(option =>
{
    option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("this world shell know pain"))
    };
});

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

//EndPoints 

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

app.MapPost("api/register", async (RegisterRequest request, IValidator<RegisterRequest> validator , IUserService _userService) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
    {
        return Results.BadRequest("register request is not valid !");
    }
    
    return await _userService.Register(request.Username , request.Password) ? Results.Ok("User Add") : Results.BadRequest("Error");
    
});

app.MapPost("api/login", async (LoginRequest request , IValidator<LoginRequest> validator , IUserService _userService) =>
{
    var validationResult = await validator.ValidateAsync(request);
    if(!validationResult.IsValid)
    {
        return Results.BadRequest("login request is not valid");
    }
    var user = _userService.FindUser(request.Username);
    if (user.Result is null) 
    {
        return Results.BadRequest("User no exist");
    }
    if (!BCrypt.Net.BCrypt.Verify(request.Password ,user.Result.PasswordHash ))
    {
        return Results.BadRequest("User no exist");
    }
    string token;
    _userService.Login(user.Result, out token);
    return Results.Ok(token);
});

app.Run();

