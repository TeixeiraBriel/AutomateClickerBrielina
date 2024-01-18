using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateClickerBrielina.Entidades
{
    public class Acao : Comandos
    {
        public Action action { get; set; }
        public Acao()
        {
            this.Tipo = 0;
        }

        public void executar()
        {
            action.Invoke();
        }
    }
}
