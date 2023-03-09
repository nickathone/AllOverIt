using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace D2ErdGenerator.Data.Entities
{
    public class Blog
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public WebSite WebSite { get; set; }

        public ICollection<Post> Posts { get; set; }
        public ICollection<AuthorBlog> AuthorBlogs { get; set; }
    }
}