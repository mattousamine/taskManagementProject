using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskApp.DB;

public class TaskItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Priority { get; set; }
    public string Status { get; set; }
    public int CategoryId { get; set; }
    public int UserId { get; set; }

    // Default constructor
    public TaskItem() { }

    // Parameterized constructor
    public TaskItem(int id, string name, string description, string priority, string status, int categoryId, int userId)
    {
        Id = id;
        Name = name;
        Description = description;
        Priority = priority;
        Status = status;
        CategoryId = categoryId;
        UserId = userId;
    }
}

