using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace DAL.Entities;

public class User : Entity
{
    public string Email { get; set; }
    public string Firstname { get; set; }
    public string Lastname { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]
    [BsonIgnore]
    public string Password { get; set; }

    [JsonIgnore]
    [IgnoreDataMember]
    public string HashedPassword { get; set; }

    public Boolean Active { get; set; }
}