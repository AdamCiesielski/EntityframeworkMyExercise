using EntityFrameworkMyExercises.Entities;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MyBoardsContext>(
        option => option.UseSqlServer(builder.Configuration.GetConnectionString("MyBoardConnectionString"))
    );

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetService<MyBoardsContext>();

var pendingMigrations = dbContext.Database.GetPendingMigrations();
if (pendingMigrations.Any())
{
    dbContext.Database.Migrate();
}

var users = dbContext.Users.ToList();
if (!users.Any())
{
    var user1 = new User()
    {
        Email = "user1@test.com",
        FirstName = "Kamil",
        LastName = "W³odarczyk",
        Address = new Address()
        {
            City = "Zbonszynek",
            Street = "Wiejska"
        }
    };
    var user2 = new User()
    {
        Email = "user2@test.com",
        FirstName = "Adrian",
        LastName = "Konieczny",
        Address = new Address()
        {
            City = "Gniezno",
            Street = "Szeroka"
        }
    };
    dbContext.Users.AddRange(user1, user2);
    dbContext.SaveChanges();
}

app.MapGet("data", async (MyBoardsContext db) =>
{
    var newComments = await db.
    Comments
    .OrderByDescending(c => c.CreatedDate)
    .Take(5)
    .ToListAsync();

    return newComments;
});

app.Run();