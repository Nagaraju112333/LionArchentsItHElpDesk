//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ArchentsIT.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class RaiseTicket
    {
        public int Sno { get; set; }
        public string TicketNo { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Status { get; set; }
        public string Assigned_Person { get; set; }
        public string Query { get; set; }
        public Nullable<System.Guid> EmployeeID { get; set; }
    
        public virtual UserRegister UserRegister { get; set; }
    }
}