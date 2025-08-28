using AutomateClickerBrielina.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AutomateClickerBrielina.Entidades
{
    public class Clique
    {
        public int Id { get; set; }
        public TipoCliqueEnum Tipo { get; set; }
        public int posX { get; set; }
        public int posY { get; set; }
        public int PreSleep { get; set; }
        public int PosSleep { get; set; }
        public int qtdCliques { get; set; }
        public int TempoIntervalo { get; set; }
        public string FileName { get; set; }
        public bool Imagem { get; set; }

        public TipoCliqueEnum validaTipo()
        {
            TipoCliqueEnum tipo;

            if (Imagem)
            {
                tipo = TipoCliqueEnum.Imagem;
            }
            else
            {
                tipo = TipoCliqueEnum.Posicional;
            }

            return tipo;
        }
    }
}
