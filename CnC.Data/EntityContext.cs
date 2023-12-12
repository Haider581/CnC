using CnC.Core;
using CnC.Core.Accounts;
using CnC.Core.Affiliates;
using CnC.Core.Audit;
using CnC.Core.Cards;
using CnC.Core.Common;
using CnC.Core.Customers;
using CnC.Core.Discounts;
using CnC.Core.Localized;
using CnC.Core.Messages;
using CnC.Core.Payments;
using CnC.Core.Proxy;
using CnC.Core.TopUps;
using System;
using System.Data.Entity;
using System.Linq;

namespace CnC.Data
{
    public class EntityContext : DbContext
    {
        public EntityContext() : base("name=DbConnectionString") { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<EntityContext>(null);
            base.OnModelCreating(modelBuilder);
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Add some logging with log4net here


                // Throw a new DbEntityValidationException with the improved exception message.
                throw new System.Data.Entity.Validation.DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);

            }
        }

        #region Accounts

        public DbSet<CnCAction> CnCActions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleAction> RoleActions { get; set; }
        public DbSet<User> Users { get; set; }

        #endregion

        #region Audit

        public DbSet<UserActivity> UserActivities { get; set; }
        public DbSet<UserSignIn> UserSignIns { get; set; }

        #endregion

        #region RequestForm

        public DbSet<RequestForm> RequestForms { get; set; }

        #endregion

        #region Cards

        public DbSet<Card> Cards { get; set; }
        public DbSet<CardRequest> CardRequests { get; set; }
        public DbSet<CardType> CardTypes { get; set; }

        #endregion

        #region TopUps

        public DbSet<TopUpRequest> TopUpRequests { get; set; }

        #endregion

        #region Customers

        public DbSet<Customer> Customers { get; set; }
        public DbSet<SecurityQuestion> SecurityQuestions { get; set; }

        #endregion

        #region Payments

        public DbSet<Payment> Payments { get; set; }

        #endregion               

        #region Common

        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrencyRate> CurrencyRates { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<CnCSetting> CnCSettings { get; set; }

        #endregion

        #region Messages

        public DbSet<EmailAccount> EmailAccounts { get; set; }
        public DbSet<MessageTemplate> MessageTemplates { get; set; }
        public DbSet<LocalizedMessageTemplate> LocalizedMessageTemplates { get; set; }
        public DbSet<QueuedEmail> QueuedEmails { get; set; }

        #endregion

        #region Ticket

        public DbSet<TicketMessage> TicketMessages { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<AssignTicket> AssignTickets { get; set; }

        public DbSet<QuickTicket> QuickTickets { get; set; }

        public DbSet<TicketProcess> TicketProcesses { get; set; }

        #endregion

        #region Proxy

        public DbSet<ProxyServer> ProxyServers { get; set; }
        public DbSet<ProxyServerUser> ProxyServerUsers { get; set; }

        #endregion

        #region Localization

        public DbSet<LocalizedStringResource> LocalizedStringResources { get; set; }

        #endregion

        #region Affiliate

        public DbSet<Affiliate> Affiliates { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Commission> Commissions { get; set; }
        public DbSet<AffiliateDiscount> AffiliateDiscounts { get; set; }

        #endregion

        #region Payment Gateways Info

        public DbSet<PaymentGatewayInfo> PaymentGatewaysInfo { get; set; }

        #endregion Payment Gateways Info

        #region Discounts&Prmotions

        public DbSet<DiscountPromotion> DiscountPromotions { get; set; }

        #endregion

    }
}
