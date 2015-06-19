using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AndBurn.HDT.Plugins.StatsConverter.HearthstoneTracker.Model
{
    public partial class Hero
    {
        public Hero()
        {
            this.ArenaSessions = new HashSet<ArenaSession>();
            this.GameResults = new HashSet<GameResult>();
            this.GameResults1 = new HashSet<GameResult>();
        }

        public Guid Id { get; set; }

        [StringLength(4000)]
        public string Name { get; set; }

        [StringLength(4000)]
        public string Description { get; set; }

        [StringLength(4000)]
        public string Icon { get; set; }

        [StringLength(4000)]
        public string ClassName { get; set; }

        [StringLength(4000)]
        public string Key { get; set; }

        public virtual ICollection<ArenaSession> ArenaSessions { get; set; }

        public virtual ICollection<GameResult> GameResults { get; set; }

        public virtual ICollection<GameResult> GameResults1 { get; set; }
    }
}
