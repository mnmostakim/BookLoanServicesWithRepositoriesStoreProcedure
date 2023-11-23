namespace BookLoansPorject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BookLoan : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BookLoans",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BorrowerName = c.String(nullable: false, maxLength: 150),
                        Address = c.String(nullable: false, maxLength: 150),
                        Phone = c.String(nullable: false, maxLength: 15),
                        LoanDate = c.DateTime(nullable: false, storeType: "date"),
                        ReturnDate = c.DateTime(nullable: false, storeType: "date"),
                        BookId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Books", t => t.BookId, cascadeDelete: true)
                .Index(t => t.BookId);
            CreateStoredProcedure("dbo.DeleteBook",
                p => new { id = p.Int() },
                "DELETE FROM dbo.Books WHERE id=@id");

            CreateTable(
                "dbo.Books",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 150),
                        Author = c.String(nullable: false, maxLength: 150),
                        Published = c.String(nullable: false, maxLength: 250),
                        Description = c.String(nullable: false, maxLength: 450),
                        Genre = c.Int(nullable: false),
                        BookCover = c.String(nullable: false, maxLength: 150),
                        IsAvailable = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BookLoans", "BookId", "dbo.Books");
            DropIndex("dbo.BookLoans", new[] { "BookId" });
            DropTable("dbo.Books");
            DropTable("dbo.BookLoans");
            DropStoredProcedure("dbo.BookLoans");
        }
    }
}
