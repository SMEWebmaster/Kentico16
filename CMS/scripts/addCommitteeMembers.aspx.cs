using System;
using System.Collections.Generic;
using System.Linq;

using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;
using System.Web.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using personifyDataservice;
using System.Net;
using CMS.CustomTables;
using CMS.DataEngine;
using CMS.PortalControls;
using CMS.Helpers;

public partial class scripts_addCommitteeMembers : System.Web.UI.Page
{
   int currentPageNumber, pageFrom = 0, pageTo = 80, pageNumber;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            string Query =
                "select distinct MasterCustomerId,LabelName from dbo.Sme_CommiteesMaster where MasterCustomerId not in(select distinct CommitteeMasterCustomer from Sme_CommiteesMembers )"; // "select distinct MasterCustomerId,LabelName from dbo.Sme_CommiteesMaster";
            var queryToGetCommitteesCount = new QueryParameters(Query, null, CMS.DataEngine.QueryTypeEnum.SQLQuery, false);

            DataSet ds = ConnectionHelper.ExecuteQuery(queryToGetCommitteesCount);
            int committeesCount = ds.Tables[0].Rows.Count;
            int noOfPages = (committeesCount / 80) + 1;

            for (int i = 0; i < noOfPages; i++)
            {
                pageNumber = i + 1;
                LinkButton pageNumbers = new LinkButton();
                pageNumbers.Text = pageNumber.ToString();
                pageNumbers.Command += new CommandEventHandler(PageClickCommand);
                pageNumbers.CommandArgument = pageNumber.ToString();
                pageNumbers.Click += new EventHandler(PageClick);
                paging.Controls.Add(pageNumbers);
            }
        }
        catch (Exception ex)
        {
            string folderName = "scripts";
            string fileName = "ExceptionLoggedCommitteesMembers.txt";
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName, fileName);

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine("Message :" + ex.Message + Environment.NewLine + "StackTrace :" + ex.StackTrace +
                   "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
            }
        }

    }

    public void PageClickCommand(Object sender, CommandEventArgs e)
    {
        currentPageNumber = Convert.ToInt32(e.CommandArgument);
        pageFrom = ((currentPageNumber - 1) * 80) + 1;
        pageTo = currentPageNumber * 80;
        LoadIndividualData(pageFrom, pageTo);
    }

    public void PageClick(object sender, EventArgs e)
    {

    }
    public void LoadIndividualData(int pageFrom, int pageTo)
    {
        StringBuilder documentsAddedStatus = new StringBuilder();

        string Query = "select * from (select distinct MasterCustomerId,LabelName,ROW_NUMBER() over (ORDER BY MasterCustomerId) AS Number from dbo.Sme_CommiteesMaster) as com where Number>=" + pageFrom + "AND Number<=" + pageTo;

        var queryToGetCommittees = new QueryParameters(Query, null, CMS.DataEngine.QueryTypeEnum.SQLQuery, false);
        DataSet ds = ConnectionHelper.ExecuteQuery(queryToGetCommittees);   //ExecQuery(Query); //("pb.account_types.select_accounts", null, where, null);
        string CommitteeMemberId = string.Empty;
        DateTime BeginDate = new DateTime();
        string CommitteeMasterCustomer = string.Empty;
        string CommitteeMemberLastFirstName = string.Empty;
        string CommitteeSubCustomer = string.Empty;
        DateTime EndDate = new DateTime();
        string MemberAddressId = string.Empty;
        string MemberAddressTypeCodeString = string.Empty;
        string MemberMasterCustomer = string.Empty;
        string ParticipationStatusCodeString = string.Empty;
        string PositionCodeDescriptionDisplay = string.Empty;
        string PositionCodeString = string.Empty;
        string VotingStatusCodeString = string.Empty;
        string CommitteeLabelName = string.Empty;

        if (ds.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Uri ServiceUri =
                        new Uri("http://smemitst.personifycloud.com/PersonifyDataServices/PersonifyDatasme.svc");
                PersonifyEntitiesBase DataAccessLayer = new PersonifyEntitiesBase(ServiceUri);
                DataAccessLayer.Credentials = new NetworkCredential("admin", "admin");
                var CommiteeMembers =
                    DataAccessLayer.CommitteeMembers.Where(p => p.CommitteeMasterCustomer == dr["MasterCustomerId"])
                        .Select(o => o)
                        .ToList();
                if (CommiteeMembers != null)
                {
                    foreach (var member in CommiteeMembers)
                    {
                        CommitteeMemberId = member.CommitteeMemberId.ToString();
                        BeginDate = Convert.ToDateTime(member.BeginDate);
                        CommitteeMasterCustomer = member.CommitteeMasterCustomer;
                        CommitteeMemberLastFirstName = member.CommitteeMemberLastFirstName;
                        CommitteeSubCustomer = member.CommitteeSubCustomer.ToString();
                        MemberAddressId = member.MemberAddressId.ToString();

                        MemberAddressTypeCodeString = member.MemberAddressTypeCodeString;
                        MemberMasterCustomer = member.MemberMasterCustomer;
                        ParticipationStatusCodeString = member.ParticipationStatusCodeString;
                        PositionCodeDescriptionDisplay = member.PositionCodeDescriptionDisplay.ToString();


                        PositionCodeString = member.PositionCodeString;
                        VotingStatusCodeString = member.VotingStatusCodeString.ToString();
                        CommitteeLabelName = member.CommitteeLabelName;
                        EndDate = Convert.ToDateTime(member.EndDate);

                        string customTableClassName = "Sme.CommiteesMembers";
                        string where = "CommitteeMemberId='" + CommitteeMemberId + "' AND CommitteeMemberLastFirstName='" + CommitteeMemberLastFirstName.Replace("'","''") +"'";

                        // Check if Custom table 'Sme.CommiteesMembers' exists
                        DataClassInfo customTable = DataClassInfoProvider.GetDataClassInfo(customTableClassName);

                        DataSet customTableRecord = CustomTableItemProvider.GetItems(customTableClassName, where);

                        int memberID = 0;

                        if (!DataHelper.DataSourceIsEmpty(customTableRecord))
                        {
                            // Get the custom table item ID
                            memberID = ValidationHelper.GetInteger(customTableRecord.Tables[0].Rows[0][0], 0);
                        }


                        if (customTable != null)
                        {
                            if (memberID == 0)
                            {

                                if (EndDate >= DateTime.Now
                                    || PositionCodeDescriptionDisplay.ToLower().Contains("president"))
                                {


                                    // Create new item for custom table with "Sme.CommiteesMembers" code name
                                    CustomTableItem item = CustomTableItem.New("Sme.CommiteesMembers");
                                    item.SetValue("CommitteeMemberId", CommitteeMemberId);
                                    item.SetValue("BeginDate", BeginDate);
                                    item.SetValue("CommitteeMasterCustomer", CommitteeMasterCustomer);
                                    item.SetValue("CommitteeMemberLastFirstName", CommitteeMemberLastFirstName.Replace("'","''"));
                                    item.SetValue("CommitteeSubCustomer", CommitteeSubCustomer);

                                    item.SetValue("MemberAddressId", MemberAddressId);
                                    item.SetValue("MemberAddressTypeCodeString", MemberAddressTypeCodeString);
                                    item.SetValue("MemberMasterCustomer", MemberMasterCustomer);
                                    item.SetValue("ParticipationStatusCodeString", ParticipationStatusCodeString);
                                    item.SetValue("PositionCodeDescriptionDisplay", PositionCodeDescriptionDisplay);
                                    item.SetValue("PositionCodeString", PositionCodeString);

                                    item.SetValue("VotingStatusCodeString", VotingStatusCodeString);
                                    item.SetValue("CommitteeLabelName", CommitteeLabelName);
                                    item.SetValue("EndDate", EndDate);
                                    item.Insert();

                                }

                                /*documentsAddedStatus.Append(
                                    "Added CommitteeLabelName : " + CommitteeLabelName + " in the CommitteeMemberId: "
                                    + CommitteeMemberId + "at" + DateTime.Now + Environment.NewLine);*/
                            }
                            else
                            {
                                if (!DataHelper.DataSourceIsEmpty(customTableRecord))
                                {
                                    // Get the custom table item
                                    CustomTableItem updateItem = CustomTableItemProvider.GetItem(memberID, customTableClassName);

                                    if (updateItem != null)
                                    {

                                        if (EndDate >= DateTime.Now
                                            || PositionCodeDescriptionDisplay.ToLower().Contains("president"))
                                        {

                                            //updateItem.SetValue("CommitteeMemberId", CommitteeMemberId);
                                            updateItem.SetValue("BeginDate", BeginDate);
                                            updateItem.SetValue("CommitteeMasterCustomer", CommitteeMasterCustomer);
                                            updateItem.SetValue("CommitteeMemberLastFirstName", CommitteeMemberLastFirstName.Replace("'","''"));
                                            updateItem.SetValue("CommitteeSubCustomer", CommitteeSubCustomer);

                                            updateItem.SetValue("MemberAddressId", MemberAddressId);
                                            updateItem.SetValue(
                                                "MemberAddressTypeCodeString",
                                                MemberAddressTypeCodeString);
                                            updateItem.SetValue("MemberMasterCustomer", MemberMasterCustomer);
                                            updateItem.SetValue(
                                                "ParticipationStatusCodeString",
                                                ParticipationStatusCodeString);
                                            updateItem.SetValue(
                                                "PositionCodeDescriptionDisplay",
                                                PositionCodeDescriptionDisplay);
                                            updateItem.SetValue("PositionCodeString", PositionCodeString);

                                            updateItem.SetValue("VotingStatusCodeString", VotingStatusCodeString);
                                            updateItem.SetValue("CommitteeLabelName", CommitteeLabelName);
                                            updateItem.SetValue("EndDate", EndDate);
                                            updateItem.Update();

                                            /*documentsAddedStatus.Append(
                                            "Updated CommitteeLabelName : " + CommitteeLabelName + " in the CommitteeMemberId: "
                                            + CommitteeMemberId + "at" + DateTime.Now + Environment.NewLine);*/
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                /*string rootFolder = "CMSWebParts";
                string folderName = "SME";
                string fileName = "CommitteeMembersStatusLogged.txt";
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rootFolder, folderName, fileName);

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine(documentsAddedStatus);
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }*/
            }
        }
    }
}