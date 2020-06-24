using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class Person
    {
        public Title Title { get; set; }

        /// <summary>
        /// The person's Age, in years
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// The first name of the person
        /// </summary>
        public string FirstName { get; set; }

        public decimal? Income { get; set; }
    }
}
