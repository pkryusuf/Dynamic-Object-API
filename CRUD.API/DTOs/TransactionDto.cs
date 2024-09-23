namespace CRUD.API.DTOs
{
    public class TransactionDto
    {
        public string MasterObjectType { get; set; }
        public Dictionary<string, object> MasterFields { get; set; }
        public List<DynamicObjectDto> SubObjects { get; set; }
    }

}
