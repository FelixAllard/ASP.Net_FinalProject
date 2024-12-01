using System.ComponentModel.DataAnnotations;

namespace FinalProject.Data;

public class Painting
{
    [Key]
    public int paintingId { get; set; } 

    [Required]
    [StringLength(100)]
    public string title { get; set; } 

    [Required]
    [StringLength(100)]
    public string artist { get; set; } 

    [Required]
    public string medium { get; set; } 

    [Required]
    public string dimensions { get; set; } 
    
    [Required]
    [DataType(DataType.Currency)]
    public decimal price { get; set; } 

    public string story { get; set; } 

    [Required]
    [StringLength(255)]
    public string imageUrl { get; set; } 

    [Required]
    public int exhibitionId { get; set; } 

    public Exhibition exhibition { get; set; } 

    public int rating { get; set; } 
}