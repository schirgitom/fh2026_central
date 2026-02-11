using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public abstract class Animal : Entity
{
    [Required]
    public string AquariumId { get; set; }

    [Required]
    public string Name { get; set; }

    public DateTime Inserted { get; set; }

    public int Amount { get; set; }

    public string Description { get; set; }
}
