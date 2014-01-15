using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace NAntEnv
{
    [TaskName("load-environment")]
    public class LoadEnvTask : Task
    {
        private EnvironmentVariableTarget _environmentVariableTarget = EnvironmentVariableTarget.Process;
        private bool _toLower = true;
        private static string _delimeters = "_";

        [TaskAttribute("delimeters")]
        public string Delimeters
        {
            get { return _delimeters; }
            set { _delimeters = value; }
        }

        [TaskAttribute("target")]
        public string Target
        {
            get { return _environmentVariableTarget.ToString(); }
            set
            {
                _environmentVariableTarget =
                    (EnvironmentVariableTarget) Enum.Parse(typeof (EnvironmentVariableTarget), value, true);
            }
        }

        [TaskAttribute("tolower")]
        public bool ToLower
        {
            get { return _toLower; }
            set { _toLower = value; }
        }

        private void LoadEnv(PropertyDictionary properties)
        {
            Project.Indent();
            Project.Log(Level.Info, "Loading environment variables into NAnt properties...");
            foreach (Property prop in EnumerateEnv().Where(Property.ValidatePropertyName))
            {
                Project.Log(Level.Verbose, "{0} = {1}", prop.Name, prop.Value);
                Project.Properties[prop.Name] = prop.Value;
            }
            Project.Log(Level.Verbose, Environment.NewLine);
            Project.Unindent();
        }

        private IEnumerable<Property> EnumerateEnv()
        {
            return from DictionaryEntry envVar in
                       Environment.GetEnvironmentVariables(_environmentVariableTarget)
                   where !string.IsNullOrWhiteSpace(envVar.Key.ToString())
                   select new Property
                       {
                           Name = ParseName(envVar.Key.ToString()),
                           Value = envVar.Value.ToString()
                       };
        }

        private string ParseName(string s)
        {
            s = ToLower ? s.ToLower().Trim() : s.Trim();
            System.Diagnostics.Debug.WriteLine(s);
            if (!s.Contains(_delimeters))
                return s;
            var sections = s.Split(_delimeters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            return string.Join(".", sections);
        }

        protected override sealed void ExecuteTask()
        {
            LoadEnv(Project.Properties);
        }
    }

    class Property
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public static bool ValidatePropertyName(Property prop)
        {
            return Regex.IsMatch(prop.Name, "^[_A-Za-z0-9][_A-Za-z0-9\\-.]*$");
        }
    }
}
