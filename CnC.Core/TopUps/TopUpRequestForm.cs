using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CnC.Core.TopUps
{
    [NotMapped]
    public class TopUpRequestForm : RequestForm
    {   
        public List<TopUpRequest> TopUpRequests{ get; set; }

        public TopUpRequestForm()
        {
            this.TypeId = (int)RequestFormType.TopUp;
            this.Type = RequestFormType.TopUp;
        }

        public TopUpRequestForm(RequestForm requestForm) : this()
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
    }
}
