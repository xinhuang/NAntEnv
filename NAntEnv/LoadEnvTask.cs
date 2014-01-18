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

        public LoadEnvTask()
        {
            ToLower = true;
            Delimeters = "_";
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
        public bool ToLower { get; set; }
        [TaskAttribute("prefix")]
        public string Prefix { get; set; }
        [TaskAttribute("delimeters")]
        public string Delimeters { get; set; }
        [TaskAttribute("overwrite")]
        public bool Overwrite { get; set; }

        private void LoadEnv(PropertyDictionary properties)
        {
            Project.Indent();
            Project.Log(Level.Info, "Loading environment variables into NAnt properties...");
            foreach (Property prop in EnumerateEnv().Where(prop => IsValidateNAntPropertyName(prop.Name)))
            {
                if (properties.IsReadOnlyProperty(prop.Name))
                    continue;
                if (properties.Contains(prop.Name) && !Overwrite)
                    continue;
                if (Verbose)
                    Project.Log(Level.Info, "{0} = {1}", prop.Name, prop.Value);
                Project.Properties[prop.Name] = prop.Value;
            }
            if (Verbose)
                Project.Log(Level.Verbose, Environment.NewLine);
            Project.Unindent();
        }

        private IEnumerable<Property> EnumerateEnv()
        {
            return from DictionaryEntry envVar in
                       Environment.GetEnvironmentVariables(_environmentVariableTarget)
                   where !string.IsNullOrWhiteSpace(envVar.Key.ToString())
                   orderby envVar.Key.ToString()
                   select new Property
                       {
                           Name = ParseName(Prefix + envVar.Key.ToString()),
                           Value = envVar.Value.ToString()
                       };
        }

        private string ParseName(string s)
        {
            s = ToLower ? s.ToLower().Trim() : s.Trim();
            System.Diagnostics.Debug.WriteLine(s);
            if (!s.Contains(Delimeters))
                return s;
            var sections = s.Split(Delimeters.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            return string.Join(".", sections);
        }

        protected override sealed void ExecuteTask()
        {
            LoadEnv(Project.Properties);
        }
        private static bool IsValidateNAntPropertyName(string s)
        {
            return Regex.IsMatch(s, "^[_A-Za-z0-9][_A-Za-z0-9\\-.]*$");
        }
    }

    class Property
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
