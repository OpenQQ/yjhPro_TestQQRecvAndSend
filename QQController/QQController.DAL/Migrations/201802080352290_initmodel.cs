namespace QQController.DAL.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initmodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.account",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsD = c.Boolean(nullable: false),
                        CreateTime = c.DateTime(nullable: false, precision: 0),
                        UpdateTime = c.DateTime(nullable: false, precision: 0),
                        AccountName = c.String(maxLength: 100, storeType: "nvarchar"),
                        pwd = c.String(maxLength: 50, storeType: "nvarchar"),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.qq_account",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsD = c.Boolean(nullable: false),
                        CreateTime = c.DateTime(nullable: false, precision: 0),
                        UpdateTime = c.DateTime(nullable: false, precision: 0),
                        QQNum = c.String(maxLength: 20, storeType: "nvarchar"),
                        pwd = c.String(maxLength: 100, storeType: "nvarchar"),
                        StatusDesc = c.String(maxLength: 100, storeType: "nvarchar"),
                        IsLogin = c.Boolean(nullable: false),
                        State = c.Int(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.qq_friend",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Guid = c.Guid(nullable: false),
                        IsD = c.Boolean(nullable: false),
                        CreateTime = c.DateTime(nullable: false, precision: 0),
                        UpdateTime = c.DateTime(nullable: false, precision: 0),
                        QQNum = c.String(maxLength: 20, storeType: "nvarchar"),
                        Nick = c.String(maxLength: 100, storeType: "nvarchar"),
                        Owner = c.String(maxLength: 20, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.receivced_message",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsD = c.Boolean(nullable: false),
                        CreateTime = c.DateTime(nullable: false, precision: 0),
                        UpdateTime = c.DateTime(nullable: false, precision: 0),
                        Type = c.Int(nullable: false),
                        From = c.String(maxLength: 20, storeType: "nvarchar"),
                        To = c.String(maxLength: 20, storeType: "nvarchar"),
                        Owner = c.String(maxLength: 20, storeType: "nvarchar"),
                        GroupNum = c.String(maxLength: 20, storeType: "nvarchar"),
                        Message = c.String(maxLength: 5000, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.sended_message",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Guid = c.Guid(nullable: false),
                        IsD = c.Boolean(nullable: false),
                        CreateTime = c.DateTime(nullable: false, precision: 0),
                        UpdateTime = c.DateTime(nullable: false, precision: 0),
                        Type = c.Int(nullable: false),
                        From = c.String(maxLength: 20, storeType: "nvarchar"),
                        To = c.String(maxLength: 20, storeType: "nvarchar"),
                        Owner = c.String(maxLength: 20, storeType: "nvarchar"),
                        GroupNum = c.String(maxLength: 20, storeType: "nvarchar"),
                        Message = c.String(maxLength: 5000, storeType: "nvarchar"),
                        IsSend = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.sended_message");
            DropTable("dbo.receivced_message");
            DropTable("dbo.qq_friend");
            DropTable("dbo.qq_account");
            DropTable("dbo.account");
        }
    }
}
