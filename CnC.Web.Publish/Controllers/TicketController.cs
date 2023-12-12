
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using CnC.Core;
using CnC.Service;
using CnC.Core.Accounts;

namespace CnC.Web.Controllers
{
    public class TicketController : Controller
    {
        int SessionUserId = 0;
        int adminRollId = 11;

        string textString;
        public ActionResult AddTicket()
        {
            return View();
        }

        //public TicketController()
        //{
        //    //Initialize(Session["UserId"] as RequestContext);
        //    SessionUserId = 6;// Convert.ToInt32(Session["UserId"]);
        //    adminRollId = 11;
        //}

        public ActionResult _ShowMessages()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddTicket(FormCollection formCollection, HttpPostedFileBase attachment)
        {
            var userSession = Session["CurrentSession"] as User;
            int priority = 0;
            var ticket = new Ticket();
            var ticketProcess = new TicketProcess();
            var helperService = new HelpDeskService();
            var users = new UserService();
            string fileUploadPath = ConfigurationManager.AppSettings["Upload"];
            ticket.IsResolved = false;
            int quickTicketId = 0;
          
            ticket.QuickTicket = new QuickTicket();
            
            string strQuickTicketId = formCollection["cmbQuickTicket"].ToString();
            if (strQuickTicketId != null && strQuickTicketId != "")
            {
                quickTicketId = Convert.ToInt32(strQuickTicketId);
            }
            else
            {
                quickTicketId = 0;
            }

            ticket.TicketProcess = new List<TicketProcess>();
            if (quickTicketId == 0)
            {

                //, Severity = priority
                //admin Roll Id   =2

                // TicketSeverity priority;
                //Enum.TryParse(formCollection["cmbQuickTicket"], out priority);
                //ticket.QuickTicket.Id = null;
                priority = Convert.ToInt32(formCollection["cmbPriority"]);
                ticket.Subject = formCollection["txtSubject"].ToString();
                ticket.SeverityId = priority;


                #region Temp

                ticket.Messages = new List<TicketMessage>();
                // ticket.Message = new TicketMessage();
                ticket.Messages.Add(new TicketMessage()
                {
                    Message = formCollection["txtComments"].ToString() != null ? formCollection["txtComments"].ToString() : formCollection["txtSubject"].ToString(),
                    UserId = Convert.ToInt32(Session["UserId"])
                });
                ticket.TicketProcess = new List<TicketProcess>();
                ticket.TicketProcess.Add(new TicketProcess() { Comments = formCollection["txtComments"].ToString() != null ? formCollection["txtComments"].ToString() : formCollection["txtSubject"].ToString(), UserId = Convert.ToInt32(Session["UserId"]), TicketStatusId = (int)TicketStatus.Open });
                //  List<UserRole> uers = users.GetAllUsers(adminRollId);//.Where (x=>x.IsCustomer==false).ToList();
                // List<UserRole> uers = users.GetUsers();// (adminRollId);//.Where (x=>x.IsCustomer==false).ToList();
                var uers = users.GetUsers();
                ticket.TicketAssignedToStaff = new List<AssignTicket>();
                foreach (var user in uers)
                {


                    ticket.TicketAssignedToStaff.Add(new AssignTicket() { UserId = user.Id });
                }

                #endregion



            }
            else
            {

                ticket.QuickTicket.Id = quickTicketId;
                ticket.QuickTicket = helperService.GetQuickTicket(quickTicketId);
                //  TicketSeverity priority=10;
                //Enum.TryParse(ticket.QuickTicket.Priority.ToString(), out priority);
                ticket.QuickTicket.Id = quickTicketId;
                //ticket.Severity = priority;
                ticket.Subject = ticket.QuickTicket.Name;
                int rolId = ticket.QuickTicket.RoleId;
                ticket.Isassign = true;
                priority = ticket.QuickTicket.PriorityId;
                //, Severity = priority
                #region temp2
                ticket.SeverityId = priority;
                ticket.Messages = new List<TicketMessage>();
                // ticket.Message = new TicketMessage();
                // ticket.Message.Message = ticket.QuickTicket.Name;
                //  ticket.Message.UserId = SessionUserId;


                ticket.Messages.Add(new TicketMessage()
                {
                    Message = ticket.QuickTicket.Name,
                    UserId = Convert.ToInt32(Session["UserId"])
                });
                ticket.TicketProcess.Add(new TicketProcess() { Comments = formCollection["txtComments"].ToString() != null ? formCollection["txtComments"].ToString() : ticket.QuickTicket.Name, UserId = Convert.ToInt32(Session["UserId"]), TicketStatusId = (int)TicketStatus.Open });
                //    List<UserRole> uers = users.GetAllUsers(rolId);

                var uers = users.GetUsers(roleId: rolId);
                ticket.TicketAssignedToStaff = new List<AssignTicket>();
                foreach (var user in uers)
                {


                    ticket.TicketAssignedToStaff.Add(new AssignTicket() { UserId = user.Id });
                }

                #endregion
            }
            Random rnd = new Random();
            ticket.TicketNo = userSession.Id + DateTime.Now.ToString("yymmddss") + rnd.Next();
            ticket.LastSeenByUserDate = DateTime.Now;

            ticket.LastSeenByUserId = Convert.ToInt32(userSession.Id);


            ticket.CustomerId = Convert.ToInt32(userSession.Id);

            //  ticket.Severity = new HelpDeskService().GetQuickTicket(ticket.QuickTicket.Id).Priority;


            // NIC Doc upload
            if (attachment != null && attachment.ContentLength > 0)
            {
                var fileName = Path.GetFileName(attachment.FileName);
                string extension = Path.GetExtension(attachment.FileName);
                string newFileName = DateTime.Now.Ticks + extension;
                ticket.FilePath = Path.Combine(Server.MapPath(fileUploadPath), newFileName);
                attachment.SaveAs(ticket.FilePath);

                // customer.NICDoc = newFileName;
            }

            helperService.AddTicket(ticket);
            return RedirectToAction("GetTickets", "ticket");
            //return View();
        }

