using System.Diagnostics.CodeAnalysis;
using ShippingRecorder.Entities.Db;
using Microsoft.EntityFrameworkCore;
using ShippingRecorder.Entities.Reporting;

namespace ShippingRecorder.Data
{
    [ExcludeFromCodeCoverage]
    public partial class ShippingRecorderDbContext : DbContext
    {
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Operator> Operators { get; set; }
        public virtual DbSet<Port> Ports { get; set; }
        public virtual DbSet<Sighting> Sightings { get; set; }
        public virtual DbSet<Vessel> Vessels { get; set; }
        public virtual DbSet<RegistrationHistory> RegistrationHistory { get; set; }
        public virtual DbSet<VesselType> VesselTypes { get; set; }
        public virtual DbSet<Voyage> Voyages { get; set; }
        public virtual DbSet<VoyageEvent> VoyageEvents { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<JobStatus> JobStatuses { get; set; }
        public virtual DbSet<LocationStatistics> LocationStatistics { get; set; }
        public virtual DbSet<SightingsByMonth> SightingsByMonth { get; set; }
        public virtual DbSet<MyVoyages> MyVoyages { get; set; }
        public virtual DbSet<OperatorStatistics> OperatorStatistics { get; set; }
        public virtual DbSet<VesselTypeStatistics> VesselTypeStatistics { get; set; }
        public virtual DbSet<FlagStatistics> FlagStatistics { get; set; }

        public ShippingRecorderDbContext(DbContextOptions<ShippingRecorderDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Initialise the ShippingRecorder model
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LocationStatistics>().HasNoKey();
            modelBuilder.Entity<SightingsByMonth>().HasNoKey();
            modelBuilder.Entity<MyVoyages>().HasNoKey();
            modelBuilder.Entity<OperatorStatistics>().HasNoKey();
            modelBuilder.Entity<VesselTypeStatistics>().HasNoKey();
            modelBuilder.Entity<FlagStatistics>().HasNoKey();

            modelBuilder.Entity<Country>(entity =>
            {
                entity.ToTable("COUNTRY", tb =>
                {
                    tb.HasCheckConstraint("CK_Code_Length", "length(code) = 2");
                    tb.HasCheckConstraint("CK_Code_Characters", "code GLOB '[A-Z0-9]*'");
                });

                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Code).IsRequired().HasColumnName("code").HasMaxLength(2);
                entity.Property(e => e.Name).IsRequired().HasColumnName("name");
                entity.HasIndex(e => e.Code).IsUnique();
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.ToTable("LOCATION");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasColumnName("name");
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<Operator>(entity =>
            {
                entity.ToTable("OPERATOR");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasColumnName("name");
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<Port>(entity =>
            {
                entity.ToTable("PORT", tb =>
                {
                    tb.HasCheckConstraint("CK_Code_Length", "length(code) = 5");
                    tb.HasCheckConstraint("CK_Code_AlphaNumeric", "code GLOB '[A-Za-z0-9]*'");
                });

                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.CountryId).IsRequired().HasColumnName("country_id");
                entity.Property(e => e.Code).IsRequired().HasColumnName("code").HasMaxLength(5);
                entity.Property(e => e.Name).IsRequired().HasColumnName("name");

                entity.HasIndex(e => e.Code).IsUnique();

                entity.HasOne(e => e.Country).WithMany().HasForeignKey(e => e.CountryId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Sighting>(entity =>
            {
                entity.ToTable("SIGHTING");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.VesselId).IsRequired().HasColumnName("vessel_id");
                entity.Property(e => e.VoyageId).HasColumnName("voyage_id");
                entity.Property(e => e.LocationId).IsRequired().HasColumnName("location_id");
                entity.Property(e => e.Date).IsRequired().HasColumnName("date").HasColumnType("DATETIME");
                entity.Property(e => e.IsMyVoyage).HasColumnName("is_my_voyage");

                entity.HasOne(e => e.Vessel).WithMany().HasForeignKey(e => e.VesselId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Voyage).WithMany().HasForeignKey(e => e.VoyageId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Location).WithMany().HasForeignKey(e => e.LocationId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Vessel>(entity =>
            {
                entity.ToTable("VESSEL", tb =>
                {
                    tb.HasCheckConstraint("CK_IMO_Length", "length(imo) = 7");
                    tb.HasCheckConstraint("CK_IMO_Numeric", "imo GLOB '[0-9]*'");
                });

                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.IMO).IsRequired().HasColumnName("imo").HasMaxLength(7);
                entity.Property(e => e.Built).HasColumnName("built");
                entity.Property(e => e.Draught).HasColumnName("draught");
                entity.Property(e => e.Length).HasColumnName("length");
                entity.Property(e => e.Beam).HasColumnName("beam");
                entity.HasIndex(e => e.IMO).IsUnique();

                entity.HasMany(e => e.RegistrationHistory).WithOne().HasForeignKey(h => h.VesselId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<RegistrationHistory>(entity =>
            {
                entity.ToTable("REGISTRATION_HISTORY", tb =>
                {
                    tb.HasCheckConstraint("CK_Callsign_Characters", "callsign GLOB '[A-Z0-9]*'");
                    tb.HasCheckConstraint("CK_MMSI_Length","length(mmsi) = 9");
                    tb.HasCheckConstraint("CK_MMSI_Numeric", "mmsi GLOB '[0-9]*'");
                });

                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.VesselId).IsRequired().HasColumnName("vessel_id");
                entity.Property(e => e.VesselTypeId).IsRequired().HasColumnName("vessel_type_id");
                entity.Property(e => e.Name).IsRequired().HasColumnName("name");
                entity.Property(e => e.Callsign).IsRequired().HasColumnName("callsign");
                entity.Property(e => e.MMSI).IsRequired().HasColumnName("mmsi").HasMaxLength(9);
                entity.Property(e => e.FlagId).IsRequired().HasColumnName("flag_id");
                entity.Property(e => e.OperatorId).IsRequired().HasColumnName("operator_id");
                entity.Property(e => e.Tonnage).HasColumnName("tonnage");
                entity.Property(e => e.Passengers).HasColumnName("passengers");
                entity.Property(e => e.Crew).HasColumnName("crew");
                entity.Property(e => e.Decks).HasColumnName("decks");
                entity.Property(e => e.Cabins).HasColumnName("cabins");
                entity.Property(e => e.Date).IsRequired().HasColumnName("date").HasColumnType("DATETIME");
                entity.Property(e => e.IsActive).HasColumnName("is_active");

                entity.HasOne(e => e.Flag).WithMany().HasForeignKey(e => e.FlagId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Operator).WithMany().HasForeignKey(e => e.OperatorId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.VesselType).WithMany().HasForeignKey(e => e.VesselTypeId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<VesselType>(entity =>
            {
                entity.ToTable("VESSEL_TYPE");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.Name).IsRequired().HasColumnName("name");
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<Voyage>(entity =>
            {
                entity.ToTable("VOYAGE");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.OperatorId).IsRequired().HasColumnName("operator_id");
                entity.Property(e => e.Number).IsRequired().HasColumnName("number");

                entity.HasOne(e => e.Operator).WithMany().HasForeignKey(e => e.OperatorId).OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(e => e.Events).WithOne().HasForeignKey(p => p.VoyageId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<VoyageEvent>(entity =>
            {
                entity.ToTable("VOYAGE_EVENT");
                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.VoyageId).IsRequired().HasColumnName("voyage_id");
                entity.Property(e => e.EventType).IsRequired().HasColumnName("event_type");
                entity.Property(e => e.PortId).IsRequired().HasColumnName("port_id");
                entity.Property(e => e.Date).HasColumnName("arrival").HasColumnType("DATETIME");

                entity.HasOne(e => e.Port).WithMany().HasForeignKey(e => e.PortId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USER");

                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
                entity.Property(e => e.UserName).IsRequired().HasColumnName("UserName");
                entity.Property(e => e.Password).IsRequired().HasColumnName("Password");
                entity.HasIndex(e => e.UserName).IsUnique();
            });

            modelBuilder.Entity<JobStatus>(entity =>
            {
                entity.ToTable("JOB_STATUS");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Name).IsRequired().HasColumnName("name");
                entity.Property(e => e.Parameters).HasColumnName("parameters");
                entity.Property(e => e.Start).IsRequired().HasColumnName("start").HasColumnType("DATETIME");
                entity.Property(e => e.End).HasColumnName("end").HasColumnType("DATETIME");
                entity.Property(e => e.Error).HasColumnName("error");
            });
        }
    }
}
