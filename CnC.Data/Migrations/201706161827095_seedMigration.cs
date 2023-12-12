namespace CnC.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class seedMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AssignTickets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        TicketID = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CardRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CardRequestFormId = c.Int(nullable: false),
                        CardTypeId = c.Int(nullable: false),
                        CardQty = c.Int(nullable: false),
                        ServiceExchangeFee = c.Decimal(precision: 18, scale: 2),
                        Fee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CardIssuerRespondedOn = c.DateTime(),
                        DeclinedReason = c.String(maxLength: 500),
                        DispatchedToTBOOn = c.DateTime(),
                        DispatchedToCustomerOn = c.DateTime(),
                        DeliveredToCustomerOn = c.DateTime(),
                        Description = c.String(maxLength: 500),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Cards",
                c => new
                    {
                        CardRequestId = c.Int(nullable: false),
                        Number = c.String(nullable: false, maxLength: 20),
                        Title = c.String(nullable: false, maxLength: 100),
                        CardServiceProviderCardId = c.String(nullable: false),
                        ExpiryDate = c.String(nullable: false, maxLength: 4),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CardRequestId)
                .Index(t => t.Number, unique: true);
            
            CreateTable(
                "dbo.CardTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Fee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsReloadable = c.Boolean(nullable: false),
                        ReloadLimit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ReloadDayLimit = c.Int(nullable: false),
                        MaximumReload = c.Int(nullable: false),
                        MinimumReloadAtATime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaximumReloadAtATime = c.Decimal(nullable: false, precision: 18, scale: 2),
                        MaxQuantity = c.Int(nullable: false),
                        IsProofOfSourceOfFundsRequired = c.Boolean(nullable: false),
                        IsProofOfAddressRequired = c.Boolean(nullable: false),
                        Requirements = c.String(maxLength: 250),
                        Description = c.String(maxLength: 500),
                        IsActive = c.Boolean(nullable: false),
                        TopUpServiceFeePercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TopUpServiceFeeMinimum = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AreaCode = c.Short(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        CountryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CnCActions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DisplayName = c.String(nullable: false, maxLength: 50),
                        ActionName = c.String(nullable: false, maxLength: 50),
                        ControllerName = c.String(nullable: false, maxLength: 50),
                        ParentActionId = c.Int(),
                        ShowInMenu = c.Boolean(nullable: false),
                        ShowInTab = c.Boolean(nullable: false),
                        Log = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => new { t.ActionName, t.ControllerName }, unique: true, name: "ActionRoute");
            
            CreateTable(
                "dbo.CnCSettings",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Value = c.String(nullable: false, maxLength: 1000),
                        Description = c.String(maxLength: 200),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CallingCode = c.Short(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Code = c.String(nullable: false, maxLength: 3),
                        CurrencyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.CallingCode, unique: true)
                .Index(t => t.Name, unique: true)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "dbo.Currencies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Code = c.String(nullable: false, maxLength: 3),
                        IsSystemCurrency = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true)
                .Index(t => t.Code, unique: true);
            
            CreateTable(
                "dbo.CurrencyRates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CurrencyId = c.Int(nullable: false),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        GenderId = c.Int(nullable: false),
                        DateOfBirth = c.DateTime(nullable: false),
                        DateOfBirthInCustomerLanguage = c.String(maxLength: 50),
                        MaritalStatusId = c.Int(nullable: false),
                        NIC = c.String(nullable: false, maxLength: 20),
                        PassportNo = c.String(nullable: false, maxLength: 20),
                        Nationality = c.String(nullable: false, maxLength: 100),
                        Address = c.String(nullable: false, maxLength: 500),
                        AddressInCustomerLanguage = c.String(nullable: false, maxLength: 500),
                        Address2 = c.String(maxLength: 500),
                        Address2InCustomerLanguage = c.String(maxLength: 500),
                        CityId = c.Int(nullable: false),
                        PostalCode = c.String(nullable: false, maxLength: 10),
                        CountryId = c.Int(nullable: false),
                        ContactNo = c.String(nullable: false, maxLength: 20),
                        NICDoc = c.String(maxLength: 100),
                        NICDocInCustomerLanguage = c.String(nullable: false, maxLength: 100),
                        PassportDoc = c.String(maxLength: 100),
                        PassportDocInCustomerLanguage = c.String(nullable: false, maxLength: 100),
                        ProofOfAddressDocType = c.String(maxLength: 100),
                        ProofOfAddressDoc = c.String(maxLength: 200),
                        ProofOfAddressDocInCustomerLanguage = c.String(maxLength: 200),
                        ProofOfSourceOfFundsDocType = c.String(maxLength: 100),
                        ProofOfSourceOfFundsDoc = c.String(maxLength: 200),
                        ProofOfSourceOfFundsDocInCustomerLanguage = c.String(maxLength: 200),
                        CustomerSignedForm = c.String(maxLength: 100),
                        CustomerSignedFormInCustomerLanguage = c.String(maxLength: 100),
                        AuthoritySignedForm = c.String(maxLength: 100),
                        AuthoritySignedFormInCustomerLanguage = c.String(maxLength: 100),
                        CurrencyId = c.Int(nullable: false),
                        LanguageId = c.Int(nullable: false),
                        SecurityQuestion = c.String(maxLength: 100),
                        Answer = c.String(maxLength: 100),
                        ValidatedOn = c.DateTime(),
                        ValidationFailureReason = c.String(maxLength: 200),
                        CardServiceProviderClientId = c.String(),
                        SentToCardIssuerOn = c.DateTime(),
                        CardIssuerRespondedOn = c.DateTime(),
                        DeclinedReason = c.String(maxLength: 500),
                        BillingAddress = c.String(maxLength: 200),
                        EmailForShopping = c.String(maxLength: 50),
                        VerificationCode = c.String(maxLength: 20),
                        VerificationCodeExpireOn = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserId)
                .Index(t => t.NIC, unique: true)
                .Index(t => t.PassportNo, unique: true);
            
            CreateTable(
                "dbo.EmailAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Email = c.String(nullable: false, maxLength: 100),
                        DisplayName = c.String(nullable: false, maxLength: 50),
                        Host = c.String(nullable: false, maxLength: 100),
                        Port = c.Int(nullable: false),
                        Username = c.String(nullable: false, maxLength: 100),
                        Password = c.String(nullable: false, maxLength: 250),
                        EnableSSL = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Email, unique: true);
            
            CreateTable(
                "dbo.Languages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        IsRightToLeftOrientation = c.Boolean(nullable: false),
                        IsSystemLanguage = c.Boolean(nullable: false),
                        ValidationFileName = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.LocalizedMessageTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MessageTemplateId = c.Int(nullable: false),
                        LanguageId = c.Int(nullable: false),
                        BccEmailAddresses = c.String(maxLength: 100),
                        Name = c.String(maxLength: 100),
                        Subject = c.String(nullable: false, maxLength: 100),
                        Body = c.String(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        EmailAccountId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EmailAccounts", t => t.EmailAccountId, cascadeDelete: true)
                .Index(t => t.EmailAccountId);
            
            CreateTable(
                "dbo.LocalizedStringResources",
                c => new
                    {
                        ResourceId = c.Int(nullable: false, identity: true),
                        ResourceName = c.String(maxLength: 150),
                        ResourceValue = c.String(maxLength: 200),
                        LanguageId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ResourceId);
            
            CreateTable(
                "dbo.MessageTemplates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RequestFormId = c.Int(nullable: false),
                        PaymentMethodId = c.Int(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TransactionNo = c.String(maxLength: 50),
                        TransactionAccountNo = c.String(maxLength: 50),
                        TransactionName = c.String(maxLength: 50),
                        ConfirmedOn = c.DateTime(),
                        ConfirmationFailureReason = c.String(maxLength: 200),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProxyServers",
                c => new
                    {
                        Domain = c.String(nullable: false, maxLength: 100),
                        IPAddress = c.String(nullable: false, maxLength: 30),
                        Port = c.Int(nullable: false),
                        Username = c.String(maxLength: 50),
                        Password = c.String(maxLength: 50),
                        PacFile = c.String(nullable: false, maxLength: 100),
                        Active = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        Description = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => new { t.Domain, t.IPAddress });
            
            CreateTable(
                "dbo.ProxyServerUsers",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        Domain = c.String(nullable: false, maxLength: 100),
                        CreatedOn = c.DateTime(nullable: false),
                        LastAccessedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.QueuedEmails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Priority = c.Int(nullable: false),
                        From = c.String(nullable: false, maxLength: 100),
                        FromName = c.String(nullable: false, maxLength: 50),
                        To = c.String(nullable: false, maxLength: 100),
                        ToName = c.String(nullable: false, maxLength: 50),
                        CC = c.String(maxLength: 100),
                        Bcc = c.String(maxLength: 100),
                        Subject = c.String(nullable: false, maxLength: 100),
                        Body = c.String(nullable: false),
                        SendTries = c.Int(nullable: false),
                        SentOn = c.DateTime(),
                        EmailAccountId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.QuickTickets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        RoleId = c.Int(nullable: false),
                        PriorityId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RequestForms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeId = c.Int(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        ExchangeRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(maxLength: 250),
                        IsCancelled = c.Boolean(),
                        SentToCardIssuerOn = c.DateTime(),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RoleActions",
                c => new
                    {
                        RoleId = c.Int(nullable: false),
                        ActionId = c.Int(nullable: false),
                        IsAllowed = c.Boolean(nullable: false),
                        DisplayOrder = c.Int(nullable: false),
                        IsDefault = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleId, t.ActionId });
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        Description = c.String(maxLength: 500),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true);
            
            CreateTable(
                "dbo.SecurityQuestions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Question = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Question, unique: true);
            
            CreateTable(
                "dbo.TicketMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TicketId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Message = c.String(nullable: false, maxLength: 2000),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TicketProcesses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TicketId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Comments = c.String(),
                        TicketStatusId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tickets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Subject = c.String(nullable: false, maxLength: 100),
                        TicketNo = c.String(nullable: false, maxLength: 80),
                        CustomerId = c.Int(nullable: false),
                        LastSeenByUserDate = c.DateTime(),
                        LastSeenByUserId = c.Int(),
                        IsResolved = c.Boolean(nullable: false),
                        FilePath = c.String(maxLength: 500),
                        SeverityId = c.Int(nullable: false),
                        Isassign = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TopUpRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TopUpRequestFormId = c.Int(nullable: false),
                        CardId = c.Int(nullable: false),
                        ServiceFee = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        UploadedOn = c.DateTime(),
                        FailureReason = c.String(maxLength: 250),
                        Description = c.String(maxLength: 250),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserActivities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        PerformedForUserId = c.Int(),
                        Action = c.String(nullable: false, maxLength: 100),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 100),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        FirstNameInCustomerLanguage = c.String(maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        LastNameInCustomerLanguage = c.String(maxLength: 50),
                        Email = c.String(nullable: false, maxLength: 50),
                        HashedPassword = c.String(nullable: false, maxLength: 250),
                        HashKey = c.String(nullable: false, maxLength: 100),
                        Status = c.Int(nullable: false),
                        RoleId = c.Int(nullable: false),
                        ChangePasswordRequired = c.Boolean(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Username, unique: true)
                .Index(t => t.Email, unique: true);
            
            CreateTable(
                "dbo.UserSignIns",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LocalizedMessageTemplates", "EmailAccountId", "dbo.EmailAccounts");
            DropIndex("dbo.Users", new[] { "Email" });
            DropIndex("dbo.Users", new[] { "Username" });
            DropIndex("dbo.SecurityQuestions", new[] { "Question" });
            DropIndex("dbo.Roles", new[] { "Name" });
            DropIndex("dbo.LocalizedMessageTemplates", new[] { "EmailAccountId" });
            DropIndex("dbo.Languages", new[] { "Name" });
            DropIndex("dbo.EmailAccounts", new[] { "Email" });
            DropIndex("dbo.Customers", new[] { "PassportNo" });
            DropIndex("dbo.Customers", new[] { "NIC" });
            DropIndex("dbo.Currencies", new[] { "Code" });
            DropIndex("dbo.Currencies", new[] { "Name" });
            DropIndex("dbo.Countries", new[] { "Code" });
            DropIndex("dbo.Countries", new[] { "Name" });
            DropIndex("dbo.Countries", new[] { "CallingCode" });
            DropIndex("dbo.CnCActions", "ActionRoute");
            DropIndex("dbo.CardTypes", new[] { "Name" });
            DropIndex("dbo.Cards", new[] { "Number" });
            DropTable("dbo.UserSignIns");
            DropTable("dbo.Users");
            DropTable("dbo.UserActivities");
            DropTable("dbo.TopUpRequests");
            DropTable("dbo.Tickets");
            DropTable("dbo.TicketProcesses");
            DropTable("dbo.TicketMessages");
            DropTable("dbo.SecurityQuestions");
            DropTable("dbo.Roles");
            DropTable("dbo.RoleActions");
            DropTable("dbo.RequestForms");
            DropTable("dbo.QuickTickets");
            DropTable("dbo.QueuedEmails");
            DropTable("dbo.ProxyServerUsers");
            DropTable("dbo.ProxyServers");
            DropTable("dbo.Payments");
            DropTable("dbo.MessageTemplates");
            DropTable("dbo.LocalizedStringResources");
            DropTable("dbo.LocalizedMessageTemplates");
            DropTable("dbo.Languages");
            DropTable("dbo.EmailAccounts");
            DropTable("dbo.Customers");
            DropTable("dbo.CurrencyRates");
            DropTable("dbo.Currencies");
            DropTable("dbo.Countries");
            DropTable("dbo.CnCSettings");
            DropTable("dbo.CnCActions");
            DropTable("dbo.Cities");
            DropTable("dbo.CardTypes");
            DropTable("dbo.Cards");
            DropTable("dbo.CardRequests");
            DropTable("dbo.AssignTickets");
        }
    }
}
