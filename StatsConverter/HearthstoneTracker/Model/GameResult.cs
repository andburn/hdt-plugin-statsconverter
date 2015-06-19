using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AndBurn.HDT.Plugins.StatsConverter.HearthstoneTracker.Model
{
    public partial class GameResult
    {
        public Guid Id { get; set; }

        public bool Victory { get; set; }

        public bool GoFirst { get; set; }

        public DateTime Started { get; set; }

        public DateTime Stopped { get; set; }

        [StringLength(4000)]
        public string DeckKey { get; set; }

        public int GameMode { get; set; }

        [StringLength(4000)]
        public string Notes { get; set; }

        public Guid? ArenaSessionId { get; set; }

        public int ArenaGameNo { get; set; }

        public int Turns { get; set; }

        public bool Conceded { get; set; }

        public Guid? Hero_Id { get; set; }

        public Guid? OpponentHero_Id { get; set; }

        [StringLength(4000)]
        public string Server { get; set; }

        public Guid? Deck_Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        [Column(TypeName = "rowversion")]
        [Required]
        [MaxLength(8)]
        public byte[] Timestamp { get; set; }

        public virtual ArenaSession ArenaSession { get; set; }

        public virtual Deck Deck { get; set; }

        public virtual Hero Hero { get; set; }

        public virtual Hero OpponentHero { get; set; }
    }
}
