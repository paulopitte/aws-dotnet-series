using Amazon.SQS;
using Consumer.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSQS>();
builder.Services.AddHostedService<UserRegisteredEventConsumer>();





var app = builder.Build();



app.UseHttpsRedirection();



app.Run();


