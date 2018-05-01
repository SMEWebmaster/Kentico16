using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.IO;

namespace SMETests
{
    [TestClass]
    public class SSOTests
    {
        private readonly string PersonifyAutoLoginUrl = ConfigurationManager.AppSettings["PersonifyAutoLoginUrl"];
        private readonly string PersonifySsoUrl = ConfigurationManager.AppSettings["personify.SSO.service"];
        private readonly string PersonifyVendorID = System.Configuration.ConfigurationManager.AppSettings["PersonifySSO_VendorID"];
        private readonly string PersonifyVendorBlock = ConfigurationManager.AppSettings["PersonifySSO_Block"];
        private readonly string PersonifyVendorName = ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
        private readonly string PersonifyVendorPassword = ConfigurationManager.AppSettings["PersonifySSO_Password"];
        private readonly string svcUri_Base = ConfigurationManager.AppSettings["svcUri_Base"];
        private readonly string svcLogin = ConfigurationManager.AppSettings["svcLogin"];
        private readonly string svcPassword = ConfigurationManager.AppSettings["svcPassword"];


        [TestMethod]
        public void SSOWebServiceTestHarness1()
        {
            try
            {
                var service = new com.personifycloud.smemitst.service();
                //this fails bigly
                var result = service.TIMSSCustomerIdentifierGet(PersonifyVendorName, PersonifyVendorPassword, "asdfsafdsafdsdf");
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        }

        [TestMethod]
        public void SSOWebServiceTestHarness2()
        {
            try
            {
                var username = "tfulton";
                var password = "Password1";

                var service = new com.personifycloud.smemitst.service();

                var vendorToken = RijndaelAlgorithm.GetVendorToken("http://testpage.com/", PersonifyVendorPassword, PersonifyVendorBlock, username, password, true);

                var url = string.Format("{0}?vi={1}&vt={2}", PersonifyAutoLoginUrl, PersonifyVendorID, vendorToken);

                System.Net.HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.AutomaticDecompression = DecompressionMethods.GZip;
                var html = string.Empty;

                var querystringDictionary = new Dictionary<string, string>();
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    html = reader.ReadToEnd();
                    var queryString = response.ResponseUri.Query;

                    if (queryString.StartsWith("?"))
                    {
                        queryString = queryString.Substring(1);

                        foreach(var pair in queryString.Split(new char[] {'&'}))
                        {
                            var items = pair.Split(new char[] { '=' });

                            if (items != null && items.Length == 2)
                            {
                                querystringDictionary.Add(items.First(), items.Last());
                            }
                        }
                    }
                }

                if (querystringDictionary.ContainsKey("ct"))
                {
                    var encryptedCT = querystringDictionary["ct"];

                    var r = service.CustomerTokenDecrypt(PersonifyVendorName, PersonifyVendorPassword, PersonifyVendorBlock, encryptedCT);

                    ///this should decrypt into a guid looking thing.  Sometimes the webservice returns a corrupt string.  When that happens, should we just retry?
                    var customerToken = r.CustomerToken;

                    var tokenIsValidResult = service.SSOCustomerTokenIsValid(PersonifyVendorName, PersonifyVendorPassword, customerToken);

                    if (tokenIsValidResult.Valid)
                    {
                        customerToken = tokenIsValidResult.NewCustomerToken;

                        var result = service.SSOCustomerGetByCustomerToken(PersonifyVendorName, PersonifyVendorPassword, customerToken);

                        if (result != null && result.UserExists)
                        {
                            var userExists = result.UserExists;
                            var userName = result.UserName;
                            var email = result.Email;
                            var flag = result.DisableAccountFlag;

                            //for giggles
                            var ciResult = service.TIMSSCustomerIdentifierGet(PersonifyVendorName, PersonifyVendorPassword, customerToken);

                            if (ciResult == null || String.IsNullOrEmpty(ciResult.CustomerIdentifier))
                            {
                                var identifier = "0517438|0";

                                var identifierSetResult = service.TIMSSCustomerIdentifierSet(PersonifyVendorName, PersonifyVendorPassword, userName, identifier);

                                if (identifierSetResult.CustomerIdentifier == identifier)
                                {
                                    System.Console.WriteLine(identifierSetResult.CustomerIdentifier);
                                }

                            }
                            var imsService = new com.personifycloud.smemitst1.IMService();

                            var allRolesResult = imsService.IMSVendorRolesGet(PersonifyVendorName, PersonifyVendorPassword);

                            var groupResult = imsService.IMSCustomerRoleGet(PersonifyVendorName, PersonifyVendorPassword, customerToken);

                            if (groupResult != null && groupResult.CustomerRoles != null)
                            {
                                foreach (var customerRole in groupResult.CustomerRoles)
                                {
                                    if (customerRole != null && !String.IsNullOrEmpty(customerRole.Value))
                                    {
                                        var aRole = customerRole.Value;
                                        aRole.Equals(aRole);
                                    }
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        }

        [TestMethod]
        public void TestMethod2()
        {
            try
            {
                var encryptedCTForPaul = "f4b1558cae0bc44ee97e2978661f1c5fe4154d3a00c47c2f30aca8eef8287fad78a8c4d55eea1fbe7a58af576980359bcb0ac9c7166f43a73ca6d2a2289eed70";
                //var encryptedCTForEddie = "e51cfa2b7203e1205807b5b5b477d4df9f2129331a2761b1d5f16cc48bc65daf96d64b631234f70308f95ff32803d163736e7c5b1a5a87b9c3c2905e3d65eb7b";


                //encryptedCTForPaul = encryptedCTForEddie;
                var service = new com.personifycloud.smemitst.service();

                var r = service.CustomerTokenDecrypt(PersonifyVendorName, PersonifyVendorPassword, PersonifyVendorBlock, encryptedCTForPaul);

                var ctForPaul = r.CustomerToken;
                // 14fd71c5-dcb2-4112-bbe6-53e5aa5612b2


                var tokenIsValid = service.SSOCustomerTokenIsValid(PersonifyVendorName, PersonifyVendorPassword, ctForPaul);

                var result = service.TIMSSCustomerIdentifierGet(PersonifyVendorName, PersonifyVendorPassword, ctForPaul);

                if (result != null && (result.Errors == null || result.Errors.Length == 0) && !String.IsNullOrEmpty(result.CustomerIdentifier))
                {
                    var splitIdentifer = result.CustomerIdentifier.Split(new char[] { '|' }, 2, StringSplitOptions.RemoveEmptyEntries);

                    if (splitIdentifer.Length == 2)
                    {
                        var masterCustomerId = splitIdentifer[0];
                        var subCustomerId = splitIdentifer[1];


                        var result2 = service.SSOCustomerGet(PersonifyVendorName, PersonifyVendorPassword, result.CustomerIdentifier);

                        if (result2 != null && (result2.Errors == null || result2.Errors.Length == 0))
                        {
                            var userExists = result2.UserExists;
                            var userName = result2.UserName;
                            var email = result2.Email;
                            var flag = result2.DisableAccountFlag;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        }


        [TestMethod]
        public void TestMethod3()
        {
            try
            {
                var service = new com.personifycloud.smemitst.service();

                var result = service.SSOCustomerRegister(PersonifyVendorName, PersonifyVendorPassword, "ewood", "Password1", "ewood@bluemodus.com", "Eddie", "Wood");
                //8bc25ca9-012e-4d39-b977-6fed9fa5e7eb

                var result2 = service.SSOCustomerRegister(PersonifyVendorName, PersonifyVendorPassword, "tfulton", "Password1", "tfulton@bluemodus.com", "Troy", "Fulton");
                //086b0f68-1852-4eae-a4b1-9bae70cf985b
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        }
    }
}
