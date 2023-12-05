using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.API.Interfaces;
public interface INanoIdInitializer
{
    void Initialize();
    string ApplicationId { get; }
}
