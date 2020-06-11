using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Office.Interop.Excel;

namespace FormatSchedule
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public double LeagueAthleticsID { get; set; }
        public string SportsEngineID { get; set; }


    }
}
