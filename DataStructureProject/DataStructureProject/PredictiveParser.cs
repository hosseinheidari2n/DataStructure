using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataStructureProject
{
    public class PredictiveParser
    {
        private Dictionary<string, Dictionary<string, string>> parsingTable;
        private Stack<string> parsingStack;
        private List<Token> tokens;

        public PredictiveParser(Dictionary<string, Dictionary<string, string>> parsingTable, List<Token> tokens)
        {
            this.parsingTable = parsingTable;
            this.tokens = tokens;
            this.parsingStack = new Stack<string>();
        }

        public void ParseInput()
        {

            parsingStack.Push("Start");

            int index = 0;
            Console.WriteLine("Derivation steps:");

            while (parsingStack.Count > 0)
            {
                if (index >= tokens.Count)
                {
                    Console.WriteLine("Error: Reached end of tokens without resolving stack.");
                    return;
                }

                string top = parsingStack.Peek();
                string currentToken = tokens[index].Value;
                string currToken = tokens[index].Type;

                // Match terminal or end-of-input marker
                if (top == currentToken || currToken == top)
                {
                    parsingStack.Pop();
                    index++;  // Move to the next token
                    continue;
                }

                // Handle non-terminal using currentToken
                if (parsingTable.ContainsKey(top) && parsingTable[top].ContainsKey(currentToken))
                {
                    string production = parsingTable[top][currentToken];
                    parsingStack.Pop();

                    if (production != "epsilon")
                    {
                        Console.WriteLine($"{top} -> {production}");

                        var symbols = production.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                        for (int j = symbols.Count - 1; j >= 0; --j)
                        {
                            parsingStack.Push(symbols[j]);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{top} -> epsilon");
                    }
                    continue;
                }

                // Handle non-terminal using currToken
                if (parsingTable.ContainsKey(top) && parsingTable[top].ContainsKey(currToken))
                {
                    string production = parsingTable[top][currToken];
                    parsingStack.Pop();

                    if (production != "epsilon")
                    {
                        Console.WriteLine($"{top} -> {production}");

                        var symbols = production.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

                        for (int j = symbols.Count - 1; j >= 0; --j)
                        {
                            parsingStack.Push(symbols[j]);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{top} -> epsilon");
                    }
                    continue;
                }

                // Error handling: if no match is found
                Console.WriteLine($"Error: Unexpected token '{currentToken}' at top of stack '{top}'");
                return;
            }

            // Ensure that we have processed all tokens
            if (index == tokens.Count)
            {
                Console.WriteLine("Parsing successful!");
            }
            else
            {
                Console.WriteLine("Parsing failed! Unprocessed tokens left.");
            }
        }
    }
}

