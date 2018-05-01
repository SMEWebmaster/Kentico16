using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Web.UI.WebControls;
using System.Linq;

using CMS.Base;
using CMS.EventLog;
using CMS.ExtendedControls;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_System_Debug_System_DebugThreads : CMSDebugPage
{
    #region "Variables"

    protected int index = 0;
    protected TimeSpan totalDuration = new TimeSpan(0);
    protected DateTime now = DateTime.Now;

    #endregion


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        EnsureScriptManager();
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        now = DateTime.Now;

        btnRunDummy.Text = GetString("DebugThreads.Test");

        gridThreads.Columns[1].HeaderText = GetString("unigrid.actions");
        gridThreads.Columns[2].HeaderText = GetString("ThreadsLog.Context");
        gridThreads.Columns[3].HeaderText = GetString("ThreadsLog.ThreadID");
        gridThreads.Columns[4].HeaderText = GetString("ThreadsLog.Status");
        gridThreads.Columns[5].HeaderText = GetString("ThreadsLog.Started");
        gridThreads.Columns[6].HeaderText = GetString("ThreadsLog.Duration");

        gridFinished.Columns[1].HeaderText = GetString("ThreadsLog.Context");
        gridFinished.Columns[2].HeaderText = GetString("ThreadsLog.ThreadID");
        gridFinished.Columns[3].HeaderText = GetString("ThreadsLog.Status");
        gridFinished.Columns[4].HeaderText = GetString("ThreadsLog.Started");
        gridFinished.Columns[5].HeaderText = GetString("ThreadsLog.Finished");
        gridFinished.Columns[6].HeaderText = GetString("ThreadsLog.Duration");

        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "Cancel", ScriptHelper.GetScript(
            @"function CancelThread(threadGuid) {
                if (confirm(" + ScriptHelper.GetLocalizedString("ViewLog.CancelPrompt") + @")) {
                    document.getElementById('" + hdnGuid.ClientID + "').value = threadGuid;" +
            Page.ClientScript.GetPostBackEventReference(btnCancel, null) +
            @"}
              }"));
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ReloadData();
        
        // Hide headers if there is nothing to show
        headThreadsRunning.Visible = gridThreads.HasControls();
        headThreadsFinished.Visible = gridFinished.HasControls();

        ScriptHelper.RegisterDialogScript(this);
    }


    protected void ReloadData()
    {
        LoadGrid(gridThreads, ThreadDebug.LiveThreadItems);
        LoadGrid(gridFinished, ThreadDebug.FinishedThreadItems);
    }


    /// <summary>
    /// Loads the grid with the data.
    /// </summary>
    /// <param name="grid">Grid to load</param>
    /// <param name="threads">List of threads</param>
    protected void LoadGrid(GridView grid, List<ThreadDebugItem> threads)
    {
        index = 0;

        // Ensure new collection for binding
        grid.DataSource = threads.ToList();
        // Bind the grid
        grid.DataBind();
    }


    protected void btnRunDummy_Click(object sender, EventArgs e)
    {
        LogContext.EnsureLog(Guid.NewGuid());

        CMSThread dummy = new CMSThread(RunTest);
        dummy.Start();

        Thread.Sleep(100);
        ReloadData();
    }


    private void RunTest()
    {
        for (int i = 0; i < 50; i++)
        {
            Thread.Sleep(100);
            LogContext.AppendLine("Sample log " + i);
        }
    }


    /// <summary>
    /// Gets the item index.
    /// </summary>
    protected int GetIndex()
    {
        return ++index;
    }


    /// <summary>
    /// Gets the duration of the thread.
    /// </summary>
    /// <param name="startTime">Start time</param>
    /// <param name="endTime">End time</param>
    protected string GetDuration(object startTime, object endTime)
    {
        TimeSpan duration = ValidationHelper.GetDateTime(endTime, now).Subtract(ValidationHelper.GetDateTime(startTime, now));
        totalDuration = totalDuration.Add(duration);

        return GetDurationString(duration);
    }


    /// <summary>
    /// Gets the duration as formatted string.
    /// </summary>
    /// <param name="duration">Duration to get</param>
    protected string GetDurationString(TimeSpan duration)
    {
        string result = null;
        if (duration.TotalHours >= 1)
        {
            result += duration.Hours + ":";
            result += duration.Minutes.ToString().PadLeft(2, '0') + ":";
            result += duration.Seconds.ToString().PadLeft(2, '0');
        }
        else if (duration.TotalMinutes >= 1)
        {
            result += duration.Minutes + ":";
            result += duration.Seconds.ToString().PadLeft(2, '0');
        }
        else
        {
            result = duration.TotalSeconds.ToString("F3");
        }

        return result;
    }


    /// <summary>
    /// Gets the actions for the thread.
    /// </summary>
    /// <param name="hasLog">Log presence</param>
    /// <param name="threadGuid">Thread GUID</param>
    /// <param name="status">Status</param>
    protected string GetActions(object hasLog, object threadGuid, object status)
    {
        string result = null;

        bool logAvailable = ValidationHelper.GetBoolean(hasLog, false);
        if (logAvailable)
        {
            string url = URLHelper.ResolveUrl("~/CMSModules/System/Debug/System_ViewLog.aspx");
            url = URLHelper.UpdateParameterInUrl(url, "threadGuid", threadGuid.ToString());
            if (WebFarmHelper.WebFarmEnabled)
            {
                url = URLHelper.UpdateParameterInUrl(url, "serverName", WebFarmHelper.ServerName);
            }

            var button = new CMSGridActionButton
            {
                IconCssClass = "icon-eye",
                IconStyle = GridIconStyle.Allow,
                ToolTip = GetString("General.View"),
                OnClientClick = "modalDialog('" + url + "', 'ThreadProgress', '1000', '700'); return false;"
            };

            result += button.GetRenderedHTML();
        }

        if (ValidationHelper.GetString(status, null) != "AbortRequested")
        {
            var button = new CMSGridActionButton
            {
                IconCssClass = "icon-times-circle",
                IconStyle = GridIconStyle.Critical,
                ToolTip = GetString("General.Cancel"),
                OnClientClick = "CancelThread('" + threadGuid + "'); return false;"
            };

            result += button.GetRenderedHTML();
        }

        return result;
    }


    protected void btnCancel_Click(object sender, EventArgs e)
    {
        Guid threadGuid = ValidationHelper.GetGuid(hdnGuid.Value, Guid.Empty);
        CMSThread thread = CMSThread.GetThread(threadGuid);
        if (thread != null)
        {
            thread.Stop();
        }
    }


    protected void timRefresh_Tick(object sender, EventArgs e)
    {
    }
}