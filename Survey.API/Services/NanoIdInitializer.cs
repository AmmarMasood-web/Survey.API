using NanoidDotNet;
using Survey.API.Interfaces;

namespace Survey.API.Services;
public class NanoIdInitializer : INanoIdInitializer
{
    public string ApplicationId { get; private set; } = "NotSet";
    public void Initialize()
    {
        ApplicationId = Nanoid.Generate();
    }
}