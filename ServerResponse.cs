using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Занятие_в_аудитории_1_05._10._2023__Сетевое_программирование_
{
    internal class ServerResponse
    {
        public String Status { get; set; }
        public IEnumerable<ChatMessage> Messages { get; set; }
    }
}
