using Microsoft.AspNetCore.Identity;

namespace AppUI.Models.CustomIdentity
{
    public class AppUser:IdentityUser<int>
    {
        // Link to publishers
        public ICollection<PublisherManager>? Publishers { get; set; }

        // Link to reviewers
        public ICollection<ReviewerCritic>? Reviewers { get; set; }
    }
}
