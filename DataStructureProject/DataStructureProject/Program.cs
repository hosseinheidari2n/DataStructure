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

            var parseTable2 = new Dictionary<string, Dictionary<string, string>>
            {
                            { "Start", new Dictionary<string, string>
        {
            { "include", "Start -> SNM" }, 
            { "using namespace std;", "" },
            { "int main()", "" },
            { "int", "" },
            { "float", "" },
            { "while", "" },
            { "cin", "" },
            { "cout", "" },
            { "identifier", "" },
            { "number", "" },
            { "+", "" },
            { "-", "" },
            { "*", "" },
            { ",", "" },
            { ";", "" },
            { "=", "" },
            { "==", "" },
            { ">=", "" },
            { "<=", "" },
            { "!=", "" },
            { ">>", "" },
            { "<<", "" },
            { "string", "" }
        }
    },
                { "S", new Dictionary<string, string>
        {
            { "include", "S -> include S" },
            { "using namespace std;", "" },
            { "int main()", "" },
            { "int", "" },
            { "float", "" },
            { "while", "" },
            { "cin", "" },
            { "cout", "" },
            { "identifier", "" },
            { "number", "" },
            { "+", "" },
            { "-", "" },
            { "*", "" },
            { ",", "" },
            { ";", "" },
            { "=", "" },
            { "==", "" },
            { ">=", "" },
            { "<=", "" },
            { "!=", "" },
            { ">>", "" },
            { "<<", "" },
            { "string", "" }
        }
    },
                { "N", new Dictionary<string, string>
                    {
                        { "#include", "" },
                        { "using namespace std;", "N -> using namespace std;" },
                        { "int main()", "" },
                        { "int", "" },
                        { "float", "" },
                        { "while", "" },
                        { "cin", "" },
                        { "cout", "" },
                        { "identifier", "" },
                        { "number", "" },
                        { "+", "" },
                        { "-", "" },
                        { "*", "" },
                        { ",", "" },
                        { ";", "" },
                        { "=", "" },
                        { "==", "" },
                        { ">=", "" },
                        { "<=", "" },
                        { "!=", "" },
                        { ">>", "" },
                        { "<<", "" },
                        { "string", "" }
                    }
                },
                { "M", new Dictionary<string, string>
                    {
                        { "#include", "" },
                        { "using namespace std;", "" },
                        { "int main()", "M -> int main(){T V}" },
                        { "int", "" },
                        { "float", "" },
                        { "while", "" },
                        { "cin", "" },
                        { "cout", "" },
                        { "identifier", "" },
                        { "number", "" },
                        { "+", "" },
                        { "-", "" },
                        { "*", "" },
                        { ",", "" },
                        { ";", "" },
                        { "=", "" },
                        { "==", "" },
                        { ">=", "" },
                        { "<=", "" },
                        { "!=", "" },
                        { ">>", "" },
                        { "<<", "" },
                        { "string", "" }
                    }
                },
                { "T", new Dictionary<string, string>
                    {
                        { "#include", "" },
                        { "using namespace std;", "" },
                        { "int main()", "" },
                        { "int", "T -> Id T" },
                        { "float", "T -> Id T" },
                        { "while", "T -> Loop" },
                        { "cin", "T -> Input" },
                        { "cout", "T -> Output" },
                        { "identifier", "T -> Id T" },
                        { "number", "" },
                        { "+", "" },
                        { "-", "" },
                        { "*", "" },
                        { ",", "" },
                        { ";", "" },
                        { "=", "" },
                        { "==", "" },
                        { ">=", "" },
                        { "<=", "" },
                        { "!=", "" },
                        { ">>", "" },
                        { "<<", "" },
                        { "string", "" }
                    }
                },
                { "V", new Dictionary<string, string>
                    {
                        { "#include", "" },
                        { "using namespace std;", "" },
                        { "int main()", "" },
                        { "int", "" },
                        { "float", "" },
                        { "while", "" },
                        { "cin", "" },
                        { "cout", "" },
                        { "identifier", "" },
                        { "number", "" },
                        { "+", "" },
                        { "-", "" },
                        { "*", "" },
                        { ",", "" },
                        { ";", "V -> return 0;" },
                        { "=", "" },
                        { "==", "" },
                        { ">=", "" },
                        { "<=", "" },
                        { "!=", "" },
                        { ">>", "" },
                        { "<<", "" },
                        { "string", "" }
                    }
                },
                { "Id", new Dictionary<string, string>
                    {
                        { "#include", "" },
                        { "using namespace std;", "" },
                        { "int main()", "" },
                        { "int", "Id -> int L" },
                        { "float", "Id -> float L" },
                        { "while", "" },
                        { "cin", "" },
                        { "cout", "" },
                        { "identifier", "" },
                        { "number", "" },
                        { "+", "" },
                        { "-", "" },
                        { "*", "" },
                        { ",", "" },
                        { ";", "" },
                        { "=", "" },
                        { "==", "" },
                        { ">=", "" },
                        { "<=", "" },
                        { "!=", "" },
                        { ">>", "" },
                        { "<<", "" },
                        { "string", "" }
                    }
                },
                { "L", new Dictionary<string, string>
                    {
                        { "#include", "" },
                        { "using namespace std;", "" },
                        { "int main()", "" },
                        { "int", "" },
                        { "float", "" },
                        { "while", "" },
                        { "cin", "" },
                        { "cout", "" },
                        { "identifier", "L -> identifier Assign Z" },
                        { "number", "" },
                        { "+", "" },
                        { "-", "" },
                        { "*", "" },
                        { ",", "L -> identifier Assign Z" },
                        { ";", "L -> identifier Assign Z" },
                        { "=", "" },
                        { "==", "" },
                        { ">=", "" },
                        { "<=", "" },
                        { "!=", "" },
                        { ">>", "" },
                        { "<<", "" },
                        { "string", "" }
                    }
                },
                { "Z", new Dictionary<string, string>
                    {
                        { "#include", "" },
                        { "using namespace std;", "" },
                        { "int main()", "" },
                        { "int", "" },
                        { "float", "" },
                        { "while", "" },
                        { "cin", "" },
                        { "cout", "" },
                        { "identifier", "" },
                        { "number", "" },
                        { "+", "" },
                        { "-", "" },
                        { "*", "" },
                        { ",", "Z -> , identifier Assign Z" },
                        { ";", "Z -> ;" },
                        { "=", "" },
                        { "==", "" },
                        { ">=", "" },
                        { "<=", "" },
                        { "!=", "" },
                        { ">>", "" },
                        { "<<", "" },
                        { "string", "" }
                    }
                },
                { "Operation", new Dictionary<string, string>
                    {
                        { "#include", "" },
                        { "using namespace std;", "" },
                        { "int main()", "" },
                        { "int", "" },
                        { "float", "" },
                        { "while", "" },
                        { "cin", "" },
                        { "cout", "" },
                        { "identifier", "Operation -> identifier P" },
                        { "number", "Operation -> number P" },
                        { "+", "" },
                        { "-", "" },
                        { "*", "" },
                        { ",", "" },
                        { ";", "" },
                        { "=", "" },
                        { "==", "" },
                        { ">=", "" },
                        { "<=", "" },
                        { "!=", "" },
                        { ">>", "" },
                        { "<<", "" },
                        { "string", "" }
                    }
                },
                { "P", new Dictionary<string, string>
                    {
                        { "#include", "" },
                        { "using namespace std;", "" },
                        { "int main()", "" },
                        { "int", "" },
                        { "float", "" },
                        { "while", "" },
                        { "cin", "" },
                        { "cout", "" },
                        { "identifier", "P -> O W P" },
                        { "number", "P -> O W P" },
                        { "+", "" },
                        { "-", "" },
                        { "*", "" },
                        { ",", "" },
                        { ";", "" },
                        { "=", "" },
                        { "==", "" },
                        { ">=", "" },
                        { "<=", "" },
                        { "!=", "" },
                        { ">>", "" },
                        { "<<", "" },
                        { "string", "" }
                    }
                },
                { "O", new Dictionary<string, string>
                    {
                        { "#include", "" },
                        { "using namespace std;", "" },
                        { "int main()", "" },
                        { "int", "" },
                        { "float", "" },
                        { "while", "" },
                        { "cin", "" },
                        { "cout", "" },
                        { "identifier", "" },
                    }
                }
            };
            CFGParser p = new CFGParser();

            PredictiveParser parser1 = new PredictiveParser(parseTable1, tokens);
            parser1.Parse();

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
