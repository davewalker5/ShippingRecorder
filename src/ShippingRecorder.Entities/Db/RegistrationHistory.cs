using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ShippingRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class RegistrationHistory : ShippingRecorderEntityBase
    {
        [Key]
        public long Id { get; set; }

        [DisplayName("Vessel")]
        [Required(ErrorMessage = "You must specify a vessel")]
        public long VesselId { get; set; }

        [DisplayName("Type")]
        [Required(ErrorMessage = "You must specify a vessel type")]
        public long VesselTypeId { get; set; }

        [DisplayName("Flag")]
        [Required(ErrorMessage = "You must specify a flag")]
        public long FlagId { get; set; }

        [DisplayName("Operator")]
        [Required(ErrorMessage = "You must specify an operator")]
        public long OperatorId { get; set; }

        [DisplayName("From")]
        [Required(ErrorMessage = "You must provide a date")]
        public DateTime Date { get; set; }

        [DisplayName("Name")]
        [Required(ErrorMessage = "You must provide a name")]
        public string Name { get; set; }

        [DisplayName("Callsign")]
        [Required(ErrorMessage = "You must provide a callsign")]
        public string Callsign { get; set; }

        [DisplayName("MMSI")]
        [Required(ErrorMessage = "You must provide an MMSI")]
        public string MMSI { get; set; }

        [DisplayName("Tonnage")]
        public int? Tonnage { get; set; }

        [DisplayName("Passengers")]
        public int? Passengers { get; set; }

        [DisplayName("Crew")]
        public int? Crew { get; set; }

        [DisplayName("Decks")]
        public int? Decks { get; set; }

        [DisplayName("Cabins")]
        public int? Cabins { get; set; }

        public bool IsActive { get; set; }

        public virtual Country Flag { get; set; }
        public virtual Operator Operator { get; set; }
        public virtual VesselType VesselType { get; set; }

        public override string ToString()
            => $"Vessel ID = {VesselId}, " +
                $"Vessel Type = {VesselType}, " +
                $"Flag = {Flag}, " +
                $"Operator = {Operator}, " +
                $"Name = {Name}, " +
                $"Callsign = {Callsign}, " +
                $"MMSI = {MMSI}" +
                $"Date = {Date}, " +
                $"Active = {IsActive}";
    }
}