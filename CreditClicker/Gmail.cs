using System;
using System.Collections.Generic;

//using System.Threading.Tasks;

namespace gmail
{
    internal class Gmail
    {
        public string sUsername;
        public string sPassword;

        public int iNumEmails;
        public int iEmail = 0;

        public string CurWildcardOption = "";

        public Gmail()
        {
            sUsername = "";
            sPassword = "";
            iNumEmails = 0;
        }

        public int ParseStandardFormat(string str, int iStart)
        {
            if (str[iStart - 1] == '"')
            {
                return str.IndexOf("\"", iStart);
            }
            else if (str[iStart - 1] == '\'')
            {
                return str.IndexOf("'", iStart);
            }
            else if (str[iStart - 1] == '>')
            {
                return str.IndexOf("<", iStart);
            }
            else
                return -1;
        }

        public int ParseNonStandardFormat(string str, int iStart)
        {
            int a, b, c = 0;

            List<int> validPositions = new List<int>();

            a = str.IndexOf("-", iStart);
            b = str.IndexOf("\"", iStart);
            c = str.IndexOf("=", iStart);

            if (a != -1)
                validPositions.Add(a);

            if (b != -1)
                validPositions.Add(b);

            if (c != -1)
            {
                // it might be phpid=231280931 instead of the ====== most emailers use for "style"
                if (str[c + 1] == '=')
                    validPositions.Add(c);
            }
            if (validPositions.Count == 0)
                return -1;

            return GetMin(validPositions);
        }

        private int GetMin(List<int> iList)
        {
            int iMin = iList[0];

            for (int i = 1; i < iList.Count; i++)
            {
                if (iList[i] < iMin)
                    iMin = iList[i];
            }

            return iMin;
        }

        private int ParseWhiteLineFormat(string str, int iStart)
        {
            int a = 0;
            int b = 0;

            a = str.IndexOf(" ", iStart);
            b = str.IndexOf(Environment.NewLine, iStart);

            if ((a < b) && (a != -1))
                return a;
            else
                return b;
        }

        private string ParseEmail(string str)
        {
            string sLink = "";

            int iBuffer = 0;
            int iStart = 0;
            int iEnd = 0;

            int a, b = 0;

            List<int> iList = new List<int>();

            foreach (WildCard w in Program.WildCards)
            {
                iStart = str.IndexOf(w.URL);
                if (iStart != -1)
                {
                    if ((iBuffer = ParseStandardFormat(str, iStart)) != -1)
                        iEnd = iBuffer;
                    else
                    {
                        if ((b = ParseStandardFormat(str, iStart)) == -1)
                            iEnd = ParseWhiteLineFormat(str, iStart);
                        else
                        {
                            a = ParseWhiteLineFormat(str, iStart);

                            if (a < b)
                                iEnd = a;
                            else
                                iEnd = b;
                        }
                    }

                    if (iEnd != -1)
                    {
                        sLink = (str.Substring(iStart, (iEnd - iStart)));
                        CurWildcardOption = w.Option;
                        return sLink;
                    }
                    else
                    {
                        return "www.google.com";
                    }
                }
            }
            return "None";
        }

        public bool ReadEmail()
        {
            return true;
        }

        public string CheckEmails(string sFolder, string Message)
        {
            string buffer = "";

            if ((buffer = ParseEmail(Message)) != "None")
                return buffer;
            else
                return "None";
        }
    }
}