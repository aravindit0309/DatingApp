namespace DatingApp.API.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] PassswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}