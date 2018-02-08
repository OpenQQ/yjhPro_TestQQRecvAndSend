using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQController.Entity
{
    public class Account
    {
        [Key]
        public int Id { get; set; }
        public bool IsD { get; set; } = false;
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        [MaxLength(100)]
        public string AccountName { set; get; }

        [MaxLength(50)]
        [Column("pwd")]
        public string Password { set; get; }

        public int Type { set; get; } = 0;
        
    }
}
