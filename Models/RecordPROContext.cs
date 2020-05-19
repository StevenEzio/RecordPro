﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RecordPRO.Models;

namespace RecordPRO.Models
{
    public class RecordPROContext:DbContext
    {
        public RecordPROContext(DbContextOptions<RecordPROContext> options)
    : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<UserBill>().ToTable("UserBill");
        }

        public DbSet<RecordPRO.Models.UserBill> UserBill { get; set; }
    }
}
