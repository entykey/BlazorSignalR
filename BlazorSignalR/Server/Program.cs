using BlazorSignalR.Server.Data;
using BlazorSignalR.Server.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// add signalr
builder.Services.AddSignalR();

// add response compression
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
        new[] { "application/octet-stream" });
});


#region SwaggerUI:
// Add Swagger UI for api debugging:
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo  // Microsoft.OpenApi.Models
    {
        Version = "v1",
        Title = "Tứng hay ho's WebApi",
        Description = "ENTYKEY AspNetCore7.0 WebApi",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Nguyễn Hữu Anh Tuấn's facebook",
            Email = "4701104228@student.hcmue.edu.vn",   // string.Empty
            Url = new Uri("https://fb.com/nguyen.tuan.entykey"),
        },
        License = new OpenApiLicense
        {
            Name = "Use under LICX",
            Url = new Uri("https://example.com/license"),
        }
    });
    // Configuring Swagger UI Authorization with Swagger
    #region Accepting Bearer Token:
    // tutorial: https://code-maze.com/swagger-authorization-aspnet-core
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
            {
                new OpenApiSecurityScheme
                {
                     Reference = new OpenApiReference
                     {
                         Type=ReferenceType.SecurityScheme,
                         Id="Bearer"
                     }
                },
                new string[]{}
            }
    });
    // End of Configuring Authorization with Swagger UI to accept bearerJWT
    #endregion
});
#endregion

#region DbContext & Identity Auth:
builder.Services.AddDbContext<BooksDbContext>(options =>
     options.UseSqlServer(builder.Configuration.GetConnectionString("BlazorSignalRConnectionString"))); // (Microsoft.EntityFrameworkCore.SqlServer)

// to use 'AddDefaultIdentity': install package 'Microsoft.AspNetCore.Identity.UI' !!! NET 7
//builder.Services.AddDefaultIdentity<ApplicationUser>()
//    .AddUserManager<CustomUserManager>()    // use the implemented CustomUserManager with password hashing algorithm replaced by SHA256
//    .AddSignInManager<CustomSignInManager<ApplicationUser>>()   // only for MVC projects with cookie auth to use SHA256 password hashing algorithm
//    .AddRoles<IdentityRole>()
//    .AddEntityFrameworkStores<AppDbContext>()

//#region Email Confirmation (1) (using EmailService;)
//    .AddTokenProvider<EmailConfirmationTokenProvider<ApplicationUser>>("emailconfirmation");
//#endregion

//builder.Services.Configure<IdentityOptions>(options =>
//{
//    // Thiết lập về Password
//    options.Password.RequireDigit = true;  // 0 - 9
//    options.Password.RequireNonAlphanumeric = false; // @!#$%^&*
//    options.Password.RequireLowercase = false;
//    options.Password.RequireUppercase = false;
//    options.Password.RequiredUniqueChars = 0;
//    options.Password.RequiredLength = 4;

//    // Cấu hình về User.
//    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
//        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
//    options.User.RequireUniqueEmail = true; // Every email is unique! (ko trùng email)

//    // Cấu hình đăng nhập. (important!!!)
//    // user may get 401 satus code in authorized enpoints after registration if email is not confirm !!!
//    options.SignIn.RequireConfirmedPhoneNumber = false; // Xác thực số điện thoại
//    options.SignIn.RequireConfirmedEmail = true; // Cấu hình confirm email after register (email must exists)
//    options.User.RequireUniqueEmail = true;

//    // EmailConfirmation (2):
//    options.Tokens.EmailConfirmationTokenProvider = "emailconfirmation";

//});
#endregion


//#region Email Confirmation (3)
//// https://www.codeproject.com/Articles/1272172/Require-Confirmed-Email-in-ASP-NET-Core-2-2-Part-1
//builder.Services.Configure<DataProtectionTokenProviderOptions>(opt =>
//    opt.TokenLifespan = TimeSpan.FromHours(2));

//builder.Services.Configure<EmailConfirmationTokenProviderOptions>(opt =>
//    opt.TokenLifespan = TimeSpan.FromDays(3));
//var emailConfig = builder.Configuration
//                .GetSection("EmailConfiguration")
//                .Get<EmailConfiguration>();
//builder.Services.AddSingleton(emailConfig);
//builder.Services.AddScoped<IEmailSender, EmailSender>();
//#endregion


//#region LocalStorage Jwt Authentication:

//var key = Encoding.UTF8.GetBytes(builder.Configuration["ApplicationSettings:JWT_Secret"]); //from appsettings.json

//// @Warning: if use Cookie to authenticate private endpoints, disable these line below
//// cuz ASP.NET Core can only use ONE auth scheme (LocalSotrage or Cookie) only!!! 

//builder.Services.AddAuthentication(x =>
//{
//    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(x =>
//{
//    x.RequireHttpsMetadata = false;
//    x.SaveToken = false;
//    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
//    {
//        ValidateIssuerSigningKey = true,
//        IssuerSigningKey = new SymmetricSecurityKey(key),
//        ValidateIssuer = false,
//        ValidIssuer = "https://localhost:7000", //some string, normally web url,  
//        ValidateAudience = false,
//        ClockSkew = TimeSpan.Zero
//    };
//});
//#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Use Swagger UI to debug API: (https://localhost:{PORT}/swagger) // PORT is in launchSettings / "applicationUrl"
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "ENTYKEY AspNetCore 7 WebApi v1");
        c.InjectStylesheet("/swagger-custom/swagger-custom-styles.css");  //Added Code for custom Swagger theme (pref: https://blog.rashik.com.np/customize-swagger-ui-with-custom-logo-and-theme-color)
        c.InjectJavascript("/swagger-custom/swagger-custom-script.js", "text/javascript");  //Added Code for custom Swagger theme
    });

    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

// SignalR
app.MapHub<BroadcastHub>("/broadcastHub");

app.Run();
