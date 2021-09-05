using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;

namespace lab_1
{
    public class KeyValueComparer : IComparer<KeyValuePair<char, int>>
    {
        public int Compare(KeyValuePair<char, int> x, KeyValuePair<char, int> y)
        {
            return -x.Value.CompareTo(y.Value);
        }
    }
    public static class MonoAlphabetSolver
    {
        public static KeyValuePair<char, int>[] GetUniqueCharacters(string text)
        {
            text = text.Replace(" ", String.Empty);
            var uniqueChars = new Dictionary<char, int>();

            foreach (var character in text)
                if (!uniqueChars.ContainsKey(character)) 
                    uniqueChars.Add(character, 1);
                else
                    uniqueChars[character]++;
            
            var arr = uniqueChars.ToArray();
            
            Array.Sort(arr, new KeyValueComparer());

            return arr;
        }
    }
}