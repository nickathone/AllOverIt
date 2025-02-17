﻿using EFEnumerationDemo.Models;
using System.ComponentModel.DataAnnotations;

namespace EFEnumerationDemo.Entities
{
    // Includes several EnrichedEnum types that use a value converter to read/write the name value
    public class Post
    {
        public int Id { get; set; }
     
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [MaxLength(20)]
        public PostRating Rating { get; set; }

        // testing without any attributes
        public PostRating RatingValue { get; set; }

        [Required]
        [MaxLength(20)]
        public PublishedStatus Status { get; set; }

        // testing without any attributes
        public PublishedStatus StatusValue { get; set; }

        [Required]
        public Blog Blog { get; set; }
    }
}