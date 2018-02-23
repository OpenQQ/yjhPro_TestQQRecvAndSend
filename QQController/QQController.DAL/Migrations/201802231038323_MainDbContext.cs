namespace QQController.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MainDbContext : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.qq_friend", "Guid");
            DropColumn("dbo.sended_message", "Guid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.sended_message", "Guid", c => c.Guid(nullable: false));
            AddColumn("dbo.qq_friend", "Guid", c => c.Guid(nullable: false));
        }
    }
}
