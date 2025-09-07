using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DbFirst.Models;

public partial class TodoContext : DbContext
{
    public TodoContext()
    {
    }

    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Division> Division { get; set; }

    public virtual DbSet<Employee> Employee { get; set; }

    public virtual DbSet<JobTitle> JobTitle { get; set; }

    public virtual DbSet<TodoList> TodoList { get; set; }

    public virtual DbSet<UploadFile> UploadFile { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<Division>(entity =>
        {
            entity.HasKey(e => e.DivisionId).HasName("PK__Division__20EFC6A8F99D2514");

            entity.Property(e => e.DivisionId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F114A9BEF50");

            entity.Property(e => e.EmployeeId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Account).IsRequired();
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Password).IsRequired();

            entity.HasOne(d => d.Division).WithMany(p => p.Employee)
                .HasForeignKey(d => d.DivisionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_ToTable_1");

            entity.HasOne(d => d.JobTitle).WithMany(p => p.Employee)
                .HasForeignKey(d => d.JobTitleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Employee_ToTable");
        });

        modelBuilder.Entity<JobTitle>(entity =>
        {
            entity.HasKey(e => e.JobTitleId).HasName("PK__JobTitle__35382FE932476E76");

            entity.Property(e => e.JobTitleId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<TodoList>(entity =>
        {
            entity.HasKey(e => e.TodoId).HasName("PK__Table__95862552FC49C675");

            entity.Property(e => e.TodoId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.InsertTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.UpdateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.InsertEmployee).WithMany(p => p.TodoListInsertEmployee)
                .HasForeignKey(d => d.InsertEmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Todo_ToTable");

            entity.HasOne(d => d.UpdateEmployee).WithMany(p => p.TodoListUpdateEmployee)
                .HasForeignKey(d => d.UpdateEmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Todo_ToTable_1");
        });

        modelBuilder.Entity<UploadFile>(entity =>
        {
            entity.HasKey(e => e.UploadFileId).HasName("PK__Table__6F0F98BF69AEBA07");

            entity.Property(e => e.UploadFileId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Src).IsRequired();

            entity.HasOne(d => d.Todo).WithMany(p => p.UploadFile)
                .HasForeignKey(d => d.TodoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_File_ToTable");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
