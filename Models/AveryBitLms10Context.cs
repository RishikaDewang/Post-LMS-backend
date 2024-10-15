using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace LMS.Models;

public partial class AveryBitLms10Context : DbContext
{
    public AveryBitLms10Context()
    {
    }

    public AveryBitLms10Context(DbContextOptions<AveryBitLms10Context> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseCategoryMapping> CourseCategoryMappings { get; set; }

    public virtual DbSet<Designation> Designations { get; set; }

    public virtual DbSet<DesignationTrainingPlan> DesignationTrainingPlans { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TrainingPlan> TrainingPlans { get; set; }

    public virtual DbSet<TrainingPlanCourse> TrainingPlanCourses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserActivityLogsAuditrail> UserActivityLogsAuditrails { get; set; }

    public virtual DbSet<UserAssignCourse> UserAssignCourses { get; set; }

    public virtual DbSet<UserCourseProgress> UserCourseProgresses { get; set; }

    public virtual DbSet<AssignedTrainingPlanCourseStatus> AssignedTrainingPlanCourseStatus { get; set; }

    public virtual DbSet<AssignedTrainingPlanCoursePriority> AssignedTrainingPlanCoursePriority { get; set; }

    /* protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
 #warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
         => optionsBuilder.UseSqlServer("Server=localhost;Database=AveryBit_LMS_1.0; Encrypt=False; Integrated Security=True");
 */
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_courseCategory");

            entity.ToTable("Category");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.CategoryName)
                .IsUnicode(false)
                .HasColumnName("category_name");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("date")
                .HasColumnName("created_date");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("date")
                .HasColumnName("modified_date");
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_courses");

            entity.Property(e => e.ID)
              //  .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CourseDuration)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("course_duration");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("date")
                .HasColumnName("created_date");
            entity.Property(e => e.DeletedDate)
                .HasColumnType("date")
                .HasColumnName("deleted_date");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.EndDate)
                .HasColumnType("date")
                .HasColumnName("end_date");
            entity.Property(e => e.FeaturedImage)
                .IsUnicode(false)
                .HasColumnName("featured_image");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
            entity.Property(e => e.Keywords)
                .IsUnicode(false)
                .HasColumnName("keywords");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("date")
                .HasColumnName("modified_date");
            entity.Property(e => e.PdfUpload)
                .IsUnicode(false)
                .HasColumnName("pdf_upload");
            entity.Property(e => e.StartDate)
                .HasColumnType("date")
                .HasColumnName("start_date");
            entity.Property(e => e.Title)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.VideoLink)
                .IsUnicode(false)
                .HasColumnName("video_link");
        });
      
        modelBuilder.Entity<CourseCategoryMapping>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_courseCategoryMapping");

            entity.ToTable("CourseCategoryMapping");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.FkCategoryId).HasColumnName("fk_category_id");
            entity.Property(e => e.FkCourseId).HasColumnName("fk_course_id");

            entity.HasOne(d => d.FkCategory).WithMany(p => p.CourseCategoryMappings)
                .HasForeignKey(d => d.FkCategoryId)
                .HasConstraintName("FK_CourseCategoryMapping_Category");

            entity.HasOne(d => d.FkCourse).WithMany(p => p.CourseCategoryMappings)
                .HasForeignKey(d => d.FkCourseId)
                .HasConstraintName("FK_CourseCategoryMapping_Courses");
        });

        modelBuilder.Entity<Designation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_designation");

            entity.ToTable("Designation");

            entity.Property(e => e.Id)
               // .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("date")
                .HasColumnName("created_date");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.DesignationName)
                .IsUnicode(false)
                .HasColumnName("designation_name");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("date")
                .HasColumnName("modified_date");
        });

        modelBuilder.Entity<DesignationTrainingPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_designationCourseTrainingPlanMapping");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.FkDesignationId).HasColumnName("fk_designation_id");
            entity.Property(e => e.FkTrainingplanId).HasColumnName("fk_trainingplan_id");

            entity.HasOne(d => d.FkDesignation).WithMany(p => p.DesignationTrainingPlans)
                .HasForeignKey(d => d.FkDesignationId)
                .HasConstraintName("FK_DesignationTrainingPlans_Designation");

            entity.HasOne(d => d.FkTrainingplan).WithMany(p => p.DesignationTrainingPlans)
                .HasForeignKey(d => d.FkTrainingplanId)
                .HasConstraintName("FK_DesignationTrainingPlans_TrainingPlans");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_role");

            entity.ToTable("Role");

            entity.Property(e => e.Id)
              //  .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("date")
                .HasColumnName("created_date");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("date")
                .HasColumnName("modified_date");
            entity.Property(e => e.RoleName)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<TrainingPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_trainingPlan");

            entity.Property(e => e.Id)
               // .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.FeaturedImage)
                .IsUnicode(false)
                .HasColumnName("featured_image");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.PlannedDurationDays).HasColumnName("planned_duration_days");
            entity.Property(e => e.TrainingPlanName)
                .IsUnicode(false)
                .HasColumnName("training_plan_name");
        });

        modelBuilder.Entity<TrainingPlanCourse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_trainingPlanCourses");

            entity.Property(e => e.Id)
              //  .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.FkCourseId).HasColumnName("fk_course_id");
            entity.Property(e => e.FkPlanId).HasColumnName("fk_plan_id");

            entity.HasOne(d => d.FkCourse).WithMany(p => p.TrainingPlanCourses)
                .HasForeignKey(d => d.FkCourseId)
                .HasConstraintName("FK_TrainingPlanCourse_Courses");

            entity.HasOne(d => d.FkPlan).WithMany(p => p.TrainingPlanCourses)
                .HasForeignKey(d => d.FkPlanId)
                .HasConstraintName("FK_TrainingPlanCourses_TrainingPlans");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable(tb =>
                {
                    tb.HasTrigger("tr_Users_fordelete");
                    tb.HasTrigger("tr_Users_forinsert");
                });

            entity.Property(e => e.Id)
               // .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("created_by");
            entity.Property(e => e.CreatedDate)
                .HasColumnType("date")
                .HasColumnName("created_date");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FkDesignationId).HasColumnName("fk_designation_id");
            entity.Property(e => e.FkRoleId).HasColumnName("fk_role_id");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.ModifiedBy)
                .IsUnicode(false)
                .HasColumnName("modified_by");
            entity.Property(e => e.ModifiedDate)
                .HasColumnType("date")
                .HasColumnName("modified_date");
            entity.Property(e => e.Name)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Profile)
                .IsUnicode(false)
                .HasColumnName("Profile");

            entity.HasOne(d => d.FkDesignation).WithMany(p => p.Users)
                .HasForeignKey(d => d.FkDesignationId)
                .HasConstraintName("FK_Users_Designation");

            entity.HasOne(d => d.FkRole).WithMany(p => p.Users)
                .HasForeignKey(d => d.FkRoleId)
                .HasConstraintName("FK_Users_Role");


        });

        modelBuilder.Entity<UserActivityLogsAuditrail>(entity =>
        {
            entity.ToTable("UserActivityLogs/Auditrail");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ActivityType)
                .IsUnicode(false)
                .HasColumnName("activity_type");
            entity.Property(e => e.Detail)
                .HasColumnType("text")
                .HasColumnName("detail");
            entity.Property(e => e.FkUserId).HasColumnName("fk_user_id");
            entity.Property(e => e.Timestamp)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasColumnName("timestamp");
        });

        modelBuilder.Entity<AssignedTrainingPlanCoursePriority>(entity =>
        {
            entity.ToTable("AssignedTrainingPlanCoursePriority");  // Use only the table name without slashes

            entity.Property(e => e.id)
               // .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.priority_name)
                 .IsUnicode(false)
                 .HasColumnName("priority_name");
        });


        modelBuilder.Entity<AssignedTrainingPlanCourseStatus>(entity =>
        {
            entity.ToTable("AssignedTrainingPlanCourseStatus");  // Use only the table name without slashes

            entity.Property(e => e.id)
              //  .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.status_name)
                 .IsUnicode(false)
                 .HasColumnName("status_name");

             entity.HasMany(e => e.UserAssignCourses)
        .WithOne(e => e.FKAssignedTrainingPlanCourseStatus) // Adjust the navigation property here
        .HasForeignKey(e => e.assigncoursestatus_id); // Adjust the foreign key property here

        });


        modelBuilder.Entity<UserAssignCourse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_userAssignCourse");

            entity.ToTable("UserAssignCourse");

            entity.Property(e => e.Id)
              //  .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.DateAssigned)
                .HasColumnType("date")
                .HasColumnName("date_assigned");
            entity.Property(e => e.DateCompleted)
                .HasColumnType("date")
                .HasColumnName("date_completed");
            entity.Property(e => e.assigncoursestatus_id).HasColumnName("assigncoursestatus_id");
            entity.Property(e => e.FkCourseId).HasColumnName("fk_course_id");
            entity.Property(e => e.FkTrainingplanId).HasColumnName("fk_trainingplan_id");
            entity.Property(e => e.FkUserId).HasColumnName("fk_user_Id");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.FkCourse).WithMany(p => p.UserAssignCourses)
                .HasForeignKey(d => d.FkCourseId)
                .HasConstraintName("FK_userAssignCourse_Courses");

            entity.HasOne(d => d.FkUser)
                 .WithMany(p => p.UserAssignCourses)
                 .HasForeignKey(d => d.FkUserId)
                 .HasConstraintName("FK_userAssignCourse_User");

            entity.HasOne(d => d.FkTrainingPlan)
                 .WithMany(p => p.UserAssignCourses)
                 .HasForeignKey(d => d.FkTrainingplanId)
                 .HasConstraintName("FK_userAssignCourse_TrainingPlan");

            entity.HasOne(d => d.FKAssignedTrainingPlanCourseStatus)
                .WithMany(p => p.UserAssignCourses)
                .HasForeignKey(d => d.assigncoursestatus_id)  // Make sure this matches your database column name
                .HasConstraintName("FK_UserAssignCourse_AssignedTrainingPlanCourseStatus");

            entity.HasOne(d => d.FKAssignedTrainingPlanCoursePriority)
                 .WithMany(p => p.UserAssignCourses)
                 .HasForeignKey(d => d.assigncoursepriority_id)  // Make sure this matches your database column name
                .HasConstraintName("FK_UserAssignCourse_AssignedTrainingPlanCoursePriority");

        });

        modelBuilder.Entity<UserCourseProgress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_userCourseProgress");

            entity.ToTable("UserCourseProgress");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.FkCourseId).HasColumnName("fk_course_id");
            entity.Property(e => e.FkUserId).HasColumnName("fk_user_id");
            entity.Property(e => e.LastAccessed)
                .HasColumnType("date")
                .HasColumnName("last_accessed");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.FkCourse).WithMany(p => p.UserCourseProgresses)
                .HasForeignKey(d => d.FkCourseId)
                .HasConstraintName("FK_UserCourseProgress_Courses");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
