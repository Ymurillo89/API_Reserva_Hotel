using API_Hotel.Notifications;
using API_Hotel.Infrastructure.Messaging;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
