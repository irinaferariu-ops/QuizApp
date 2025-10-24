using System.Collections.Generic;

namespace QuizApp2
{
    /// <summary>
    /// Klasse die eine Frage repräsentiert
    /// Wird aus der question.json Datei geladen
    /// </summary>
    public class Frage
    {
        // Eindeutige Nummer der Frage
        public int Id { get; set; }
        
        // Typ der Frage: "Offen" oder "MultipleChoice"
        public string Typ { get; set; } = string.Empty;
        
        // Der Text der Frage
        public string FrageText { get; set; } = string.Empty;
        
        // Liste der Antwortoptionen (nur bei MultipleChoice)
        public List<string> Antworten { get; set; } = new List<string>();
        
        // Liste der richtigen Antwort-Indizes (nur bei MultipleChoice)
        // z.B. [1] bedeutet die zweite Antwort ist richtig
        public List<int> RichtigeAntworten { get; set; } = new List<int>();
        
        // Die richtige Antwort als Text (nur bei offenen Fragen)
        public string RichtigeAntwort { get; set; } = string.Empty;
    }
}


