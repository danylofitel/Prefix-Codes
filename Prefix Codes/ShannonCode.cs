//-----------------------------------------------------------------------
// <copyright file="ShannonCode.cs" company="FTL">
//     FTL Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PrefixCodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class for building Shannon's prefix codes
    /// </summary>
    public class ShannonCode
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
        /// Initializes a new instance of the <see cref="ShannonCode"/> class and builds Shannon's prefix codes.
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
        public ShannonCode(char[] symbols, double[] frequencies)
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

            // Construct the codes using recursive procedure
            this.BuildCodes(0, this.symbols.Length, 1.0, string.Empty);
        }

        /// <summary>
        /// Gets the <see cref="System.String"/> representation of Shannon's code of specified character.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="symbol">The symbol.</param>
        /// <returns>Shannon's code of the symbol.</returns>
        public string this[char symbol]
        {
            get
            {
                return this.codes[symbol];
            }
        }

        /// <summary>
        /// Traverses the code tree recursively and construct prefix codes at leaf nodes.
        /// </summary>
        /// <param name="left">Inclusive left substring bound.</param>
        /// <param name="right">Exclusive right substring bound.</param>
        /// <param name="freq">Sum of frequencies of characters in current substring.</param>
        /// <param name="path">Code corresponding to the path to current substring.</param>
        private void BuildCodes(int left, int right, double freq, string path)
        {
            if (left + 1 == right)
            {
                // This is a leaf node (a one-character substring), current path is the code of current character
                this.codes[this.symbols[left]] = path;
            }
            else
            {
                // Separation between two substrings with equal frequency sums
                // Start with only the first character on the left side
                int half = left;
                double halfFreq = this.frequencies[left];

                // Continue growth of the left side only if there are more than 2 characters in current substring
                if (right - left > 2)
                {
                    // Find a partition that is as to equal as possible
                    while (half + 1 < right - 1
                        && Math.Abs((0.5 * freq) - halfFreq) > Math.Abs((0.5 * freq) - halfFreq - this.frequencies[half + 1]))
                    {
                        ++half;
                        halfFreq += this.frequencies[half];
                    }
                }

                // Recursively traverse left and right substrings
                this.BuildCodes(left, half + 1, halfFreq, path + "0");
                this.BuildCodes(half + 1, right, freq - halfFreq, path + "1");
            }
        }
    }
}
