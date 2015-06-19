using System.Data.Entity;

namespace AndBurn.HDT.Plugins.StatsConverter.HearthstoneTracker.Model
{   
    public partial class HSDbContext : DbContext
    {
        public HSDbContext(string connectionStringOrName)
            : base(connectionStringOrName)
        {
        }

        public virtual DbSet<ArenaSession> ArenaSessions { get; set; }
        public virtual DbSet<Deck> Decks { get; set; }
        public virtual DbSet<GameResult> GameResults { get; set; }
        public virtual DbSet<Hero> Heroes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ArenaSession>()
                .Property(e => e.Timestamp)
                .IsFixedLength();

            modelBuilder.Entity<ArenaSession>()
                .HasMany(e => e.GameResults)
                .WithOptional(e => e.ArenaSession)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Deck>()
                .HasMany(e => e.GameResults)
                .WithOptional(e => e.Deck)
                .HasForeignKey(e => e.Deck_Id);

            modelBuilder.Entity<GameResult>()
                .Property(e => e.Timestamp)
                .IsFixedLength();

            modelBuilder.Entity<Hero>()
                .HasMany(e => e.ArenaSessions)
                .WithOptional(e => e.Hero)
                .HasForeignKey(e => e.Hero_Id);

            modelBuilder.Entity<Hero>()
                .HasMany(e => e.GameResults)
                .WithOptional(e => e.Hero)
                .HasForeignKey(e => e.Hero_Id);

            modelBuilder.Entity<Hero>()
                .HasMany(e => e.GameResults1)
                .WithOptional(e => e.OpponentHero)
                .HasForeignKey(e => e.OpponentHero_Id);
        }
    }
}
