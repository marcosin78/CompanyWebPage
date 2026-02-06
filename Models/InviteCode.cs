namespace CompanyWebMarcBravo.Models
{
    public class InviteCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public bool IsUsed { get; set; } = false;
    }
}