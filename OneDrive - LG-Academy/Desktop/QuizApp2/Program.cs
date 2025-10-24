using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;

namespace QuizApp2
{
    // Hauptprogramm für die Quiz-App
    // Hier startet die Anwendung und verwaltet das Hauptmenü
    public class Program
    {
    // Hauptmethode - hier startet das Programm
        static void Main(string[] args)
        {
            // UTF-8 Encoding aktivieren, damit deutsche Umlaute korrekt angezeigt werden
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Zeige den Willkommensbildschirm
            ShowHeader();

            // Endlosschleife für das Hauptmenü - läuft bis Benutzer "Beenden" wählt
            while (true)
            {
                // Zeige das Hauptmenü an
                ZeigeHauptmenue();
                
                // Lese die Benutzereingabe und entferne Leerzeichen
                var auswahl = Console.ReadLine()?.Trim() ?? string.Empty;

                // Überprüfe welche Option gewählt wurde
                if (auswahl == "1")
                {
                    // Starte Quiz im Lernmodus mit Pomodoro-Timer
                    StarteQuizLernmodus();
                    Console.WriteLine("\n✅ Quiz Lernmodus beendet!");
                    PauseAndClear();
                }
                else if (auswahl == "2")
                {
                    // Starte Prüfungsmodus (1,5 Stunden, ohne Pausen)
                    StartePruefungsmodus();
                    Console.WriteLine("\n✅ Prüfungsmodus beendet!");
                    PauseAndClear();
                }
                else if (auswahl == "3")
                {
                    // Beende das Programm
                    Console.WriteLine("\n👋 Auf Wiedersehen! Viel Erfolg beim Lernen!");
                    break;
                }
                else
                {
                    // Ungültige Eingabe - zeige Fehlermeldung
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("❌ Ungültige Auswahl! Bitte wähle 1-3.");
                    Console.ResetColor();
                }
            }

        }

    // Zeigt den Willkommensbildschirm mit dem Titel der App
        static void ShowHeader()
        {
            Console.Clear(); // Bildschirm löschen
            Console.WriteLine("=========================================");
            Console.WriteLine("\t🎓 QuizApp - Prüfungssimulation");
            Console.WriteLine("\tProjektmanagement - Lernmodus");
            Console.WriteLine("=========================================");
        }

    // Zeigt das Hauptmenü mit allen verfügbaren Optionen
        static void ZeigeHauptmenue()
        {
            Console.WriteLine("\n📋 Hauptmenü:");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.WriteLine("1️⃣  📚 Quiz Lernmodus (mit Pomodoro)");
            Console.WriteLine("2️⃣  📝 Prüfungsmodus (1,5 Std, ohne Pause)");
            Console.WriteLine("3️⃣  🚪 Beenden");
            Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            Console.Write("➡️  Deine Auswahl: ");
        }

    // Startet das Quiz im Lernmodus mit Pomodoro-Technik
    // Pomodoro: 4 Runden à 25 Min Fokus mit 5 Min Pause, dann 20 Min lange Pause
        static void StarteQuizLernmodus()
        {
            Console.Clear();
            Console.WriteLine("\n📚 Quiz Lernmodus (Pomodoro)");
            Console.WriteLine("Du hast 4x 25 Minuten Fokus, dazwischen 5 Minuten Pause. Nach 4 Runden gibt es 20 Minuten Pause. Viel Erfolg!");
            
            // Initialisiere Variablen für Punktestand und Fragenzähler
            double punkte = 0;
            int frageNummer = 1;
            
            // Lade alle Fragen aus der JSON-Datei
            string pfad = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "question.json");
            var fragen = JsonConvert.DeserializeObject<List<Frage>>(File.ReadAllText(pfad));
            int totalFragen = fragen.Count;
            int frageIndex = 0;
            // Starte 4 Pomodoro-Runden
            for (int runde = 1; runde <= 4; runde++)
            {
                // Berechne wann die Fokusphase endet (jetzt + 25 Minuten)
                DateTime fokusEnde = DateTime.Now.AddMinutes(25);
                TimeSpan dauerGesamt = TimeSpan.FromMinutes(25);
                
                Console.WriteLine($"\n▶️ Fokus-Runde {runde}: 25 Minuten (bis {fokusEnde:HH:mm} Uhr)");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"⏱️  Timer: {dauerGesamt.Minutes:D2}:00 min\n");
                Console.ResetColor();
                
