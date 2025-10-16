using System;
using System.Collections.Generic;

namespace QuizApp
{
    class Question
    {
        public string Text { get; set; }
        public List<string> Optionen { get; set; }
        public string RichtigeAntwort { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Question> fragen = new List<Question>
            {
                new Question { Text = "Was ist ein Projekt?", Optionen = new List<string>{ "a) Eine dauerhafte Aufgabe", "b) Eine einmalige Aufgabe mit Ziel", "c) Eine Routinearbeit" }, RichtigeAntwort = "b" },
                new Question { Text = "Was bedeutet SMART bei Zieldefinitionen?", Optionen = new List<string>{ "a) Spezifisch, Messbar, Attraktiv, Realistisch, Terminiert", "b) Schnell, Machbar, Aktiv, Real, Testbar", "c) Sicher, Modern, Aktiv, Realistisch, Terminiert" }, RichtigeAntwort = "a" },
                new Question { Text = "Was ist ein Meilenstein?", Optionen = new List<string>{ "a) Ein Projektrisiko", "b) Ein wichtiger Projektzeitpunkt", "c) Ein Budgetposten" }, RichtigeAntwort = "b" },
                new Question { Text = "Was ist ein Projektstrukturplan?", Optionen = new List<string>{ "a) Ein Zeitplan", "b) Eine Aufgabenübersicht", "c) Ein Budgetplan" }, RichtigeAntwort = "b" },
                new Question { Text = "Was ist ein Gantt-Diagramm?", Optionen = new List<string>{ "a) Ein Balkendiagramm zur Zeitplanung", "b) Ein Kreisdiagramm", "c) Ein Organigramm" }, RichtigeAntwort = "a" },
                new Question { Text = "Was ist ein Stakeholder?", Optionen = new List<string>{ "a) Ein Projektleiter", "b) Eine interessierte Person/Gruppe", "c) Ein Sponsor" }, RichtigeAntwort = "b" },
                new Question { Text = "Was ist ein Risiko im Projekt?", Optionen = new List<string>{ "a) Eine Chance", "b) Eine Unsicherheit mit negativer Auswirkung", "c) Ein Ziel" }, RichtigeAntwort = "b" },
                new Question { Text = "Was ist ein Projektauftrag?", Optionen = new List<string>{ "a) Eine Rechnung", "b) Eine Zielvereinbarung", "c) Eine formale Beauftragung" }, RichtigeAntwort = "c" },
                new Question { Text = "Was ist ein Projektziel?", Optionen = new List<string>{ "a) Ein Wunsch", "b) Ein klar definiertes Ergebnis", "c) Eine Idee" }, RichtigeAntwort = "b" },
                new Question { Text = "Was ist ein Projektleiter?", Optionen = new List<string>{ "a) Ein Teammitglied", "b) Eine Führungskraft", "c) Verantwortlich für Planung und Steuerung" }, RichtigeAntwort = "c" },
                new Question { Text = "Was ist ein Projektteam?", Optionen = new List<string>{ "a) Eine Abteilung", "b) Eine Gruppe von Projektbeteiligten", "c) Die Geschäftsführung" }, RichtigeAntwort = "b" },
                new Question { Text = "Was ist ein Projektstart-Workshop?", Optionen = new List<string>{ "a) Ein Schulungstag", "b) Ein Kick-off zur Abstimmung", "c) Ein Abschlussgespräch" }, RichtigeAntwort = "b" },
                new Question { Text = "Was ist ein Projektabschlussbericht?", Optionen = new List<string>{ "a) Ein Budgetplan", "b) Eine Zusammenfassung der Ergebnisse", "c) Ein Zeitplan" }, RichtigeAntwort = "b" },
                new Question { Text = "Was ist Projektcontrolling?", Optionen = new List<string>{ "a) Kontrolle der Mitarbeiter", "b) Überwachung von Zeit, Kosten, Qualität", "c) Eine Abschlussprüfung" }, RichtigeAntwort = "b" },
                new Question { Text = "Was ist ein Projektbudget?", Optionen = new List<string>{ "a) Anzahl der Mitarbeiter", "b) Geplante Kosten", "c) Die Dauer" }, RichtigeAntwort = "b" },
                new Question { Text = "Was ist ein Projektstatusbericht?", Optionen = new List<string>{ "a) Bericht über Projektfortschritt", "b) Ein Abschlussbericht", "c) Ein Zeitplan" }, RichtigeAntwort = "a" },
                new Question { Text = "Was ist ein Projektlebenszyklus?", Optionen = new List<string>{ "a) Dauer eines Meetings", "b) Phasen eines Projekts", "c) Lebensdauer eines Produkts" }, RichtigeAntwort = "b" },
                new Question { Text = "Was ist ein Projektzielkonflikt?", Optionen = new List<string>{ "a) Unterschiedliche Erwartungen", "b) Ein technisches Problem", "c) Ein Zeitverzug" }, RichtigeAntwort = "a" },
                new Question { Text = "Was ist ein Projektmeilensteinplan?", Optionen = new List<string>{ "a) Ein Budgetplan", "b) Plan mit wichtigen Terminen", "c) Ein Organigramm" }, RichtigeAntwort = "b" },
                new Question { Text = "Was ist ein Projektkommunikationsplan?", Optionen = new List<string>{ "a) Plan für Meetings", "b) Plan für Informationsflüsse", "c) Ein Zeitplan" }, RichtigeAntwort = "b" }
            }

            bool running = true;
            while (running)
            {
                Console.WriteLine("Willkommen bei QuizApp");
                Console.WriteLine("1. Quiz starten");
                Console.WriteLine("2. Beenden");
                Console.Write("Bitte wählen: ");
                string auswahl = Console.ReadLine();

                if (auswahl == "1")
                {
                    Console.WriteLine("Drücken Sie Enter, um das Quiz zu starten...");
                    Console.ReadKey(true);

                    int punkte = 0;
                    foreach (var frage in fragen)
                    {
                        Console.WriteLine("" + frage.Text);
                        foreach (var option in frage.Optionen)
                        {
                            Console.WriteLine(option);
                        }

                        string antwort = "";
                        while (true)
                        {
                            Console.Write("Antwort: ");
                            antwort = Console.ReadLine();
                            if (!string.IsNullOrWhiteSpace(antwort)) break;
                            Console.WriteLine("Bitte geben Sie eine gültige Antwort ein.");
                        }

                        antwort = antwort.ToLower();
                        if (antwort == frage.RichtigeAntwort)
                        {
                            Console.WriteLine("Richtig!");
                            punkte++;
                        }
                        else
                        {
                            Console.WriteLine($"Falsch. Die richtige Antwort ist: {frage.RichtigeAntwort}");
                        }
                        Console.WriteLine("Weiter mit Enter...");
                        Console.ReadKey(true);
                    }

                    Console.WriteLine($"Du hast {punkte} von {fragen.Count} Punkten erreicht.");
                    Console.WriteLine("Drücke eine Taste zum Menü ");
                    Console.ReadKey(true);
                }
                else if (auswahl == "2")
                {
                    running = false;
                }
                else
                {
                    Console.WriteLine("Ungültige Eingabe. Bitte erneut versuchen.");
                    Console.ReadKey(true);
                }
            }
        }
    }
}
