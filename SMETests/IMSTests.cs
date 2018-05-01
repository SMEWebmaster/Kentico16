using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SMETests
{
    [TestClass]
    public class IMSTests
    {
        private readonly string PersonifyVendorBlock = System.Configuration.ConfigurationManager.AppSettings["PersonifySSO_Block"];
        private readonly string PersonifyVendorPassword = System.Configuration.ConfigurationManager.AppSettings["PersonifySSO_Password"];
        private readonly string PersonifyVendorName = System.Configuration.ConfigurationManager.AppSettings["PersonifySSO_VendorName"];
        private readonly string PersonifyVendorID = System.Configuration.ConfigurationManager.AppSettings["PersonifySSO_VendorID"];


        [TestMethod]
        public void CanCall_IMSWebService_Success()
        {
            try
            {
                var imsService = new com.personifycloud.smemitst1.IMService();

                //currently just two roles: COMMITTEE_MEMBER & MEMBER
                var allRolesResult = imsService.IMSVendorRolesGet(PersonifyVendorName, PersonifyVendorPassword);

                foreach(var roleResult in allRolesResult.VendorRoles)
                {
                    var customers = imsService.IMSRoleCustomersGet(PersonifyVendorName, PersonifyVendorPassword, roleResult.RoleId);
                }

                //one web-role, deactivated with no role description
                var allWebRolesResult= imsService.IMSVendorWebRolesGet(PersonifyVendorName, PersonifyVendorPassword);

                foreach (var roleResult in allWebRolesResult.VendorRoles)
                {
                    var customers = imsService.IMSRoleCustomersGet(PersonifyVendorName, PersonifyVendorPassword, roleResult.RoleId);
                }

                var results = imsService.IMSCustomerRoleGetByTimssCustomerId(PersonifyVendorName, PersonifyVendorPassword, "02991320|0");
                var results2 = imsService.IMSCustomerWebRoleGet(PersonifyVendorName, PersonifyVendorPassword, "02991320|0");
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        }
    }
}