                // Variable um zu prüfen ob Timer läuft
                bool timerLaeuft = true;
                
                // Zeige einen Timer im Fenstertitel während der Fokusphase
                System.Threading.Tasks.Task timerTask = System.Threading.Tasks.Task.Run(() =>
                {
                    while (DateTime.Now < fokusEnde && timerLaeuft)
                    {
                        TimeSpan verbleibend = fokusEnde - DateTime.Now;
                        // Aktualisiere Fenstertitel
                        try 
                        { 
                            Console.Title = $"⏱️ Runde {runde}/4 - Verbleibend: {verbleibend.Minutes:D2}:{verbleibend.Seconds:D2}";
                        }
                        catch { /* Ignoriere Fehler wenn Titel nicht gesetzt werden kann */ }
                        Thread.Sleep(1000);
                    }
                });
                
                // Stelle Fragen bis Zeit abläuft oder alle Fragen durch sind
                while (DateTime.Now < fokusEnde && frageIndex < totalFragen)
                {
                    // Hole die nächste Frage
                    var frage = fragen[frageIndex];
                    Console.WriteLine($"\nFrage {frageNummer}/{totalFragen}:");
                    Console.WriteLine(frage.FrageText);
                    
                    // Überprüfe ob es eine Multiple-Choice oder offene Frage ist
                    if (frage.Typ == "MultipleChoice")
                    {
                        // Multiple-Choice Frage: Zeige alle Antwortoptionen an
                        for (int i = 0; i < frage.Antworten.Count; i++)
                            Console.WriteLine($"{i + 1}) {frage.Antworten[i]}");
                        
                        Console.Write("Antwort: ");
                        var eingabe = Console.ReadLine() ?? "";
                        
                        // Verarbeite die Eingabe: Benutzer kann mehrere Antworten mit Komma trennen (z.B. "1,3")
                        var antworten = eingabe.Split(',')
                            .Select(s => int.TryParse(s.Trim(), out int v) ? v - 1 : -1)
                            .Where(i => i >= 0)
                            .ToList();
                        
                        // Überprüfe ob alle gewählten Antworten korrekt sind
                        if (antworten.All(a => frage.RichtigeAntworten.Contains(a)) && 
                            antworten.Count == frage.RichtigeAntworten.Count)
                        { 
                            Console.WriteLine("✅ Richtig!"); 
                            punkte++; 
                        }
                        else 
                        { 
                            Console.WriteLine("❌ Falsch!"); 
                        }
                    }
                    else
                    {
                        // Offene Frage: Benutzer gibt Freitext-Antwort ein
                        // Für alle offenen Fragen inkl. Frage 3: eine Zeile, Antworten durch Komma getrennt
                        Console.Write("Antwort: ");
                        string userAntwort = Console.ReadLine() ?? "";
                        
                        // Bewerte die Antwort: Akzeptiere auch kurze/ähnliche Antworten
                        string richtigeAntwort = frage.RichtigeAntwort.ToLower();
                        string userAntwortLower = userAntwort.ToLower().Trim();
                        
                        // Teile die richtige Antwort in einzelne Schlüsselwörter
                        string[] schluesselwoerter = richtigeAntwort.Split(new[] {' ', ';', ','}, StringSplitOptions.RemoveEmptyEntries);
                        
                        // Prüfe ob mindestens ein Schlüsselwort in der Benutzerantwort vorkommt
                        bool enthaltSchluesselwort = schluesselwoerter.Any(s => 
                            s.Length > 3 && userAntwortLower.Contains(s.Trim()));
                        
                        // Berechne Ähnlichkeitsscore als Fallback
                        double score = enthaltSchluesselwort ? 1.0 : EvaluateOpenQuestion(userAntwort, frage.RichtigeAntwort);
                        
                        // Bewerte basierend auf dem Score
                        if (score >= 0.8)
                        {
                            Console.WriteLine("✅ Richtig!"); 
                            punkte++;
                        }
                        else if (score >= 0.5)
                        {
                            Console.WriteLine("➖ Teilweise richtig!"); 
                            punkte += 0.5;
                        }
                        else
                        {
                            Console.WriteLine("❌ Falsch!");
                        }
                    }
                    
                    // Gehe zur nächsten Frage
                    frageNummer++;
                    frageIndex++;
                }
                
