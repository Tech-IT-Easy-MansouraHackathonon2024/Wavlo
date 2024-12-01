using ChatApp.Models;
using Microsoft.AspNetCore.Identity;

namespace Authentication.Data;

public class ApplicationUser: IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? VerificationCode { get; set; }
    public DateTime? CodeExpiration { get; set; }
    public ApplicationUser() : base()
    {
        Chats = new List<ChatUser>();
    }
    public ICollection<ChatUser> Chats { get; set; }

}