using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Survey.API.RequestDTOs;

namespace Survey.API.Models;

public class SurveyModel
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("surveyId")]
    public Guid Id { get; set; } = Guid.NewGuid();
    [Column("code")]
    public string? respCode { get; set; }
    [Column("message")]
    public string? message { get; set; }
    [Column("jsonData")]
    public string? jsonData { get; set; } 
    [Column("CreatedOn")]
    public DateTime DateTime { get; set; } = DateTime.Now;
}
