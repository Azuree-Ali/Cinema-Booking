namespace Cinema.Services
{
    public class CreateActorVM
    {
        public string Name { get; set; } = string.Empty;
        public string? Img { get; set; }
        public string Gender { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
