using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QQController.Entity;

namespace QQController.DAL
{
    [DbConfigurationType(typeof(MySql.Data.Entity.MySqlEFConfiguration))]
    public class MainDbContext:DbContext
    {
        public DbSet<Account> AccountSet { set; get; }

        public DbSet<QQAccount> QQAccountSet { set; get; }

        public DbSet<QQFriend> QQFriendsSet { set; get; }

        public DbSet<ReceivcedMessage> ReceivcedMessageSet { set; get; }

        public DbSet<SendedMessage> SendedMessageSet { set; get; }

        public MainDbContext() : base("dbConnStr")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<MainDbContext, Migrations.Configuration>());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().ToTable("account");
            modelBuilder.Entity<QQAccount>().ToTable("qq_account");
            modelBuilder.Entity<QQFriend>().ToTable("qq_friend");
            modelBuilder.Entity<ReceivcedMessage>().ToTable("receivced_message");
            modelBuilder.Entity<SendedMessage>().ToTable("sended_message");
        }
    }
}
