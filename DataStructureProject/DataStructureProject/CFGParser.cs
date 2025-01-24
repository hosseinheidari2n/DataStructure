using System;
using System.Collections.Generic;
using System.Linq;

namespace DataStructureProject
{
    public class CFGParser
    {
        // Rule definition: Non-terminal -> List of Productions
        private readonly Dictionary<string, List<string>> grammar = new Dictionary<string, List<string>>
        {
             { "Start", new List<string> { "SNM" } },
             { "S", new List<string> { "#include S", "ϵ" } },
             { "N", new List<string> { "using namespace std;", "ϵ" } },
             { "M", new List<string> { "int main(){TV}" } },
             { "T", new List<string> { "Id T", "L T", "Loop T", "Input T", "Output T", "ϵ" } },
             { "V", new List<string> { "return 0;", "ϵ" } },
             { "Id", new List<string> { "int L", "float L" } },
             { "L", new List<string> { "identifier Assign Z" } },
             { "Z", new List<string> { ", identifier Assign Z", ";" } },
             { "Operation", new List<string> { "number P", "identifier P" } },
             { "P", new List<string> { "O W P", "ϵ" } },
             { "O", new List<string> { "+", "−", "∗" } },
             { "W", new List<string> { "number", "identifier" } },
             { "Assign", new List<string> { "= Operation", "ϵ" } },
             { "Expression", new List<string> { "Operation K Operation" } },
             { "K", new List<string> { "==", ">=", "<=", "!=" } },
             { "Loop", new List<string> { "while(Expression){T}" } },
             { "Input", new List<string> { "cin >> identifier F;" } },
             { "F", new List<string> { ">> identifier F", "ϵ" } },
             { "Output", new List<string> { "cout << CH;" } },
             { "H", new List<string> { "<< CH", "ϵ" } },
             { "C", new List<string> { "number", "string", "identifier" } }
        };

        private readonly Dictionary<string, HashSet<string>> firstSets = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<string, HashSet<string>> followSets = new Dictionary<string, HashSet<string>>();
        private readonly Dictionary<(string, string), string> parseTable = new Dictionary<(string, string), string>();

        public void ComputeFirstSets()
        {
            foreach (var nonTerminal in grammar.Keys)
                firstSets[nonTerminal] = new HashSet<string>();

            bool updated;
            do
            {
                updated = false;

                foreach (var kvp in grammar)
                {
                    var nonTerminal = kvp.Key;
                    var productions = kvp.Value;
                    foreach (var production in productions)
                    {
                        var symbols = production.Split(' ');

                        foreach (var symbol in symbols)
                        {
                            if (!grammar.ContainsKey(symbol)) // Terminal
                            {
                                if (firstSets[nonTerminal].Add(symbol))
                                    updated = true;
                                break;
                            }
                            else // Non-terminal
                            {
                                var currentFirst = firstSets[symbol].Where(s => s != "ϵ").ToHashSet();
                                if (currentFirst.Any(s => firstSets[nonTerminal].Add(s)))
                                    updated = true;

                                if (!firstSets[symbol].Contains("ϵ"))
                                    break;
                            }
                        }
                    }
                }
            } while (updated);
        }

