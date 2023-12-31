using disasterrelief_be.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
//ADD CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000",
                                "http://localhost:7287")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<DisasterReliefDbContext>(builder.Configuration["DisasterReliefDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();

//Add for Cors
app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// #### END POINTS ######

//CHECK USER EXISTS
app.MapGet("/api/checkuser/{uid}", (DisasterReliefDbContext db, string uid) =>
{
    var userExists = db.Users.Where(x => x.Uid == uid).FirstOrDefault();
    if (userExists == null)
    {
        return Results.StatusCode(204);
    }
    return Results.Ok(userExists);
});

//GET All Users
app.MapGet("/api/users", (DisasterReliefDbContext db) =>
{
    return db.Users.ToList();

});

//Create a User
app.MapPost("/api/user", (DisasterReliefDbContext db, User user) =>
{
    db.Users.Add(user);
    db.SaveChanges();
    return Results.Created($"/api/user/{user.Id}", user);
});

// Delete an user
app.MapDelete("/api/users/{userId}", (DisasterReliefDbContext db, int id) =>
{
    User user = db.Users.SingleOrDefault(u => u.Id == id);
    if (user == null)
    {
        return Results.NotFound();
    }
    db.Users.Remove(user);
    db.SaveChanges();
    return Results.NoContent();
});

// get all disasters 
app.MapGet("/api/disaster", (DisasterReliefDbContext db) =>
{
    return db.Disasters.ToList();
});

// get disaster by Id
app.MapGet("/api/disaster/{id}", (DisasterReliefDbContext db, int id) =>
{
    var disaster = db.Disasters.Where(s => s.Id == id).Include(x => x.Items).FirstOrDefault();
    return disaster;
}
);

// create a disaster
app.MapPost("api/disaster", async (DisasterReliefDbContext db, Disaster disaster) =>
{
    db.Disasters.Add(disaster);
    db.SaveChanges();
    return Results.Created($"/api/disaster{disaster.Id}", disaster);
});

// Update Disaster 
app.MapPut("api/disaster/{id}", async (DisasterReliefDbContext db, int id, Disaster disaster) =>
{
    Disaster disasterToUpdate = await db.Disasters.SingleOrDefaultAsync(disaster => disaster.Id == id);
    if (disasterToUpdate == null)
    {
        return Results.NotFound();
    }
    disasterToUpdate.Id = disaster.Id;
    disasterToUpdate.DisasterName = disaster.DisasterName;
    disasterToUpdate.Description = disaster.Description;
    disasterToUpdate.Location = disaster.Location;
    disasterToUpdate.Severity = disaster.Severity;
    disasterToUpdate.Image = disaster.Image;
    db.SaveChanges();
    return Results.NoContent();
});

//Delete Disaster
app.MapDelete("api/disaster/{id}", (DisasterReliefDbContext db, int id) =>
{
    Disaster disaster = db.Disasters.SingleOrDefault(disaster => disaster.Id == id);
    if (disaster == null)
    {
        return Results.NotFound();
    }
    db.Disasters.Remove(disaster);
    db.SaveChanges();
    return Results.NoContent();
});


// GET All Categories
app.MapGet("/api/category", (DisasterReliefDbContext db) =>
{
    return db.Categories.ToList();
});

// Post Category
app.MapPost("/api/category", (DisasterReliefDbContext db, Category category) =>
{
    db.Categories.Add(category);
    db.SaveChanges();
    return Results.Created($"/api/category)", category);
});


// Delete Category
app.MapDelete("/api/category/{id}", (DisasterReliefDbContext db, int id) =>
{
    Category categoryToDelete = db.Categories.SingleOrDefault(c => c.Id == id);
    db.Categories.Remove(categoryToDelete);
    db.SaveChanges();
});

// GET All Items
app.MapGet("/api/item", (DisasterReliefDbContext db) =>
{
    return db.Items.ToList();
});

// Create A Item
app.MapPost("/api/item", (DisasterReliefDbContext db, Item item) =>
{
    db.Items.Add(item);
    db.SaveChanges();
    return Results.Created($"/api/item)", item);
});

// Delete Item
app.MapDelete("/api/item/{id}", (DisasterReliefDbContext db, int id) =>
{
    Item itemToDelete = db.Items.SingleOrDefault(c => c.Id == id);
    db.Items.Remove(itemToDelete);
    db.SaveChanges();
});

//Get count by category
app.MapGet("/api/item/category/{id}", (DisasterReliefDbContext db, int id) =>
{
    return db.Items.Where(i => i.CategoryId == id).Count();
});


// Add item to existing disaster
app.MapPost("/api/item/{disasterId}", (DisasterReliefDbContext db, Item newItem, int disasterId) =>
{

    var existingDisaster = db.Disasters
        .Include(o => o.Items)
        .FirstOrDefault(o => o.Id == disasterId);

    if (existingDisaster == null)
    {

        return Results.NotFound();
    }

    // Set the order ID for the new item
    newItem.Disasters = new List<Disaster> { existingDisaster };

    db.Items.Add(newItem);
    db.SaveChanges();

    return Results.Created($"/api/item/{disasterId}", newItem);



});


app.Run();

