using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using DbFirst.Dtos;

#nullable disable

namespace DbFirst.Models
{
    public partial class TodoContext2 : TodoContext
    {
        public TodoContext2()
        {
        }

        public TodoContext2(DbContextOptions<TodoContext> options)
            : base(options)
        {
        }
     
        public virtual DbSet<TodoListDto> TodoListSelectDto { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TodoListDto>().HasNoKey();           
        }

    }
}
