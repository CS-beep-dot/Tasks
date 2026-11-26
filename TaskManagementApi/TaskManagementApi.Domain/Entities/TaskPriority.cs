using System.Text.Json.Serialization;

namespace TaskManagementApi.Domain.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TaskPriority
    {
        Low,
        Medium,
        High
    }
}
