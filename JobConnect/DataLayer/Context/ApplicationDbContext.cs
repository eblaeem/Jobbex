using Azure.Identity;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Net;
using System.Security.Claims;

namespace DataLayer.Context
{
    public class ApplicationDbContext : DbContext, IUnitOfWork
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public ApplicationDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _contextAccessor = httpContextAccessor;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.AddInterceptors(new TaggedQueryCommandInterceptor());

        public virtual DbSet<AccessCode> AccessCodes { get; set; }
        public virtual DbSet<Attachment> Attachments { get; set; }
        public virtual DbSet<AttachmentType> AttachmentTypes { get; set; }
        public virtual DbSet<Audit> Audits { get; set; }
        public virtual DbSet<EducationLevel> EducationLevels { get; set; }
        public virtual DbSet<Group> JobGroups { get; set; }
        public virtual DbSet<Position> JobPositions { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<LanguageLevel> LanguageLevels { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<StateModel> States { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<LocationType> LocationTypes { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }

        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<SmsOutBox> SmsOutBoxs { get; set; }
        public virtual DbSet<Select2Suggestion> Select2Suggestions { get; set; }
        public virtual DbSet<SmsOutboxType> SmsOutboxTypes { get; set; }
        public virtual DbSet<SoftwareSkill> SoftwareSkills { get; set; }
        public virtual DbSet<SoftwareSkillGroup> SoftwareSkillGroups { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAccessCode> UserAccessCodes { get; set; }
        public virtual DbSet<UserCourse> UserCourses { get; set; }
        public virtual DbSet<UserEducation> UserEducations { get; set; }
        public virtual DbSet<UserJob> UserJobs { get; set; }
        public virtual DbSet<UserLanguage> UserLanguages { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<UserSoftwareSkill> UserSoftwareSkills { get; set; }
        public virtual DbSet<UserToken> UserTokens { set; get; }
        public virtual DbSet<UserPriority> UserPriorities { get; set; }

        public virtual DbSet<Company> Companies { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<JobSkill> JobSkills { get; set; }
        public virtual DbSet<UserRequestedStatus> UserRequestedStatus { get; set; }
        public virtual DbSet<JobRequested> JobRequesteds { get; set; }
        public virtual DbSet<JobRequestState> JobRequestStates { get; set; }
        public virtual DbSet<OrganizationSize> OrganizationSizes { get; set; }
        public virtual DbSet<WorkExperienceYear> WorkExperienceYears { get; set; }
        public virtual DbSet<Benefit> Benefit { get; set; }
        public virtual DbSet<ContractType> ContractTypes { get; set; }
        public virtual DbSet<SkillLevel> SkillLevels { get; set; }
        public virtual DbSet<MaritalStatus> MaritalStatus { get; set; }
        public virtual DbSet<MilitaryStatus> MilitaryStatus { get; set; }
        public virtual DbSet<SalaryRequestedType> SalaryRequestedTypes { get; set; }
        public virtual DbSet<JobBenefit> JobBenefits { get; set; }
        public virtual DbSet<Gender> Genders{ get; set; }
        public virtual DbSet<UserPinJob> UserPinJobs{ get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>(entity =>
            {
                entity.Property(e => e.Username).HasMaxLength(450).IsRequired();
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.SerialNumber).HasMaxLength(450);
            });

            builder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(450).IsRequired();
                entity.HasIndex(e => e.Name).IsUnique();
            });

            builder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.RoleId);
                entity.Property(e => e.UserId);
                entity.Property(e => e.RoleId);
                entity.HasOne(d => d.Role).WithMany(p => p.UserRoles).HasForeignKey(d => d.RoleId);
                entity.HasOne(d => d.User).WithMany(p => p.UserRoles).HasForeignKey(d => d.UserId);
            });

            builder.Entity<UserToken>(entity =>
            {
                entity.HasOne(ut => ut.User)
                      .WithMany(u => u.UserTokens)
                      .HasForeignKey(ut => ut.UserId);

                entity.Property(ut => ut.RefreshTokenIdHash).HasMaxLength(450).IsRequired();
                entity.Property(ut => ut.RefreshTokenIdHashSource).HasMaxLength(450);
            });
            
            builder.Entity<UserPinJob>(entity =>
            {
                entity.HasKey(x=> new {x.UserId,x.JobId});

            });
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var auditEntries = OnBeforeSaveChanges();
                var result = await base.SaveChangesAsync(cancellationToken);
                await OnAfterSaveChanges(auditEntries);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private List<AuditEntry> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var auditEntries = new List<AuditEntry>();
            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                    continue;
                if (_contextAccessor.HttpContext == null)
                {
                    continue;
                }
                var claimsIdentity = _contextAccessor.HttpContext.User.Identity as ClaimsIdentity;
                var userDataClaim = claimsIdentity?.FindFirst(ClaimTypes.UserData);
                var userId = userDataClaim?.Value;

                var ipAddress = _contextAccessor?.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                ipAddress ??= _contextAccessor?.HttpContext.Connection?.RemoteIpAddress?.ToString();


                var auditEntry = new AuditEntry(entry)
                {
                    TableName = entry.Metadata.GetDefaultTableName(),
                    UserId = Convert.ToInt32(userId),
                    IpAddress = ipAddress,
                };
                auditEntries.Add(auditEntry);

                foreach (var property in entry.Properties)
                {
                    if (property.IsTemporary)
                    {
                        auditEntry.TemporaryProperties.Add(property);
                        continue;
                    }

                    string propertyName = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[propertyName] = property.CurrentValue;
                        continue;
                    }

                    switch (entry.State)
                    {
                        case EntityState.Added:
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                            auditEntry.TypeName = "Added";
                            break;

                        case EntityState.Deleted:
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.TypeName = "Deleted";
                            break;

                        case EntityState.Modified:
                            if (property.IsModified)
                            {
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                                auditEntry.TypeName = "Modified";
                            }
                            break;
                    }
                }
            }
            foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
            {
                Audits.Add(auditEntry.ToAudit());
            }
            return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
        }
        private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return Task.CompletedTask;

            foreach (var auditEntry in auditEntries)
            {
                foreach (var prop in auditEntry.TemporaryProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }
                Audits.Add(auditEntry.ToAudit());
            }
            return SaveChangesAsync();
        }
    }
}
