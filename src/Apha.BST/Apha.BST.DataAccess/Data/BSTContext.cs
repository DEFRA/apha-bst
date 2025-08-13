using System;
using System.Collections.Generic;
using Apha.BST.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Apha.BST.DataAccess.Data;

public partial class BstContext : DbContext
{
    public BstContext()
    {
    }

    public BstContext(DbContextOptions<BstContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> Auditlogs { get; set; }

    public virtual DbSet<AuditlogArchived> AuditlogArchiveds { get; set; }

    public virtual DbSet<Persons> Persons { get; set; }

    public virtual DbSet<Site> Sites { get; set; }

    public virtual DbSet<Training> Trainings { get; set; }

    public virtual DbSet<DataEntry> DataEntries { get; set; }

    public virtual DbSet<News> News { get; set; }

    public virtual DbSet<TblSite> TblSites { get; set; }

    public virtual DbSet<Trainees> Trainees { get; set; }

    public virtual DbSet<Trainers> Trainers { get; set; }

    public virtual DbSet<Trainings> TblTrainings { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VlaLoc> Vlalocs { get; set; }   
    public DbSet<SiteTrainee> SiteTrainees { get; set; }
    public DbSet<TraineeTrainer> Traines { get; set; }
    public DbSet<TrainerTraining> TrainerTrainings { get; set; }
    public DbSet<TrainerHistory> TrainerHistorys { get; set; }
    public DbSet<TrainerTrained> TrainerTraineds { get; set; }
    public DbSet<UserRoleInfo> UserRoleInfos { get; set; }
    public DbSet<AddPerson> AddPersons { get; set; }
    public DbSet<PersonSiteLookup> PersonSiteLookups { get; set; }
    public DbSet<PersonLookup> PersonLookups { get; set; }
    public DbSet<PersonDetail> PersonDetails { get; set; }
    public virtual DbSet<UserView> UserViews { get; set; }
    public virtual DbSet<VlaLocView> VlaLocViews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Auditlog");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Parameters).HasColumnType("text");
            entity.Property(e => e.Procedure)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TransactionType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.User)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<AuditlogArchived>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("AuditlogArchived");

            entity.Property(e => e.Date).HasColumnType("datetime");
            entity.Property(e => e.Parameters).HasColumnType("text");
            entity.Property(e => e.Procedure)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.TransactionType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.User)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Persons>(entity =>
        {
            entity.HasKey(e => e.PersonId);

            entity.ToTable("tPerson");

            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.LocationId)
                .HasMaxLength(11)
                .HasColumnName("LocationID");
            entity.Property(e => e.Person).HasMaxLength(100);
            entity.Property(e => e.VlalocationId).HasColumnName("VLALocationID");
        });

        modelBuilder.Entity<Site>(entity =>
        {
            entity
                //.HasNoKey()
                .ToTable("tSites");
            entity.HasKey(e => e.PlantNo);
            entity.Property(e => e.AddressCounty).HasMaxLength(50);
            entity.Property(e => e.AddressLine1).HasMaxLength(50);
            entity.Property(e => e.AddressLine2).HasMaxLength(50);
            entity.Property(e => e.AddressPostCode).HasMaxLength(50);
            entity.Property(e => e.AddressTown).HasMaxLength(50);
            entity.Property(e => e.Ahvla)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("AHVLA");
            entity.Property(e => e.Fax).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.PlantNo).HasMaxLength(11);
            entity.Property(e => e.Telephone).HasMaxLength(50);
        });

        modelBuilder.Entity<Training>(entity =>
        {
            entity.HasKey(e => new { e.PersonId, e.TrainingAnimal, e.TrainingDateTime }).HasName("PK_tTraining_1");

            entity.ToTable("tTraining");

            entity.Property(e => e.PersonId).HasColumnName("PersonID");
            entity.Property(e => e.TrainingAnimal).HasMaxLength(50);
            entity.Property(e => e.TrainingDateTime).HasColumnType("datetime");
            entity.Property(e => e.TrainerId).HasColumnName("TrainerID");
            entity.Property(e => e.TrainingType).HasMaxLength(50);
        });

        modelBuilder.Entity<DataEntry>(entity =>
        {
            entity.HasKey(e => e.ActiveView);

            entity.ToTable("tblDataEntry");

            entity.Property(e => e.ActiveView).ValueGeneratedNever();
            entity.Property(e => e.ActiveViewName).HasMaxLength(50);
            entity.Property(e => e.CanWrite)
                .HasMaxLength(1)
                .IsFixedLength();
        });

        modelBuilder.Entity<News>(entity =>
        {
            entity.HasKey(e => e.DatePublished);

            entity.ToTable("tblNews");

            entity.Property(e => e.DatePublished)
                .HasColumnType("smalldatetime")
                .HasColumnName("Date Published");
            entity.Property(e => e.Author).HasMaxLength(50);
            entity.Property(e => e.NewsContent).HasColumnName("News Content");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.AuthorNavigation).WithMany(p => p.News)
                .HasForeignKey(d => d.Author)
                .HasConstraintName("FK_tblNews_tblUser");
        });

        modelBuilder.Entity<TblSite>(entity =>
        {
            entity.HasKey(e => e.PlantNo);

            entity.ToTable("tblSites");

            entity.Property(e => e.PlantNo)
                .HasMaxLength(11)
                .HasColumnName("Plant_No");
            entity.Property(e => e.AddressCounty)
                .HasMaxLength(50)
                .HasColumnName("Address_County");
            entity.Property(e => e.AddressLine1)
                .HasMaxLength(50)
                .HasColumnName("Address_Line1");
            entity.Property(e => e.AddressLine2)
                .HasMaxLength(50)
                .HasColumnName("Address_Line2");
            entity.Property(e => e.AddressPostCode)
                .HasMaxLength(50)
                .HasColumnName("Address_PostCode");
            entity.Property(e => e.AddressTown)
                .HasMaxLength(50)
                .HasColumnName("Address_Town");
            entity.Property(e => e.Fax).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.Telephone).HasMaxLength(50);
        });

        modelBuilder.Entity<Trainees>(entity =>
        {
            entity.HasKey(e => e.TraineeNo);

            entity.ToTable("tblTrainee");

            entity.Property(e => e.TraineeNo).HasColumnName("Trainee_No");
            entity.Property(e => e.PlantNo)
                .HasMaxLength(11)
                .HasColumnName("Plant_No");
            entity.Property(e => e.Trainee).HasMaxLength(100);

            entity.HasOne(d => d.PlantNoNavigation).WithMany(p => p.TblTrainees)
                .HasForeignKey(d => d.PlantNo)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblTrainee_tblSites");
        });

        modelBuilder.Entity<Trainers>(entity =>
        {
            entity.HasKey(e => e.TrainerId);

            entity.ToTable("tblTrainers");

            entity.Property(e => e.TrainerId).HasColumnName("Trainer_ID");
            entity.Property(e => e.LocId)
                .HasMaxLength(50)
                .HasColumnName("Loc_ID");
            entity.Property(e => e.PersonId).HasColumnName("Person_ID");
            entity.Property(e => e.Trainer).HasMaxLength(100);

            entity.HasOne(d => d.Loc).WithMany(p => p.TblTrainers)
                .HasForeignKey(d => d.LocId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_tblTrainers_tblVLALoc");
        });

        modelBuilder.Entity<Trainings>(entity =>
        {
            entity.HasKey(e => new { e.TraineeNo, e.DateTrained, e.SpeciesTrained }).HasName("PK_tblTraining_1");

            entity.ToTable("tblTraining");

            entity.Property(e => e.TraineeNo).HasColumnName("Trainee_No");
            entity.Property(e => e.DateTrained)
                .HasMaxLength(255)
                .HasColumnName("Date_Trained");
            entity.Property(e => e.SpeciesTrained)
                .HasMaxLength(50)
                .HasColumnName("Species_Trained");
            entity.Property(e => e.TrainerId).HasColumnName("Trainer_ID");

            entity.HasOne(d => d.TraineeNoNavigation).WithMany(p => p.TblTrainings)
                .HasForeignKey(d => d.TraineeNo)
                .HasConstraintName("FK_tblTraining_tblTrainee");

            entity.HasOne(d => d.Trainer).WithMany(p => p.TblTrainings)
                .HasForeignKey(d => d.TrainerId)
                .HasConstraintName("FK_tblTraining_tblTrainers");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("tblUser");

            entity.Property(e => e.UserId)
                .HasMaxLength(50)
                .HasColumnName("UserID");
            entity.Property(e => e.UserLoc)
                .HasMaxLength(50)
                .HasColumnName("User_Loc");
            entity.Property(e => e.UserName).HasMaxLength(100);
        });

        modelBuilder.Entity<VlaLoc>(entity =>
        {
            entity.HasKey(e => e.LocId);

            entity.ToTable("tblVLALoc");

            entity.Property(e => e.LocId)
                .HasMaxLength(50)
                .HasColumnName("Loc_ID");
            entity.Property(e => e.Ahvla)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("AHVLA");
            entity.Property(e => e.VlaLocation)
                .HasMaxLength(50)
                .HasColumnName("VLA_Location");
        });

        modelBuilder.Entity<SiteTrainee>().HasNoKey();
        modelBuilder
            .Entity<TraineeTrainer>()
            .HasNoKey()  // Redundant if you use [Keyless], but explicit
            .ToView(null);  // Prevents EF from trying to map to a table or view

        modelBuilder.Entity<TrainerTraining>().HasNoKey();
        modelBuilder.Entity<TrainerHistory>().HasNoKey();
        modelBuilder.Entity<TrainerTrained>().HasNoKey();
        modelBuilder.Entity<PersonDetail>().HasNoKey();
        modelBuilder.Entity<VlaLocView>().HasNoKey().ToView("vwVlaLocations");
        modelBuilder.Entity<UserView>().HasNoKey().ToView("vwUsers");

    }
  
}
