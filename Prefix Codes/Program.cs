//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="FTL">
//     FTL Inc.
// </copyright>
//-----------------------------------------------------------------------

namespace PrefixCodes
{
    using System;

    /// <summary>
    /// Demo class
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method.
        /// </summary>
        /// <param name="args">The arguments.</param>
        public static void Main(string[] args)
        {
            PrefixCode(new char[] { 'a', 'b', 'c', 'd' }, new double[] { 0.125, 0.125, 0.25, 0.5 });
            PrefixCode(new char[] { 'a', 'b', 'c', 'd', 'e' }, new double[] { 0.2, 0.1, 0.3, 0.25, 0.15 });
            PrefixCode(new char[] { 'a', 'b', 'c', 'd', 'e' }, new double[] { 0.38461538, 0.17948718, 0.15384616, 0.15384614, 0.12820513 });
            PrefixCode(new char[] { '5', '6', '1', '7', '2' }, new double[] { 0.3, 0.25, 0.2, 0.15, 0.1 });
            PrefixCode(new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j' }, new double[] { 0.00693, 0.134712, 0.153271, 0.173098, 0.112272, 0.067446, 0.111512, 0.037859, 0.099983, 0.102918 });
        }

        /// <summary>
        /// Prints Shannon's prefix codes for a given set of characters and their frequencies.
        /// </summary>
        /// <param name="characters">The characters.</param>
        /// <param name="frequencies">The frequencies.</param>
        private static void PrefixCode(char[] characters, double[] frequencies)
        {
            Console.WriteLine("Shannon's code");
            ShannonCode shannon = new ShannonCode(characters, frequencies);
            foreach (var c in characters)
            {
                Console.WriteLine("{0} : {1}", c, shannon[c]);
            }

            Console.WriteLine();
            Console.WriteLine("Huffman's code");
            HuffmanCode huffman = new HuffmanCode(characters, frequencies);
            foreach (var c in characters)
            {
                Console.WriteLine("{0} : {1}", c, huffman[c]);
            }

            Console.WriteLine();
        }
    }
}
