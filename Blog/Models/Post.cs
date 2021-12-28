namespace Blog.Models
{
    public class Post
    {
        public Post(string text, int userId)
        {
            Text = text;
            UserId = userId;
        }

        public int Id { get; set; }
        public string Text { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
