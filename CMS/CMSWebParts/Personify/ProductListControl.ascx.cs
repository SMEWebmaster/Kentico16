using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using AjaxControlToolkit;
using CMS.Controls;
using CMS.Helpers;
using CMS.PortalControls;
using System.Configuration;
using Personify.WebControls.Base.Business;
using Telerik.Web.UI;
using System.Web;


namespace PWC
{
    //Error Message  ButtonText NumberofColumns  ProductDetailUrl

    public partial class ProductListControl : CMSAbstractLayoutWebPart
    {
        private const string HostTitle = "ProductListControl";

        private string m_ButtonText; //

        private string m_ErrorMessage; //

        private string m_ProductColumnCount; //

        private string m_ProductDetailUrl; // 

        #region "Future"

        private string m_CustomCssClass;



        private string m_ExcludePrimaryMemberGroup;

        private string m_ImageDirectory;

        private string m_PredefinedProductCategories;

        private string m_PredefinedProductClassesExclude;

        private string m_PredefinedProductClassesInclude;

        private string m_PredefinedProductIds;

        private string m_PredefinedSubsystems;



        private string m_ProductIdUrlParameter;

        private string m_ProductMeetingUrl;

        private string m_ProductMemberJoinUrl;

        private string m_RedirectUrl;

        private string m_SearchTitle;

        private string m_ShowFacebook;

        private string m_ShowLinkedIn;

        private string m_ShowMailToLink;

        private string m_ShowPinterest;

        private string m_ShowTwitter;

        private string m_TextWidth;

        private string m_Width;

        private string m_WidgetDataSaved;

        #endregion

        #region "Public properties"

        /// <summary>
        /// Number of panes.
        /// </summary>
        public int Panes
        {
            get
            {
                return ValidationHelper.GetInteger(GetValue("Panes"), 2);
            }
            set
            {
                SetValue("Panes", value);
            }
        }


        /// <summary>
        /// Pane headers.
        /// </summary>
        public string ButtonText
        {
            get
            {
                return ValidationHelper.GetString(GetValue("m_ButtonText"), "");
            }
            set
            {
                SetValue("m_ButtonText", value);
            }
        }


        /// <summary>
        /// Active pane index.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return ValidationHelper.GetString(GetValue("m_ErrorMessage"), "");
            }
            set
            {
                SetValue("m_ErrorMessage", value);
            }
        }
 
        public string ProductColumnCount
        {
            get
            {
                return ValidationHelper.GetString(GetValue("m_ProductColumnCount"), "");
            }
            set
            {
                SetValue("m_ProductColumnCount", value);
            }
        }

        public string ProductDetailUrl
        {
            get
            {
                return ValidationHelper.GetString(GetValue("m_ProductDetailUrl"), "");
            }
            set
            {
                SetValue("m_ProductDetailUrl", value);
            }
        }




        #endregion


        protected void Page_Load(object sender, EventArgs e)
        {
            PrepareLayout();

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            //unpw.CurrentIdentity = new PersonifyIdentity()
            //                           {
            //                               MasterCustomerId = "000000001475",
            //                               SubCustomerId = 0,
            //                               IsLoggedIn = true
            //                           };

        }

        #region "Methods"

        /// <summary>
        /// Prepares the layout of the web part.
        /// </summary>
        protected override void PrepareLayout()
        {
            StartLayout();
            PersonifyControlBase objbase = new PersonifyControlBase();


            var ctl = new Personify.WebControls.Store.UI.ProductListControl();

            objbase.InitPersonifyWebControl(ctl);
        /*   if (!string.IsNullOrEmpty(ErrorMessage)) { ctl.ErrorMessage = ErrorMessage; }
            if (!string.IsNullOrEmpty(ProductColumnCount)) { ctl.ProductColumnCount = Convert.ToInt32(ProductColumnCount); }
            if (!string.IsNullOrEmpty(ProductDetailUrl)) { ctl.ProductDetailUrl = ProductDetailUrl; } */
            phPersonifyControl.Controls.Add(ctl);


            FinishLayout();
        }

        #endregion

    }
}