using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Gerenciador_de_NF_e
{
    public partial class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            InitializeComponent();
        }

        private void panelConteudo_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnConsultar_Click(object sender, EventArgs e)
        {
            AbrirConsulta();
        }

        private void btnSolicitacao_Click(object sender, EventArgs e)
        {
            AbrirSolicitacao();
        }



        //Funções do painel

        private void AbrirConsulta()
        {
            panelConteudo.Controls.Clear();
            UcCadastro uc = new UcCadastro();
            uc.Dock = DockStyle.Fill;
            panelConteudo.Controls.Add(uc);
        }

        private void AbrirSolicitacao()
        {
            panelConteudo.Controls.Clear();
            UcSolicitação uc = new UcSolicitação();
            uc.Dock = DockStyle.Fill;
            panelConteudo.Controls.Add(uc);
        }

        
    }
}
