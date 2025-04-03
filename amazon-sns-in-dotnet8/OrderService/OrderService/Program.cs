using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using OrderService.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonSimpleNotificationService>();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/OrderCreated", async (CreateOrderRequest request, 
            IAmazonSimpleNotificationService sns) =>
{

    // CRIAÇÃO DA NOTIFICAÇÃO
    var notification = new OrderCreatedNotification(request.OrderId, request.CustomerId, request.ProductDetails);

    // CRIAÇÃO DO TÓPICO
    var topicName = builder.Configuration["AmazonSns:TopicName"]!;
    var topicArn = builder.Configuration["AmazonSns:TopicArn"]!;


    var topicExists = await sns.FindTopicAsync(topicName);
    if (topicExists is null)    
        topicArn = topicExists.TopicArn;    
    else
    {
        var newTopic = await sns.CreateTopicAsync(topicName);
        topicArn = newTopic.TopicArn;
    }

    // OBJETO PARA PUBLICAÇÃO
    PublishRequest publishRequest = new()
    {
        TopicArn = topicArn,
        Message = JsonSerializer.Serialize(notification),
        Subject = $"Order#{request.OrderId}"
    };

    publishRequest.MessageAttributes.Add("Scope", new MessageAttributeValue()
    {
        DataType = "String",
        StringValue = "Lambda"
    });

    await sns.PublishAsync(publishRequest);
});

app.Run();