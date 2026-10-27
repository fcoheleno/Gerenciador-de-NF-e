namespace Gerenciador_de_NF_e
{
    public partial class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            InitializeComponent();

            // Estes dois botões não têm clique ligado no designer
            guna2Button1.Click += btnPainel_Click;    // "Consultar NF-e"
            guna2Button2.Click += btnEditar_Click;    // "Editar dados"
            Load += (_, _) =>
            {
                var auto = SolicitacaoAutomatica.Instancia;
                auto.StatusAlterado += (_, _) => Text = "Gerenciador de NF-e  |  " + auto.Status;
                auto.Iniciar();
            };

            FormClosing += (_, _) => SolicitacaoAutomatica.Instancia.Parar();
        }

        private void MostrarTela(UserControl uc)
        {
            foreach (Control c in panelConteudo.Controls.Cast<Control>().ToList())
            {
                panelConteudo.Controls.Remove(c);
                if (c is UserControl) c.Dispose();   // para o timer do painel
            }

            uc.Dock = DockStyle.Fill;
            panelConteudo.Controls.Add(uc);
        }
        private void btnSolicitacao_Click(object sender, EventArgs e) => AbrirUc(new UcSolicitação());

        private void btnConsultar_Click(object? sender, EventArgs e) =>
            MostrarTela(new UcCadastro());            // "Cadastrar Cliente"

        private void btncertificados_Click(object? sender, EventArgs e) =>
            MostrarTela(new UcSolicitação());         // "Fazer solicitação"

        private void btnPainel_Click(object? sender, EventArgs e) =>
            MostrarTela(new UcPainel());              // "Consultar NF-e"

        //Funções do painel

        private void btnEditar_Click(object? sender, EventArgs e) =>
            MessageBox.Show("A tela de edição não está disponível nesta versão do projeto.");

        private void panelConteudo_Paint(object? sender, PaintEventArgs e)
        {
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            panelConteudo.Controls.Clear();
            UcEdicao uc = new UcEdicao();
            uc.Dock = DockStyle.Fill;
            panelConteudo.Controls.Add(uc);
        }

        private void AbrirUc(UserControl uc)
        {
            panelConteudo.Controls.Clear();
            uc.Dock = DockStyle.Fill;
            panelConteudo.Controls.Add(uc);
            uc.BringToFront();
        }
    }
}