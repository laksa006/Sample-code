using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailTrigger
{
    public class ContactGroupInfo
    {
   
        public string login_id { get; set; }

        public string server_name { get; set; }

        public string database_name { get; set; }

        public string login_name { get; set; }

        public string password { get; set; }

        public string created_by { get; set; }

        public string created_datetime { get; set; }

        public string updated_by { get; set; }

        public string updated_datetime { get; set; }

        public string password_expiry_date { get; set; }

        public string service_owner_email { get; set; }

        public string distribution_list_email { get; set; }

        public string last_password_updated_date { get; set; }

    }
}
