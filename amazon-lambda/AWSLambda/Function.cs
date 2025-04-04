using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace AWSLambda;

public class Function
{

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public CreateProductResponse FunctionHandler(CreateProductRequest input, ILambdaContext context)
    {
        return new CreateProductResponse()
        {
            ProductId = Guid.NewGuid().ToString(),
            Name = input.Name,
            Description = input.Description
        };
    }

    public record CreateProductRequest
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
    public record CreateProductResponse
    {
        public string? ProductId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
