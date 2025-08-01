﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apha.BST.Core.Entities;
using Apha.BST.DataAccess.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Apha.BST.Application.UnitTests
{
    public static class SqliteTestContextMother
    {
        public static BstContext CreateContext()
        {
            var connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<BstContext>()
                .UseSqlite(connection)
                .Options;


            var dbContext = new BstContext(options);

            dbContext.Database.EnsureCreated();

            return dbContext;
        }
    }
}
