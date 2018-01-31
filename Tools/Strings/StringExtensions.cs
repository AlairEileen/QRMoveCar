using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tools.Strings
{
    public static class StringExtensions
    {
        //public static string GetLetterFirst(this string text)
        //{
        //    if (string.IsNullOrEmpty(text))
        //    {
        //        return null;
        //    }
        //    string str = "#";
        //    char firstChar = text.ToArray()[0];
        //    if ((firstChar >= 'a' && firstChar <= 'z') || (firstChar >= 'A' && firstChar <= 'Z'))///首字母如果是字母获取到字母直接返回
        //    {
        //        str = firstChar.ToString().ToUpper();
        //    }
        //    else if (Char.IsNumber(firstChar))  ///首字母如果是数字获取到数字直接返回
        //    {
        //        str = firstChar.ToString();
        //    }
        //    else
        //    {
        //        string[][] simpleString = StringTool.GetCNPinyinSimple(text);
        //        if (simpleString != null && simpleString.Count() > 0&&simpleString[0]!=null&&simpleString[0].Count()>0)
        //        {
        //            str = simpleString[0][0].Substring(0, 1);
        //        }
        //        else
        //        {
        //            str = firstChar.ToString();
        //        }
        //    }
        //    return str;
        //}

    }
}
