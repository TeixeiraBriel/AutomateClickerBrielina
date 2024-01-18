using AutoIt;
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

        public static Acao CriarAcao(MainWindow Mw, string Descricao, int PosX, int PosY, bool Move = true, bool Click = true)
        {
            Acao acao = new Acao()
            {
                action = new Action(() =>
                {
                    Mw.imprimeConsole(Descricao);

                    if (Move)
                        AutoItX.MouseMove(PosX, PosY);
                    if (Click)
                        AutoItX.MouseClick("left");
                })
            };

            return acao;
        }
    }
}
