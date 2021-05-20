using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace BulkMailSender.TableEntities
{
    public partial class BekoDBContext : DbContext
    {
        public BekoDBContext()
        {
        }

        public BekoDBContext(DbContextOptions<BekoDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual DbSet<TblAllTdsEmail> TblAllTdsEmails { get; set; }
        public virtual DbSet<TblRestructuring> TblRestructurings { get; set; }
        public virtual DbSet<TblTdsCertificate> TblTdsCertificates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=agssql16.hyd;Initial Catalog=VOLTASBEKO;Integrated Security=true;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Latin1_General_CI_AI");

            modelBuilder.Entity<AspNetRole>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetRoleClaim>(entity =>
            {
                entity.Property(e => e.RoleId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetUser>(entity =>
            {
                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaim>(entity =>
            {
                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogin>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserToken>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<TblAllTdsEmail>(entity =>
            {
                entity.HasKey(e => e.MailId)
                    .HasName("PK__TBL_All___09A874FA346BC6CB");

                entity.ToTable("TBL_All_Tds_Emails");

                entity.Property(e => e.MailId).HasColumnName("MailID");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Administator')");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.RestructuringKey)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.TdsMailId)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TdsPdfName).HasMaxLength(100);

                entity.Property(e => e.TdsPdfUrl)
                    .HasMaxLength(256)
                    .HasColumnName("TdsPdfURL");

                entity.Property(e => e.TdsUserName)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Tds)
                    .WithMany(p => p.TblAllTdsEmails)
                    .HasForeignKey(d => d.TdsId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_TBL_All_Tds_Emails_TdsId");
            });

            modelBuilder.Entity<TblRestructuring>(entity =>
            {
                entity.HasKey(e => e.Rid)
                    .HasName("Pk_Tbl_Restructuring_RID");

                entity.ToTable("Tbl_Restructuring");

                entity.Property(e => e.Rid).HasColumnName("RID");

                entity.Property(e => e.CreatedBy).HasMaxLength(20);

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.TdsEid).HasColumnName("TdsEId");

                entity.Property(e => e.UpdatedBy).HasMaxLength(20);

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.Property(e => e.VendorCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.VendorName)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.HasOne(d => d.TdsE)
                    .WithMany(p => p.TblRestructurings)
                    .HasForeignKey(d => d.TdsEid)
                    .HasConstraintName("Fk_TBL_All_Tds_Emails_MailId_Tbl_Restructuring");
            });

            modelBuilder.Entity<TblTdsCertificate>(entity =>
            {
                entity.HasKey(e => e.TdsId);

                entity.ToTable("TBL_Tds_Certificates");

                entity.Property(e => e.TdsId).HasColumnName("TdsID");

                entity.Property(e => e.IsIndividualEmailBody).HasDefaultValueSql("((0))");

                entity.Property(e => e.TdsCompleted).HasDefaultValueSql("((0))");

                entity.Property(e => e.TdsCreatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("('Administrator')");

                entity.Property(e => e.TdsCreatedOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.TdsEmailBody).IsRequired();

                entity.Property(e => e.TdsEmailCc)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TdsEmailCC");

                entity.Property(e => e.TdsEmailFrom)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TdsExcelName).HasMaxLength(100);

                entity.Property(e => e.TdsExcelUrl)
                    .HasMaxLength(500)
                    .HasColumnName("TdsExcelURL");

                entity.Property(e => e.TdsSubject)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.TdsTitle)
                    .HasMaxLength(256)
                    .IsUnicode(false);

                entity.Property(e => e.TdsTxnId)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasColumnName("TdsTxnID");

                entity.Property(e => e.TdsUpdatedBy)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.TdsUpdatedOn).HasColumnType("datetime");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
