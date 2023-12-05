using Survey.API.RequestDTOs;

namespace Survey.API.ResponseDTOs;
public class SurveyDTO
{
    public string message { get; set; } = "failure";
    public string code { get; set; } = "001";
    public List<SurveyRequest> data { get; set; } = new List<SurveyRequest>();
}
