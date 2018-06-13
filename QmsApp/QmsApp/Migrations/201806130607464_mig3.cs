namespace QmsApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Services", "PossibleServiceTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Services", "PossibleServiceTime", c => c.Time(precision: 7));
        }
    }
}
