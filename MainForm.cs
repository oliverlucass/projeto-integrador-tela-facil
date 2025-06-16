using System;
using Npgsql;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using static System.Windows.Forms.DataFormats;
using System.Runtime.Intrinsics.X86;


namespace TelaFacilLauncher
{
    public partial class MainForm : MaterialForm
    {

        //private List<Shortcut> atalhos = new List<Shortcut>
        //    {
        //    new Shortcut
        //    {
        //    nome_atalho = "Santander",
        //    caminho_atalho = "https://www.santander.com.br/"
        //    },
        //    new Shortcut
        //    {
        //    nome_atalho = "Facebook",
        //    caminho_atalho = "https://www.facebook.com",
        //    }
        //    ,new Shortcut
        //    {
        //    nome_atalho = "Whatsapp",
        //    caminho_atalho = "https://web.whatsapp.com/"
        //    }
        //    ,new Shortcut
        //    {
        //    nome_atalho = "Gmail",
        //    caminho_atalho = "https://www.gmail.com"
        //    }
        //    };

        public MainForm()
        {
            InitializeComponent();

            //this.BackColor = Color.LightSteelBlue;

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new MaterialSkin.ColorScheme(Primary.Teal200, Primary.Teal600, Primary.Teal500, Accent.Indigo700, TextShade.BLACK);


            this.Text = string.Empty;

            string imagePath = Path.Combine(Application.StartupPath, "Images", "icons8-chatbot-96.png");
            materialFloatingActionButton1.Image = Image.FromFile(imagePath);

            this.WindowState = FormWindowState.Maximized;

            CarregarBotoes();
        }

        private async void CarregarBotoes()
        {
            flowLayoutPanel1.Controls.Clear();


            var baseUrl = "https://uqpwewtyurmtadidficb.supabase.co";
            var supabaseRestUrl = baseUrl + "/rest/v1/";
            var apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVxcHdld3R5dXJtdGFkaWRmaWNiIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDkxODExMDIsImV4cCI6MjA2NDc1NzEwMn0.Ke1knn5AyaYN6tlmB_U-Yuj4bbo_iGjuIRth8HvxWug";
            var email = "teste_telafacil@gmail.com";
            var senha = "admin987@";

            var auth = new SupabaseAuth(baseUrl, apiKey);
            var jwt = await auth.LoginAndGetAccessTokenAsync(email, senha);

            var supabaseApi = new SupabaseApi(baseUrl, jwt, apiKey);


            var carregarAtalhos = await supabaseApi.GetAtalhosAsync();

            foreach (var a in carregarAtalhos)
            {
                var botao = new MaterialButton
                {
                    Text = a.nome_atalho,
                    AutoSize = false,
                    MinimumSize = new Size(110, 120),
                    MaximumSize = new Size(522, 120),
                    //Margin = new Padding(5),
                    HighEmphasis = true,
                    Margin = new Padding(12, 6, 12, 6),
                    Density = MaterialButton.MaterialButtonDensity.Default,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,

                };
                botao.Size = botao.PreferredSize;

                // Exemplo de evento de clique para cada botão
                botao.Click += (sender, e) =>
                {
                    AbrirAtalho(a.caminho_atalho);
                };

                flowLayoutPanel1.Controls.Add(materialButton1);
                flowLayoutPanel1.Controls.Add(botao);
            }

            flowLayoutPanel1.PerformLayout();
            flowLayoutPanel1.Invalidate();
        }

        private void AbrirAtalho(string caminho)
        {
            try
            {
                if (caminho.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                {
                    // Site
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = caminho,
                        UseShellExecute = true
                    });
                }
                else if (Directory.Exists(caminho))
                {
                    // Pasta local
                    Process.Start("explorer.exe", caminho);
                }
                else if (File.Exists(caminho))
                {
                    // Arquivo ou executável local
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = caminho,
                        UseShellExecute = true
                    });
                }
                else
                {
                    MessageBox.Show("Caminho inválido ou não encontrado.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao abrir o atalho: " + ex.Message);
            }
        }

        // 🔒 Confirmação ao fechar o app
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var resultado = MessageBox.Show(
                "Tem certeza que deseja sair do TelaFácil?",
                "Confirmação de saída",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (resultado != DialogResult.Yes)
            {
                e.Cancel = true;
            }
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            if (!File.Exists("config.json"))
            {
                MessageBox.Show("Você precisará criar uma senha mestra ao abrir o cofre pela primeira vez.", "Informação");
            }

            if (PasswordVaultForm.TentarAutenticacao())
            {
                var vaultForm = new PasswordVaultForm();
                vaultForm.ShowDialog();
            }
        }

        private async void materialButton2_Click(object sender, EventArgs e)
        {
            var baseUrl = "https://uqpwewtyurmtadidficb.supabase.co";
            var supabaseRestUrl = baseUrl + "/rest/v1/";
            var apiKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVxcHdld3R5dXJtdGFkaWRmaWNiIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDkxODExMDIsImV4cCI6MjA2NDc1NzEwMn0.Ke1knn5AyaYN6tlmB_U-Yuj4bbo_iGjuIRth8HvxWug";
            var email = "teste_telafacil@gmail.com";
            var senha = "admin987@";

            var auth = new SupabaseAuth(baseUrl, apiKey);
            var jwt = await auth.LoginAndGetAccessTokenAsync(email, senha);

            var supabaseApi = new SupabaseApi(baseUrl, jwt, apiKey);

            var carregarAtalhos = await supabaseApi.GetAtalhosAsync();


            var configForm = new ConfigForm(carregarAtalhos);
            var resultado = configForm.ShowDialog();

            if (resultado == DialogResult.OK && configForm.NovoAtalho != null)
            {
                var atalho = configForm.NovoAtalho;
                var insereAtalho = await supabaseApi.InserirAtalhoAsync(atalho.nome_atalho, atalho.caminho_atalho);
                MessageBox.Show("Atalho criado com sucesso!");

                // Atualiza a lista local e recarrega os botões
                //atalhos.Add(atalho);
                CarregarBotoes();


            }
            else
            {
                CarregarBotoes(); // Mesmo que cancele, atualiza visualmente os atalhos
            }
        }

        private void materialFloatingActionButton1_Click(object sender, EventArgs e)
        {
            var chatForm = new FormChat();
            chatForm.ShowDialog();
            //CriarTituloDinamico();
            //LoadAtalhos();
        }

        private void materialTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void materialLabel1_Click(object sender, EventArgs e)
        {

        }

        private void materialLabel2_Click(object sender, EventArgs e)
        {

        }
    }
}
