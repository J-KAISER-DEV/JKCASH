namespace JKCASH.WinForms
{
    partial class FrmPrincipal
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pnlMenuPrincipal = new Panel();
            pnlConteudo = new Panel();
            SuspendLayout();
            // 
            // pnlMenuPrincipal
            // 
            pnlMenuPrincipal.BackColor = Color.FromArgb(20, 38, 61);
            pnlMenuPrincipal.Dock = DockStyle.Left;
            pnlMenuPrincipal.Location = new Point(0, 0);
            pnlMenuPrincipal.Name = "pnlMenuPrincipal";
            pnlMenuPrincipal.Size = new Size(220, 860);
            pnlMenuPrincipal.TabIndex = 0;
            // 
            // pnlConteudo
            // 
            pnlConteudo.Dock = DockStyle.Fill;
            pnlConteudo.Location = new Point(220, 0);
            pnlConteudo.Name = "pnlConteudo";
            pnlConteudo.Size = new Size(1180, 860);
            pnlConteudo.TabIndex = 1;
            // 
            // FrmPrincipal
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(244, 246, 248);
            ClientSize = new Size(1400, 860);
            Controls.Add(pnlConteudo);
            Controls.Add(pnlMenuPrincipal);
            Font = new Font("Segoe UI", 10F);
            MinimumSize = new Size(1200, 760);
            Name = "FrmPrincipal";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "JKCASH";
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlMenuPrincipal;
        private Panel pnlConteudo;
    }
}
