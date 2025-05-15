using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace TelaFacilLauncher
{
    public partial class PasswordVaultForm : MaterialForm
    {
        private string pathSenhas = "senhas.json";
        private string pathConfig = "config.json";
        private string senhaMestra;
        private List<Senha> senhas;
        private bool mostrarSenhas = false;
        private ListBox listBox;

        public PasswordVaultForm()
        {
            InitializeComponent();
            LoadSenhas();
            ShowInterface();
        }

        public static bool TentarAutenticacao()
        {
            string pathConfig = "config.json";
            string senhaMestra;

            if (!File.Exists(pathConfig))
            {
                string novaSenha = Microsoft.VisualBasic.Interaction.InputBox("Crie uma senha mestra:", "Primeiro Acesso");
                if (string.IsNullOrWhiteSpace(novaSenha))
                {
                    MessageBox.Show("Senha inválida.");
                    return false;
                }

                var config = new Config { SenhaMestra = novaSenha };
                File.WriteAllText(pathConfig, JsonSerializer.Serialize(config));
                MessageBox.Show("Senha mestra criada com sucesso!");
                return true;
            }
            else
            {
                var json = File.ReadAllText(pathConfig);
                var config = JsonSerializer.Deserialize<Config>(json);
                senhaMestra = config.SenhaMestra;

                string input = Microsoft.VisualBasic.Interaction.InputBox("Digite a senha mestra:", "Acesso ao Cofre de Senhas");
                if (input != senhaMestra)
                {
                    MessageBox.Show("Senha incorreta.");
                    return false;
                }

                return true;
            }
        }

        private void LoadSenhas()
        {
            if (File.Exists(pathSenhas))
            {
                string json = File.ReadAllText(pathSenhas);
                senhas = JsonSerializer.Deserialize<List<Senha>>(json);
            }
            else
            {
                senhas = new List<Senha>();
            }
        }

        private void AtualizarLista()
        {
            listBox.Items.Clear();
            foreach (var s in senhas)
            {
                string senhaExibida = mostrarSenhas ? s.Valor : new string('*', s.Valor.Length);
                listBox.Items.Add($"{s.Servico} | {s.Usuario} | {senhaExibida}");
            }
        }

        private void ShowInterface()
        {
            this.Text = "Minhas Senhas";
            this.Width = 500;
            this.Height = 400;

            listBox = new ListBox { Left = 20, Top = 70, Width = 440, Height = 250 };
            AtualizarLista();

            Button btnNova = new Button { Text = "Nova Senha", Left = 20, Top = 340, Width = 100 };
            btnNova.Click += (s, e) =>
            {
                string servico = Microsoft.VisualBasic.Interaction.InputBox("Serviço:", "Nova Senha", "");
                string usuario = Microsoft.VisualBasic.Interaction.InputBox("Usuário:", "Nova Senha", "");
                string senha = Microsoft.VisualBasic.Interaction.InputBox("Senha:", "Nova Senha", "");
                if (!string.IsNullOrWhiteSpace(servico))
                {
                    senhas.Add(new Senha { Servico = servico, Usuario = usuario, Valor = senha });
                    File.WriteAllText(pathSenhas, JsonSerializer.Serialize(senhas, new JsonSerializerOptions { WriteIndented = true }));
                    MessageBox.Show("Senha salva!");
                    AtualizarLista();
                }
            };

            Button btnMostrar = new Button { Text = "👁️ Mostrar", Left = 140, Top = 340, Width = 100 };
            btnMostrar.Click += (s, e) =>
            {
                mostrarSenhas = !mostrarSenhas;
                btnMostrar.Text = mostrarSenhas ? "🔒 Ocultar" : "👁️ Mostrar";
                AtualizarLista();
            };

            this.Controls.Add(listBox);
            this.Controls.Add(btnNova);
            this.Controls.Add(btnMostrar);
        }
    }

    public class Config
    {
        public string SenhaMestra { get; set; }
    }
}
