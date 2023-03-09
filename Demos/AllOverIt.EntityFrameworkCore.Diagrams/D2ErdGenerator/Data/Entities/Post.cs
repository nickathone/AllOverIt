using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace D2ErdGenerator.Data.Entities
{
    public class Post
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public Blog Blog { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}