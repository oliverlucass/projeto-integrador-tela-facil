using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace TelaFacilLauncher
{
    public partial class ConfigForm : MaterialForm
    {

        private TextBox txtNome;
        private TextBox txtCaminho;
        private Button btnSalvar;
        private string path = "shortcuts.json";
        private ListBox listAtalhos;
        private List<Shortcut> atalhos;
        public Shortcut NovoAtalho { get; private set; }



        public ConfigForm(List<Shortcut> atalhosExistentes)
        {
            InitializeComponent();
            InicializarComponentesManuais();

            this.atalhos = atalhosExistentes;
            AtualizarLista();
        }

        private void AtualizarLista()
        {
            listAtalhos.Items.Clear();
            foreach (var atalho in atalhos)
            {
                listAtalhos.Items.Add($"{atalho.nome_atalho} | {atalho.caminho_atalho}");
            }
        }


        private void InicializarComponentesManuais()
        {
            this.Text = "Editar Atalho";
            this.Width = 400;
            this.Height = 320;

            Label lblNome = new Label { Text = "Nome:", Left = 20, Top = 70, Width = 100 };
            txtNome = new TextBox { Left = 120, Top = 70, Width = 200 };

            Label lblCaminho = new Label { Text = "Caminho:", Left = 20, Top = 110, Width = 100 };
            txtCaminho = new TextBox { Left = 120, Top = 110, Width = 200 };

            btnSalvar = new Button { Text = "Salvar", Left = 120, Top = 160, Width = 100 };
            btnSalvar.Click += BtnSalvar_Click;

            // Lista de atalhos existentes
            listAtalhos = new ListBox { Left = 20, Top = 200, Width = 350, Height = 100 };
            listAtalhos.SelectedIndexChanged += (s, e) =>
            {
                int index = listAtalhos.SelectedIndex;
                if (index >= 0 && index < atalhos.Count)
                {
                    var atalhoSelecionado = atalhos[index];
                    txtNome.Text = atalhoSelecionado.nome_atalho;
                    txtCaminho.Text = atalhoSelecionado.caminho_atalho;
                }
            };

            // Botão de excluir
            Button btnExcluir = new Button { Text = "Excluir", Left = 230, Top = 160, Width = 100 };
            btnExcluir.Click += async (s, e) =>
            {
                int index = listAtalhos.SelectedIndex;
                if (index >= 0 && index < atalhos.Count)
                {
                    var confirmar = MessageBox.Show("Deseja excluir este atalho?", "Confirmar", MessageBoxButtons.YesNo);
                    if (confirmar == DialogResult.Yes)
                    {
                        var atalhoSelecionado = atalhos[index].nome_atalho;

                        try
                        {
                            // Supabase config
                            var baseUrl = "https://uqpwewtyurmtadidficb.supabase.co";
                            var apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVxcHdld3R5dXJtdGFkaWRmaWNiIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDkxODExMDIsImV4cCI6MjA2NDc1NzEwMn0.Ke1knn5AyaYN6tlmB_U-Yuj4bbo_iGjuIRth8HvxWug";
                            var email = "teste_telafacil@gmail.com";
                            var senha = "admin987@";

                            var auth = new SupabaseAuth(baseUrl, apiKey);
                            var jwt = await auth.LoginAndGetAccessTokenAsync(email, senha);

                            var supabaseApi = new SupabaseApi(baseUrl, jwt, apiKey);

                            // Chamada DELETE no Supabase
                            await supabaseApi.DeletarAtalhoAsync(atalhoSelecionado);

                            // Remove da lista local após sucesso no Supabase
                            atalhos.RemoveAt(index);
                            AtualizarLista();

                            MessageBox.Show("Atalho excluído com sucesso!");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Erro ao excluir atalho: " + ex.Message + "\n\n" + ex.StackTrace);
                        }
                    }
                }
            };

            this.Controls.Add(lblNome);
            this.Controls.Add(txtNome);
            this.Controls.Add(lblCaminho);
            this.Controls.Add(txtCaminho);
            this.Controls.Add(btnSalvar);
            this.Controls.Add(btnExcluir);
            this.Controls.Add(listAtalhos);
        }


        private async void BtnSalvar_Click(object sender, EventArgs e)
        {
            var baseUrl = "https://uqpwewtyurmtadidficb.supabase.co";
            var supabaseRestUrl = baseUrl + "/rest/v1/";
            var apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVxcHdld3R5dXJtdGFkaWRmaWNiIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDkxODExMDIsImV4cCI6MjA2NDc1NzEwMn0.Ke1knn5AyaYN6tlmB_U-Yuj4bbo_iGjuIRth8HvxWug";
            var email = "teste_telafacil@gmail.com";
            var senha = "admin987@";

            var auth = new SupabaseAuth(baseUrl, apiKey);
            var jwt = await auth.LoginAndGetAccessTokenAsync(email, senha);

            var supabaseApi = new SupabaseApi(baseUrl, jwt, apiKey);

            var carregarAtalhos = await supabaseApi.GetAtalhosAsync(txtNome.Text);

            if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtCaminho.Text))
            {
                MessageBox.Show("Preencha todos os campos.");
                return;
            }

            if (carregarAtalhos != null && carregarAtalhos.Count > 0)
            {
                var insereAtalho = await supabaseApi.AtualizarAtalhoAsync(carregarAtalhos[0].nome_atalho, txtNome.Text, txtCaminho.Text);
                MessageBox.Show("Atalho Atualizado com sucesso!");

                this.Close();
                return ;
            }


            NovoAtalho = new Shortcut
                {
                    nome_atalho = txtNome.Text,
                    caminho_atalho = txtCaminho.Text,
                };
            

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
