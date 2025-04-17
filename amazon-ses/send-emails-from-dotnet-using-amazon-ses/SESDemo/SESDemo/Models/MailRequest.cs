namespace SESDemo.Models;

public record MailRequest
{
    public string? Recepient { get; set; }
    public string? Subject { get; set; }
    public string? Body { get; set; }
}