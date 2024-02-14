using AppUI.Models.CustomIdentity;
using AppUI.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Models.ViewModels;
using AppUI.Models.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppUI.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser,AppRole,int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Information> Informations { get; set; }
        public DbSet<Privacy> Privacies { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<Developer> Developers { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }
        public DbSet<VideoGame> VideoGames { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<DeveloperVideoGame> DevelopersVideoGame { get; set; }
        public DbSet<VideoGameOffer> Offers { get; set; }
        public DbSet<PublisherManager> PublisherManagers { get; set; }
        public DbSet<ReviewerCritic> ReviewerCritics { get; set; }
        public DbSet<ShopCart> ShopCarts { get; set; }
        public DbSet<WorkHour> WorkHours { get; set; }
        public DbSet<SpecialDay> SpecialDays { get; set; }
        public DbSet<EmployeeSchedule> EmployeeSchedules { get; set; }
        public DbSet<PayingMethod> PayingMethods { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderContent> OrderContents { get; set; }
        public DbSet<FeedBack> FeedBacks { get; set; }
        public DbSet<UserData> UserDatas { get; set; }
        public DbSet<AppUI.Models.ViewModels.CardDeveloperVm> CardDeveloperVm { get; set; } = default!;
        public DbSet<AppUI.Models.ViewModels.CardReviewerVm> CardReviewerVm { get; set; } = default!;

        //protected override void OnModelCreating(ModelBuilder modelBuilding)
        //{
        //    modelBuilding.Entity<PublisherManager>()
        //        .HasKey(pk => new { pk.PublisherId, pk.ManagerId });
        //}
    }
}
