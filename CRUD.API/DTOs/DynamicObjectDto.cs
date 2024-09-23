namespace CRUD.API.DTOs
{
    public class DynamicObjectDto
    {
        public string ObjectType { get; set; }
        public Dictionary<string, object> Fields { get; set; }
    }

}
