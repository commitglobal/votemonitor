namespace Vote.Monitor.Core.Exceptions;
public class NotFoundException<T> : Exception
{
    public T? Entity { get; set; }

    public NotFoundException() { }

    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
}