                // Stoppe den Timer-Task
                timerLaeuft = false;
                
                // Nach jeder Fokusrunde kommt eine Pause
                if (runde < 4)
                {
                    // Kurze Pause nach Runden 1-3 (5 Minuten)
                    Console.WriteLine("\n⏰ Fokus vorbei! Jetzt 5 Minuten Pause!");
                    SpieleSignalTon();
                    
                    // Countdown für 5 Minuten Pause
                    for (int min = 5; min > 0; min--)
                    {
                        for (int sec = 59; sec >= 0; sec--)
                        {
                            Console.Write($"Pause: {min - 1}:{sec:D2} min...\r");
                            Thread.Sleep(1000); // Warte 1 Sekunde
                        }
                    }
                    Console.WriteLine("\nWeiter mit Fokus!");
                }
                else
                {
                    // Lange Pause nach Runde 4 (20 Minuten)
                    Console.WriteLine("\n⏰ 4 Runden geschafft! Jetzt 20 Minuten lange Pause!");
                    SpieleSignalTon();
                    
                    // Countdown für 20 Minuten Pause
                    for (int min = 20; min > 0; min--)
                    {
                        for (int sec = 59; sec >= 0; sec--)
                        {
                            Console.Write($"Lange Pause: {min - 1}:{sec:D2} min...\r");
                            Thread.Sleep(1000); // Warte 1 Sekunde
                        }
                    }
                    Console.WriteLine("\nWeiter mit Fokus!");
                }
            }
            
