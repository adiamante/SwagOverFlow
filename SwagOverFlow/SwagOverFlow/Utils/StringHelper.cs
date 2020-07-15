using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SwagOverFlow.Utils
{
    public static class StringHelper
    {
        public static string ToCamelCase(string strInput, params char [] delims)
        {
            String strOutput = "";
            List<Char> lstDelims = delims.ToList();

            for (int i = 0; i < strInput.Length; i++)
            {
                if (lstDelims.Contains(strInput[i]))
                {
                    if (i + 1 < strInput.Length)
                    {
                        strOutput += Char.ToUpper(strInput[i + 1]);
                    }
                    i++;
                }
                else
                {
                    strOutput += strInput[i];
                }
            }

            return strOutput;
        }
    }
}
