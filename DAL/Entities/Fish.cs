namespace DAL.Entities;

public class Fish : Animal
{
    public DateTime DeathDate { get; set; } = DateTime.MinValue;
}
