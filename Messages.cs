using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Globalization;

namespace Personify.ErrorHandling
{
	
	public class Messages
	{
		
		// ELEMENTS
		[XmlElement("Exceptions")]
		public Exceptions Exceptions { get; set; }
		
		[XmlElement("ValidationIssues")]
		public ValidationIssues ValidationIssues { get; set; }
		
		// CONSTRUCTOR
		public Messages()
		{}
	}
}
