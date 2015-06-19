using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AndBurn.HDT.Plugins.StatsConverter.HearthstoneTracker.Model
{
    public partial class Deck
    {
        public Deck()
        {
            this.GameResults = new HashSet<GameResult>();
        }

        public Guid Id { get; set; }

        [StringLength(4000)]
        public string Key { get; set; }

        [StringLength(4000)]
        public string Name { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public bool Deleted { get; set; }

        [StringLength(4000)]
        public string Server { get; set; }

        [StringLength(4000)]
        public string Notes { get; set; }

        public virtual ICollection<GameResult> GameResults { get; set; }
    }
}
