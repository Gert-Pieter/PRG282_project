using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Baobab_Ridge_Wildlife_Rehabilitation_Records_System
{
    public partial class DeleteAnimal : Form
    {
        private readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "animals.txt");
        List<Animal> animalList = new List<Animal>();
        int index;
        string id;
        public DeleteAnimal()
        {
            InitializeComponent();
            InitializeDataGridView();
            LoadAnimalData();
        }
        private void InitializeDataGridView()
        {
            dgvAnimal.AutoGenerateColumns = false;
            dgvAnimal.ReadOnly = true;
            dgvAnimal.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAnimal.MultiSelect = false;
            dgvAnimal.Columns.Clear();

            dgvAnimal.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "AnimalID", HeaderText = "Animal ID" });
            dgvAnimal.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Name" });
            dgvAnimal.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Species", HeaderText = "Species" });
            dgvAnimal.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Age", HeaderText = "Age" });
            dgvAnimal.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RecoveryScore", HeaderText = "Recovery Score" });
            dgvAnimal.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Status" });
            dgvAnimal.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "HousingUnit", HeaderText = "Housing Unit" });
        }

        private void FileOverwrite()
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                foreach (Animal a in animalList)
                {
                    sw.WriteLine($"{a.AnimalID}|{a.Name}|{a.Species}|{a.Age}|{a.RecoveryScore}|{a.Status}|{a.HousingUnit}");
                }
            }
        }
        public void LoadAnimalData()
        {
            int skippedLinesCount = 0;

            try
            {
                //handle missing file
                if (!File.Exists(filePath))
                {
                    File.Create(filePath).Close();
                }

                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split('|');

                    //check if it has all 7 fields
                    if (parts.Length != 7)
                    {
                        skippedLinesCount++;
                        continue;
                    }

                    string id = parts[0].Trim();
                    string name = parts[1].Trim();
                    string species = parts[2].Trim();

                    //validate that Age and Recovery Score are valid whole numbers between 0 and 100
                    if (!int.TryParse(parts[3].Trim(), out int age) || age < 0 || age > 100 || !int.TryParse(parts[4].Trim(), out int score) || score < 0 || score > 100)
                    {
                        skippedLinesCount++;
                        continue;
                    }

                    string status = parts[5].Trim();
                    string housing = parts[6].Trim();

                    //add valid animal record to the list
                    animalList.Add(new Animal(id, name, species, age, score));
                }

                //notify the user if and how many lines were skipped
                if (skippedLinesCount > 0)
                {
                    MessageBox.Show($"File load complete. {skippedLinesCount} malformed line(s) were skipped.", "Data Load Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Access denied while trying to read animals.txt. Check file permissions.", "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"An I/O error occurred while accessing the file: {ex.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            id = txtID.Text.Trim();
            bool found = false;
            foreach (Animal p in animalList)
            {
                if (p.AnimalID == id)
                {
                    index = animalList.IndexOf(p);
                    found = true;
                }
                
            }
            if (found == true)
            {
                dgvAnimal.DataSource = new List<Animal>
                {
                    animalList[index]
                };

                btnDelete.Enabled = true;
            }
            else
            {
                MessageBox.Show($"Animal (id: {id}) was not found", "Animal Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to delete the record?",
                "Delete an Animal",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);//Message Dialog used for delete confirmation

            if (result == DialogResult.Yes)
            {
                MessageBox.Show($"Animal {id} has been deleted", "Record Deleted");
                animalList.RemoveAt(index);
                FileOverwrite();
            }
            btnDelete.Enabled = false;
            dgvAnimal.DataSource = null;
        }
    }
}
