using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssignedTrainingPlanCoursePriority",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    priority_name = table.Column<string>(type: "text", unicode: false, nullable: true),
                    assigncoursepriority_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignedTrainingPlanCoursePriority", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "AssignedTrainingPlanCourseStatus",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    status_name = table.Column<string>(type: "text", unicode: false, nullable: true),
                    assigncoursestatus_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssignedTrainingPlanCourseStatus", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    category_name = table.Column<string>(type: "text", unicode: false, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "date", nullable: true),
                    modified_date = table.Column<DateTime>(type: "date", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courseCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "text", unicode: false, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    video_link = table.Column<string>(type: "text", unicode: false, nullable: true),
                    course_duration = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    keywords = table.Column<string>(type: "text", unicode: false, nullable: true),
                    start_date = table.Column<DateTime>(type: "date", nullable: true),
                    end_date = table.Column<DateTime>(type: "date", nullable: true),
                    created_date = table.Column<DateTime>(type: "date", nullable: true),
                    modified_date = table.Column<DateTime>(type: "date", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    deleted_date = table.Column<DateTime>(type: "date", nullable: true),
                    pdf_upload = table.Column<string>(type: "text", unicode: false, nullable: true),
                    featured_image = table.Column<string>(type: "text", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courses", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Designation",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    designation_name = table.Column<string>(type: "text", unicode: false, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    created_date = table.Column<DateTime>(type: "date", nullable: true),
                    modified_date = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_designation", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_name = table.Column<string>(type: "text", unicode: false, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    created_date = table.Column<DateTime>(type: "date", nullable: true),
                    modified_date = table.Column<DateTime>(type: "date", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "TrainingPlans",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    training_plan_name = table.Column<string>(type: "text", unicode: false, nullable: true),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    planned_duration_days = table.Column<int>(type: "integer", nullable: true),
                    featured_image = table.Column<string>(type: "text", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trainingPlan", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserActivityLogs/Auditrail",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    fk_user_id = table.Column<int>(type: "integer", nullable: true),
                    activity_type = table.Column<string>(type: "text", unicode: false, nullable: true),
                    timestamp = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true),
                    detail = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivityLogs/Auditrail", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "CourseCategoryMapping",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    fk_course_id = table.Column<int>(type: "integer", nullable: true),
                    fk_category_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_courseCategoryMapping", x => x.id);
                    table.ForeignKey(
                        name: "FK_CourseCategoryMapping_Category",
                        column: x => x.fk_category_id,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CourseCategoryMapping_Courses",
                        column: x => x.fk_course_id,
                        principalTable: "Courses",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "UserCourseProgress",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    fk_user_id = table.Column<int>(type: "integer", nullable: true),
                    fk_course_id = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<bool>(type: "boolean", nullable: true),
                    last_accessed = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userCourseProgress", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserCourseProgress_Courses",
                        column: x => x.fk_course_id,
                        principalTable: "Courses",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", unicode: false, nullable: true),
                    email = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    password = table.Column<string>(type: "text", unicode: false, nullable: true),
                    created_date = table.Column<DateTime>(type: "date", nullable: true),
                    created_by = table.Column<string>(type: "character varying(50)", unicode: false, maxLength: 50, nullable: true),
                    modified_date = table.Column<DateTime>(type: "date", nullable: true),
                    modified_by = table.Column<string>(type: "text", unicode: false, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: true),
                    fk_role_id = table.Column<int>(type: "integer", nullable: true),
                    fk_designation_id = table.Column<int>(type: "integer", nullable: true),
                    Profile = table.Column<string>(type: "text", unicode: false, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                    table.ForeignKey(
                        name: "FK_Users_Designation",
                        column: x => x.fk_designation_id,
                        principalTable: "Designation",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Users_Role",
                        column: x => x.fk_role_id,
                        principalTable: "Role",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "DesignationTrainingPlans",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    fk_designation_id = table.Column<int>(type: "integer", nullable: true),
                    fk_trainingplan_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_designationCourseTrainingPlanMapping", x => x.id);
                    table.ForeignKey(
                        name: "FK_DesignationTrainingPlans_Designation",
                        column: x => x.fk_designation_id,
                        principalTable: "Designation",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_DesignationTrainingPlans_TrainingPlans",
                        column: x => x.fk_trainingplan_id,
                        principalTable: "TrainingPlans",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "TrainingPlanCourses",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fk_plan_id = table.Column<int>(type: "integer", nullable: true),
                    fk_course_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trainingPlanCourses", x => x.id);
                    table.ForeignKey(
                        name: "FK_TrainingPlanCourse_Courses",
                        column: x => x.fk_course_id,
                        principalTable: "Courses",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_TrainingPlanCourses_TrainingPlans",
                        column: x => x.fk_plan_id,
                        principalTable: "TrainingPlans",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "UserAssignCourse",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    fk_user_Id = table.Column<int>(type: "integer", nullable: true),
                    fk_course_id = table.Column<int>(type: "integer", nullable: true),
                    date_assigned = table.Column<DateTime>(type: "date", nullable: true),
                    date_completed = table.Column<DateTime>(type: "date", nullable: true),
                    fk_trainingplan_id = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<bool>(type: "boolean", nullable: true),
                    assigncoursestatus_id = table.Column<int>(type: "integer", nullable: true),
                    assigncoursepriority_id = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userAssignCourse", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserAssignCourse_AssignedTrainingPlanCoursePriority",
                        column: x => x.assigncoursepriority_id,
                        principalTable: "AssignedTrainingPlanCoursePriority",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_UserAssignCourse_AssignedTrainingPlanCourseStatus",
                        column: x => x.assigncoursestatus_id,
                        principalTable: "AssignedTrainingPlanCourseStatus",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_userAssignCourse_Courses",
                        column: x => x.fk_course_id,
                        principalTable: "Courses",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_userAssignCourse_TrainingPlan",
                        column: x => x.fk_trainingplan_id,
                        principalTable: "TrainingPlans",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_userAssignCourse_User",
                        column: x => x.fk_user_Id,
                        principalTable: "Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseCategoryMapping_fk_category_id",
                table: "CourseCategoryMapping",
                column: "fk_category_id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCategoryMapping_fk_course_id",
                table: "CourseCategoryMapping",
                column: "fk_course_id");

            migrationBuilder.CreateIndex(
                name: "IX_DesignationTrainingPlans_fk_designation_id",
                table: "DesignationTrainingPlans",
                column: "fk_designation_id");

            migrationBuilder.CreateIndex(
                name: "IX_DesignationTrainingPlans_fk_trainingplan_id",
                table: "DesignationTrainingPlans",
                column: "fk_trainingplan_id");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanCourses_fk_course_id",
                table: "TrainingPlanCourses",
                column: "fk_course_id");

            migrationBuilder.CreateIndex(
                name: "IX_TrainingPlanCourses_fk_plan_id",
                table: "TrainingPlanCourses",
                column: "fk_plan_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignCourse_assigncoursepriority_id",
                table: "UserAssignCourse",
                column: "assigncoursepriority_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignCourse_assigncoursestatus_id",
                table: "UserAssignCourse",
                column: "assigncoursestatus_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignCourse_fk_course_id",
                table: "UserAssignCourse",
                column: "fk_course_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignCourse_fk_trainingplan_id",
                table: "UserAssignCourse",
                column: "fk_trainingplan_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAssignCourse_fk_user_Id",
                table: "UserAssignCourse",
                column: "fk_user_Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserCourseProgress_fk_course_id",
                table: "UserCourseProgress",
                column: "fk_course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_fk_designation_id",
                table: "Users",
                column: "fk_designation_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_fk_role_id",
                table: "Users",
                column: "fk_role_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CourseCategoryMapping");

            migrationBuilder.DropTable(
                name: "DesignationTrainingPlans");

            migrationBuilder.DropTable(
                name: "TrainingPlanCourses");

            migrationBuilder.DropTable(
                name: "UserActivityLogs/Auditrail");

            migrationBuilder.DropTable(
                name: "UserAssignCourse");

            migrationBuilder.DropTable(
                name: "UserCourseProgress");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "AssignedTrainingPlanCoursePriority");

            migrationBuilder.DropTable(
                name: "AssignedTrainingPlanCourseStatus");

            migrationBuilder.DropTable(
                name: "TrainingPlans");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Designation");

            migrationBuilder.DropTable(
                name: "Role");
        }
    }
}
