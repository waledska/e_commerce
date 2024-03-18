namespace e_commerce.vModels
{
    public class role
    {
        public string? roleId { get; set; }
        public string? roleName { get; set; }
    }
    public class userRoles
    {
        public string? userId { get; set; }
        public List<role> userInRoles { get; set; }
        public string? message { get; set; }
        public userRoles()
        {
                userInRoles = new List<role>();
        }
    }
}