        public void ComputeFollowSets()
        {
            foreach (var nonTerminal in grammar.Keys)
                followSets[nonTerminal] = new HashSet<string>();

            followSets["Start"].Add("$");

            bool updated;
            do
            {
                updated = false;

                foreach (var kvp in grammar)    
                {
                    var nonTerminal = kvp.Key;
                    var productions = kvp.Value;
                    foreach (var production in productions)
                    {
                        var symbols = production.Split(' ');

                        for (int i = 0; i < symbols.Length; i++)
                        {
                            if (!grammar.ContainsKey(symbols[i])) continue;

                            var follow = followSets[symbols[i]];

                            // Add First of the rest of the production
                            for (int j = i + 1; j < symbols.Length; j++)
                            {
                                var nextSymbol = symbols[j];
                                if (!grammar.ContainsKey(nextSymbol)) // Terminal
                                {
                                    if (follow.Add(nextSymbol))
                                        updated = true;
                                    break;
                                }
                                else
                                {
                                    var firstNext = firstSets[nextSymbol].Where(s => s != "ϵ").ToHashSet();
                                    if (firstNext.Any(s => follow.Add(s)))
                                        updated = true;

                                    if (!firstSets[nextSymbol].Contains("ϵ"))
                                        break;
                                }
                            }

                            // Add Follow of current non-terminal if rest contains ϵ
                            if (i == symbols.Length - 1 || symbols.Skip(i + 1).All(s => grammar.ContainsKey(s) && firstSets[s].Contains("ϵ")))
                            {
                                foreach (var f in followSets[nonTerminal])
                                {
                                    if (follow.Add(f))
                                        updated = true;
                                }
                            }
                        }
                    }
                }
            } while (updated);
        }

        public void BuildParseTable()
        {
            foreach (var kvp in grammar)
            {
                var nonTerminal = kvp.Key;
                var productions = kvp.Value;
                foreach (var production in productions)
                {
                    var first = new HashSet<string>();
                    var symbols = production.Split(' ');

                    foreach (var symbol in symbols)
                    {
                        if (!grammar.ContainsKey(symbol)) // Terminal
                        {
                            first.Add(symbol);
                            break;
                        }
                        else
                        {
                            first.UnionWith(firstSets[symbol].Where(s => s != "ϵ"));
                            if (!firstSets[symbol].Contains("ϵ"))
                                break;
                        }
                    }

                    if (symbols.All(s => grammar.ContainsKey(s) && firstSets[s].Contains("ϵ")))
                        first.Add("ϵ");

                    foreach (var terminal in first)
                    {
                        if (terminal == "ϵ")
                        {
                            foreach (var follow in followSets[nonTerminal])
                                parseTable[(nonTerminal, follow)] = production;
                        }
                        else
                        {
                            parseTable[(nonTerminal, terminal)] = production;
                        }
                    }
                }
            }
        }

        public void PrintParseTable()
        {
            Console.WriteLine("\nParse Table:");
            Console.WriteLine("+-------------------+-------------------+-----------------------+");
            Console.WriteLine("| Non-Terminal      | Terminal          | Production            |");
            Console.WriteLine("+-------------------+-------------------+-----------------------+");

            foreach (var entry in parseTable)
            {
                var (nonTerminal, terminal) = entry.Key;
                string production = entry.Value;
                Console.WriteLine($"| {nonTerminal,-17} | {terminal,-17} | {production,-21} |");
            }

            Console.WriteLine("+-------------------+-------------------+-----------------------+");
        }

        public void PrintFirstSets()
        {
            Console.WriteLine("\nFirst Sets:");
            Console.WriteLine("+-------------------+-----------------------------+");
            Console.WriteLine("| Non-Terminal      | First Set                  |");
            Console.WriteLine("+-------------------+-----------------------------+");

            foreach (var entry in firstSets)
            {
                string nonTerminal = entry.Key;
                string firstSet = string.Join(", ", entry.Value);
                Console.WriteLine($"| {nonTerminal,-17} | {firstSet,-27} |");
            }

            Console.WriteLine("+-------------------+-----------------------------+");
        }

        public void PrintFollowSets()
        {
            Console.WriteLine("\nFollow Sets:");
            Console.WriteLine("+-------------------+-----------------------------+");
            Console.WriteLine("| Non-Terminal      | Follow Set                 |");
            Console.WriteLine("+-------------------+-----------------------------+");

            foreach (var entry in followSets)
            {
                string nonTerminal = entry.Key;
                string followSet = string.Join(", ", entry.Value);
                Console.WriteLine($"| {nonTerminal,-17} | {followSet,-27} |");
            }

            Console.WriteLine("+-------------------+-----------------------------+");
        }
    }
}
