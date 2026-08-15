using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Linear_Programming_381.Models;

namespace Linear_Programming_381.File_Handling
{
    public class FileReader
    {
        public LPModel ReadModel(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(
                    "The input file could not be found.",
                    filePath);
            }
            string[] lines = File.ReadAllLines(filePath);
            List<string> validLines = new List<string>();
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    validLines.Add(line.Trim());
                }
            }
            if (validLines.Count < 3)
            {
                throw new FormatException(
                    "The input file must contain an objective, " +
                    "at least one constraint and sign restrictions.");
            }
            LPModel model = new LPModel();
            ReadObjective(validLines[0], model);
            int variableCount = model.Variables.Count;
            for (int i = 1; i < validLines.Count - 1; i++)
            {
                ReadConstraint(
                    validLines[i],
                    variableCount,
                    model);
            }
            ReadRestrictions(
                validLines[validLines.Count - 1],
                model);
            return model;
        }
        private void ReadObjective(
    string line,
    LPModel model)
        {
            string[] tokens = line.Split(
                new[] { ' ', '\t' },
                StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length < 2)
            {
                throw new FormatException(
                    "Invalid objective function.");
            }
            string objectiveType =
                tokens[0].ToLower();
            if (objectiveType != "max" &&
                objectiveType != "min")
            {
                throw new FormatException(
                    "The first word must be 'max' or 'min'.");
            }
            model.ObjectiveType = objectiveType;
            for (int i = 1; i < tokens.Length; i++)
            {
                string token = tokens[i];
                if (token.Length < 2)
                {
                    throw new FormatException(
                        "Invalid objective coefficient: " +
                        token);
                }
                char objectiveSign =
                    token[0];
                if (objectiveSign != '+' &&
                    objectiveSign != '-')
                {
                    throw new FormatException(
                        "Objective coefficient must have " +
                        "a + or - sign: " + token);
                }
                double coefficient =
                    ParseSignedNumber(
                        token,
                        "Objective coefficient");
                Variable variable =
                    new Variable(
                        "x" + (model.Variables.Count + 1),
                        Math.Abs(coefficient),
                        objectiveSign,
                        "+");
                model.Variables.Add(variable);
            }
        }
        private void ReadConstraint(
    string line,
    int variableCount,
    LPModel model)
        {
            string[] tokens = line.Split(
                new[] { ' ', '\t' },
                StringSplitOptions.RemoveEmptyEntries);

            if (tokens.Length != variableCount + 1)
            {
                throw new FormatException(
                    "A constraint contains an incorrect " +
                    "number of values.");
            }
            List<double> coefficients =
                new List<double>();
            List<char> signs =
                new List<char>();
            for (int i = 0;
                 i < variableCount;
                 i++)
            {
                string token = tokens[i];
                if (token.Length < 2)
                {
                    throw new FormatException(
                        "Invalid technological coefficient: " +
                        token);
                }
                char sign = token[0];
                double coefficient =
                    ParseSignedNumber(
                        token,
                        "Technological coefficient");
                coefficients.Add(
                    Math.Abs(coefficient));

                signs.Add(sign);
            }
            string relationAndRhs =
                tokens[variableCount];
            string relation;
            double rhs;
            if (relationAndRhs.StartsWith("<="))
            {
                relation = "<=";
                rhs = ParseRhs(
                    relationAndRhs.Substring(2));
            }
            else if (relationAndRhs.StartsWith(">="))
            {
                relation = ">=";
                rhs = ParseRhs(
                    relationAndRhs.Substring(2));
            }
            else if (relationAndRhs.StartsWith("="))
            {
                relation = "=";
                rhs = ParseRhs(
                    relationAndRhs.Substring(1));
            }
            else
            {
                throw new FormatException(
                    "Constraint must contain <=, >= or =.");
            }
            Constraint constraint =
                new Constraint(
                    coefficients,
                    signs,
                    relation,
                    rhs);
            model.Constraints.Add(
                constraint);
        }
        private void ReadRestrictions(
            string line,
            LPModel model)
        {
            string[] tokens = line.Split(
                new[] { ' ', '\t' },
                StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length != model.Variables.Count)
            {
                throw new FormatException(
                    "The number of sign restrictions must " +
                    "match the number of variables.");
            }
            for (int i = 0; i < tokens.Length; i++)
            {
                string restriction =
                    tokens[i].ToLower();
                switch (restriction)
                {
                    case "+":
                        model.Variables[i].Restriction = "+";
                        break;
                    case "-":
                        model.Variables[i].Restriction = "-";
                        break;
                    case "urs":
                        model.Variables[i].Restriction = "urs";
                        break;
                    case "int":
                        model.Variables[i].Restriction = "int";
                        break;
                    case "bin":
                        model.Variables[i].Restriction = "bin";
                        break;
                    default:
                        throw new FormatException(
                            "Invalid sign restriction: " +
                            restriction);
                }
            }
        }
        private double ParseSignedNumber(
            string value,
            string description)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new FormatException(
                    description + " cannot be empty.");
            }
            if (value[0] != '+' &&
                value[0] != '-')
            {
                throw new FormatException(
                    description +
                    " must have a + or - sign: " +
                    value);
            }
            string number =
                value.Substring(1);
            if (!double.TryParse(
                    number,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double result))
            {
                throw new FormatException(
                    "Invalid " +
                    description.ToLower() +
                    ": " +
                    value);
            }
            if (value[0] == '-')
            {
                result = -result;
            }
            return result;
        }
        private double ParseRhs(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new FormatException(
                    "Right-hand-side value cannot be empty.");
            }
            if (!double.TryParse(
                    value,
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out double result))
            {
                throw new FormatException(
                    "Invalid right-hand-side value: " +
                    value);
            }
            return result;
        }
    }
}