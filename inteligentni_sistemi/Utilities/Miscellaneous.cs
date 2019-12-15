using System;
using System.Collections.Generic;

namespace etf.dotsandboxes.cl160127d.Utilities
{
    class Miscellaneous
    {
        public static void ShuffleList<T>(List<T> inputList)
        {
            List<T> randomList = new List<T>();

            Random r = new Random((int)DateTime.Now.Ticks);
            int randomIndex = 0;

            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count);
                randomList.Add(inputList[randomIndex]);
                inputList.RemoveAt(randomIndex);
            }

            inputList.AddRange(randomList);
        }

        public static char TranslateAxisToLetter(int coordinate)
        {
            return (char)(coordinate + 65);
        }

        public static int TranslateLetterToAxis(char letter)
        {
            return (int)(letter - 65);
        }

    }
}
