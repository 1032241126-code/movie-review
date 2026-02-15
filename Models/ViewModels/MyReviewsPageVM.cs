namespace MovieReviewSystem.Models.ViewModels
{
    public class RatingGraphVM
    {
        public int Rating { get; set; }
        public int Count { get; set; }
    }

    public class MyReviewsPageVM
    {
        public List<MyReviewVM> Reviews { get; set; }
        public List<RatingGraphVM> RatingGraph { get; set; }

        public string FavGenre { get; set; }
        public double Avg { get; set; }
        public int Count { get; set; }
    }
}
