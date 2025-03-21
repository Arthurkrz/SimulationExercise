﻿using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationExercise.Core.DTOS
{
    [IgnoreFirst(1)]
    [DelimitedRecord(",")]
    public class ReadingDTO
    {
        public ReadingDTO() { }

        public ReadingDTO(long sensorID, string sensorTypeName, string unit, long stationId, string stationName, int value,
               string province, string city, bool isHistoric, DateTime startDate, DateTime? stopDate, int utmNord,
               int utmEst, string latitude, string longitude)
        {
            SensorID = sensorID;
            SensorTypeName = sensorTypeName;
            Unit = unit;
            StationId = stationId;
            StationName = stationName;
            Value = value;
            Province = province;
            City = city;
            IsHistoric = isHistoric;
            StartDate = startDate;
            StopDate = stopDate;
            UtmNord = utmNord;
            UtmEst = utmEst;
            Latitude = latitude;
            Longitude = longitude;
        }

        public long SensorID { get; set; }
        public string SensorTypeName { get; set; }
        public string Unit { get; set; }
        public long StationId { get; set; }
        public string StationName { get; set; }
        public int Value { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public bool IsHistoric { get; set; }

        [FieldConverter(ConverterKind.Date, "dd/MM/yyyy HH:mm:ss")]
        public DateTime StartDate { get; set; }
        [FieldConverter(ConverterKind.Date, "dd/MM/yyyy HH:mm:ss")]
        public DateTime? StopDate { get; set; }

        public int UtmNord { get; set; }
        public int UtmEst { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Location { get; set; }
    }
}
