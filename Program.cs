// Copyright (c) 2018 Joshua Arus <joshua.arus@gmail.com>

// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions
// are met:
// 1. Redistributions of source code must retain the above copyright
//    notice, this list of conditions and the following disclaimer.
// 2. Redistributions in binary form must reproduce the above copyright
//    notice, this list of conditions and the following disclaimer in the
//    documentation and/or other materials provided with the distribution.

// THIS SOFTWARE IS PROVIDED BY THE AUTHOR AND CONTRIBUTORS ``AS IS'' AND
// ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
// OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
// LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
// OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
// SUCH DAMAGE.

using System;
using LightNovelSniffer.Config;
using LightNovelSniffer.Web;
using LightNovelSniffer_CLI.Resources;

namespace LightNovelSniffer_CLI
{
    public static class Program
    {
        private static WebCrawler webCrawler;
        private static ConsoleTools consoleTools;

        public static void Main(string[] args)
        {
            try
            {
                ConfigTools.InitConf();
                ConfigTools.InitLightNovels();
                consoleTools = new ConsoleTools(1);
                ConsoleTools ctForWebcrawler = new ConsoleTools(3);
                webCrawler = new WebCrawler(ctForWebcrawler, ctForWebcrawler);
            }
            catch (ApplicationException e)
            {
                consoleTools.Log(e.Message);
                return;
            }

            if (!consoleTools.Ask(String.Format(LightNovelSniffer_CLI_Strings.AskOutputFolderConfirmation, Globale.OUTPUT_FOLDER)))
            {
                string folder = consoleTools.AskInformation(LightNovelSniffer_CLI_Strings.AskOutputFolder);
                if (!string.IsNullOrEmpty(folder))
                    Globale.OUTPUT_FOLDER = folder;
            }

            consoleTools.Log(LightNovelSniffer_CLI_Strings.LogProgramStart);

            foreach (LnParameters ln in Globale.LN_TO_RETRIEVE)
            {
                if (consoleTools.Ask(string.Format(LightNovelSniffer_CLI_Strings.AskRetrieveLn, ln.name)))
                    GetNovel(ln);
            }

            if (Globale.INTERACTIVE_MODE && consoleTools.AskNegative(LightNovelSniffer_CLI_Strings.LogEndOfLnInConfig))
            {
                LnParameters ln;
                do
                {
                    ln = BuildDynamicLn();
                    GetNovel(ln);
                } while (!string.IsNullOrEmpty(ln.name));

            }

            consoleTools.Log(LightNovelSniffer_CLI_Strings.LogProgramEnd);
            if (Globale.INTERACTIVE_MODE)
                Console.ReadLine();
        }

        private static void GetNovel(LnParameters ln)
        {
            foreach (UrlParameter up in ln.urlParameters)
            {
                if (consoleTools.Ask(string.Format(LightNovelSniffer_CLI_Strings.AskRetrieveLnLanguage, up.language)))
                {
                    if (string.IsNullOrEmpty(up.url))
                        up.url = consoleTools.AskUrl(string.Format(LightNovelSniffer_CLI_Strings.AskLnUrl, ln.name));

                    if (!string.IsNullOrEmpty(up.url))
                        webCrawler.DownloadChapters(ln, up.language);
                    else
                        consoleTools.Log(string.Format(LightNovelSniffer_CLI_Strings.LogNoLnUrlStopProcess, up.language));
                }
            }
        }

        private static LnParameters BuildDynamicLn()
        {
            LnParameters ln = new LnParameters();

            ln.name = consoleTools.AskInformation(LightNovelSniffer_CLI_Strings.AskLnName_EmptyToStop);
            if (string.IsNullOrEmpty(ln.name))
                return ln;
            ln.urlCover = consoleTools.AskInformation(LightNovelSniffer_CLI_Strings.AskCoverUrl);
            string authors = consoleTools.AskInformation(LightNovelSniffer_CLI_Strings.AskCsvAuthor);
            if (!string.IsNullOrEmpty(authors))
                foreach (string author in authors.Split(','))
                    ln.authors.Add(author.Trim());

            do
            {
                UrlParameter up = new UrlParameter();

                up.language = consoleTools.AskInformation(LightNovelSniffer_CLI_Strings.AskLnVersionLanguage);
                up.url = consoleTools.AskInformation(LightNovelSniffer_CLI_Strings.AskLnVersionChapterUrl);
                int.TryParse(consoleTools.AskInformation(LightNovelSniffer_CLI_Strings.AskFirstChapterNumber), out up.firstChapterNumber);
                int.TryParse(consoleTools.AskInformation(LightNovelSniffer_CLI_Strings.AskLastChapterNumber), out up.lastChapterNumber);

                if (!string.IsNullOrEmpty(up.language) && !string.IsNullOrEmpty(up.url))
                    ln.urlParameters.Add(up);
                else
                    consoleTools.Log(LightNovelSniffer_CLI_Strings.LogNoUrlOrLanguageForThisVersion);
            } while (consoleTools.Ask(LightNovelSniffer_CLI_Strings.AskAddAnotherLnVersion));

            return ln;
        }
    }
}
