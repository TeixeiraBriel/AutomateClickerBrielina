using AutoIt;
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
        public void AdicionaAcao(MainWindow Mw, string Descricao, int PosX, int PosY, bool Move = true, bool Click = true)
        {
            Acoes.Add(Acao.CriarAcao(Mw,Descricao,PosX,PosY,Move,Click));
        }
    }
}
