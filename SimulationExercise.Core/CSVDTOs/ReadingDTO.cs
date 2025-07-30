using FileHelpers;

namespace SimulationExercise.Core.CSVDTOs
{    
    [DelimitedRecord(",")]
    public class ReadingDTO
    {
        public ReadingDTO() { }

        public ReadingDTO(long idSensore, string nomeTipoSensore, string unitaMisura, long idStazione, string nomeStazione, int quota,
               string provincia, string comune, string storico, DateTime dataStart, DateTime? dataStop, int utmNord,
               int utmEst, string latitude, string longitude, string location)
        {
            IdSensore = idSensore;
            NomeTipoSensore = nomeTipoSensore;
            UnitaMisura = unitaMisura;
            Idstazione = idStazione;
            NomeStazione = nomeStazione;
            Quota = quota;
            Provincia = provincia;
            Comune = comune;
            Storico = storico;
            DataStart = dataStart;
            DataStop = dataStop;
            Utm_Nord = utmNord;
            UTM_Est = utmEst;
            lat = latitude;
            lng = longitude;
            Location = location;
        }

        public long IdSensore { get; set; }
        public string NomeTipoSensore { get; set; }
        public string UnitaMisura { get; set; }
        public long Idstazione { get; set; }
        public string NomeStazione { get; set; }
        public int Quota { get; set; }
        public string Provincia { get; set; }
        public string Comune { get; set; }
        public string Storico { get; set; }

        [FieldConverter(ConverterKind.Date, "dd/MM/yyyy")]
        public DateTime DataStart { get; set; }
        [FieldConverter(ConverterKind.Date, "dd/MM/yyyy")]
        public DateTime? DataStop { get; set; }

        public int Utm_Nord { get; set; }
        public int UTM_Est { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string Location { get; set; }
    }
}
