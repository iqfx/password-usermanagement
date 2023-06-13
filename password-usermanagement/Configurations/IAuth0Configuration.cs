namespace password_usermanagement.Configurations;

public interface IAuth0Configuration
{
    public string domain { get; set; }
    public string clientId { get; set; }
    public string clientSecret { get; set; }
}