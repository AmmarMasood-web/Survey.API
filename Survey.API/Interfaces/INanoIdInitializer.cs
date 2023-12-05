namespace Survey.API.Interfaces;
public interface INanoIdInitializer
{
    void Initialize();
    string ApplicationId { get; }
}
