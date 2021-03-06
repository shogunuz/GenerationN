﻿
using System;
using System.Collections.Generic;
using System.Linq;
using Morphoanalyzer.Exceptions;
using Morphoanalyzer.Features;
using Morphoanalyzer.StaticData;


namespace Morphoanalyzer.CalcEndingsByStemming
{
    public class CalcEndings
    {
        private Dictionary<string, Dictionary<string, string>> KhorezmDict;
        protected Dictionary<string, string> KhorezmDictForReturn { get; set; }
        public CalcEndings()
        { 
            KhorezmDict = new Dictionary<string, Dictionary<string, 
                string>>(new KhorezmUzbekWords().Dict);
        }
        private string MainWord { get; set; }
        public Dictionary<string, string> GetResult(string word)
        {
            int last_index = word.Length;
            List<int> numberOfElementsInDict = new List<int>();

            ///Вычисляем по KhorezmDict слово
            string[] listOfKhorezmKeys = KhorezmDict.Keys.ToArray();
            Array.Sort(listOfKhorezmKeys);
            int key2 = Array.BinarySearch(listOfKhorezmKeys, word);
            for (int i = 0; i < listOfKhorezmKeys.Length; i++)
            {
                int t = 0;
                t = StringDistance.GetDamerauLevenshteinDistance(listOfKhorezmKeys[i], word);
                if (t <= 2)
                {
                    this.KhorezmDictForReturn = KhorezmDict[listOfKhorezmKeys[i]];
                    this.KhorezmDictForReturn.Add("Perhaps, you meant this Khorezm word: ", listOfKhorezmKeys[i]);
                    return KhorezmDictForReturn;
                }
            }

           // Dictionary<string, string>[] InnerDict =
              //  new Dictionary<string, string>[last_index];
            Dictionary<string, string> InnerDict =
                new Dictionary<string, string>();
            int k = 0;
            /*
            for(int i = last_index-1; i >= 0; i--)
            {
                this.MainWord = word.Remove(i, k);
            */
                InnerDict = new Dictionary<string, string>(GetResultPrivate(word));
                
            //}
            
            //Возвращаем результат если словарь пуст
            //InnerDict["Message"] = StaticString.NotFoundedEng;

            return InnerDict;
        }
        private Dictionary<string, string> CalcBiggestDict(
            Dictionary<string, string>[] resultDictionary,
            List<int> numberOfElementsInDict,
            string word)
        {
            // Получаю индекс словаря, который содержит больше всего окончаний.
            int t = numberOfElementsInDict.IndexOf(numberOfElementsInDict.Max<int>());


            //Проверяю, есть ли вообще хоть что-то в словаре, если словарь пуст,
            //то заполняю просто дефолтным значением
            if (resultDictionary[t].Count == 0)
            {
                resultDictionary[t] = new Dictionary<string, string>
                {
                    {$"{word}", StaticString.RootWord}
                };
            }
            return resultDictionary[t];
        }
        private Dictionary<string, string> GetResultPrivate(string word)
        {

            const int N = 5;

            Dictionary<string, string>[] resultDictionary = 
                new Dictionary<string, string>[N-1];

            Dictionary<string, string> InnerDict;
            CalcEndingsGeneral.exceptionWordInt = 0;
            List<int> numberOfElementsInDict = new List<int>();
            GetEndingsParent[] getEnds = new GetEndingsParent[N-1];
            getEnds[0] = new GetEndingsParent(new CalcNounEndings(word));
            getEnds[1] = new GetEndingsParent(new CalcAdjEndings(word));
            getEnds[2] = new GetEndingsParent(new CalcVerbEndings(word));
            getEnds[3] = new GetEndingsParent(new CalcAdvEndings(word));


            for (int i = 0; i < getEnds.Length; i++)
            {
                InnerDict = new Dictionary<string, string>();
                foreach (KeyValuePair<string, string> kvp in getEnds[i].GetEndings())
                {
                    InnerDict.Add(kvp.Key, kvp.Value);
                } 
                resultDictionary[i] = new Dictionary<string, string>(InnerDict);
                numberOfElementsInDict.Add(InnerDict.Count);
            }
            if (CalcEndingsGeneral.exceptionWordInt == 3)
            {
                CalcEndingsGeneral.exceptionWordInt = 0;
                return resultDictionary[2];
            }
            return CalcBiggestDict(resultDictionary,numberOfElementsInDict,word);
        }

       
    }
}
