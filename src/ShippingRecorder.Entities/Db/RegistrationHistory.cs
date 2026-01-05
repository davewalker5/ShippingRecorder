using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using ShippingRecorder.Entities.Attributes;

namespace ShippingRecorder.Entities.Db
{
    [ExcludeFromCodeCoverage]
    public class RegistrationHistory : ShippingRecorderEntityBase, IEquatable<RegistrationHistory>
    {
        [Key]
        public long Id { get; set; }

        [DisplayName("Vessel")]
        [Required(ErrorMessage = "You must specify a vessel")]
        public long VesselId { get; set; }

        [DisplayName("Type")]
        [LongRange(1, "You must specify a vessel type")]
        [Required(ErrorMessage = "You must specify a vessel type")]
        public long VesselTypeId { get; set; }

        [DisplayName("Flag")]
        [LongRange(1, "You must specify a flag")]
        [Required(ErrorMessage = "You must specify a flag")]
        public long FlagId { get; set; }

        [DisplayName("Operator")]
        [LongRange(1, "You must specify an operator")]
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
        [StringLength(9, MinimumLength = 9, ErrorMessage = "IMO must be 7 digits long")]
        [RegularExpression(@"^\d+$", ErrorMessage = "IMO must contain digits only")]
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

        /// <summary>
        /// Override "ToString" to return the property valies
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Strongly-typed equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(RegistrationHistory other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return  (Id == other.Id) &&
                    (VesselTypeId == other.VesselTypeId) &&
                    (FlagId == other.FlagId) &&
                    (OperatorId == other.OperatorId) &&
                    (Callsign == other.Callsign) &&
                    (Name == other.Name) &&
                    (Tonnage == other.Tonnage) &&
                    (Passengers == other.Passengers) &&
                    (Crew == other.Crew) &&
                    (Decks == other.Decks) &&
                    (Cabins == other.Cabins) &&
                    (IsActive == other.IsActive);
        }

        /// <summary>
        /// Object.Equals equality override
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            => Equals(obj as RegistrationHistory);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Id);
            hash.Add(VesselTypeId);
            hash.Add(FlagId);
            hash.Add(OperatorId);
            hash.Add(Callsign);
            hash.Add(Name);
            hash.Add(Tonnage);
            hash.Add(Passengers);
            hash.Add(Crew);
            hash.Add(Decks);
            hash.Add(Cabins);
            hash.Add(IsActive);
            return hash.ToHashCode();
        }

        /// <summary>
        /// == operator overload
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(RegistrationHistory left, RegistrationHistory right)
            => Equals(left, right);

        /// <summary>
        /// != operator overload
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(RegistrationHistory left, RegistrationHistory right)
            => !Equals(left, right);
    }
}