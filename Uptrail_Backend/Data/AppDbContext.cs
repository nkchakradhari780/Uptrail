using Microsoft.EntityFrameworkCore;
using Uptrail_Backend.Models.Entities;

namespace Uptrail_Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<PlatformAdmin> PlatformAdmins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ==========================================
            // COMPANY
            // ==========================================

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.Property(c => c.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(c => c.PhoneNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(c => c.Address)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(c => c.Website)
                    .HasMaxLength(2048);

                entity.Property(c => c.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(r => r.CreatedAt)
                   .IsRequired()
                   .HasPrecision(3);

                entity.Property(r => r.UpdatedAt)
                    .HasPrecision(3);

                entity.HasOne(c => c.PlatformAdmin)
                    .WithMany(pa => pa.Companies)
                    .HasForeignKey(c => c.PlatformAdminId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(c => c.PlatformAdminId)
                    .HasDatabaseName("IX_Companies_PlatformAdminId");

                // Unique company email
                entity.HasIndex(c => c.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Companies_Email");
            });


            // ==========================================
            // PLATFORM ADMIN
            // ==========================================

            modelBuilder.Entity<PlatformAdmin>(entity =>
            {
                entity.HasKey(pa => pa.Id);

                entity.Property(pa => pa.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(pa => pa.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(pa => pa.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(pa => pa.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(pa => pa.CreatedAt)
                    .IsRequired()
                    .HasPrecision(3);

                entity.Property(pa => pa.UpdatedAt)
                    .HasPrecision(3);

                // A platform admin is assigned to a role.
                entity.HasOne(pa => pa.Role)
                    .WithMany()
                    .HasForeignKey(pa => pa.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Platform-admin email addresses are globally unique.
                entity.HasIndex(pa => pa.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_PlatformAdmins_Email");

                entity.HasIndex(pa => pa.RoleId)
                    .HasDatabaseName("IX_PlatformAdmins_RoleId");
            });


            // ==========================================
            // ROLE
            // ==========================================

            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(r => r.CreatedAt)
                    .IsRequired()
                    .HasPrecision(3);

                entity.Property(r => r.UpdatedAt)
                    .HasPrecision(3);

                entity.Property(r => r.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(r => r.IsPlatformAdmin)
                    .IsRequired()
                    .HasDefaultValue(false);
               
                // Role belongs to a Company
                entity.HasOne(r => r.Company)
                    .WithMany(c => c.Roles)
                    .HasForeignKey(r => r.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Composite key for company-level role validation
                entity.HasAlternateKey(r => new { r.Id, r.CompanyId });

                // Prevent duplicate role names within the same company
                entity.HasIndex(r => new { r.CompanyId, r.Name })
                    .IsUnique()
                    .HasDatabaseName("IX_Roles_CompanyId_Name");
            });


            // ==========================================
            // USER
            // ==========================================

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(u => u.Email)
                    .IsRequired()
                    .HasMaxLength(256);

                entity.Property(u => u.PasswordHash)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(u => u.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(u => u.CreatedAt)
                    .IsRequired()
                    .HasPrecision(3);
                   
                entity.Property(u => u.UpdatedAt)
                    .HasPrecision(3);

                // User belongs to a Company
                entity.HasOne(u => u.Company)
                    .WithMany(c => c.Users)
                    .HasForeignKey(u => u.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                // User belongs to a Role within the same Company
                entity.HasOne(u => u.Role)
                    .WithMany()
                    .HasForeignKey(u => new { u.RoleId, u.CompanyId })
                    .HasPrincipalKey(r => new { r.Id, r.CompanyId })
                    .OnDelete(DeleteBehavior.Restrict);

                // Email must be unique within a company
                entity.HasIndex(u => new { u.CompanyId, u.Email })
                    .IsUnique()
                    .HasDatabaseName("IX_Users_CompanyId_Email");

                // Indexes for frequently queried foreign keys
                entity.HasIndex(u => u.RoleId)
                    .HasDatabaseName("IX_Users_RoleId");
            });


            // ==========================================
            // REFRESH TOKEN
            // ==========================================

            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(rt => rt.Id);

                entity.Property(rt => rt.TokenHash)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(rt => rt.Revoked)
                    .IsRequired()
                    .HasDefaultValue(false);

                entity.Property(rt => rt.IsActive)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.Property(rt => rt.CreatedAt)
                    .IsRequired()
                    .HasPrecision(3);

                entity.Property(rt => rt.RevokedAt)
                    .HasPrecision(3);
                   
                entity.Property(rt => rt.ExpiresAt)
                    .IsRequired()
                    .HasPrecision(3);

                // RefreshToken belongs to a User.
                entity.HasOne<User>()
                    .WithMany(u => u.RefreshTokens)
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // RefreshToken also belongs to a PlatformAdmin.
                entity.HasOne<PlatformAdmin>()
                    .WithMany(pa => pa.RefreshTokens)
                    .HasForeignKey(rt => rt.PlatformAdminId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Token hash must be globally unique
                entity.HasIndex(rt => rt.TokenHash)
                    .IsUnique()
                    .HasDatabaseName("IX_RefreshTokens_TokenHash");

                // Useful for retrieving a user's active/expired tokens
                entity.HasIndex(rt => new { rt.UserId, rt.ExpiresAt })
                    .HasDatabaseName("IX_RefreshTokens_UserId_ExpiresAt");

                entity.HasIndex(rt => new { rt.PlatformAdminId, rt.ExpiresAt })
                    .HasDatabaseName("IX_RefreshTokens_PlatformAdminId_ExpiresAt");

                // Expiry must be later than creation
                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_RefreshTokens_ExpiresAt",
                    "\"ExpiresAt\" > \"CreatedAt\""
                ));
            });
        }


    }
}
