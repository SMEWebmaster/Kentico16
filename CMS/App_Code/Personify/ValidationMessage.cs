using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Globalization;

namespace Personify.ErrorHandling
{
	
	public class ValidationMessage
	{
		// ATTRIBUTES
		[XmlAttribute("Message")]
		public string Message { get; set; }
		
		// ELEMENTS
		[XmlText]
		public string Value { get; set; }
		
		// CONSTRUCTOR
		public ValidationMessage()
		{}
	}
}
