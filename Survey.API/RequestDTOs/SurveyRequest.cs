﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Survey.API.RequestDTOs;
public class SurveyRequest
{
    public string? title { get; set; }
    public string? answers { get; set; }
}
