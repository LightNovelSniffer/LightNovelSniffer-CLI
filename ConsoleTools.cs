using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        private List<TextWriter> fileWriters;

        public ConsoleTools(TextWriter fileWriter, int defaultIndentation)
            : this(new List<TextWriter> {fileWriter}, defaultIndentation)
        {
        }

        public ConsoleTools(List<TextWriter> fileWriters, int defaultIndentation)
        {
            this.defaultIndentation = defaultIndentation;
            this.fileWriters = fileWriters;
        }

        private void CheckProgress()
        {
            if (isProgressOngoing)
            {
                Write("\r\n");
                isProgressOngoing = false;
            }
        }

        private void WriteToStream(string str)
        {
            foreach (TextWriter sw in fileWriters)
            {
                sw.Write(str);
            }
        }

        private void Write(string str)
        {
            WriteToStream(str);
            Console.Out.Write(str);
        }

        private void OutputString(string str)
        {
            string output = DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern) + " : " +
                            str;
            Write(output);
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
            Write("\r" + DateTime.Now.ToString(CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern) + " : " +
                  Indent(text, tab));
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
            string input =
                AskInformation(question + " " + LightNovelSniffer_CLI_Strings.AskQuestionChoicesPositiveAnswer.ToUpper() +
                               "/" + LightNovelSniffer_CLI_Strings.AskQuestionChoicesNegativeAnswer.ToLower());
            return input != null &&
                   !input.ToUpper().Equals(LightNovelSniffer_CLI_Strings.AskQuestionChoicesNegativeAnswer.ToUpper());
        }

        public string AskInformation(string question)
        {
            if (!Globale.INTERACTIVE_MODE)
                return "";
            CheckProgress();
            OutputString(question + " ");
            string input = Console.ReadLine();
            WriteToStream(input + "\r\n");
            return input;
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
            string input =
                AskInformation(question + " " + LightNovelSniffer_CLI_Strings.AskQuestionChoicesPositiveAnswer.ToLower() +
                               "/" + LightNovelSniffer_CLI_Strings.AskQuestionChoicesNegativeAnswer.ToUpper());
            return
                !(string.IsNullOrEmpty(input) ||
                  input.ToUpper().Equals(LightNovelSniffer_CLI_Strings.AskQuestionChoicesNegativeAnswer.ToUpper()));
        }
    }
}