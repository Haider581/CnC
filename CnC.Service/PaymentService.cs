using CnC.Core;
using CnC.Core.Exceptions;
using CnC.Core.Payments;
using CnC.Data;
using log4net;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static CnC.Core.PaymentMethod;

namespace CnC.Service
{
    public class PaymentService
    {
        private static log4net.ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(CardService));

        public int AddPayment(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException("Payment cannot be null");

            try
            {
                using (var context = new EntityContext())
                {
                    context.Payments.Add(payment);
                    context.SaveChanges();

                    new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                    return payment.Id;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return -1;
            }
        }

        public List<Payment> GetPaymentsByRequestFormId(int requestFormId)
        {
            if (requestFormId <= 0)
                throw new ArgumentException("Request Form Id is invalid");

            try
            {
                using (var context = new EntityContext())
                {
                    var payments = context.Payments.Where(p => p.RequestFormId == requestFormId).ToList();

                    foreach (var paymentItem in payments)
                    {
                        paymentItem.PaymentMethod = (PaymentMethod)paymentItem.PaymentMethodId;
                    }

                    return payments;
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public List<RequestForm> GetPaymentsPendingForConfirmationOrFailed(out int totalCount, int? customerId = null,
            string nic = null, string passportNo = null, DateTime? createdDateFrom = null, DateTime? createdDateTo = null,
          PaymentMethod? paymentMethod = null, int pageIndex = 0, int? pageSize = null, OrderBy orderBy = OrderBy.Asc
            , bool showFailed = true)
        {
            totalCount = 0;
            try
            {
                if (createdDateTo.HasValue)
                    createdDateTo = createdDateTo.Value.AddDays(1);

                if (!pageSize.HasValue)
                    pageSize = new SettingService().ResultPageSize;

                int skipRows = pageSize.Value * pageIndex;

                using (var context = new EntityContext())
                {
                    var result = (from payment in context.Payments
                                  join rf in context.RequestForms on payment.RequestFormId equals rf.Id
                                  join customer in context.Customers on rf.CustomerId equals customer.UserId
                                  join user in context.Users on customer.UserId equals user.Id
                                  where true == (customerId.HasValue ? customer.UserId == customerId :
                                  !string.IsNullOrEmpty(nic) ?
                                  customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase) :
                                  !string.IsNullOrEmpty(passportNo) ?
                                  customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true)
                                  && payment.CreatedOn >= (createdDateFrom.HasValue ?
                                  createdDateFrom.Value : payment.CreatedOn)
                                  && payment.CreatedOn <= (createdDateTo.HasValue ?
                                  createdDateTo.Value : payment.CreatedOn)
                                   && payment.PaymentMethodId != (int)PaymentMethod.Admin
                                  && payment.PaymentMethodId != (int)PaymentMethod.RAHYAB
                                  && payment.PaymentMethodId == (paymentMethod.HasValue ?
                                  (int)paymentMethod.Value : payment.PaymentMethodId)
                                  && ((payment.ConfirmedOn == null)
                                  || (payment.ReConfirmedOn == null && string.IsNullOrEmpty(payment.ConfirmationFailureReason))
                                  || (showFailed && !string.IsNullOrEmpty(payment.ConfirmationFailureReason)))
                                  group
                                  new
                                  {
                                      RequestForm = rf,
                                      Customer = customer,
                                      User = user,
                                      Payment = payment
                                  }
                                  by rf.Id).OrderByDescending(r => r.FirstOrDefault().RequestForm.CreatedOn);

                    totalCount = result.Count();
                    var customerCards = result.Skip(skipRows).Take(pageSize.Value).ToList();

                    if (customerCards != null && customerCards.Count() > 0)
                    {
                        List<RequestForm> requestForms = new List<RequestForm>();

                        foreach (var grp in customerCards)
                        {
                            var firstRecord = grp.First();
                            RequestForm requestForm = firstRecord.RequestForm;
                            requestForm.Customer = firstRecord.Customer;
                            requestForm.Customer.User = firstRecord.User;
                            requestForm.Payments = new List<Payment>();

                            foreach (var record in grp.DistinctBy(r => r.Payment))
                            {
                                record.Payment.StatusString = GetPaymentStatusString(record.Payment);
                                requestForm.Payments.Add(record.Payment);
                            }

                            requestForms.Add(requestForm);
                        }

                        return requestForms;
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

        public List<RequestForm> GetPayments(out int totalCount, int? customerId = null, string nic = null
            , string passportNo = null, DateTime? createdDateFrom = null, DateTime? createdDateTo = null
            , PaymentStatus? paymentStatus = null, int? requestFormTypeId = null, PaymentMethod? paymentMethod = null
            , bool isForCustomer = false, int pageIndex = 0, int? pageSize = null, OrderBy orderBy = OrderBy.Asc)
        {
            totalCount = 0;
            try
            {
                if (createdDateTo.HasValue)
                    createdDateTo = createdDateTo.Value.AddDays(1);

                if (!pageSize.HasValue)
                    pageSize = new SettingService().ResultPageSize;

                int skipRows = pageSize.Value * pageIndex;

                using (var context = new EntityContext())
                {
                    var result = (from payment in context.Payments
                                  join rf in context.RequestForms on payment.RequestFormId equals rf.Id
                                  join customer in context.Customers on rf.CustomerId equals customer.UserId
                                  join user in context.Users on customer.UserId equals user.Id
                                  where true == (customerId.HasValue ? customer.UserId == customerId :
                                  !string.IsNullOrEmpty(nic) ? customer.NIC.Equals(nic, StringComparison.OrdinalIgnoreCase) :
                                  !string.IsNullOrEmpty(passportNo) ? customer.PassportNo.Equals(passportNo, StringComparison.OrdinalIgnoreCase) : true)
                                  && payment.CreatedOn >= (createdDateFrom.HasValue ? createdDateFrom.Value : payment.CreatedOn)
                                  && payment.CreatedOn <= (createdDateTo.HasValue ? createdDateTo.Value : payment.CreatedOn)
                                  && payment.PaymentMethodId != (int)PaymentMethod.Admin
                                  && payment.PaymentMethodId != (int)PaymentMethod.RAHYAB
                                  && payment.PaymentMethodId == (paymentMethod.HasValue ? (int)paymentMethod.Value : payment.PaymentMethodId)
                                  && true == (paymentStatus.HasValue ?
                                  (paymentStatus.Value == PaymentStatus.Pending
                                  && payment.ConfirmedOn == null
                                  && payment.ReConfirmedOn == null
                                  && string.IsNullOrEmpty(payment.ConfirmationFailureReason))
                                  || (paymentStatus.Value == PaymentStatus.Confirmed
                                  && payment.ConfirmedOn != null && payment.ReConfirmedOn != null
                                  && string.IsNullOrEmpty(payment.ConfirmationFailureReason))
                                  || (paymentStatus.Value == PaymentStatus.ConfirmationFailed
                                  && payment.ConfirmedOn != null && !string.IsNullOrEmpty(payment.ConfirmationFailureReason))
                                  : true)
                                  group
                                  new
                                  {
                                      RequestForm = rf,
                                      Customer = customer,
                                      User = user,
                                      Payment = payment
                                  }
                                  by rf.Id).OrderByDescending(r => r.FirstOrDefault().RequestForm.CreatedOn);

                    totalCount = result.Count();
                    var payments = result.Skip(skipRows).Take(pageSize.Value).ToList();
                    if (payments != null && payments.Count() > 0)
                    {
                        List<RequestForm> requestForms = new List<RequestForm>();

                        foreach (var grp in payments)
                        {
                            var firstRecord = grp.First();
                            RequestForm requestForm = firstRecord.RequestForm;
                            requestForm.Customer = firstRecord.Customer;
                            requestForm.Customer.User = firstRecord.User;
                            requestForm.Payments = new List<Payment>();

                            foreach (var record in grp.DistinctBy(r => r.Payment))
                            {
                                record.Payment.StatusString = GetPaymentStatusString(record.Payment, isForCustomer);
                                requestForm.Payments.Add(record.Payment);
                            }

                            requestForms.Add(requestForm);
                        }

                        return requestForms;
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

        /// <summary>
        /// Set Payment Confirmed or Failed
        /// In case of Confirmed pass Failure Reason null
        /// In case of Failed pass message in Failure Reason
        /// </summary>
        public bool SetPaymentConfirmedOrFailed(int paymentId, string failureReason)
        {
            if (paymentId <= 0)
                throw new ArgumentNullException("Payment Id is invalid");

            try
            {
                using (var context = new EntityContext())
                {
                    var payment = (from p in context.Payments
                                   join rf in context.RequestForms on p.RequestFormId equals rf.Id
                                   join customer in context.Customers on rf.CustomerId equals customer.UserId
                                   join user in context.Users on customer.UserId equals user.Id
                                   where p.Id == paymentId
                                   && (p.ConfirmedOn == null
                                   || (p.ConfirmedOn != null || p.ReConfirmedOn != null &&
                                   !string.IsNullOrEmpty(p.ConfirmationFailureReason)))
                                   select new { Payment = p, Customer = customer, User = user }).SingleOrDefault();

                    if (payment == null)
                        throw new UserException("Either Payment not found or already confirmed");

                    if (payment.Payment.ConfirmedOn == null ||
                        (payment.Payment.ConfirmedOn != null &&
                        !string.IsNullOrEmpty(payment.Payment.ConfirmationFailureReason)
                        && payment.Payment.ReConfirmedOn == null))
                    {
                        payment.Payment.ConfirmedOn = DateTime.Now;

                        // If Payment Confirmation is passed and Reconfirmation not required then stamp it too
                        if (!new SettingService().PaymentReconfirmationRequired && string.IsNullOrEmpty(failureReason))
                            payment.Payment.ReConfirmedOn = DateTime.Now;

                        payment.Payment.ConfirmationFailureReason = string.IsNullOrEmpty(failureReason) ?
                            null : failureReason;

                        if (context.SaveChanges() <= 0)
                            throw new UserException("Unable to save");

                        // If Payment Confirmation failed then send Customer email with reason
                        if (!string.IsNullOrEmpty(failureReason))
                        {
                            payment.Customer.User = payment.User;
                            new MessageService().
                                SendCustomerPaymentConfirmationFailedMessage(payment.Customer, payment.Payment
                                , failureReason, payment.Customer.LanguageId);
                        }

                        new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name
                            , performedForUserId: payment.Customer.UserId);

                        return true;
                    }

                    if (payment.Payment.ReConfirmedOn == null ||
                        (payment.Payment.ReConfirmedOn != null
                        && !string.IsNullOrEmpty(payment.Payment.ConfirmationFailureReason)))
                    {
                        payment.Payment.ReConfirmedOn = DateTime.Now;
                        payment.Payment.ConfirmationFailureReason = string.IsNullOrEmpty(failureReason) ?
                            null : failureReason;
                        if (context.SaveChanges() <= 0)
                            throw new UserException("Unable to save");

                        // If Payment Confirmation failed then send Customer email with reason
                        if (!string.IsNullOrEmpty(failureReason))
                        {
                            payment.Customer.User = payment.User;
                            new MessageService()
                                .SendCustomerPaymentConfirmationFailedMessage(payment.Customer, payment.Payment
                                , failureReason, payment.Customer.LanguageId);
                        }

                        new AuditService().LogAction(action: MethodBase.GetCurrentMethod().Name);

                        return true;
                    }

                    return false;

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

        /// <summary>
        /// Return Payment Status String
        /// </summary>
        public string GetPaymentStatusString(Payment payment, bool isForCustomer = false)
        {
            try
            {
                if (isForCustomer)
                {
                    if (payment.ConfirmedOn != null && payment.ReConfirmedOn != null
                        && string.IsNullOrEmpty(payment.ConfirmationFailureReason))
                        return "Confirmed";
                    else if ((payment.ConfirmedOn != null || payment.ReConfirmedOn != null)
                        && !string.IsNullOrEmpty(payment.ConfirmationFailureReason))
                        return "Confirmation Failed";
                    else
                        return "Confirmation Underway";
                }
                else
                {
                    if (payment.ConfirmedOn != null && payment.ReConfirmedOn != null
                        && string.IsNullOrEmpty(payment.ConfirmationFailureReason))
                        return "Confirmed";
                    else if ((payment.ConfirmedOn != null || payment.ReConfirmedOn != null)
                        && !string.IsNullOrEmpty(payment.ConfirmationFailureReason))
                        return "Confirmation Failed";
                    else
                        return "Pending";
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
