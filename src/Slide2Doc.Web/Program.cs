using FluentValidation;
using Slide2Doc.Application;
using Slide2Doc.Domain.Entities;
using Slide2Doc.Infrastructure;
using Slide2Doc.Web.Services;
using Slide2Doc.Web.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddLogging();
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddScoped<AppState>();
builder.Services.AddValidatorsFromAssemblyContaining<UploadRequestValidator>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
