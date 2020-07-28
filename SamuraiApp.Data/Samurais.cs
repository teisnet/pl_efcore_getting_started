using System;
using System.Collections.Generic;

namespace SamuraiApp.Data
{
    public partial class Samurais
    {
        public Samurais()
        {
            Quotes = new HashSet<Quotes>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int? ClanId { get; set; }

        public virtual Clans Clan { get; set; }
        public virtual ICollection<Quotes> Quotes { get; set; }
    }
}
