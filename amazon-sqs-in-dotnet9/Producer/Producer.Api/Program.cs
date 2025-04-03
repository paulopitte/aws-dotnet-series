using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;
using Contracts;
using Producer.Api.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
//builder.Services.AddAWSService<IAmazonSQS>();
var app = builder.Build();



if (app.Environment.IsDevelopment())
    app.MapOpenApi();





app.MapPost("/register", async (
    //IAmazonSQS sqs,
    ILogger<Program> logger, UserRegistrationCommand command) =>
{
    AmazonSQSClient? sqs = null;
    var queueName = builder.Configuration["AmazonSqs:QueueName"]!;
    var queueUrl = builder.Configuration["AmazonSqs:QueueUrl"]!;

    // VALIDA A CONSISTENCIA DE DADOS DO COMANDO
    // APLICA REGRAS, PERSISTE EM BASE ETC...
    var userId = Guid.NewGuid();
    // CRIA O EVENTO DE DOMINIO PARA SER PUBLICADO NO MESSAGEBROKER  
    var userRegisteredEvent = new UserRegisteredEvent(userId, command.UserName, command.Email);



    try
    {   
      
        var options = new AmazonSQSConfig
        {
            RegionEndpoint = RegionEndpoint.USEast1
        };

        var credentials = new BasicAWSCredentials(
            builder.Configuration["AmazonSqs:AccessKey"],
            builder.Configuration["AmazonSqs:SecretKey"]
        );
        sqs = new AmazonSQSClient(credentials, options);
        var response = await sqs.GetQueueUrlAsync(queueName);
        queueUrl = response.QueueUrl;
    }
    catch (QueueDoesNotExistException)
    {
        logger.LogInformation("Queue {queueName} doesn't exist. Creating...", queueName);
        var response = await sqs.CreateQueueAsync(queueName);
        queueUrl = response.QueueUrl;
    }

    // CRIA SQS REQUEST
    var sendMessageRequest = new SendMessageRequest()
    {
        QueueUrl = queueUrl,
        MessageBody = JsonSerializer.Serialize(userRegisteredEvent)
    };
    logger.LogInformation("Publishing message to Queue {queueName} with body : \n {request}", queueName, sendMessageRequest.MessageBody);

    // ENVIA SQS REQUEST
    var result = await sqs.SendMessageAsync(sendMessageRequest);
    return Results.Ok();
});


app.UseHttpsRedirection();
app.Run();
