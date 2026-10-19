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
using Baobab_Ridge_Wildlife_Rehabilitation_Records_System.DataLayer;

namespace Baobab_Ridge_Wildlife_Rehabilitation_Records_System
{
    public partial class AddAnimal : Form
    {
        //true while an existing record is loaded for editing (Feature 3)
        private bool isEditMode = false;

        //raised after every successful add or save so the main form can refresh its grid straight away
        public event EventHandler AnimalsChanged;

        //opens the form in Add mode
        public AddAnimal()
        {
            InitializeComponent();

            // data validation for recovery score range
            RecoveryScoreRange.Minimum = 0;
            RecoveryScoreRange.Maximum = 100;

            //data validation for animal age
            animalAge.Minimum = 0;
            animalAge.Maximum = 100;

            EnterAddMode();
        }

        //opens the form with an existing record already loaded for editing (used when a grid row is clicked)
        public AddAnimal(string animalIdToEdit) : this()
        {
            LoadRecordById(animalIdToEdit);
        }

        // accidently clikced on label its just here for the ride i guess
        private void label1_Click(object sender, EventArgs e){}

        private void RecoveryScoreRange_ValueChanged(object sender, EventArgs e)
        {

        }

        //validates every input box using the Section 3 rules; shared by Add and Save Changes
        //checkIdIsUnique is false when saving, because the ID of an existing record cannot change
        private bool ValidateInputs(bool checkIdIsUnique, out Animal validAnimal)
        {
            validAnimal = null;

            //ensures that there is no whitespace, and the ID is upper case before it is checked
            string AnimalID = animalId.Text.Trim().ToUpper();
            string AnimalName = animalName.Text.Trim();
            string AnimalSpecies = animalSpecies.Text.Trim();

            // Validation for Animal ID. Ensures that there are input as well as the correct format
            if (string.IsNullOrEmpty(AnimalID) || !Regex.IsMatch(AnimalID, @"^WR-\d{4}$"))
            {
                ShowValidationError("Animal ID must be 'WR-' followed by exactly 4 digits (e.g. WR-0007).", animalId);
                return false;
            }
            //Vaildation for Animal name. Required, 1-30 characters, no pipe character
            if (string.IsNullOrEmpty(AnimalName) || AnimalName.Length > 30 || AnimalName.Contains("|"))
            {
                ShowValidationError("Name is required, must be 1 to 30 characters long and must not contain the | character.", animalName);
                return false;
            }
            //Validation for animal species. Required, 1-30 characters, no pipe character
            if (string.IsNullOrEmpty(AnimalSpecies) || AnimalSpecies.Length > 30 || AnimalSpecies.Contains("|"))
            {
                ShowValidationError("Species is required, must be 1 to 30 characters long and must not contain the | character.", animalSpecies);
                return false;
            }
            //NumericUpDown keeps a typed decimal like 3.5 in Value even though it displays 4, so decimals are rejected here
            if (animalAge.Value != Math.Floor(animalAge.Value))
            {
                ShowValidationError("Age must be a whole number between 0 and 100.", animalAge);
                return false;
            }
            if (RecoveryScoreRange.Value != Math.Floor(RecoveryScoreRange.Value))
            {
                ShowValidationError("Recovery Score must be a whole number between 0 and 100.", RecoveryScoreRange);
                return false;
            }

            //uniqueness check only applies to Add
            if (checkIdIsUnique)
            {
                try
                {
                    if (DataHandler.FindAnimalById(AnimalID) != null)
                    {
                        MessageBox.Show($"An animal ID {AnimalID} already exisits.", "ID Already exisits", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        animalId.Focus();
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error checking existing records {ex.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }

            //the Animal constructor calls ClassificationRule(), so status and housing are always recalculated here
            validAnimal = new Animal(AnimalID, AnimalName, AnimalSpecies, (int)animalAge.Value, (int)RecoveryScoreRange.Value);
            return true;
        }

        //shows one validation message that names the field and puts the cursor back in that field
        private void ShowValidationError(string message, Control field)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            field.Focus();
        }

        //validates the inputs and appends one new animal to animals.txt
        private void AddAnimalBtn_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs(true, out Animal ani)) return;

            try
            {
                DataHandler.AppendAnimal(ani);

                MessageBox.Show($"Animal {ani.AnimalID} successfull added!\n\nStatus: {ani.Status}\nHousing Unit: {ani.HousingUnit}",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                AnimalsChanged?.Invoke(this, EventArgs.Empty);
                EnterAddMode();
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

        //loads a record by typed Animal ID (exact match, not case-sensitive)
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchId = txtSearchId.Text.Trim();

            if (string.IsNullOrEmpty(searchId))
            {
                MessageBox.Show("Please type an Animal ID to search for.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSearchId.Focus();
                return;
            }

            LoadRecordById(searchId);
        }

        //finds a record in animals.txt and loads it into the input boxes; if not found, the form is left unchanged
        private void LoadRecordById(string searchId)
        {
            try
            {
                Animal found = DataHandler.FindAnimalById(searchId);

                if (found == null)
                {
                    MessageBox.Show($"No animal with ID {searchId.ToUpper()} was found.", "Animal Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                EnterEditMode(found);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Access denied while trying to read animals.txt. Check file permissions.", "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"The file animals.txt could not be read. It may be open in another program.\n\n{ex.Message}", "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //validates the edited record and rewrites animals.txt with only that record changed
        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            //guards against the button being clicked when nothing is loaded
            if (!isEditMode)
            {
                MessageBox.Show("Load an animal first by clicking a row in the grid or searching by ID.", "Nothing Loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!ValidateInputs(false, out Animal updated)) return;

            try
            {
                if (!DataHandler.UpdateAnimal(updated))
                {
                    //the record was removed from the file after it was loaded
                    MessageBox.Show($"Animal {updated.AnimalID} no longer exists in animals.txt. Nothing was saved.", "Animal Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    EnterAddMode();
                    AnimalsChanged?.Invoke(this, EventArgs.Empty);
                    return;
                }

                MessageBox.Show($"Animal {updated.AnimalID} updated.\n\nStatus: {updated.Status}\nHousing Unit: {updated.HousingUnit}",
                    "Changes Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

                AnimalsChanged?.Invoke(this, EventArgs.Empty);
                EnterAddMode();
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Permission denied: Cannot write to animals.txt. The file may be read-only.", "Access Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (IOException)
            {
                MessageBox.Show("The file animals.txt is currently in use by another program. Please close it and try again.", "File In Use Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //leaves edit mode and returns the form to Add mode with empty boxes
        private void btnClear_Click(object sender, EventArgs e)
        {
            EnterAddMode();
        }

        //puts a loaded record into the input boxes and locks the form into edit mode
        private void EnterEditMode(Animal animal)
        {
            isEditMode = true;

            animalId.Text = animal.AnimalID;
            animalName.Text = animal.Name;
            animalSpecies.Text = animal.Species;
            animalAge.Value = animal.Age;
            RecoveryScoreRange.Value = animal.RecoveryScore;

            animalId.ReadOnly = true;
            AddAnimalBtn.Enabled = false;
            btnSaveChanges.Enabled = true;
            lblMode.Text = $"Mode: Editing {animal.AnimalID}";
        }

        //clears the input boxes and switches back to Add mode
        private void EnterAddMode()
        {
            isEditMode = false;

            animalId.Clear();
            animalName.Clear();
            animalSpecies.Clear();
            txtSearchId.Clear();
            RecoveryScoreRange.Value = 0;
            animalAge.Value = 0;

            animalId.ReadOnly = false;
            AddAnimalBtn.Enabled = true;
            btnSaveChanges.Enabled = false;
            lblMode.Text = "Mode: Add new animal";
            animalId.Focus();
        }
    }
}
