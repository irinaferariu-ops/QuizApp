using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace QuizApp2
{
    public class Program
    {
        // Speichere Scores im Benutzer-AppData-Ordner
        static string ScoresFile
        {
            get
            {
                var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                var dir = Path.Combine(appData, "QuizApp2");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return Path.Combine(dir, "scores.json");
            }
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            ShowHeader();

            var scores = LoadScores();

            while (true)
            {
                ShowMainMenu();
                var choice = Console.ReadLine()?.Trim() ?? string.Empty;

                if (choice == "1")
                {
                    var user = CollectUserData();
                    RunQuizForUser(user);
                    scores.Add(user);
                    SaveScores(scores);
                    Console.WriteLine("\n\u2705 Ergebnis gespeichert.");
                    PauseAndClear();
                }
                else if (choice == "2")
                {
                    ShowHighscores(scores);
                    PauseAndClear();
                }
                else if (choice == "3")
                {
                    Console.WriteLine("Beende Programm.\n");
                    break;
                }
                else
                {
                    Console.WriteLine("Ungültige Auswahl, bitte 1-3 wählen.");
                }
            }

        }

        static void ShowHeader()
        {
            Console.Clear();
            Console.WriteLine("=========================================");
            Console.WriteLine("\t🎓 QuizApp - Prüfungssimulation");
            Console.WriteLine("\tProjektmanagement - Lernmodus");
            Console.WriteLine("=========================================");
        }

        static void ShowMainMenu()
        {
            Console.WriteLine("\nHauptmenü:");
            Console.WriteLine("1) Quiz starten");
            Console.WriteLine("2) Highscores anzeigen");
            Console.WriteLine("3) Beenden");
            Console.Write("Auswahl: ");
        }

        static Benutzer CollectUserData()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("👤 Bitte gib deinen Namen ein: ");
            Console.ResetColor();
            string name = Console.ReadLine() ?? string.Empty;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("🏙️ Stadt: ");
            Console.ResetColor();
            string stadt = Console.ReadLine() ?? string.Empty;

            string datum = string.Empty;
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("📅 Datum (TT.MM.JJJJ): ");
                Console.ResetColor();
                var input = Console.ReadLine() ?? string.Empty;
                if (DateTime.TryParseExact(input, new[] { "dd.MM.yyyy", "d.M.yyyy" }, null, System.Globalization.DateTimeStyles.None, out var dt))
                {
                    datum = dt.ToString("dd.MM.yyyy");
                    break;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Ungültiges Datum. Bitte im Format TT.MM.JJJJ eingeben.");
                    Console.ResetColor();
                }
            }

            Console.WriteLine("📘 Welches Niveau möchtest du prüfen?");
            Console.WriteLine("1. Basiszertifizierung");
            Console.WriteLine("2. Level D");
            Console.WriteLine("3. Level C");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("Deine Auswahl (1-3): ");
                    Console.ResetColor();
                    string niveau = Console.ReadLine() ?? string.Empty;

                    return new Benutzer(name, stadt, datum, niveau);
        }

        static void RunQuizForUser(Benutzer user)
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string pfad = Path.Combine(basePath, "question.json");

                Console.WriteLine($"\n🔎 Lade Fragen aus: {pfad}");
                List<Frage> fragen = JsonConvert.DeserializeObject<List<Frage>>(File.ReadAllText(pfad)) ?? new List<Frage>();

                double punktzahl = 0.0;
                int frageNummer = 1;

                foreach (var frage in fragen)
                {
                    Console.WriteLine($"\nFrage {frageNummer}/{fragen.Count}:");
                    Console.WriteLine(frage.FrageText);

                    if (frage.Typ == "MultipleChoice")
                    {
                        for (int i = 0; i < frage.Antworten.Count; i++)
                        {
                            Console.WriteLine($"{i + 1}. {frage.Antworten[i]}");
                        }

                        Console.Write("➡️ Deine Antwort (z.B. 1,2): ");
                        var eingabe = Console.ReadLine() ?? string.Empty;
                        var antworten = eingabe.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(s => int.TryParse(s.Trim(), out int val) ? val - 1 : -1)
                            .Where(i => i >= 0)
                            .ToList();

                        if (antworten.Count > 0 &&
                            antworten.All(a => frage.RichtigeAntworten.Contains(a)) &&
                            antworten.Count == frage.RichtigeAntworten.Count)
                        {
                            Console.WriteLine("✅ Richtig!");
                            punktzahl++;
                        }
                        else
                        {
                            Console.WriteLine("❌ Falsch!");
                            Console.WriteLine("✔️ Richtige Antwort(en): " + string.Join(", ", frage.RichtigeAntworten.Select(i => (i + 1).ToString())));
                        }
                    }
                    else if (frage.Typ == "Offen")
                    {
                        Console.Write("✏️ Deine Antwort: ");
                        var userAntwort = Console.ReadLine() ?? string.Empty;

                        double score = AehnlichkeitsScore(userAntwort, frage.RichtigeAntwort);
                        if (score >= 0.8)
                        {
                            Console.WriteLine("✅ Vollständig korrekt!");
                            punktzahl++;
                        }
                        else if (score >= 0.5)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("🟡 Teilweise korrekt (0.5 Punkte).");
                            Console.ResetColor();
                            punktzahl += 0.5;
                        }
                        else
                        {
                            Console.WriteLine("❌ Leider falsch.");
                            Console.WriteLine("✔️ Erwartete Antwort: " + frage.RichtigeAntwort);
                        }
                    }

                    frageNummer++;
                }

                user.Punktestand = punktzahl;
                Console.WriteLine($"\n🏁 Prüfung beendet! {user.Name}, du hast {punktzahl} von {fragen.Count} Punkten erreicht.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der Fragen: {ex.Message}");
            }
        }

    public static double AehnlichkeitsScore(string benutzerAntwort, string richtigeAntwort)
        {
            benutzerAntwort = benutzerAntwort.ToLower().Trim();
            richtigeAntwort = richtigeAntwort.ToLower().Trim();

            var schluesselwoerter = richtigeAntwort.Split(' ')
                .Where(w => w.Length > 3)
                .Distinct()
                .ToList();

            if (schluesselwoerter.Count == 0)
                return 0;

            int treffer = schluesselwoerter.Count(w => benutzerAntwort.Contains(w));
            return (double)treffer / schluesselwoerter.Count;
        }

        // Bewertet MultipleChoice-Antworten: 1.0 für komplett korrekt, sonst 0.0
        public static double EvaluateMultipleChoice(Frage frage, List<int> benutzerAntworten)
        {
            if (benutzerAntworten == null || benutzerAntworten.Count == 0)
                return 0.0;

            if (benutzerAntworten.All(a => frage.RichtigeAntworten.Contains(a)) && benutzerAntworten.Count == frage.RichtigeAntworten.Count)
                return 1.0;

            return 0.0;
        }

        // Bewertet offene Frage: nutzt AehnlichkeitsScore und gibt 1.0, 0.5 oder 0.0 zurück
        public static double EvaluateOpenQuestion(string benutzerAntwort, string richtigeAntwort)
        {
            double score = AehnlichkeitsScore(benutzerAntwort, richtigeAntwort);
            if (score >= 0.8) return 1.0;
            if (score >= 0.5) return 0.5;
            return 0.0;
        }

        static List<Benutzer> LoadScores()
        {
            try
            {
                if (!File.Exists(ScoresFile))
                    return new List<Benutzer>();

                return JsonConvert.DeserializeObject<List<Benutzer>>(File.ReadAllText(ScoresFile)) ?? new List<Benutzer>();
            }
            catch
            {
                return new List<Benutzer>();
            }
        }

        static void SaveScores(List<Benutzer> scores)
        {
            try
            {
                File.WriteAllText(ScoresFile, JsonConvert.SerializeObject(scores, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Speichern der Scores: {ex.Message}");
            }
        }

        static void ShowHighscores(List<Benutzer> scores)
        {
            Console.WriteLine("\n🏆 Highscores:");
            var top = scores.OrderByDescending(s => s.Punktestand).ThenBy(s => s.Name).Take(10).ToList();
            if (top.Count == 0)
            {
                Console.WriteLine("Keine Scores vorhanden.");
                return;
            }

            int rank = 1;
            foreach (var s in top)
            {
                Console.WriteLine($"{rank}. {s.Name} - {s.Punktestand} Punkte ({s.Datum})");
                rank++;
            }
        }

        static void PauseAndClear()
        {
            Console.WriteLine("\nDrücke Enter zum Zurückkehren zum Menü...");
            Console.ReadLine();
            Console.Clear();
            ShowHeader();
        }
    }
}


















