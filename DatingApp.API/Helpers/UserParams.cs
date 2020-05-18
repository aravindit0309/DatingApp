namespace DatingApp.API.Helpers
{
    public class UserParams
    {
        private const int MaxSize = 50;

        public int PageNumber { get; set; } = 1;
        private int pageSize = 5;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxSize ? MaxSize : value); }
        }

        public int userId { get; set; }
        public string Gender { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 99;
        public string OderBy { get; set; }
        public bool Likees { get; set; }
        public bool Likers { get; set; }
        
    }
}