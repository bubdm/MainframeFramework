using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDevCenter.MainFrame
{

    public class ReadOnlyField
    {
        protected internal int Length { get; internal set; }
        public object Value { get; internal set; }
        protected internal Position Position { get; internal set; }


    }

    public class Field
    {
        protected internal int Length { get; internal set; }
        protected internal string Regex { get; internal set; }
        protected internal object Value { get; internal set; }
        protected internal Position Position { get; internal set; }
        protected internal string Label { get; set; }


        public void Set(object value) => Value = value;
        public string Get() => Value?.ToString() ?? string.Empty;

        public void RegexBind(string content)
        {
            var matches = System.Text.RegularExpressions.Regex.Match(content, Regex);
            Value = matches.Groups[1].Value;
        }

    }

}
