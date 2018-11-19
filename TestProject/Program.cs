using System;
using System.Collections.Generic;
using System.Linq;
using NEAT;
using System.Text;
using System.Threading.Tasks;


//Settings are a Singleton?
//What if multiple populations are required at once?
//Fix Network initialization


namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            int time = DateTime.Now.Millisecond;
            Random rando = new Random(time);

            List<Tuple<NodeType, ActivationStyle>> nodes = new List<Tuple<NodeType, ActivationStyle>>()
            {
                new Tuple<NodeType, ActivationStyle>( NodeType.BIAS, ActivationStyle.None ),
                new Tuple<NodeType, ActivationStyle>( NodeType.SENSOR, ActivationStyle.None ),
                new Tuple<NodeType, ActivationStyle>( NodeType.SENSOR, ActivationStyle.None ),
                new Tuple<NodeType, ActivationStyle>( NodeType.SENSOR, ActivationStyle.None ),
                new Tuple<NodeType, ActivationStyle>( NodeType.SENSOR, ActivationStyle.None ),
                new Tuple<NodeType, ActivationStyle>( NodeType.OUTPUT, ActivationStyle.SigmoidNEAT ),
                new Tuple<NodeType, ActivationStyle>( NodeType.OUTPUT, ActivationStyle.SigmoidNEAT )
            };

            List<Tuple<int, int, double>> connections = new List<Tuple<int, int, double>>()
            {
                /*
                new Tuple<int, int, double>(0,5,rando.NextDouble() * 16 - 8),
                new Tuple<int, int, double>(1,5,rando.NextDouble() * 16 - 8),
                new Tuple<int, int, double>(2,5,rando.NextDouble() * 16 - 8),
                new Tuple<int, int, double>(3,5,rando.NextDouble() * 16 - 8),
                new Tuple<int, int, double>(4,5,rando.NextDouble() * 16 - 8),
                new Tuple<int, int, double>(0,6,rando.NextDouble() * 16 - 8),
                new Tuple<int, int, double>(1,6,rando.NextDouble() * 16 - 8),
                new Tuple<int, int, double>(2,6,rando.NextDouble() * 16 - 8),
                new Tuple<int, int, double>(3,6,rando.NextDouble() * 16 - 8),
                new Tuple<int, int, double>(4,6,rando.NextDouble() * 16 - 8)
                */
                
                new Tuple<int, int, double>(0,5,0.0),
                new Tuple<int, int, double>(1,5,0.0),
                new Tuple<int, int, double>(2,5,0.0),
                new Tuple<int, int, double>(3,5,0.0),
                new Tuple<int, int, double>(4,5,0.0),
                new Tuple<int, int, double>(0,6,0.0),
                new Tuple<int, int, double>(1,6,0.0),
                new Tuple<int, int, double>(2,6,0.0),
                new Tuple<int, int, double>(3,6,0.0),
                new Tuple<int, int, double>(4,6,0.0)

            };

            Population p = new Population(nodes, connections, 150, rando);

            do
            {
                List<Genome> genomes = p.GetAllGenomes();

                foreach (var gen in genomes)
                {
                    Network n = gen.GetPhenotype();
                    Pole1.Pole1_Evaluate(rando, n, gen.Fitness);
                }

                PrintGeneration(p, time);
                p.Epoch(rando);

                PrintGenome(p.Fitness.GenerationalChampion);

            } while (p.Fitness.ChampionScore < 100001 && p.Generation < 100);

            Console.Read();
        }

        public static void PrintGeneration(Population p, int seed)
        {
            Console.WriteLine("---------------");
            Console.WriteLine("Generation: " + p.Generation + " (" + seed + ")");

            Console.Write("Species: " + p.Species.Count + " [ ");
            Console.Write(p.Species[0].Size());
            for (int i = 1; i < p.Species.Count; i++)
            {
                Console.Write(", " + p.Species[i].Size());
            }

            Console.WriteLine(" ]");
            Console.WriteLine("Best: " + p.Fitness.ChampionScore);
        }

        public static void PrintGenome(Genome g)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@"C:\Users\Paul\Desktop\Test.txt"))
            {
                foreach (var gene in g.Genes)
                {
                    if (!gene.Frozen)
                    {
                        Console.WriteLine(gene.link.InNode + " -> " + gene.link.OutNode + " ->\t" + gene.Weight);
                        file.WriteLine(gene.link.InNode + " -> " + gene.link.OutNode + " ->\t" + gene.Weight);
                    }
                }
                Console.WriteLine(g.Fitness.Score);
                file.WriteLine(g.Fitness.Score);

                if (g.Validate(true))
                {
                    Console.WriteLine("Valid!");
                    file.WriteLine("Valid!");
                }
                else
                {
                    Console.WriteLine("BAD!!");
                    file.WriteLine("BAD!!");
                }
            }
        }

    }
}
