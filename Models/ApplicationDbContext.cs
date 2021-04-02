using InvoiceManager.Models.Domains;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace InvoiceManager.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        //wszystkie tabele musimy zdefiniować tutaj, tabela Users jest już zrobiona w schemacie
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoicePosition> InvoicePositions { get; set; }
        public DbSet<MethodOfPayment> MethodOfPayments { get; set; }
        public DbSet<Product> Products{ get; set; }

        //funkcja musi być nadpisana aby wyłączyć kaskadowe usuwanie dla tab ApplicationUser
        //podczas tworzenia bazy databes-update pokazał sie komunikat
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            modelBuilder.Entity<ApplicationUser>()
                 .HasMany(x => x.Invoices)
                 .WithRequired(x => x.User)
                 .HasForeignKey(x => x.UserId)
                 .WillCascadeOnDelete(false);
            
            modelBuilder.Entity<ApplicationUser>()
                .HasMany(x => x.Clients)
                .WithRequired(x => x.User)
                .HasForeignKey(x => x.UserId)
                .WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);

        }


        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}