        [HttpPost]
        public ActionResult Multiple(IEnumerable<HttpPostedFileBase> files)
        {
            foreach (var file in files)
            {
                if (file != null && file.ContentLength > 0)
                {
                    file.SaveAs(Path.Combine(Server.MapPath("/upload"), Guid.NewGuid() + Path.GetExtension(file.FileName)));
                }
            }
            return View();
        }
        public List<QuickTicket> GetQuickTickets()
        {
            HelpDeskService helpService = new HelpDeskService();
            var quickTickets = helpService.GetQuickTickets();
            return quickTickets;
        }

        public ActionResult GetTickets()
        {
            // SessionUserId = Convert.ToInt32(Session["UserId"]);
           
            if (Session["CurrentSession"] == null)
            {
                RedirectToAction("Login", "Account");               
            }
            var userSession = Session["CurrentSession"] as User;
            ViewBag.Id = userSession.Id;
            //Session["abc"] = Convert.ToInt32(SessionUserId);


            //var getTicketsData = getTickets.GetAllTickets();
            //List<QuickTicket> quickTickets = helpService.GetQuickTickets();


            //TempTicketModel model = new TempTicketModel();
            //quicketTicketClollectyion= helpService.GetQuickTickets();

            //model.

            return View();
        }

        public ActionResult DatatableView()
        {
            return View();
        }

        public List<Ticket> GetAllTickets(int session)
        {
            //int a = 6;// Convert.ToInt32(Session["abc"]);
            UserService userService = new UserService();
            HelpDeskService helpService = new HelpDeskService();
            var user = userService.GetUser(session);

            //if (user.IsCustomer == true)
            //{
            var getTickets = helpService.GetTickets().Where(x => x.CustomerId == session).ToList();
            return getTickets;
            //}
            //else
            //{
            //    var getTickets = helpService.GetAssignedTickets(session).ToList();
            //    return getTickets;
            //}

        }

        //public List<Ticket> GetAllTickets(int priority)
        //{
        //    // var a = Request["id"].ToString();
        //    HelpDeskService helpService = new HelpDeskService();
        //    var getTickets = helpService.GetTickets();
        //    return getTickets;
        //}

        [HttpPost]
        public List<Ticket> GetAllTickets(FormCollection formCollection)
        {
            // var a = Request["id"].ToString();
            string priority = Request["cmbPriority"].ToString();
            HelpDeskService helpService = new HelpDeskService();
            var getTickets = helpService.GetTickets().Where(x => x.SeverityId.ToString() == priority).ToList();
            return getTickets;
        }
        public List<TicketMessage> GetMessages(int ticketId)
        {
            HelpDeskService helpService = new HelpDeskService();
            var ticket = helpService.GetMessages(ticketId).OrderBy(i => i.CreatedOn).Where(t => t.TicketId == ticketId);
            // var test = ticket.Where(t => t.TicketId == ticketId);
            return ticket.ToList();

        }
        [HttpPost]
        public ActionResult AddMessage(FormCollection formCollection)
        {
            try
            {
                var userSession = Session["CurrentSession"] as User;
                string message = formCollection["txtPost"].ToString();
                var helperService = new HelpDeskService();
                var ticketMessage = new TicketMessage();
                int ticketId = Convert.ToInt32(formCollection["ticketId"]);
                ticketMessage.CreatedOn = DateTime.UtcNow;
                ticketMessage.UserId = Convert.ToInt32(userSession.Id);
                ticketMessage.Message = message;
                ticketMessage.TicketId = ticketId;
                helperService.AddMessage(ticketMessage);

                return RedirectToAction("GetMessagesByTicket", "ticket", new { ticketId = ticketId, SessionUserId = ticketMessage.UserId });
            }
            catch (Exception exception)
            {
                return View();
            }
        }

