using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Globalization;

namespace Personify.ErrorHandling
{
	
	public class ValidationIssues
	{
		
		// ELEMENTS
		[XmlElement("ValidationMessage")]
		public List<ValidationMessage> ValidationMessage { get; set; }
		
		// CONSTRUCTOR
		public ValidationIssues()
		{}
	}
}
