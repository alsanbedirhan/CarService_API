using System;
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

    public virtual DbSet<Avaliabledate> Avaliabledates { get; set; }

    public virtual DbSet<Avaliableday> Avaliabledays { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Make> Makes { get; set; }

    public virtual DbSet<Makemodel> Makemodels { get; set; }

    public virtual DbSet<Requestlog> Requestlogs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Userauth> Userauths { get; set; }

    public virtual DbSet<Usercar> Usercars { get; set; }

    public virtual DbSet<Userdate> Userdates { get; set; }

    public virtual DbSet<Userdateoffer> Userdateoffers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      => optionsBuilder.UseLazyLoadingProxies().UseOracle(_configuration.GetConnectionString(_configuration["DATABASE"]));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasDefaultSchema("CAR")
            .UseCollation("USING_NLS_COMP");

        modelBuilder.Entity<Avaliabledate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008249");

            entity.ToTable("AVALIABLEDATES");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Avaliablecount)
                .HasPrecision(6)
                .HasDefaultValueSql("0\r\n")
                .HasColumnName("AVALIABLECOUNT");
            entity.Property(e => e.Avaliabledate1)
                .HasColumnType("DATE")
                .HasColumnName("AVALIABLEDATE");
            entity.Property(e => e.Companyid)
                .HasColumnType("NUMBER")
                .HasColumnName("COMPANYID");

            entity.HasOne(d => d.Company).WithMany(p => p.Avaliabledates)
                .HasForeignKey(d => d.Companyid)
                .HasConstraintName("AVALIABLEDATES_COMPANIES_FK");
        });

        modelBuilder.Entity<Avaliableday>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008246");

            entity.ToTable("AVALIABLEDAYS");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Avaliablecount)
                .HasPrecision(6)
                .HasDefaultValueSql("0\r\n")
                .HasColumnName("AVALIABLECOUNT");
            entity.Property(e => e.Companyid)
                .HasColumnType("NUMBER")
                .HasColumnName("COMPANYID");
            entity.Property(e => e.Daykey)
                .HasPrecision(1)
                .HasColumnName("DAYKEY");

            entity.HasOne(d => d.Company).WithMany(p => p.Avaliabledays)
                .HasForeignKey(d => d.Companyid)
                .HasConstraintName("AVALIABLEDAYS_COMPANIES_FK");
        });

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

        modelBuilder.Entity<Userauth>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008284");

            entity.ToTable("USERAUTHS");

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
            entity.Property(e => e.Authbit)
                .HasPrecision(2)
                .HasDefaultValueSql("0\r\n")
                .HasColumnName("AUTHBIT");
            entity.Property(e => e.Cdate)
                .HasPrecision(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("CDATE");
            entity.Property(e => e.Cuser)
                .HasColumnType("NUMBER")
                .HasColumnName("CUSER");
            entity.Property(e => e.Udate)
                .HasPrecision(6)
                .HasColumnName("UDATE");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");
            entity.Property(e => e.Uuser)
                .HasColumnType("NUMBER")
                .HasColumnName("UUSER");

            entity.HasOne(d => d.User).WithMany(p => p.Userauths)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("USERAUTHS_USERS_FK");
        });

        modelBuilder.Entity<Usercar>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008315");

            entity.ToTable("USERCARS");

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
            entity.Property(e => e.Makemodelid)
                .HasColumnType("NUMBER")
                .HasColumnName("MAKEMODELID");
            entity.Property(e => e.Plate)
                .HasMaxLength(100)
                .HasColumnName("PLATE");
            entity.Property(e => e.Pyear)
                .HasPrecision(4)
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

        modelBuilder.Entity<Userdate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008262");

            entity.ToTable("USERDATES");

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
            entity.Property(e => e.Approved)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("'N'")
                .IsFixedLength()
                .HasColumnName("APPROVED");
            entity.Property(e => e.Carid)
                .HasColumnType("NUMBER")
                .HasColumnName("CARID");
            entity.Property(e => e.Carok)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("'N' ")
                .IsFixedLength()
                .HasColumnName("CAROK");
            entity.Property(e => e.Cdate)
                .HasPrecision(6)
                .HasDefaultValueSql("CURRENT_TIMESTAMP \r\n")
                .HasColumnName("CDATE");
            entity.Property(e => e.Datevalue)
                .HasColumnType("DATE")
                .HasColumnName("DATEVALUE");
            entity.Property(e => e.Explanation)
                .HasMaxLength(1000)
                .HasColumnName("EXPLANATION");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.User).WithMany(p => p.Userdates)
                .HasForeignKey(d => d.Userid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("USERDATES_USERS_FK");
        });

        modelBuilder.Entity<Userdateoffer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("SYS_C008272");

            entity.ToTable("USERDATEOFFERS");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("NUMBER")
                .HasColumnName("ID");
            entity.Property(e => e.Astatus)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasDefaultValueSql("'W'")
                .IsFixedLength()
                .HasColumnName("ASTATUS");
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
                .HasMaxLength(1000)
                .HasColumnName("EXPLANATION");
            entity.Property(e => e.Userdateid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERDATEID");
            entity.Property(e => e.Userid)
                .HasColumnType("NUMBER")
                .HasColumnName("USERID");

            entity.HasOne(d => d.Company).WithMany(p => p.Userdateoffers)
                .HasForeignKey(d => d.Companyid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("USERDATEOFFERS_COMPANIES_FK");

            entity.HasOne(d => d.Userdate).WithMany(p => p.Userdateoffers)
                .HasForeignKey(d => d.Userdateid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("USERDATEOFFERS_USERDATES_FK");

            entity.HasOne(d => d.User).WithMany(p => p.Userdateoffers)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("USERDATEOFFERS_USERS_FK");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
