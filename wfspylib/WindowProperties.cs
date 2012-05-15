using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace wfspy
{
	/// <summary>
	/// Summary description for WindowProperties.
	/// </summary>
	public class WindowProperties : ICustomTypeDescriptor
	{
		private Control targetControl;
		private PropertyDescriptorCollection properties;		
		private PropertyDescriptorCollection thisProperties;
		private PropertyDescriptorCollection controlProperties;

		public WindowProperties(Control targetControl)
		{
			this.targetControl = targetControl;
		}
		

		[Category("Type")]
		public string Name
		{
			get
			{
				return targetControl.Name;
			}
		}

		[Category("Type")]
		public string ControlAssemblyName
		{
			get
			{
				return targetControl.GetType().Assembly.FullName;
			}
		}

		[Category("Type")]
		public string ControlAssemblyLocation
		{
			get
			{
				return targetControl.GetType().Assembly.Location;
			}
		}
		
		[Category("Type")]
		public string ControlTypeName
		{
			get
			{
				return targetControl.GetType().FullName;
			}
		}
		
		private void CreateProperties()
		{
			PropertyDescriptor[] descArray = new PropertyDescriptor[thisProperties.Count + controlProperties.Count];
			thisProperties.CopyTo(descArray, 0);
			controlProperties.CopyTo(descArray, thisProperties.Count);
			
			properties = new PropertyDescriptorCollection(descArray);
		}

		#region Implementation of ICustomTypeDescriptor
		public System.ComponentModel.TypeConverter GetConverter()
		{
			return null;
		}

		public System.ComponentModel.EventDescriptorCollection GetEvents(System.Attribute[] attributes)
		{
			return new EventDescriptorCollection(null);
		}

		public System.ComponentModel.EventDescriptorCollection GetEvents()
		{
			return new EventDescriptorCollection(null);
		}

		public string GetComponentName()
		{
			return this.GetType().Name;
		}

		public object GetPropertyOwner(System.ComponentModel.PropertyDescriptor pd)
		{
			if (pd == null)
				return this;
			
			if (thisProperties.Contains(pd))
				return this;

			return targetControl;
		}

		public System.ComponentModel.AttributeCollection GetAttributes()
		{
			return new AttributeCollection(null);
		}

		public System.ComponentModel.PropertyDescriptorCollection GetProperties(System.Attribute[] attributes)
		{
			controlProperties = TypeDescriptor.GetProperties(targetControl, attributes);
			thisProperties = TypeDescriptor.GetProperties(this, attributes, true);
			
			CreateProperties();
			
			return properties;
		}

		public System.ComponentModel.PropertyDescriptorCollection GetProperties()
		{
			controlProperties = TypeDescriptor.GetProperties(targetControl);
			thisProperties = TypeDescriptor.GetProperties(this, true);
			
			CreateProperties();
			
			return properties;
		}

		public object GetEditor(System.Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(targetControl, editorBaseType);
		}

		public System.ComponentModel.PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(targetControl);
		}

		public System.ComponentModel.EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(targetControl);
		}

		public string GetClassName()
		{
			return this.GetType().Name;
		}
		#endregion
	}
}
