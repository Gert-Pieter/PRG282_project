namespace Baobab_Ridge_Wildlife_Rehabilitation_Records_System
{
    partial class AddAnimal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.animalId = new System.Windows.Forms.TextBox();
            this.animalName = new System.Windows.Forms.TextBox();
            this.animalSpecies = new System.Windows.Forms.TextBox();
            this.RecoveryScoreRange = new System.Windows.Forms.NumericUpDown();
            this.AddAnimalBtn = new System.Windows.Forms.Button();
            this.animalAge = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.RecoveryScoreRange)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.animalAge)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Animal ID";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Species";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 121);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "Age (In Years)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 149);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "Recovery Score";
            // 
            // animalId
            // 
            this.animalId.Location = new System.Drawing.Point(151, 33);
            this.animalId.Name = "animalId";
            this.animalId.Size = new System.Drawing.Size(142, 22);
            this.animalId.TabIndex = 5;
            // 
            // animalName
            // 
            this.animalName.Location = new System.Drawing.Point(151, 61);
            this.animalName.Name = "animalName";
            this.animalName.Size = new System.Drawing.Size(142, 22);
            this.animalName.TabIndex = 6;
            // 
            // animalSpecies
            // 
            this.animalSpecies.Location = new System.Drawing.Point(151, 91);
            this.animalSpecies.Name = "animalSpecies";
            this.animalSpecies.Size = new System.Drawing.Size(142, 22);
            this.animalSpecies.TabIndex = 7;
            // 
            // RecoveryScoreRange
            // 
            this.RecoveryScoreRange.Location = new System.Drawing.Point(151, 150);
            this.RecoveryScoreRange.Name = "RecoveryScoreRange";
            this.RecoveryScoreRange.Size = new System.Drawing.Size(120, 22);
            this.RecoveryScoreRange.TabIndex = 9;
            this.RecoveryScoreRange.ValueChanged += new System.EventHandler(this.RecoveryScoreRange_ValueChanged);
            // 
            // AddAnimalBtn
            // 
            this.AddAnimalBtn.Location = new System.Drawing.Point(28, 185);
            this.AddAnimalBtn.Name = "AddAnimalBtn";
            this.AddAnimalBtn.Size = new System.Drawing.Size(142, 23);
            this.AddAnimalBtn.TabIndex = 10;
            this.AddAnimalBtn.Text = "Add Animal";
            this.AddAnimalBtn.UseVisualStyleBackColor = true;
            this.AddAnimalBtn.Click += new System.EventHandler(this.AddAnimalBtn_Click);
            // 
            // animalAge
            // 
            this.animalAge.Location = new System.Drawing.Point(151, 119);
            this.animalAge.Name = "animalAge";
            this.animalAge.Size = new System.Drawing.Size(120, 22);
            this.animalAge.TabIndex = 11;
            // 
            // AddAnimal
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.animalAge);
            this.Controls.Add(this.AddAnimalBtn);
            this.Controls.Add(this.RecoveryScoreRange);
            this.Controls.Add(this.animalSpecies);
            this.Controls.Add(this.animalName);
            this.Controls.Add(this.animalId);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "AddAnimal";
            this.Text = "AddAnimal";
            ((System.ComponentModel.ISupportInitialize)(this.RecoveryScoreRange)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.animalAge)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox animalId;
        private System.Windows.Forms.TextBox animalName;
        private System.Windows.Forms.TextBox animalSpecies;
        private System.Windows.Forms.NumericUpDown RecoveryScoreRange;
        private System.Windows.Forms.Button AddAnimalBtn;
        private System.Windows.Forms.NumericUpDown animalAge;
    }
}