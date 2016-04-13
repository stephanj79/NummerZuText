using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;


    internal static class ErweiterungsMethoden
    {
        public static string NummerZuText(this double num, bool mitZeichen = true, bool upperCase=false)
        {
            var i = num.ToString(CultureInfo.InvariantCulture);

            return Vorbereiten(i, mitZeichen, upperCase);
        }

        public static string NummerZuText(this string num, bool mitZeichen = true, bool upperCase = false)
        {
            return Vorbereiten(num, mitZeichen, upperCase);
        }

        private static string Vorbereiten(string s, bool mitZeichen = true, bool upperCase = false)
        {
            var regex = new Regex(@"(\d+)([,\.]{0,1})(\d{0,2})");
            var match = regex.Match(s);
            if (!match.Success) return !match.Success ? " FEHLER " : match.Value;

            var vorne = Convert.ToInt64(match.Groups[1].Value);
            var zeichen = match.Groups[2].Value;
            var hinten = match.Groups[3].Value;

            var wortevorne = vorne.ToText();
            var wortehinten = hinten.ToText(true);

            var returnString = wortevorne + (mitZeichen ? (zeichen == "," ? " Komma " : (wortehinten == "" ? "" : " Punkt ")) : " - ") + (wortehinten != "" ? wortehinten : "");

            return (!upperCase ? returnString : returnString.ToUpper());
        }
    }

    internal static class NumberTextExtensionMethod
    {
        public static string ToText(this long num, bool nachkomma = false)
        {
            var numberText = new NumberText();
            return numberText.ToText(num, nachkomma);
        }

        public static string ToText(this string num, bool nachkomma = false)
        {
            var numberText = new NumberText();
            return numberText.ToText(num, nachkomma);
        }
    }

internal class NumberText
{
    private readonly Dictionary<long, string> _textStrings = new Dictionary<long, string>();
    private readonly Dictionary<long, string> _einheitenStrings = new Dictionary<long, string>();
    private StringBuilder _builderString;

    public long Einheit { get; set; }

    public NumberText()
    {
        #region init

        #region Text Strings

        _textStrings.Add(0, "Null");
        _textStrings.Add(1, "Eins");
        _textStrings.Add(2, "Zwei");
        _textStrings.Add(3, "Drei");
        _textStrings.Add(4, "Vier");
        _textStrings.Add(5, "Fünf");
        _textStrings.Add(6, "Sechs");
        _textStrings.Add(7, "Sieben");
        _textStrings.Add(8, "Acht");
        _textStrings.Add(9, "Neun");
        _textStrings.Add(10, "Zehn");
        _textStrings.Add(11, "Elf");
        _textStrings.Add(12, "Zwölf");
        _textStrings.Add(13, "Dreizehn");
        _textStrings.Add(14, "Vierzehn");
        _textStrings.Add(15, "Fünfzehn");
        _textStrings.Add(16, "Sechszehn");
        _textStrings.Add(17, "Siebzehn");
        _textStrings.Add(18, "Achtzehn");
        _textStrings.Add(19, "Neunzehn");
        _textStrings.Add(20, "Zwanzig");
        _textStrings.Add(30, "Dreizig");
        _textStrings.Add(40, "Vierzig");
        _textStrings.Add(50, "Fünfzig");
        _textStrings.Add(60, "Sechszig");
        _textStrings.Add(70, "Siebzig");
        _textStrings.Add(80, "Achtzig");
        _textStrings.Add(90, "Neuzig");
        _textStrings.Add(100, "Hundert");

        #endregion

        #region Einheiten Strings

        _einheitenStrings.Add(1000000000, "Milliarden");
        _einheitenStrings.Add(1000000, "Millionen");
        _einheitenStrings.Add(1000, "Tausend");

        #endregion

        #endregion
    }

    public string ToText(string num, bool nachkomma = false)
    {
        _builderString = new StringBuilder();
        foreach (var x in num.Select(n => n.ToString()).Select(long.Parse))
        {
            _builderString.AppendFormat($"{_textStrings[x]} ");
        }

        return _builderString.ToString().Trim();
    }

    public string ToText(long num, bool nachkomma = false)
    {
        _builderString = new StringBuilder();

        if (!nachkomma)
        {
            if (num == 0)
            {
                _builderString.Append(_textStrings[num]);
                return _builderString.ToString();
            }

            num = _einheitenStrings.Aggregate(num, (cur, einheit) => Append(cur, einheit.Key));
            AppendKleinerTausend(num);
        }
        else
        {
            foreach (var x in num.ToString().Select(n => n.ToString()).Select(long.Parse))
            {
                _builderString.AppendFormat($"{_textStrings[x]} ");
            }
        }

        return _builderString.ToString().Trim();
    }

    private long Append(long num, long einheit)
    {
        if (num <= einheit - 1) return num;
        Einheit = einheit;
        var basisEinheit = num/einheit;
        AppendKleinerTausend(basisEinheit);
        _builderString.AppendFormat("{0} ", _einheitenStrings[einheit]);
        num = num - (basisEinheit*einheit);
        Einheit = 0;

        return num;
    }

    private long AppendKleinerTausend(long num)
    {
        num = AppendHunderter(num);
        num = AppendZehner(num);

        return num;
    }

    private void AppendEinheit(long num, string s = "")
    {
        if (num <= 0) return;

        if (Einheit >= 1000)
        {
            var text = $"{_textStrings[num]} und " + s;
            if (_builderString.Length == 0)
            {
                text = ReplaceEins(text);
            }
            else
            {
                text = ReplaceEins(text, false);
            }
            _builderString.AppendFormat(text);
        }
        else if (Einheit >= 1000000)
        {
            var text = $"{_textStrings[num]} " + s;
            text = ReplaceEins(text);

            _builderString.AppendFormat(text);
        }
        else if (Einheit >= 1000000000)
        {
            var text = $"{_textStrings[num]} " + s;
            text = ReplaceEins(text);
            _builderString.AppendFormat(text);
        }
        else
        {
            _builderString.AppendFormat($"{_textStrings[num]} und " + s);
        }
    }

    private long AppendZehner(long num)
    {
        if (num > 20)
        {
            var zehner = (num/10)*10;
            string eins = $"{_textStrings[zehner]} ";
            num = num - zehner;
            AppendEinheit(num, eins);
        }
        else
        {
            AppendEinheit(num);
        }
        return num;
    }

    private long AppendHunderter(long num)
    {
        if (num <= 99) return num;
        var hunderter = (num/100);
        var s = ReplaceEins(_textStrings[hunderter]);
        _builderString.AppendFormat("{0}hundert ", s);
        num = num - (hunderter*100);
        return num;
    }

    private string ReplaceEins(string text, bool ohneUnd = true)
    {
        switch (Einheit)
        {
            case 1000:
                var r = text.Replace(@"Eins", "Ein");
                if (ohneUnd)
                {
                    r = r.Replace("und", "");
                }
                r = r.Trim();
                return r;

            case 1000000:
                return text.Replace("Eins", "Eine").Replace("und", "");

            case 1000000000:
                return text.Replace("Eins", "Eine");

        }
        return text;
    }
}