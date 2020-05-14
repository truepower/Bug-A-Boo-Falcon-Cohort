﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bugaboo.Models
{
    public class TicketNotification
    {
        public int Id { get; set; }

        public int TicketId { get; set; }

        public string SubmitterId { get; set; }

        public string RecipientId { get; set; }

        public bool IsRead { get; set; }

        public string NotificationBody { get; set; }

        public virtual Ticket Ticket { get; set; }

        public virtual ApplicationUser Submitter { get; set; }

        public virtual ApplicationUser Recipient { get; set; }
    }
}