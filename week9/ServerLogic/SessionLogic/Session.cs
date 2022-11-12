namespace HttpServer.ServerLogic.SessionLogic;

public class Session
{
    public readonly Guid Id;
    public readonly int AccountId;
    public readonly string Email;
    public readonly DateTime CreateDateTime;

    public Session(Guid id, int accountId, string email, DateTime created)
    {
        Id = id;
        AccountId = accountId;
        Email = email;
        CreateDateTime = created;
    }
}