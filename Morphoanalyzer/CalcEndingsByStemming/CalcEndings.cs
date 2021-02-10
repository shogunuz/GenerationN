﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Morphoanalyzer.StaticData;
using Morphoanalyzer.CalcEndingsByStemming;

/*
 * Developers: N. Abdurakhmonova, D.Mengliev
 * Year: 2020
 */

namespace Morphoanalyzer.Features
{
    public class CalcEndings
    {
        public Dictionary<string, string> GetResult(string word)
        {
            Dictionary<string, string>[] resultDictionary = new Dictionary<string, string>[2];

            Dictionary<string, string> InnerDict;

            List<int> numbers = new List<int>();

            GetEndingsGeneral[] getEnds = new GetEndingsGeneral[2];
            getEnds[0] = new GetEndingsGeneral(new CalcNounEndings(word));
            getEnds[1] = new GetEndingsGeneral(new CalcAdjEndings(word));

           
            for (int i = 0; i < getEnds.Length; i++)
            {
                InnerDict = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> kvp in getEnds[i].GetEndings())
                {
                    InnerDict.Add(kvp.Key, kvp.Value);
                } 
                resultDictionary[i] = new Dictionary<string, string>(InnerDict);
                numbers.Add(InnerDict.Count);
            }

            // Получаю индекс словаря, который содержит больше всего окончаний.
            int t = numbers.IndexOf(numbers.Max<int>());

            if(resultDictionary[t].Count == 0)
            {
                resultDictionary[t] = new Dictionary<string, string>
                {
                    {$"{word}", StaticString.RootWord}
                };
            }

            //I am checking if the system find out exception word or not
            //if it found, then, it will return found word
            return CalcEndingsGeneral.exceptionWordInt switch
            {
                1 => resultDictionary[0], //1 is Noun
                2 => resultDictionary[1], //2 is Adjective
                _ => resultDictionary[t], // OR it's noun\adj
            };
        }

       
    }
}
