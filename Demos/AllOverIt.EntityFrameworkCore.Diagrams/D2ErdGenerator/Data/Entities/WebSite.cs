using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace D2ErdGenerator.Data.Entities
{
    public class WebSite
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        public Settings Settings { get; set; }

        public ICollection<Blog> Blogs { get; set; }
    }
}