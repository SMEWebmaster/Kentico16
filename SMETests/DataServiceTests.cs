using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Data.Services.Client;
using SMETests.PersonifyData;
using Personify.WebControls.Base.Business;
using Personify.ErrorHandling;
using System.Text.RegularExpressions;

namespace SMETests
{
    [TestClass]
    public class DataServiceTests
    {
        private readonly string PersonifyVendorBlock = System.Configuration.ConfigurationManager.AppSettings["PersonifySSO_Block"];
        private readonly string PersonifyVendorPassword = System.Configuration.ConfigurationManager.AppSettings["PersonifySSO_Password"];
        private readonly string PersonifyVendorName = System.Configuration.ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
        private readonly string PersonifyVendorID = System.Configuration.ConfigurationManager.AppSettings["PersonifySSO_VendorID"];

        private readonly string svcLogin = System.Configuration.ConfigurationManager.AppSettings["svcLogin"];
        private readonly string svcPassword = System.Configuration.ConfigurationManager.AppSettings["svcPassword"];
        private readonly string svcUri_Base = System.Configuration.ConfigurationManager.AppSettings["svcUri_Base"];
        private readonly string svcUri_BaseAlt = System.Configuration.ConfigurationManager.AppSettings["svcUri_BaseAlt"];


        [TestMethod]
        public void CanCall_DataService_Success()
        {
            try
            {
                Uri serviceUri = new Uri(svcUri_Base);

                var service = new PersonifyData.PersonifyEntitiesBase(serviceUri);
                service.IgnoreMissingProperties = true;
                service.Credentials = new System.Net.NetworkCredential(svcLogin, svcPassword);
                var membershipDetails1 = service.WebMembershipJoinProducts.Where(x => x.ProductId > 0L).ToList();
                var membershipDetails = service.WebMembershipJoinProducts.AddQueryOption("$filter", "ProductId gt 0").ToList();

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        }

        [TestMethod]
        public void CreateIndividualSampleCode1()
        {
            try
            {
                //Create an individual Customer with 1 address linked to employer and another individual address
                var a = new DataServiceCollection<SaveAddressInput>(null, TrackingMode.None);

                //Address linked to a company
                SaveAddressInput a1 = new SaveAddressInput()
                {
                    AddressTypeCode = "WORK",
                    //Address1 = "One DesCombes Drive",
                    City = "Broomfield",
                    State = "CO",
                    PostalCode = "80020",
                    CountryCode = "USA",
                    OverrideAddressValidation = false,
                    SetRelationshipAsPrimary = true,
                    PrimaryAddress = true,
                    CreateNewAddressIfOrdersExist = true,
                    EndOldPrimaryRelationship = true,
                    WebMobileDirectory = true,
                };

                a.Add(a1);


                SaveCustomerInput s = new SaveCustomerInput()
                {
                    FirstName = "Troy_New_Member",
                    LastName = "Fulton",
                    CustomerClassCode = "INDIV",

                    Addresses = a,

                };

                SaveCustomerOutput op = SvcClient.Post<SaveCustomerOutput>("CreateIndividual", s);
                op.Equals(op);
            }
            catch (Exception ex)
            {
                var message = ex.Message;

                var messages = message.ParseXML<Messages>();

                throw ex;
            }
        }

        [TestMethod]
        public void testregex()
        {
            String str = "Lorem ipsum dolor sit amet, consectetur adipiscing elit." + System.Environment.NewLine
            + "My second question? Sed amet elementum."
            + "Integer nec diam erat, eu consectetur nibh?"
            + "Cum sociis natoque penatibus et magnis dis parturient montes.";
            var pattern = new Regex("([^.?!]*)\\?", RegexOptions.Compiled);

            var result = pattern.Replace(str, String.Empty);

            var help = result.ToString();
        }


        [TestMethod]
        public void CreateIndividualSampleCode2()
        {
            try
            {
                //Create an individual Customer with 1 address linked to employer and another individual address
                var a = new DataServiceCollection<SaveAddressInput>(null, TrackingMode.None);

                SaveCustomerInput s = new SaveCustomerInput()
                {
                    FirstName = "Troy",
                    LastName = "Fulton",
                    CustomerClassCode = "INDIV"
                };

                SaveCustomerOutput op = SvcClient.Post<SaveCustomerOutput>("CreateIndividual", s);
                op.Equals(op);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [TestMethod]
        public void CountryStateTest1()
        {
            Uri serviceUri = new Uri(svcUri_Base);

            var service = new PersonifyData.PersonifyEntitiesBase(serviceUri);
            service.IgnoreMissingProperties = true;
            service.Credentials = new System.Net.NetworkCredential(svcLogin, svcPassword);
            var countries = service.Countries.Where(x => x.ActiveFlag == true).ToList().OrderBy(x => x.CountryCode == "USA" ? 0 : 1).ThenBy(x => x.CountryDescription).ToList();

            var state = service.States.Where(x => x.ActiveFlag == true && x.CountryCodeString == "USA" ).ToList();

            Assert.IsTrue(true);
       }

        [TestMethod]
        public void Stuff1()
        {
            Uri serviceUri = new Uri(svcUri_Base);

            var service = new PersonifyData.PersonifyEntitiesBase(serviceUri);
            service.IgnoreMissingProperties = true;
            service.Credentials = new System.Net.NetworkCredential(svcLogin, svcPassword);
            var stuff = service.ApplicationCodes.Where(x => x.ActiveFlag == true).ToList();

            var subsystems = stuff.Select(x => x.SubsystemString).Distinct().ToList();
            var types = stuff.Select(x => x.Type).Distinct().ToList();
            var appCodesSubCodes = stuff.Select(x => new { x.Code, x.ApplicationSubcode }).Distinct().ToList();


            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Stuff2()
        {
            Uri serviceUri = new Uri(svcUri_Base);

            var service = new PersonifyData.PersonifyEntitiesBase(serviceUri);
            service.IgnoreMissingProperties = true;
            service.Credentials = new System.Net.NetworkCredential(svcLogin, svcPassword);
            var stuff = service.ApplicationCodes.Where(x => x.ActiveFlag == true && x.SubsystemString == "CUS" && x.Type == "Demographic").ToList();

            var appCodesSubCodes = stuff.Select(x => new { x.Code, x.ApplicationSubcode }).Distinct().ToList();

            var stuff2 = service.ApplicationSubcodes.Where(x => x.ActiveFlag == true && x.Subsystem == "CUS" && x.Type == "Demographic").ToList();


            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Stuff3()
        {
            Uri serviceUri = new Uri(svcUri_Base);

            var service = new PersonifyData.PersonifyEntitiesBase(serviceUri);
            service.IgnoreMissingProperties = true;
            service.Credentials = new System.Net.NetworkCredential(svcLogin, svcPassword);
            var stuff = service.CustomerInfos.Where(x => x.LastName == "Hoiberg").ToList();

            var firstOne = stuff.FirstOrDefault();

            var ssoservice = new com.personifycloud.smemitst.service();

            var personifyIdentifier = firstOne.MasterCustomerId + "|0";

            var customer = ssoservice.SSOCustomerGet(PersonifyVendorName, PersonifyVendorPassword, personifyIdentifier);

            var imsService = new com.personifycloud.smemitst1.IMService();

            var webRoles = imsService.IMSCustomerWebRoleGet(PersonifyVendorName, PersonifyVendorPassword, personifyIdentifier);

            var otherRoles = imsService.IMSCustomerRoleGet(PersonifyVendorName, PersonifyVendorPassword, personifyIdentifier);


            Assert.IsTrue(true);
        }
    }
}
