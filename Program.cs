using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace QuizApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            User user = new User();
            ZeigeMenü(user);

            List<Question> fragen = LadeFragen("questions.json");

            Console.Clear();
            Console.WriteLine("==== Prüfung Start ====");
            Console.WriteLine($"Name: {user.Name}");
            Console.WriteLine($"Stadt: {user.Stadt}");
            Console.WriteLine($"Datum: {user.Datum}");
            Console.WriteLine($"Niveau: {user.Niveau}");
            Console.WriteLine("\nDrücke eine Taste, um zu beginnen...");
            Console.ReadKey();

            foreach (var frage in fragen)
            {
                Console.Clear();
                Console.WriteLine($"Frage: {frage.Text}\n");

                if (frage.Typ == "MultipleChoice")
                {
                    for (int i = 0; i < frage.Optionen.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {frage.Optionen[i]}");
                    }

                    Console.Write("\nDeine Antwort (Nummern mit Komma trennen, z.B. 1,3): ");
                    string input = Console.ReadLine() ?? "";

                    var eingaben = input
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(i => i.Trim())
                        .ToList();

                    List<string> gegebeneAntworten = new List<string>();

                    foreach (var eingabe in eingaben)
                    {
                        if (int.TryParse(eingabe, out int nummer) &&
                            nummer >= 1 &&
                            nummer <= frage.Optionen.Count)
                        {
                            gegebeneAntworten.Add(frage.Optionen[nummer - 1]);
                        }
                    }

                    if (Enumerable.SequenceEqual(
                            gegebeneAntworten.OrderBy(x => x),
                            frage.RichtigeAntworten.OrderBy(x => x)))
                    {
                        Console.WriteLine("✅ Richtig!");
                        user.Punktestand++;
                    }
                    else
                    {
                        Console.WriteLine("❌ Falsch!");
                        Console.WriteLine($"Richtige Antwort(en): {string.Join(", ", frage.RichtigeAntworten)}");
                    }
                }
                else if (frage.Typ == "OffeneFrage")
                {
                    Console.Write("Deine Antwort: ");
                    string antwort = Console.ReadLine() ?? "";

                    if (string.IsNullOrWhiteSpace(antwort))
                    {
                        Console.WriteLine("⚠️ Du hast keine Antwort eingegeben.");
                    }
                    else if (AntwortIstAehnlich(antwort, frage.RichtigeAntworten))
                    {
                        Console.WriteLine("✅ Richtig (inhaltlich erkannt)");
                        user.Punktestand++;
                    }
                    else
                    {
                        Console.WriteLine("❌ Nicht korrekt erkannt.");
                        if (frage.RichtigeAntworten.Count > 0)
                            Console.WriteLine($"Mögliche Antwort: {frage.RichtigeAntworten[0]}");
                    }
                }
                Console.WriteLine("\nDrücke eine Taste für die nächste Frage...");
                Console.ReadKey();
            }
            Console.Clear();
            Console.WriteLine("🎉 Prüfung abgeschlossen!");
            Console.WriteLine($"Punktestand: {user.Punktestand} / {fragen.Count}");
            Console.WriteLine("Vielen Dank und viel Erfolg weiterhin!");
        }
        static void ZeigeMenü(User user)
        {
            Console.Clear();
            Console.WriteLine("==== Projektmanagement Prüfung ====\n");

            Console.Write("Gib deinen Namen ein: ");
            string? nameInput = Console.ReadLine();
            user.Name = nameInput ?? "";

            Console.Write("Stadt der Prüfung: ");
            string? stadtInput = Console.ReadLine();
            user.Stadt = stadtInput ?? "";

            Console.Write("Datum (z.B. 17.10.2025): ");
            string? datumInput = Console.ReadLine();
            user.Datum = datumInput ?? "";

            Console.WriteLine("Wähle das Prüfungsniveau:");
            Console.WriteLine("1. Basiszertifizierung");
            Console.WriteLine("2. Niveau D");
            Console.WriteLine("3. Niveau C");
            Console.Write("Deine Wahl (1-3): ");
            string? auswahl = Console.ReadLine();

            switch (auswahl)
            {
                case "1":
                    user.Niveau = "Basiszertifizierung";
                    break;
                case "2":
                    user.Niveau = "Niveau D";
                    break;
                case "3":
                    user.Niveau = "Niveau C";
                    break;
                default:
                    user.Niveau = "Unbekannt";
                    break;
            }
        }

        static List<Question> LadeFragen(string dateiPfad)
        {
            if (!File.Exists(dateiPfad))
            {
                Console.WriteLine("❌ Fehler: questions.json nicht gefunden.");
                return new List<Question>();
            }

            string json = File.ReadAllText(dateiPfad);
            var fragen = JsonSerializer.Deserialize<List<Question>>(json);
            return fragen ?? new List<Question>();
        }

        static bool AntwortIstAehnlich(string benutzerAntwort, List<string> richtigeAntworten, double toleranz = 0.6)
        {
            if (string.IsNullOrWhiteSpace(benutzerAntwort)) return false;

            benutzerAntwort = Regex.Replace(benutzerAntwort.ToLower(), @"[^\w\s]", "");

            foreach (var richtige in richtigeAntworten)
            {
                string richtigeClean = Regex.Replace(richtige.ToLower(), @"[^\w\s]", "");
                double score = BerechneAehnlichkeit(benutzerAntwort, richtigeClean);
                if (score >= toleranz)
                    return true;
            }

            return false;
        }

        static double BerechneAehnlichkeit(string s1, string s2)
        {
            if (string.IsNullOrEmpty(s1) || string.IsNullOrEmpty(s2)) return 0.0;

            int len1 = s1.Length;
            int len2 = s2.Length;
            int[,] matrix = new int[len1 + 1, len2 + 1];

            for (int i = 0; i <= len1; i++) matrix[i, 0] = i;
            for (int j = 0; j <= len2; j++) matrix[0, j] = j;

            for (int i = 1; i <= len1; i++)
            {
                for (int j = 1; j <= len2; j++)
                {
                    int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;
                    matrix[i, j] = Math.Min(
                        Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                        matrix[i - 1, j - 1] + cost
                    );
                }
            }

            int maxLen = Math.Max(len1, len2);
            return 1.0 - (double)matrix[len1, len2] / maxLen;
        }
    }
}

