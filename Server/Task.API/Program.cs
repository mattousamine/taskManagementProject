using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Microsoft.OpenApi.Models;
using TaskApp.DB;
using System;
using System.Linq;
using Newtonsoft.Json;

namespace TaskApp.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddCors();
        // Add services to the container.
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Task API", Version = "v1" });
        });

        var app = builder.Build();
        app.UseCors(builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod()
                   .AllowAnyHeader();
        });

        Environment.SetEnvironmentVariable("MyConn", app.Configuration["MyConn"]);
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();


        app.MapTaskEndpoints();
        app.MapUserEndpoints();
        app.MapCategoryEndpoints();

        app.Run();

    }
}
public static class AppExtensions
{
    public static void MapTaskEndpoints(this WebApplication app)
    {
        app.MapGet("/tasks", async (HttpRequest req, HttpResponse res) =>
        {
            // Extract query parameters
            string userIdStr = req.Query["userId"];
            string pageStr = req.Query["page"];
            string pageSizeStr = req.Query["pageSize"];

            if (!int.TryParse(userIdStr, out int userId) ||
                !int.TryParse(pageStr, out int page) ||
                !int.TryParse(pageSizeStr, out int pageSize))
            {
                res.StatusCode = 400; // Bad Request
                await res.WriteAsJsonAsync(new { message = "Invalid parameters" });
                return;
            }

           
            page = Math.Max(page - 1, 0);
            pageSize = Math.Clamp(pageSize, 1, 100); 

            try
            {
                var tasks = TaskManager.ListTasks(userId, page * pageSize, pageSize);
                await res.WriteAsJsonAsync(tasks);
            }
            catch (Exception ex)
            {
                res.StatusCode = 500; // Internal Server Error
                await res.WriteAsJsonAsync(new { message = $"Error retrieving tasks: {ex.Message}" });
            }
        });
        app.MapGet("/tasks/count", async (HttpRequest req, HttpResponse res) =>
        {
            string userIdStr = req.Query["userId"];
            if (!int.TryParse(userIdStr, out int userId))
            {
                res.StatusCode = 400; // Bad Request
                await res.WriteAsJsonAsync(new { message = "Invalid user ID parameter" });
                return;
            }

            try
            {
                int count = TaskManager.CountTasksByUserId(userId);
                await res.WriteAsJsonAsync(new { count = count });
            }
            catch (Exception ex)
            {
                res.StatusCode = 500; // Internal Server Error
                await res.WriteAsJsonAsync(new { message = $"Error retrieving tasks count: {ex.Message}" });
            }
        });


        app.MapPost("/tasks", async (HttpRequest req, HttpResponse res) =>
        {
            using (var reader = new StreamReader(req.Body))
            {
                var body = await reader.ReadToEndAsync();
                var newTask = JsonConvert.DeserializeObject<TaskItem>(body);

                try
                {
                    // Wrapping synchronous code in Task.Run to execute it asynchronously
                    await System.Threading.Tasks.Task.Run(() => TaskManager.AddTask(newTask));
                    res.StatusCode = 201; // Created
                    await res.WriteAsJsonAsync(new { message = "Task created successfully." });
                }
                catch (Exception ex)
                {
                    res.StatusCode = 400; // Bad Request
                    await res.WriteAsJsonAsync(new { message = $"Error creating task: {ex.Message}" });
                }
            }
        });


        app.MapPut("/tasks/{id}", (int id, TaskItem task) => TaskManager.UpdateTask(task));
        app.MapDelete("/tasks/{id}", (int id) => TaskManager.DeleteTask(id));
    }

    public static void MapUserEndpoints(this WebApplication app)
    {
        app.MapGet("/users", () => TaskManager.ListUsers());
        app.MapPost("/users", async (HttpRequest req, HttpResponse res) =>
        {
            using (var reader = new StreamReader(req.Body))
            {
                var body = await reader.ReadToEndAsync();
                var newUser = JsonConvert.DeserializeObject<User>(body);

                try
                {
                    // Assuming CreateUser returns a Task; adjust as necessary
                    await TaskManager.CreateUser(newUser);
                    res.StatusCode = 201; // Created
                    await res.WriteAsJsonAsync(new { message = "User created successfully." });
                }
                catch (Exception ex)
                {
                    res.StatusCode = 400; // Bad Request
                    await res.WriteAsJsonAsync(new { message = $"Error creating user: {ex.Message}" });
                }
            }
        });
        app.MapDelete("/users/{id}", (int id) => TaskManager.DeleteUser(id));
        app.MapPost("/users/signin", async (HttpRequest req, HttpResponse res) =>
        {
            using (var reader = new StreamReader(req.Body))
            {
                var body = await reader.ReadToEndAsync();
                var userCredentials = JsonConvert.DeserializeObject<User>(body);
                var user = TaskManager.SignInUser(userCredentials.Email, userCredentials.Password);
                if (user != null)
                {
                    await res.WriteAsJsonAsync(new
                    {
                        message = "Sign-in successful.",
                        id = user.Id,
                        email = user.Email 
                    });
                    
                }
                else
                {
                    res.StatusCode = 401; // Unauthorized
                    await res.WriteAsJsonAsync(new { message = "Invalid email or password." });
                }
            }
        });
    }


    public static void MapCategoryEndpoints(this WebApplication app)
    {
        app.MapGet("/categories", () => TaskManager.ListCategories());
        app.MapPost("/categories", async (HttpRequest req, HttpResponse res) =>
        {
            using (var reader = new StreamReader(req.Body))
            {
                var body = await reader.ReadToEndAsync();
                var category = JsonConvert.DeserializeObject<Category>(body);
                if (category != null && !string.IsNullOrEmpty(category.Name))
                {
                    TaskManager.CreateCategory(category);
                    res.StatusCode = 201; // Created
                    await res.WriteAsJsonAsync(new { message = "Category created successfully.", name = category.Name });
                }
                else
                {
                    res.StatusCode = 400; // Bad Request
                    await res.WriteAsJsonAsync(new { message = "Invalid category data." });
                }
            }
        });


        app.MapDelete("/categories/{id}", (int id) => TaskManager.DeleteCategory(id));
    }
}

