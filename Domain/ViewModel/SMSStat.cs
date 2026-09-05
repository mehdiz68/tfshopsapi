using Domain;
using Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
namespace Domain.ViewModels
{
    public class SMSStat
    {
        public SMSStat()
        {

        }
        public int AllInQueue { get; set; }
        public int SentInQueue { get; set; }
        public int UnsenInQueue { get; set; }
        public int ProblemInQueue { get; set; }
        public double AllPercent { get; set; }

    }

}
