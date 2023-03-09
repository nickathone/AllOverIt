using System.ComponentModel.DataAnnotations;

namespace D2ErdGenerator.Data.Entities
{
    public class AuthorBlog
    {
        public int Id { get; set; }

        [Required]
        public Author Author { get; set; }

        [Required]
        public Blog Blog { get; set; }
    }
}