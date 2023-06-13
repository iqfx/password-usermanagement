using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using password_usermanagement.Configurations;
using password_usermanagement.Data;
using password_usermanagement.Models;
using password_usermanagement.Queue;
using RabbitMQ.Client;

namespace password_usermanagement.Services;

public class UserService:  IUserService
{
    private DatabaseContext _context;
    private static HttpClient _client;
    private readonly IRabbitMQPublish _publisher;
    private readonly IRabbitMQConnection _connection;
    private IModel _channel;
    private readonly string _queueName = "userRoleRequestResponse";
    private readonly string _exchangeName = "authService";
    private readonly string _routingKey = "roleResponse";
    private IAuth0Configuration _auth0Config;
    public UserService(DatabaseContext context, IRabbitMQPublish publisher,IRabbitMQConnection connection, IAuth0Configuration auth0Domain)
    {
        _context = context;
        _client = new HttpClient();
        _publisher = publisher;
        _connection = connection;
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct, durable: true);
        _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(_queueName, _exchangeName, _queueName);
        _auth0Config = auth0Domain;
    }
    public async Task<List<User>> GetAll()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User> GetById(Guid id)
    {
        return await _context.Users.FindAsync(id) ?? throw new ArgumentException();
    }

    public string GetUserIdFromHeader(string header)
    {
        string jwt = header.Replace("Bearer ", string.Empty);

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadToken(jwt);
        var tokenS = jsonToken as JwtSecurityToken;
        var sub = tokenS.Claims.First(claim => claim.Type == "sub").Value;
        return sub;
    }

    public async Task<User> GetUserByUserId(string id)
    {
        try
        {
            return await _context.Users.Where(u => u.userId == id).Include(r => r.Roles).SingleOrDefaultAsync() ??
                   throw new ArgumentException();
        }
        catch (ArgumentException e)
        {
            var user = await SaveNewUser(id);
            return user;
        }

    }

    public async Task<User> SaveNewUser(string userId)
    {
        var user = new User()
        {
            userId = userId
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> SaveUserSetPasswordSetToTrue(User user)
    {
        var userFromDb = await GetUserByUserId(user.userId);
        userFromDb.HasSetMasterPassword = true;
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteUser(User userToDelete, User actor)
    {
        if (true)
        {
            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();
            // await _publisher.Publish(userToDelete.id, "authService", "deleteRequest", null);
            string apiUrl = $"https://{_auth0Config.domain}/api/v2/users/{userToDelete.userId}";


            // var jsonRequestBody = JsonConvert.SerializeObject(requestBody);
            // var content = new StringContent(jsonRequestBody, Encoding.UTF8, "application/json");
            var accessToken = GetAccessToken(_auth0Config.clientId, _auth0Config.clientSecret);

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            HttpResponseMessage response = await _client.DeleteAsync(apiUrl);
            if (!response.IsSuccessStatusCode)
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Failed to delete user. Error: " + errorMessage);
            }
            else
            {
                Console.WriteLine("User deleted successfully.");
            }

        }

    }
    private string GetAccessToken(string clientId, string clientSecret)
    {
        string apiUrl = $"https://{_auth0Config.domain}/oauth/token";

        var requestBody = new Dictionary<string, string>
        {
            { "grant_type", "client_credentials" },
            { "scope", "delete:users" },
            { "client_id", clientId },
            { "client_secret", clientSecret },
            { "audience", $"https://{_auth0Config.domain}/api/v2/" }
        };

        using (var client = new HttpClient())
        {
            var response = client.PostAsync(apiUrl, new FormUrlEncodedContent(requestBody)).Result;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                dynamic tokenInfo = JsonConvert.DeserializeObject(responseContent);
                string accessToken = tokenInfo.access_token;
                return accessToken;
            }
            else
            {
                string errorMessage = response.Content.ReadAsStringAsync().Result;
                Console.WriteLine("Failed to obtain access token. Error: " + errorMessage);
                return null;
            }
        }
    }
}