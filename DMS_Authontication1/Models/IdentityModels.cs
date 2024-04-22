using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using DMS_Authontication1.Migrations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DMS_Authontication1.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string Type { get; set; }

        public string PhoneNumder1 { get; set; }

        public string Provider { get; set; }

        public string Address { get; set; }

        public string TypeId { get; set; }
        public ICollection<ProposalMain> Proposals { get; set; }
        public ICollection<ProposalNotification> ProposalNotifications { get; set; } = new List<ProposalNotification>();

        // public string CardID { get; set; }
        //   public virtual DoctorPersonalData doctorPersonalData { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, ClaimsIdentity identity)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            if (identity != null)
            {
                //userIdentity.AddClaims(identity.Claims);
                foreach (var Claim in identity.Claims)
                {
                    if (!userIdentity.HasClaim(Claim.Type, Claim.Value))
                        userIdentity.AddClaim(new Claim(Claim.Type, Claim.Value));
                }
            }
            return userIdentity;
        }

        //public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser, string> manager, ClaimsIdentity CurrentIdentity)
        //{
        //    // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
        //    var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

        //    // Re validate existing Claims here
        //    userIdentity.AddClaims(CurrentIdentity.Claims);


        //    return userIdentity;
        //}
    }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }


    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<DoctorPersonalData> DoctorPersonalData { get; set; }
        public DbSet<DoctorWorkPlace> DoctorWorkPlace { get; set; }
        public DbSet<DailyPermission> DailyPermission { get; set; }
        public DbSet<MonthlyPermission> MonthlyPermission { get; set; }
        public DbSet<Doctors_Permissions> Doctors_Permissions { get; set; }
        public DbSet<Basic_Data> BASIC_DATA { get; set; }
        public DbSet<Company> Comapnies { get; set; }
        public DbSet<Broker> Brokers { get; set; }
        public DbSet<ProposalMain> ProposalMains { get; set; }
        public DbSet<ProposalStepTwo> ProposalStepTwos { get; set; }
        public DbSet<ProposalInsideMedicalAuthority> ProposalInsideMedicalAuthorities { get; set; }
        public DbSet<ProposalOutsideMedicalAuthority> ProposalOutsideMedicalAuthorities { get; set; }
        public DbSet<CompanyActivity> CompanyActivities { get; set; }
        public DbSet<Area> Areas { get; set; }
        public DbSet<CardColor> CardColors { get; set; }
        public DbSet<MedicalNetwork> MedicalNetworks { get; set; }
        public DbSet<ResidenceDegree> ResidenceDegrees { get; set; }
        public DbSet<Pricing> Pricings { get; set; }
        public DbSet<ProposalNotification> ProposalNotifications { get; set; }
        public DbSet<ProposalNotificationApplicationUser> ProposalNotificationApplicationUsers { get; set; }
        public DbSet<RenewalMain> RenewalMains { get; set; }
        public DbSet<PricesOutsideMedicalAuthority> PricesOutsideMedicalAuthorities { get; set; }
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //public System.Data.Entity.DbSet<DMS_Authontication1.Models.RoleViewModel> RoleViewModels { get; set; }
    }
}