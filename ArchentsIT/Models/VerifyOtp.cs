using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace ArchentsIT.Models
{
    public class VerifyOtp
    {
        [Required]
        public int Otp { get; set; }    
    }
}