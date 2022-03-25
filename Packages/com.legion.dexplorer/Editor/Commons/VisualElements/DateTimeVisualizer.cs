namespace dExplorer.Editor.Commons
{
	using System;
	using System.Collections.Generic;
	using UnityEditor.UIElements;
	using UnityEngine.UIElements;

    /// <summary>
    /// DateTime Visual Element.
    /// </summary>
    public class DateTimeVisualizer : VisualElement
    {
		#region Enums
        /// <summary>
        /// DateTime display format.
        /// </summary>
		public enum DateTimeFormat
        {
            DATE_SHORT = 0,
            DATE_LONG = 1,
            ADATE_SHORT = 2,
            ADATE_LONG = 3,
            EDATE_SHORT = 4,
            EDATE_LONG = 5,
            SDATE_SHORT = 6,
            SDATE_LONG = 7,
            DATETIME_SHORT = 8,
            DATETIME_LONG = 9,
            YMDHMS_SHORT = 10,
            YMDHMS_LONG = 11
        }
		#endregion Enums

		#region Classes
		public new class UxmlFactory : UxmlFactory<DateTimeVisualizer, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            #region Fields
            private UxmlStringAttributeDescription m_Title = new UxmlStringAttributeDescription { name = "Title", defaultValue = String.Empty };

            private UxmlIntAttributeDescription m_Year = new UxmlIntAttributeDescription { name = "Year", defaultValue = 2000 };
            private UxmlIntAttributeDescription m_Month = new UxmlIntAttributeDescription { name = "Month", defaultValue = 01 };
            private UxmlIntAttributeDescription m_Day = new UxmlIntAttributeDescription { name = "Day", defaultValue = 01 };
            private UxmlIntAttributeDescription m_Hour = new UxmlIntAttributeDescription { name = "Hour", defaultValue = 00 };
            private UxmlIntAttributeDescription m_Minute = new UxmlIntAttributeDescription { name = "Minute", defaultValue = 00 };
            private UxmlIntAttributeDescription m_Second = new UxmlIntAttributeDescription { name = "Second", defaultValue = 00 };
            private UxmlIntAttributeDescription m_Millisecond = new UxmlIntAttributeDescription { name = "Millisecond", defaultValue = 00 };

            private UxmlEnumAttributeDescription<DateTimeKind> m_Zone = new UxmlEnumAttributeDescription<DateTimeKind> { name = "Zone", defaultValue = DateTimeKind.Unspecified };
            private UxmlEnumAttributeDescription<DateTimeFormat> m_Format = new UxmlEnumAttributeDescription<DateTimeFormat> { name = "Format", defaultValue = DateTimeFormat.DATETIME_SHORT };
            #endregion Fields

            #region Accessors
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            #endregion Accessors

            #region Methods
            public override void Init(VisualElement visualElement, IUxmlAttributes bag, CreationContext creationContext)
            {
                base.Init(visualElement, bag, creationContext);
                DateTimeVisualizer dateTimeVisualizer = visualElement as DateTimeVisualizer;

                dateTimeVisualizer.Clear();

                dateTimeVisualizer._title = m_Title.GetValueFromBag(bag, creationContext);
                dateTimeVisualizer._year = m_Year.GetValueFromBag(bag, creationContext);
                dateTimeVisualizer._month = m_Month.GetValueFromBag(bag, creationContext);
                dateTimeVisualizer._day = m_Day.GetValueFromBag(bag, creationContext);
                dateTimeVisualizer._hour = m_Hour.GetValueFromBag(bag, creationContext);
                dateTimeVisualizer._minute = m_Minute.GetValueFromBag(bag, creationContext);
                dateTimeVisualizer._second = m_Second.GetValueFromBag(bag, creationContext);
                dateTimeVisualizer._millisecond = m_Millisecond.GetValueFromBag(bag, creationContext);
                dateTimeVisualizer._zone = m_Zone.GetValueFromBag(bag, creationContext);
                dateTimeVisualizer._format = m_Format.GetValueFromBag(bag, creationContext);

                dateTimeVisualizer._header = new TextField();

                dateTimeVisualizer._configuration = new Foldout();

                dateTimeVisualizer._zoneEnumField = new EnumField("Zone");
                dateTimeVisualizer._zoneEnumField.Init(m_Zone.defaultValue);
                dateTimeVisualizer._zoneEnumField.RegisterValueChangedCallback(dateTimeVisualizer.OnDateTimeChanged);

                dateTimeVisualizer._formatEnumField = new EnumField("Format");
                dateTimeVisualizer._formatEnumField.Init(m_Format.defaultValue);
                dateTimeVisualizer._formatEnumField.RegisterValueChangedCallback(dateTimeVisualizer.OnDateTimeChanged);

                dateTimeVisualizer.Add(dateTimeVisualizer._header);
                dateTimeVisualizer._configuration.Add(dateTimeVisualizer._zoneEnumField);
                dateTimeVisualizer._configuration.Add(dateTimeVisualizer._formatEnumField);
                dateTimeVisualizer.Add(dateTimeVisualizer._configuration);

                dateTimeVisualizer.Update();
            }
            #endregion Methods
        }
        #endregion Classes

        #region Fields
        private TextField _header;
        private Foldout _configuration;
        private EnumField _zoneEnumField;
        private EnumField _formatEnumField;

        private string _title;
        private int _year;
        private int _month;
        private int _day;
        private int _hour;
        private int _minute;
        private int _second;
        private int _millisecond;
        private DateTimeKind _zone;
        private DateTimeFormat _format;
        #endregion Fields

        #region Accessors
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                Update();
            }
        }

        public int Year 
        {
            get
            {
                return _year;
            }
            set
            {
                _year = value;
                Update();
            }
        }

        public int Month 
        {
            get
            {
                return _month;
            }
            set
            {
                _month = value;
                Update();
            }
        }

        public int Day 
        {
            get
            {
                return _day;
            }
            set
            {
                _day = value;
                Update();
            }
        }

        public int Hour 
        {
            get
            {
                return _hour;
            }
            set
            {
                _hour = value;
                Update();
            }
        }

        public int Minute
        {
            get
            {
                return _minute;
            }
            set
            {
                _minute = value;
                Update();
            }
        }

        public int Second 
        {
            get
            {
                return _second;
            }
            set
            {
                _second = value;
                Update();
            }
        }

        public int Millisecond 
        {
            get
            {
                return _millisecond;
            }
            set
            {
                _millisecond = value;
                Update();
            }
        }

        public DateTimeKind Zone 
        {
            get
            {
                return _zone;
            }
            set
            {
                _zone = value;
                Update();
            }
        }

        public DateTimeFormat Format 
        {
            get
            {
                return _format;
            }
            set
            {
                _format = value;
                Update();
            }
        }
        #endregion Accessors

        #region Static Methods
		/// <summary>
		/// Convert DateTime into string value.
		/// </summary>
		/// <param name="format">DateTime display formal</param>
		/// <param name="dateTime">Selected DateTime</param>
		/// <returns></returns>
        public static string ToString(DateTimeFormat format, DateTime dateTime)
        {
			string result = format switch
			{
				DateTimeFormat.DATE_SHORT => dateTime.ToString("dd-MMM-yy"),
				DateTimeFormat.DATE_LONG => dateTime.ToString("dd-MMM-yyyy"),
				DateTimeFormat.ADATE_SHORT => dateTime.ToString("MM/dd/yy"),
				DateTimeFormat.ADATE_LONG => dateTime.ToString("MM/dd/yyyy"),
				DateTimeFormat.EDATE_SHORT => dateTime.ToString("dd.MM.yy"),
				DateTimeFormat.EDATE_LONG => dateTime.ToString("dd.MM.yyyy"),
				DateTimeFormat.SDATE_SHORT => dateTime.ToString("yy/MM/dd"),
				DateTimeFormat.SDATE_LONG => dateTime.ToString("yyyy/MM/dd"),
				DateTimeFormat.DATETIME_SHORT => dateTime.ToString("dd-MMM-yyyy HH:mm"),
				DateTimeFormat.DATETIME_LONG => dateTime.ToString("dd-MMM-yyyy HH:mm:ss"),
				DateTimeFormat.YMDHMS_SHORT => dateTime.ToString("yyyy-MM-dd HH:mm"),
				DateTimeFormat.YMDHMS_LONG => dateTime.ToString("yyyy-MM-dd HH:mm:ss"),
				_ => string.Empty,
			};
			return result;
        }
        #endregion Static Methods

        #region Methods
        private void OnDateTimeChanged(ChangeEvent<Enum> _)
        {
            _zone = (DateTimeKind)_zoneEnumField.value;
            _format = (DateTimeFormat)_formatEnumField.value;

            Update();
        }

        private void Update()
		{
            DateTime dateTime = new DateTime(_year, _month, _day, _hour, _minute, _second, _millisecond, DateTimeKind.Utc);

            if (_zone == DateTimeKind.Local)
            {
                dateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, TimeZoneInfo.Local);
            }
            else if (_zone == DateTimeKind.Unspecified)
            {
                _zone = DateTimeKind.Utc;
            }

            _header.label = _title;
            _header.value = DateTimeVisualizer.ToString(_format, dateTime);
            _header.focusable = false;
            _header.isReadOnly = true;

            string timeZoneId = dateTime.Kind == DateTimeKind.Utc ? TimeZoneInfo.Utc.Id : TimeZoneInfo.Local.Id;
            _configuration.text = string.Format("Time Zone : {0}", timeZoneId);

            _zoneEnumField.value = _zone;
            _formatEnumField.value = _format;
        }

		/// <summary>
		/// Set DateTime value.
		/// </summary>
		/// <param name="dateTime">New DateTime value</param>
        public void SetDateTime(DateTime dateTime)
		{
            DateTimeKind initialZone = dateTime.Kind;

            if (initialZone == DateTimeKind.Local)
			{
                dateTime = TimeZoneInfo.ConvertTimeToUtc(dateTime);
			}

            _year = dateTime.Year;
            _month = dateTime.Month;
            _day = dateTime.Day;
            _hour = dateTime.Hour;
            _minute = dateTime.Minute;
            _second = dateTime.Second;
            _millisecond = dateTime.Millisecond;
            _zone = initialZone;

            Update();
		}
        #endregion Methods
    }
}
