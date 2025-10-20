using System.Collections.Generic;

namespace QuizApp2
{
    public class Frage
    {
        public int Id { get; set; }
        public string Typ { get; set; } = string.Empty; // Initialisierung um null-Warnungen zu vermeiden
        public string FrageText { get; set; } = string.Empty;
        public List<string> Antworten { get; set; } = new List<string>(); // Initialisierung
        public List<int> RichtigeAntworten { get; set; } = new List<int>(); // Initialisierung
        public string RichtigeAntwort { get; set; } = string.Empty;
    }
}


