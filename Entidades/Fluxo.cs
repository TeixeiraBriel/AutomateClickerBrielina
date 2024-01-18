using AutoIt;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace AutomateClickerBrielina.Entidades
{
    public class Fluxo
    {
        public List<Comandos> comandos { get; set; } = new List<Comandos>();

        public void AdicionaAcao(MainWindow Mw, string Descricao, int PosX, int PosY, bool Move = true, bool Click = true)
        {
            comandos.Add(Acao.CriarAcao(Mw,Descricao,PosX,PosY,Move,Click));
        }

        public void AdicionaLaco(Laco laco)
        {
            comandos.Add(laco);
        }

        public void Run()
        {
            foreach (var cmd in comandos)
            {
                int repeticoes = cmd.Tipo == 0 ? 1 : (cmd as Laco).Repeticoes;
                for (int i = 0; i < repeticoes; i++)
                {
                    if (repeticoes > 1)
                    {
                        foreach (Acao acao in (cmd as Laco).Acoes)
                        {
                            acao.executar();
                        }
                    }
                    else
                    {
                        (cmd as Acao).executar();
                    }
                }
            }
        }
    }
}
