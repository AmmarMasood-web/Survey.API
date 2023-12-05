using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.API.RequestDTOs;
public class AuthenticationRequest
{
    public required string clientId { get; set; }
}
