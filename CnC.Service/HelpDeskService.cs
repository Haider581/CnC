
using System;
using System.Collections.Generic;
using System.Linq;
using CnC.Data;
using CnC.Core;
using log4net;

namespace CnC.Service
{

    public class HelpDeskService
    {
        private static ILog Log { get; set; }
        ILog log = LogManager.GetLogger(typeof(CardService));

        //private EntityContext context = new EntityContext();
        //private EntityContext context = new EntityContext();
        public int AddTicket(Ticket ticket)
        {
            try
            {
                // Ticket ticketInDto = new Ticket();
                using (var context = new EntityContext())
                {
                    //ticketInDto.Subject = ticket.Subject;
                    context.Tickets.Add(ticket);
                    if (context.SaveChanges() > 0)
                    {

                        // ticket.Message.TicketId = 9;
                        // context.TicketMessages.Add(ticket.Message);


                        foreach (var item in ticket.Messages)
                        {
                            item.TicketId = ticket.Id;
                            context.TicketMessages.Add(item);
                        }

                        foreach (var ticketProcess in ticket.TicketProcess)
                        {
                            ticketProcess.TicketId = ticket.Id;
                            context.TicketProcesses.Add(ticketProcess);
                        }
                        foreach (var assignTicket in ticket.TicketAssignedToStaff)
                        {
                            assignTicket.TicketID = ticket.Id;
                            context.AssignTickets.Add(assignTicket);
                        }
                        context.SaveChanges();
                    }
                    return ticket.Id;
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return -1;
            }

        }


        //public List<Ticket> GetTickets(int pageSize = 10, int pageIndex = 0)
        public List<Ticket> GetTickets()
        {
            // int skipRows = pageSize * pageIndex;

            try
            {
                using (var context = new EntityContext())

                {

                    var ticketProcesses = (from t in context.Tickets
                                           join tp in context.TicketProcesses
                                           .GroupBy(tp => tp.TicketId)
                                           .Select(tpg => tpg.OrderByDescending(tp => tp.Id).FirstOrDefault())
                                           //.Select(tp => tp.FirstOrDefault())
                                           on t.Id equals tp.TicketId
                                           select new { Ticket = t, TicketProcess = tp }).ToList();
                    //.Skip(skipRows).Take(pageSize).ToList();

                    foreach (var tp in ticketProcesses)
                        tp.Ticket.TicketProcess = new List<TicketProcess> { tp.TicketProcess };

                    return ticketProcesses.Select(tp => tp.Ticket).ToList();
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return null;
            }


            //  var tickets = new List<Ticket>();
            //  var ticket = new Ticket();
            //  ticket.Id = 100;
            //  ticket.TicketNo = 1;
            //  ticket.CustomerId = 1;
            //  ticket.Subject = "TopUp Card Problem";
            //  ticket.LastSeenByUserDate = DateTime.UtcNow;
            //  ticket.Messages = new List<TicketMessage>();
            //  var FirstMessage = new TicketMessage();
            //  FirstMessage.CreatedOn = DateTime.Now;
            //  FirstMessage.Message = "This is first Message";
            //  FirstMessage.User = new User();
            //  FirstMessage.User.UserName = "Client User";
            //  FirstMessage.TicketId = 100;
            //  ticket.Messages.Add(FirstMessage);

            //  var SecondMessage = new TicketMessage();
            //  SecondMessage.CreatedOn = DateTime.Now;
            //  SecondMessage.Message = "This is Second Message";
            //  SecondMessage.User = new User();
            //  SecondMessage.User.UserName = "TBO User";
            //  SecondMessage.TicketId = 100;
            //  ticket.Messages.Add(SecondMessage);

            //  ticket.TicketProcess = new List<TicketProcess>();
            //  var ticketProces = new TicketProcess();
            //  ticketProces.TicketId = 100;
            //  ticketProces.Id = 1;
            //  ticketProces.CreatedOn = DateTime.Parse("02-10-17");
            //  //ticketProces.TicketStatus = TicketStatus.Open;
            //  ticket.TicketProcess.Add(ticketProces);

            //  var SecondticketProces = new TicketProcess();
            //  SecondticketProces.TicketId = 100;
            //  SecondticketProces.Id = 2;
            //  SecondticketProces.CreatedOn = DateTime.UtcNow;
            // // SecondticketProces.TicketStatus = TicketStatus.Closed;
            //  ticket.TicketProcess.Add(SecondticketProces);

            // // ticket.Severity = TicketSeverity.Critical;
            //  tickets.Add(ticket);
            //  var ticket1 = new Ticket();
            //  ticket1.Id = 101;
            //  ticket1.TicketNo = 2;
            //  ticket1.CustomerId = 1;
            //  ticket1.Subject = "Website not Working";
            //  ticket1.LastSeenByUserDate = DateTime.UtcNow;
            //  ticket1.Messages = new List<TicketMessage>();
            //  var FirstMessage1 = new TicketMessage();
            //  FirstMessage1.CreatedOn = DateTime.Now;
            //  FirstMessage1.Message = "This is first Message";
            //  FirstMessage1.User = new User();
            //  FirstMessage1.User.UserName = "Client User";
            //  FirstMessage1.TicketId = 101;
            //  ticket.Messages.Add(FirstMessage1);

            //  var SecondMessage1 = new TicketMessage();
            //  SecondMessage1.CreatedOn = DateTime.Now;
            //  SecondMessage1.Message = "This is Second Message";
            //  SecondMessage1.User = new User();
            //  SecondMessage1.User.UserName = "TBO Admin";
            //  SecondMessage1.TicketId = 101;
            //  ticket.Messages.Add(SecondMessage1);

            //  ticket1.TicketProcess = new List<TicketProcess>();
            //  var ticketProces1 = new TicketProcess();
            //  ticketProces1.TicketId = 101;
            //  ticketProces1.CreatedOn = DateTime.UtcNow;
            // // ticketProces1.TicketStatus = TicketStatus.Open;
            //  ticket1.TicketProcess.Add(ticketProces1);

            // // ticket1.Severity = TicketSeverity.Low;
            //  tickets.Add(ticket1);

            //  var ticket2 = new Ticket();
            //  ticket2.Id = 102;
            //  ticket2.TicketNo = 3;
            //  ticket2.CustomerId = 1;
            //  ticket2.Subject = "Need to change Email Address";
            //  ticket2.LastSeenByUserDate = DateTime.UtcNow;
            //  ticket2.Messages = new List<TicketMessage>();
            //  var FirstMessage2 = new TicketMessage();
            //  FirstMessage2.CreatedOn = DateTime.Now;
            //  FirstMessage2.Message = "This is first Message";
            //  FirstMessage2.User = new User();
            //  FirstMessage2.User.UserName = "Client User";
            //  FirstMessage2.TicketId = 101;
            //  ticket.Messages.Add(FirstMessage2);

            //  var SecondMessage2 = new TicketMessage();
            //  SecondMessage2.CreatedOn = DateTime.Now;
            //  SecondMessage2.Message = "This is Second Message";
            //  SecondMessage2.User = new User();
            //  SecondMessage2.User.UserName = "TBO Admin";
            //  SecondMessage2.TicketId = 102;
            //  ticket.Messages.Add(SecondMessage2);

            //  ticket2.TicketProcess = new List<TicketProcess>();
            //  var ticketProces2 = new TicketProcess();
            //  ticketProces2.TicketId = 102;
            //  ticketProces2.CreatedOn = DateTime.Parse("02-11-2017");
            // // ticketProces2.TicketStatus = TicketStatus.Reopened;
            //  ticket2.TicketProcess.Add(ticketProces2);

            ////  ticket2.Severity = TicketSeverity.Low;


            //  tickets.Add(ticket2);
            //return tickets;


        }

        public Ticket GetTicket(int ticketId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    Ticket ticket = context.Tickets.SingleOrDefault(x => x.Id == ticketId);
                    return ticket;
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return null;
            }



            //    var tickets = new List<Ticket>();
            //var ticket = new Ticket();
            //ticket.Id = 100;
            //ticket.TicketNo = "1";
            //ticket.Subject = "TopUp Card Problem";
            //ticket.LastSeenByUserDate = DateTime.UtcNow;
            //ticket.Messages = new List<TicketMessage>();
            //var FirstMessage = new TicketMessage();
            //FirstMessage.CreatedOn = DateTime.Now;
            //FirstMessage.Message = "This is first Message";
            //FirstMessage.Id = 1;
            //FirstMessage.User = new User();
            //FirstMessage.User.UserName = "Client User";
            //FirstMessage.TicketId = 1;
            //ticket.Messages.Add(FirstMessage);

            //var SecondMessage = new TicketMessage();
            //SecondMessage.CreatedOn = DateTime.Now;
            //SecondMessage.Message = "This is Second Message";
            //SecondMessage.Id = 2;
            //SecondMessage.User = new User();
            //SecondMessage.User.UserName = "TBO User";
            //SecondMessage.TicketId = 1;
            //ticket.Messages.Add(SecondMessage);

            //ticket.TicketProcess = new List<TicketProcess>();
            //var ticketProces = new TicketProcess();
            //ticketProces.TicketId = 1;
            //ticketProces.CreatedOn = DateTime.UtcNow;
            //ticketProces.TicketStatusId = 1;
            //ticket.TicketProcess.Add(ticketProces);

            //ticket.SeverityId = 10;
            //return ticket;
        }



        public List<QuickTicket> GetQuickTickets()
        {
            try
            {
                using (var context = new EntityContext())
                {
                    // List<CardType> cardTypes = context.CardTypes.ToList();
                    //   return cardTypes;

                    List<QuickTicket> quickTickets = context.QuickTickets.ToList();

                    //var quickTickets = new List<QuickTicket>();
                    //quickTickets.Add(new QuickTicket() { Id = 1, Name = "My Card is blocked", Priority = TicketSeverity.Low, RoleId = 1 });
                    //quickTickets.Add(new QuickTicket() { Id = 2, Name = "My Card is taking too long time", Priority = TicketSeverity.Low, RoleId = 1 });
                    //quickTickets.Add(new QuickTicket() { Id = 3, Name = "How can i change my password", Priority = TicketSeverity.Low, RoleId = 1 });
                    return quickTickets;
                }
            }
            catch(Exception exception)
            {
                log.Error(exception);
                return null;
            }
        }

        public QuickTicket GetQuickTicket(int quickTicketId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    QuickTicket quickTicket = context.QuickTickets.Where(x => x.Id == quickTicketId).SingleOrDefault();


                    return quickTicket;
                }
            }
            catch(Exception exception)
            {
                log.Error(exception);
                return null;
            }

        }
        public int AddMessage(TicketMessage ticketMessage)
        {
            try
            {
                using (var db = new EntityContext())
                {
                    db.TicketMessages.Add(ticketMessage);

                    db.SaveChanges();

                    return 1;
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return -1;
            }
        }

        public int AddQuickTicket(QuickTicket quickTicket)
        {
            return 1;
        }
        //public List<TicketMessage> GetAllMessagesbyTicket(int ticketId)
        //{
        //    var mesaages = new List<TicketMessage>();
        //    var message = new TicketMessage();
        //    message.Message = "First Message";
        //    message.TicketId = ticketId;
        //    message.UserId = 1;
        //    message.CreatedOn = DateTime.UtcNow;
        //    mesaages.Add(message);

        //    var message1 = new TicketMessage();
        //    message1.Message = "Second Message";
        //    message1.TicketId = ticketId;
        //    message1.UserId = 1;
        //    message1.CreatedOn = DateTime.UtcNow;
        //    mesaages.Add(message1);
        //    return mesaages;
        //}

        public int AddTicketAssigned(AssignTicket assignTicket)
        {
            using (var context = new EntityContext())
            {
                context.AssignTickets.Add(assignTicket);
                if (context.SaveChanges() > 0)
                    return 1;
                else
                    return -1;

            }

        }
        public int AddTicketProcess(TicketProcess ticketProcess)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    context.TicketProcesses.Add(ticketProcess);
                    context.SaveChanges();
                    return ticketProcess.Id;
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return -1;
            }
        }

        public List<AssignTicket> GetAllTicketAssigned()
        {
            try
            {
                var ticketsAssigned = new List<AssignTicket>();
                var ticketAssigned = new AssignTicket();
                ticketAssigned.TicketID = 1;
                ticketAssigned.UserId = 1;
                ticketAssigned.CreatedOn = DateTime.UtcNow;

                ticketsAssigned.Add(ticketAssigned);
                return ticketsAssigned;
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return null;
            }
        }

        //public List<User> getAllUser()
        //{
        //    var users = new List<User>();
        //    var user = new User();
        //    user.Id = 1;
        //    user.UserName = "Admin";
        //    users.Add(user);

        //    var user1 = new User();
        //    user1.Id = 2;
        //    user1.UserName = "Agent";
        //    users.Add(user1);

        //    return users;


        //}

        //public User getUser(int userId)
        //{
        //    var user = new User();
        //    user.UserName = "ALi";
        //    user.IsCustomer = false;
        //    return user;
        //}

        public List<TicketMessage> GetMessages(int? ticketId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var messagesUsers = (from t in context.TicketMessages
                                         join u in context.Users on t.UserId equals u.Id
                                         select new { TicketMessage = t, User = u }).ToList();
                    foreach (var m in messagesUsers)
                    {
                        m.TicketMessage.User = m.User;
                    }
                    return messagesUsers.Select(x => x.TicketMessage).ToList();

                    //foreach (var tp in ticketProcesses)
                    //    tp.Ticket.TicketProcess = new List<TicketProcess> { tp.TicketProcess };

                    //return ticketProcesses.Select(tp => tp.Ticket).ToList();
                    //List < TicketMessage > ticketMessage = context.TicketMessages.ToList();
                    //return ticketMessage;
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return null;
            }
            //var messages = new List<TicketMessage>();
            //var message = new TicketMessage();
            //message.User = new User();
            //message.TicketId = 100;
            //message.Id = 1;
            //message.CreatedOn = DateTime.UtcNow;
            //message.Message = "Problem in Topup Card";
            //message.User.UserName = "Oun abbas";
            //messages.Add(message);

            //var secondMessage = new TicketMessage();
            //secondMessage.User = new User();
            //secondMessage.TicketId = 100;
            //secondMessage.Id = 2;
            //secondMessage.CreatedOn = DateTime.UtcNow;
            //secondMessage.Message = "We are working Please wait";
            //secondMessage.User.UserName = "TBO Agent User";
            //messages.Add(secondMessage);

            //var thirdMessage = new TicketMessage();
            //thirdMessage.User = new User();
            //thirdMessage.TicketId = 100;
            //thirdMessage.Id = 3;
            //thirdMessage.CreatedOn = DateTime.UtcNow;
            //thirdMessage.Message = "Still no response";
            //thirdMessage.User.UserName = "Oun abbas";
            //messages.Add(thirdMessage);

            //var fourMessage = new TicketMessage();
            //fourMessage.User = new User();
            //fourMessage.TicketId = 100;
            //fourMessage.Id = 4;
            //fourMessage.CreatedOn = DateTime.UtcNow;
            //fourMessage.Message = "Still no response";
            //fourMessage.User.UserName = "Oun abbas";
            //messages.Add(fourMessage);

            //var fifthMessage = new TicketMessage();
            //fifthMessage.User = new User();
            //fifthMessage.TicketId = 100;
            //fifthMessage.Id = 5;
            //fifthMessage.CreatedOn = DateTime.UtcNow;
            //fifthMessage.Message = "Still no response";
            //fifthMessage.User.UserName = "Oun abbas";
            //messages.Add(fifthMessage);

            //var sixthMessage = new TicketMessage();
            //sixthMessage.User = new User();
            //sixthMessage.TicketId = 100;
            //sixthMessage.Id = 6;
            //sixthMessage.CreatedOn = DateTime.UtcNow;
            //sixthMessage.Message = "Still no response";
            //sixthMessage.User.UserName = "Oun abbas";
            //messages.Add(sixthMessage);


            //return messages;
        }

        public int UpdateTicket(bool assignStatus, int ticketId)
        {
            try
            {
                using (var context = new EntityContext())
                {
                    var tickedata = context.Tickets.Where(x => x.Id == ticketId).SingleOrDefault();
                    tickedata.Isassign = assignStatus;
                    if (context.SaveChanges() > 0)
                    {
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }

                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return -1;
            }
        }

        public List<Ticket> GetAssignedTickets(int userId)
        {
            int ticketId = 0;
            int oldTicketId = 0;
            try
            {
                using (var context = new EntityContext())
                {
                    var result = (from t in context.Tickets
                                  join a in context.AssignTickets
                                  on t.Id equals a.TicketID
                                  join p in context.TicketProcesses
                                  on t.Id equals p.TicketId

                                  // where a.UserId == (userId.HasValue ? userId.Value : a.UserId)
                                  where a.UserId == userId
                                  //orderby p.CreatedOn descending
                                  group new { Ticket = t, AssignTicket = a, TicketProcess = p } by t.Id
                                          ).ToList();

                    if (result != null)
                    {
                        var tickets = new List<Ticket>();

                        foreach (var grp in result)
                        {
                            var firstRecord = grp.LastOrDefault();
                            var ticket = firstRecord.Ticket;
                            ticket.TicketProcess = new List<TicketProcess> { firstRecord.TicketProcess };
                            ticket.TicketAssignedToStaff = new List<AssignTicket>();

                            foreach (var assignTicket in grp.Select(g => g.AssignTicket).Distinct())
                            {
                                ticket.TicketAssignedToStaff.Add(assignTicket);
                            }

                            tickets.Add(ticket);

                            //ticketId = ticketData.Ticket.Id;
                            //  if (ticketId != oldTicketId)
                            // {
                            //grp.Ticket.TicketProcess = new List<TicketProcess> { grp.TicketProcess }.GroupBy(x => x.TicketId).LastOrDefault().ToList();              //.GroupBy(x => x.TicketId).Select(pg => pg.OrderByDescending(p => p.Id).FirstOrDefault()).ToList());//      ticketData.Ticket.TicketProcess.GroupBy(x => x.TicketId)

                            // }
                            //grp.Ticket.TicketAssignedToStaff = new List<AssignTicket> { grp.AssignTicket };
                            //tickets.Add(grp.Ticket);
                            // oldTicketId = ticketId;
                            // ticketData
                        }
                        return tickets;
                    }
                    return null;
                }
            }
            catch (Exception exception)
            {
                log.Error(exception);
                return null;
            }
        }
    }


}
