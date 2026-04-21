using EmergencySimulator.AdminPanel.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace EmergencySimulator.AdminPanel.Data
{
    public class DatabaseContext : DbContext
    {
        // Таблицы БД
        public DbSet<User> Users { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<PLAData> PLA_Data { get; set; }
        public DbSet<TrainingResult> TrainingResults { get; set; }
        public DbSet<SessionAction> SessionActions { get; set; }
        public DbSet<SessionLog> SessionLogs { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<EmergencyCall> EmergencyCalls { get; set; }
        public DbSet<VGKMember> VGKMembers { get; set; }
        public DbSet<NotificationList> NotificationLists { get; set; }
        public DbSet<Admin> Admins { get; set; }

        public DatabaseContext() { }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string dbPath = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "emergency_simulator.db");
                optionsBuilder.UseSqlite($"Data Source={dbPath}");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка таблицы Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserID);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Surname).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Position).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PasswordHash).HasMaxLength(64);
                entity.Property(e => e.Salt).HasMaxLength(32);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasMany(e => e.TrainingResults)
                      .WithOne(e => e.User)
                      .HasForeignKey(e => e.UserID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Настройка таблицы Staff
            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(e => e.StaffID);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Surname).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Position).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Phone).HasMaxLength(20);

                entity.HasOne(e => e.NotificationList)
                      .WithMany(e => e.StaffMembers)
                      .HasForeignKey(e => e.ListID)
                      .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.VGKMember)
                      .WithOne(e => e.Staff)
                      .HasForeignKey<VGKMember>(e => e.StaffID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Настройка таблицы PLAData
            modelBuilder.Entity<PLAData>(entity =>
            {
                entity.HasKey(e => e.PlaID);
                entity.ToTable("PLA_Data");
                entity.Property(e => e.PositionNumber).IsRequired().HasMaxLength(10);
                entity.Property(e => e.PositionName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ActionStep).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Description).HasMaxLength(1000);

                entity.HasMany(e => e.SessionActions)
                      .WithOne(e => e.PLAData)
                      .HasForeignKey(e => e.PlaID)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Настройка таблицы TrainingResults
            modelBuilder.Entity<TrainingResult>(entity =>
            {
                entity.HasKey(e => e.ResultID);
                entity.Property(e => e.SessionStart).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).HasDefaultValue("В процессе");
                entity.Property(e => e.PDFReportPath).HasMaxLength(500);
                entity.Property(e => e.SignificantErrorsCount).HasMaxLength(50);

                entity.HasOne(e => e.User)
                      .WithMany(e => e.TrainingResults)
                      .HasForeignKey(e => e.UserID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.SessionActions)
                      .WithOne(e => e.TrainingResult)
                      .HasForeignKey(e => e.ResultID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.SessionLogs)       
                      .WithOne(e => e.TrainingResult)
                      .HasForeignKey(e => e.ResultID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(e => e.EmergencyCalls)
                      .WithOne(e => e.TrainingResult)
                      .HasForeignKey(e => e.ResultID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Настройка таблицы SessionActions
            modelBuilder.Entity<SessionAction>(entity =>
            {
                entity.HasKey(e => new { e.ResultID, e.ActionOrder });

                entity.Property(e => e.ActionTime).IsRequired();
                entity.Property(e => e.TimeTaken).IsRequired();
                entity.Property(e => e.ErrorType).HasMaxLength(50);
                entity.Property(e => e.ErrorDescription).HasMaxLength(500);

                entity.HasOne(e => e.TrainingResult)
                      .WithMany(e => e.SessionActions)
                      .HasForeignKey(e => e.ResultID)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.PLAData)
                      .WithMany(e => e.SessionActions)
                      .HasForeignKey(e => e.PlaID)
                      .OnDelete(DeleteBehavior.Restrict);  // NOT NULL, поэтому Restrict

                entity.HasOne(e => e.Equipment)
                      .WithMany(e => e.SessionActions)
                      .HasForeignKey(e => e.EquipmentID)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Настройка таблицы SessionLogs
            modelBuilder.Entity<SessionLog>(entity =>
            {
                entity.HasKey(e => new { e.ResultID, e.LogEntryNumber });

                entity.Property(e => e.EventTime).IsRequired();
                entity.Property(e => e.EventType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.EventDescription).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Executor).HasMaxLength(200);
                entity.Property(e => e.Note).HasMaxLength(1000);

                entity.HasOne(e => e.TrainingResult)
                      .WithMany(e => e.SessionLogs)       // коллекция
                      .HasForeignKey(e => e.ResultID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Настройка таблицы Equipments
            modelBuilder.Entity<Equipment>(entity =>
            {
                entity.HasKey(e => e.EquipmentID);
                entity.Property(e => e.EquipmentName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.EquipmentType).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Location).HasMaxLength(200);

                entity.HasMany(e => e.SessionActions)
                      .WithOne(e => e.Equipment)
                      .HasForeignKey(e => e.EquipmentID)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Настройка таблицы EmergencyCalls
            modelBuilder.Entity<EmergencyCall>(entity =>
            {
                entity.HasKey(e => new { e.ResultID, e.CallTime });

                entity.Property(e => e.CallTime).IsRequired();
                entity.Property(e => e.IncidentLocation).IsRequired().HasMaxLength(200);
                entity.Property(e => e.PeopleInZone).IsRequired();
                entity.Property(e => e.VictimName).HasMaxLength(200);
                entity.Property(e => e.ReceiverName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ReceiverPosition).IsRequired().HasMaxLength(200);

                entity.HasOne(e => e.TrainingResult)
                      .WithMany(e => e.EmergencyCalls)
                      .HasForeignKey(e => e.ResultID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Настройка таблицы VGKMembers
            modelBuilder.Entity<VGKMember>(entity =>
            {
                entity.HasKey(e => e.MemberID);
                entity.ToTable("VGKMembers");
                entity.Property(e => e.Role).IsRequired().HasMaxLength(100);

                entity.HasOne(e => e.Staff)
                      .WithOne(e => e.VGKMember)
                      .HasForeignKey<VGKMember>(e => e.StaffID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Настройка таблицы NotificationLists
            modelBuilder.Entity<NotificationList>(entity =>
            {
                entity.HasKey(e => e.ListID);
                entity.Property(e => e.ListName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);

                entity.HasMany(e => e.StaffMembers)
                      .WithOne(e => e.NotificationList)
                      .HasForeignKey(e => e.ListID)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ДОБАВЛЕНО: Настройка таблицы Admins
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.AdminID);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(64);
                entity.Property(e => e.Salt).IsRequired().HasMaxLength(32);
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.Role).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

                // Уникальный индекс для Username
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Индексы для оптимизации
            modelBuilder.Entity<User>().HasIndex(e => new { e.Surname, e.Name });
            modelBuilder.Entity<Staff>().HasIndex(e => e.Position);
            modelBuilder.Entity<TrainingResult>().HasIndex(e => e.SessionStart);
            modelBuilder.Entity<TrainingResult>().HasIndex(e => e.UserID);
            modelBuilder.Entity<SessionAction>().HasIndex(e => e.ActionTime);
            modelBuilder.Entity<EmergencyCall>().HasIndex(e => e.CallTime);
            modelBuilder.Entity<SessionLog>().HasIndex(e => e.EventTime);
        }

        /// <summary>
        /// Метод для применения миграций и создания БД, если её нет
        /// </summary>
        public void InitializeDatabase()
        {
            try
            {
                // Создаём базу данных и все таблицы
                Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка инициализации базы данных: {ex.Message}", ex);
            }
        }
    }
}
