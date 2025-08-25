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
    public DbSet<SiteReport> SiteReports { get; set; } = default!;
    public DbSet<TrainerReport> TrainerReports { get; set; } = default!;
    public DbSet<PeopleReport> PeopleReports { get; set; } = default!;
    public DbSet<TrainingReport> TrainingReports { get; set; } = default!;
    public DbSet<AphaReport> AphaReports { get; set; } = default!;
    public virtual DbSet<StoredProcedureList> StoredProcedureLists { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BstContext).Assembly);
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
        modelBuilder.Entity<SiteReport>().HasNoKey();
        modelBuilder.Entity<TrainerReport>().HasNoKey();
        modelBuilder.Entity<PeopleReport>().HasNoKey();
        modelBuilder.Entity<TrainingReport>().HasNoKey();
        modelBuilder.Entity<AphaReport>().HasNoKey();
        modelBuilder.Entity<StoredProcedureList>().HasNoKey();
    }
  
}
