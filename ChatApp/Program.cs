using ChatApp.BLL.Infrastructure.JwtHelper;
using ChatApp.BLL.Interfaces;
using ChatApp.BLL.Services;
using ChatApp.DAL;
using ChatApp.DAL.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Blazored.LocalStorage;
using ChatApp.DAL.Infrastructure.Repositories;
using ChatApp.DAL.Interfaces;
using ChatApp.DAL.Infrastructure;
using ChatApp.IntermediateServices;
using ChatApp.BLL;
using ChatApp.BLL.Hubs;
using Microsoft.AspNetCore.ResponseCompression;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddHttpClient();
builder.Services.AddAutoMapper(typeof(AutomapperProfile));
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});

builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IIntermediateUserService, IntermediateUserService>();
builder.Services.AddTransient<IRoomRepository, RoomRepository>();
builder.Services.AddTransient<IMessageRepository, MessageRepository>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddTransient<IChatService, ChatService>();
builder.Services.AddTransient<IMessageService, MessageService>();
builder.Services.AddTransient<IIntermediateChatService, IntermediateChatService>();
builder.Services.AddTransient<IIntermediateMessageService, IntemediateMessageService>();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
     options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});


builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
}).AddEntityFrameworkStores<AppDbContext>()
  .AddDefaultTokenProviders();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

builder.Services
                .AddAuthorization()
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Issuer,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                        ClockSkew = TimeSpan.Zero
                    };
                });


var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapBlazorHub();
    endpoints.MapFallbackToPage("/_Host");
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chat");
});

app.Run();
