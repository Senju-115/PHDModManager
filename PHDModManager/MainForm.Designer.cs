namespace PHDModManager
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.PanelWeapon = new System.Windows.Forms.Panel();
            this.WeaponLabel = new System.Windows.Forms.Label();
            this.ToggleSwitchWeapons = new ToggleSwitch();
            this.ExpandButtonWeapons = new ExpandButton();
            this.PanelPerks = new System.Windows.Forms.Panel();
            this.PerksLabel = new System.Windows.Forms.Label();
            this.ToggleSwitchPerks = new ToggleSwitch();
            this.ExpandButtonPerks = new ExpandButton();
            this.PanelScripts = new System.Windows.Forms.Panel();
            this.ScriptsLabel = new System.Windows.Forms.Label();
            this.ToggleSwitchScripts = new ToggleSwitch();
            this.ExpandButtonScripts = new ExpandButton();
            this.PanelHud = new System.Windows.Forms.Panel();
            this.HudLabel = new System.Windows.Forms.Label();
            this.ToggleSwitchHud = new ToggleSwitch();
            this.ExpandButtonHud = new ExpandButton();
            this.PanelGloves = new System.Windows.Forms.Panel();
            this.Gloves = new System.Windows.Forms.Label();
            this.ToggleSwitchGloves = new ToggleSwitch();
            this.ExpandButtonGloves = new ExpandButton();
            this.DetailWeapons = new System.Windows.Forms.Panel();
            this.ItemsWeapons = new System.Windows.Forms.Panel();
            this.IconSearchWeapons = new System.Windows.Forms.Label();
            this.SearchWeapons = new System.Windows.Forms.TextBox();
            this.DetailPerks = new System.Windows.Forms.Panel();
            this.ItemsPerks = new System.Windows.Forms.Panel();
            this.IconSearchPerks = new System.Windows.Forms.Label();
            this.SearchPerks = new System.Windows.Forms.TextBox();
            this.DetailGlove = new System.Windows.Forms.Panel();
            this.ItemsGloves = new System.Windows.Forms.Panel();
            this.IconSearchGloves = new System.Windows.Forms.Label();
            this.SearchGloves = new System.Windows.Forms.TextBox();
            this.DetailScripts = new System.Windows.Forms.Panel();
            this.ItemsScripts = new System.Windows.Forms.Panel();
            this.IconSearchScripts = new System.Windows.Forms.Label();
            this.SearchScripts = new System.Windows.Forms.TextBox();
            this.DetailHud = new System.Windows.Forms.Panel();
            this.ItemsHud = new System.Windows.Forms.Panel();
            this.IconSearchHud = new System.Windows.Forms.Label();
            this.SearchHud = new System.Windows.Forms.TextBox();
            this.PanelWeapon.SuspendLayout();
            this.PanelPerks.SuspendLayout();
            this.PanelScripts.SuspendLayout();
            this.PanelHud.SuspendLayout();
            this.PanelGloves.SuspendLayout();
            this.DetailWeapons.SuspendLayout();
            this.DetailPerks.SuspendLayout();
            this.DetailGlove.SuspendLayout();
            this.DetailScripts.SuspendLayout();
            this.DetailHud.SuspendLayout();
            this.SuspendLayout();
            // 
            // PanelWeapon
            // 
            this.PanelWeapon.Controls.Add(this.WeaponLabel);
            this.PanelWeapon.Controls.Add(this.ToggleSwitchWeapons);
            this.PanelWeapon.Controls.Add(this.ExpandButtonWeapons);
            this.PanelWeapon.Location = new System.Drawing.Point(10, 10);
            this.PanelWeapon.Name = "PanelWeapon";
            this.PanelWeapon.Size = new System.Drawing.Size(360, 40);
            this.PanelWeapon.TabIndex = 0;
            // 
            // WeaponLabel
            // 
            this.WeaponLabel.AutoSize = true;
            this.WeaponLabel.Location = new System.Drawing.Point(12, 13);
            this.WeaponLabel.Name = "WeaponLabel";
            this.WeaponLabel.Size = new System.Drawing.Size(95, 16);
            this.WeaponLabel.TabIndex = 0;
            this.WeaponLabel.Text = "Weapon Skins";
            this.WeaponLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // ToggleSwitchWeapons
            // 
            this.ToggleSwitchWeapons.Checked = false;
            this.ToggleSwitchWeapons.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ToggleSwitchWeapons.Location = new System.Drawing.Point(260, 9);
            this.ToggleSwitchWeapons.Name = "ToggleSwitchWeapons";
            this.ToggleSwitchWeapons.Size = new System.Drawing.Size(46, 22);
            this.ToggleSwitchWeapons.TabIndex = 1;
            // 
            // ExpandButtonWeapons
            // 
            this.ExpandButtonWeapons.BackColor = System.Drawing.Color.Transparent;
            this.ExpandButtonWeapons.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ExpandButtonWeapons.Expanded = false;
            this.ExpandButtonWeapons.Location = new System.Drawing.Point(320, 8);
            this.ExpandButtonWeapons.Name = "ExpandButtonWeapons";
            this.ExpandButtonWeapons.Size = new System.Drawing.Size(24, 24);
            this.ExpandButtonWeapons.TabIndex = 2;
            this.ExpandButtonWeapons.Click += new System.EventHandler(this.ExpandButtonWeapons_Click);
            // 
            // PanelPerks
            // 
            this.PanelPerks.Controls.Add(this.PerksLabel);
            this.PanelPerks.Controls.Add(this.ToggleSwitchPerks);
            this.PanelPerks.Controls.Add(this.ExpandButtonPerks);
            this.PanelPerks.Location = new System.Drawing.Point(10, 60);
            this.PanelPerks.Name = "PanelPerks";
            this.PanelPerks.Size = new System.Drawing.Size(360, 40);
            this.PanelPerks.TabIndex = 2;
            // 
            // PerksLabel
            // 
            this.PerksLabel.AutoSize = true;
            this.PerksLabel.Location = new System.Drawing.Point(12, 13);
            this.PerksLabel.Name = "PerksLabel";
            this.PerksLabel.Size = new System.Drawing.Size(71, 16);
            this.PerksLabel.TabIndex = 0;
            this.PerksLabel.Text = "Perk Skins";
            this.PerksLabel.Click += new System.EventHandler(this.label1_Click_1);
            // 
            // ToggleSwitchPerks
            // 
            this.ToggleSwitchPerks.Checked = false;
            this.ToggleSwitchPerks.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ToggleSwitchPerks.Location = new System.Drawing.Point(260, 9);
            this.ToggleSwitchPerks.Name = "ToggleSwitchPerks";
            this.ToggleSwitchPerks.Size = new System.Drawing.Size(46, 22);
            this.ToggleSwitchPerks.TabIndex = 1;
            // 
            // ExpandButtonPerks
            // 
            this.ExpandButtonPerks.BackColor = System.Drawing.Color.Transparent;
            this.ExpandButtonPerks.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ExpandButtonPerks.Expanded = false;
            this.ExpandButtonPerks.Location = new System.Drawing.Point(320, 8);
            this.ExpandButtonPerks.Name = "ExpandButtonPerks";
            this.ExpandButtonPerks.Size = new System.Drawing.Size(24, 24);
            this.ExpandButtonPerks.TabIndex = 2;
            // 
            // PanelScripts
            // 
            this.PanelScripts.Controls.Add(this.ScriptsLabel);
            this.PanelScripts.Controls.Add(this.ToggleSwitchScripts);
            this.PanelScripts.Controls.Add(this.ExpandButtonScripts);
            this.PanelScripts.Location = new System.Drawing.Point(10, 158);
            this.PanelScripts.Name = "PanelScripts";
            this.PanelScripts.Size = new System.Drawing.Size(360, 40);
            this.PanelScripts.TabIndex = 6;
            // 
            // ScriptsLabel
            // 
            this.ScriptsLabel.AutoSize = true;
            this.ScriptsLabel.Location = new System.Drawing.Point(12, 13);
            this.ScriptsLabel.Name = "ScriptsLabel";
            this.ScriptsLabel.Size = new System.Drawing.Size(48, 16);
            this.ScriptsLabel.TabIndex = 0;
            this.ScriptsLabel.Text = "Scripts";
            // 
            // ToggleSwitchScripts
            // 
            this.ToggleSwitchScripts.Checked = false;
            this.ToggleSwitchScripts.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ToggleSwitchScripts.Location = new System.Drawing.Point(260, 9);
            this.ToggleSwitchScripts.Name = "ToggleSwitchScripts";
            this.ToggleSwitchScripts.Size = new System.Drawing.Size(46, 22);
            this.ToggleSwitchScripts.TabIndex = 1;
            // 
            // ExpandButtonScripts
            // 
            this.ExpandButtonScripts.BackColor = System.Drawing.Color.Transparent;
            this.ExpandButtonScripts.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ExpandButtonScripts.Expanded = false;
            this.ExpandButtonScripts.Location = new System.Drawing.Point(320, 8);
            this.ExpandButtonScripts.Name = "ExpandButtonScripts";
            this.ExpandButtonScripts.Size = new System.Drawing.Size(24, 24);
            this.ExpandButtonScripts.TabIndex = 2;
            // 
            // PanelHud
            // 
            this.PanelHud.Controls.Add(this.HudLabel);
            this.PanelHud.Controls.Add(this.ToggleSwitchHud);
            this.PanelHud.Controls.Add(this.ExpandButtonHud);
            this.PanelHud.Location = new System.Drawing.Point(12, 206);
            this.PanelHud.Name = "PanelHud";
            this.PanelHud.Size = new System.Drawing.Size(360, 40);
            this.PanelHud.TabIndex = 8;
            this.PanelHud.Paint += new System.Windows.Forms.PaintEventHandler(this.panel4_Paint);
            // 
            // HudLabel
            // 
            this.HudLabel.AutoSize = true;
            this.HudLabel.Location = new System.Drawing.Point(12, 13);
            this.HudLabel.Name = "HudLabel";
            this.HudLabel.Size = new System.Drawing.Size(79, 16);
            this.HudLabel.TabIndex = 0;
            this.HudLabel.Text = "HUD Player";
            // 
            // ToggleSwitchHud
            // 
            this.ToggleSwitchHud.Checked = false;
            this.ToggleSwitchHud.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ToggleSwitchHud.Location = new System.Drawing.Point(260, 9);
            this.ToggleSwitchHud.Name = "ToggleSwitchHud";
            this.ToggleSwitchHud.Size = new System.Drawing.Size(46, 22);
            this.ToggleSwitchHud.TabIndex = 1;
            // 
            // ExpandButtonHud
            // 
            this.ExpandButtonHud.BackColor = System.Drawing.Color.Transparent;
            this.ExpandButtonHud.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ExpandButtonHud.Expanded = false;
            this.ExpandButtonHud.Location = new System.Drawing.Point(320, 8);
            this.ExpandButtonHud.Name = "ExpandButtonHud";
            this.ExpandButtonHud.Size = new System.Drawing.Size(24, 24);
            this.ExpandButtonHud.TabIndex = 2;
            // 
            // PanelGloves
            // 
            this.PanelGloves.Controls.Add(this.Gloves);
            this.PanelGloves.Controls.Add(this.ToggleSwitchGloves);
            this.PanelGloves.Controls.Add(this.ExpandButtonGloves);
            this.PanelGloves.Location = new System.Drawing.Point(10, 110);
            this.PanelGloves.Name = "PanelGloves";
            this.PanelGloves.Size = new System.Drawing.Size(360, 40);
            this.PanelGloves.TabIndex = 4;
            // 
            // Gloves
            // 
            this.Gloves.AutoSize = true;
            this.Gloves.Location = new System.Drawing.Point(12, 13);
            this.Gloves.Name = "Gloves";
            this.Gloves.Size = new System.Drawing.Size(50, 16);
            this.Gloves.TabIndex = 0;
            this.Gloves.Text = "Gloves";
            this.Gloves.Click += new System.EventHandler(this.label4_Click);
            // 
            // ToggleSwitchGloves
            // 
            this.ToggleSwitchGloves.Checked = false;
            this.ToggleSwitchGloves.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ToggleSwitchGloves.Location = new System.Drawing.Point(260, 9);
            this.ToggleSwitchGloves.Name = "ToggleSwitchGloves";
            this.ToggleSwitchGloves.Size = new System.Drawing.Size(46, 22);
            this.ToggleSwitchGloves.TabIndex = 1;
            // 
            // ExpandButtonGloves
            // 
            this.ExpandButtonGloves.BackColor = System.Drawing.Color.Transparent;
            this.ExpandButtonGloves.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ExpandButtonGloves.Expanded = false;
            this.ExpandButtonGloves.Location = new System.Drawing.Point(320, 8);
            this.ExpandButtonGloves.Name = "ExpandButtonGloves";
            this.ExpandButtonGloves.Size = new System.Drawing.Size(24, 24);
            this.ExpandButtonGloves.TabIndex = 2;
            // 
            // DetailWeapons
            // 
            this.DetailWeapons.Controls.Add(this.ItemsWeapons);
            this.DetailWeapons.Controls.Add(this.IconSearchWeapons);
            this.DetailWeapons.Controls.Add(this.SearchWeapons);
            this.DetailWeapons.Location = new System.Drawing.Point(10, 54);
            this.DetailWeapons.Name = "DetailWeapons";
            this.DetailWeapons.Size = new System.Drawing.Size(360, 0);
            this.DetailWeapons.TabIndex = 1;
            // 
            // ItemsWeapons
            // 
            this.ItemsWeapons.AutoScroll = true;
            this.ItemsWeapons.Location = new System.Drawing.Point(15, 31);
            this.ItemsWeapons.Name = "ItemsWeapons";
            this.ItemsWeapons.Size = new System.Drawing.Size(322, 111);
            this.ItemsWeapons.TabIndex = 2;
            // 
            // IconSearchWeapons
            // 
            this.IconSearchWeapons.AutoSize = true;
            this.IconSearchWeapons.Location = new System.Drawing.Point(12, 6);
            this.IconSearchWeapons.Name = "IconSearchWeapons";
            this.IconSearchWeapons.Size = new System.Drawing.Size(19, 16);
            this.IconSearchWeapons.TabIndex = 1;
            this.IconSearchWeapons.Text = "🔍";
            this.IconSearchWeapons.Click += new System.EventHandler(this.IconSearchWeapons_Click);
            // 
            // SearchWeapons
            // 
            this.SearchWeapons.Location = new System.Drawing.Point(37, 3);
            this.SearchWeapons.Name = "SearchWeapons";
            this.SearchWeapons.Size = new System.Drawing.Size(300, 22);
            this.SearchWeapons.TabIndex = 0;
            // 
            // DetailPerks
            // 
            this.DetailPerks.Controls.Add(this.ItemsPerks);
            this.DetailPerks.Controls.Add(this.IconSearchPerks);
            this.DetailPerks.Controls.Add(this.SearchPerks);
            this.DetailPerks.Location = new System.Drawing.Point(10, 104);
            this.DetailPerks.Name = "DetailPerks";
            this.DetailPerks.Size = new System.Drawing.Size(360, 0);
            this.DetailPerks.TabIndex = 3;
            // 
            // ItemsPerks
            // 
            this.ItemsPerks.AutoScroll = true;
            this.ItemsPerks.Location = new System.Drawing.Point(15, 31);
            this.ItemsPerks.Name = "ItemsPerks";
            this.ItemsPerks.Size = new System.Drawing.Size(322, 111);
            this.ItemsPerks.TabIndex = 2;
            // 
            // IconSearchPerks
            // 
            this.IconSearchPerks.AutoSize = true;
            this.IconSearchPerks.Location = new System.Drawing.Point(12, 6);
            this.IconSearchPerks.Name = "IconSearchPerks";
            this.IconSearchPerks.Size = new System.Drawing.Size(19, 16);
            this.IconSearchPerks.TabIndex = 1;
            this.IconSearchPerks.Text = "🔍";
            // 
            // SearchPerks
            // 
            this.SearchPerks.Location = new System.Drawing.Point(37, 3);
            this.SearchPerks.Name = "SearchPerks";
            this.SearchPerks.Size = new System.Drawing.Size(300, 22);
            this.SearchPerks.TabIndex = 0;
            // 
            // DetailGlove
            // 
            this.DetailGlove.Controls.Add(this.ItemsGloves);
            this.DetailGlove.Controls.Add(this.IconSearchGloves);
            this.DetailGlove.Controls.Add(this.SearchGloves);
            this.DetailGlove.Location = new System.Drawing.Point(10, 154);
            this.DetailGlove.Name = "DetailGlove";
            this.DetailGlove.Size = new System.Drawing.Size(360, 0);
            this.DetailGlove.TabIndex = 5;
            // 
            // ItemsGloves
            // 
            this.ItemsGloves.AutoScroll = true;
            this.ItemsGloves.Location = new System.Drawing.Point(15, 31);
            this.ItemsGloves.Name = "ItemsGloves";
            this.ItemsGloves.Size = new System.Drawing.Size(322, 111);
            this.ItemsGloves.TabIndex = 2;
            // 
            // IconSearchGloves
            // 
            this.IconSearchGloves.AutoSize = true;
            this.IconSearchGloves.Location = new System.Drawing.Point(12, 6);
            this.IconSearchGloves.Name = "IconSearchGloves";
            this.IconSearchGloves.Size = new System.Drawing.Size(19, 16);
            this.IconSearchGloves.TabIndex = 1;
            this.IconSearchGloves.Text = "🔍";
            // 
            // SearchGloves
            // 
            this.SearchGloves.Location = new System.Drawing.Point(37, 3);
            this.SearchGloves.Name = "SearchGloves";
            this.SearchGloves.Size = new System.Drawing.Size(300, 22);
            this.SearchGloves.TabIndex = 0;
            // 
            // DetailScripts
            // 
            this.DetailScripts.Controls.Add(this.ItemsScripts);
            this.DetailScripts.Controls.Add(this.IconSearchScripts);
            this.DetailScripts.Controls.Add(this.SearchScripts);
            this.DetailScripts.Location = new System.Drawing.Point(10, 204);
            this.DetailScripts.Name = "DetailScripts";
            this.DetailScripts.Size = new System.Drawing.Size(360, 0);
            this.DetailScripts.TabIndex = 7;
            // 
            // ItemsScripts
            // 
            this.ItemsScripts.AutoScroll = true;
            this.ItemsScripts.Location = new System.Drawing.Point(15, 31);
            this.ItemsScripts.Name = "ItemsScripts";
            this.ItemsScripts.Size = new System.Drawing.Size(322, 111);
            this.ItemsScripts.TabIndex = 2;
            // 
            // IconSearchScripts
            // 
            this.IconSearchScripts.AutoSize = true;
            this.IconSearchScripts.Location = new System.Drawing.Point(12, 6);
            this.IconSearchScripts.Name = "IconSearchScripts";
            this.IconSearchScripts.Size = new System.Drawing.Size(19, 16);
            this.IconSearchScripts.TabIndex = 1;
            this.IconSearchScripts.Text = "🔍";
            // 
            // SearchScripts
            // 
            this.SearchScripts.Location = new System.Drawing.Point(37, 3);
            this.SearchScripts.Name = "SearchScripts";
            this.SearchScripts.Size = new System.Drawing.Size(300, 22);
            this.SearchScripts.TabIndex = 0;
            // 
            // DetailHud
            // 
            this.DetailHud.Controls.Add(this.ItemsHud);
            this.DetailHud.Controls.Add(this.IconSearchHud);
            this.DetailHud.Controls.Add(this.SearchHud);
            this.DetailHud.Location = new System.Drawing.Point(10, 260);
            this.DetailHud.Name = "DetailHud";
            this.DetailHud.Size = new System.Drawing.Size(360, 0);
            this.DetailHud.TabIndex = 9;
            // 
            // ItemsHud
            // 
            this.ItemsHud.AutoScroll = true;
            this.ItemsHud.Location = new System.Drawing.Point(15, 31);
            this.ItemsHud.Name = "ItemsHud";
            this.ItemsHud.Size = new System.Drawing.Size(322, 111);
            this.ItemsHud.TabIndex = 2;
            // 
            // IconSearchHud
            // 
            this.IconSearchHud.AutoSize = true;
            this.IconSearchHud.Location = new System.Drawing.Point(12, 6);
            this.IconSearchHud.Name = "IconSearchHud";
            this.IconSearchHud.Size = new System.Drawing.Size(19, 16);
            this.IconSearchHud.TabIndex = 1;
            this.IconSearchHud.Text = "🔍";
            // 
            // SearchHud
            // 
            this.SearchHud.Location = new System.Drawing.Point(37, 3);
            this.SearchHud.Name = "SearchHud";
            this.SearchHud.Size = new System.Drawing.Size(300, 22);
            this.SearchHud.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 719);
            this.Controls.Add(this.PanelWeapon);
            this.Controls.Add(this.DetailWeapons);
            this.Controls.Add(this.PanelPerks);
            this.Controls.Add(this.DetailPerks);
            this.Controls.Add(this.PanelGloves);
            this.Controls.Add(this.DetailGlove);
            this.Controls.Add(this.PanelScripts);
            this.Controls.Add(this.DetailScripts);
            this.Controls.Add(this.PanelHud);
            this.Controls.Add(this.DetailHud);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "PHDModManager";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.PanelWeapon.ResumeLayout(false);
            this.PanelWeapon.PerformLayout();
            this.PanelPerks.ResumeLayout(false);
            this.PanelPerks.PerformLayout();
            this.PanelScripts.ResumeLayout(false);
            this.PanelScripts.PerformLayout();
            this.PanelHud.ResumeLayout(false);
            this.PanelHud.PerformLayout();
            this.PanelGloves.ResumeLayout(false);
            this.PanelGloves.PerformLayout();
            this.DetailWeapons.ResumeLayout(false);
            this.DetailWeapons.PerformLayout();
            this.DetailPerks.ResumeLayout(false);
            this.DetailPerks.PerformLayout();
            this.DetailGlove.ResumeLayout(false);
            this.DetailGlove.PerformLayout();
            this.DetailScripts.ResumeLayout(false);
            this.DetailScripts.PerformLayout();
            this.DetailHud.ResumeLayout(false);
            this.DetailHud.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.Panel PanelWeapon;
        private System.Windows.Forms.Label WeaponLabel;
        private ToggleSwitch ToggleSwitchWeapons;
        private ExpandButton ExpandButtonWeapons;
        private System.Windows.Forms.Panel PanelPerks;
        private ExpandButton ExpandButtonPerks;
        private ToggleSwitch ToggleSwitchPerks;
        private System.Windows.Forms.Label PerksLabel;
        private System.Windows.Forms.Panel PanelScripts;
        private ExpandButton ExpandButtonScripts;
        private ToggleSwitch ToggleSwitchScripts;
        private System.Windows.Forms.Label ScriptsLabel;
        private System.Windows.Forms.Panel PanelHud;
        private ExpandButton ExpandButtonHud;
        private ToggleSwitch ToggleSwitchHud;
        private System.Windows.Forms.Label HudLabel;
        private System.Windows.Forms.Panel PanelGloves;
        private ExpandButton ExpandButtonGloves;
        private ToggleSwitch ToggleSwitchGloves;
        private System.Windows.Forms.Label Gloves;
        private System.Windows.Forms.Panel DetailWeapons;
        private System.Windows.Forms.Panel DetailPerks;
        private System.Windows.Forms.Panel DetailGlove;
        private System.Windows.Forms.Panel DetailScripts;
        private System.Windows.Forms.Panel DetailHud;
        private System.Windows.Forms.Label IconSearchWeapons;
        private System.Windows.Forms.Panel ItemsWeapons;
        private System.Windows.Forms.TextBox SearchWeapons;
        private System.Windows.Forms.Panel ItemsPerks;
        private System.Windows.Forms.Label IconSearchPerks;
        private System.Windows.Forms.TextBox SearchPerks;
        private System.Windows.Forms.Panel ItemsGloves;
        private System.Windows.Forms.Label IconSearchGloves;
        private System.Windows.Forms.TextBox SearchGloves;
        private System.Windows.Forms.Panel ItemsScripts;
        private System.Windows.Forms.Label IconSearchScripts;
        private System.Windows.Forms.TextBox SearchScripts;
        private System.Windows.Forms.Panel ItemsHud;
        private System.Windows.Forms.Label IconSearchHud;
        private System.Windows.Forms.TextBox SearchHud;
    }
}