using NanoidDotNet;
using Survey.API.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.API.Services;
public class NanoIdInitializer : INanoIdInitializer
{
    public string ApplicationId { get; private set; } = "NotSet";
    public void Initialize()
    {
        ApplicationId = Nanoid.Generate();
    }
}