            // Zeige das Endergebnis an
            Console.WriteLine($"\nQuiz beendet! Du hast {punkte} Punkte.");
        }

    // Startet den Prüfungsmodus: 1,5 Stunden Zeit ohne Pausen
    // Alle Fragen werden durchgegangen bis Zeit abläuft oder alle beantwortet sind
        static void StartePruefungsmodus()
        {
            Console.Clear();
            Console.WriteLine("\n📝 Prüfungsmodus (1,5 Stunden, keine Pause)");
            
            // Speichere Start- und Endzeit der Prüfung
            DateTime start = DateTime.Now;
            DateTime ende = start.AddMinutes(90);
            
            // Initialisiere Punktestand und Fragenzähler
            double punkte = 0;
            int frageNummer = 1;
            
            // Lade alle Fragen aus der JSON-Datei
            string pfad = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "question.json");
            var fragen = JsonConvert.DeserializeObject<List<Frage>>(File.ReadAllText(pfad));
            foreach (var frage in fragen)
            {
                if (DateTime.Now >= ende)
                {
                    Console.WriteLine("\n⏰ Zeit vorbei! Prüfung beendet.");
                    SpieleSignalTon();
                    break;
                }
                Console.WriteLine($"\nFrage {frageNummer}/{fragen.Count}:");
                Console.WriteLine(frage.FrageText);
                if (frage.Typ == "MultipleChoice")
                {
                    for (int i = 0; i < frage.Antworten.Count; i++)
                        Console.WriteLine($"{i + 1}) {frage.Antworten[i]}");
                    Console.Write("Antwort: ");
                    var eingabe = Console.ReadLine() ?? "";
                    var antworten = eingabe.Split(',').Select(s => int.TryParse(s.Trim(), out int v) ? v - 1 : -1).Where(i => i >= 0).ToList();
                    if (antworten.All(a => frage.RichtigeAntworten.Contains(a)) && antworten.Count == frage.RichtigeAntworten.Count)
                    { Console.WriteLine("✅ Richtig!"); punkte++; }
                    else { Console.WriteLine("❌ Falsch!"); }
                }
                else
                {
                    Console.Write("Antwort: ");
                    var userAntwort = Console.ReadLine() ?? "";
                    double score = EvaluateOpenQuestion(userAntwort, frage.RichtigeAntwort);
                    if (score >= 0.8)
                    {
                        Console.WriteLine("✅ Richtig!"); punkte++;
                    }
                    else if (score >= 0.5)
                    {
                        Console.WriteLine("➖ Teilweise richtig!"); punkte += 0.5;
                    }
                    else
                    {
                        Console.WriteLine("❌ Falsch!");
                    }
                }
                frageNummer++;
            }
            
            // Zeige das Endergebnis mit der benötigten Zeit
            TimeSpan gebraucht = DateTime.Now - start;
            Console.WriteLine($"\nPrüfung beendet! Du hast {punkte} Punkte in {gebraucht.Minutes} Minuten.");
        }

    // Berechnet wie ähnlich die Benutzerantwort zur richtigen Antwort ist
    // Gibt einen Wert zwischen 0.0 (gar nicht ähnlich) und 1.0 (identisch) zurück
        public static double AehnlichkeitsScore(string benutzerAntwort, string richtigeAntwort)
        {
            // Konvertiere beide Antworten zu Kleinbuchstaben und entferne Leerzeichen
            benutzerAntwort = benutzerAntwort.ToLower().Trim();
            richtigeAntwort = richtigeAntwort.ToLower().Trim();

            // Extrahiere Schlüsselwörter aus der richtigen Antwort (Wörter länger als 3 Buchstaben)
            var schluesselwoerter = richtigeAntwort.Split(' ')
                .Where(w => w.Length > 3)
                .Distinct()
                .ToList();

            // Wenn keine Schlüsselwörter vorhanden, return 0
            if (schluesselwoerter.Count == 0)
                return 0;

            // Zähle wie viele Schlüsselwörter in der Benutzerantwort vorkommen
            int treffer = schluesselwoerter.Count(w => benutzerAntwort.Contains(w));
            
            // Berechne Prozentsatz der Treffer
            return (double)treffer / schluesselwoerter.Count;
        }

    // Bewertet eine Multiple-Choice Antwort
    // Gibt 1.0 zurück wenn alles korrekt ist, sonst 0.0
        public static double EvaluateMultipleChoice(Frage frage, List<int> benutzerAntworten)
        {
            // Wenn keine Antworten gegeben wurden
            if (benutzerAntworten == null || benutzerAntworten.Count == 0)
                return 0.0;

            // Prüfe ob alle gewählten Antworten richtig sind UND keine fehlt
            if (benutzerAntworten.All(a => frage.RichtigeAntworten.Contains(a)) && 
                benutzerAntworten.Count == frage.RichtigeAntworten.Count)
                return 1.0;

            return 0.0;
        }

    // Bewertet eine offene Frage basierend auf Ähnlichkeit
    // Gibt 1.0 (richtig), 0.5 (teilweise) oder 0.0 (falsch) zurück
        public static double EvaluateOpenQuestion(string benutzerAntwort, string richtigeAntwort)
        {
            // Berechne Ähnlichkeitsscore
            double score = AehnlichkeitsScore(benutzerAntwort, richtigeAntwort);
            
            // Bewerte basierend auf Score
            if (score >= 0.8) return 1.0;  // 80% oder mehr Übereinstimmung = richtig
            if (score >= 0.5) return 0.5;  // 50-80% Übereinstimmung = teilweise richtig
            return 0.0;                     // Unter 50% = falsch
        }

    // Wartet auf Enter-Taste und zeigt dann wieder das Hauptmenü
        static void PauseAndClear()
        {
            Console.WriteLine("\nDrücke Enter zum Zurückkehren zum Menü...");
            Console.ReadLine();
            Console.Clear();
            ShowHeader();
        }

   
        static void SpieleSignalTon()
        {
            try
            {
                for (int i = 0; i < 3; i++)
                {
                    Console.Beep(2000, 180); 
                    Thread.Sleep(80); // kurze Pause zwischen den Tönen
                }
            }
            catch
            {
                // Fallback: System-Beep mehrmals
                for (int i = 0; i < 3; i++)
                {
                    Console.Write("\a");
                    Thread.Sleep(80);
                }
            }
        }
    }
}


















