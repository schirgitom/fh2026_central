namespace DAL.Entities;

public enum CoralTyp
{
    HardCoral,
    SoftCoral
}

public class Coral : Animal
{
    public CoralTyp CoralTyp { get; set; }
}
