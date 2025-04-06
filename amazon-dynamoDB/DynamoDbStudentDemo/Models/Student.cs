using Amazon.DynamoDBv2.DataModel;
namespace DynamoStudentManager.Models;
[DynamoDBTable("students")]
public record Student
{
    [DynamoDBHashKey("id")]
    public int? Id { get; set; }

    [DynamoDBProperty("firstname")]
    public string? FirstName { get; set; }

    [DynamoDBProperty("lastname")]
    public string? LastName { get; set; }


}

