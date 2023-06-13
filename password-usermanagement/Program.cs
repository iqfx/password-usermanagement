using Microsoft.EntityFrameworkCore;
using password_usermanagement.Configurations;
using password_usermanagement.Data;
using password_usermanagement.Mappers;
using password_usermanagement.Queue;
using password_usermanagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<RoleService>();

builder.Services.AddAutoMapper(typeof(RoleMapper));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<DatabaseContext>(options =>
        options.UseMySQL(builder.Configuration["UserManagement:ConnectionString"]));
}
else
{
    builder.Services.AddDbContext<DatabaseContext>(options =>
        options.UseSqlServer(builder.Configuration["UserManagement:ConnectionString"]));
}


// Configure RabbitMQ connection
builder.Services.AddScoped<RabbitMQConnection>(sp =>
{
    var hostname = builder.Configuration["RabbitMQ:Uri"];
    var username = builder.Configuration["RabbitMQ:Username"];
    var password = builder.Configuration["RabbitMQ:Password"];
    return new RabbitMQConnection(hostname, username, password);
});
builder.Services.AddScoped<RabbitMQPublish>(sp =>
{
    var config = sp.GetRequiredService<RabbitMQConnection>();
    return new RabbitMQPublish(config);
});

builder.Services.AddHostedService(provider =>
{
    return new RabbitMQListener(provider);
});
builder.Services.AddScoped<IAuth0Configuration>(sp=>
{
    var domain = builder.Configuration["Auth0:Domain"];
    var clientId = builder.Configuration["Auth0:Client_Id"];
    var clientSecret = builder.Configuration["Auth0:Client_Secret"];

    return new Auth0Configuration(domain, clientId, clientSecret);
});
builder.Services.AddScoped<IUserService>(sp =>
{
    var conn = sp.GetService<RabbitMQConnection>();
    var pub = sp.GetService<RabbitMQPublish>();
    var auth0 = sp.GetService<IAuth0Configuration>();
    var context = sp.GetService<DatabaseContext>();
    return new UserService(context, pub, conn, auth0);

});

var app = builder.Build();
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetService<DatabaseContext>();

context.Database.EnsureCreated();
context.Database.Migrate();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();