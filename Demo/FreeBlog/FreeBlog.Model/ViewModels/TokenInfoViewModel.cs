namespace FreeBlog.Model.ViewModels
{
    public class TokenInfoViewModel
    {
        public bool success { get; set; }
        public string token { get; set; }
        public string expireTime { get; set; }
        public string token_type { get; set; }
    }
}
