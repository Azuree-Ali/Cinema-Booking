using Microsoft.EntityFrameworkCore;

namespace Cinema.Models
{
    [Index(nameof(OTP), IsUnique = true)]
    public class ApplicationUserOTP
    {
        public string Id { get; set; }
        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string OTP { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsValid { get; set; }
        public DateTime ValidTo { get; set; }

        public ApplicationUserOTP(string userId, string otp)
        {
            Id = Guid.NewGuid().ToString();
            ApplicationUserId = userId;
            OTP = otp;
            IsValid = true;
            CreatedAt = DateTime.UtcNow;
            ValidTo = DateTime.UtcNow.AddMinutes(10);
        }
        public ApplicationUserOTP()
        {
        }
    }
}
