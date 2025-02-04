﻿using MediatR;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Request.API.Applications.Commands
{
    [DataContract]
    public class CreateRequestCommand : IRequest<bool>
    {
        [DataMember]
        public Guid ApproverId { get; set; }

        [DataMember]
        public DateTime DayOffStart { get; set; }

        [DataMember]
        public DateTime DayOffEnd { get; set; }

        [DataMember]
        public DateTime? CompensationDayStart { get; set; }

        [DataMember]
        public DateTime? CompensationDayEnd { get; set; }

        [DataMember]
        public string? Message { get; set; }

        public CreateRequestCommand()
        {
        }

        public CreateRequestCommand(
            Guid approverId,
            DateTime dayOffStart,
            DateTime dayOffEnd,
            DateTime compensationDayStart,
            DateTime compensationDayEnd,
            string message)
        {
            ApproverId = approverId;
            DayOffStart = dayOffStart;
            DayOffEnd = dayOffEnd;
            CompensationDayStart = compensationDayStart;
            CompensationDayEnd = compensationDayEnd;
            Message = message;
        }
    }
}