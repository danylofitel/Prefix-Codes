//-----------------------------------------------------------------------
// <copyright file="HuffmanCode.cs" company="FTL">
//     FTL Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PrefixCodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class for building Huffman's prefix codes
    /// </summary>
    public class HuffmanCode
    {
        /// <summary>
        /// The symbols sorted by their frequencies
        /// </summary>
        private char[] symbols;

        /// <summary>
        /// The frequencies of symbols sorted by ascending order
        /// </summary>
        private double[] frequencies;

        /// <summary>
        /// The table of codes for each character
        /// </summary>
        private IDictionary<char, string> codes;

        /// <summary>
        /// Initializes a new instance of the <see cref="HuffmanCode"/> class and builds Huffman's prefix codes.
        /// </summary>
        /// <param name="symbols">Array of symbols.</param>
        /// <param name="frequencies">Array of frequencies of corresponding symbols.</param>
        /// <exception cref="System.ArgumentException">
        /// Array sizes do not match.
        /// or
        /// Not all characters are distinct.
        /// or
        /// Negative frequencies.
        /// or
        /// Sum of frequencies is not equal to 1.
        /// </exception>
        public HuffmanCode(char[] symbols, double[] frequencies)
        {
            if (symbols.Length != frequencies.Length)
            {
                throw new ArgumentException("Array sizes do not match.");
            }

            if (symbols.Distinct().Count() != symbols.Length)
            {
                throw new ArgumentException("Not all characters are distinct.");
            }

            if (frequencies.Any(i => i < 0))
            {
                throw new ArgumentException("Negative frequencies.");
            }

            if (Math.Abs(frequencies.Sum() - 1.0) > 0.00001)
            {
                throw new ArgumentException("Sum of frequencies is not equal to 1.");
            }

            // Deep copies are required
            this.symbols = new char[symbols.Length];
            this.frequencies = new double[frequencies.Length];

            Array.Copy(symbols, this.symbols, symbols.Length);
            Array.Copy(frequencies, this.frequencies, frequencies.Length);

            // Sort characters by their frequencies in ascending order
            Array.Sort(this.frequencies, this.symbols);

            this.codes = new Dictionary<char, string>();

            // Construct the prefix code tree using Huffman's algorithm and traverse it to build prefi codes
            if (this.symbols.Length > 0)
            {
                this.BuildCodes(this.BuildPrefixCodeTree(), string.Empty);
            }
        }

        /// <summary>
        /// Gets the <see cref="System.String"/> representation of Huffman's code of specified character.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="symbol">The symbol.</param>
        /// <returns>Huffman's code of the symbol.</returns>
        public string this[char symbol]
        {
            get
            {
                return this.codes[symbol];
            }
        }

        /// <summary>
        /// Builds the prefix code tree.
        /// </summary>
        /// <returns>An optimal prefix code tree for given characters and their frequencies.</returns>
        private PrefixCodeTree BuildPrefixCodeTree()
        {
            // A MinPriorityQueue is used for selecting two nodes with minimal weights at a time
            MinPriorityQueue<PrefixCodeTree> pq = new MinPriorityQueue<PrefixCodeTree>(this.symbols.Length);

            // Add all leaf nodes to the priority queue separately
            for (int i = 0; i < this.symbols.Length; ++i)
            {
                PrefixCodeTree node = new PrefixCodeTree();
                node.Symbol = this.symbols[i];
                node.Weight = this.frequencies[i];
                pq.Enqueue(node);
            }

            // Select two nodes with minimal weights and join them under a new node until a root tree is built
            while (pq.Size > 1)
            {
                PrefixCodeTree first = pq.Dequeue();
                PrefixCodeTree second = pq.Dequeue();
                PrefixCodeTree newNode = new PrefixCodeTree();
                newNode.Symbol = null;
                newNode.Weight = first.Weight + second.Weight;
                newNode.Left = first;
                newNode.Right = second;
                pq.Enqueue(newNode);
            }

            // The last node in the queue is the root of the prefix code tree
            return pq.Dequeue();
        }

        /// <summary>
        /// Traverses the code tree recursively and construct prefix codes at leaf nodes.
        /// </summary>
        /// <param name="tree">Prefix code tree.</param>
        /// <param name="path">Path to the current node from the tree root.</param>
        private void BuildCodes(PrefixCodeTree tree, string path)
        {
            if (tree.Symbol != null)
            {
                // This is a leaf node, a path has been found
                this.codes[tree.Symbol.Value] = path;
            }
            else
            {
                // Traverse left and right children with updated paths
                this.BuildCodes(tree.Left, path + "0");
                this.BuildCodes(tree.Right, path + "1");
            }
        }

        /// <summary>
        /// A prefix code tree representation
        /// </summary>
        private class PrefixCodeTree : IComparable<PrefixCodeTree>
        {
            /// <summary>
            /// Gets or sets the symbol corresponding to the leaf node.
            /// </summary>
            /// <value>
            /// The symbol or null if the node is not terminal.
            /// </value>
            public char? Symbol { get; set; }

            /// <summary>
            /// Gets or sets the weight of the tree.
            /// </summary>
            /// <value>
            /// The weight.
            /// </value>
            public double Weight { get; set; }

            /// <summary>
            /// Gets or sets the left child node.
            /// </summary>
            /// <value>
            /// The left child node.
            /// </value>
            public PrefixCodeTree Left { get; set; }

            /// <summary>
            /// Gets or sets the right child node.
            /// </summary>
            /// <value>
            /// The right child node.
            /// </value>
            public PrefixCodeTree Right { get; set; }

            /// <summary>
            /// Compares the current object with another object of the same type by their weight.
            /// </summary>
            /// <param name="other">An object to compare with this object.</param>
            /// <returns>
            /// A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.
            /// </returns>
            public int CompareTo(PrefixCodeTree other)
            {
                return this.Weight < other.Weight ? -1 : this.Weight == other.Weight ? 0 : 1;
            }
}
    }
}
