namespace dExplorer.Runtime.Mathematics
{
	using System;
	using System.Runtime.CompilerServices;
	using UnityEngine;

	/// <summary>
	/// Base class for differential equation report.
	/// </summary>
	public abstract class DEReport : ScriptableObject, ISerializationCallbackReceiver
	{
		#region Fields
		public string Name;
		public string ShortDescription;
		public string LongDescription;

		protected DateTime _creationDateTime;

		#region Serialization Fields
		[HideInInspector] [SerializeField] private int _serializedCreationYear = 0;
		[HideInInspector] [SerializeField] private int _serializedCreationMonth = 0;
		[HideInInspector] [SerializeField] private int _serializedCreationDay = 0;
		[HideInInspector] [SerializeField] private int _serializedCreationHour = 0;
		[HideInInspector] [SerializeField] private int _serializedCreationMinute = 0;
		[HideInInspector] [SerializeField] private int _serializedCreationSecond = 0;
		[HideInInspector] [SerializeField] private int _serializedCreationMillisecond = 0;
		[HideInInspector] [SerializeField] private DateTimeKind _serializedCreationDateTimeZone = DateTimeKind.Unspecified;
		#endregion Serialization Fields
		#endregion Fields

		#region Constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		public DEReport() : base()
		{
			Name = string.Empty;
			ShortDescription = string.Empty;
			LongDescription = string.Empty;

			_creationDateTime = DateTime.UtcNow;
		}
		#endregion Constructors

		#region Properties
		/// <summary>
		/// Creation date.
		/// </summary>
		public DateTime CreationDateTime { get { return _creationDateTime; } }
		#endregion Properties

		#region Methods
		/// <summary>
		/// Method called before Unity serializes the report.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void PreSerialize()
		{
			_serializedCreationYear = _creationDateTime.Year;
			_serializedCreationMonth = _creationDateTime.Month;
			_serializedCreationDay = _creationDateTime.Day;
			_serializedCreationHour = _creationDateTime.Hour;
			_serializedCreationMinute = _creationDateTime.Minute;
			_serializedCreationSecond = _creationDateTime.Second;
			_serializedCreationMillisecond = _creationDateTime.Millisecond;
			_serializedCreationDateTimeZone = _creationDateTime.Kind;
		}

		/// <summary>
		/// Method called after Unity deserializes the report.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void PostDeserialize()
		{
			_creationDateTime = new DateTime(_serializedCreationYear, _serializedCreationMonth, _serializedCreationDay, _serializedCreationHour,
				_serializedCreationMinute, _serializedCreationSecond, _serializedCreationMillisecond, _serializedCreationDateTimeZone);
		}

		/// <summary>
		/// Get name of the report.
		/// </summary>
		/// <returns>Name of the report</returns>
		public string GetName()
		{
			return Name;
		}

		/// <summary>
		/// Get short description of the report.
		/// </summary>
		/// <returns>Short description of the report</returns>
		public string GetShortDescription()
		{
			return ShortDescription;
		}

		/// <summary>
		/// Get long description of the report.
		/// </summary>
		/// <returns>Long description of the report</returns>
		public string GetLongDescription()
		{
			return LongDescription;
		}

		/// <summary>
		/// Get creation date of the report.
		/// </summary>
		/// <returns>Creation date of the report</returns>
		public DateTime GetCreationDate()
		{
			return _creationDateTime;
		}

		/// <summary>
		/// Callback called before Unity serializes the report.
		/// </summary>
		abstract public void OnBeforeSerialize();

		/// <summary>
		/// Callback called after Unity deserializes the report.
		/// </summary>
		abstract public void OnAfterDeserialize();
		#endregion Methods
	}
}
