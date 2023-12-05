using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Survey.API.Classes;
using Survey.API.Database;
using Survey.API.Helpers;
using Survey.API.Interfaces;
using Survey.API.Models;
using Survey.API.RequestDTOs;
using Survey.API.ResponseDTOs;

namespace Survey.API.Services;

public class SurveyService : ISurvey
{
    private readonly DatabaseContext _context;
    private readonly INanoIdInitializer _nanoId;
    private readonly IConfiguration configuration;
    private readonly IMapper _mapper;

    public SurveyService(DatabaseContext context, INanoIdInitializer nanoId, IConfiguration configuration, IMapper mapper)
    {
        _context = context;
        _nanoId = nanoId;
        this.configuration = configuration;
        _mapper = mapper;
    }
    public async Task<SurveyDTO> createSurvey(SurveyRequest request)
    {
        try
        {
            Log.Information("In create survey service with nano id: " + _nanoId.ApplicationId);
            var mappedDto = _mapper.Map<SurveyDTO>(request);
            Log.Information($"Nano id : {_nanoId.ApplicationId} ... Mapped Response = {JsonConvert.SerializeObject(mappedDto)}");
            var saveDto = _mapper.Map<SurveyModel>(mappedDto);
            Log.Information($"Nano id : {_nanoId.ApplicationId} ... Saving Response in db = {JsonConvert.SerializeObject(saveDto)}");
            _context.Add(saveDto);
            await _context.SaveChangesAsync();
            Log.Information("Transaction saved to DB");
            return mappedDto;
        }
        catch (Exception ex)
        {
            Log.Error("Error occured: " + ex.Message);
            return new SurveyDTO
            {
                data = null!
            };
        }
    }
}
