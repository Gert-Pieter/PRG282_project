using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Baobab_Ridge_Wildlife_Rehabilitation_Records_System
{
    internal class Animal
    {
        public string AnimalID { get; set; }
        public string Name { get; set; }
        public string Species { get; set; }
        public int Age { get; set; }
        public int RecoveryScore { get; set; }
        public string Status { get; set; }
        public string HousingUnit { get; set; }

        //constructor 
        public Animal(string animalID, string name, string species, int age, int recoveryScore)
        {
            AnimalID = animalID;
            Name = name;
            Species = species;
            Age = age;
            RecoveryScore = recoveryScore;

            // removed as the method automatically does it

            ClassificationRule();
        }
       



        //Method to determine the classification (Status and housing unit)
        //No validation for out of range number is done as the form makes use of NumericUpDown which ensures data validation on its own
        public void ClassificationRule()
        {
            // The status and Housing unit for recovery score between 0 and 19
            if (RecoveryScore >= 0 && RecoveryScore < 20)
            {
                Status = "Critical";
                HousingUnit = "Intensive Care Unit";
            }
            // The status and Housing unit for recovery score between 20 and 39
            else if (RecoveryScore > 19 && RecoveryScore < 40)
            {
                Status = "Serious";
                HousingUnit = "High-Dependency Ward";
            }
            // The status and Housing unit for recovery score between 40 and 59
            else if (RecoveryScore > 39 && RecoveryScore < 60)
            {
                Status = "Stable";
                HousingUnit = "Recovery Ward";
            }
            // The status and Housing unit for revocery method between 60 and 79
            else if (RecoveryScore > 59 && RecoveryScore < 80)
            {
                Status = "Recovering";
                HousingUnit = "Outdoor Enclosure";
            }
            //The Status and Housing Unit for recovery method between 80 and 100 
            else 
            {
                Status = "Release-Ready";
                HousingUnit = "Pre-Release Camp";
            }
        }


    }
}