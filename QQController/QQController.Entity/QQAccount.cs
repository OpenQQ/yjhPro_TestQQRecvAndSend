using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QQController.Entity
{
    public class QQAccount
    {
        [Key]
        public int Id { get; set; }

        public bool IsD { get; set; } = false;
        public DateTime CreateTime { get; set; }
        public DateTime UpdateTime { get; set; }

        [MaxLength(20)]
        public string QQNum { set; get; }

        [MaxLength(100)]
        [Column("pwd")]
        public  string Password { set; get; }

        [MaxLength(100)]
        public string StatusDesc { set; get; }

        public bool IsLogin { set; get; }

        public int State { set; get; }

        public int Type { set; get; }
    }
}
