using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskApp.DB;
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }

    public Category() { }

    // Parameterized constructor
    public Category(int id, string name)
    {
        Id = id;
        Name = name;
    }
}
