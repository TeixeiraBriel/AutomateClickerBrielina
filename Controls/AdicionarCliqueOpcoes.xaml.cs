using AutomateClickerBrielina.Entidades;
using AutomateClickerBrielina.Enums;
using System;
using System.Collections.Generic;
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
    /// Interação lógica para AdicionarCliqueOpcoes.xam
    /// </summary>
    public partial class AdicionarCliqueOpcoes : Page
    {
        CliquesAdionador _janelaPai;
        private FuncaoCrudCliqueEnum _funcaoCrudCliqueEnum;
        private Clique _refClique;

        public AdicionarCliqueOpcoes(FuncaoCrudCliqueEnum funcaoCrudCliqueEnum, Clique refClique = null)
        {
            InitializeComponent();
            _funcaoCrudCliqueEnum = funcaoCrudCliqueEnum;
            _refClique = refClique == null ? new Clique() : refClique;
        }

        private void PosicionalClick(object sender, RoutedEventArgs e)
        {
            _janelaPai = Window.GetWindow(this) as CliquesAdionador;
            _janelaPai.JanelaCliquesAdionador.Navigate(new AdicionarCliquePosicional(_funcaoCrudCliqueEnum, _refClique));
        }

        private void PorImagemClick(object sender, RoutedEventArgs e)
        {
            (Window.GetWindow(this) as CliquesAdionador).Fechar(false);  
            new Transparente("Print").Show();
        }
    }
}
