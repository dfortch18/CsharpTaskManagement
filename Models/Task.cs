namespace CsharpTaskManagement.Models
{
    public class Task(string name, string description, bool done)
    {
        public int Id { get; set; }

        public string Name { get; set; } = name;

        public string Description { get; set; } = description;

        public bool Done { get; set; } = done;

        public Task(string name, string description) : this(name, description, false)
        {
        }

        public Task() : this(string.Empty, string.Empty)
        {
        }
    }
}