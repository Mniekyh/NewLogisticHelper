using System;
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


    public class GoogleMap
    {
        //to jest zle!!!!!!!
        
        public string WOJ { get; set; }
        public string? POW { get; set; }
        public string? GMI { get; set; }
        public string? RODZ_GMI { get; set; }
        public string? RM { get; set; }
        public string? MZ { get; set; }
        public string NAZWA { get; set; }
        [Key]
        public string SYM { get; set; }
        public string SYMPOD { get; set; }
        public string STAN_NA { get; set; }



    }
}
