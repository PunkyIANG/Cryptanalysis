using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;

namespace lab_1
{
    public static class MonoAlphabetSolver
    {
        public static string[] charBlacklist = new string[]
        {
            " ",
            "\n",
            ((char) 13).ToString()  // carriage return
        };
        
        public static KeyValuePair<char, int>[] GetUniqueCharacters(string text)
        {
            foreach (var s in charBlacklist) 
                text = text.Replace(s, string.Empty);

            var uniqueChars = new Dictionary<char, int>();

            foreach (var character in text)
                if (!uniqueChars.ContainsKey(character)) 
                    uniqueChars.Add(character, 1);
                else
                    uniqueChars[character]++;
            
            var arr = uniqueChars.ToArray();
            
            Array.Sort(arr, (x, y) 
                => y.Value - x.Value);

            // foreach (var (key, value) in arr) 
            //     Console.WriteLine(key + " " + (int)key);

            return arr;
        }
        
        
    }
}