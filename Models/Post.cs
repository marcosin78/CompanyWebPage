namespace CompanyWebMarcBravo.Models
{
   public class Post
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Text { get; set; }
    public string? ImagePath { get; set; }
    public DateTime CreatedAt { get; set; }
} 

}
