using CMS.Base;
using System;
using System.Collections.Generic;

/// <summary>
/// SearchTerm custom data container
/// </summary>
public class ShoojuSearchTerm : IDataContainer
{
    public ShoojuSearchTerm()
    {

    }

    public ShoojuSearchTerm(String searchTerm)
    {
        this.SearchTerm = searchTerm;
    }

    public String SearchTerm { get; set; }

    public List<String> ColumnNames
    {
        get
        {
            return new List<String>() { "SearchTerm" };
        }
    }

    // Not implemented for brevity
    public bool ContainsColumn(string columnName)
    {
        throw new NotImplementedException();
    }

    // Not implemented for brevity
    public bool SetValue(string columnName, object value)
    {
        throw new NotImplementedException();
    }

    public bool TryGetValue(string columnName, out object value)
    {
        switch (columnName.ToLowerCSafe())
        {
            case "searchterm":
                value = SearchTerm;
                return true;
        }

        value = null;
        return false;
    }

    public object this[string columnName]
    {
        get
        {
            return GetValue(columnName);
        }
        set
        {
            SetValue(columnName, value);
        }
    }

    public object GetValue(string columnName)
    {
        object value;
        TryGetValue(columnName, out value);
        return value;
    }
}