using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tools.Strings
{
    public class StringTool
    {
        //public static string[][] GetCNPinyinSimple(string text)
        //{
        //    char[] chars = text.ToArray();
        //    string[][] stringArray = new string[chars.Length][];
        //    for (int i = 0; i < chars.Length; i++)
        //    {

        //        stringArray[i] = Pinyin4net.PinyinHelper.ToHanyuPinyinStringArray(chars[i], new Pinyin4net.Format.HanyuPinyinOutputFormat() { CaseType = Pinyin4net.Format.HanyuPinyinCaseType.UPPERCASE, ToneType = Pinyin4net.Format.HanyuPinyinToneType.WITHOUT_TONE });

        //    }
        //    return stringArray;
        //}

        //public static string IsLetter(string text,out bool isLetter)
        //{
        //    byte[] ZW = System.Text.Encoding.Default.GetBytes(text);
        //    //如果是字母，则直接返回
        //    if (ZW.Length == 1)
        //    {
        //        isLetter = true;
        //        return text.ToUpper();
        //    }
        //    else
        //    {
        //        isLetter = false;
        //    }
        //    return null;
        //}
    }
    public class RandomNumber
    {
        public static object _lock = new object();
        public static int count = 1;

        public string GetRandom1()
        {
            lock (_lock)
            {
                if (count >= 10000)
                {
                    count = 1;
                }
                var number = "P" + DateTime.Now.ToString("yyMMddHHmmss") + count.ToString("0000");
                count++;
                return number;
            }
        }


        public string GetRandom2()
        {
            lock (_lock)
            {
                return "T" + DateTime.Now.Ticks;

            }
        }

        public string GetRandom3()
        {
            lock (_lock)
            {
                Random ran = new Random();
                return "U" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ran.Next(1000, 9999).ToString();
            }
        }
    }
}
