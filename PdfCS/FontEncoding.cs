using System;
using System.Collections.Generic;
using System.Linq;

namespace PdfCS
{
    /// <summary>
    /// Класс кодировки символов шрифта
    /// </summary>
    public class FontEncoding
    {
        /// <summary>
        /// Имя базовой кодировки
        /// </summary>
        public string baseEncoding;

        /// <summary>
        /// Массив соответствия кодов именам символов
        /// </summary>
        public string[] encoding;

        /// <summary>
        /// Словарь кодировок с соответствующими индексами в standartCodes.
        /// </summary>
        private Dictionary<string, int> namesEncodings = new Dictionary<string, int>() {
            { "StandardEncoding", 0},
            { "MacRomanEncoding", 1},
            { "WinAnsiEncoding", 2},
            { "PDFDocEncoding", 3 }
        };

        private static Dictionary<string, int[]> standartCodes = new Dictionary<string, int[]>
        {
            {"A", new int[]{ 0101, 0101, 0101, 0101} },
            {"Agrave", new int[]{ -1, 0313, 0300, 0300 } },
            {"AE", new int[]{0341, 0256, 0306, 0306 }},
            {"Aacute", new int[]{ -1, 0347, 0301, 0301 }},
            {"Acircumflex", new int[]{ -1, 0345, 0302, 0302 }},
            {"Adieresis", new int[]{ -1, 0200, 0304, 0304 }},
            {"Aring", new int[]{ -1, 0201, 0305, 0305 }},
            {"Atilde", new int[]{ -1, 0314, 0303, 0303 }},
            {"B", new int[]{ 0102, 0102, 0102, 0102 }},
            {"C", new int[]{ 0103, 0103, 0103, 0103 }},
            {"Ccedilla", new int[]{ -1, 0202, 0307, 0307 }},
            {"D", new int[]{ 0104, 0104, 0104, 0104 }},
            {"E", new int[]{ 0105, 0105, 0105, 0105 }},
            {"Eacute", new int[]{ -1, 0203, 0311, 0311 }},
            {"Ecircumflex", new int[]{ -1, 0346, 0312, 0312 }},
            {"Edieresis", new int[]{ -1, 0350, 0313, 0313 }},
            {"Egrave", new int[]{ -1, 0351, 0310, 0310 }},
            {"Eth", new int[]{ -1, -1, 0320, 0320 }},
            {"Euro", new int[]{ -1, -1, 0200, 0240 }},
            {"F", new int[]{ 0106, 0106, 0106, 0106 }},
            {"G", new int[]{ 0106, 0106, 0106, 0106 }},
            {"H", new int[]{ 0110, 0110, 0110, 0110 }},
            {"I", new int[]{ 0111, 0111, 0111, 0111 }},
            {"Iacute", new int[]{ -1, 0352, 0315, 0315 }},
            {"Icircumflex", new int[]{ -1, 0353, 0316, 0316 }},
            {"Idieresis", new int[]{ -1, 0354, 0317, 0317 }},
            {"Igrave", new int[]{ -1, 0355, 0314, 0314 }},
            {"J", new int[]{ 0112, 0112, 0112, 0112 }},
            {"K", new int[]{ 0113, 0113, 0113, 0113 } },
            {"L", new int[]{ 0114, 0114, 0114, 0114 } },
            {"Lslash", new int[]{ 0350, -1, -1, 0225 }},
            {"M", new int[]{ 0115, 0115, 0115, 0115 }},
            {"N", new int[]{ 0116, 0116, 0116, 0116 }},
            {"Ntilde", new int[]{ -1, 0204, 0321, 0321 }},
            {"O", new int[]{ 0117, 0117, 0117, 0117 }},
            {"OE", new int[]{ 0352, 0316, 0214, 0226 }},
            {"Oacute", new int[]{ -1, 0356, 0323, 0323 }},
            {"Ocircumflex", new int[]{ -1, 0357, 0324, 0324 }},
            {"Odieresis", new int[]{ -1, 0205, 0326, 0326 }},
            {"Ograve", new int[]{ 0117, 0361, 0322, 0322 }},
            {"Oslash", new int[]{ 0351, 0257, 0330, 0330 }},
            {"Otilde", new int[]{ -1, 0315, 0325, 0325 }},
            {"P", new int[]{ 0120, 0120, 0120, 0120 }},
            {"Q", new int[]{ 0121, 0121, 0121, 0121 }},
            {"R", new int[]{ 0122, 0122, 0122, 0122 }},
            {"S", new int[]{ 0123, 0123, 0123, 0123 }},
            {"Scaron", new int[]{ -1, -1, 0212, 0227 }},
            {"T", new int[]{ 0124, 0124, 0124, 0124 }},
            {"Thorn", new int[]{ -1, -1, 0336, 0336 }},
            {"U", new int[]{ 0125, 0125, 0125, 0125 }},
            {"Uacute", new int[]{ -1, 0362, 0332, 0332 }},
            {"Ucircumflex", new int[]{ -1, 0363, 0333, 0333 }},
            {"Udieresis", new int[]{ -1, 0206, 0334, 0334 }},
            {"Ugrave", new int[]{ -1, 0364, 0331, 0331 }},
            {"V", new int[]{ 0126, 0126, 0126, 0126 }},
            {"W", new int[]{ 0127, 0127, 0127, 0127 }},
            {"X", new int[]{ 0130, 0130, 0130, 0130 }},
            {"Y", new int[]{ 0131, 0131, 0131, 0131 }},
            {"Yacute", new int[]{ -1, -1, 0335, 0335 }},
            {"Ydieresis", new int[]{ -1, 0331, 0237, 0230 }},
            {"Z", new int[]{ 0132, 0132, 0132, 0132 }},
            {"Zcaron", new int[]{ -1, -1, 0216, 0231 }},
            {"a", new int[]{ 0141, 0141, 0141, 0141 }},
            {"aacute", new int[]{ -1, 0207, 0341, 0341 }},
            {"acircumflex", new int[]{ -1, 0211, 0342, 0342 }},
            {"acute", new int[]{ 0302, 0253, 0264, 0264 }},
            {"adieresis", new int[]{ -1, 0212, 0344, 0344 }},
            {"ae", new int[]{ 0361, 0276, 0346, 0346 }},
            {"agrave", new int[]{ -1, 0210, 0340, 0340 }},
            {"ampersand", new int[]{ 0046, 0046, 0046, 0046 }},

            {"aring", new int[]{ -1, 0314, 0345, 0345} },
            {"asciicircum", new int[]{ 0136, 0136, 0136, 0136 } },
            {"asciitilde", new int[]{ 0176, 0176, 0176, 0176 }},
            {"asterisk", new int[]{ 0052, 0052, 0052, 0052 }},
            {"at", new int[]{ 0100, 0100, 0100, 0100 }},
            {"atilde", new int[]{ -1, 0213, 0343, 0343 }},
            {"b", new int[]{ 0142, 0142, 0142, 0142 }},
            {"backslash", new int[]{ 0134, 0134, 0134, 0134 }},
            {"bar", new int[]{ 0174, 0174, 0174, 0174 }},
            {"braceleft", new int[]{ 0173, 0173, 0173, 0173 }},
            {"braceright", new int[]{ 0175, 0175, 0175, 0175 }},
            {"bracketleft", new int[]{ 0133, 0133, 0133, 0133 }},
            {"bracketright", new int[]{ 0135, 0135, 0135, 0135 }},
            {"breve", new int[]{ 0306, 0371, -1, 0030 }},
            {"brokenbar", new int[]{ -1, -1, 0246, 0246 }},
            {"bullet", new int[]{ 0267, 0245, 0225, 0200 }},
            {"c", new int[]{ 0143, 0143, 0143, 0143 }},
            {"caron", new int[]{ 0317, 0377, -1, 0031 }},
            {"ccedilla", new int[]{ -1, 0215, 0347, 0347 }},
            {"cedilla", new int[]{ 0313, 0374, 0270, 0270 }},
            {"cent", new int[]{ 0242, 0242, 0242, 0242 }},
            {"circumflex", new int[]{ 0303, 0366, 0210, 0032 }},
            {"colon", new int[]{ 0072, 0072, 0072, 0072 }},
            {"comma", new int[]{ 0054, 0054, 0054, 0054 }},
            {"copyright", new int[]{ -1, 0251, 0251, 0251 }},
            {"currency", new int[]{ 0250, 0333, 0244, 0244 }},
            {"d", new int[]{ 0144, 0144, 0144, 0144 }},
            {"dagger", new int[]{ 0262, 0240, 0206, 0201 }},
            {"daggerdbl", new int[]{ 0263, 0340, 0207, 0202 } },
            {"degree", new int[]{ -1, 0241, 0260, 0260 } },
            {"dieresis", new int[]{ 0310, 0254, 0250, 0250 }},
            {"divide", new int[]{ -1, 0326, 0367, 0367 }},
            {"dollar", new int[]{ 0044, 0044, 0044, 0044 }},
            {"dotaccent", new int[]{ 0307, 0372, -1, 0033 }},
            {"dotlessi", new int[]{ 0365, 0365, -1, 0232 }},
            {"e", new int[]{ 0145, 0145, 0145, 0145 }},
            {"eacute", new int[]{ -1, 0216, 0351, 0351 }},
            {"ecircumflex", new int[]{ -1, 0220, 0352, 0352 }},
            {"edieresis", new int[]{ -1, 0221, 0353, 0353 }},
            {"egrave", new int[]{ -1, 0217, 0350, 0350 }},
            {"eight", new int[]{ 0070, 0070, 0070, 0070 }},
            {"ellipsis", new int[]{ 0274, 0311, 0205, 0203 }},
            {"emdash", new int[]{ 0320, 0321, 0227, 0204 }},
            {"endash", new int[]{ 0261, 0320, 0226, 0205 }},
            {"equal", new int[]{ 0075, 0075, 0075, 0075 }},
            {"eth", new int[]{ -1, -1, 0360, 0360 }},
            {"exclam", new int[]{ 0041, 0041, 0041, 0041 }},
            {"exclamdown", new int[]{ 0241, 0301, 0241, 0241 }},
            {"f", new int[]{ 0146, 0146, 0146, 0146 }},
            {"fi", new int[]{ 0256, 0336, -1, 0223 }},
            {"five", new int[]{ 0065, 0065, 0065, 0065 }},
            {"fl", new int[]{ 0257, 0337, -1, 0234 }},
            {"florin", new int[]{ 0246, 0304, 0203, 0206 }},
            {"four", new int[]{ 0064, 0064, 0064, 0064 }},
            {"fraction", new int[]{ 0244, 0332, -1, 0207 }},
            {"g", new int[]{ 0147, 0147, 0147, 0147 }},
            {"germandbls", new int[]{ 0373, 0247, 0337, 0337 }},
            {"grave", new int[]{ 0301, 0140, 0140, 0140 }},
            {"greater", new int[]{ 0076, 0076, 0076, 0076 }},
            {"guillemotleft", new int[]{ 0253, 0307, 0253, 0253 }},
            {"guillemotright", new int[]{ 0273, 0310, 0273, 0273 }},
            {"guilsinglleft", new int[]{ 0254, 0334, 0213, 0210 }},
            {"guilsinglright", new int[]{ 0255, 0335, 0233, 0211 }},
            {"h", new int[]{ 0150, 0150, 0150, 0150 }},
            {"hungarumlaut", new int[]{ 0315, 0375, -1, 0034 }},
            {"hyphen", new int[]{ 0055, 0055, 0055, 0055 }},
            {"i", new int[]{ 0151, 0151, 0151, 0151 }},
            {"iacute", new int[]{ -1, 0222, 0355, 0355 }},
            {"icircumflex", new int[]{ -1, 0224, 0356, 0356 }},
            {"idieresis", new int[]{ -1, 0225, 0357, 0357 }},
            {"igrave", new int[]{ -1, 0223, 0354, 0354 }},
            {"j", new int[]{ 0152, 0152, 0152, 0152 }},
            {"k", new int[]{ 0153, 0153, 0153, 0153 }},
            {"l", new int[]{ 0154, 00154, 0154, 0154 }},

            {"less", new int[]{ 0074, 0074, 0074, 0074} },
            {"logicalnot", new int[]{ -1, 0302, 0254, 0254 } },
            {"lslash", new int[]{ 0370, -1, -1, 0233 }},
            {"m", new int[]{ 0155, 0155, 0155, 0155 }},
            {"macron", new int[]{ 0305, 0370, 0257, 0257 }},
            {"minus", new int[]{ -1, -1, -1, 0212 }},
            {"mu", new int[]{ -1, 0265, 0265, 0265 }},
            {"multiply", new int[]{ -1, -1, 0327, 0327 }},
            {"n", new int[]{ 0156, 0156, 0156, 0156 }},
            {"nine", new int[]{ 0071, 0071, 0071, 0071 }},
            {"ntilde", new int[]{ -1, 0226, 0361, 0361 }},
            {"numbersign", new int[]{ 0043, 0043, 0043, 0043 }},
            {"o", new int[]{ 0157, 0157, 0157, 0157 }},
            {"oacute", new int[]{ -1, 0227, 0363, 0363 }},
            {"ocircumflex", new int[]{ -1, 0231, 0364, 0364 }},
            {"odieresis", new int[]{ -1, 0232, 0366, 0366 }},
            {"oe", new int[]{ 0372, 0317, 0234, 0234 }},
            {"ogonek", new int[]{ 0316, 0376, -1, 0035 }},
            {"ograve", new int[]{ -1, 0230, 0362, 0362 }},
            {"one", new int[]{ 0061, 0061, 0061, 0061 }},
            {"onehalf", new int[]{ -1, -1, 0275, 0275 }},
            {"onequarter", new int[]{ -1, -1, 0274, 0274 }},
            {"onesuperior", new int[]{ -1, -1, 0271, 0271 }},
            {"ordfeminine", new int[]{ 0343, 0273, 0252, 0252 }},
            {"ordmasculine", new int[]{ 0353, 0274, 0272, 0272 }},
            {"oslash", new int[]{ 0371, 0277, 0370, 0370 }},
            {"otilde", new int[]{ -1, 0233, 0365, 0365 }},
            {"p", new int[]{ 0160, 0160, 0160, 0160 }},
            {"paragraph", new int[]{ 0266, 0246, 0266, 0266 } },
            {"parenleft", new int[]{ 0050, 0050, 0050, 0050 } },
            {"parenright", new int[]{ 0051, 0051, 0051, 0051 }},
            {"percent", new int[]{ 0045 , 0045, 0045, 0045 }},
            {"period", new int[]{ 0056, 0056, 0056, 0056 }},
            {"periodcentered", new int[]{ 0264, 0341, 0267, 0267 }},
            {"perthousand", new int[]{ 0275, 0344, 0211, 0213 }},
            {"plus", new int[]{ 0053, 0053, 0053, 0053 }},
            {"plusminus", new int[]{ -1, 0261, 0261, 0261 }},
            {"q", new int[]{ 0161, 0161, 0161, 0161 }},
            {"question", new int[]{ 0077, 0077, 0077, 0077 }},
            {"questiondown", new int[]{ 0277, 0300 , 0277, 0277  }},
            {"quotedbl", new int[]{ 0042, 0042, 0042, 0042 }},
            {"quotedblbase", new int[]{ 0271, 0343, 0204, 0214 }},
            {"quotedblleft", new int[]{ 0252, 0322, 0223, 0215 }},
            {"quotedblright", new int[]{ 0272, 0323, 0224, 0216 }},
            {"quoteleft", new int[]{ 0140, 0324, 0221, 0217 }},
            {"quoteright", new int[]{ 0047, 0325, 0222, 0220 }},
            {"quotesinglbase", new int[]{ 0270, 0342, 0202, 0221 }},
            {"guotesingle", new int[]{ 0251, 0047, 0047, 0047 }},
            {"r", new int[]{ 0162, 0162, 0162, 0162 }},
            {"registered", new int[]{ -1, 0250, 0256, 0256 }},
            {"ring", new int[]{ 0312, 0373, -1, 0036 }},
            {"s", new int[]{ 0163, 0163, 0163, 0163 }},
            {"scaron", new int[]{ -1, -1, 0232, 0235 }},
            {"section", new int[]{ 0247, 0244, 0247, 0247 }},
            {"semicolon", new int[]{ 0073, 0073, 0073, 0073 }},
            {"seven", new int[]{ 0067, 0067, 0067, 0067 }},
            {"six", new int[]{ 0066, 0066, 0066, 0066 }},
            {"slash", new int[]{ 0057, 0057, 0057, 0057 }},
            {"space", new int[]{ 0040, 0040, 0040, 0040 }},
            {"sterling", new int[]{ 0243, 0243, 0243, 0243 }},
            {"t", new int[]{ 0164, 0164, 0164, 0164 }},
            {"thorn", new int[]{ -1, -1, 0376, 0376 }},
            {"three", new int[]{ 0063, 0063, 0063, 0063 }},
            {"threequarters", new int[]{ -1, -1, 0276, 0276 }},
            {"threesuperior", new int[]{ -1, -1, 0263, 0263 }},
            {"tilde", new int[]{ 0304, 0367, 0230, 0037 }},
            {"trademark", new int[]{ -1, 0252, 0231, 0222 }},
            {"two", new int[]{ 0062, 0062, 0062, 0062 }},
            {"twosuperior", new int[]{ -1, -1, 0262, 0262 }},
            {"u", new int[]{ 0165, 0165, 0165, 0165 }},
            {"uacute", new int[]{ -1, 0234, 0372, 0372 }},
            {"ucircumflex", new int[]{ -1, 0236, 0373, 0373 }},
            {"udieresis", new int[]{ -1, 0237, 0374, 0374 }},
            {"ugrave", new int[]{ -1, 0235, 0371, 0371 }},

            {"underscore", new int[]{ 0137, 0137, 0137, 0137 }},
            {"v", new int[]{ 0166, 0166, 0166, 0166 }},
            {"w", new int[]{ 0167, 0167, 0167, 0167 }},
            {"x", new int[]{ 0170, 0170, 0170, 0170 }},
            {"y", new int[]{ 0171, 0171, 0171, 0171 }},
            {"yacute", new int[]{ -1, -1, 0375, 0375 }},
            {"ydieresis", new int[]{ -1, 0330, 0377, 0377 }},
            {"yen", new int[]{ 0245, 0264, 0245, 0245 }},
            {"z", new int[]{ 0172, 0172, 0172, 0172 }},
            {"zcaron", new int[]{ -1, -1, 0236, 0236 }},
            {"zero", new int[]{ 0060, 0060, 0060, 0060 }},
        };

