namespace CompanyWebMarcBravo.Models
{

    public enum UserRole
    {
        Employee,
        DepartmentHead,
        Administrator

    }

public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }

}