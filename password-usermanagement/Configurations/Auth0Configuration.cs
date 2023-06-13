namespace password_usermanagement.Configurations;

public class Auth0Configuration: IAuth0Configuration
{
    public string domain { get; set; }
    public string clientId { get; set; }
    public string clientSecret { get; set; }

    public Auth0Configuration(string domain, string clientId, string clientSecret)
    {
        this.domain = domain;
        this.clientId = clientId;
        this.clientSecret = clientSecret;
    }
}