using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
