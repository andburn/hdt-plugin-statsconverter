using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AndBurn.HDT.Plugins.StatsConverter.HearthstoneTracker.Model
{
    public partial class ArenaSession
    {
        public ArenaSession()
        {
            this.GameResults = new HashSet<GameResult>();
        }

        public Guid Id { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int Wins { get; set; }

        public int Losses { get; set; }

        public int RewardGold { get; set; }

        public int RewardDust { get; set; }

        public int RewardPacks { get; set; }

        [StringLength(4000)]
        public string RewardOther { get; set; }

        public bool Retired { get; set; }

        public Guid? Hero_Id { get; set; }

        [StringLength(4000)]
        public string Notes { get; set; }

        [StringLength(4000)]
        public string Server { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        [Column(TypeName = "rowversion")]
        [Required]
        [MaxLength(8)]
        public byte[] Timestamp { get; set; }

        public Guid? Image1_Id { get; set; }

        public Guid? Image2_Id { get; set; }

        public virtual Hero Hero { get; set; }

        public virtual ICollection<GameResult> GameResults { get; set; }
    }
}
