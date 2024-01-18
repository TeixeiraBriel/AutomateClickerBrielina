using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateClickerBrielina.Entidades
{
    public class Laco : Comandos
    {
        public int Repeticoes { get; set; }
        public List<Comandos> Acoes { get; set; }

        public Laco()
        {
            this.Tipo = 1;
            Acoes = new List<Comandos>();
        }
    }
}
