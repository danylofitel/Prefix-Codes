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
        /// The cumulative frequencies used by binary search during encoding
        /// </summary>
        private double[] cumulativeFrequencies;

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
            this.cumulativeFrequencies = new double[frequencies.Length];

            Array.Copy(symbols, this.symbols, symbols.Length);
            Array.Copy(frequencies, this.frequencies, frequencies.Length);

            // Sort characters by their frequencies in ascending order
            Array.Sort(this.frequencies, this.symbols);

            // Fill array of cumulative frequencies
            this.cumulativeFrequencies[0] = this.frequencies[0];
            for (int i = 1; i < frequencies.Length; ++i)
            {
                this.cumulativeFrequencies[i] = this.cumulativeFrequencies[i - 1] + this.frequencies[i];
            }

            this.codes = new Dictionary<char, string>();

            // Construct the codes using recursive procedure
            this.BuildCodes(0, this.symbols.Length, string.Empty);
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
        /// <param name="path">Code corresponding to the path to current substring.</param>
        private void BuildCodes(int left, int right, string path)
        {
            if (left + 1 == right)
            {
                // This is a leaf node (a one-character substring), current path is the code of current character
                this.codes[this.symbols[left]] = path;
            }
            else
            {
                // Sum of frequencies of symbols before the leftmost one, i.e. extra weight
                double extraFreq = this.cumulativeFrequencies[left] - this.frequencies[left];

                // Exactly half of frequency sum of elements in current substring
                double halfFreq = 0.5 * (this.cumulativeFrequencies[right - 1] - extraFreq);

                // Find the median using binary search
                int l = left;
                int r = right - 1;
                while (l != r)
                {
                    int mid = (l + r) / 2;

                    // Find the direction with better approximation of median and cut off the other direction
                    if (mid + 1 <= r && Math.Abs(halfFreq - this.cumulativeFrequencies[mid] + extraFreq) > Math.Abs(halfFreq - this.cumulativeFrequencies[mid + 1] + extraFreq))
                    {
                        l = mid + 1;
                    }
                    else
                    {
                        r = r == mid ? mid - 1 : mid;
                    }
                }

                // Recursively traverse left and right substrings
                this.BuildCodes(left, l + 1, path + "0");
                this.BuildCodes(l + 1, right, path + "1");
            }
        }
    }
}