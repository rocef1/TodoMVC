using System.ComponentModel.DataAnnotations;

namespace TodoMVC.Config
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public string CollectionName { get; set; } = string.Empty;
    }
}

namespace TodoMVC.Models
{
    public class Zavdania
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string Description { get; set; } = string.Empty;
        
        [Required]
        public string Priority { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }
    }
}
