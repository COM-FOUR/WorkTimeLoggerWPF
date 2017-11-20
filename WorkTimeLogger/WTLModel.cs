using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Windows.Shell;

//using System.Drawing;

namespace WorkTimeLogger
{
    #region model
    /// <summary>
    /// model for mvvm pattern
    /// </summary>
    public class WTLModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged(string propName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }
    /// <summary>
    /// contains login credentials and related methods
    /// </summary>
    public class Login : WTLModel
    {
        #region  Privates
        internal string userID { get; set; }
        internal string password { get; set; }
        internal bool useWindowsAuthentication = false;
        internal string newPassword = "";
        internal string newPassword2 = "";
        internal bool changeSuccessfull = false;
        #endregion

        #region  Publics
        public string UserID { get { return userID; } set { userID = value; NotifyPropertyChanged("UserID"); } }
        public string Password { get { return password; } set { password = value; NotifyPropertyChanged("Password"); } }
        public string Signature {
            get
            {
                return Convert.ToBase64String(Encoding.Unicode.GetBytes(String.Format("{0}|{1}|{2}", this.loginType(), userID, password.Replace("=", "?"))));
            }
        }
        public bool isComplete { get { return (userID != "" && password != "" && userID != null && password != null); } }
        public string NewPassword { get { return newPassword; } set { newPassword = value; NotifyPropertyChanged("NewPassword"); NotifyPropertyChanged("NewPasswordValid"); NotifyPropertyChanged("NewPasswordsDifferent"); } }
        public string NewPassword2 { get { return newPassword2; } set { newPassword2 = value; NotifyPropertyChanged("NewPassword2"); NotifyPropertyChanged("NewPasswordValid"); NotifyPropertyChanged("NewPasswordsDifferent"); } }
        public bool NewPasswordsDifferent { get { return newPassword != newPassword2; } }
        public bool NewPasswordValid { get { return (newPassword != "") && (newPassword == newPassword2); } }
        public bool ChangeSuccessfull { get { return changeSuccessfull; } set { changeSuccessfull = value; NotifyPropertyChanged("ChangeSuccessfull"); } }
        public bool UseWindowsAuthentication { get { return useWindowsAuthentication; } set { useWindowsAuthentication = value; NotifyPropertyChanged("UseWindowsAuthentication"); } }
        public bool IsSteffen { get { return this.UserID == "209"; } }
        #endregion

        #region Methods
        public string GetNewUserSignature(string staffId)
        {
            return Convert.ToBase64String(Encoding.Unicode.GetBytes(String.Format("{0}|{1}", staffId, newPassword.Replace("=", "?"))));
        }
        public Login() : this("", "") { }
        public Login(string userid, string password)
        {
            UserID = userid;
            Password = password;
        }
        internal string loginType()
        {
            string result="XX0";
            if (useWindowsAuthentication)
            {
                result = "XX1";
            }

            return result;
        }
        #endregion
    }
    /// <summary>
    /// Datamodel for Worktime
    /// </summary>
    public class WorkTime : WTLModel
    {
        //constructor
        public WorkTime()
        {
            date = DateTime.Today;
            hours = 0;
            minutes = 0;
            seconds = 0;
        }

        #region privates
        internal DateTime date;
        internal int hours;
        internal int minutes;
        internal int seconds;
        #endregion

