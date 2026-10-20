using System.ComponentModel.DataAnnotations;

namespace Models.Models
{
    public class TestData
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}