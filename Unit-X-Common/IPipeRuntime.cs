namespace Unit_X_Common
{
    public interface IPipeRuntime
    {
        string PipeName { get; }
    }
    public interface IPipeClientRuntime : IPipeRuntime
    {
        string ServerName { get; }
    }
}
