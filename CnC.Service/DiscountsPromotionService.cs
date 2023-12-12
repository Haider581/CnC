using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CnC.Core.Discounts;
using CnC.Data;
using CnC.Core.Exceptions;

namespace CnC.Service
{
    public class DiscountsPromotionService
    {
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(CardService));
        public List<DiscountPromotion> GetDiscountsPromotions()
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var discountsPromotions = (from query in context.DiscountPromotions
                                               where query.Active && (DateTime.Now >= query.StartOn
                                               && DateTime.Now <= query.EndOn)
                                               select query).ToList();

                    return discountsPromotions;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }
        public List<DiscountPromotion> GetAllDiscountsPromotions(out int totalCount, int pageIndex = 0,
                                                                 int? pageSize = null)
        {
            if (!pageSize.HasValue)
                pageSize = new SettingService().ResultPageSize;

            int skipRows = pageSize.Value * pageIndex;
            totalCount = 0;
            try
            {
                using (var context = new EntityContext())
                {
                    var discountsPromotions = (from query in context.DiscountPromotions
                                               select query);
                    totalCount = discountsPromotions.Count();
                    return discountsPromotions.OrderBy(c => c.Id).Skip(skipRows).Take(pageSize.Value).ToList();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }
        public DiscountPromotion GetDiscountPromotions(string id)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        int discountPromotionId = Convert.ToInt32(id);
                        var result = context.DiscountPromotions.SingleOrDefault(c => c.Id == discountPromotionId);
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
            //return 
        }
        public bool UpdateDiscountPrmotion(DiscountPromotion discountPromotion)
        {
            if (discountPromotion == null)
                throw new ArgumentNullException("Discount Promotion is required");

            try
            {
                using (var context = new EntityContext())
                {
                    if (IsDiscountAlreadyAvailable(discountPromotion))
                        throw new UserException("Discount for this Card already exists within same Date Range");

                    var discountPromotionResult = context.DiscountPromotions
                                                  .SingleOrDefault(c => c.Id == discountPromotion.Id);

                    if (discountPromotionResult == null)
                        throw new UserException("Affiliate Discount with this Id not found");

                    discountPromotionResult.CardTypeId = discountPromotion.CardTypeId;
                    discountPromotionResult.Discount = discountPromotion.Discount;
                    discountPromotionResult.IsPercent = discountPromotion.IsPercent;
                    discountPromotionResult.StartOn = discountPromotion.StartOn;
                    discountPromotionResult.EndOn = discountPromotion.EndOn;
                    discountPromotionResult.Active = discountPromotion.Active;

                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    return true;

                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }

        public int AddDiscountPromotion(DiscountPromotion discountPromotion)
        {
            if (discountPromotion == null)
                throw new ArgumentNullException("Discount Promotion is required");

            try
            {
                using (var context = new EntityContext())
                {
                    if (IsDiscountAlreadyAvailable(discountPromotion))
                        throw new UserException("Discount for this Card already exists within same Date Range");

                    context.DiscountPromotions.Add(discountPromotion);
                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    return discountPromotion.Id;
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
        private bool IsDiscountAlreadyAvailable(DiscountPromotion discountPromotion)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return context.DiscountPromotions.Where(
                                                           dp => dp.CardTypeId == discountPromotion.CardTypeId
                                                           && (dp.StartOn <= discountPromotion.StartOn
                                                                && dp.EndOn >= discountPromotion.EndOn)
                                                                || (dp.StartOn >= discountPromotion.StartOn
                                                                && dp.EndOn <= discountPromotion.EndOn))
                                                                .Count() > 0;
                }
            }
            catch(Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return false;
            }
        }
    }
}
