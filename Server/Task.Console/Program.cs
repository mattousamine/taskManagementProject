using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using TaskApp.DB;

namespace TaskApp;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Task Management System");

        IConfigurationBuilder builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("config.json", optional: false);

        IConfiguration config = builder.Build();

        Environment.SetEnvironmentVariable("MyConn", config.GetConnectionString("MyConn"));

        int operation = 0;
        while (operation != 12)
        {
            Console.WriteLine("\nChoose an operation:");
            Console.WriteLine("1. List Tasks");
            Console.WriteLine("2. Add Task");
            Console.WriteLine("3. Update Task");
            Console.WriteLine("4. Delete Task");
            Console.WriteLine("5. List Users");
            Console.WriteLine("6. Add User");
            Console.WriteLine("7. Delete User");
            Console.WriteLine("8. List Categories");
            Console.WriteLine("9. Add Category");
            Console.WriteLine("10. Delete Category");
            Console.WriteLine("11. Exit");

            Console.Write("Enter option: ");
            if (!int.TryParse(Console.ReadLine(), out operation))
            {
                Console.WriteLine("Please enter a valid number.");
                continue;
            }

            try
            {
                switch (operation)
                {
                    case 1:
                        ListTasks();
                        break;
                    case 2:
                        AddTask();
                        break;
                    case 3:
                        UpdateTask();
                        break;
                    case 4:
                        DeleteTask();
                        break;
                    case 5:
                        ListUsers();
                        break;
                    case 6:
                        AddUser();
                        break;
                    case 7:
                        DeleteUser();
                        break;
                    case 8:
                        ListCategories();
                        break;
                    case 9:
                        AddCategory();
                        break;
                    case 10:
                        DeleteCategory();
                        break;
                    case 11:
                        Console.WriteLine("Exiting...");
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }

    private static void ListTasks()
    {
        var tasks = TaskManager.ListTasks();
        foreach (var task in tasks)
        {
            Console.WriteLine($"{task.Id}: {task.Name}, {task.Description}, {task.Priority}, {task.Status}, Category: {task.CategoryId}, User: {task.UserId}");
        }
    }


    private static void AddTask()
    {
        Console.WriteLine("Adding a new task:");
        Console.Write("Name: ");
        string name = Console.ReadLine();
        Console.Write("Description: ");
        string description = Console.ReadLine();
        Console.Write("Priority (red, green, blue): ");
        string priority = Console.ReadLine();
        Console.Write("Status (ongoing, completed): ");
        string status = Console.ReadLine();
        Console.Write("Category ID: ");
        int categoryId = int.Parse(Console.ReadLine());
        Console.Write("User ID: ");
        int userId = int.Parse(Console.ReadLine());

        TaskItem task = new TaskItem { Name = name, Description = description, Priority = priority, Status = status, CategoryId = categoryId, UserId = userId };
        TaskManager.AddTask(task);
        Console.WriteLine("Task added successfully.");
    }

    private static void UpdateTask()
    {
        Console.WriteLine("Updating an existing task:");
        Console.Write("Task ID: ");
        int id = int.Parse(Console.ReadLine());
        Console.Write("Name: ");
        string name = Console.ReadLine();
        Console.Write("Description: ");
        string description = Console.ReadLine();
        Console.Write("Priority (red, green, blue): ");
        string priority = Console.ReadLine();
        Console.Write("Status (ongoing, completed): ");
        string status = Console.ReadLine();
        Console.Write("Category ID: ");
        int categoryId = int.Parse(Console.ReadLine());
        Console.Write("User ID: ");
        int userId = int.Parse(Console.ReadLine());

        TaskItem task = new TaskItem(id, name, description, priority, status, categoryId, userId);
        TaskManager.UpdateTask(task);
        Console.WriteLine("Task updated successfully.");
    }

    private static void DeleteTask()
    {
        Console.Write("Enter the ID of the task to delete: ");
        int taskId = int.Parse(Console.ReadLine());
        TaskManager.DeleteTask(taskId);
        Console.WriteLine("Task deleted successfully.");
    }

    private static void ListUsers()
    {
        var users = TaskManager.ListUsers();
        foreach (var user in users)
        {
            Console.WriteLine($"{user.Id}: {user.Email}");
        }
    }

    private static void AddUser()
    {
        Console.Write("Email: ");
        string email = Console.ReadLine();
        Console.Write("Password: ");
        string password = Console.ReadLine(); // Consider hashing the password before saving

        User user = new User(0, email, password);
        TaskManager.CreateUser(user);
        Console.WriteLine("User added successfully.");
    }

    private static void DeleteUser()
    {
        Console.Write("Enter the ID of the user to delete: ");
        int userId = int.Parse(Console.ReadLine());
        TaskManager.DeleteUser(userId);
        Console.WriteLine("User deleted successfully.");
    }

    private static void ListCategories()
    {
        var categories = TaskManager.ListCategories();
        foreach (var category in categories)
        {
            Console.WriteLine($"{category.Id}: {category.Name}");
        }
    }
    private static void AddCategory()
    {
        Console.Write("Enter the name of the category to add: ");
        string categoryName = Console.ReadLine();

        Category category = new Category(0, categoryName);
        TaskManager.CreateCategory(category);
        Console.WriteLine("Category added successfully.");
    }

    private static void DeleteCategory()
    {
        Console.Write("Enter the ID of the category to delete: ");
        int categoryId = int.Parse(Console.ReadLine());

        TaskManager.DeleteCategory(categoryId);
        Console.WriteLine("Category deleted successfully.");
    }

}