        #region publics
        public int Hours { get { return hours; } set { CheckIntegerBounds(ref value, 0, 23); hours = value; NotifyPropertyChanged("Hours"); NotifyPropertyChanged("LogTime"); } }
        public int Minutes { get { return minutes; } set { CheckIntegerBounds(ref value, 0, 59); minutes = value; NotifyPropertyChanged("Minutes"); NotifyPropertyChanged("LogTime"); } }
        public int Seconds { get { return seconds; } set { CheckIntegerBounds(ref value, 0, 59); seconds = value; NotifyPropertyChanged("Seconds"); NotifyPropertyChanged("LogTime"); } }
        public DateTime LogDate
        {
            get {return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);}
            set
            {
                date = new DateTime(value.Year, value.Month, value.Day, 0, 0, 0);
                NotifyPropertyChanged("LogDate");
            }
        }
        public DateTime LogTime
        {
            get { return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hours, minutes, seconds); }
            set
            {
                hours = value.Hour;
                NotifyPropertyChanged("Hours");
                minutes = value.Minute;
                NotifyPropertyChanged("Minutes");
                seconds = value.Second;
                NotifyPropertyChanged("Seconds");

                NotifyPropertyChanged("LogTime");
            }
        }
        public DateTime LogDateTime 
        { 
            get 
            { 
                return new DateTime(date.Year, date.Month, date.Day, hours, minutes, seconds); 
            }
            set
            {
                LogDate = value;
                LogTime = value;
            }
        }
        #endregion

        #region methods
        internal void CheckIntegerBounds(ref int value, int minvalue, int maxvalue)
        {
            if (value < minvalue)
            {
                value = minvalue;
            }
            if (value > maxvalue)
            {
                value = maxvalue;
            }
        }
        #endregion
    }
    /// <summary>
    /// contains information/flags needed at runtime
    /// </summary>
    public class ProcessInformation : WTLModel
    {
        #region privates
        internal bool customTimes;
        internal bool selectEmployee;
        internal bool deleteAllowed;
        internal bool stayOpen;
        internal int selectedTab;
        internal bool needsLogin;
        internal bool loggedIn;
        internal bool smokingBreakEnabled;
        internal string errorMessage;
        internal bool useWindowsAuthentication;
        internal bool startmaximized;
        internal bool btnStartWorkDayBold;
        internal bool btnEndWorkDayBold;
        internal bool btnStartBreakBold;
        internal bool btnEndBreakBold;
        internal bool btnStartSmokeBreakBold;
        internal bool btnEndSmokeBreakBold;
        internal ImageSource overlayIcon;
        #endregion

        #region publics
        public bool SilentProcessing { get; set; }
        public bool CustomTimes { get { return customTimes; } set { customTimes = value; NotifyPropertyChanged("CustomTimes"); } }
        public bool SelectEmployee { get { return selectEmployee; } set { selectEmployee = value; NotifyPropertyChanged("SelectEmployee"); } }
        public bool ErrorOcurred { get { return errorMessage!=""; }}
        public string ErrorMessage { get { return errorMessage; } set { errorMessage = value; NotifyPropertyChanged("ErrorOcurred"); NotifyPropertyChanged("ErrorMessage"); } }
        public bool UseWindowsAuthentication { get { return useWindowsAuthentication; } set { useWindowsAuthentication = value; NotifyPropertyChanged("UseWindowsAuthentication"); } }
        public bool StayOpen { get { return stayOpen; } set { stayOpen = value; NotifyPropertyChanged("StayOpen"); } }
        public bool StartMaximized { get { return startmaximized; } set { startmaximized = value; NotifyPropertyChanged("StartMaximized"); } }
        public bool GodSlayer { get; set; }
        public bool isManager { get; set; }
        public bool DeleteAllowed { get { return deleteAllowed; } set { deleteAllowed = value; NotifyPropertyChanged("DeleteAllowed"); } }
        public string SilentWorkToDo { get; set; }
        public int SelectedTab { get { return selectedTab; } set { selectedTab = value; NotifyPropertyChanged("SelectedTab"); } }
        public bool NeedsLogin { get { return needsLogin; } set { needsLogin = value; loggedIn=!value; NotifyPropertyChanged("NeedsLogin"); NotifyPropertyChanged("LoggedIn"); } }
        public bool LoggedIn { get { return loggedIn; } set { loggedIn = value; needsLogin =!value; NotifyPropertyChanged("LoggedIn"); NotifyPropertyChanged("NeedsLogin"); } }
        public bool SmokingBreakEnabled { get { return smokingBreakEnabled; } set { smokingBreakEnabled = value; NotifyPropertyChanged("SmokingBreakEnabled"); } }
        public bool BtnStartWorkDayBold { get { return btnStartWorkDayBold; } set { btnStartWorkDayBold = value; NotifyPropertyChanged("BtnStartWorkDayBold"); } }
        public bool BtnEndWorkDayBold { get { return btnEndWorkDayBold; } set { btnEndWorkDayBold = value; NotifyPropertyChanged("BtnEndWorkDayBold"); } }
        public bool BtnStartBreakBold { get { return btnStartBreakBold; } set { btnStartBreakBold = value; NotifyPropertyChanged("BtnStartBreakBold"); } }
        public bool BtnEndBreakBold { get { return btnEndBreakBold; } set { btnEndBreakBold = value; NotifyPropertyChanged("BtnEndBreakBold"); } }
        public bool BtnStartSmokeBreakBold { get { return btnStartSmokeBreakBold; } set { btnStartSmokeBreakBold = value; NotifyPropertyChanged("BtnStartSmokeBreakBold"); } }
        public bool BtnEndSmokeBreakBold { get { return btnEndSmokeBreakBold; } set { btnEndSmokeBreakBold = value; NotifyPropertyChanged("BtnEndSmokeBreakBold"); } }
        public ImageSource OverlayIcon { get { return overlayIcon; } set { overlayIcon = value; NotifyPropertyChanged("OverlayIcon"); } }
        #endregion

        //constructor
        public ProcessInformation()
        {
            SilentProcessing = false;
            CustomTimes = false;
            StayOpen = false;
            DeleteAllowed = false;
            SilentWorkToDo = "";
            errorMessage = "";
            selectedTab = 0;
            needsLogin = true;
            loggedIn = false;
            useWindowsAuthentication = Properties.Settings.Default.useWindowsAuthentication;
        }
    }
    // deprecated class
    //public class UIElementInfo : WTLModel
    //{
    //    //windowtitle
    //    private string windowTitle;
    //    //tabnames
    //    private string tabHeadLogin;
    //    private string tabHeadEntry;
    //    private string tabHeadHistory;
    //    private string tabHeadShiftPlan;
    //    private string tabHeadVacation;
    //    private string tabHeadChangePass;
    //    //labels
    //    private string lblLoginUserID;
    //    private string lblLoginPassword;
    //    private string lblOldPassword;
    //    private string lblNewPassword;
    //    private string lblNewPassword2;
    //    private string lblNewPwDiffers;
    //    private string lblPwChangeSuccess;
    //    //button labels
    //    private string btnlblLogin;
    //    private string btnlblChangePass;
    //    private string btnlblStartDay;
    //    private string btnlblEndDay;
    //    private string btnlblStartBreak;
    //    private string btnlblEndBreak;
    //    private string btnlblStartSmokingBreak;
    //    private string btnlblEndSmokingBreak;
    //    private string btnlblExit;

    //    //messages
    //    //private string errorWebService;
    //    //private string errorLoginInvalid;

    //    //publics
    //    public string WindowTitle { get { return windowTitle; } set { windowTitle = value; NotifyPropertyChanged("WindowTitle"); } }
    //    public string TabHeadLogin { get { return tabHeadLogin; } set { tabHeadLogin = value; NotifyPropertyChanged("TabHeadLogin"); } }
    //    public string TabHeadEntry { get { return tabHeadEntry; } set { tabHeadEntry = value; NotifyPropertyChanged("TabHeadEntry"); } }
    //    public string TabHeadHistory { get { return tabHeadHistory; } set { tabHeadHistory = value; NotifyPropertyChanged("TabHeadHistory"); } }
    //    public string TabHeadShiftPlan { get { return tabHeadShiftPlan; } set { tabHeadShiftPlan = value; NotifyPropertyChanged("TabHeadShiftPlan"); } }
    //    public string TabHeadVacation { get { return tabHeadVacation; } set { tabHeadVacation = value; NotifyPropertyChanged("TabHeadVacation"); } }

    //    public string TabHeadChangePass { get { return tabHeadChangePass; } set { tabHeadChangePass = value; NotifyPropertyChanged("TabHeadChangePass"); } }

    //    public string LblLoginUserID { get { return lblLoginUserID; } set { lblLoginUserID = value; NotifyPropertyChanged("LblLoginUserID"); } }
    //    public string LblLoginPassword { get { return lblLoginPassword; } set { lblLoginPassword = value; NotifyPropertyChanged("LblLoginPassword"); } }
    //    public string LblOldPassword { get { return lblOldPassword; } set { lblOldPassword = value; NotifyPropertyChanged("LblOldPassword"); } }
    //    public string LblNewPassword { get { return lblNewPassword; } set { lblNewPassword = value; NotifyPropertyChanged("LblNewPassword"); } }
    //    public string LblNewPassword2 { get { return lblNewPassword2; } set { lblNewPassword2 = value; NotifyPropertyChanged("LblNewPassword2"); } }
    //    public string LblNewPwDiffers { get { return lblNewPwDiffers; } set { lblNewPwDiffers = value; NotifyPropertyChanged("LblNewPwDiffers"); } }
    //    public string LblPwChangeSuccess { get { return lblPwChangeSuccess; } set { lblPwChangeSuccess = value; NotifyPropertyChanged("LblPwChangeSuccess"); } }

    //    public string BtnlblLogin { get { return btnlblLogin; } set { btnlblLogin = value; NotifyPropertyChanged("BtnlblLogin"); } }
    //    public string BtnlblChangePass { get { return btnlblChangePass; } set { btnlblChangePass = value; NotifyPropertyChanged("BtnlblChangePass"); } }
    //    public string BtnlblStartDay { get { return btnlblStartDay; } set { btnlblStartDay = value; NotifyPropertyChanged("BtnlblStartDay"); } }
    //    public string BtnlblEndDay { get { return btnlblEndDay; } set { btnlblEndDay = value; NotifyPropertyChanged("BtnlblEndDay"); } }
    //    public string BtnlblStartBreak { get { return btnlblStartBreak; } set { btnlblStartBreak = value; NotifyPropertyChanged("BtnlblStartBreak"); } }
    //    public string BtnlblEndBreak { get { return btnlblEndBreak; } set { btnlblEndBreak = value; NotifyPropertyChanged("BtnlblEndBreak"); } }
    //    public string BtnlblStartSmokingBreak { get { return btnlblStartSmokingBreak; } set { btnlblStartSmokingBreak = value; NotifyPropertyChanged("BtnlblStartSmokingBreak"); } }
    //    public string BtnlblEndSmokingBreak { get { return btnlblEndSmokingBreak; } set { btnlblEndSmokingBreak = value; NotifyPropertyChanged("BtnlblEndSmokingBreak"); } }
    //    public string BtnlblExit { get { return btnlblExit; } set { btnlblExit = value; NotifyPropertyChanged("BtnlblExit"); } }

    //    public string ErrorWebService { get; set; }
    //    public string ErrorLoginInvalid { get; set; }
    //    public UIElementInfo()
    //    {
    //        windowTitle = "Arbeitszeiterfassung";

    //        tabHeadLogin = "Login";
    //        tabHeadEntry = "Eingabe";
    //        tabHeadHistory = "Historie";
    //        tabHeadShiftPlan = "Schichtplan";
    //        tabHeadVacation = "Urlaub";
    //        tabHeadChangePass = "Passwort ändern";

    //        lblLoginUserID = "BenutzerID";
    //        lblLoginPassword = "Passwort";
    //        lblOldPassword = "altes Passwort";
    //        lblNewPassword = "neues Passwort";
    //        lblNewPassword2 = "Wiederholung";
    //        lblNewPwDiffers = "Passwörter müssen übereinstimmen";
    //        lblPwChangeSuccess = "Passwort erfolgreich geändert.";

    //        btnlblLogin = "Login";
    //        btnlblChangePass = "Ändern";
    //        btnlblStartDay = "Arbeitstag Anfang";
    //        btnlblEndDay = "Arbeitstag Ende";
    //        btnlblStartBreak = "Pause Anfang";
    //        btnlblEndBreak = "Pause Ende";
    //        btnlblStartSmokingBreak = "Raucherpause Anfang";
    //        btnlblEndSmokingBreak = "Raucherpause Ende";
    //        btnlblExit = "Beenden";

    //        ErrorWebService = "Fehler beim WebService";
    //        ErrorLoginInvalid ="Login fehlerhaft";
    //    }
    //}
    /// <summary>
    /// contains information of current worktime situation, binding properties for a progressbar
    /// </summary>
    public class DailyProgress : WTLModel
    {
        #region statics
        static Brush workTimeBrush;
        static Brush breakBrush;
        static Brush sbreakBrush;
        static Brush overTimeBrush;
        #endregion

        #region privates
        internal  Brush barColor;
        internal double barMaxValue;
        internal double barValue;
        internal double workTime;
        internal double dworkTime;
        internal double sbreakTime;
        internal double breakTime;
        internal string barText;
        internal int seconds;
        internal double progressValue = 0;
        internal TaskbarItemProgressState progressState = TaskbarItemProgressState.None;
        internal System.Windows.Threading.DispatcherTimer timer;
        #endregion

        #region publics
        public Brush BarColor { get { return barColor; } set { barColor = value; NotifyPropertyChanged("BarColor"); } }
        public double BarMaxValue { get { return barMaxValue; } set { barMaxValue = Math.Round(value, 2); NotifyPropertyChanged("BarMaxValue"); } }
        public double BarValue { get { return barValue; } set { barValue = Math.Round(value,2); NotifyPropertyChanged("BarValue"); } }
        public double WorkTime { get { return workTime; } set { workTime = Math.Round(value, 2); NotifyPropertyChanged("WorkTime"); } }
        public double DWorkTime { get { return dworkTime; } set { dworkTime = Math.Round(value, 2); NotifyPropertyChanged("DWorkTime"); } }
        public double BreakTime { get { return breakTime; } set { breakTime = Math.Round(value, 2); NotifyPropertyChanged("BreakTime"); } }
        public double SBreakTime { get { return sbreakTime; } set { sbreakTime = Math.Round(value, 2); NotifyPropertyChanged("SBreakTime"); } }
        public string BarText { get { return barText; } set { barText = value; NotifyPropertyChanged("BarText"); } }
        public TaskbarItemProgressState ProgressState { get { return progressState; } set { progressState = value; NotifyPropertyChanged("ProgressState"); } }
        public double ProgressValue { get { return progressValue; } set { progressValue = value; NotifyPropertyChanged("ProgressValue"); } }
        #endregion 
        //constructor
        public DailyProgress()
        {
            LinearGradientBrush wtbrush = new LinearGradientBrush();
            wtbrush.StartPoint = new Point(0.5, 0);
            wtbrush.EndPoint = new Point(0.5,1);
            wtbrush.GradientStops.Add(new GradientStop(Colors.Green, 0));
            wtbrush.GradientStops.Add(new GradientStop(Colors.LimeGreen, 0.5));
            wtbrush.GradientStops.Add(new GradientStop(Colors.Green, 1));
            workTimeBrush = wtbrush;

            LinearGradientBrush bbrush = new LinearGradientBrush();
            bbrush.StartPoint = new Point(0.5, 0);
            bbrush.EndPoint = new Point(0.5, 1);
            bbrush.GradientStops.Add(new GradientStop(Colors.Yellow, 0));
            bbrush.GradientStops.Add(new GradientStop(Colors.Orange, 0.5));
            bbrush.GradientStops.Add(new GradientStop(Colors.Yellow, 1));
            breakBrush = bbrush;

            LinearGradientBrush sbbrush = new LinearGradientBrush();
            sbbrush.StartPoint = new Point(0.5, 0);
            sbbrush.EndPoint = new Point(0.5, 1);
            sbbrush.GradientStops.Add(new GradientStop(Colors.Orange, 0));
            sbbrush.GradientStops.Add(new GradientStop(Colors.Yellow, 0.5));
            sbbrush.GradientStops.Add(new GradientStop(Colors.Orange, 1));
            sbreakBrush = sbbrush;

            LinearGradientBrush otbrush = new LinearGradientBrush();
            otbrush.StartPoint = new Point(0.5, 0);
            otbrush.EndPoint = new Point(0.5, 1);
            otbrush.GradientStops.Add(new GradientStop(Colors.Red, 0));
            otbrush.GradientStops.Add(new GradientStop(Colors.Violet, 0.5));
            otbrush.GradientStops.Add(new GradientStop(Colors.Red, 1));
            overTimeBrush = otbrush;

            barText = "";
            barMaxValue = 100;
            barValue = barMaxValue/2;
            barColor = workTimeBrush;

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
        }

        //events
        void timer_Tick(object sender, EventArgs e)
        {
            seconds += 1;
            BarValue = seconds;
            TimeSpan ts = new TimeSpan(0, 0, seconds);
            BarText = ts.ToString("g");

            if (BarValue>BarMaxValue)
            {
                BarColor = overTimeBrush;
            }
            
        }
        /// <summary>
        /// updates current values with provided params
        /// </summary>
        /// <param name="dailyworktime">no. of daily working hours</param>
        /// <param name="dailybreaktime">no. of daily break hours</param>
        /// <param name="breakafter">after how many hours is a break mandatory</param>
        /// <param name="currworktime">currently spent hours working</param>
        /// <param name="currbreaktime">currently spent hours idling</param>
        /// <param name="currsbreaktime">currently spent hours smoking</param>
        /// <param name="currstatus">current status (working,idling,smoking,neither)</param>
        public void UpdateProgress(double dailyworktime, double dailybreaktime, double breakafter, double currworktime, double currbreaktime, double currsbreaktime, int currstatus)
        {
            bool overtime = false;
            BarText = "";

            if (timer.IsEnabled)
            {
                timer.Stop();
            }

            if (breakafter != 0 & currworktime >= breakafter)
            {
                if (dailybreaktime>currbreaktime)
                {
                    dailyworktime += dailybreaktime;
                }
                else
                {
                    dailyworktime += currbreaktime;
                }
            }
            else
            {
                dailyworktime += currbreaktime;
            }

            BarValue = currworktime+currbreaktime;

            if (dailyworktime > currworktime + currbreaktime)
            {
                BarMaxValue = dailyworktime;
            }
            else
            {
                BarMaxValue = currworktime;
                overtime = true;
            }

            BreakTime = currbreaktime*60;
            SBreakTime = currsbreaktime * 60;
            WorkTime = currworktime;
            DWorkTime = dailyworktime;

            switch (currstatus)
            {
                case 1: BarColor = breakBrush; ProgressState = TaskbarItemProgressState.Paused; 
                    break;
                case 2: BarColor = sbreakBrush; ProgressState = TaskbarItemProgressState.Paused; 
                    break;
                default:
                    if (overtime)
                    {
                        BarColor = overTimeBrush;
                        ProgressState = TaskbarItemProgressState.Error;
                    }
                    else
                    {
                        BarColor = workTimeBrush;
                        ProgressState = TaskbarItemProgressState.Normal;
                    }
                    break;
            }
            ProgressValue = currworktime / barMaxValue;
        }
        /// <summary>
        /// starts a counter for break, how many time left for break 
        /// </summary>
        /// <param name="dailybreaktime">no. of daily break hours</param>
        /// <param name="currbreaktime">currently spent hours idling</param>
        public void StartBreakCounter(double dailybreaktime, double currbreaktime)
        {
            BarColor = breakBrush;

            if (currbreaktime >= dailybreaktime)
            {
                BarMaxValue = dailybreaktime;
                BarValue = dailybreaktime;
                BarColor = overTimeBrush;
            }
            else
            {
                BarMaxValue = Convert.ToInt32(60 * 60 * dailybreaktime);
            }

            //seconds = Convert.ToInt32(60 * 60 * Math.Round(currbreaktime, 2, MidpointRounding.ToEven));
            
            timer.Start();
        }
    }
    /// <summary>
    /// contains historical data, for binding purposes
    /// </summary>
    public class WorkTimeSurplus : WTLModel
    {
        #region privates
        internal DateTime dateFrom;
        internal DateTime dateTo;
        internal double workTime;
        internal double workPlanned;
        internal ObservableCollection<SurplusEntry> surplusses;
        #endregion

        #region publics
        public DateTime DateFrom { get { return dateFrom; } set { dateFrom = value; NotifyPropertyChanged("DateFrom"); } }
        public DateTime DateTo { get { return dateTo; } set { dateTo = value; NotifyPropertyChanged("DateTo"); } }
        public double WorkTime { get { return workTime; } set { workTime = value; NotifyPropertyChanged("WorkTime"); NotifyPropertyChanged("Surplus"); } }
        public double WorkPlanned { get { return workPlanned; } set { workPlanned = value; NotifyPropertyChanged("WorkPlanned"); NotifyPropertyChanged("Surplus"); } }
        public double Surplus { get { return Math.Round(workTime - workPlanned,2); } }
        public ObservableCollection<SurplusEntry> Surplusses { get { return surplusses; } set { surplusses = value; NotifyPropertyChanged("Surplusses"); } }
        #endregion

        //cosntructor
        public WorkTimeSurplus()
        {
            dateTo = DateTime.Today.AddDays(-1);
            dateFrom = new DateTime(dateTo.Year, 1, 1);
            
            workTime = 0;
            workPlanned = 0;

            surplusses = new ObservableCollection<SurplusEntry>();
        }
    }
    /// <summary>
    /// viewmodel for mvvm pattern
    /// </summary>
    public class WTLViewModel : WTLModel
    {
        #region privates
        internal  Application currApp;
        internal Login currLogin;
        internal ProcessInformation currProcessInfo;
        //private UIElementInfo currUIElementInfo;
        internal Employee currEmployee;
        internal WorkTime currWorkTime;
        internal WorkTimeLedger lastLogEntry;
        internal List<Employee> employees;
        internal List<WorkTimeLedger> todaysLedgers;
        internal DateTime currentDate;
        internal int currentYear;
        internal double yearlyVacationDays;
        internal double remainingVacationDays;
        internal double daysofIllness;
        internal ObservableCollection<WorkTimeLedger> currentLedgers;
        internal ObservableCollection<VacationDay> vacationDays;
        internal int currentWeek;
        internal ObservableCollection<ShiftPlanEntry> shiftPlan;
        internal DailyProgress currProgress;
        internal WorkTimeSurplus currSurplus;
        #endregion

        #region relaycommands
        internal RelayCommand loginCommand;
        internal RelayCommand changePassCommand;
        internal RelayCommand getVacationCommand;
        internal RelayCommand getShiftPlanCommand;
        internal RelayCommand getSurplusCommand;
        internal RelayCommand setDefaultBreaksCommand;
        internal RelayCommand exitCommand;
        internal RelayCommand startDayCommand;
        internal RelayCommand endDayCommand;
        internal RelayCommand startBreakCommand;
        internal RelayCommand endBreakCommand;
        internal RelayCommand startSmokingBreakCommand;
        internal RelayCommand endSmokingBreakCommand;
        internal RelayCommand getServerTimeCommand;
        internal RelayCommand easyBreakCommand;
        internal RelayCommand easySBreakCommand;
        #endregion

        #region public members
        public Login CurrLogin { get { return currLogin; } set { currLogin = value; this.NotifyPropertyChanged("CurrLogin"); } }
        public ProcessInformation CurrProcessInfo { get { return currProcessInfo; } set { currProcessInfo = value; this.NotifyPropertyChanged("CurrProcessInfo"); } }
        //public UIElementInfo CurrUIElementInfo { get { return currUIElementInfo; } set { currUIElementInfo = value; this.NotifyPropertyChanged("CurrUIElementInfo"); } }
        public Employee CurrEmployee { get { return currEmployee; } 
            set 
            { 
                currEmployee = value;

                if (value == null | value.StaffNo  == null | value.StaffNo == "")
                    return;

                CurrProcessInfo.SmokingBreakEnabled = value.SmokingBreakEnabled; 
                this.NotifyPropertyChanged("CurrEmployee"); 
                
                if (GetWorkTimeLedgers(currWorkTime.LogDate, out todaysLedgers))
                {
                    SetCurrentLedgers(todaysLedgers);
                }
                
                GetLastLogEntry();
                UpdateSegments(todaysLedgers);
            } 
        }
        public WorkTime CurrWorkTime { get { return currWorkTime; } set { currWorkTime = value; this.NotifyPropertyChanged("CurrWorkTime"); } }
        public WorkTimeLedger LastLogEntry { get { return lastLogEntry; } set { lastLogEntry = value; this.NotifyPropertyChanged("LastLogEntry"); } }
        public List<Employee> Employees { get { return employees; } set { employees = value; this.NotifyPropertyChanged("Employees"); } }
        public List<WorkTimeLedger> TodaysLedgers { get { return todaysLedgers; } set { todaysLedgers = value; CalcSegments(); this.NotifyPropertyChanged("TodaysLedgers"); } }
        public DateTime CurrentDate { get { return currentDate; } 
            set 
            { 
                currentDate = value;
                
                NotifyPropertyChanged("CurrentDate");

                if (CurrEmployee.StaffNo != null)
                {
                    SetCurrentLedgers(null);
                }
                
                CurrentWeek = Helpers.GetCurrentWeek(value);
            } 
        }
        public int CurrentYear {get { return currentYear; } set{currentYear = value; NotifyPropertyChanged("CurrentYear"); } }
        public double YearlyVacationDays { get { return yearlyVacationDays; } set { yearlyVacationDays = value; this.NotifyPropertyChanged("YearlyVacationDays"); } }
        public double RemainingVacationDays { get { return remainingVacationDays; } set { remainingVacationDays = value; this.NotifyPropertyChanged("RemainingVacationDays"); } }
        public double DaysofIllness { get { return daysofIllness; } set { daysofIllness = value; this.NotifyPropertyChanged("DaysofIllness"); } }
        public ObservableCollection<WorkTimeLedger> CurrentLedgers { get { return currentLedgers; } set { currentLedgers = value; NotifyPropertyChanged("CurrentLedgers"); } }
        public ObservableCollection<VacationDay> VacationDays { get { return vacationDays; } set { vacationDays = value; NotifyPropertyChanged("VacationDays"); } }
        public int CurrentWeek { get { return currentWeek; } set { currentWeek = value; NotifyPropertyChanged("CurrentWeek"); } }
        public ObservableCollection<ShiftPlanEntry> ShiftPlan { get { return shiftPlan; } set { shiftPlan = value; NotifyPropertyChanged("ShiftPlan"); } }
        public DailyProgress CurrProgress { get { return currProgress; } set { currProgress = value; NotifyPropertyChanged("CurrProgress"); } }
        public WorkTimeSurplus CurrSurplus { get { return currSurplus; } set { currSurplus = value; NotifyPropertyChanged("CurrSurplus"); } }
        #endregion

        #region icommands
        public ICommand LoginCommand
        {
            get
            {
                if (loginCommand == null)
                {
                    loginCommand = new RelayCommand(param => this.PerformLogin(),
                        param => this.currLogin.isComplete);
                }
                return loginCommand;
            }
        }
        public ICommand ChangePassCommand
        {
            get
            {
                if (changePassCommand == null)
                {
                    changePassCommand = new RelayCommand(param => this.ChangePassword(),
                        param => (this.currLogin.NewPasswordValid));
                }
                return changePassCommand;
            }
        }
        public ICommand GetVacationCommand
        {
            get
            {
                if (getVacationCommand == null)
                {
                    getVacationCommand = new RelayCommand(param => this.GetVacations(),
                        param => (this.currentYear !=0));
                }
                return getVacationCommand;
            }
        }
        public ICommand GetShiftPlanCommand
        {
            get
            {
                if (getShiftPlanCommand == null)
                {
                    getShiftPlanCommand = new RelayCommand(param => this.GetShiftPlan(),
                        param => (true));
                }
                return getShiftPlanCommand;
            }
        }
        public ICommand GetSurplusCommand
        {
            get
            {
                if (getSurplusCommand == null)
                {
                    getSurplusCommand = new RelayCommand(param => this.GetSurplus(),
                        param => (this.currSurplus.DateFrom<=this.currSurplus.DateTo));
                }
                return getSurplusCommand;
            }
        }
        public ICommand SetDefaultBreaksCommand
        {
            get
            {
                if (setDefaultBreaksCommand == null)
                {
                    setDefaultBreaksCommand = new RelayCommand(param => this.SetDefaultBreaks(),
                        param => (this.currProcessInfo.LoggedIn));
                }
                return setDefaultBreaksCommand;
            }
        }
        public ICommand ExitCommand
        {
            get
            {
                if (exitCommand == null)
                {
                    exitCommand = new RelayCommand(param => this.currApp.Shutdown(),
                        param => true);
                }
                return exitCommand;
            }
        }
        public ICommand StartDayCommand
        {
            get
            {
                if (startDayCommand == null)
                {
                    startDayCommand = new RelayCommand(param => this.SetWorkTime(LogTypes.DayStart),
                        param => true);
                }
                return startDayCommand;
            }
        }
        public ICommand EndDayCommand
        {
            get
            {
                if (endDayCommand == null)
                {
                    endDayCommand = new RelayCommand(param => this.SetWorkTime(LogTypes.DayEnd),
                        param => true);
                }
                return endDayCommand;
            }
        }
        public ICommand StartBreakCommand
        {
            get
            {
                if (startBreakCommand == null)
                {
                    startBreakCommand = new RelayCommand(param => this.SetWorkTime(LogTypes.BreakStart),
                        param => true);
                }
                return startBreakCommand;
            }
        }
        public ICommand EndBreakCommand
        {
            get
            {
                if (endBreakCommand == null)
                {
                    endBreakCommand = new RelayCommand(param => this.SetWorkTime(LogTypes.BreakEnd),
                        param => true);
                }
                return endBreakCommand;
            }
        }
        public ICommand StartSmokingBreakCommand
        {
            get
            {
                if (startSmokingBreakCommand == null)
                {
                    startSmokingBreakCommand = new RelayCommand(param => this.SetWorkTime(LogTypes.SmokingBreakStart),
                        param => true);
                }
                return startSmokingBreakCommand;
            }
        }
        public ICommand EndSmokingBreakCommand
        {
            get
            {
                if (endSmokingBreakCommand == null)
                {
                    endSmokingBreakCommand = new RelayCommand(param => this.SetWorkTime(LogTypes.SmokingBreakEnd),
                        param => true);
                }
                return endSmokingBreakCommand;
            }
        }
        public ICommand GetServerTimeCommand
        {
            get
            {
                if (getServerTimeCommand == null)
                {
                    getServerTimeCommand = new RelayCommand(param => this.GetUpdatedServerTime(true),
                        param => currProcessInfo.LoggedIn);
                }
                return getServerTimeCommand;
            }
        }
        public ICommand EasyBreakCommand
        {
            get
            {
                if (easyBreakCommand == null)
                {
                    easyBreakCommand = new RelayCommand(param => this.SetEasyBreak(),
                        param => this.currProcessInfo.LoggedIn);
                }
                return easyBreakCommand;
            }
        }
        public ICommand EasySBreakCommand
        {
            get
            {
                if (easySBreakCommand == null)
                {
                    easySBreakCommand = new RelayCommand(param => this.SetEasySBreak(),
                        param => this.currProcessInfo.LoggedIn);
                }
                return easySBreakCommand;
            }
        }
        #endregion

        #region constructors
        public WTLViewModel(Application app) : this(app,new string[0]) { }
        public WTLViewModel(Application app,string[] startupArgs)
        {
            currApp = app;

            //currUIElementInfo = new UIElementInfo();

            currProcessInfo = new ProcessInformation();

            SetCurrLogin(Properties.Settings.Default.Default_Login, Properties.Settings.Default.Default_Password);

            currEmployee = new Employee();
            currWorkTime = new WorkTime();

            currentLedgers = new ObservableCollection<WorkTimeLedger>();

            employees = new List<Employee>();

            currentYear = DateTime.Now.Year;

            currentWeek = 1;

            currProgress = new DailyProgress();

            shiftPlan = new ObservableCollection<ShiftPlanEntry>();

            currSurplus = new WorkTimeSurplus();

            if (startupArgs.Length > 0)
            {
                ProcessStartupArgs(startupArgs);
            }

            this.CurrentLedgers.CollectionChanged += currentLedgers_CollectionChanged;
            
        }
        #endregion

        //events
        void currentLedgers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action!= System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                return;
            }

            bool erroroccurred = false;

            foreach (WorkTimeLedger item in e.OldItems)
	        {
                if (!DeleteWorkTime(item))
                {
                    erroroccurred = true;
                }
	        }

            if (erroroccurred)
            {
                SetCurrentLedgers(null);
                SetErrorMessage("Beim Löschen der Arbeitszeit(en) ist ein Fehler aufgetreten");
            }

            if (currentDate.Date==currWorkTime.LogDate.Date)
            {
                GetLastLogEntry();
                UpdateSegments(null);
            }
        }

        #region internal Methods
        /// <summary>
        /// processing of provided startup arguments
        /// </summary>
        /// <param name="startupArgs">startup arguments</param>
        internal void ProcessStartupArgs(string[] startupArgs)
        {
            foreach (string arg in startupArgs)
            {
                if (arg.Contains("="))
                {
                    ProcessComplexStartupArg(arg);
                }
                else
                {
                    ProcessSimpleStartupArg(arg.ToUpper());
                }
            }
        }
        /// <summary>
        /// processing of one simple startup argument (containing no composite argument)
        /// </summary>
        /// <param name="arg">current argument</param>
        internal void ProcessSimpleStartupArg(string arg)
        {
            switch (arg)
            {
                case "STARTDAY":
                case "ENDDAY":
                case "STARTBREAK":
                case "ENDBREAK":
                case "STARTSBREAK":
                case "ENDSBREAK":
                    currProcessInfo.SilentProcessing = true;
                    currProcessInfo.SilentWorkToDo = arg;
                    break;
                //case "DEMIGOD":
                //    currProcessInfo.StayOpen = true;
                //    currProcessInfo.SelectEmployee = true;
                //    break;
                //case "GODMODE":
                //    currProcessInfo.DeleteAllowed = true;
                //    currProcessInfo.StayOpen = true;
                //    currProcessInfo.CustomTimes = true;
                //    currProcessInfo.SelectEmployee = true;

                //    break;
                case "GODSLAYER":
                    currProcessInfo.DeleteAllowed = true;
                    currProcessInfo.StayOpen = true;
                    currProcessInfo.CustomTimes = true;
                    currProcessInfo.GodSlayer = true;
                    currProcessInfo.SelectEmployee = true;
                    break;
                case "STAYOPEN": currProcessInfo.StayOpen = true;
                    break;
                case "USENTLM": currProcessInfo.UseWindowsAuthentication = true;
                    break;
                case "MAXIMIZED": currProcessInfo.StartMaximized = true;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// processing of one complex startup argument (containing argument and value, divided by "=")
        /// </summary>
        /// <param name="arg">current composite argument</param>
        internal void ProcessComplexStartupArg(string arg)
        {
            string argtype = arg.Substring(0, arg.IndexOf("=")).ToUpper();
            string argvalue = arg.Substring(arg.IndexOf("=") + 1);

            switch (argtype)
            {
                case "LOGIN": this.currLogin.UserID = argvalue;  break;
                case "PASSWORD": this.currLogin.Password = argvalue; break;
            }
        }
        /// <summary>
        /// sets current login
        /// </summary>
        /// <param name="userid">user id </param>
        /// <param name="password">password</param>
        internal void SetCurrLogin(string userid, string password)
        {
            SetCurrLogin(userid, password, false);
        }
        /// <summary>
        /// sets current login
        /// </summary>
        /// <param name="userid">user id</param>
        /// <param name="password">password</param>
        /// <param name="overrideexisting">override if allready exits</param>
        internal void SetCurrLogin(string userid, string password, bool overrideexisting)
        {
            if (CurrLogin == null | overrideexisting)
            {
                CurrLogin = new Login();
            }

            if (CurrProcessInfo!=null)
            {
                CurrLogin.UseWindowsAuthentication = CurrProcessInfo.UseWindowsAuthentication;
            }
            
            if (CurrLogin.UseWindowsAuthentication)
            {
                CurrLogin.UserID = String.Format("{0}\\{1}",Environment.UserDomainName,Environment.UserName);
                CurrLogin.Password = String.Format("{0}\\{1}", Environment.UserName,Environment.UserDomainName);
            }
            else
            {
                if (userid != "")
                {
                    CurrLogin.UserID = userid;
                }

                if (password != "")
                {
                    CurrLogin.Password = password;
                }
            }
        }
        /// <summary>
        /// checks if a button should be shown with a bold font
        /// </summary>
        internal void CheckBtnFontBold()
        { 
            CurrProcessInfo.BtnStartWorkDayBold = (lastLogEntry.LogType=="Logout" & lastLogEntry.LogSubtype=="WorkDay");
            CurrProcessInfo.BtnEndWorkDayBold = (lastLogEntry.LogType=="Login");
            CurrProcessInfo.BtnStartBreakBold = (lastLogEntry.LogType=="Login");
            CurrProcessInfo.BtnEndBreakBold = (lastLogEntry.LogType=="Logout" & lastLogEntry.LogSubtype=="Break");
            CurrProcessInfo.BtnStartSmokeBreakBold = (lastLogEntry.LogType=="Login");
            CurrProcessInfo.BtnEndSmokeBreakBold = (lastLogEntry.LogType == "Logout" & lastLogEntry.LogSubtype == "SmokeBreak");
        }
        /// <summary>
        /// sets the overlay icon in windows Taskbar
        /// </summary>
        internal void SetOverlayIcon()
        {
            BitmapImage bi = null;

            if (lastLogEntry.LogType=="Login")
            {
                bi = new BitmapImage( new Uri("pack://application:,,,/WorkTimeLogger;component/Images/Income.ico"));
            }
            if (lastLogEntry.LogType=="Logout" & lastLogEntry.LogSubtype=="Break")
            {
                bi = new BitmapImage(new Uri("pack://application:,,,/WorkTimeLogger;component/Images/cocktail.ico"));
            }
            if (lastLogEntry.LogType == "Logout" & lastLogEntry.LogSubtype == "SmokeBreak")
            {
                bi = new BitmapImage(new Uri("pack://application:,,,/WorkTimeLogger;component/Images/smoking.ico"));
            }

            CurrProcessInfo.OverlayIcon = bi;
        }
        /// <summary>
        /// sets the CurrentLedgers or retrieves them, if null provided
        /// </summary>
        /// <param name="ledgers">List of WorkTimeLedger entries</param>
        internal void SetCurrentLedgers(List<WorkTimeLedger> ledgers)
        {
            if (CurrEmployee.StaffNo == null)
            {
                return;
            }

            if (ledgers == null) 
            { 
                if (!GetWorkTimeLedgers(currentDate, out ledgers))
                {
                    return;
                }
            }

            this.CurrentLedgers.CollectionChanged -= currentLedgers_CollectionChanged;
            this.CurrentLedgers = new ObservableCollection<WorkTimeLedger>(ledgers);
            this.CurrentLedgers.CollectionChanged += currentLedgers_CollectionChanged;
        }
        /// <summary>
        /// sets the bindable ErrorMessage property
        /// </summary>
        /// <param name="error">new error message</param>
        internal void SetErrorMessage(string error)
        {
            currProcessInfo.ErrorMessage = error;
        }
        /// <summary>
        /// retrieves the ServerTime via WTL_Service
        /// </summary>
        /// <param name="serverTime">set to ServerTime if successfully retrieved</param>
        /// <returns>true, if succeeded</returns>
        internal bool GetServerTime(ref DateTime serverTime)
        {
            bool result = false;

            SetErrorMessage("");

            try
            {
                WTL_Service.WTL_PortClient wtl = new WTL_Service.WTL_PortClient("WTL_Port", Properties.Settings.Default.ServiceURL);
                wtl.ClientCredentials.Windows.ClientCredential = new NetworkCredential(Properties.Settings.Default.ServiceLogin, Properties.Settings.Default.ServicePassword);

                result = wtl.GetServerTime(currLogin.Signature, ref serverTime);

                serverTime = serverTime.ToLocalTime();
            }
            catch (Exception e)
            {
                SetErrorMessage(e.Message);
                return false;
            }
            
            return result;
        }
        /// <summary>
        /// changes password via WTL_Service
        /// </summary>
        /// <returns>true if changed successfully</returns>
        internal bool ChangePassword()
        {
            bool result = false;

            if (!currLogin.NewPasswordValid)
            {
                return false;
            }

            SetErrorMessage("");

            try
            {
                WTL_Service.WTL_PortClient wtl = new WTL_Service.WTL_PortClient("WTL_Port", Properties.Settings.Default.ServiceURL);
                wtl.ClientCredentials.Windows.ClientCredential = new NetworkCredential(Properties.Settings.Default.ServiceLogin, Properties.Settings.Default.ServicePassword);

                result = wtl.ChangePassword(currLogin.Signature,currLogin.GetNewUserSignature(currEmployee.StaffNo));

                currLogin.ChangeSuccessfull = result;

                if (result)
                {
                    if (currEmployee.isCurrentLogin == "Yes")
                    {
                        currLogin.Password = currLogin.NewPassword;
                    }

                    currLogin.NewPassword = "";
                    currLogin.NewPassword2 = "";
                }

            }
            catch (Exception e)
            {
                SetErrorMessage(e.Message);
                return false;
            }

            return result;
        }
        /// <summary>
        /// get list of Employees via WTL_Service
        /// </summary>
        /// <param name="employeeNoFilter">filter for Staff No., if provided, only the own Employee-Record will be retrieved</param>
        /// <returns>true, if succeeded</returns>
        internal bool GetEmployeeList(string employeeNoFilter)
        {
            bool result = false;

            SetErrorMessage("");

            try
            {
                WTL_Service.WTL_PortClient wtl = new WTL_Service.WTL_PortClient("WTL_Port", Properties.Settings.Default.ServiceURL);
                wtl.ClientCredentials.Windows.ClientCredential = new NetworkCredential(Properties.Settings.Default.ServiceLogin, Properties.Settings.Default.ServicePassword);

                WTL_Service.Member staffMember = new WTL_Service.Member();

                if (wtl.GetStaffMember(CurrLogin.Signature, employeeNoFilter, ref staffMember))
                {
                    result = true;
                    employees.Clear();
                    foreach (WTL_Service.Staff staff in staffMember.Staff)
                    {
                        Employee newemp = new Employee(staff);
                        employees.Add(newemp);

                        if (newemp.isCurrentLogin=="Yes")
                        {
                            CurrEmployee = newemp;
                            currProcessInfo.isManager = newemp.isManager == "Yes";

                            if (!currProcessInfo.isManager & !currProcessInfo.GodSlayer)
                            {
                                currProcessInfo.CustomTimes = false;
                                currProcessInfo.DeleteAllowed = false;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SetErrorMessage(e.Message);
                return false;
            }
            
            return result;
        }
        /// <summary>
        /// creates a WorkTimeLedger via WTL_Service
        /// </summary>
        /// <param name="logtype">type of Ledger</param>
        /// <returns>true, if succeeded</returns>
        internal bool SetWorkTime(LogTypes logtype)
        {
            bool result = false;

            SetErrorMessage("");

            string logType = "";
            string logSubType = "";

            switch (logtype)
            {
                case LogTypes.DayStart: logType = "Login"; logSubType = "WorkDay";
                    break;
                case LogTypes.DayEnd: logType = "Logout"; logSubType = "WorkDay";
                    break;
                case LogTypes.BreakStart: logType = "Logout"; logSubType = "Break";
                    break;
                case LogTypes.BreakEnd: logType = "Login"; logSubType = "Break";
                    break;
                case LogTypes.SmokingBreakStart: logType = "Logout"; logSubType = "SmokingBreak";
                    break;
                case LogTypes.SmokingBreakEnd: logType = "Login"; logSubType = "SmokingBreak";
                    break;
                default:
                    break;
            }

            string creator = "Staff";

            if (CurrProcessInfo.CustomTimes && !CurrProcessInfo.GodSlayer)
            {
                creator = "Manual";
            }

            try
            {
                WTL_Service.WTL_PortClient wtl = new WTL_Service.WTL_PortClient("WTL_Port", Properties.Settings.Default.ServiceURL);
                wtl.ClientCredentials.Windows.ClientCredential = new NetworkCredential(Properties.Settings.Default.ServiceLogin, Properties.Settings.Default.ServicePassword);

                if (currProcessInfo.CustomTimes)
                {
                    result = wtl.SetWorkTime(currLogin.Signature, currEmployee.StaffNo, currWorkTime.LogDateTime.ToUniversalTime().ToString("o"), logType, logSubType, creator);
                }
                else
                {
                    result = wtl.SetWorkTime(currLogin.Signature, currEmployee.StaffNo, "", logType, logSubType, creator);
                }
            }
            catch (Exception e)
            {
                SetErrorMessage(e.Message);
                return false;
                
            }
            
            if (!currProcessInfo.StayOpen)
            {
                ExitCommand.Execute(this);
            }

            if (result && currProcessInfo.StayOpen)
            {
                this.SetCurrentLedgers(null);
                this.GetLastLogEntry();
                this.UpdateSegments(null);
                DateTime currdt = new DateTime();
                this.GetServerTime(ref currdt);
                CurrWorkTime.LogDateTime = currdt;
            }
            return result;
        }
        /// <summary>
        /// directly creates a 30min Break via WTL_Service
        /// </summary>
        internal void SetEasyBreak()
        {
            bool cust = currProcessInfo.CustomTimes;
            currProcessInfo.CustomTimes = true;
            currProcessInfo.GodSlayer = true;
                        currWorkTime.LogDateTime = DateTime.Now;

            SetWorkTime(LogTypes.BreakStart);

            currWorkTime.LogDateTime = currWorkTime.LogDateTime.AddMinutes(30);

            SetWorkTime(LogTypes.BreakEnd);

            currWorkTime.LogDateTime = DateTime.Now;
            currProcessInfo.GodSlayer = false;
            currProcessInfo.CustomTimes = cust;
        }
        /// <summary>
        /// directly creates a 30min SmokingBreak via WTL_Service
        /// </summary>
        internal void SetEasySBreak()
        {
            bool cust = currProcessInfo.CustomTimes;
            currProcessInfo.CustomTimes = true;
            currProcessInfo.GodSlayer = true;
            currWorkTime.LogDateTime = DateTime.Now;

            SetWorkTime(LogTypes.SmokingBreakStart);

            currWorkTime.LogDateTime = currWorkTime.LogDateTime.AddMinutes(5);

            SetWorkTime(LogTypes.SmokingBreakEnd);

            currWorkTime.LogDateTime = DateTime.Now;
            currProcessInfo.GodSlayer = false;
            currProcessInfo.CustomTimes = cust;
        }
        /// <summary>
        /// deletes a specific WorkTimeLedger via WTL_Service
        /// </summary>
        /// <param name="ledger">WorkTimeLedger to be deleted</param>
        /// <returns>true, if succeeded</returns>
        internal bool DeleteWorkTime(WorkTimeLedger ledger)
        {
            bool result = false;

            SetErrorMessage("");

            try
            {
                WTL_Service.WTL_PortClient wtl = new WTL_Service.WTL_PortClient("WTL_Port", Properties.Settings.Default.ServiceURL);
                wtl.ClientCredentials.Windows.ClientCredential = new NetworkCredential(Properties.Settings.Default.ServiceLogin, Properties.Settings.Default.ServicePassword);

                result = wtl.DeleteWorkTime(currLogin.Signature, currEmployee.StaffNo, ledger.LogDateTime, ledger.LogType, ledger.LogSubtype);
                
            }
            catch (Exception e)
            {
                SetErrorMessage(e.Message);
                return false;
            }
            return result;
        }
        /// <summary>
        /// retrieves the last WorkTimeLedger via WTL_Service
        /// </summary>
        /// <returns>true, if succeeded</returns>
        internal bool GetLastLogEntry()
        {
            bool result = false;

            SetErrorMessage("");

            try
            {
                WTL_Service.WTL_PortClient wtl = new WTL_Service.WTL_PortClient("WTL_Port", Properties.Settings.Default.ServiceURL);
                wtl.ClientCredentials.Windows.ClientCredential = new NetworkCredential(Properties.Settings.Default.ServiceLogin, Properties.Settings.Default.ServicePassword);

                WTL_Service.WorkTimes worktimes = new WTL_Service.WorkTimes();

                result = wtl.GetLastWorkTime(CurrLogin.Signature, currEmployee.StaffNo, ref worktimes);

                if (result)
                {
                    foreach (WTL_Service.WorkTime item in worktimes.WorkTime)
                    {
                        if (item.LogDateTime != "")
                        {
                            LastLogEntry = new WorkTimeLedger(item);
                        }
                    }
                    CheckBtnFontBold();
                    SetOverlayIcon();
                }
            }
            catch (Exception e)
            {
                SetErrorMessage(e.Message);
                return false;
            }
            
            return result;
        }
        /// <summary>
        /// retrieves a list of WorkTimeLedgers for a given date, via WTL_Service
        /// </summary>
        /// <param name="date">date for which WorkTimeLedger should be retrieved</param>
        /// <param name="ledgers">retrieved WorkTimeLedgers</param>
        /// <returns>true, if succeeded</returns>
        internal bool GetWorkTimeLedgers(DateTime date, out List<WorkTimeLedger> ledgers)
        {
            bool result = false;

            ledgers = new List<WorkTimeLedger>();

            SetErrorMessage("");
            
            try
            {
                WTL_Service.WTL_PortClient wtl = new WTL_Service.WTL_PortClient("WTL_Port", Properties.Settings.Default.ServiceURL);
                wtl.ClientCredentials.Windows.ClientCredential = new NetworkCredential(Properties.Settings.Default.ServiceLogin, Properties.Settings.Default.ServicePassword);

                WTL_Service.WorkTimes worktimes = new WTL_Service.WorkTimes();
                
                result = wtl.GetWorkTime(CurrLogin.Signature, currEmployee.StaffNo, date.ToUniversalTime().ToString("o"), date.ToUniversalTime().ToString("o"), ref worktimes);

                if (result)
                {
                    foreach (WTL_Service.WorkTime item in worktimes.WorkTime)
                    {
                        if (item.LogDateTime!="")
                        {
                            ledgers.Add(new WorkTimeLedger(item));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SetErrorMessage(e.Message);
                return false;
            }

            return result;
        }
        /// <summary>
        /// retrieves a list of VacationDays for a given year, via WTL_Service
        /// </summary>
        /// <param name="year">year fow which VacationDays should be retrieved</param>
        /// <param name="vacations">retrieved VacationDays</param>
        /// <returns>true, if succeeded</returns>
        internal bool GetVacationDays(int year, out ObservableCollection<VacationDay> vacations)
        {
            bool result = false;

            SetErrorMessage("");

            try
            {
                WTL_Service.WTL_PortClient wtl = new WTL_Service.WTL_PortClient("WTL_Port", Properties.Settings.Default.ServiceURL);
                wtl.ClientCredentials.Windows.ClientCredential = new NetworkCredential(Properties.Settings.Default.ServiceLogin, Properties.Settings.Default.ServicePassword);

                WTL_Service.VacationDays vacationdays = new WTL_Service.VacationDays();

                List<VacationDay> templist = new List<VacationDay>();

                result = wtl.GetVacation(currLogin.Signature, currEmployee.StaffNo, year, ref vacationdays);
                if (result)
                {
                    YearlyVacationDays = Double.Parse(vacationdays.YearlyVacation, CultureInfo.InvariantCulture);
                    DaysofIllness = Double.Parse(vacationdays.DaysOfIllness, CultureInfo.InvariantCulture);
                    RemainingVacationDays = Double.Parse(vacationdays.RemainingVacation, CultureInfo.InvariantCulture);

                    foreach (WTL_Service.VacationDay item in vacationdays.VacationDay)
                    {
                        templist.Add(new VacationDay(item));
                    }
                }

                vacations = new ObservableCollection<VacationDay>(templist);

            }
            catch (Exception e)
            {
                SetErrorMessage(e.Message);
                vacations = new ObservableCollection<VacationDay>();
                return false;
            }

            return result;
        }
        /// <summary>
        /// warpper for GetVacationDays(int year, out ObservableCollection<VacationDay> vacations)
        /// </summary>
        internal void GetVacations()
        {
            this.GetVacationDays(this.currentYear, out this.vacationDays);
            NotifyPropertyChanged("VacationDays");
        }
        /// <summary>
        /// updates current WorkTimeLedgers, with provided, or if null, retrieves them
        /// </summary>
        /// <param name="ledgers">new list of WorkTimeLedgers to be set</param>
        internal void UpdateSegments(List<WorkTimeLedger> ledgers)
        {
            if (ledgers == null)
            {
                if (!GetWorkTimeLedgers(currWorkTime.LogDate, out ledgers))
                {
                    return;
                }
            }

            TodaysLedgers = ledgers;
        }
        /// <summary>
        /// calculates sums of worktime,breaks and smokingbreaks, based on TodaysLedgers
        /// </summary>
        internal void CalcSegments()
        {
            DateTime lastTime = DateTime.Parse(string.Format("{0} 00:00:00", CurrentDate.ToShortDateString()));
            DateTime currTime;

            int subType = 0;
            int i = 0;

            WorkTimeLedger lastEntry = null;

            double workinghours = 0;
            double breaks = 0;
            double sbreaks = 0;

            foreach (WorkTimeLedger entry in TodaysLedgers)
            {
                if (entry.Skipped == false)
                {
                    i++;

                    currTime = DateTime.Parse(string.Format("{0} {1}", lastTime.ToShortDateString(), entry.LogTime));

                    if (i == 1)
                    {
                        if ((entry.LogType == "Logout" & entry.LogSubtype == "WorkDay") | (entry.LogType == "Login" & entry.LogSubtype != "WorkDay"))
                        {
                            workinghours+=Helpers.DateDiff(lastTime, currTime);
                        }
                    }
                    else
                    {
                        subType = -1;

                        if (entry.LogType == "Logout")
                        {
                            subType = 0;
                        }

                        if (entry.LogType == "Login" && entry.LogSubtype == "Break")
                        {
                            subType = 1;
                        }
                        if (entry.LogType == "Login" && entry.LogSubtype == "SmokingBreak")
                        {
                            subType = 2;
                        }

                        switch (subType)
                        {
                            case 0: workinghours += Helpers.DateDiff(lastTime, currTime); break;
                            case 1: breaks += Helpers.DateDiff(lastTime, currTime); break;
                            case 2: sbreaks += Helpers.DateDiff(lastTime, currTime); break;
                        }
                    }

                    lastTime = currTime;
                    lastEntry = entry;
                }
            }
            if (lastEntry != null)
            {
                if (lastEntry.LogType != "Logout" | lastEntry.LogSubtype != "WorkDay")
                {
                    i++;
                    if (lastEntry.LogType == "Login")
                    {
                        DateTime tempdt = DateTime.Parse(string.Format("{0} {1}", CurrentDate.ToShortDateString(), DateTime.Now.TimeOfDay.ToString()));
                        workinghours += Helpers.DateDiff(lastTime, tempdt);
                    }
                    else
                    {
                        DateTime tempdt = DateTime.Parse(string.Format("{0} {1}", CurrentDate.ToShortDateString(), DateTime.Now.TimeOfDay.ToString()));
                        //breaks += Helpers.DateDiff(lastTime, tempdt);
                        if (lastEntry.LogType == "Logout" && lastEntry.LogSubtype == "Break")
                        {
                            breaks += Helpers.DateDiff(lastTime, tempdt);
                        }
                        if (lastEntry.LogType == "Logout" && lastEntry.LogSubtype == "SmokingBreak")
                        {
                            sbreaks += Helpers.DateDiff(lastTime, tempdt);
                        }
                    }
                }
            }

            int status = 0;

            if (lastLogEntry!=null)
            {
                if (lastLogEntry.LogType == "Logout" & lastLogEntry.LogSubtype == "Break")
                {
                    status = 1;
                }
                if (lastLogEntry.LogType == "Logout" & lastLogEntry.LogSubtype == "SmokingBreak")
                {
                    status = 2;
                }    
            }

            CurrProgress.UpdateProgress(currEmployee.DailyWorkingTime, currEmployee.DailyBreakTime, currEmployee.BreakAfterTime, Math.Round(workinghours, 2), Math.Round(breaks, 2), Math.Round(sbreaks, 2), status);

            if (status==1)
            {
                CurrProgress.StartBreakCounter(currEmployee.DailyBreakTime, breaks);    
            }
        }
        /// <summary>
        /// retrieves List of ShiftPlanEntry, based on year and week, via WTL_service
        /// </summary>
        /// <param name="year">year for ShiftPlanEntries, to be retrieved</param>
        /// <param name="week">week for ShiftPlanEntries, to be retrieved</param>
        /// <param name="plan">retrieved list of ShiftPlanEntries</param>
        /// <returns>true, if success</returns>
        internal bool GetShiftPlanEntries(int year, int week, out ObservableCollection<ShiftPlanEntry> plan)
        {
            bool result = false;

            SetErrorMessage("");

            try
            {
                WTL_Service.WTL_PortClient wtl = new WTL_Service.WTL_PortClient("WTL_Port", Properties.Settings.Default.ServiceURL);
                wtl.ClientCredentials.Windows.ClientCredential = new NetworkCredential(Properties.Settings.Default.ServiceLogin, Properties.Settings.Default.ServicePassword);

                WTL_Service.ShiftPlans plans = new WTL_Service.ShiftPlans();

                List<ShiftPlanEntry> templist = new List<ShiftPlanEntry>();

                result = wtl.GetShiftplan(currLogin.Signature, currentYear, currentWeek, ref plans);
                if (result)
                {
                    foreach (var item in plans.PlanEntry)
                    {
                        if (item.StaffNo!="")
                        {
                            templist.Add(new ShiftPlanEntry(item));
                        }   
                    }
                }

                plan = new ObservableCollection<ShiftPlanEntry>(templist);

            }
            catch (Exception e)
            {
                SetErrorMessage(e.Message);
                plan = new ObservableCollection<ShiftPlanEntry>();
                return false;
            }

            return result;
        }
        /// <summary>
        /// wrapper for GetShiftPlanEntries(int year, int week, out ObservableCollection<ShiftPlanEntry> plan)
        /// </summary>
        internal void GetShiftPlan()
        {
            this.GetShiftPlanEntries(this.currentYear, this.currentWeek, out this.shiftPlan);
            NotifyPropertyChanged("ShiftPlan");
        }
        /// <summary>
        /// retrieves list of surplus worktime via WTL_Service
        /// </summary>
        /// <returns>true, if succeeded</returns>
        internal bool GetSurplus()
        {
            bool result = false;

            SetErrorMessage("");

            try
            {
                WTL_Service.WTL_PortClient wtl = new WTL_Service.WTL_PortClient("WTL_Port", Properties.Settings.Default.ServiceURL);
                wtl.ClientCredentials.Windows.ClientCredential = new NetworkCredential(Properties.Settings.Default.ServiceLogin, Properties.Settings.Default.ServicePassword);

                WTL_Service.WorkTimeSurplus surplusses = new WTL_Service.WorkTimeSurplus();

                List<SurplusEntry> templist = new List<SurplusEntry>();

                result = wtl.GetWorkTimeSurplus(currLogin.Signature, currEmployee.StaffNo, 0, currSurplus.DateFrom.ToUniversalTime().ToString("o"), currSurplus.DateTo.ToUniversalTime().ToString("o"), ref surplusses);
                if (result)
                {
                    double logged = 0, planned = 0;

                    if (Double.TryParse(surplusses.SumWorkHoursLogged.Replace(".", ","),out logged))
                    {
                        CurrSurplus.WorkTime = Math.Round( logged,2);
                        
                    }
                    if (Double.TryParse(surplusses.SumWorkHoursPlanned.Replace(".", ","), out planned))
                    {
                        CurrSurplus.WorkPlanned = Math.Round( planned,2);
                    }

                    foreach (var item in surplusses.Surplus)
                    {
                        if (item.Date != "")
                        {
                            templist.Add(new SurplusEntry(item));
                        }
                    }
                }

                CurrSurplus.Surplusses = new ObservableCollection<SurplusEntry>(templist);

            }
            catch (Exception e)
            {
                SetErrorMessage(e.Message);
                CurrSurplus.Surplusses = new ObservableCollection<SurplusEntry>();
                return false;
            }

            return result;
        }
        /// <summary>
        /// creates break entries for the daily break time
        /// </summary>
        internal void SetDefaultBreaks()
        {
            bool result = false;

            SetErrorMessage("");

            try
            {
                WTL_Service.WTL_PortClient wtl = new WTL_Service.WTL_PortClient("WTL_Port", Properties.Settings.Default.ServiceURL);
                wtl.ClientCredentials.Windows.ClientCredential = new NetworkCredential(Properties.Settings.Default.ServiceLogin, Properties.Settings.Default.ServicePassword);

                result = wtl.SetDefaultBreak(currLogin.Signature, currEmployee.StaffNo, currentDate.Year, Helpers.GetCurrentWeek(currentDate));

                if (result)
                {
                    SetCurrentLedgers(null);
                }

            }
            catch (Exception e)
            {
                SetErrorMessage(e.Message);
            }
        }
        /// <summary>
        /// retrieves ServerTime via WTL_Service
        /// </summary>
        /// <param name="updateSegments">update segments, if true</param>
        /// <returns>true, if succeeded</returns>
        internal bool GetUpdatedServerTime(bool updateSegments)
        {
            DateTime dt = new DateTime();

            bool result = GetServerTime(ref dt);

            CurrWorkTime.LogDate = dt;
            CurrWorkTime.LogTime = dt;

            CurrentDate = dt;

            if (updateSegments)
            {
                CalcSegments();
            }

            return result;
        }
        #endregion

        #region public Methods
        /// <summary>
        /// performs login via WTL_Service, if credentials provided. Retrieves employeelist and sets internal variables
        /// </summary>
        /// <returns>true, if succeeded</returns>
        public bool PerformLogin()
        {
            SetErrorMessage("");

            if (!CurrLogin.isComplete)
            {
                return false;
            }

            bool result = GetUpdatedServerTime(false);

            if (result)
            {
                currProcessInfo.LoggedIn = true;
                currProcessInfo.SelectedTab = 1;

                bool ok = false;

                if (CurrProcessInfo.CustomTimes | CurrProcessInfo.SelectEmployee)
                {
                    ok = GetEmployeeList("");
                }
                else
                {
                    ok = GetEmployeeList(CurrLogin.UserID);

                    if (CurrProcessInfo.isManager & !CurrProcessInfo.CustomTimes & !CurrProcessInfo.UseWindowsAuthentication)
                    {
                        ok = GetEmployeeList("");
                        currProcessInfo.DeleteAllowed = true;
                        currProcessInfo.StayOpen = true;
                        currProcessInfo.CustomTimes = true;
                        currProcessInfo.SelectEmployee = true;
                    }
                }
            }
            else
            {
                //SetErrorMessage(currUIElementInfo.ErrorLoginInvalid);
                // TODO Fix this
                SetErrorMessage("Login fehlerhaft");
            }

            return result;
        }
        /// <summary>
        /// performs tasks, provided by startup argumetns, without showing ui 
        /// </summary>
        public void PerformSilentProcessing()
        {
            if (!CurrLogin.isComplete | !CurrProcessInfo.SilentProcessing)
            {
                return;
            }

            switch (CurrProcessInfo.SilentWorkToDo)
            {
                case "STARTDAY": SetWorkTime(LogTypes.DayStart); break;
                case "ENDDAY": SetWorkTime(LogTypes.DayEnd); break;
                case "STARTBREAK": SetWorkTime(LogTypes.BreakStart); break;
                case "ENDBREAK": SetWorkTime(LogTypes.DayEnd); break;
                case "STARTSBREAK": SetWorkTime(LogTypes.SmokingBreakStart); break;
                case "ENDSBREAK": SetWorkTime(LogTypes.SmokingBreakEnd); break;
                default:
                    break;
            }
        }
        #endregion
    }
    #endregion

    #region datamodels
    /// <summary>
    /// wrapper class for autogenerated WTL_Service.Staff
    /// </summary>
    public class Employee : WTL_Service.Staff
    {
        #region additional fields
        public string FullName { get { return String.Format("{0} {1}", FirstName, LastName); } }
        public bool SmokingBreakEnabled { get; set; }
        public double DailyWorkingTime { get; set; }
        public double DailyBreakTime { get; set; }
        public double BreakAfterTime { get; set; }
        #endregion

        #region constructors
        public Employee() { }
        public Employee(WTL_Service.Staff staff)
        {
            StaffNo = staff.StaffNo;
            FirstName = staff.FirstName;
            LastName = staff.LastName;
            StatisticsGroupCode = staff.StatisticsGroupCode;
            isManager = staff.isManager;
            isCurrentLogin = staff.isCurrentLogin;
            hasSmokingBreak = staff.hasSmokingBreak;
            SmokingBreakEnabled = (hasSmokingBreak == "Yes");
            WeeklyWorkingHours = staff.WeeklyWorkingHours.Replace(".",",");
            DailyBreak = staff.DailyBreak.Replace(".", ",");
            BreakAfter = staff.BreakAfter.Replace(".", ",");
            DailyWorkingTime = double.Parse(WeeklyWorkingHours) / 5;
            DailyBreakTime = double.Parse(DailyBreak);
            BreakAfterTime = double.Parse(BreakAfter);
            LoginMessage = staff.LoginMessage;
            LogoutMessage = staff.LogoutMessage;
        }
        #endregion
    }
    /// <summary>
    /// wrapper class for autogenerated WTL_Service.WorkTime
    /// </summary>
    public class WorkTimeLedger : WTL_Service.WorkTime
    {
        #region additional fields
        public bool Skipped { get; set; }
        public string LogTypeString 
        { 
            get 
            {
                string result = "";

                // TODO fix this 
                switch (LogType+LogSubtype)
                {
                    case "LoginWorkDay": result = "Arbeitstag Anfang"; break;
                    case "LogoutWorkDay": result = "Arbeitstag Ende"; break;
                    case "LoginBreak": result = "Pause Ende"; break;
                    case "LogoutBreak": result = "Pause Anfang"; break;
                    case "LoginSmokingBreak": result = "Raucherpause Ende"; break;
                    case "LogoutSmokingBreak": result = "Raucherpause Anfang"; break;
                    default:
                        break;
                }

                return result;
            } 
        }
        public string LogDate { get; set; }
        public string LogTime { get; set; }
        public string LogText { get { return String.Format("{0}\n{1}\n{2}", LogDate, LogTime, LogTypeString); } }
        #endregion

        #region constructors
        public WorkTimeLedger(){ }
        public WorkTimeLedger(WTL_Service.WorkTime wtl)
        {
            this.Creator = wtl.Creator;
            this.LogDateTime = wtl.LogDateTime;
            DateTime dt = DateTime.Parse(wtl.LogDateTime);
            this.LogDate = dt.Date.ToShortDateString();
            this.LogTime = dt.ToShortTimeString();
            this.LogType = wtl.LogType;
            this.LogSubtype = wtl.LogSubtype;
            this.Skipped = wtl.Skip=="Yes";
            this.StaffID = wtl.StaffID;
        }
        #endregion
    }
    /// <summary>
    /// wrapper class for autogenerated WTL_Service.VacationDay
    /// </summary>
    public class VacationDay : WTL_Service.VacationDay
    {
        #region additional fields
        public string FormatedWorkDate 
        { 
            get 
            {
                string result = Date;

                DateTime dt = DateTime.Parse(result);
                result = dt.ToShortDateString();

                return result;
            } 
        }
        public bool isHalfDay { get { return HalfDay == "Yes"; } }
        public bool isPosted { get { return Posted == "Yes"; } }
        #endregion

        //constructor
        public VacationDay(WTL_Service.VacationDay day)
        {
            this.Date = day.Date;
            this.WorkShiftCode = day.WorkShiftCode;
            this.HalfDay = day.HalfDay;
            this.Posted = day.Posted;
        }
    }
    /// <summary>
    /// wrapper class for autogenerated WTL_Service.WeekDay
    /// </summary>
    public class ShiftPlanDay : WTL_Service.WeekDay
    {
        //additional 
        public string ColorName { get; set; }

        #region constructors
        public ShiftPlanDay()
        {
            this.Date = "";
            this.DayOfWeek = "";
            this.ColorName = "Gainsboro";
            this.WorkShift = "";
            this.WorkTime = "";
        }
        public ShiftPlanDay(WTL_Service.WeekDay day)
        {
            this.Date = day.Date;
            this.DayOfWeek = day.DayOfWeek;
            this.Color = day.Color;

            int color = int.Parse(Color);

            if (Color=="0")
            {
                ColorName = "Gainsboro";
            }
            else
            {
                string hexvalue = color.ToString("X");

                while (hexvalue.Length < 6)
                {
                    hexvalue = "00" + hexvalue;
                }
                
                ColorName = "#" + hexvalue.Substring(4, 2) + hexvalue.Substring(2, 2) + hexvalue.Substring(0, 2);
            }

            this.WorkShift = day.WorkShift;
            this.WorkTime = day.WorkTime;
        }
        #endregion
    }
    /// <summary>
    /// wrapper class for autogenerated WTL_Service.PlanEntry 
    /// </summary>
    public class ShiftPlanEntry : WTL_Service.PlanEntry 
    {
        #region additional fields
        public ShiftPlanDay Monday { get; set; }
        public ShiftPlanDay Tuesday { get; set; }
        public ShiftPlanDay Wednesday { get; set; }
        public ShiftPlanDay Thursday { get; set; }
        public ShiftPlanDay Friday { get; set; }
        public ShiftPlanDay Saturday { get; set; }
        public ShiftPlanDay Sunday { get; set; }
        #endregion
        //constructor
        public ShiftPlanEntry(WTL_Service.PlanEntry entry)
        {
            this.FullName = entry.FullName;
            this.StaffNo = entry.StaffNo;

            Monday = new ShiftPlanDay(); 
            Tuesday = new ShiftPlanDay();
            Wednesday = new ShiftPlanDay();
            Thursday = new ShiftPlanDay();
            Friday = new ShiftPlanDay();
            Saturday = new ShiftPlanDay();
            Sunday = new ShiftPlanDay();

            if (entry.hasWeekDays=="Yes")
            {
                foreach (WTL_Service.WeekDay item in entry.WeekDay)
                {
                    switch (item.DayOfWeek)
                    {
                        case "1": Monday = new ShiftPlanDay(item); break;
                        case "2": Tuesday = new ShiftPlanDay(item); break;
                        case "3": Wednesday = new ShiftPlanDay(item); break;
                        case "4": Thursday = new ShiftPlanDay(item); break;
                        case "5": Friday = new ShiftPlanDay(item); break;
                        case "6": Saturday = new ShiftPlanDay(item); break;
                        case "7": Sunday = new ShiftPlanDay(item); break;
                        default:
                            break;
                    }
                }
            }
        }
    }
    /// <summary>
    /// wrapper class for autogenerated WTL_Service.Surplus
    /// </summary>
    public class SurplusEntry : WTL_Service.Surplus
    {
        //additional field
        public double WorkHoursSurplus { 
            get 
            {
                double logged=0, planned = 0, breaks=0;

                if (Double.TryParse(WorkHoursLogged, out logged) & Double.TryParse(WorkHoursPlanned, out planned) & Double.TryParse(WorkBreaks, out breaks))
                {
                    return Math.Round( logged - planned,2);
                }

                return 0; 
            } 
        }
        //constructor
        public SurplusEntry(WTL_Service.Surplus surplus)
        {
            DateTime dt = DateTime.Parse(surplus.Date);
            this.Date = dt.ToShortDateString();
            this.ErrorOccurred = surplus.ErrorOccurred;
            this.Name = surplus.Name;
            this.WorkBreaks = surplus.WorkBreaks.Replace(".", ",");
            this.WorkHoursLogged = surplus.WorkHoursLogged.Replace(".", ",");
            this.WorkHoursPlanned = surplus.WorkHoursPlanned.Replace(".", ",");
            this.WorkShiftOfDate = surplus.WorkShiftOfDate;
        }
    }
    #endregion

    #region enums
    /// <summary>
    /// types of WorkTime Ledgers
    /// </summary>
    public enum LogTypes { DayStart, DayEnd, BreakStart, BreakEnd, SmokingBreakStart, SmokingBreakEnd }
    #endregion

    #region relaycommand
    public class RelayCommand : ICommand
    {
        #region Fields

        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        #endregion // Fields

        #region Constructors

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }
        #endregion // Constructors

        #region ICommand Members

        
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        #endregion // ICommand Members
    }
    #endregion

    #region helpers
    public static class Helpers
    {
        public static int GetCurrentWeek(DateTime date)
        {
            var cal = CultureInfo.CurrentCulture.Calendar;
            int currentweek = cal.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);

            return currentweek;
        }
        public static float DateDiff(DateTime Start, DateTime End)
        {
            double hours = 0;

            TimeSpan span = End - Start;
            hours = span.TotalHours;
            return (float)Math.Round(hours, 2, MidpointRounding.AwayFromZero);
        }
    }
    #endregion

}
