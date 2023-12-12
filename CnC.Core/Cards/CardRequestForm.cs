using CnC.Core.Audit;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnC.Core.Cards
{
    [NotMapped]
    public class CardRequestForm : RequestForm
    {
        public List<CardRequest> CardRequests { get; set; }

        public CardRequestForm()
        {
            this.TypeId = (int)RequestFormType.Card;
            this.Type = RequestFormType.Card;
        }

        public CardRequestForm(RequestForm requestForm) : this()
        {
            this.CreatedOn = requestForm.CreatedOn;
            this.Customer = requestForm.Customer;
            this.CustomerId = requestForm.CustomerId;
            this.Description = requestForm.Description;
            this.ExchangeRate = requestForm.ExchangeRate;
            this.Id = requestForm.Id;
            this.IsCancelled = requestForm.IsCancelled;
            this.Payments = requestForm.Payments;
            this.SentToCardIssuerOn = requestForm.SentToCardIssuerOn;
            this.Status = requestForm.Status;
            this.Type = requestForm.Type;
            this.TypeId = requestForm.TypeId;
        }
        public UserActivity UserActivity { get; set; }

        [NotMapped]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedDateFrom { get; set; }
        [NotMapped]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? CreatedDateTo { get; set; }
    }
}
