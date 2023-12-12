using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CnC.Web.Helper;
using CnC.Service;
using CnC.Core.Exceptions;
using ClosedXML.Excel;
using System.IO;
using CnC.Core;
using System.Configuration;

namespace CnC.Web.Controllers
{
    [Authorize]
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

        public void PaymentPendingForConfirmation_ExportToExcel(List<RequestForm> payments, string fileName)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(new Utility().GetTable(payments));
                wb.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                wb.Style.Font.Bold = true;

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "";
                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                Response.AddHeader("content-disposition", "attachment;filename= " + fileName + ".xlsx");

                using (MemoryStream myMemoryStream = new MemoryStream())
                {
                    wb.SaveAs(myMemoryStream);
                    myMemoryStream.WriteTo(Response.OutputStream);
                    Response.Flush();
                    Response.End();
                }
            }
        }

        public ActionResult PaymentConfirmation(int page = 0)
        {
            int totalCount = 0;
            int pageSize = new SettingService().ResultPageSize;
            var paymentsPendingForConfirmationOrFailed = new PaymentService()
                .GetPaymentsPendingForConfirmationOrFailed(out totalCount, pageIndex: page, pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(paymentsPendingForConfirmationOrFailed);
        }

        [HttpPost]
        public ActionResult PaymentConfirmation(FormCollection formCollection)
        {
            string nic = null;
            string passport = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int totalCount = 0;
            int page = 0;
            string showFailedPayments = string.Empty;
            bool isShowFailedPayments = true;

            if (!string.IsNullOrEmpty(formCollection["ShowFailedPayments"]))
            {
                showFailedPayments = Request.Form["ShowFailedPayments"];
                if (showFailedPayments.Contains("1"))
                    isShowFailedPayments = true;
                else
                    isShowFailedPayments = false;
            }

            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
            {
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            }
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

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

            if (!string.IsNullOrEmpty(formCollection["exportPaymentPendingForConfirmation"]))
            {
                if (createdDateFrom == null || createdDateTo == null)
                {
                    ViewBag.Message = "Date From and To are required";
                    return View();
                }

                var payments = new PaymentService()
                    .GetPaymentsPendingForConfirmationOrFailed(out totalCount, createdDateFrom: createdDateFrom
                    , createdDateTo: createdDateTo, pageSize: 500);

                if (payments != null && payments.Count > 0)
                {
                    string fileName = "PaymentsPendingForConfirmation_" +
                        createdDateFrom.Value.ToString("yyyy-MM-dd") + "_"
                        + createdDateTo.Value.ToString("yyyy-MM-dd");
                    PaymentPendingForConfirmation_ExportToExcel(payments, fileName);
                    ViewBag.Message = "";
                    return null;
                }
                else
                {
                    nic = null;
                    passport = null;
                    createdDateFrom = null;
                    createdDateTo = null;
                    ViewBag.Message = "No record found for Export";
                    return View();
                }
            }
            int pageSize = new SettingService().ResultPageSize;
            var paymentsPendingForConfirmationOrFailed = new PaymentService()
                .GetPaymentsPendingForConfirmationOrFailed(out totalCount, nic: nic, passportNo: passport,
                   createdDateFrom: createdDateFrom, createdDateTo: createdDateTo, pageIndex: page, pageSize: pageSize
                   , showFailed: isShowFailedPayments);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(paymentsPendingForConfirmationOrFailed);
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
                        var isUpdated = new PaymentService()
                            .SetPaymentConfirmedOrFailed(Convert.ToInt32(key), failureReason);
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
                catch (UserException exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Failed: " + exp.Message;
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SetRePaymentReConfirmedOrFailed(string key, string failureReason)
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
                        var isUpdated = new PaymentService()
                            .SetPaymentConfirmedOrFailed(Convert.ToInt32(key), failureReason);
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
                catch (UserException exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Failed: " + exp.Message;
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
                catch (Exception exp)
                {
                    statusArray[0] = "101";
                    statusArray[1] = "Sorry, we are unable to process your request. Please try again later.";
                    return Json(statusArray, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(statusArray, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Payment Search   

        public ActionResult PaymentsSearch(int page = 0)
        {
            int totalCount = 0;
            int pageSize = new SettingService().ResultPageSize;
            var payments = new PaymentService().GetPayments(out totalCount, pageIndex: page, pageSize: pageSize);
            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(payments);
        }

        [HttpPost]
        public ActionResult PaymentsSearch(FormCollection formCollection)
        {
            if (!string.IsNullOrEmpty(formCollection["btnClearSearch"]))
                return Redirect(Request.UrlReferrer.ToString());

            string nic = null;
            string requestFormType = null;
            DateTime? createdDateFrom = null;
            DateTime? createdDateTo = null;
            int totalCount = 0;
            int page = 0;
            if (!string.IsNullOrEmpty(formCollection["btnPagination"]))
            {
                page = Convert.ToInt32(Request.Form["btnPagination"]);
            }
            if (!string.IsNullOrEmpty(Request.Form["txtNICNo"]))
                nic = Request.Form["txtNICNo"];

            if (!string.IsNullOrEmpty(Request.Form["ddlCustomerStatus"]))
                requestFormType = Request.Form["ddlCustomerStatus"];

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

            int pageSize = new SettingService().ResultPageSize;
            var payments = new PaymentService().GetPayments(out totalCount, nic: nic
                , requestFormTypeId: Convert.ToInt32(requestFormType), createdDateFrom: createdDateFrom
                , createdDateTo: createdDateTo, pageIndex: page, pageSize: pageSize);

            this.ViewBag.MaxPage = (totalCount / pageSize) - (totalCount % pageSize == 0 ? 1 : 0);
            this.ViewBag.Page = page;
            return View(payments);
        }

        public ActionResult PendingPayments()
        {
            int totalCount = 0;
            return View(new PaymentService().GetPaymentsPendingForConfirmationOrFailed(out totalCount));
        }

        [HttpPost]
        public ActionResult PendingPayments(FormCollection formCollection)
        {
            return View();
        }

        #endregion
    }
}