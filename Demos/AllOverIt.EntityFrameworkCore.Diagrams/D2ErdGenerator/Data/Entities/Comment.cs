using System.ComponentModel.DataAnnotations;

namespace D2ErdGenerator.Data.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public Post Post { get; set; }

        [Required]
        public Author Author { get; set; }
    }
}