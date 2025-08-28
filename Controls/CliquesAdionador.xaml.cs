using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutomateClickerBrielina.Controls
{
    /// <summary>
    /// Interação lógica para CliquesAdionador.xam
    /// </summary>
    public partial class CliquesAdionador : Window
    {
        public CliquesAdionador()
        {
            InitializeComponent();
        }

        public void Fechar(bool AbrirGerenciaFluxo = true)
        {
            this.Close();
            if (AbrirGerenciaFluxo)
                new GerenciaFluxo().Show();
        }
    }
}
