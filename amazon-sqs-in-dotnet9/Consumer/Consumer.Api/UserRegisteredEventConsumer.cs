using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace Consumer.Api;

public class UserRegisteredEventConsumer(IConfiguration configuration, ILogger<UserRegisteredEventConsumer> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        AmazonSQSClient? sqs = null;
        var queueName = configuration["AmazonSqs:QueueName"]!;
        var queueUrl = configuration["AmazonSqs:QueueUrl"]!;

        logger.LogInformation("Polling Queue {queueName}", queueName);

        try
        {
            var options = new AmazonSQSConfig
            {
                RegionEndpoint = RegionEndpoint.USEast1
            };

            var credentials = new BasicAWSCredentials(
                configuration["AmazonSqs:AccessKey"],
                configuration["AmazonSqs:SecretKey"]
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

        var receiveRequest = new ReceiveMessageRequest()
        {
            QueueUrl = queueUrl
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await sqs.ReceiveMessageAsync(receiveRequest);
            if (response.Messages.Count > 0)
            {
                foreach (var message in response.Messages)
                {
                    logger.LogInformation("Received Message from Queue {queueName} with body as : \n {body}", queueName, message.Body);
                    Task.Delay(2000).Wait();
                    await sqs.DeleteMessageAsync(queueUrl, message.ReceiptHandle);
                }
            }
        }
    }
}
