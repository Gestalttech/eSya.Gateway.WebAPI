using eSya.Gateway.WebAPI.Utility;
using eSya.Gateway.WebAPI.Filters;
using Microsoft.Extensions.Configuration;
using DL_Gateway = eSya.Gateway.DL.Entities;
using eSya.Gateway.IF;
using eSya.Gateway.DL.Repository;
using Microsoft.Extensions.Localization;
using eSya.Gateway.DL.Localization;
using System.Globalization;
using eSya.Gateway.WebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

DL_Gateway.eSyaEnterprise._connString = builder.Configuration.GetConnectionString("dbConn_eSyaEnterprise");

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ApikeyAuthAttribute>();
});

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<HttpAuthAttribute>();
});

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<CultureAuthAttribute>();
});
//Localization

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
                   //new CultureInfo(name:"en-IN"),
                     new CultureInfo("en-US"),
                     new CultureInfo("hi-IN"),
                     new CultureInfo("ar-EG"),
                };
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(culture: supportedCultures[0], uiCulture: supportedCultures[0]);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

});

builder.Services.AddLocalization();
//localization


builder.Services.AddScoped<IeSyaUserRepository, eSyaUserRepository>();
builder.Services.AddScoped<IApplicationRulesRepository, ApplicationRulesRepository>();
builder.Services.AddScoped<ICommonRepository, CommonRepository>();
builder.Services.AddScoped<ILocalizationRepository, LocalizationRepository>();
builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();
builder.Services.AddScoped<ISmsStatementRepository, SmsStatementRepository>();
builder.Services.AddScoped<ISmsSender, SmsSender>();
builder.Services.AddScoped<ISmsReminderRepository, SmsReminderRepository>();
builder.Services.AddScoped<IForgotUserPasswordRepository, ForgotUserPasswordRepository>();

builder.Services.AddScoped<IRazorpayPaymentApi, RazorpayPaymentApi>();

builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseAuthorization();

//Localization

var supportedCultures = new[] { /*"en-IN", */ "en-US", "hi-IN", "ar-EG" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);
//Localization



app.MapControllers();
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=values}/{action=Get}/{id?}");

app.Run();
