namespace Gerenciador_de_NF_e
{
    public partial class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            InitializeComponent();

            // Estes dois botões não têm clique ligado no designer
            //btnConsultar.Click += btnPainel_Click;    // "Consultar NF-e"
            //btnEditar.Click += btnEditar_Click;    // "Editar dados"
            Load += (_, _) =>
            {
                var auto = SolicitacaoAutomatica.Instancia;
                auto.StatusAlterado += (_, _) => Text = "Gerenciador de NF-e  |  " + auto.Status;
                auto.Iniciar();
            };

            FormClosing += (_, _) => SolicitacaoAutomatica.Instancia.Parar();
        }

        
        private void btnSolicitacao_Click(object sender, EventArgs e) => 
            AbrirUc(new UcSolicitação());

        private void btnCadastrar_Click(object sender, EventArgs e) =>
            AbrirUc(new UcCadastro());            // "Cadastrar Cliente"

        //private void btnSolicitar_Click(object? sender, EventArgs e) =>
            //AbrirUc(new UcSolicitação());         // "Fazer solicitação"

        private void btnPainel_Click(object? sender, EventArgs e) =>
            AbrirUc(new UcPainel());              // "Consultar NF-e"

        //Funções do painel



        private void panelConteudo_Paint(object? sender, PaintEventArgs e)
        {
        }

        private void btnEditar_Click(object sender, EventArgs e) => AbrirUc(new UcEdicao());


        private void AbrirUc(UserControl uc)
        {
            foreach (Control c in panelConteudo.Controls.Cast<Control>().ToList())
            {
                panelConteudo.Controls.Remove(c);
                if (c is UserControl) c.Dispose();   // para o timer do painel
            }

            panelConteudo.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            panelConteudo.Controls.Add(uc);
            uc.BringToFront();

            
        }


    }
}