using System;

namespace TaskApp.DB;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }

    // Default constructor
    public User() { }

    // Parameterized constructor
    public User(int id, string email, string password)
    {
        Id = id;
        Email = email;
        Password = password;
    }
}