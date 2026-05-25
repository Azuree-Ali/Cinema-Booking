namespace Cinema.Services
{
    public class CreateCinemaVM
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Img { get; set; }
        public string Address { get; set; } = string.Empty;
        public bool Status { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