        public ActionResult GetMessagesByTicket(int ticketId, int SessionUserId)
        {
            return View();
        }

        public ActionResult Manage()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddTicketProcess(FormCollection formCollection)
        {
            var userSession = Session["CurrentSession"] as User;
            TicketStatus status;
            Enum.TryParse(formCollection["cmbManageStatus"], out status);
            var helperService = new HelpDeskService();
            var ticketProcess = new TicketProcess();
            // string strTicket = (formCollection["cmbStatus"].ToString());
            string strMessage = (formCollection["txtComments"].ToString());
            //int ticketId     = Convert.ToInt32(formCollection["ticketId"]);
            int ticketId = Convert.ToInt32(formCollection["hdTicketId"]);

            //  ticketProcess.TicketId = ticketId;
            ticketProcess.UserId = Convert.ToInt32(userSession.Id);
            ticketProcess.Comments = strMessage;
            ticketProcess.TicketStatusId = Convert.ToInt32(formCollection["cmbManageStatus"]);
            ticketProcess.TicketId = ticketId;
            helperService.AddTicketProcess(ticketProcess);
            return RedirectToAction("GetTickets", "Ticket");
        }

        //public ActionResult GetTicketsByPriority (int Priority)
        //{
        //    List<Ticket> ticketByPriority = GetAllTickets(Priority);

        //    return Json(ticketByPriority, JsonRequestBehavior.AllowGet);
        //}

        public ActionResult GetTicketBySubject(int ticketId)
        {
            List<Ticket> ticketBySubject = new List<Ticket>();
            HelpDeskService helpService = new HelpDeskService();
            ticketBySubject = helpService.GetTickets().Where(x => x.Id == ticketId).ToList();
            return Json(ticketBySubject, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AdminHelpDesk()
        {
            SessionUserId = Convert.ToInt32(Session["UserId"]);
            ViewBag.UserId = SessionUserId;
            return View();
        }

        public List<User> GetAllAssignedUsers()
        {


            UserService userService = new UserService();
            //   var users = userService.GetAllUsers().Where(x => x.IsCustomer == false && x.IsAdmin == false).ToList();
            var users = userService.GetUsers();//.Where(x => x.IsCustomer == false && x.IsAdmin == false).ToList();
            return users;
        }

        public List<Ticket> GetAllOpenTicket()
        {
            HelpDeskService helpService = new HelpDeskService();
            var getTickets = helpService.GetTickets()
               //    .Where(x => x.CustomerId == 1
               //&& x.Isassign==false
               //   && x.TicketProcess.OrderBy(tp => tp.CreatedOn)
               //   .Last().TicketStatus != TicketStatus.Closed &&
               //   x.TicketProcess.OrderBy(tp => tp.CreatedOn)
               //   .Last().TicketStatus != TicketStatus.Resolved).ToList()

               ;
            return getTickets;
        }

        public ActionResult AddAssignTicket(int[] chkTicketId, string[] cmbAssigned)
        {
            var helperService = new HelpDeskService();
            var assignTicket = new AssignTicket();

            for (int i = 0; i <= chkTicketId.Length - 1; i++)
            {
                // int tid = Convert.ToInt32(chkTicketId[i]);
                string assign = cmbAssigned[i].ToString();
                if (assign != "")
                {
                    assignTicket.UserId = Convert.ToInt32(assign);
                }

                assignTicket.TicketID = Convert.ToInt32(chkTicketId[i]);
                assignTicket.CreatedOn = DateTime.UtcNow;
                assignTicket.IsAssign = true;
                if (helperService.AddTicketAssigned(assignTicket) > 0)
                    helperService.UpdateTicket(true, assignTicket.TicketID);
            }
            return RedirectToAction("AdminHelpDesk", "Ticket");
        }

        public ActionResult GetQuickDataById(int quickTicketId)
        {
            QuickTicket quickTicket = new QuickTicket();
            HelpDeskService helpService = new HelpDeskService();

            quickTicket = helpService.GetQuickTicket(quickTicketId);
            return Json(quickTicket, JsonRequestBehavior.AllowGet);
        }

        public Ticket GetTicketById(int ticketId)
        {
            Ticket ticket = new Ticket();
            HelpDeskService helpService = new HelpDeskService();
            ticket = helpService.GetTicket(ticketId);
            return ticket;

        }
        public List<Ticket> GetAssignedTicket(int userId)
        {

            HelpDeskService helpService = new HelpDeskService();
            var getAssignedTicket = helpService.GetAssignedTickets(Convert.ToInt32(Session["UserId"]));
            return getAssignedTicket;
        }

        public ActionResult Search()
        {
            return View();
        }
    }
}