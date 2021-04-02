namespace InvoiceManager.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class second : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Invoices", "AddressId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Invoices", "AddressId", c => c.Int(nullable: false));
        }
    }
}
