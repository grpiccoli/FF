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
                new Region{Name="Uruguay solid2",Command="uruguay_solid2"},
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
                var e = context.Species.Single(i => i.Command == "eucalyptus").Id;
                var egrandis = context.Species.Single(i => i.Command == "eucalyptus_grandis").Id;
                var eglobolus = context.Species.Single(i => i.Command == "eucalyptus_globulus").Id;
                var enitens = context.Species.Single(i => i.Command == "eucalyptus_nitens").Id;
                var g = context.Species.Single(i => i.Command == "gmelina").Id;
                var pradiata = context.Species.Single(i => i.Command == "pinus_radiata").Id;
                var ptaeda = context.Species.Single(i => i.Command == "pinus_taeda").Id;
                var t = context.Species.Single(i => i.Command == "teak").Id;

                var Trees = new Tree[]
                {
                    new Tree { SpeciesId = e,       RegionId = context.Region.Single(i => i.Command == "brazil_ms").Id, Default = false },
                    new Tree { SpeciesId = e,       RegionId = context.Region.Single(i => i.Command == "uruguay").Id, Default = false },
                    new Tree { SpeciesId = e,       RegionId = context.Region.Single(i => i.Command == "brazil_rs").Id, Default = false },
                    new Tree { SpeciesId = egrandis,RegionId = context.Region.Single(i => i.Command == "uruguay_pulp").Id,Default=false},
                    new Tree { SpeciesId = egrandis,RegionId = context.Region.Single(i => i.Command == "uruguay_solid").Id,Default=true},
                    new Tree { SpeciesId = egrandis,RegionId = context.Region.Single(i => i.Command == "uruguay_solid2").Id,Default=false},
                    new Tree { SpeciesId = egrandis,RegionId = context.Region.Single(i => i.Command == "brazil_ms").Id,Default=false},
                    new Tree { SpeciesId = egrandis,RegionId = context.Region.Single(i => i.Command == "brazil_rs").Id,Default=false},
                    new Tree { SpeciesId = eglobolus,RegionId = context.Region.Single(i => i.Command == "uruguay").Id,Default=false},
                    new Tree { SpeciesId = eglobolus,RegionId = context.Region.Single(i => i.Command == "chile").Id,Default=false},
                    new Tree { SpeciesId = enitens, RegionId = context.Region.Single(i => i.Command == "chile").Id,Default=false},
                    new Tree { SpeciesId = g,       RegionId = context.Region.Single(i => i.Command == "ecuador").Id,Default=false},
                    new Tree { SpeciesId = pradiata,RegionId = context.Region.Single(i => i.Command == "chile").Id,Default=false},
                    new Tree { SpeciesId = pradiata,RegionId = context.Region.Single(i => i.Command == "new_zealand").Id,Default=false},
                    new Tree { SpeciesId = ptaeda,  RegionId = context.Region.Single(i => i.Command == "uruguay").Id,Default=false},
                    new Tree { SpeciesId = ptaeda,  RegionId = context.Region.Single(i => i.Command == "argentina").Id,Default=false},
                    new Tree { SpeciesId = ptaeda,  RegionId = context.Region.Single(i => i.Command == "south-east-usa").Id,Default=false},
                    new Tree { SpeciesId = t,       RegionId = context.Region.Single(i => i.Command == "brazil").Id,Default=false},
                    new Tree { SpeciesId = t,       RegionId = context.Region.Single(i => i.Command == "ecuador").Id,Default=false},
                    new Tree { SpeciesId = t,       RegionId = context.Region.Single(i => i.Command == "nicaragua").Id,Default=false},
                    new Tree { SpeciesId = t,       RegionId = context.Region.Single(i => i.Command == "panama").Id,Default=false}
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
