using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Baobab_Ridge_Wildlife_Rehabilitation_Records_System.DataLayer;

namespace Baobab_Ridge_Wildlife_Rehabilitation_Records_System
{
    public partial class Form1 : Form
    {
        //this ensures that the txt file path is always wherte the exe is
        private readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "animals.txt");
        List<Animal> animalList = new List<Animal>();

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

        //loads animals.txt into the DataGridView; called on startup and after every add, update and delete
        public void LoadAnimalData()
        {
            int skippedLinesCount = 0;

            //start from an empty list each time so a refresh never shows duplicate rows
            animalList.Clear();

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

                    //ParseLine returns null for a wrong field count, a bad ID, or a non-numeric/out-of-range age or score
                    Animal animal = DataHandler.ParseLine(line);
                    if (animal == null)
                    {
                        skippedLinesCount++;
                        continue;
                    }

                    //add valid animal record to the list
                    animalList.Add(animal);
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

            //re-binding is needed because a plain List does not tell the grid that its contents changed
            dgvAnimals.DataSource = null;
            dgvAnimals.DataSource = animalList;
        }

        //opens the Add / Update form; if an ID is given, that record is loaded straight into edit mode
        private void OpenAnimalForm(string animalIdToEdit)
        {
            AddAnimal animalForm = string.IsNullOrEmpty(animalIdToEdit) ? new AddAnimal() : new AddAnimal(animalIdToEdit);

            //refresh the grid as soon as a record is added or saved, not only when the form closes
            animalForm.AnimalsChanged += (s, args) => LoadAnimalData();
            animalForm.ShowDialog();
            LoadAnimalData();
        }

        //opens the Add / Update form in Add mode
        private void AddAnimalForm_Click(object sender, EventArgs e)
        {
            OpenAnimalForm(null);
        }

        //clicking a row loads that animal into the Add / Update form in edit mode
        private void dgvAnimals_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //RowIndex is -1 when the column header is clicked
            if (e.RowIndex < 0) return;

            if (dgvAnimals.Rows[e.RowIndex].DataBoundItem is Animal selected)
            {
                OpenAnimalForm(selected.AnimalID);
            }
        }

        //opens the Add / Update form empty, so the user can type an Animal ID and click Search
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            OpenAnimalForm(null);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteAnimal delete = new DeleteAnimal();
            delete.ShowDialog();
            LoadAnimalData();
        }

        private void btnSummary_Click(object sender, EventArgs e)
        {
            string summaryFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Summary.txt");
            DateTime dt = DateTime.Now;
            int[] statusArray = { 0, 0, 0, 0, 0 };
            int total = animalList.Count;
            int combinedAge = 0, combinedRecovery = 0;
            double avgAge, avgRecovery;


            animalList.Clear();
            LoadAnimalData();

            using(StreamWriter writer = new StreamWriter(summaryFile))
            {
                writer.WriteLine("BAOBAB RIDGE WILDLIFE REHABILITATION CENTRE");
                writer.WriteLine("ANIMAL SUMMARY REPORT");
                writer.WriteLine($"Generated: {dt:yyyy-MM-dd HH:mm}");
                writer.WriteLine("---------------------------------------------");
                writer.WriteLine($"{"Total animals:",-25}{total}");
                if(total > 0)
                {
                    foreach (Animal animal in animalList)
                    {
                        combinedAge += animal.Age;
                        combinedRecovery += animal.RecoveryScore;
                        switch (animal.Status)
                        {
                            case "Critical":
                                statusArray[0]++;
                                break;
                            case "Serious":
                                statusArray[1]++;
                                break;
                            case "Stable":
                                statusArray[2]++;
                                break;
                            case "Recovering":
                                statusArray[3]++;
                                break;
                            case "Release-Ready":
                                statusArray[4]++;
                                break;
                        }
                    }
                    avgAge = combinedAge / total;
                    avgRecovery = combinedRecovery / total;
                    string formattedAvgAge = avgAge.ToString("F2", CultureInfo.InvariantCulture);//Makes sure decimal is '.' instead ','
                    string formattedAvgRecovery = avgRecovery.ToString("F2", CultureInfo.InvariantCulture);

                    writer.WriteLine($"{"Average age (years):",-25}{formattedAvgAge}");
                    writer.WriteLine($"{"Average recovery score:",-25}{formattedAvgRecovery}");
                }
                
                writer.WriteLine("---------------------------------------------");
                writer.WriteLine("Animals per Status");
                writer.WriteLine($"{"  Critical:",-25}{statusArray[0]}");
                writer.WriteLine($"{"  Serious:",-25}{statusArray[1]}");
                writer.WriteLine($"{"  Stable:",-25}{statusArray[2]}");
                writer.WriteLine($"{"  Recovering:",-25}{statusArray[3]}");
                writer.WriteLine($"{"  Release-Ready:",-25}{statusArray[4]}");
                writer.WriteLine("---------------------------------------------");
            }
        }
    }
}