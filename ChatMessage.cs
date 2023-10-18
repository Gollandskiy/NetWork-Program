using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Занятие_в_аудитории_1_05._10._2023__Сетевое_программирование_
{
    public class ChatMessage
    {
        public string Login { get; set; }
        public string Text { get; set; }
        public DateTime Moment { get; set; }

        public override string ToString()
        {
            return $"{Login}: {Text}";
        }
    }
}
