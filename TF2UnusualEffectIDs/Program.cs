using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Extensions;
using Newtonsoft.Json;

namespace TF2UnusualEffectIDs
{
    class Program
    {
        class ParticleInfo
        {
            public string Name { get; set; }
            public int ParticleId { get; set; }
            public string SystemName { get; set; }
        }

        static async Task Main(string[] args)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var address = "http://optf2.com/440/particles";
            var document = await BrowsingContext.New(config).OpenAsync(address);
            var cells = document.QuerySelector("#particle-list")
                .Children.Where(x => x.ClassName == "item_cell").ToList()
                .Select(x => x.Children.FirstOrDefault(y => y.ClassName == "tooltip"));

            var infos = new List<ParticleInfo>(130);

            foreach (var cell in cells)
            {
                var info = new ParticleInfo
                {
                    Name = cell.Children[0].Text(),
                    ParticleId = int.Parse(cell.Children[1].Text().Replace("ID: ", "")),
                    SystemName = cell.Children[2].Text().Replace("System name: ", "")
                };

                infos.Add(info);
            }

            File.WriteAllText("particle_info.json", JsonConvert.SerializeObject(infos, Formatting.Indented));
            
            Console.WriteLine("particle_info.json written.");
            Console.ReadLine();
        }
    }
}
