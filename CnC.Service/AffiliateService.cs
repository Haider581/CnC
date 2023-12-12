using CnC.Core.Affiliates;
using CnC.Core.Audit;
using CnC.Core.Caching;
using CnC.Core.Exceptions;
using CnC.Data;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CnC.Service
{
    public class AffiliateService
    {
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(AffiliateService));

        public Affiliate GetAffiliate(int affiliateId)
        {
            if (affiliateId <= 0)
                return null;

            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from a in context.Affiliates
                                  join addr in context.Addresses on a.AddressId equals addr.Id
                                  join country in context.Countries on addr.CountryId equals country.Id
                                  join city in context.Cities on addr.CityId equals city.Id
                                  select new
                                  {
                                      affiliate = a,
                                      address = addr,
                                      country = country,
                                      city = city,
                                  }).SingleOrDefault(a => a.affiliate.Id == affiliateId);

                    if (result == null)
                        return null;

                    result.affiliate.Address = result.address;
                    result.affiliate.Address.City = result.city;
                    result.affiliate.Address.Country = result.country;

                    return result.affiliate;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }
        public List<Affiliate> GetAffiliates(out int totalCount, string firstName = null, string lastName = null
                                            , string email = null, int pageIndex = 0, int? pageSize = null
                                            , bool showInActive = false)
        {
            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;

            int skipRows = pageSize.Value * pageIndex;
            totalCount = 0;

            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from a in context.Affiliates
                                  join addr in context.Addresses on a.AddressId equals addr.Id
                                  where (showInActive ? (a.Active || !a.Active) : a.Active)
                                  && (true == (!string.IsNullOrEmpty(firstName)
                                  ? addr.FirstName.Contains(firstName.ToLower())
                                  : true))
                                   && (true == (!string.IsNullOrEmpty(lastName)
                                  ? addr.LastName.Contains(lastName.ToLower())
                                  : true))
                                   && (true == (!string.IsNullOrEmpty(email)
                                  ? addr.Email.Contains(email.ToLower())
                                  : true))
                                  select new
                                  {
                                      affiliate = a,
                                      address = addr
                                  }).OrderByDescending(a => a.affiliate.Id);

                    totalCount = result.Count();
                    var affiliates = new List<Affiliate>();

                    if (result != null && totalCount > 0)
                    {
                        foreach (var affil in result)
                        {
                            var affiliate = new Affiliate();
                            affiliate = affil.affiliate;
                            affiliate.Address = affil.address;
                            //affiliate.Address.City = a.city;
                            //affiliate.Address.Country = a.country;

                            affiliates.Add(affiliate);
                        }

                        return affiliates.Skip(skipRows).Take(pageSize.Value).ToList();
                    }

                    return null;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }
        public int AddAffiliate(Affiliate affiliate)
        {
            if (affiliate == null)
                throw new ArgumentNullException("Affiliate is required");

            try
            {
                using (var context = new EntityContext())
                {
                    if (context.Addresses.Where(add => add.Email.Equals(affiliate.Address.Email)).Count() > 0)
                        throw new UserException("Email Address Already Exists");

                    context.Addresses.Add(affiliate.Address);
                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    affiliate.AddressId = affiliate.Address.Id;
                    affiliate.Active = affiliate.Active;
                    affiliate.Deleted = affiliate.Deleted;

                    context.Affiliates.Add(affiliate);

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    return affiliate.Id;
                }
            }
            catch (UserException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                throw new UserException("Sorry, we are unable to process your request. Please try again later.");
            }
        }

        public bool UpdateAffiliate(Affiliate affiliate)
        {
            if (affiliate == null)
                throw new ArgumentNullException("Affiliate is required");

            try
            {
                using (var context = new EntityContext())
                {
                    var checkAffiliate = (from a in context.Affiliates
                                          join addr in context.Addresses on a.AddressId equals addr.Id
                                          //join country in context.Countries on addr.CountryId equals country.Id
                                          //join city in context.Cities on addr.CityId equals city.Id
                                          //where a.AddressId == addr.Id
                                          select new
                                          {
                                              affiliate = a,
                                              address = addr,
                                              //country = country,
                                              //city = city,
                                          }).SingleOrDefault(a => a.affiliate.Id == affiliate.Id);

                    if (checkAffiliate == null)
                        throw new UserException("Affiliate with this Id not found");

                    checkAffiliate.address.FirstName = affiliate.Address.FirstName;
                    checkAffiliate.address.LastName = affiliate.Address.LastName;
                    checkAffiliate.address.Email = affiliate.Address.Email;
                    checkAffiliate.address.Company = affiliate.Address.Company;
                    checkAffiliate.address.CountryId = affiliate.Address.CountryId;
                    checkAffiliate.address.CityId = affiliate.Address.CityId;
                    checkAffiliate.address.Address1 = affiliate.Address.Address1;
                    checkAffiliate.address.Address2 = affiliate.Address.Address2;
                    checkAffiliate.address.ZipPostalCode = affiliate.Address.ZipPostalCode;
                    checkAffiliate.address.PhoneNumber = affiliate.Address.PhoneNumber;
                    checkAffiliate.address.FaxNumber = affiliate.Address.FaxNumber;


                    checkAffiliate.affiliate.AddressId = affiliate.Address.Id;
                    checkAffiliate.affiliate.Comment = affiliate.Comment;
                    checkAffiliate.affiliate.FriendlyUrlName = affiliate.FriendlyUrlName;
                    checkAffiliate.affiliate.Deleted = affiliate.Deleted;
                    checkAffiliate.affiliate.Active = affiliate.Active;

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    return true;
                }
            }
            catch (UserException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }

        public List<AffiliateDiscount> GetAffiliateDiscounts(out int totalCount, int pageIndex = 0
            , int? pageSize = null, int? cardTypeId = null)
        {
            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;

            int skipRows = pageSize.Value * pageIndex;
            totalCount = 0;

            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from query in context.AffiliateDiscounts
                                  where query.CardTypeId == (cardTypeId.HasValue && cardTypeId.Value > 0 ? cardTypeId : query.CardTypeId)
                                  select query).ToList();
                    totalCount = result.Count();

                    if (result != null)
                    {
                        return result.Skip(skipRows).Take(pageSize.Value).ToList();
                    }
                    return null;
                }
            }

            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }

        }
        public AffiliateDiscount GetAffiliateDiscount(string id)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        int aaffiliateDiscountId = Convert.ToInt32(id);
                        var result = context.AffiliateDiscounts.SingleOrDefault(c => c.Id == aaffiliateDiscountId);
                        if (result != null)
                        {
                            return result;
                        }
                        return null;
                    }
                    return null;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }
        public bool UpdateAffiliateDiscount(AffiliateDiscount affiliateDiscount)
        {
            if (affiliateDiscount == null)
                throw new ArgumentNullException("Affiliate Discount is required");

            try
            {
                using (var context = new EntityContext())
                {
                    var affiliateDiscountResult = context.AffiliateDiscounts
                                                         .SingleOrDefault(c => c.Id == affiliateDiscount.Id);

                    if (affiliateDiscountResult.AffiliateId != affiliateDiscount.AffiliateId)
                        throw new UserException("Affiliate Id is a Readonly Property, you cannot update it");

                    if ((affiliateDiscount.CardTypeId != affiliateDiscountResult.CardTypeId)
                        || affiliateDiscount.StartOn != affiliateDiscountResult.StartOn
                        || affiliateDiscount.EndOn != affiliateDiscountResult.EndOn)
                        if (IsAffiliateDiscountAlreadyAvailable(affiliateDiscount))
                            throw new UserException("Discount already availaible for this Affiliate for \n"
                                                    + "same Card in same Date Range");

                    if (affiliateDiscountResult == null)
                        throw new UserException("Affiliate Discount with this Id not found");

                    affiliateDiscountResult.AffiliateId = affiliateDiscount.AffiliateId;
                    affiliateDiscountResult.CardTypeId = affiliateDiscount.CardTypeId;
                    affiliateDiscountResult.Discount = affiliateDiscount.Discount;
                    affiliateDiscountResult.IsPercent = affiliateDiscount.IsPercent;
                    affiliateDiscountResult.StartOn = affiliateDiscount.StartOn;
                    affiliateDiscountResult.EndOn = affiliateDiscount.EndOn;
                    affiliateDiscountResult.Active = affiliateDiscount.Active;

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    return true;

                }
            }
            catch (UserException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }

        public int AddAffiliateDiscount(List<AffiliateDiscount> affiliateDiscounts)
        {
            if (affiliateDiscounts == null || affiliateDiscounts.Count() <= 0)
                throw new ArgumentNullException("Affiliate Discount is required");

            try
            {
                using (var context = new EntityContext())
                {
                    List<AffiliateDiscount> listAffiliateDiscounts = new List<AffiliateDiscount>();
                    for (int j = 0; j < affiliateDiscounts.Count(); j++)
                    {
                        if ((affiliateDiscounts[j].CardTypeIds == null && affiliateDiscounts[j].CardTypeIds.Count() < 0)
                            || affiliateDiscounts[j].StartOn == null || affiliateDiscounts[j].EndOn == null
                            || affiliateDiscounts[j].Discount <= 0)
                            throw new UserException("Missing Fields Required");

                        for (int i = 0; i < affiliateDiscounts[j].CardTypeIds.Count(); i++)
                        {
                            if (affiliateDiscounts[j].CardTypeIds[i].Contains(","))
                                throw new UserException("Card Type is not valid");

                            var affiliateDiscount = new AffiliateDiscount();
                            affiliateDiscount = affiliateDiscounts[j];
                            affiliateDiscount.CardTypeId = Convert.ToInt32(affiliateDiscounts[j].CardTypeIds[i]);

                            if (IsAffiliateDiscountAlreadyAvailable(affiliateDiscount))
                                throw new UserException("Discount already availaible for " + affiliateDiscount.AffiliateId + "-" + new AffiliateService().GetAffiliate(affiliateDiscount.AffiliateId).Address.FirstName + " within " + affiliateDiscount.StartOn + "-" + affiliateDiscount.EndOn + " against " + new CardService().GetCardType(affiliateDiscount.CardTypeId).Name + " Card");

                            //if (DateTime.Now < affiliateDiscount.StartOn)
                            //    throw new UserException("Start Date cannot be greater than Current Date");

                            context.AffiliateDiscounts.Add(affiliateDiscount);
                            context.SaveChanges();
                        }
                    }

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    return 1;
                }
            }
            catch (UserException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return 0;
            }
        }

        private bool IsAffiliateDiscountAlreadyAvailable(AffiliateDiscount affiliateDiscount)
        {
            using (var context = new EntityContext())
            {
                //bool isAvailable = context.AffiliateDiscounts.Where(
                //                                       ad => (ad.AffiliateId == affiliateDiscount.AffiliateId)
                //                                       && (ad.CardTypeId == affiliateDiscount.CardTypeId)
                //                                       && ((ad.StartOn <= affiliateDiscount.StartOn
                //                                            && ad.EndOn >= affiliateDiscount.EndOn)
                //                                            || (ad.StartOn <= affiliateDiscount.EndOn
                //                                            && ad.EndOn <= affiliateDiscount.EndOn)))
                //                                            .Count() > 0;

                var discounts = context.AffiliateDiscounts.Where(c => c.AffiliateId == affiliateDiscount.AffiliateId)
                                                         .ToList();

                foreach (var discount in discounts)
                {
                    if ((discount.CardTypeId == affiliateDiscount.CardTypeId)
                        && ((discount.StartOn <= affiliateDiscount.StartOn && discount.EndOn >= affiliateDiscount.EndOn)
                            || (discount.StartOn <= affiliateDiscount.EndOn && affiliateDiscount.EndOn <= discount.EndOn)
                            || (affiliateDiscount.StartOn <= discount.StartOn
                            && affiliateDiscount.EndOn >= discount.EndOn)))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        private bool IsDiscountAlreadyAvailable(AffiliateDiscount affiliateDiscount)
        {
            using (var context = new EntityContext())
            {
                return context.DiscountPromotions.Where(
                                                       dp => dp.CardTypeId.Equals(affiliateDiscount.CardTypeId)
                                                       && (dp.StartOn <= affiliateDiscount.StartOn
                                                            && dp.EndOn >= affiliateDiscount.EndOn)
                                                            || (dp.StartOn >= affiliateDiscount.StartOn
                                                            && dp.EndOn <= affiliateDiscount.EndOn))
                                                            .Count() > 0;
            }
        }
        public List<Affiliate> GetAffiliates()
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var affiliates = (from affiliate in context.Affiliates
                                      join address in context.Addresses on affiliate.AddressId equals address.Id
                                      select new
                                      {
                                          Affiliate = affiliate,
                                          Address = address
                                      }).ToList();

                    List<Affiliate> listAffiliates = new List<Affiliate>();
                    for (int i = 0; i < affiliates.Count(); i++)
                    {
                        affiliates[i].Affiliate.Address = affiliates[i].Address;
                        listAffiliates.Add(affiliates[i].Affiliate);
                    }
                    return listAffiliates;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }
    }
}
