using Newtonsoft.Json;

namespace DocumentDbDemo.Models
{
public class Person
{
    [JsonProperty(PropertyName = "personId")]
    public string PersonId { get; set; }

    [JsonProperty(PropertyName = "firstName")]
    public string FirstName { get; set; }

    [JsonProperty(PropertyName = "lastName")]
    public string LastName { get; set; }

    [JsonProperty(PropertyName = "friends")]
    public int[] Friends { get; set; }
}
}
    