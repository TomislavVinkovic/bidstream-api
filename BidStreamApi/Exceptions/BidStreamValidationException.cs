namespace RealWorld.Exceptions;

public class BidStreamValidationException : Exception
{
    public string Field { get; }
    public BidStreamValidationException(string field, string message) : base(message)
    {
        Field = field;
    }
}