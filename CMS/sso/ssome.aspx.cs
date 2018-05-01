﻿using System; 
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class ssoonelogin : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //string hash = GetMD5HashData("testestet");
if (CMS.Helpers.RequestContext.IsUserAuthenticated)
          {
             if (Session["userClass"] != null && Session["userClass"] != "")
                {
                    userinfo ui = (userinfo)Session["userClass"];
                   OneMine(ui.ID);
                    
                }
                
                else
                {
                OneMine("01786850");
                }
          }
          
          else
          {
           // OneMine("01786850");
          Response.Redirect("http://me.smenet.org"); 
          }
         
    
    }
    
   /* public void OneMine(string MemberID)
    
    {
    
           Response.Write(" http://me.smenet.org/?pid=1&ts=20141125172456&securecode=FEF3B8F26A7C5124D627ECBA8D9D8946&userid="+MemberID);
   
    }*/
     public void OneMine(string MemberID)
    
    {
    string Pid = "1";
      string partnerKey = "P3U2N9E4E9";
      DateTime dt = DateTime.UtcNow;// DateTime.Now;//YYYYMMDDHHmmSS
       
      string Day = dt.Day.ToString();
      if (Day.Count() == 1)
      { Day = "0" + Day; }
      string Month = dt.Month.ToString();
      if (Month.Count()  == 1)
      { Month = "0" + Month; }
      string hour = dt.Hour.ToString();
      if (hour.Count() == 1)
      { hour = "0" + hour; }
        string second =  dt.Second.ToString();
        if (second.Count() == 1)
        { second = "0" + second; }
        string minute = dt.Minute.ToString();
        if (minute.Count() == 1)
        { minute = "0" + minute; }
        string time = dt.Year.ToString() + Month + Day + dt.ToString("HHmmss");// hour + minute + second;// +partnerKey; 
           Response.Redirect("http://me.smenet.org/?pid=" + Pid + "&ts=" + time + "&securecode=" + GetMD5HashData(Pid + time + partnerKey) + "&userid="+MemberID);
       // Response.Write(GetMD5HashData(partnerKey + time + Pid) + "--" + (partnerKey + time + Pid));
  
    }
    
    private string GetMD5HashData(string data)
    {
        //create new instance of md5
        MD5 md5 = MD5.Create();

        //convert the input text to array of bytes
        byte[] result = md5.ComputeHash(Encoding.Default.GetBytes(data));



         StringBuilder sb = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                sb.Append(result[i].ToString("X2"));
            }

            // And return it
            return sb.ToString();

        // return hexadecimal string
        //return returnValue.ToString();
    }
    
   

 

    
     
   



}
