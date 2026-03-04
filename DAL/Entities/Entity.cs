
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DAL.Entities
{
    public abstract class Entity : IEntity
    {
        [Key]
        [JsonPropertyName("id")]
        public string ID { get; set; } = string.Empty;

 
    }
}
