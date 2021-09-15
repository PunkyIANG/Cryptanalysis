using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace lab_1
{
    public static class PolyAlphabetSolver
    {
        public static List<int> GetKeyLength(string text)
        {
            text = text.Replace(" ", String.Empty);
            var ngramDict = new Dictionary<string, List<int>>();

            for (int length = 3; length <= 4; length++)
                for (int i = 0; i < text.Length - length + 1; i++)
                {
                    var ngram = text.Substring(i, length);

                    if (ngramDict.Keys.Contains(ngram))
                        ngramDict[ngram].Add(i);
                    else
                        ngramDict.Add(ngram, new List<int> {i});
                }

            var arr = ngramDict.ToArray();

            Array.Sort(arr, (x, y)
                => y.Value.Count - x.Value.Count);

            var distances = new List<int>();
            int minDistance = 0;
            
            
            // TODO: get this data for more ngrams, rather than just the first
            // TODO: render this on a 2d table (https://github.com/AntonC9018/uni_cryptanalysis/blob/master/doc/Cryptanalysis_1.md#metoda-kasiski-ex4-a)
            
            for (int i = 0; i < arr[0].Value.Count; i++)
                for (int j = i + 1; j < arr[0].Value.Count; j++)
                {
                    int tempDist = arr[0].Value[j] - arr[0].Value[i];

                    distances.Add(tempDist);

                    if (minDistance == 0 || minDistance > tempDist)
                        minDistance = tempDist;
                }

            var divisors = new List<int>();

            for (int i = 2; i <= minDistance; i++)
            {
                bool isDivisor = true;

                foreach (var distance in distances)
                {
                    if (distance % i != 0)
                    {
                        isDivisor = false;
                        break;
                    }
                }

                if (isDivisor)
                    divisors.Add(i);
            }

            // foreach (var divisor in divisors) 
            //     Console.WriteLine(divisor);

            return divisors;
        }

        public static List<string> SplitToMonoAlphabet(string text, int keyLength)
        {
            var results = new List<string>();
            
            for (int i = 0; i < keyLength; i++)
            {
                var stringBuilder = new StringBuilder();

                for (int j = i; j < text.Length; j += keyLength)
                    stringBuilder.Append(text[j]);
                
                results.Add(stringBuilder.ToString());
            }

            // foreach (var result in results)
            //     Console.WriteLine(result);

            return results;
        }
    }
}