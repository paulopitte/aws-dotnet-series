namespace Contracts;
public record UserRegisteredEvent : IEvent
{
    private const int GMT = -3;

    public Guid UserId { get; }
    public string UserName { get; }
    public string Email { get; }
    public DateTime CreatedDate { get; }

    public UserRegisteredEvent(Guid userId, string userName, string email)
    {
        UserId = userId;
        UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        CreatedDate = DateTime.UtcNow.AddHours(GMT);
    }
}