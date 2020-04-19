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
            if (!context.Species.Any())
            {
            var Species = new Species[]
            {
                new Species{Name="Eucalyptus",Command="eucalyptus"},
                new Species{Name="Eucalyptus grandis",Command="eucalyptus_grandis"},
                new Species{Name="Eucalyptus globulus",Command="eucalyptus_globulus"},
                new Species{Name="Eucalyptus nitens",Command="eucalyptus_nitens"},
                new Species{Name="Gmelina",Command="gmelina"},
                new Species{Name="Pinus radiata",Command="pinus_radiata"},
                new Species{Name="Pinus taeda",Command="pinus_taeda"},
                new Species{Name="Teak",Command="teak"}
            };
            foreach (Species c in Species)
            {
                context.Species.Add(c);
            }
            context.SaveChanges();
            }


            if (!context.Region.Any())
            {
            var Regions = new Region[]
            {
                new Region{Name="Pradaria",Command="pradaria"},
                new Region{Name="Uruguay",Command="uruguay"},
                new Region{Name="FDS",Command="fds"},
                new Region{Name="Uruguay pulp",Command="uruguay_pulp"},
                new Region{Name="Uruguay solid",Command="uruguay_solid"},
                new Region{Name="Uruguay Guanare",Command="uruguay_guanare"},
                new Region{Name="Brazil MS",Command="brazil_ms"},
                new Region{Name="Brazil RS",Command="brazil_rs"},
                new Region{Name="Chile",Command="chile"},
                new Region{Name="Ecuador",Command="ecuador"},
                new Region{Name="New Zealand",Command="new_zealand"},
                new Region{Name="Argentina",Command="argentina"},
                new Region{Name="South East USA",Command="south-east-usa"},
                new Region{Name="Brazil",Command="brazil"},
                new Region{Name="Nicaragua",Command="nicaragua"},
                new Region{Name="Panama",Command="panama"}
            };
            foreach (Region c in Regions)
            {
                context.Region.Add(c);
            }
            context.SaveChanges();
            }

            if (!context.Tree.Any())
            {
                var Trees = new Tree[]
                {
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "eucalyptus").Id,RegionId=context.Region.Single(i => i.Command == "pradaria").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "eucalyptus").Id,RegionId=context.Region.Single(i => i.Command == "uruguay").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "eucalyptus").Id,RegionId=context.Region.Single(i => i.Command == "fds").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "eucalyptus_grandis").Id,RegionId=context.Region.Single(i => i.Command == "uruguay_pulp").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "eucalyptus_grandis").Id,RegionId=context.Region.Single(i => i.Command == "uruguay_solid").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "eucalyptus_grandis").Id,RegionId=context.Region.Single(i => i.Command == "uruguay_guanare").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "eucalyptus_grandis").Id,RegionId=context.Region.Single(i => i.Command == "brazil_ms").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "eucalyptus_grandis").Id,RegionId=context.Region.Single(i => i.Command == "brazil_rs").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "eucalyptus_globulus").Id,RegionId=context.Region.Single(i => i.Command == "uruguay").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "eucalyptus_globulus").Id,RegionId=context.Region.Single(i => i.Command == "chile").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "eucalyptus_nitens").Id,RegionId=context.Region.Single(i => i.Command == "chile").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "gmelina").Id,RegionId=context.Region.Single(i => i.Command == "ecuador").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "pinus_radiata").Id,RegionId=context.Region.Single(i => i.Command == "chile").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "pinus_radiata").Id,RegionId=context.Region.Single(i => i.Command == "new_zealand").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "pinus_taeda").Id,RegionId=context.Region.Single(i => i.Command == "uruguay").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "pinus_taeda").Id,RegionId=context.Region.Single(i => i.Command == "argentina").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "pinus_taeda").Id,RegionId=context.Region.Single(i => i.Command == "south-east-usa").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "teak").Id,RegionId=context.Region.Single(i => i.Command == "brazil").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "teak").Id,RegionId=context.Region.Single(i => i.Command == "ecuador").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "teak").Id,RegionId=context.Region.Single(i => i.Command == "nicaragua").Id},
                new Tree{SpeciesId=context.Species.Single(i => i.Command == "teak").Id,RegionId=context.Region.Single(i => i.Command == "panama").Id},
                };
                foreach (Tree c in Trees)
                {
                    context.Tree.Add(c);
                }
                context.SaveChanges();
            }
        }
    }
}
