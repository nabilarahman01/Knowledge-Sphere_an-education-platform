using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NuGet.Protocol.Core.Types;

namespace knowledgeSphere.Models
{
    public partial class KnowledgeContext : DbContext
    {
        public KnowledgeContext()
        {
        }

        public KnowledgeContext(DbContextOptions<KnowledgeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<Teacher> Teachers { get; set; } 
        public virtual DbSet<Course> Courses { get; set; } = null!;
        public DbSet<EnrollStudents> EnrollStudents { get; set; }
        public DbSet<CoursePost> CoursePosts { get; set; }
        public DbSet<ResourceView> ResourceView { get; set; }
        public DbSet<QnAViewModel> Questions { get; set; }
        public DbSet<AnswerViewModel> Answers { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(entity =>
            {
                entity.ToTable("Student");

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Email).HasMaxLength(50);
                entity.HasIndex(e => e.Email).IsUnique();


                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.Gender).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(50);

                entity.Property(e => e.StudentId).HasColumnName("StudentID");
            });

            modelBuilder.Entity<Teacher>(entity =>
            {
                entity.ToTable("Teacher");

                entity.Property(e => e.TeacherId).HasColumnName("teacherID");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FirstName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(50);
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.ToTable("Course"); 

                entity.Property(e => e.Id).HasColumnName("Id");

                entity.Property(e => e.CourseNo).HasMaxLength(50); 

                entity.Property(e => e.CourseTitle).HasMaxLength(50);

                entity.Property(e => e.Description).HasMaxLength(450);

                entity.Property(e => e.Mail).HasMaxLength(50);
            });

            modelBuilder.Entity<EnrollStudents>(entity =>
            {
                entity.ToTable("EnrollStudents");
                entity.Property(e => e.Id).HasColumnName("Id");
                entity.Property(e => e.StudentEmail).HasMaxLength(100);
                entity.Property(e => e.TeacherMail).HasMaxLength(100);
                entity.Property(e => e.CourseId);

                entity.HasOne(e => e.Course)
            .WithMany()  // Assuming Course has a collection of EnrollStudents
            .HasForeignKey(e => e.CourseId)
            .HasConstraintName("FK_EnrollStudents_Course_CourseId");
            });

            modelBuilder.Entity<ResourceView>()
                .Property(r => r.FileData)
                .HasColumnType("varbinary(max)");

            //modelBuilder.Entity<EnrollStudents>()
            //    .HasOne(e => e.Student)
            //    .WithMany(s => s.EnrolledCourses)
            //    .HasForeignKey(e => e.StudentEmail);

            //modelBuilder.Entity<EnrollStudents>()
            //    .HasOne(e => e.Course)
            //    .WithMany(c => c.EnrolledStudents)
            //    .HasForeignKey(e => e.SelectedCourseId);

            modelBuilder.Entity<CoursePost>().ToTable("CoursePost");

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
