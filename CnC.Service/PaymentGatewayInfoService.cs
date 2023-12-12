using CnC.Core;
using CnC.Core.Exceptions;
using CnC.Data;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnC.Service
{
    public class PaymentGatewayInfoService
    {
        private static ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(CardService));

        /// <summary>
        /// Return First Payment Gateway Info among active
        /// </summary>
        /// <returns></returns>
        public PaymentGatewayInfo GetActivePaymentGatewayInfo(int gateWayType)
        {
            var type = Type.GetType("CnC.Service.PaymentGateway.Mabna.MabnaPaymentGateway");
            try
            {
                using (var context = new EntityContext())
                {
                    var gateway = context.PaymentGatewaysInfo.Where(x => x.IsActive && x.GatewayType != null ?
                     (x.GatewayType == gateWayType) : x.GatewayType == null).OrderBy(c => c.Id).ToList();
                    var gateways = gateway.Count();
                    PaymentGatewayInfo obj = null;
                    if (gateways > 1)
                    {
                        obj = gateway.Where(c => c.IsActive).FirstOrDefault();
                    }
                    else if (gateways == 1)
                    {
                        obj = gateway.FirstOrDefault();
                    }
                    else
                    {
                        return null;
                    }
                    return obj;
                    //.OrderBy(p => p.IsActive).FirstOrDefault();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public IPaymentGateway GetActivePaymentGateway(int gatewayType)
        {
            try
            {
                PaymentGatewayInfo paymentGatwayInfoActive
                                        = GetActivePaymentGatewayInfo(gatewayType);

                IPaymentGateway paymentGateway = (IPaymentGateway)Activator.CreateInstance
                            (Type.GetType(paymentGatwayInfoActive.ClassPath));

                paymentGateway.PaymentGatewayInfo = paymentGatwayInfoActive;
                return paymentGateway;
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public List<PaymentGatewayInfo> GetPaymentGatewaysInfo()
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return context.PaymentGatewaysInfo.Where(x => x.IsActive).OrderBy(p => p.IsActive).ToList();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }
        public List<PaymentGatewayInfo> GetPaymentGateways()
        {
            try
            {
                using (var context = new EntityContext())
                {
                    return context.PaymentGatewaysInfo.ToList();
                }
            }
            catch (Exception exception)
            {
                new MessageService().SendExceptionMessage(exception);
                log.Error(exception);
                return null;
            }
        }

        public int AddPaymentGateway(PaymentGatewayInfo paymentGateway)
        {
            if (paymentGateway == null)
                throw new ArgumentNullException("Payment Gateway is required");
            try
            {
                using (var context = new EntityContext())
                {
                    context.PaymentGatewaysInfo.Add(paymentGateway);
                    if (context.SaveChanges() <= 0)
                        throw new UserException("Unable to save");

                    return paymentGateway.Id;
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

        public PaymentGatewayInfo GetPaymentGateway(string id)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        int paymentGatewayId = Convert.ToInt32(id);
                        var result = context.PaymentGatewaysInfo.SingleOrDefault(c => c.Id == paymentGatewayId);
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
        public bool UpdatePaymentGateway(PaymentGatewayInfo paymentGateway)
        {
            if (paymentGateway == null)
                throw new ArgumentNullException("Payment Gateway is required");

            try
            {
                using (var context = new EntityContext())
                {

                    var paymentGatewayData = context.PaymentGatewaysInfo.SingleOrDefault(c => c.Id == paymentGateway.Id);

                    if (paymentGatewayData == null)
                        throw new UserException("Payment Gateway with this Id not found");

                    paymentGatewayData.Name = paymentGateway.Name;
                    paymentGatewayData.RedirectUrl = paymentGateway.RedirectUrl;
                    paymentGatewayData.RetrunUrl = paymentGateway.RedirectUrl;
                    paymentGatewayData.EncryptionKey1 = paymentGateway.EncryptionKey1;
                    paymentGatewayData.EncryptionKey2 = paymentGateway.EncryptionKey2;
                    paymentGatewayData.IsActive = paymentGateway.IsActive;
                    paymentGatewayData.ClassPath = paymentGateway.ClassPath;
                    paymentGatewayData.TerminalId = paymentGateway.TerminalId;
                    paymentGatewayData.MerchantId = paymentGateway.MerchantId;

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

    }
}
