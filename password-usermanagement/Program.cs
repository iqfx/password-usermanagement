using Microsoft.EntityFrameworkCore;
using password_usermanagement.Data;
using password_usermanagement.Mappers;
using password_usermanagement.Models;
using password_usermanagement.Queue;
using password_usermanagement.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();

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
builder.Services.AddSingleton<IRabbitMQConnection, RabbitMQConnection>(sp =>
{
    var hostname = builder.Configuration["RabbitMQ:Uri"];
    var username = builder.Configuration["RabbitMQ:Username"];
    var password = builder.Configuration["RabbitMQ:Password"];
    return new RabbitMQConnection(hostname, username, password);
});
builder.Services.AddScoped<IRabbitMQPublish>(sp =>
{
    var config = sp.GetRequiredService<IRabbitMQConnection>();
    return new RabbitMQPublish(config);
});
builder.Services.AddScoped<IRoleService, RoleService>();

builder.Services.AddScoped<RabbitMQListener>(sp =>
{
    var connection = sp.GetRequiredService<IRabbitMQConnection>();
    var roleService = sp.GetRequiredService<IRoleService>();
    var publisher = sp.GetRequiredService<IRabbitMQPublish>();

    return new RabbitMQListener(connection, publisher, roleService);
});
builder.Services.AddScoped<BackgroundService>(sp => sp.GetRequiredService<RabbitMQListener>());


var app = builder.Build();
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetService<DatabaseContext>();

context.Database.EnsureCreated();
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