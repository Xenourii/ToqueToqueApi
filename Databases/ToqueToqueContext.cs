using Microsoft.EntityFrameworkCore;
using ToqueToqueApi.Databases.Models;

namespace ToqueToqueApi.Databases
{
    //Install-Package Microsoft.EntityFrameworkCore.Tools
    //Add-Migration MyFirstMigration
    //Update-Database
    public class ToqueToqueContext : DbContext
    {
        public DbSet<AllergenDb> Allergens { get; set; }
        public DbSet<BookingStateDb> BookingStates { get; set; }
        public DbSet<ConversationDb> Conversations { get; set; }
        public DbSet<DifficultyDb> Difficulties { get; set; }
        public DbSet<MealDb> Meals { get; set; }
        public DbSet<MessageDb> Messages { get; set; }
        public DbSet<ParticularityDb> Particularities { get; set; }
        public DbSet<SessionDb> Sessions { get; set; }
		public DbSet<UserDb> Users { get; set; }
        public DbSet<TagDb> Tags { get; set; }
        public DbSet<GeolocationDb> Geolocations { get; set; }

        public ToqueToqueContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSnakeCaseNamingConvention();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
            modelBuilder.Entity<AllergenDb>().ToTable("Allergen");
            modelBuilder.Entity<BookingStateDb>().ToTable("BookingState");
            modelBuilder.Entity<ConversationDb>().ToTable("Conversation");
            modelBuilder.Entity<DifficultyDb>().ToTable("Difficulty");
            modelBuilder.Entity<MealDb>().ToTable("Meal");
            modelBuilder.Entity<MessageDb>().ToTable("Message");
            modelBuilder.Entity<ParticularityDb>().ToTable("Particularity");
            modelBuilder.Entity<SessionDb>().ToTable("Session");
            modelBuilder.Entity<TagDb>().ToTable("Tag");
            modelBuilder.Entity<UserDb>().ToTable("User");
            modelBuilder.Entity<GeolocationDb>().ToTable("Geolocation");

            // Relation One to One entre Session et Geolocation
            modelBuilder.Entity<SessionDb>()
                .HasOne(s => s.Geolocation)
                .WithOne(g => g.Session)
                .HasForeignKey<GeolocationDb>(g => g.SessionId);

            // Table de liaison entre les allergènes et les plats
            modelBuilder.Entity<AllergenMealDb>()
                .HasKey(am => new { am.AllergenId, am.MealId });
            modelBuilder.Entity<AllergenMealDb>()
                .HasOne(am => am.Allergen)
                .WithMany(a => a.AllergenMeal)
                .HasForeignKey(am => am.AllergenId);
            modelBuilder.Entity<AllergenMealDb>()
                .HasOne(am => am.Meal)
                .WithMany(m => m.AllergenMeals)
                .HasForeignKey(am => am.MealId);


            // Table de liaison entre les allergènes et les utilisateurs
            modelBuilder.Entity<AllergenUserDb>()
                .HasKey(au => new { au.AllergenId, au.UserId });
            modelBuilder.Entity<AllergenUserDb>()
                .HasOne(au => au.Allergen)
                .WithMany(a => a.AllergenUser)
                .HasForeignKey(au => au.AllergenId);
            modelBuilder.Entity<AllergenUserDb>()
                .HasOne(au => au.User)
                .WithMany(u => u.AllergenUser)
                .HasForeignKey(au => au.UserId);


            // Table de liaison entre les sessions et les plats
            modelBuilder.Entity<SessionMealDb>()
                .HasKey(sm => new { sm.SessionId, sm.MealId });
            modelBuilder.Entity<SessionMealDb>()
                .HasOne(sm => sm.Session)
                .WithMany(s => s.SessionMeals)
                .HasForeignKey(sm => sm.SessionId);
            modelBuilder.Entity<SessionMealDb>()
                .HasOne(sm => sm.Meal)
                .WithMany(m => m.SessionMeals)
                .HasForeignKey(sm => sm.MealId);


            // Table de liaison entre les session, les état de réservation et les utilisateurs
            modelBuilder.Entity<BookingStateSessionUserDb>()
                .HasKey(bsu => new { bsu.SessionId, bsu.UserId });
            modelBuilder.Entity<BookingStateSessionUserDb>()
                .HasOne(bsu => bsu.BookingState)
                .WithMany(b => b.BookingStateSessionUser)
                .HasForeignKey(bsu => bsu.BookingStateId);
            modelBuilder.Entity<BookingStateSessionUserDb>()
                .HasOne(bsu => bsu.Session)
                .WithMany(s => s.BookingStateSessionUser)
                .HasForeignKey(bsu => bsu.SessionId);
            modelBuilder.Entity<BookingStateSessionUserDb>()
                .HasOne(bsu => bsu.User)
                .WithMany(u => u.BookingStateSessionUser)
                .HasForeignKey(bsu => bsu.UserId);


            // Table de liaison entre les conversations et les utilisateurs
            modelBuilder.Entity<ConversationUserDb>()
                .HasKey(cmu => new { cmu.ConversationId, cmu.UserId });
            modelBuilder.Entity<ConversationUserDb>()
                .HasOne(cmu => cmu.Conversation)
                .WithMany(c => c.ConversationUser)
                .HasForeignKey(cmu => cmu.ConversationId);
            modelBuilder.Entity<ConversationUserDb>()
                .HasOne(cmu => cmu.User)
                .WithMany(u => u.ConversationUser)
                .HasForeignKey(cmu => cmu.UserId);
        }
    }
}