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

        private void CarregarAtalhos()
        {
            //atalhos = new List<Shortcut>();
            //if (File.Exists(path))
            //{
            //    string json = File.ReadAllText(path);
            //    atalhos = JsonSerializer.Deserialize<List<Shortcut>>(json);
            //}

            AtualizarLista();
        }

        private void AtualizarLista()
        {
            listAtalhos.Items.Clear();
            foreach (var atalho in atalhos)
            {
                listAtalhos.Items.Add($"{atalho.Nome} | {atalho.Caminho}");
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

            // Botão de excluir
            Button btnExcluir = new Button { Text = "Excluir", Left = 230, Top = 160, Width = 100 };
            btnExcluir.Click += (s, e) =>
            {
                int index = listAtalhos.SelectedIndex;
                if (index >= 0 && index < atalhos.Count)
                {
                    var confirmar = MessageBox.Show("Deseja excluir este atalho?", "Confirmar", MessageBoxButtons.YesNo);
                    if (confirmar == DialogResult.Yes)
                    {
                        atalhos.RemoveAt(index);
                        //File.WriteAllText(path, JsonSerializer.Serialize(atalhos, new JsonSerializerOptions { WriteIndented = true }));
                        AtualizarLista();
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


        //private void BtnSalvar_Click(object sender, EventArgs e)
        //{
        //    if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtCaminho.Text))
        //    {
        //        MessageBox.Show("Preencha todos os campos.");
        //        return;
        //    }

        //    var novoAtalho = new Shortcut
        //    {
        //        Nome = txtNome.Text,
        //        Caminho = txtCaminho.Text,
        //        Icone = "" // se não for usar, pode remover da classe
        //    };

        //    atalhos.Add(novoAtalho);
        //    File.WriteAllText(path, JsonSerializer.Serialize(atalhos, new JsonSerializerOptions { WriteIndented = true }));

        //    MessageBox.Show("Atalho salvo!");
        //    this.Close(); // fecha a janela e retorna para MainForm
        //}

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtCaminho.Text))
            {
                MessageBox.Show("Preencha todos os campos.");
                return;
            }

            NovoAtalho = new Shortcut
            {
                Nome = txtNome.Text,
                Caminho = txtCaminho.Text,
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
