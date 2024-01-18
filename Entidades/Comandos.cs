using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomateClickerBrielina.Entidades
{
    public class Comandos
    {
        public int Id { get; set; }
        public int Ordem { get; set; }
        public int Tipo { get; set;} //0 - Acao 1 - Laço
        public string Nome { get; set; }
    }
}
