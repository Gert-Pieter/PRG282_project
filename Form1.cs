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
    public partial class Form1 : Form
    {
        //this ensures that the txt file path is always wherte the exe is
        private readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "animals.txt");

        public Form1()
        {
            InitializeComponent();
            InitializeDataGridView();
            LoadAnimalData();
        }

        //configures DataGridView columns, headers, and read-only constraints per requirements.
        private void InitializeDataGridView()
        {
            dgvAnimals.AutoGenerateColumns = false;
            dgvAnimals.ReadOnly = true;
            dgvAnimals.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAnimals.MultiSelect = false;
            dgvAnimals.Columns.Clear();

            dgvAnimals.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "AnimalID", HeaderText = "Animal ID" });
            dgvAnimals.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Name", HeaderText = "Name" });
            dgvAnimals.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Species", HeaderText = "Species" });
            dgvAnimals.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Age", HeaderText = "Age" });
            dgvAnimals.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "RecoveryScore", HeaderText = "Recovery Score" });
            dgvAnimals.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "Status", HeaderText = "Status" });
            dgvAnimals.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "HousingUnit", HeaderText = "Housing Unit" });
        }

        //loads animals.txt into the DataGridView on startup
        public void LoadAnimalData()
        {
        List<Animal> animalList = new List<Animal>();
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

                dgvAnimals.DataSource = animalList;
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

        private void AddAnimalForm_Click(object sender, EventArgs e)
        {
            AddAnimal addAnimal = new AddAnimal();
            addAnimal.ShowDialog();

            LoadAnimalData();

        }
    }
}