using System;
using System.Runtime.CompilerServices;
using LightNovelSniffer.Config;
using LightNovelSniffer.Web;

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
                consoleTools = new ConsoleTools(1);
                ConsoleTools ctForWebcrawler = new ConsoleTools(3);
                webCrawler = new WebCrawler(ctForWebcrawler, ctForWebcrawler);
            }
            catch (ApplicationException e)
            {
                consoleTools.Log(e.Message);
                return;
            }

            if (!consoleTools.Ask("Utiliser le dossier de sortie \"" + Globale.OUTPUT_FOLDER + "\" ?"))
            {
                string folder = consoleTools.AskInformation("Merci de saisir le chemin du dossier racine à utiliser : ");
                if (!string.IsNullOrEmpty(folder))
                    Globale.OUTPUT_FOLDER = folder;
            }

            consoleTools.Log("Début du programme");

            foreach (LnParameters ln in Globale.LN_TO_RETRIEVE)
            {
                if (consoleTools.Ask(string.Format("Voulez-vous récupérer {0} ?", ln.name.ToUpper())))
                    GetNovel(ln);
            }

            if (Globale.INTERACTIVE_MODE && consoleTools.AskNegative("Fin du traitement des Light Novel prédéfinies. Voulez vous en traiter d'autres ?"))
            {
                LnParameters ln;
                do
                {
                    ln = BuildDynamicLn();
                    GetNovel(ln);
                } while (!string.IsNullOrEmpty(ln.name));

            }

            consoleTools.Log("Fin du programme");
            if (Globale.INTERACTIVE_MODE)
                Console.ReadLine();
        }

        private static void GetNovel(LnParameters ln)
        {
            foreach (UrlParameter up in ln.urlParameters)
            {
                if (consoleTools.Ask(string.Format("Voulez-vous récupérer la version {0} ?", up.language)))
                {
                    if (string.IsNullOrEmpty(up.url))
                        up.url = consoleTools.AskUrl(string.Format("Aucune URL renseignée pour {0} dans cette langue. Merci de saisir l'url d'un chapitre ici: ", ln.name));

                    if (!string.IsNullOrEmpty(up.url))
                        webCrawler.DownloadChapters(ln, up.language);
                    else
                        consoleTools.Log(string.Format("Pas d'URL pour la version {0}. Traitement arrêté", up.language));
                }
            }
        }

        private static LnParameters BuildDynamicLn()
        {
            LnParameters ln = new LnParameters();

            ln.name = consoleTools.AskInformation("Nom de la LN (vide pour arrêter) : ");
            if (string.IsNullOrEmpty(ln.name))
                return ln;
            ln.urlCover = consoleTools.AskInformation("URL de l'image de cover : ");
            string authors = consoleTools.AskInformation("Auteurs (séparés par des virgules) : ");
            if (!string.IsNullOrEmpty(authors))
                foreach (string author in authors.Split(','))
                    ln.authors.Add(author.Trim());

            do
            {
                UrlParameter up = new UrlParameter();

                up.language = consoleTools.AskInformation("Entrez la langue de cette version (FR/EN/...) : ");
                up.url = consoleTools.AskInformation("Entrez l'url d'un des chapitres : ");
                int.TryParse(consoleTools.AskInformation("A quel chapitre voulez vous commencer (si vide, 1) : "), out up.firstChapterNumber);
                int.TryParse(consoleTools.AskInformation("A quel chapitre voulez vous vous arrêter (si vide, jusqu'au dernier paru) : "), out up.lastChapterNumber);

                if (!string.IsNullOrEmpty(up.language) && !string.IsNullOrEmpty(up.url))
                    ln.urlParameters.Add(up);
                else
                    consoleTools.Log("Pas d'URL ou de language pour cette version. Ajout impossible");
            } while (consoleTools.Ask("Voulez vous ajouter une autre version pour cette LN ?"));

            return ln;
        }
    }
}
