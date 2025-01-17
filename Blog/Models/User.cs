﻿using System.Collections.Generic;

namespace Blog.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Admin { get; set; }
        public ICollection<Post> Posts { get; set; }
    }
}
