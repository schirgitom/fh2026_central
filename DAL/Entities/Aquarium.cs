using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public abstract class Aquarium : Entity
{
    [Required]
    public String Name { get; set; }
    
    [Required]
    
    public String OwnerId { get; set; }

    public Double Depth { get; set; }

    public Double Height { get; set; }

    public Double Length { get; set; }

    public Double Liters { get; set; }
}
