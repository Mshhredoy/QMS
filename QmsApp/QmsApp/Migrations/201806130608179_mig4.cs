namespace QmsApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "PossibleServiceTime", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "PossibleServiceTime");
        }
    }
}
