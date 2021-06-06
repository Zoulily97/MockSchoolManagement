using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MockSchoolManagement.ViewModels
{
    public class EditUserViewModel
    {
        public IList<string> Claims { get; set; }
        public IList<string> Roles { get; set; }
        public string Id { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Email { get; set; }
        
        public string City { get; set; }


        public EditUserViewModel()
        {
            Claims = new List<string>();
            Roles = new List<string>();
        }


    }
}
