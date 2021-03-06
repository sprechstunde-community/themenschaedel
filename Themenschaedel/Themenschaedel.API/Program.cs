using Themenschaedel.API.Services;
using Themenschaedel.API.Worker;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseSentry("https://5e34602797cf4674aefaed441f03092b@sentry.alyra.dev/5");

// Add services to the container.
builder.Services.AddSingleton<ISearchService, MeilisearchService>();
builder.Services.AddSingleton<DapperService>();
builder.Services.AddSingleton<IDatabaseService, DatabaseService>();
builder.Services.AddHttpClient<IMailService, MailService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MailjetURL"]);
});
builder.Services.AddSingleton<IAuthenticationService, AuthenticationService>();
builder.Services.AddSingleton<IClaimService, ClaimService>();
// Add Workers
builder.Services.AddHostedService<RssFeedScrapperWorker>();
builder.Services.AddHostedService<ExpiredClaimClearingWorker>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
