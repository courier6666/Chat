namespace Chat.Message;

public static class MessageFactory
{
    public static IdentificationMessage CreateIdentificationMessage()
    {
        return new IdentificationMessage()
        {
            ProvidedId = Guid.NewGuid(),
            TimeUtc = DateTime.UtcNow,
        };
    }

    public static ErrorMessage CreateErrorMessage(string error)
    {
        return new ErrorMessage()
        {
            Error = error,
            TimeUtc = DateTime.UtcNow,
        };
    }
}