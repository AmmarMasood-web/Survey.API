using Survey.API.RequestDTOs;
using Survey.API.ResponseDTOs;

namespace Survey.API.Interfaces;
public interface IAuthentication
{
    AuthenticationDTO generateToken(AuthenticationRequest authenticationRequest);
}