        /// <summary>
        /// Поля словаря:
        /// Type(необязательное) - должно быть "Encoding"
        /// BaseEncoding(необязательное) - имя базовой кодировки
        /// Differences(необязательное) - массив отличий от базовой кодировки или полный массив кодировки
        /// содержит записи
        /// code1(число) name1(имя) name2...
        /// code2(число) name1(имя) name2...
        /// каждый код - это первый индекс кода для последующих имен, которые должны быть изменены
        /// первое имя соответствует этому коду, последующие увеличиваются на единицу, пока не встретится новый код
        /// последовательности могут быть в любом порядке, но не пересекаются
        /// </summary>
        /// <param name="dic">словарь из поля Encoding у шрифта</param>
        /// <param name="lastChar">наибольший код символа</param>
        public FontEncoding(Dictionary<string, object> dic, int lastChar)
        {
            encoding = new string[lastChar];
            if (dic.ContainsKey("BaseEncoding"))
                baseEncoding = (string)dic["BaseEncoding"];
            CheckNameEncoding(baseEncoding);
            if (dic.ContainsKey("Differences"))
            {
                var differences = (object[])dic["Differences"];
                int startValueIndex = 0;
                for (int i = 0; i < differences.Length; i++)
                {
                    if (differences[i] is int)
                    {
                        startValueIndex = i;
                        continue;
                    }
                    encoding[(int)differences[startValueIndex] + i - startValueIndex - 1] = (string)differences[i];
                }
            }
        }

        /// <summary>
        /// Проверка названия кодировки с передачей соответствующего индекса в FillEncoding.
        /// </summary>
        /// <param name="baseEncoding">Название базовой кодировки</param>
        private void CheckNameEncoding(string baseEncoding)
        {
            foreach (var name in namesEncodings)
                if (name.Key.Equals(baseEncoding))
                {
                    FillEncoding(name.Value);
                    break;
                }
        }

        /// <summary>
        /// Заполнение массива encoding с соответствующей кодировкой.
        /// </summary>
        /// <param name="index">Индекс соответствующей кодировки в standartCodes.</param>
        private void FillEncoding(int index)
        {
            for (int i = 0; i < standartCodes.Count; i++)
                if (standartCodes.ElementAt(i).Value[index] != -1 && standartCodes.ElementAt(i).Value[index] < encoding.Length)
                    encoding[standartCodes.ElementAt(i).Value[index]] = standartCodes.ElementAt(i).Key;
        }
    }
}
