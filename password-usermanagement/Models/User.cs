using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace password_usermanagement.Models;
[Table("users")]
public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid id { get; set; }
    public string userId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool HasSetMasterPassword { get; set; }
    public ICollection<Role> Roles { get; set; }

    public User()
    {
        Roles = new HashSet<Role>();
    }
}