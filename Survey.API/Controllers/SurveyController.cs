using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Survey.API.Database;
using Survey.API.Interfaces;
using Survey.API.RequestDTOs;
using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace SurveyBackend.API.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/surveys")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        private readonly DatabaseContext context;
        private readonly ISurvey survey;
        private readonly INanoIdInitializer nanoId;

        public SurveyController(DatabaseContext context, ISurvey survey, INanoIdInitializer nanoId)
        {
            this.context = context;
            this.survey = survey;
            this.nanoId = nanoId;
        }

        [HttpGet]
        [Route("getSurvey")]
        public async Task<IActionResult> GetSurvey()
        {
            try
            {
                var surveys = await context.Surveys.ToListAsync();
                Log.Information("{0} : ", nanoId.ApplicationId + "  Get Survey Response : " + JsonConvert.SerializeObject(surveys));
                return Ok(surveys);
            }
            catch (Exception ex)
            {
                Log.Error("{0} :", nanoId.ApplicationId + "Error occurred in GetSurvey controller:" + ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet]
        [Route("getSurveyById/{id}")]
        public IActionResult GetSurveyById(Guid id)
        {
            try
            {
                var survey = context.Surveys.Find(id);

                if (survey == null)
                {
                    return NotFound();
                }
                Log.Information("{0} :", nanoId.ApplicationId + " Get SurveyById Response : " + JsonConvert.SerializeObject(survey));
                return Ok(survey);
            }
            catch (Exception ex)
            {
                Log.Error("{0} :", nanoId.ApplicationId + "Error occurred in GetSurveyById controller: " + ex.Message);
                return StatusCode(statusCode: 500, "Internal Server Error");
            }
        }

        [HttpPost]
        [Route("createSurvey")]
        public async Task<IActionResult> CreateSurvey([FromBody] SurveyRequest surveyRequest)
        {
            try
            {
                Log.Debug("Create Survey with nano id: " + nanoId.ApplicationId);
                Log.Information("{0} :", nanoId.ApplicationId + " Survey Request body : " + JsonConvert.SerializeObject(surveyRequest));
                var result = await this.survey.createSurvey(surveyRequest);
                Log.Information("{0} :", nanoId.ApplicationId + " Survey Response body : " + JsonConvert.SerializeObject(result));
                if (result.code == "000")
                {
                    return CreatedAtAction(nameof(GetSurvey), result);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                Log.Error("{0} :", nanoId.ApplicationId + "Error occurred in CreateSurvey controller: " + ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}
