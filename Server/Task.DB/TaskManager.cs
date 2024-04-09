using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace TaskApp.DB;

public class TaskManager
{
    private static MySqlConnection GetConnection()
    {
        // Fetch the connection string securely
        return new MySqlConnection(Environment.GetEnvironmentVariable("MyConn"));

    }

    // User Operations
    public static async Task CreateUser(User user)
    {
        using (var conn = GetConnection())
        {
            await conn.OpenAsync();
            var cmd = new MySqlCommand("INSERT INTO users (Email, Password) VALUES (@Email, @Password)", conn);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@Password", user.Password);
            await cmd.ExecuteNonQueryAsync();
        }
    }


    public static List<User> ListUsers()
    {
        List<User> result = new List<User>();
        using (var conn = GetConnection())
        {
            conn.Open();
            var cmd = new MySqlCommand("SELECT Id, Email, Password FROM users", conn);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new User(reader.GetInt32("Id"), reader.GetString("Email"), reader.GetString("Password")));
                }
            }
        }
        return result;
    }

    public static void DeleteUser(int userId)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            var cmd = new MySqlCommand("DELETE FROM users WHERE Id = @Id AND NOT EXISTS (SELECT * FROM tasks WHERE user_id = @Id)", conn);
            cmd.Parameters.AddWithValue("@Id", userId);
            cmd.ExecuteNonQuery();
        }
    }

    public static User SignInUser(string email, string password)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            var cmd = new MySqlCommand("SELECT Id, Email, Password FROM users WHERE Email = @Email AND Password = @Password LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);

            using (var reader = cmd.ExecuteReader())
            {
                if (reader.Read())
                {
                    return new User(reader.GetInt32("Id"), reader.GetString("Email"), reader.GetString("Password"));
                }
            }
        }
        return null; 
    }


    // Category Operations
    public static void CreateCategory(Category category)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            var cmd = new MySqlCommand("INSERT INTO categories (Name) VALUES (@Name)", conn);
            cmd.Parameters.AddWithValue("@Name", category.Name);
            cmd.ExecuteNonQuery();
        }
    }

    public static List<Category> ListCategories()
    {
        List<Category> result = new List<Category>();
        using (var conn = GetConnection())
        {
            conn.Open();
            var cmd = new MySqlCommand("SELECT Id, Name FROM categories", conn);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new Category(reader.GetInt32("Id"), reader.GetString("Name")));
                }
            }
        }
        return result;
    }

    public static void DeleteCategory(int categoryId)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            var cmd = new MySqlCommand("DELETE FROM categories WHERE Id = @Id AND NOT EXISTS (SELECT * FROM tasks WHERE category_id = @Id)", conn);
            cmd.Parameters.AddWithValue("@Id", categoryId);
            cmd.ExecuteNonQuery();
        }
    }

    // Task Operations
    public static int CountTasksByUserId(int userId)
    {
        int count = 0;
        using (var conn = GetConnection())
        {
            conn.Open();
            var cmd = new MySqlCommand("SELECT COUNT(*) FROM tasks WHERE user_id = @userId", conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            count = Convert.ToInt32(cmd.ExecuteScalar());
        }
        return count;
    }

    public static List<TaskItem> ListTasks(int userId, int offset, int pageSize)
    {
        List<TaskItem> result = new List<TaskItem>();

        using (var conn = GetConnection())
        {
            conn.Open();
            var cmd = new MySqlCommand("SELECT id, name, description, priority, status, category_id, user_id FROM taskdb.tasks WHERE user_id = @userId ORDER BY id LIMIT @offset, @pageSize", conn);
            cmd.Parameters.AddWithValue("@userId", userId);
            cmd.Parameters.AddWithValue("@offset", offset);
            cmd.Parameters.AddWithValue("@pageSize", pageSize);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    result.Add(new TaskItem(
                        reader.GetInt32("id"),
                        reader.GetString("name"),
                        reader.GetString("description"),
                        reader.GetString("priority"),
                        reader.GetString("status"),
                        reader.GetInt32("category_id"),
                        reader.GetInt32("user_id")
                    ));
                }
            }
        }

        return result;
    }

    public static void AddTask(TaskItem task)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            var cmd = new MySqlCommand("INSERT INTO tasks (name, description, priority, status, category_id, user_id) VALUES (@Name, @Description, @Priority, @Status, @CategoryId, @UserId)", conn);
            cmd.Parameters.AddWithValue("@Name", task.Name);
            cmd.Parameters.AddWithValue("@Description", task.Description);
            cmd.Parameters.AddWithValue("@Priority", task.Priority);
            cmd.Parameters.AddWithValue("@Status", task.Status);
            cmd.Parameters.AddWithValue("@CategoryId", task.CategoryId);
            cmd.Parameters.AddWithValue("@UserId", task.UserId);
            cmd.ExecuteNonQuery();
        }
    }

    public static void UpdateTask(TaskItem task)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            var cmd = new MySqlCommand("UPDATE tasks SET name = @Name, description = @Description, priority = @Priority, status = @Status, category_id = @CategoryId, user_id = @UserId WHERE id = @Id", conn);
            cmd.Parameters.AddWithValue("@Name", task.Name);
            cmd.Parameters.AddWithValue("@Description", task.Description);
            cmd.Parameters.AddWithValue("@Priority", task.Priority);
            cmd.Parameters.AddWithValue("@Status", task.Status);
            cmd.Parameters.AddWithValue("@CategoryId", task.CategoryId);
            cmd.Parameters.AddWithValue("@UserId", task.UserId);
            cmd.Parameters.AddWithValue("@Id", task.Id);
            cmd.ExecuteNonQuery();
        }
    }

    public static void DeleteTask(int taskId)
    {
        using (var conn = GetConnection())
        {
            conn.Open();
            var cmd = new MySqlCommand("DELETE FROM tasks WHERE id = @Id", conn);
            cmd.Parameters.AddWithValue("@Id", taskId);
            cmd.ExecuteNonQuery();
        }
    }
}
