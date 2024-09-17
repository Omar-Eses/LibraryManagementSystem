using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class GeneralUpdatesToEntityTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ReturnedDate",
                table: "BorrowingRecord",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Fine",
                table: "BorrowingRecord",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DueDate",
                table: "BorrowingRecord",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "BorrowedDate",
                table: "BorrowingRecord",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Username",
                table: "Users",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ReturnedDate",
                table: "BorrowingRecord",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Fine",
                table: "BorrowingRecord",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DueDate",
                table: "BorrowingRecord",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "BorrowedDate",
                table: "BorrowingRecord",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone");
        }
    }
}
