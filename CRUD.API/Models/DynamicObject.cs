namespace CRUD.API.Models
{
    public class DynamicObject
    {
        public int Id { get; set; }
        public string ObjectType { get; set; } 
        public string Data { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }

}
