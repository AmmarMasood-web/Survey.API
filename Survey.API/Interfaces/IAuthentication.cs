using Survey.API.RequestDTOs;
using Survey.API.ResponseDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.API.Interfaces;
public interface IAuthentication
{
    AuthenticationDTO generateToken(AuthenticationRequest authenticationRequest);
}
