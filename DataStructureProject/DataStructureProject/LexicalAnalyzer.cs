using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataStructureProject
{
    public class LexicalAnalyzer
    {
        public List<Token> Tokens { get; private set; } = new List<Token>();

        public List<Token> Analyze(string input)
        {
            Tokens.Clear();
            int position = 0;

            while (position < input.Length)
            {
                char current = input[position];

                if (IsWhitespace(current))
                {
                    position++;
                    continue;
                }

                if (IsLetter(current))
                {
                    string word = ReadWhile(input, ref position, IsLetterOrDigit);
                    string type = IsReservedWord(word) ? "reservedword" : "identifier";
                    Tokens.Add(new Token(type, word));
                }
                else if (current == '#' && input.Substring(position).StartsWith("#include"))
                {
                    Tokens.Add(new Token("reservedword", "#include"));
                    position += 8;
                }
                else if (IsDigit(current))
                {
                    string number = ReadWhile(input, ref position, IsDigit);
                    Tokens.Add(new Token("number", number));
                }
                else if (IsSymbol(current))
                {
                    string symbol = current.ToString();

                    if (position + 1 < input.Length)
                    {
                        char next = input[position + 1];


                        if ((current == '<' || current == '>') && (next == '=' || next == current))
                        {
                            symbol += next;
                            position++;
                        }
                    }

                    Tokens.Add(new Token("symbol", symbol));
                    position++;
                }
                else if (current == '"')
                {
                    string str = ReadString(input, ref position);
                    Tokens.Add(new Token("string", str));
                }
                else
                {
                    Tokens.Add(new Token("unknown", current.ToString()));
                    position++;
                }
            }

            return Tokens;
        }

        private static bool IsLetter(char ch) => char.IsLetter(ch);
        private static bool IsDigit(char ch) => char.IsDigit(ch);
        private static bool IsLetterOrDigit(char ch) => char.IsLetterOrDigit(ch) || ch == '_';
        private static bool IsWhitespace(char ch) => char.IsWhiteSpace(ch);

        private static bool IsReservedWord(string st)
        {
            var reservedWords = new HashSet<string>
            {
                 "int", "float", "void", "return", "if", "while", "cin", "cout", "continue",
                 "break", "using", "iostream", "namespace", "std", "main"
            };
            return reservedWords.Contains(st);
        }

        private static bool IsSymbol(char ch)
        {
            var symbols = new HashSet<char>
            {
                '(', ')', '[', ']', ',', ';', '+', '*', '-', '/', '=', '<', '>', '|', '&', '{', '}'
            };
            return symbols.Contains(ch);
        }

        private static string ReadWhile(string input, ref int position, Func<char, bool> condition)
        {
            int start = position;
            while (position < input.Length && condition(input[position]))
            {
                position++;
            }
            return input.Substring(start, position - start);
        }

        private static string ReadString(string input, ref int position)
        {
            int start = position++;
            while (position < input.Length && input[position] != '"')
            {
                position++;
            }
            position++;
            return input.Substring(start, position - start);
        }


        // Bonus: Find the first declaration
        public string FindFirstDeclaration(List<Token> tokens)
        {
            for (int i = 0; i < tokens.Count - 1; i++)
            {
                if (tokens[i].Type == "reservedword" &&
                    (tokens[i].Value == "int" || tokens[i].Value == "float") &&
                    tokens[i + 1].Type == "identifier")
                {
                    return tokens[i + 1].Value;
                }
            }
            return null;
        }

        public List<string> ValidateTokens(List<Token> tokens)
        {
            List<string> errors = new List<string>();

            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].Type == "unknown")
                {
                    errors.Add($"Unknown token: {tokens[i].Value}");
                }

                // Check for missing semicolon at the end of statements
                if (tokens[i].Type == "reservedword" &&
                    (tokens[i].Value == "while" || tokens[i].Value == "if" || tokens[i].Value == "return" || tokens[i].Value == "int" || tokens[i].Value == "float"))
                {
                    // Make sure the next token is a semicolon
                    bool hasSemicolon = false;
                    int j = i + 1;

                    // Skip over any tokens like identifiers or operators that might be part of the statement
                    while (j < tokens.Count && tokens[j].Type != "symbol")
                    {
                        j++;
                    }

                    // Check for semicolon at the end of the statement
                    if (j < tokens.Count && tokens[j].Value == ";")
                    {
                        hasSemicolon = true;
                    }

                    if (!hasSemicolon)
                    {
                        errors.Add($"Missing semicolon after '{tokens[i].Value}' statement.");
                    }
                }

                // Check for invalid assignment (ensuring valid operands after assignment operator)
                if (tokens[i].Type == "identifier" &&
                    i + 2 < tokens.Count && tokens[i + 1].Value == "=" &&
                    tokens[i + 2].Type != "number" && tokens[i + 2].Type != "identifier")
                {
                    errors.Add($"Invalid assignment to {tokens[i].Value}");
                }
            }

            return errors;
        }


        public List<(string TokenType, string TokenValue, int HashValue)> CreateTokenTable()
        {
            return Tokens
                .OrderBy(t => t.Hash)
                .ThenBy(t => t.Type)
                .Select(t => (t.Type, t.Value, t.Hash))
                .ToList();
        }
    }
}
