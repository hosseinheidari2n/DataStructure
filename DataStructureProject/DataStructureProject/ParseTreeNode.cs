using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DataStructureProject
{
    public class ParseTreeNode : TreeNode
    {
        public ParseTreeNode(string text) : base(text) { }
        public void AddChild(ParseTreeNode child) => Nodes.Add(child);
    }

    public class ParseTree
    {
        public ParseTreeNode Root { get; set; }

        public ParseTree()
        {
            Root = new ParseTreeNode("Start");
        }

        public void GenerateParseTree(List<Token> tokens, Dictionary<string, Dictionary<string, string>> parsingTable)
        {
            var parsingStack = new Stack<ParseTreeNode>();
            parsingStack.Push(Root);

            int index = 0;

            while (parsingStack.Count > 0)
            {
                if (index >= tokens.Count)
                {
                    MessageBox.Show("Error: Reached end of tokens without resolving stack.");
                    return;
                }

                ParseTreeNode topNode = parsingStack.Pop();
                string top = topNode.Text;
                string currentToken = tokens[index].Value;
                string currToken = tokens[index].Type;

                if (top == currentToken || currToken == top)
                {
                    index++;
                    continue;
                }

                if (parsingTable.ContainsKey(top) && parsingTable[top].ContainsKey(currentToken))
                {
                    string production = parsingTable[top][currentToken];
                    if (production != "epsilon")
                    {
                        var symbols = production.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        for (int j = symbols.Count - 1; j >= 0; --j)
                        {
                            var newNode = new ParseTreeNode(symbols[j]);
                            topNode.AddChild(newNode);
                            parsingStack.Push(newNode);
                        }
                    }
                    continue;
                }

                if (parsingTable.ContainsKey(top) && parsingTable[top].ContainsKey(currToken))
                {
                    string production = parsingTable[top][currToken];
                    if (production != "epsilon")
                    {
                        var symbols = production.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        for (int j = symbols.Count - 1; j >= 0; --j)
                        {
                            var newNode = new ParseTreeNode(symbols[j]);
                            topNode.AddChild(newNode);
                            parsingStack.Push(newNode);
                        }
                    }
                    continue;
                }

                MessageBox.Show($"Error: Unexpected token '{currentToken}' at top of stack '{top}'");
                return;
            }

            if (index == tokens.Count)
            {
                MessageBox.Show("Parsing successful!");
                DisplayTree();
            }
            else
            {
                MessageBox.Show("Parsing failed! Unprocessed tokens left.");
            }
        }

        public void DisplayTree()
        {
            Application.Run(new TreeForm(Root));
        }

        private class TreeForm : Form
        {
            public TreeForm(TreeNode root)
            {
                var treeView = new TreeView { Dock = DockStyle.Fill, Nodes = { root } };
                Controls.Add(treeView);
            }
        }
    }
}