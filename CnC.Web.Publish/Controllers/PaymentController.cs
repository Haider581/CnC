using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CnC.Web.Dev;
using CnC.Service;

namespace CnC.Web.Controllers
{
    [RoleActionValidator]
    public class PaymentController : Controller
    {
        [HttpPost]
        public ActionResult BankToBankTransaction(int customerId)
        {
            return View();
        }

        public ActionResult TopupPaymentSuccess()
        {
            return View();
        }

        public ActionResult TopupPaymentFail()
        {
            return View();
        }
        public ActionResult CardPaymentSuccess()
        {
            return View();
        }

        public ActionResult CardPaymentFail()
        {
            return View();
        }

        #region Payment Confirmation

        public ActionResult PaymentConfirmation()
        {
            var payments = new PaymentService().GetPaymentsPendingForConfirmationOrFailed();
            return View(payments);
        }

        [HttpPost]
        public ActionResult PaymentConfirmation(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            string nic = null;
            string passport = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;

            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(Request.Form["txtPassportNo"]))
                passport = Request.Form["txtPassportNo"];

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            return View(new PaymentService().GetPaymentsPendingForConfirmationOrFailed(nic: nic, passportNo: passport,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo));
        }

        [HttpGet]
        public JsonResult SetPaymentConfirmedOrFailed(string key, string failureReason)
        {
            var statusArray = new string[3];
            if (!string.IsNullOrEmpty(key))
            {
                try
                {
                    if (string.IsNullOrEmpty(failureReason))
                    {
                        // Confirmed
                        var isUpdated = new PaymentService().SetPaymentConfirmedOrFailed(Convert.ToInt32(key), null);
                        if (isUpdated)
                        {
                            statusArray[0] = "200";
                            statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Status updated successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Payment/PaymentConfirmation'>Go back</a></span>";
                            return Json(statusArray, JsonRequestBehavior.AllowGet);
                        }
                        statusArray[0] = "101";
                        statusArray[1] = "Unable to update.";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                    else if (!string.IsNullOrEmpty(failureReason) && !string.IsNullOrEmpty(key))
                    {
                        var isUpdated = new PaymentService().SetPaymentConfirmedOrFailed(Convert.ToInt32(key), failureReason);
                        if (isUpdated)
                        {
                            statusArray[0] = "200";
                            statusArray[1] = "<span id='spanSuccess'><span id='completeSpan' class='text-center text-success'><strong><i class='fa fa-check fa-lg'></i> Complete</strong></span><br />Status updated successfully!<br /><a id='lkGoBack' style='font-size: 14px; ' href='/Payment/PaymentConfirmation'>Go back</a></span>";
                            return Json(statusArray, JsonRequestBehavior.AllowGet);
                        }
                        statusArray[0] = "101";
                        statusArray[1] = "Unable to update.";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);

                    }
                    else
                    {
                        statusArray[0] = "101";
                        statusArray[1] = "Missing required values.";
                        return Json(statusArray, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = exp.Message;
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region Payment Search   

        public ActionResult PaymentsSearch()
        {
            var payments = new PaymentService().GetPayments();
            return View(payments);
        }

        [HttpPost]
        public ActionResult PaymentsSearch(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            string nic = null;
            string passport = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;

            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(Request.Form["txtPassportNo"]))
                passport = Request.Form["txtPassportNo"];

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateFrom"]))
            {
                DateTime txtCreatedDateFrom;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateFrom"], out txtCreatedDateFrom))
                {
                    return View();
                }

                createdDateFrom = txtCreatedDateFrom;
            }

            if (!string.IsNullOrEmpty(formCollection["txtCreatedDateTo"]))
            {
                DateTime txtCreatedDateTo;
                if (!DateTime.TryParse(Request.Form["txtCreatedDateTo"], out txtCreatedDateTo))
                {
                    return View();
                }

                createdDateTo = txtCreatedDateTo;
            }

            return View(new PaymentService().GetPayments(nic: nic, passportNo: passport,
                createdDateFrom: createdDateFrom, createdDateTo: createdDateTo));
        }

        public ActionResult PendingPayments()
        {
            var payments = new PaymentService().GetPaymentsPendingForConfirmationOrFailed();
            return View(payments);
        }

        [HttpPost]
        public ActionResult PendingPayments(FormCollection formCollection)
        {
            return View();
        }

        #endregion
    }
}