using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCSugar.Samples.Models
{
    public enum Heroes
    { 
        Batman,
        Superman,
        [Display(Name="Wonder Woman")]
        WonderWoman
    }
}