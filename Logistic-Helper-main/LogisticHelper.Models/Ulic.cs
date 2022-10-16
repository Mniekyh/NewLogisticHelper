﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;


namespace LogisticHelper.Models
{
    //[Table("Terc", Schema = "Terc")]

    [Keyless]
    public class Ulic
    {

       
        public string WOJ { get; set; }
        public string? POW { get; set; }
        public string? GMI { get; set; }
        public string? RODZ_GMI { get; set; } 
        public string SYM { get; set; }
        public string SYM_Ul { get; set; }
        public string? CECHA { get; set; }
        public string? NAZWA1 { get; set; }
        public string NAZWA2{ get; set; }
        public string STAN_NA { get; set; }

    }
}
