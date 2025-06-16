using Npgsql;
using System;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace TelaFacilLauncher
{
    public class FormChat : MaterialForm
    {

        private RichTextBox chatBox;
        private TextBox inputBox;
        private Button btnEnviar;

<<<<<<< HEAD
        private readonly string apiKey = "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";
=======
        private readonly string apiKey = "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";
>>>>>>> eaaa9718d013aea533b2811cf23cad79dba6290b

        public FormChat()
        {
            this.Text = "Chat com IA";
            this.Size = new Size(700, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            chatBox = new RichTextBox
            {
                ReadOnly = true,
                Font = new Font("Segoe UI", 16F),
                Dock = DockStyle.Fill,
                BackColor = Color.WhiteSmoke
            };

            inputBox = new TextBox
            {
                Font = new Font("Segoe UI", 12F),
                Dock = DockStyle.Fill
            };

            btnEnviar = new Button
            {
                Text = "Enviar",
                Font = new Font("Segoe UI", 12F),
                Dock = DockStyle.Right,
                Width = 120,
                BackColor = Color.LightBlue
            };
            btnEnviar.Click += BtnEnviar_Click;

            Panel painel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 45,
                Padding = new Padding(5)
            };

            painel.Controls.Add(inputBox);
            painel.Controls.Add(btnEnviar);

            this.Controls.Add(chatBox);
            this.Controls.Add(painel);

            MostrarMensagemIA("Oi! Eu sou sua ajudante virtual. Pergunte algo sobre o uso do computador!");
        }

        private async void BtnEnviar_Click(object sender, EventArgs e)
        {
            string pergunta = inputBox.Text.Trim();
            if (string.IsNullOrEmpty(pergunta)) return;

            MostrarMensagemUsuario(pergunta);
            inputBox.Clear();
            await ObterRespostaIA(pergunta);
        }

        private void MostrarMensagemUsuario(string texto)
        {
            chatBox.SelectionColor = Color.DarkBlue;
            chatBox.AppendText($"\nVocê: {texto}\n");
        }

        private void MostrarMensagemIA(string texto)
        {
            chatBox.SelectionColor = Color.DarkGreen;
            chatBox.AppendText($"IA: {texto}\n");
        }

        private async Task ObterRespostaIA(string pergunta)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var data = new
            {
                model = "gpt-3.5-turbo",
                messages = new[] {
                    new { role = "system", content = "Você é um assistente de um APP chamado 'TelaFacil', que tem o intuíto de auxiliar idosos,aja calmo e extremamente didático. Sempre explique as coisas de forma simples e com paciência, como se estivesse ensinando um idoso com pouca experiência em informática. A função do APP é armazenar atalhos (para web app ou desktop app) em formato de botões que ficam visiveis na tela para o idoso apenas clicar no botão e ser redirecionado para a aplicação." },
                    new { role = "user", content = pergunta }
                }
            };

            var json = JsonSerializer.Serialize(data);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                if (doc.RootElement.TryGetProperty("choices", out var choices))
                {
                    string resposta = choices[0].GetProperty("message").GetProperty("content").GetString();
                    MostrarMensagemIA(resposta.Trim());
                }
                else
                {
                    MostrarMensagemIA("[Erro] Formato inesperado. Resposta:");
                    MostrarMensagemIA(responseBody);
                }
            }
            catch (Exception ex)
            {
                MostrarMensagemIA("[Erro de parsing] " + ex.Message);
                MostrarMensagemIA("Resposta bruta:");
                MostrarMensagemIA(responseBody);
            }
        }
    }
}
