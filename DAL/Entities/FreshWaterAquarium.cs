namespace DAL.Entities;

public class FreshWaterAquarium : Aquarium
{
    public bool HasFreshAir { get; set; }
    
    public bool HasCo2System { get; set; }
}