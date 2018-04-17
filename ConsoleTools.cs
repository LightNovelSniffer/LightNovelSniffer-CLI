using System;
using System.Globalization;
using System.Text.RegularExpressions;
using LightNovelSniffer.Config;
using LightNovelSniffer.Libs;
using LightNovelSniffer_CLI.Resources;

namespace LightNovelSniffer_CLI
{
    internal class ConsoleTools : IInput, IOutput
    {
        private bool isProgressOngoing = false;
        private int defaultIndentation = 1;
        
        public ConsoleTools(int defaultIndentation)
        {
            this.defaultIndentation = defaultIndentation;
        }

        private void CheckProgress()
        {
            if (isProgressOngoing)
            {
                Console.WriteLine("");
                isProgressOngoing = false;
            }
        }

        private void OutputString(string str)
        {
            Console.Write(DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern) + " : " + str);
        }

        private string Indent(string text, int tab)
        {
            while (tab > 1)
            {
                text = "  " + text;
                tab--;
            }

            return text;
        }

        public void Log(string text, int tab)
        {
            CheckProgress();
            OutputString(Indent(text, tab) + "\r\n");
        }

        public void Progress(string text, int tab)
        {
            isProgressOngoing = true;
            Console.Write("\r" + DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern) + " : " + Indent(text, tab));
        }

        public void Log(string text)
        {
            Log(text, defaultIndentation);
        }

        public void Progress(string text)
        {
            Progress(text, defaultIndentation);
        }

        public bool Ask(string question)
        {
            string input = AskInformation(question + " " + LightNovelSniffer_CLI_Strings.AskQuestionChoicesPositiveAnswer.ToUpper() + "/" + LightNovelSniffer_CLI_Strings.AskQuestionChoicesNegativeAnswer.ToLower() + " ");
            return input != null && !input.ToUpper().Equals(LightNovelSniffer_CLI_Strings.AskQuestionChoicesNegativeAnswer.ToUpper());
        }

        public string AskInformation(string question)
        {
            if (!Globale.INTERACTIVE_MODE)
                return "";
            CheckProgress();
            OutputString(question);
            return Console.ReadLine();
        }

        public string AskUrl(string question)
        {
            string url = AskInformation(question);

            if (string.IsNullOrEmpty(url))
                return null;

            url = url.TrimEnd('/');

            while (Regex.Match(url, @"\d$").Success)
            {
                url = url.Substring(0, url.Length - 1);
            }

            return url + "{0}";
        }

        public bool AskNegative(string question)
        {
            string input = AskInformation(question + " " + LightNovelSniffer_CLI_Strings.AskQuestionChoicesPositiveAnswer.ToLower() + "/" + LightNovelSniffer_CLI_Strings.AskQuestionChoicesNegativeAnswer.ToUpper() + " ");
            return !(string.IsNullOrEmpty(input) || input.ToUpper().Equals(LightNovelSniffer_CLI_Strings.AskQuestionChoicesNegativeAnswer.ToUpper()));
        }
    }
}
