using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Bedrock.Common
{
    public interface IVariableList : IEnumerable<string>
    {
        void Set(string key, object value);
        object Get(string key);
        void Unset(string key);
    }
    public class VariableList<T> : IVariableList
    {
        protected Dictionary<string, T> Values { get; private set; }

        public VariableList()
        {
            Values = new Dictionary<string, T>();
        }

        public virtual void Set(string key, object value)
        {
            if (value != null && !(value is T))
                throw new ArgumentException("Invalid type of argument");

            if (!Values.ContainsKey(key))
                Values.Add(key, (T)value);
            else
                Values[key] = (T)value;
        }
        public virtual object Get(string key)
        {
            if (!Values.ContainsKey(key))
                return default(T);
            else
                return Values[key];
        }
        public virtual void Unset(string key)
        {
            if (Values.ContainsKey(key))
                Values.Remove(key);
        }

        public virtual IEnumerator<string> GetEnumerator()
        {
            return Values.Keys.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public enum AutoCompletionType
    {
        None,
        Class,
        Constant,
        Enum,
        EnumItem,
        Event,
        Field,
        Keyword,
        Method,
        Module,
        Namespace,
        Property,
        Snippet,
        Structure
    }
    public class AutoCompletion : MarshalByRefObject
    {
        public string Text { get; set; }
        public string Result { get; set; }
        public string Description { get; set; }
        public AutoCompletionType Type { get; set; }

        public AutoCompletion(string text, string result)
        {
            Text = text;
            Result = result;
        }
        public AutoCompletion(string text, string result, string description)
        {
            Text = text;
            Result = result;

            Description = description;
        }
        public AutoCompletion(string text, string result, AutoCompletionType type)
        {
            Text = text;
            Result = result;
            Type = type;
        }
        public AutoCompletion(string text, string result, AutoCompletionType type, string description)
        {
            Text = text;
            Result = result;
            Type = type;
            Description = description;
        }
    }

    public enum TextBlockType
    {
        None,
        Warning,
        Error,
        Comment,
        String,
        Keyword
    }
    public class TextBlock : MarshalByRefObject
    {
        public string Text { get; private set; }
        public TextBlockType Type { get; private set; }

        public TextBlock(string text) : this(text, TextBlockType.None) { }
        public TextBlock(string text, TextBlockType type)
        {
            Text = text;
            Type = type;
        }
    }

    public delegate void OutputCallback(TextBlock[] text);

    public abstract class ShellSession : MarshalByRefObject
    {
        public abstract IVariableList Variables { get; }

        public abstract bool Run(string command);
        public virtual AutoCompletion[] GetCompletions(string line)
        {
            return Array.Empty<AutoCompletion>();
        }
        public virtual TextBlock[] Colorize(string line)
        {
            return new TextBlock[] { new TextBlock(line) };
        }

        public event OutputCallback Output;
        protected void OnOutput(string text)
        {
            if (text != null)
                OnOutput(new TextBlock[] { new TextBlock(text) });
        }
        protected void OnOutput(string text, TextBlockType type)
        {
            if (text != null)
                OnOutput(new TextBlock[] { new TextBlock(text, type) });
        }
        protected void OnOutput(IEnumerable<TextBlock> text)
        {
            Output?.Invoke(text as TextBlock[] ?? text.ToArray());
        }

        public event Action Clear;
        protected void OnClear()
        {
            Clear?.Invoke();
        }
    }
}