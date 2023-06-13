using System.Linq.Expressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using password_usermanagement.Configurations;
using password_usermanagement.Data;
using password_usermanagement.Models;
using password_usermanagement.Queue;
using password_usermanagement.Services;
using RabbitMQ.Client;

namespace usermanagement_tests;



public class Tests
{
    private Mock<DatabaseContext> _mockDbContext;
    private Mock<IRabbitMQConnection> _mockRabbitMQConnection;
    private Mock<IRabbitMQPublish> _mockRabbitMQPublish;
    private Mock<IAuth0Configuration> _mockAuth0Config;
    private Mock<IModel> _mockModel;
    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        _mockDbContext = new Mock<DatabaseContext>();
        _mockModel = new Mock<IModel>();
        _mockRabbitMQConnection = new Mock<IRabbitMQConnection>();
        _mockRabbitMQConnection.Setup(m => m.CreateModel()).Returns(_mockModel.Object);
        _mockRabbitMQPublish = new Mock<IRabbitMQPublish>();
        _mockAuth0Config = new Mock<IAuth0Configuration>();
        _userService = new UserService(_mockDbContext.Object, _mockRabbitMQPublish.Object, _mockRabbitMQConnection.Object,_mockAuth0Config.Object);
    }

    [Test]
    public async Task GetUserByUserId()
    {
        //arrange
        string userId = "fhjlkdsfjkhfgksdlajhgfh";
        IEnumerable<User> userList = new List<User>()
        {
            new User
            {
                userId = userId,
                HasSetMasterPassword = false
            },
            new User
            {
                userId = "fdhgfdsghsdgfhfhds",
                HasSetMasterPassword = false
            },
        };
        var mock = userList.BuildMock().BuildMockDbSet();
        _mockDbContext.Setup(m => m.Users).Returns(mock.Object);

        //act
        User userFromDb = await _userService.GetUserByUserId(userId);
        // assert
        Assert.AreEqual(userId, userFromDb.userId);
    }

    [Test]
    public async Task GetAll()
    {
        //arrange
        string userId = "fhjlkdsfjkhfgksdlajhgfh";
        IEnumerable<User> userList = new List<User>()
        {
            new User
            {
                userId = userId,
                HasSetMasterPassword = false
            },
            new User
            {
                userId = "fdhgfdsghsdgfhfhds",
                HasSetMasterPassword = false
            },
        };
        var mock = userList.BuildMock().BuildMockDbSet();
        _mockDbContext.Setup(m => m.Users).Returns(mock.Object);

        //act
        List<User> userFromDb = await _userService.GetAll();
        // assert
        Assert.AreEqual(userList.Count(), userFromDb.Count());
    }

    [Test]
    public async Task SaveNewUser()
    {
        //arrange
        string userId = "fhjlkdsfjkhfgksdlajhgfh";
        User savedUser = null;

        _mockDbContext.Setup(m => m.Users.Add(It.IsAny<User>()))
            .Callback<User>(user => savedUser = user);

        //act
        User userFromDb = await _userService.SaveNewUser(userId);
        // assert

        _mockDbContext.Verify(m => m.Users.Add(It.IsAny<User>()), Times.Once);
        _mockDbContext.Verify(m => m.SaveChangesAsync(default), Times.Once);

        Assert.AreEqual(userId, userFromDb.userId);
        Assert.AreEqual(userId, userFromDb.userId);

    }

    [Test]
    public async Task GetById()
    {
        //arrange
        string userId = "fhjlkdsfjkhfgksdlajhgfh";
        Guid id = Guid.NewGuid();
        User savedUser = new User
        {
            id = id,
            userId = userId,
            HasSetMasterPassword = false
        };
        IEnumerable<User> userList = new List<User>()
        {
            new User
            {
                id = id,
                userId = userId,
                HasSetMasterPassword = false
            },
            new User
            {
                id = Guid.NewGuid(),
                userId = "fdhgfdsghsdgfhfhds",
                HasSetMasterPassword = false
            },
        }.AsQueryable();
        _mockDbContext.Setup(x => x.Users.FindAsync(It.IsAny<object[]>())).ReturnsAsync((object[] ids) =>
        {
            var id = (Guid)ids[0];
            return userList.FirstOrDefault(x => x.id == id);
        });

        //act
        User userFromDb = await _userService.GetById(id);
        // assert
        Assert.AreEqual(id, userFromDb.id);
    }
}
