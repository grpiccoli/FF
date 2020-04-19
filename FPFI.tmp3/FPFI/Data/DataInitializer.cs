using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPFI.Models;

namespace FPFI.Data
{
    public class DataInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            //if (context.Species.Any())
            //{
            //    return;
            //}
            var Species = new Species[]
            {
                //new Species{SpeciesName=Sp3.eucalyptus,Region=Region.pradaria},
                //new Species{SpeciesName=Sp3.eucalyptus,Region=Region.uruguay},
                //new Species{SpeciesName=Sp3.eucalyptus,Region=Region.fds},
                //new Species{SpeciesName=Sp3.eucalyptus_grandis,Region=Region.uruguay_pulp},
                //new Species{SpeciesName=Sp3.eucalyptus_grandis,Region=Region.uruguay_solid},
                //new Species{SpeciesName=Sp3.eucalyptus_grandis,Region=Region.uruguay_guanare},
                //new Species{SpeciesName=Sp3.eucalyptus_grandis,Region=Region.brazil_ms},
                //new Species{SpeciesName=Sp3.eucalyptus_grandis,Region=Region.brazil_rs},
                //new Species{SpeciesName=Sp3.eucalyptus_globulus,Region=Region.uruguay},
                //new Species{SpeciesName=Sp3.eucalyptus_globulus,Region=Region.chile},
                //new Species{SpeciesName=Sp3.eucalyptus_nitens,Region=Region.chile},
                //new Species{SpeciesName=Sp3.gmelina,Region=Region.ecuador},
                //new Species{SpeciesName=Sp3.pinus_radiata,Region=Region.chile},
                //new Species{SpeciesName=Sp3.pinus_radiata,Region=Region.new_zealand},
                //new Species{SpeciesName=Sp3.pinus_taeda,Region=Region.uruguay},
                //new Species{SpeciesName=Sp3.pinus_taeda,Region=Region.argentina},
                //new Species{SpeciesName=Sp3.pinus_taeda,Region=Region.south_east_usa},
                //new Species{SpeciesName=Sp3.teak,Region=Region.brazil},
                //new Species{SpeciesName=Sp3.teak,Region=Region.ecuador},
                //new Species{SpeciesName=Sp3.teak,Region=Region.nicaragua},
                //new Species{SpeciesName=Sp3.teak,Region=Region.panama}
            };
        }
    }
}
