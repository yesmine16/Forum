namespace Forum.Models
{
    public class ResetPasswordModel
    {
        public string NewPassword { get; set; }

        public string ConfirmNewPassword { get; set; }

        public string Email { get; set; }
    }
}
