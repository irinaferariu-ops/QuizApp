namespace QuizApp2
{
    // Modell für Benutzerdaten, serialisierbar für Persistenz
    public class Benutzer
    {
        public string Name { get; set; } = string.Empty;
        public string Stadt { get; set; } = string.Empty;
        public string Datum { get; set; } = string.Empty;
        public string Niveau { get; set; } = string.Empty;
        public double Punktestand { get; set; } = 0.0;

        public Benutzer() { }

        public Benutzer(string name, string stadt, string datum, string niveau)
        {
            Name = name;
            Stadt = stadt;
            Datum = datum;
            Niveau = niveau;
        }

        public override string ToString()
        {
            return $"{Name} ({Stadt}) - {Datum} - Niveau: {Niveau} - Punkte: {Punktestand:0.##}";
        }
    }
}
