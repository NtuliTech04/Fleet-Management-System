namespace FleetTours___Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssignDrivers",
                c => new
                    {
                        AssignID = c.Int(nullable: false, identity: true),
                        SelectedDriver = c.Int(nullable: false),
                        BkID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AssignID)
                .ForeignKey("dbo.Bookings", t => t.BkID, cascadeDelete: true)
                .Index(t => t.BkID);
            
            CreateTable(
                "dbo.Bookings",
                c => new
                    {
                        BkID = c.Int(nullable: false, identity: true),
                        DateCreated = c.DateTime(nullable: false),
                        PayMethod = c.String(nullable: false),
                        Status = c.String(),
                        PaymentStatus = c.String(),
                        Confirmation = c.String(),
                        DriverID = c.Int(nullable: false),
                        Driver = c.String(),
                        BookDetID = c.Int(nullable: false),
                        Email = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.BkID)
                .ForeignKey("dbo.BookDetails", t => t.BookDetID, cascadeDelete: true)
                .ForeignKey("dbo.UserProfiles", t => t.Email)
                .Index(t => t.BookDetID)
                .Index(t => t.Email);
            
            CreateTable(
                "dbo.BookDetails",
                c => new
                    {
                        BookDetID = c.Int(nullable: false, identity: true),
                        Vacation = c.String(nullable: false),
                        VacationDate = c.DateTime(nullable: false),
                        LessonTime = c.Time(nullable: false, precision: 7),
                        HirePeriod = c.Int(nullable: false),
                        ReturnDate = c.DateTime(nullable: false),
                        Passengers = c.Int(nullable: false),
                        Total = c.Double(nullable: false),
                        Email = c.String(nullable: false, maxLength: 128),
                        VehicleID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.BookDetID)
                .ForeignKey("dbo.UserProfiles", t => t.Email, cascadeDelete: true)
                .ForeignKey("dbo.Vehicles", t => t.VehicleID, cascadeDelete: true)
                .Index(t => t.Email)
                .Index(t => t.VehicleID);
            
            CreateTable(
                "dbo.UserProfiles",
                c => new
                    {
                        Email = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        SAID = c.String(nullable: false),
                        BirthDate = c.String(),
                        Cl_Age = c.String(),
                        Cl_Gender = c.String(),
                        ProfileImage = c.Binary(),
                    })
                .PrimaryKey(t => t.Email);
            
            CreateTable(
                "dbo.UserAddresses",
                c => new
                    {
                        AddressID = c.Int(nullable: false, identity: true),
                        PickUp = c.String(),
                        Destination = c.String(),
                        Email = c.String(maxLength: 128),
                        BookDetID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AddressID)
                .ForeignKey("dbo.BookDetails", t => t.BookDetID, cascadeDelete: true)
                .ForeignKey("dbo.UserProfiles", t => t.Email)
                .Index(t => t.Email)
                .Index(t => t.BookDetID);
            
            CreateTable(
                "dbo.Vehicles",
                c => new
                    {
                        VehicleID = c.Int(nullable: false, identity: true),
                        RegNo = c.String(nullable: false),
                        VehicleMake = c.String(nullable: false),
                        Model = c.String(nullable: false),
                        Type = c.String(nullable: false),
                        Capacity = c.Int(nullable: false),
                        Status = c.String(),
                        DriverID = c.Int(nullable: false),
                        Driver = c.String(),
                        VehicleImage = c.Binary(),
                        Duty = c.String(),
                    })
                .PrimaryKey(t => t.VehicleID);
            
            CreateTable(
                "dbo.BookedVehicles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        HireDate = c.DateTime(nullable: false),
                        ReturnDate = c.DateTime(nullable: false),
                        VehicleID = c.Int(nullable: false),
                        BookID = c.Int(nullable: false),
                        Status = c.String(),
                        DriverID = c.Int(nullable: false),
                        Driver = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Drivers",
                c => new
                    {
                        DriverID = c.Int(nullable: false, identity: true),
                        VehicleID = c.Int(nullable: false),
                        FirstName = c.String(nullable: false, maxLength: 30),
                        LastName = c.String(nullable: false, maxLength: 30),
                        SAID = c.String(nullable: false, maxLength: 13),
                        BirthDate = c.String(),
                        Cl_Age = c.String(),
                        Cl_Gender = c.String(),
                        Phone = c.String(nullable: false, maxLength: 10),
                        Email = c.String(nullable: false),
                        DriverImage = c.Binary(),
                    })
                .PrimaryKey(t => t.DriverID);
            
            CreateTable(
                "dbo.RideAddresses",
                c => new
                    {
                        RadID = c.Int(nullable: false, identity: true),
                        PickUp = c.String(nullable: false),
                        Destination = c.String(nullable: false),
                        Email = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.RadID)
                .ForeignKey("dbo.UserProfiles", t => t.Email, cascadeDelete: true)
                .Index(t => t.Email);
            
            CreateTable(
                "dbo.Rides",
                c => new
                    {
                        RideID = c.Int(nullable: false, identity: true),
                        VehicleID = c.Int(nullable: false),
                        RadID = c.Int(nullable: false),
                        DriverID = c.Int(nullable: false),
                        Email = c.String(maxLength: 128),
                        Distance = c.Double(nullable: false),
                        BasicCost = c.Double(nullable: false),
                        Discounts = c.Double(nullable: false),
                        TotalCost = c.Double(nullable: false),
                        PaymentMethod = c.String(),
                        PaymentStatus = c.String(),
                        RideStatus = c.String(),
                    })
                .PrimaryKey(t => t.RideID)
                .ForeignKey("dbo.RideAddresses", t => t.RadID, cascadeDelete: true)
                .ForeignKey("dbo.UserProfiles", t => t.Email)
                .Index(t => t.RadID)
                .Index(t => t.Email);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FirstName = c.String(),
                        LastName = c.String(),
                        passNumber = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.Rides", "Email", "dbo.UserProfiles");
            DropForeignKey("dbo.Rides", "RadID", "dbo.RideAddresses");
            DropForeignKey("dbo.RideAddresses", "Email", "dbo.UserProfiles");
            DropForeignKey("dbo.AssignDrivers", "BkID", "dbo.Bookings");
            DropForeignKey("dbo.Bookings", "Email", "dbo.UserProfiles");
            DropForeignKey("dbo.Bookings", "BookDetID", "dbo.BookDetails");
            DropForeignKey("dbo.BookDetails", "VehicleID", "dbo.Vehicles");
            DropForeignKey("dbo.BookDetails", "Email", "dbo.UserProfiles");
            DropForeignKey("dbo.UserAddresses", "Email", "dbo.UserProfiles");
            DropForeignKey("dbo.UserAddresses", "BookDetID", "dbo.BookDetails");
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.Rides", new[] { "Email" });
            DropIndex("dbo.Rides", new[] { "RadID" });
            DropIndex("dbo.RideAddresses", new[] { "Email" });
            DropIndex("dbo.UserAddresses", new[] { "BookDetID" });
            DropIndex("dbo.UserAddresses", new[] { "Email" });
            DropIndex("dbo.BookDetails", new[] { "VehicleID" });
            DropIndex("dbo.BookDetails", new[] { "Email" });
            DropIndex("dbo.Bookings", new[] { "Email" });
            DropIndex("dbo.Bookings", new[] { "BookDetID" });
            DropIndex("dbo.AssignDrivers", new[] { "BkID" });
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.Rides");
            DropTable("dbo.RideAddresses");
            DropTable("dbo.Drivers");
            DropTable("dbo.BookedVehicles");
            DropTable("dbo.Vehicles");
            DropTable("dbo.UserAddresses");
            DropTable("dbo.UserProfiles");
            DropTable("dbo.BookDetails");
            DropTable("dbo.Bookings");
            DropTable("dbo.AssignDrivers");
        }
    }
}
