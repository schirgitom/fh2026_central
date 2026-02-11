using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace DAL.Entities;

public class User : Entity
{
    public string Email { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }

    [JsonIgnore]
    public string Password { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]
    public string HashedPassword { get; set; }

    public Boolean Active { get; set; }
}