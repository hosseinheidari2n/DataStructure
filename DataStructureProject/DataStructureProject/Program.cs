using DataStructureProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace DataStructureProject
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your C++ code (end input with an empty line):");

            // Read C++ code
            string inputCode = ReadUserInput();

            LexicalAnalyzer analyzer = new LexicalAnalyzer();
            List<Token> tokens = analyzer.Analyze(inputCode);

            Console.WriteLine("\nTokens:");
            foreach (var token in tokens)
            {
                Console.WriteLine($"[{token.Type}, {token.Value}]");
            }

            // Bonus: Search for the first declaration
            Console.WriteLine("\nBonus: Searching for first identifier declaration...");
            string firstDeclaration = analyzer.FindFirstDeclaration(tokens);
            Console.WriteLine(firstDeclaration != null
                ? $"First declaration: {firstDeclaration}"
                : "No declarations found.");

            // Bonus: Error handling
            Console.WriteLine("\nBonus: Error Handling...");
            List<string> errors = analyzer.ValidateTokens(tokens);
            if (errors.Count > 0)
            {
                Console.WriteLine("Errors found:");
                foreach (var error in errors)
                {
                    Console.WriteLine(error);
                }
            }
            else
            {
                Console.WriteLine("No errors found.");
            }

            //---------------------------------------------------------------------------------

            // Create Token Table
            Console.WriteLine("\nToken Table:");

            var tokenTable = analyzer.CreateTokenTable();

            // Print the table
            PrintTokenTable(tokenTable);


            //--------------------------------------------------------------------------------

            // Make First , Follow and ParseTable

            var parser = new CFGParser();

            parser.ComputeFirstSets();
            parser.ComputeFollowSets();
            parser.BuildParseTable();

            parser.PrintFirstSets();
            parser.PrintFollowSets();
            parser.PrintParseTable();

            //-----------------------------------------------------------------------------------

            string parseTablePath = "C:\\Users\\pc\\Desktop\\New folder\\parse_table.csv";

            Dictionary<string, Dictionary<string, string>> parseTable1 = LoadParseTable(parseTablePath);
            Dictionary<string, Dictionary<string, string>> parsingTable = new Dictionary<string, Dictionary<string, string>>();

            parsingTable["Start"] = new Dictionary<string, string> { { "#include", "S N M" } };
            parsingTable["S"] = new Dictionary<string, string> { { "#include", "#include S" }, { "using", "epsilon" } };
            parsingTable["N"] = new Dictionary<string, string> { { "using", "using namespace std ;" }, { "int", "epsilon" } };
            parsingTable["M"] = new Dictionary<string, string> { { "int", "int main ( ) { T V }" }, { "using", "epsilon" } };
            parsingTable["T"] = new Dictionary<string, string>
            {
                { "int", "Id T" },
                { "float", "L T" },
                { "while", "Loop T" },
                { "cin", "Input T" },
                { "cout", "Output T" },
                { "epsilon", "epsilon" },
                { "identifier", "L T" },
                { "}", "epsilon" },
                { "return", "epsilon" }
            };

            parsingTable["V"] = new Dictionary<string, string> { { "return", "return 0 ;" }, { "epsilon", "epsilon" } };
            parsingTable["Id"] = new Dictionary<string, string> { { "int", "int L" }, { "float", "float L" } };
            parsingTable["L"] = new Dictionary<string, string> { { "identifier", "identifier Assign Z" } };
            parsingTable["Z"] = new Dictionary<string, string> { { ",", ", identifier Assign Z" }, { ";", ";" } };
            parsingTable["Assign"] = new Dictionary<string, string> { { "=", "= Operation" }, { "epsilon", "epsilon" }, { ";", "epsilon" } };
            parsingTable["Operation"] = new Dictionary<string, string> { { "number", "number P" }, { "identifier", "identifier P" } };
            parsingTable["P"] = new Dictionary<string, string>
            {
                { "+", "O W P" },
                { "-", "O W P" },
                { "*", "O W P" },
                { ",", "epsilon" },
                { ";", "epsilon" },
                { ">=", "epsilon" },
                { "<=", "epsilon" },
                { "==", "epsilon" },
                { ")", "epsilon" },
                { "epsilon", "epsilon" }
            };

            parsingTable["O"] = new Dictionary<string, string> { { "+", "+" }, { "-", "-" }, { "*", "*" } };
            parsingTable["W"] = new Dictionary<string, string> { { "number", "number" }, { "identifier", "identifier" } };
            parsingTable["Expression"] = new Dictionary<string, string> { { "number", "Operation K Operation" }, { "identifier", "Operation K Operation" } };
            parsingTable["K"] = new Dictionary<string, string>
            {
                { "==", "==" },
                { ">=", ">=" },
                { ">", ">=" },
                { "<=", "<=" },
                { "!=", "!=" }
            };

            parsingTable["Loop"] = new Dictionary<string, string> { { "while", "while ( Expression ) { T }" } };
            parsingTable["Input"] = new Dictionary<string, string> { { "cin", "cin >> identifier F ;" } };
            parsingTable["F"] = new Dictionary<string, string> { { ">>", ">> identifier F" }, { "epsilon", "epsilon" }, { ";", "epsilon" } };
            parsingTable["Output"] = new Dictionary<string, string> { { "cout", "cout << C H ;" } };
            parsingTable["H"] = new Dictionary<string, string> { { "<<", "<< C H" }, { "epsilon", "epsilon" }, { ";", "epsilon" } };
            parsingTable["C"] = new Dictionary<string, string> { { "number", "number" }, { "string", "string" }, { "identifier", "identifier" } };

            PredictiveParser parser1 = new PredictiveParser(parsingTable, tokens);
            parser1.ParseInput();

            Console.ReadKey();
        }

        private static string ReadUserInput()
        {
            string line;
            string inputCode = "";

            while ((line = Console.ReadLine()) != null && line.Trim() != "")
            {
                inputCode += line + "\n";
            }

            return inputCode;
        }

        private static void PrintTokenTable(List<(string TokenType, string TokenValue, int HashValue)> tokenTable)
        {
            // Print table headers
            Console.WriteLine("+-----------------+---------------------------+--------------+");
            Console.WriteLine("| Token Type      | Token Value               | Hash Value   |");
            Console.WriteLine("+-----------------+---------------------------+--------------+");

            // Print table rows
            foreach (var entry in tokenTable)
            {
                Console.WriteLine($"| {entry.TokenType,-15} | {entry.TokenValue,-25} | {entry.HashValue,-12} |");
            }

            Console.WriteLine("+-----------------+---------------------------+--------------+");
        }

        private static Dictionary<string, Dictionary<string, string>> LoadParseTable(string path)
        {
            var parseTable = new Dictionary<string, Dictionary<string, string>>();
            var lines = File.ReadAllLines(path);

            var headers = lines[0].Split(',');

            for (int i = 1; i < lines.Length; i++)
            {
                var cells = lines[i].Split(',');
                var nonTerminal = cells[0];

                parseTable[nonTerminal] = new Dictionary<string, string>();

                for (int j = 1; j < cells.Length; j++)
                {
                    if (!string.IsNullOrWhiteSpace(cells[j]))
                    {
                        parseTable[nonTerminal][headers[j]] = cells[j];
                    }
                }
            }

            return parseTable;
        }
    }
}
