using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Baobab_Ridge_Wildlife_Rehabilitation_Records_System
{
    public partial class AddAnimal : Form
    {
        //this ensures that the txt file path is always wherte the exe is
        private readonly string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "animals.txt");

        public AddAnimal()
        {
            InitializeComponent();

            // data validation for recovery score range
            RecoveryScoreRange.Minimum = 0;
            RecoveryScoreRange.Maximum = 100;

            //data validation for animal age
            animalAge.Minimum = 0;
            animalAge.Maximum = 100;

        }
        // accidently clikced on label its just here for the ride i guess
        private void label1_Click(object sender, EventArgs e){}

        private void RecoveryScoreRange_ValueChanged(object sender, EventArgs e)
        {

        }

        private void AddAnimalBtn_Click(object sender, EventArgs e)
        {
            //ensures that there is no whitespace
            string AnimalID = animalId.Text.Trim(); 
            string AnimalName = animalName.Text.Trim();
            string AnimalSpecies = animalSpecies.Text.Trim();
            string AnimalRange = RecoveryScoreRange.Value.ToString();
            string AnimalAge = animalAge.Value.ToString();


            // Validation for Animal ID. Ensures that there are input as well as the correct format
            if (string.IsNullOrEmpty(AnimalID) || !Regex.IsMatch(AnimalID,@"^[Ww][Rr]-\d{4}$"))
            {
                MessageBox.Show("Error: Invalid Animal ID format. Format should follow with 'WR-' followed by 4 numbers (eg. WR-1139)"
                    ,"Validation Error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                animalId.Focus();
                return;
            }
            //Vaildation for Animal name. Ensures that there are input and the name follows the correct format
            if (string.IsNullOrEmpty(AnimalName) || AnimalName.Length > 50)
            {
                MessageBox.Show("Error: Invalid Animal Name format, Name should be between 1 and 30 characters long", "Validation Error"
                    , MessageBoxButtons.OK, MessageBoxIcon.Warning);
                animalName.Focus();
                return;
            }
            //Validation for animal species, make sure there are input
            if (string.IsNullOrEmpty(AnimalSpecies))
            {
                MessageBox.Show("Error: Invalid animal Spesies","Validation Error",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                animalSpecies.Focus(); 
                return;
            }

            try 
            {
                if (File.Exists(filePath))
                {
                    string[] data = File.ReadAllLines(filePath);
                    foreach (var item in data)
                    {
                        string[] dataParts = item.Split('|');
                        if (dataParts.Length > 0 && dataParts[0].Trim().Equals(AnimalID,StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show($"An animal ID {AnimalID} already exisits.", "ID Already exisits", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            animalId.Focus();
                            return;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking existing records {ex.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // calling the classification method to determine the housing and status 



            Animal ani = new Animal(AnimalID, AnimalName, AnimalSpecies, int.Parse(AnimalAge), int.Parse(AnimalRange));
            string status = ani.Status;
            string housing = ani.HousingUnit;

            string NewLine = $"{AnimalID.ToUpper()}|{AnimalName}|{AnimalSpecies}|{AnimalAge}|{AnimalRange}|{status}|{housing}";

            try
            {
                File.AppendAllText(filePath, NewLine + Environment.NewLine);

                MessageBox.Show($"Animal {AnimalID} successfull added!","Success",MessageBoxButtons.OK,MessageBoxIcon.Information);

                animalId.Clear();
                animalName.Clear();
                animalSpecies.Clear();
                RecoveryScoreRange.Value = 0;
                animalAge.Value = 0;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Permission denied: Cannot write to animals.txt. Check folder permissions.",
            "Access Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException)
            {
                MessageBox.Show("The file animals.txt is currently in use by another program. Please close it and try again.",
                    "File In Use Error", MessageBoxButtons.OK, MessageBoxIcon.Error); 
    }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
