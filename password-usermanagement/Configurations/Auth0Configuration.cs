namespace password_usermanagement.Configurations;

public class Auth0Configuration
{
    public string domain;
    public string clientId;
    public string clientSecret;

    public Auth0Configuration(string domain, string clientId, string clientSecret)
    {
        this.domain = domain;
        this.clientId = clientId;
        this.clientSecret = clientSecret;
    }
}