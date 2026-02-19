using MassTransit;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = NewId.Next().ToGuid();
    }
}
