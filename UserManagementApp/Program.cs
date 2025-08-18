using Microsoft.EntityFrameworkCore;
using UserManagementApp.Auth;
using UserManagementApp.Data;
using UserManagementApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services
    .AddAuthentication("ApiKeyScheme")
    .AddScheme<ApiKeySchemeOptions, ApiKeyAuthHandler>("ApiKeyScheme",
        options =>
        {
            options.AllowedApiKeys = builder.Configuration.GetSection("AllowedApiKeys").Get<string[]>() ?? [];
        });


builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("ApiKeyAuthz", (pb) =>
    {
        pb.RequireAuthenticatedUser().AddAuthenticationSchemes("ApiKeyScheme");
    });
});

builder.Services.AddDbContext<AppDbContext>(opts => { opts.UseSqlite("DataSource=mydb.db"); });

builder.Services.AddCors(opts =>
{
    opts.AddPolicy("AllowAnyone", (pb) => { pb.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
});

var app = builder.Build();

app.UseCors("AllowAnyone");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Ok());

app.MapGet("/groups",
        (AppDbContext ctx) => { return Results.Ok(ctx.Groups.Where(g => g.GroupType == GroupType.Standard).ToList()); })
    .RequireAuthorization("ApiKeyAuthz");

app.MapGet("/user/{email}",
        (AppDbContext ctx, string email) => { return Results.Ok(ctx.Users.FirstOrDefault(u => u.UserEmail == email)); })
    .RequireAuthorization("ApiKeyAuthz");

app.MapGet("/userGroupAssignments",
        (AppDbContext ctx) => { return Results.Ok(ctx.UserGroupAssignments.ToList()); })
    .RequireAuthorization("ApiKeyAuthz");

app.MapPost("/userToGroup", async (UserToGroup userToGroup, AppDbContext ctx) =>
{
    await ctx.UserGroupAssignments.AddAsync(new()
    {
        UserId = userToGroup.UserId,
        GroupId = userToGroup.GroupId,
    });

    await ctx.SaveChangesAsync();

    return Results.Ok(1);
}).RequireAuthorization("ApiKeyAuthz");

app.MapPost("/group", async (Group group, AppDbContext ctx) =>
{
    await ctx.Groups.AddAsync(new()
    {
        GroupName = group.GroupName,
        GroupType = GroupType.Standard
    });

    await ctx.SaveChangesAsync();

    return Results.Ok();
}).RequireAuthorization("ApiKeyAuthz");

app.MapPost("/user", async (User user, AppDbContext ctx) =>
{
    await ctx.Users.AddAsync(new()
    {
        UserEmail = user.UserEmail,
        UserName = user.UserName,
    });

    await ctx.SaveChangesAsync();

    return Results.Ok();
}).RequireAuthorization("ApiKeyAuthz");

app.Run();