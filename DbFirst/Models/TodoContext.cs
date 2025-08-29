using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DbFirst.Models;

public partial class TodoContext : DbContext
{
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TodoList> TodoList { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoList>(entity =>
        {
            entity.HasKey(e => e.todoId).HasName("PK__TodoList__E5C578A17DAABBEB");

            entity.Property(e => e.todoId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.enable).HasDefaultValue(true);
            entity.Property(e => e.insertEmployeeName).HasMaxLength(250);
            entity.Property(e => e.insertTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.updateEmployeeName).HasMaxLength(250);
            entity.Property(e => e.updateTime)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.uploadFiles).HasMaxLength(250);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
