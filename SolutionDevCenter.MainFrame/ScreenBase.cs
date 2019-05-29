using Open3270;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SolutionDevCenter.MainFrame
{
    public abstract class ScreenBase
    {
       // protected static Logger _logger = LogManager.GetCurrentClassLogger();

        public delegate void OnScreenChangedDelegate(string screen);

        public event OnScreenChangedDelegate OnScreenChanged;

        private readonly string[] _messagesAlreadySigOn = {"Your Accessor ID is Already in Use",
                                                           "You are already signed on at another terminal" };

        protected internal virtual Field ScreenName { get; }

        protected internal Emulator Emulator;

        protected TnKey KeyForward { get; set; }
        protected TnKey KeyPreview { get; set; }
        protected TnKey KeyNavigate { get; set; }

        private IEnumerable<PropertyInfo> Properties
        {
            get
            {
                return GetType()
                    .GetProperties()
                    .ToList()
                    .Where(propInfo => propInfo.PropertyType == typeof(Field));
            }
        }

        private IEnumerable<PropertyInfo> PropertiesReadOnly
        {
            get
            {
                return GetType()
                    .GetProperties()
                    .ToList()
                    .Where(propInfo => propInfo.PropertyType == typeof(ReadOnlyField));
            }
        }

        protected ScreenBase(string screenName)
        {
            Emulator = SolutionDevCenter.MainFrame.Emulator.GetInstance();
            ScreenName = new Field();
            InitializeFields();

            SetKeyForward(TnKey.F6);
            SetKeyPreview(TnKey.F5);
            SetKeyNavigate(TnKey.Enter);
            ConfigureScreen(screenName);
            Configure();
        }

        public abstract void Configure();
        protected string Screen => Emulator.GetScreen();

        #region Configuration Screen
        protected virtual void SetKeyForward(TnKey key)
        {
            KeyForward = key;
        }

        protected virtual void SetKeyPreview(TnKey key)
        {
            KeyPreview = key;
        }

        protected virtual void SetKeyNavigate(TnKey key)
        {
            KeyNavigate = key;
        }

        protected virtual void ConfigureScreen(string screenName)
        {
            ScreenName.Position = new Position(0, 8);
            ScreenName.Set(screenName);
        }

        #endregion

        #region Navigation Screen 
        public virtual ScreenBase Go()
        {
            SetFieldValue(ScreenName.Position, ScreenName.Value.ToString());
            Navigate();
            return this;
        }

        public virtual void Navigate()
        {
            Navigate(true);
        }

        public virtual void Navigate(bool clearFieldsValue)
        {
            Emulator.SendKey(KeyNavigate);

            if (clearFieldsValue)
                ClearFieldsValue();

            Debug();
        }

        public virtual void Forward()
        {
            Emulator.SendKey(KeyForward);
            Debug();
        }

        public virtual void Preview()
        {
            Emulator.SendKey(KeyPreview);
            Debug();
        }
        #endregion

        public virtual void Apply()
        {
            var properties = Properties;

            foreach (var property in properties)
            {
                var value = property?.GetValue(this) as Field;

                if (value != null && !string.IsNullOrWhiteSpace(value?.Get()))
                    SetFieldValue(value.Position, value.Get());
            }
            Debug();
        }

        public void WaitFor(string text, Position position)
        {
            Emulator.WaitForText(text, position);
        }

        public void WaitForScreenToChange()
        {
            Emulator.WaitForScreenToChange();
        }

        private void InitializeFields()
        {
            Properties
                 .Where(propInfo => propInfo.PropertyType == typeof(Field) &&
                                        propInfo.CanWrite &&
                                        propInfo.GetSetMethod(/*nonPublic*/ true).IsPublic)
               .ToList()
               .ForEach(x => x.SetValue(this, new Field()));

            PropertiesReadOnly
                .Where(propInfo => propInfo.PropertyType == typeof(ReadOnlyField) &&
                                       propInfo.CanWrite &&
                                       propInfo.GetSetMethod(/*nonPublic*/ true).IsPublic)
              .ToList()
              .ForEach(x => x.SetValue(this, new ReadOnlyField()));
        }

        private void ClearFieldsValue()
        {
            Properties
                 .Where(propInfo => propInfo.PropertyType == typeof(Field) &&
                                        propInfo.CanWrite &&
                                        propInfo.GetSetMethod(/*nonPublic*/ true).IsPublic)
               .ToList()
                .ForEach(x =>
                {
                    var _field = x.GetValue(this) as Field;
                    _field.Value = null;
                });
        }

        public virtual void Bind()
        {
            var screen = Emulator.GetScreen();
            var stringSeparators = new[] { "\n" };
            var dataGrid = screen.Split(stringSeparators, StringSplitOptions.None);

            Properties
                .Where(propInfo => propInfo.PropertyType == typeof(Field) &&
                                   propInfo.CanWrite &&
                                   propInfo.GetSetMethod( /*nonPublic*/ true).IsPublic)
                .ToList()
                .ForEach(x =>
                {
                    var _field = x.GetValue(this) as Field;

                    if (_field?.Position == null) return;

                    if (dataGrid.Length < _field.Position.Row)
                        throw new ArgumentOutOfRangeException("Row number out range from data");

                    var row = dataGrid[_field.Position.Row];

                    if (row.Length < _field.Position.Column)
                        throw new ArgumentOutOfRangeException("Column index outside range.");

                    if (row.Length < (_field.Position.Column + _field.Length))
                        throw new ArgumentOutOfRangeException("Expected content is greater than available.");

                    var data = row.Substring(_field.Position.Column, _field.Length);

                    _field.Value = data;
                });
        }

        public void FillReadOnlyField(ReadOnlyField field)
        {
            field.Value = Emulator.GetText(field.Position, field.Length);
        }

        protected void SetFieldValue(Position position, string value)
        {
            if (position == null) return;
            Emulator.SetText(value, position.Row, position.Column);
        }

        protected void VerifyIfSignedAtAnotherTerminal()
        {
            if (_messagesAlreadySigOn.Any(item => Screen.Contains(item)))
                throw new Exception("Usuário já está logado em outro terminal");
        }

        public void Debug()
        {
            //Console.Clear();
            Console.Write(Emulator.GetScreen());
            var screen = Emulator.GetScreen();
            OnScreenChanged?.Invoke(screen);
            System.Diagnostics.Debug.Flush();
            System.Diagnostics.Debug.Print(screen);
           // _logger.Log(LogLevel.Trace, screen);
        }
    }
}
