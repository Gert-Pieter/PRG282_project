using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Baobab_Ridge_Wildlife_Rehabilitation_Records_System.DataLayer
{
    internal class DataHandler
    {
        //path to animals.txt, always in the same folder as the running .exe
        public static readonly string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "animals.txt");

        //turns an Animal into one pipe-separated line in the order the brief requires
        public static string ToFileLine(Animal animal)
        {
            return $"{animal.AnimalID}|{animal.Name}|{animal.Species}|{animal.Age}|{animal.RecoveryScore}|{animal.Status}|{animal.HousingUnit}";
        }

        //tries to turn one line from animals.txt into an Animal; returns null if the line is malformed
        public static Animal ParseLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;

            string[] parts = line.Split('|');
            if (parts.Length != 7) return null;

            string id = parts[0].Trim().ToUpper();
            if (!Regex.IsMatch(id, @"^WR-\d{4}$")) return null;

            if (!int.TryParse(parts[3].Trim(), out int age) || age < 0 || age > 100) return null;
            if (!int.TryParse(parts[4].Trim(), out int score) || score < 0 || score > 100) return null;

            //status and housing are recalculated by the Animal constructor, so the file's values are not trusted
            return new Animal(id, parts[1].Trim(), parts[2].Trim(), age, score);
        }

        //appends one animal as a new line at the end of animals.txt, leaving existing lines untouched
        //file exceptions are passed up so the form can show a friendly message
        public static void AppendAnimal(Animal animal)
        {
            string newLine = ToFileLine(animal) + Environment.NewLine;

            //if the file's last line has no line break (e.g. an assessor's file), add one first
            //so the new record is not glued onto the end of the previous record
            if (File.Exists(FilePath) && new FileInfo(FilePath).Length > 0)
            {
                string existing = File.ReadAllText(FilePath);
                if (!existing.EndsWith("\n"))
                {
                    newLine = Environment.NewLine + newLine;
                }
            }

            File.AppendAllText(FilePath, newLine, new UTF8Encoding(false));
        }

        //searches animals.txt for an exact, case-insensitive ID match; returns null if not found
        //file exceptions are passed up so the form can show a friendly message
        public static Animal FindAnimalById(string animalId)
        {
            if (!File.Exists(FilePath)) return null;

            foreach (string line in File.ReadAllLines(FilePath))
            {
                Animal animal = ParseLine(line);
                if (animal != null && animal.AnimalID.Equals(animalId.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return animal;
                }
            }
            return null;
        }

        //rewrites animals.txt with only the edited record replaced; returns false if the ID is no longer in the file
        //file exceptions are passed up so the form can show a friendly message
        public static bool UpdateAnimal(Animal updatedAnimal)
        {
            if (!File.Exists(FilePath)) return false;

            string[] lines = File.ReadAllLines(FilePath);
            List<string> newLines = new List<string>();
            bool replaced = false;

            foreach (string line in lines)
            {
                //drop blank lines, the file format does not allow them
                if (string.IsNullOrWhiteSpace(line)) continue;

                string lineId = line.Split('|')[0].Trim();

                //only the first matching line is replaced, so a record can never be written twice
                if (!replaced && lineId.Equals(updatedAnimal.AnimalID, StringComparison.OrdinalIgnoreCase))
                {
                    newLines.Add(ToFileLine(updatedAnimal));
                    replaced = true;
                }
                else
                {
                    //every other line is written back exactly as it was read
                    newLines.Add(line);
                }
            }

            if (!replaced) return false;

            //UTF8Encoding(false) writes UTF-8 without a byte order mark
            File.WriteAllLines(FilePath, newLines, new UTF8Encoding(false));
            return true;
        }
    }
}
