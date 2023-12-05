using Survey.API.RequestDTOs;
using Survey.API.ResponseDTOs;

namespace Survey.API.Interfaces;

public interface ISurvey
{
    public Task<SurveyDTO> createSurvey(SurveyRequest request);
}
