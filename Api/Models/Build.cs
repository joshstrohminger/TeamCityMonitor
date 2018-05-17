namespace Api.Models
{
    public class Build
    {
        public string Number { get; set; }
        public string Status { get; set; }
        public string State { get; set; }
        public string WebUrl { get; set; }
        public string StatusText { get; set; }
        public string FinishDate { get; set; }
    }
}
