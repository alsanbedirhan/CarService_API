﻿using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CarService_API.Models.DB;

public partial class ModelContext : DbContext
{
    IConfiguration _configuration;
    public ModelContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ModelContext(DbContextOptions<ModelContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Companywork> Companyworks { get; set; }

    public virtual DbSet<Companyworkdetail> Companyworkdetails { get; set; }

    public virtual DbSet<Make> Makes { get; set; }

    public virtual DbSet<Makemodel> Makemodels { get; set; }

    public virtual DbSet<Requestlog> Requestlogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Usercar> Usercars { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      => optionsBuilder.UseLazyLoadingProxies().UseOracle(_configuration.GetConnectionString(_configuration["DATABASE"]));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("CAR")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008289");

            entity.ToTable("COMPANIES");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Active)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("'Y'\r\n")
                .IsFixedLength()
                .HasColumnName("ACTIVE");
            entity.Property(e => e.Companyname)
                .HasMaxLength(500)
                .HasDefaultValueSql("1")
                .HasColumnName("COMPANYNAME");
        });

        modelBuilder.Entity<Companywork>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008323");

            entity.ToTable("COMPANYWORKS");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Active)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("'Y'")
                .IsFixedLength()
                .HasColumnName("ACTIVE");
            entity.Property(e => e.Cdate)
                .HasPrecision(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("CDATE");
            entity.Property(e => e.Companyid)
                .HasColumnType("NUMBER")
                .HasColumnName("COMPANYID");
            entity.Property(e => e.Cuser)
                .HasColumnType("NUMBER")
                .HasColumnName("CUSER");
            entity.Property(e => e.Explanation)
                .HasMaxLength(500)
                .HasColumnName("EXPLANATION");
            entity.Property(e => e.Isdone)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("'N'")
                .IsFixedLength()
                .HasColumnName("ISDONE");
            entity.Property(e => e.Isout)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("'N'")
                .IsFixedLength()
                .HasColumnName("ISOUT");
            entity.Property(e => e.Usercarid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERCARID");

            entity.HasOne(d => d.Company).WithMany(p => p.Companyworks)
                .HasForeignKey(d => d.Companyid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("COMPANYWORKS_COMPANIES_FK");

            entity.HasOne(d => d.Usercar).WithMany(p => p.Companyworks)
                .HasForeignKey(d => d.Usercarid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("COMPANYWORKS_USERCARS_FK");
        });

        modelBuilder.Entity<Companyworkdetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008332");

            entity.ToTable("COMPANYWORKDETAILS");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Active)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("'Y' ")
                .IsFixedLength()
                .HasColumnName("ACTIVE");
            entity.Property(e => e.Cdate)
                .HasPrecision(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("CDATE");
            entity.Property(e => e.Companyworkid)
                .HasColumnType("NUMBER")
                .HasColumnName("COMPANYWORKID");
            entity.Property(e => e.Cuser)
                .HasColumnType("NUMBER")
                .HasColumnName("CUSER");
            entity.Property(e => e.Explanation)
                .HasMaxLength(500)
                .HasColumnName("EXPLANATION");
            entity.Property(e => e.Price)
                .HasDefaultValueSql("0\r\n")
                .HasColumnType("NUMBER(15,2)")
                .HasColumnName("PRICE");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.Companywork).WithMany(p => p.Companyworkdetails)
                .HasForeignKey(d => d.Companyworkid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("COMPANYWORKDETAILS_COMPANYWORKS_FK");

            entity.HasOne(d => d.User).WithMany(p => p.Companyworkdetails)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("COMPANYWORKDETAILS_USERS_FK");
        });

        modelBuilder.Entity<Make>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008305");

            entity.ToTable("MAKES");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Cdate)
                .HasPrecision(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("CDATE");
            entity.Property(e => e.Cuser)
                .HasColumnType("NUMBER")
                .HasColumnName("CUSER");
            entity.Property(e => e.Explanation)
                .HasMaxLength(100)
                .HasColumnName("EXPLANATION");
        });

        modelBuilder.Entity<Makemodel>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008309");

            entity.ToTable("MAKEMODELS");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Cdate)
                .HasPrecision(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("CDATE");
            entity.Property(e => e.Cuser)
                .HasColumnType("NUMBER")
                .HasColumnName("CUSER");
            entity.Property(e => e.Explanation)
                .HasMaxLength(500)
                .HasColumnName("EXPLANATION");
            entity.Property(e => e.Makeid)
                .HasColumnType("NUMBER")
                .HasColumnName("MAKEID");

            entity.HasOne(d => d.Make).WithMany(p => p.Makemodels)
                .HasForeignKey(d => d.Makeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("MAKEMODELS_MAKES_FK");
        });

        modelBuilder.Entity<Requestlog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008280");

            entity.ToTable("REQUESTLOGS");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Deviceid)
                .HasMaxLength(100)
                .HasColumnName("DEVICEID");
            entity.Property(e => e.Hostip)
                .HasMaxLength(100)
                .HasColumnName("HOSTIP");
            entity.Property(e => e.Rdate)
                .HasPrecision(6)
                .HasColumnName("RDATE");
            entity.Property(e => e.UserAgent)
                .HasMaxLength(1000)
                .HasColumnName("USER_AGENT");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.User).WithMany(p => p.Requestlogs)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("REQUESTLOGS_USERS_FK");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008228");

            entity.ToTable("USERS");

            entity.HasIndex(e => e.Mail, "USERS_UNIQUE").IsUnique();

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Active)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("'Y' ")
                .IsFixedLength()
                .HasColumnName("ACTIVE");
            entity.Property(e => e.Cdate)
                .HasPrecision(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("CDATE");
            entity.Property(e => e.Companyid)
                .HasColumnType("NUMBER")
                .HasColumnName("COMPANYID");
            entity.Property(e => e.Cuser)
                .HasColumnType("NUMBER")
                .HasColumnName("CUSER");
            entity.Property(e => e.Mail)
                .HasMaxLength(100)
                .HasColumnName("MAIL");
            entity.Property(e => e.Name)
                .HasMaxLength(200)
                .HasColumnName("NAME");
            entity.Property(e => e.Passhash)
                .HasMaxLength(512)
                .IsUnicode(false)
                .HasColumnName("PASSHASH");
            entity.Property(e => e.Passsalt)
                .HasMaxLength(512)
                .HasColumnName("PASSSALT");
            entity.Property(e => e.Surname)
                .HasMaxLength(200)
                .HasColumnName("SURNAME");
            entity.Property(e => e.Usertype)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("USERTYPE");

            entity.HasOne(d => d.Company).WithMany(p => p.Users)
                .HasForeignKey(d => d.Companyid)
                .HasConstraintName("USERS_COMPANIES_FK");
        });

        modelBuilder.Entity<Usercar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008315");

            entity.ToTable("USERCARS");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Active)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("'Y' ")
                .IsFixedLength()
                .HasColumnName("ACTIVE");
            entity.Property(e => e.Cdate)
                .HasPrecision(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("CDATE");
            entity.Property(e => e.Cuser)
                .HasColumnType("NUMBER")
                .HasColumnName("CUSER");
            entity.Property(e => e.Explanation)
                .HasMaxLength(100)
                .HasColumnName("EXPLANATION");
            entity.Property(e => e.Makemodelid)
                .HasColumnType("NUMBER")
                .HasColumnName("MAKEMODELID");
            entity.Property(e => e.Plate)
                .HasMaxLength(100)
                .HasColumnName("PLATE");
            entity.Property(e => e.Pyear)
                .HasPrecision(5)
                .HasColumnName("PYEAR");
            entity.Property(e => e.Uniquekey)
                .HasMaxLength(500)
                .HasColumnName("UNIQUEKEY");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.Makemodel).WithMany(p => p.Usercars)
                .HasForeignKey(d => d.Makemodelid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("USERCARS_MAKEMODELS_FK");

            entity.HasOne(d => d.User).WithMany(p => p.Usercars)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("USERCARS_USERS_FK");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
