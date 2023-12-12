namespace CnC.Core
{
    public enum TicketSeverity
    {
        Low = 10,
        Normal = 20,
        Critical = 30
    }
    public enum TicketStatus
    {
        Open = 10,
        // Resolved = 20,
        Closed = 30//,
        //Reopened = 40
    }
    public enum RequestFormType
    {
        Card = 10,
        TopUp = 20
    }

    public enum UserStatus
    {
        Active = 1,
        Inactive = 2,
        Blocked = 3,
        Deleted = 4
    }

    /// <summary>
    /// Customer (KYC Form) Status For Staff
    /// </summary>
    public enum CustomerStatus
    {
        Pending = 1,
        Validated = 2,
        ValidationFailed = 3,
        //WaitingToSendToCardIssuer = 4,
        //SentToCardIssuer = 5,
        Approved = 6,
        Failed = 7
    }

    /// <summary>
    /// Customer (KYC Form) Status For Customer
    /// </summary>
    public enum CustomerStatusForCustomer
    {
        Pending = 1,
        Validated = 2,
        ValidationFailed = 3,
        Processing = 4,
        Approved = 5,
        Failed = 6
    }

    /// <summary>
    /// For Staff
    /// </summary>
    public enum RequestFormStatus
    {
        Pending = 1,
        PaymentConfirmed = 2,
        PaymentConfirmationFailed = 3,
        SentToCardIssuer = 4
    }

    /// <summary>
    /// For Customer
    /// </summary>
    public enum RequestFormStatusForCustomer
    {
        WaitingForPaymentConfirmation = 1,
        Processing = 2,
        PaymentConfirmationFailed = 3
    }

    /// <summary>
    /// For Staff
    /// </summary>
    public enum CardRequestStatus
    {
        Pending = 1,
        ApprovedByCardIssuer = 2,
        RejectedByCardIssuer = 3,
        DispatchPackageToTBO = 4,
        WaitingToDeliverPackageToCustomer = 5,
        PackageDeliveredToCustomer = 6
    }

    /// <summary>
    /// For Customer
    /// </summary>
    public enum CardRequestStatusForCustomer
    {
        Processing = 1,
        Processed = 2,
        Failed = 3,
        Delivered = 4
    }

    public enum CardStatus
    {
        Active = 1,
        Inactive = 2,
        Blocked = 3,
        Closed = 4
    }

    public enum TopUpRequestStatus
    {
        Pending = 1,
        Approved = 2,
        Uploaded = 3,
        Failed = 4,
        Cancelled = 5,
        History = 6
    }
    public enum PaymentMethod
    {
        Bank = 1,
        RAHYAB = 2,
        Admin = 3,
        Online = 4,
        TBOSuperAgent = 5
    }
    public enum RegistrationMode
    {
        RAHYAB = 1,
        Online = 2,
        TBOAgent = 3,
        Admin = 4,
        TBOSuperAgent = 5
    }
    public enum PaymentStatus
    {
        Pending = 1,
        Confirmed = 2,
        ConfirmationFailed = 3
    }
    public enum MaritalStatus
    {
        Single = 10,
        Married = 20
    }

    public enum Gender
    {
        Male = 10,
        Female = 20
    }
    public enum OrderBy
    {
        Asc = 1,
        Desc = 2
    }
    
}
