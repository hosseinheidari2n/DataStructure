using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructureProject
{
    public class ParseTable
    {
        public Dictionary<string, Dictionary<string, string>> Table { get; private set; } = new Dictionary<string, Dictionary<string, string>>();

        public void AddEntry(string nonTerminal, string terminal, string production)
        {
            if (!Table.ContainsKey(nonTerminal))
                Table[nonTerminal] = new Dictionary<string, string>();

            Table[nonTerminal][terminal] = production;
        }

        public void PrintTable()
        {
            Console.WriteLine("Parse Table:");
            foreach (var nonTerminal in Table.Keys)
            {
                Console.WriteLine($"{nonTerminal}:");
                foreach (var entry in Table[nonTerminal])
                {
                    Console.WriteLine($"  {entry.Key} -> {entry.Value}");
                }
            }
        }
    }
}
