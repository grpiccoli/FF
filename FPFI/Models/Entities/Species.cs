using System.Collections.Generic;

namespace FPFI.Models
{
    public class Species
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Command { get; set; }

        public virtual ICollection<Tree> Trees { get; set; }
    }
}
