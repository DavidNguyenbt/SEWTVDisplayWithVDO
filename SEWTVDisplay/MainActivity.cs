using Acr.UserDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using CSDL;
using Java.IO;
using Microcharts;
using Microcharts.Droid;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using A1ATeam;
using Orientation = Android.Widget.Orientation;
using Timer = System.Timers.Timer;
using OxyPlot.Xamarin.Android;
using OxyPlot.Series;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Annotations;
using System.Collections;
using Android.Graphics.Drawables;
using Xamarin.Essentials;
using Android.Net;

namespace SEWTVDisplay
{
    [Activity(Label = "Line Performance", Theme = "@style/Theme.AppCompat.Light.NoActionBar", MainLauncher = true, ScreenOrientation = ScreenOrientation.Landscape, Icon = "@drawable/performance")]
    public class MainActivity : AppCompatActivity //AppCompat
    {
        bool BeginOrEnd = true, myLanguague = true, isadShow = false, newVer = false, forceUpdate = false;
        Android.Widget.Button btBeginDateSelect, btEndDateSelect, btKanbanSelect;
        TextView tvFromDate, tvToDate, tvBeginDate, tvEndDate, tvMyClock, tvSMV, tvRunDay, tvProductType, tvWFT, tvSelectedLine, tvDT, tvTargetPH;
        TextView ActualEff, ActualOutput, ActualRFT, InLineDF, InlineDFList, EndlineDFList, tvshowpercent, tvServer;
        Spinner snLineList, snLangList, snFacList;
        TextView InLineDF1, InLineDF2, InLineDF3, InLineDF4, InLineDF5, InLineDF6;
        TextView tv1hr, tv2hr, tv3hr, tv4hr, tv5hr, tv6hr, tv7hr, tv8hr, tv9hr, tv10hr, tv11hr, tv12hr;
        TextView ActualEff1, ActualEff2, ActualEff3, ActualEff4, ActualEff5, ActualEff6, ActualEff7, ActualEff8, ActualEff9, ActualEff10, ActualEff11, ActualEff12;
        TextView ActualPCS1, ActualPCS2, ActualPCS3, ActualPCS4, ActualPCS5, ActualPCS6, ActualPCS7, ActualPCS8, ActualPCS9, ActualPCS10, ActualPCS11, ActualPCS12;
        TextView ActualRFT1, ActualRFT2, ActualRFT3, ActualRFT4, ActualRFT5, ActualRFT6, ActualRFT7, ActualRFT8, ActualRFT9, ActualRFT10, ActualRFT11, ActualRFT12;
        TextView TargetTotalPCS, ActualTotalPCS, TotalRJ, tvChartTitle, txtlinewip, tvlastrefresh;
        ChartView chartView, chartView2;
        Chart chart, chart2;
        CheckBox chkauto;
        ImageView logo;
        //ConnectToSQL conn = new ConnectToSQL();
        decimal TargetPCS_total, ActualPCS_total, ActualRFT_total, AveEFFLine_total, TargetEFF_total;
        decimal Per1, Per2, Per3, Per4, Per5, Per6, Per7, Per8, Per9, Per10, Per11, Per12;
        string LangTTTargetPCS, LangTTActualPCS, LangTTDFPCS, ShowTTActualPCS, ShowTTTargetPCS, ShowTTRJ, RunDay, RunDayNum, ProductType, TargetPH;
        string TimeTemp = "", strTest = "", WrkHrId = "", ShiftId = "";
        string LinkUpdateAPP = "http://192.168.10.133/TTPefmTVIndivSewL.apk", ImgURL = "", Fac = "";
        //Connect cn = new Connect(CSDL.chuoi);
        DataSet myLineList = new DataSet();
        //SelectedLine = "", BeginDate = "", EndDate = "";// move to public

        int mySpinerPositionID = 0, InlineDF = 0, EndlineDF = 0, ColumnCount = 0, ss = 0, mm = 0, hh = 0, RefT = 600000;
        Timer MyTimer = new Timer();
        Timer LanguageTimer = new Timer();
        Timer SecondTimer = new Timer();

        long TTRC = 0, TTDL = 0;
        int PercentComplete = 0, Progr = 0;


        DataTable dtMyLineData = new DataTable();

        Android.App.AlertDialog ad;

        Connect kn = new Connect(CSDL.chuoi);
        Dialog d;
        bool ShowBottleneck = false, autoload = true;
        Timer showbtn = new Timer();
        int disshowbtn = 0;

        PlotView piechart1, piechart2;

        DataSet QrMyLineData = new DataSet();
        A1ATeam.LayoutRequest rq = new A1ATeam.LayoutRequest(1280, 752, 1);

        List<CheckListItem> lsttxt = new List<CheckListItem>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            //RequestInit.Init(this);
            UserDialogs.Init(this);

            StrictMode.VmPolicy.Builder builder = new StrictMode.VmPolicy.Builder();
            StrictMode.SetVmPolicy(builder.Build());

            d = new Dialog(this, Resource.Style.MyDialog);

            #region initialize
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);

            //ShowingLoading("Downloading...", 3);
            //mToast("Data loaded", 1500);

            MyTimer.Enabled = true;
            MyTimer.Interval = RefT;
            MyTimer.Elapsed += MyTimer_Elapsed;
            MyTimer.Start();

            LanguageTimer.Enabled = true;
            LanguageTimer.Interval = 10000;
            LanguageTimer.Elapsed += LanguageTimer_Elapsed;
            LanguageTimer.Start();

            SecondTimer.Enabled = true;
            SecondTimer.Interval = 1000;
            SecondTimer.Elapsed += SecondTimer_Elapsed;
            SecondTimer.Start();

            btBeginDateSelect = FindViewById<Button>(Resource.Id.btBeginDateSelect);
            btEndDateSelect = FindViewById<Button>(Resource.Id.btEndDateSelect);
            btKanbanSelect = FindViewById<Button>(Resource.Id.btKanbanSelect);
            snLineList = FindViewById<Spinner>(Resource.Id.snLineList);
            snLangList = FindViewById<Spinner>(Resource.Id.snLangList);
            snFacList = FindViewById<Spinner>(Resource.Id.snFacList);

            tvBeginDate = FindViewById<TextView>(Resource.Id.tvBeginDate);
            tvEndDate = FindViewById<TextView>(Resource.Id.tvEndDate);
            tvFromDate = FindViewById<TextView>(Resource.Id.tvFromDate);
            tvToDate = FindViewById<TextView>(Resource.Id.tvToDate);
            tvServer = FindViewById<TextView>(Resource.Id.tvServer);

            tvWFT = FindViewById<TextView>(Resource.Id.tvWFT);
            tvSelectedLine = FindViewById<TextView>(Resource.Id.tvSelectedLine);
            tvDT = FindViewById<TextView>(Resource.Id.tvDT);
            ActualEff = FindViewById<TextView>(Resource.Id.tvActualEff);
            ActualOutput = FindViewById<TextView>(Resource.Id.tvActualPCS);
            ActualRFT = FindViewById<TextView>(Resource.Id.tvActualRFT);
            InLineDF = FindViewById<TextView>(Resource.Id.tvInLineDF);
            tvMyClock = FindViewById<TextView>(Resource.Id.tvMyClock);
            tvSMV = FindViewById<TextView>(Resource.Id.tvSMV);
            tvRunDay = FindViewById<TextView>(Resource.Id.tvRunDay);
            tvProductType = FindViewById<TextView>(Resource.Id.tvProductType);
            tvTargetPH = FindViewById<TextView>(Resource.Id.tvTargetPerH);

            tv1hr = FindViewById<TextView>(Resource.Id.tv1hr);
            tv2hr = FindViewById<TextView>(Resource.Id.tv2hr);
            tv3hr = FindViewById<TextView>(Resource.Id.tv3hr);
            tv4hr = FindViewById<TextView>(Resource.Id.tv4hr);
            tv5hr = FindViewById<TextView>(Resource.Id.tv5hr);
            tv6hr = FindViewById<TextView>(Resource.Id.tv6hr);
            tv7hr = FindViewById<TextView>(Resource.Id.tv7hr);
            tv8hr = FindViewById<TextView>(Resource.Id.tv8hr);
            tv9hr = FindViewById<TextView>(Resource.Id.tv9hr);
            tv10hr = FindViewById<TextView>(Resource.Id.tv10hr);
            tv11hr = FindViewById<TextView>(Resource.Id.tv11hr);
            tv12hr = FindViewById<TextView>(Resource.Id.tv12hr);

            ActualEff1 = FindViewById<TextView>(Resource.Id.edActualEff1);
            ActualEff2 = FindViewById<TextView>(Resource.Id.edActualEff2);
            ActualEff3 = FindViewById<TextView>(Resource.Id.edActualEff3);
            ActualEff4 = FindViewById<TextView>(Resource.Id.edActualEff4);
            ActualEff5 = FindViewById<TextView>(Resource.Id.edActualEff5);
            ActualEff6 = FindViewById<TextView>(Resource.Id.edActualEff6);
            ActualEff7 = FindViewById<TextView>(Resource.Id.edActualEff7);
            ActualEff8 = FindViewById<TextView>(Resource.Id.edActualEff8);
            ActualEff9 = FindViewById<TextView>(Resource.Id.edActualEff9);
            ActualEff10 = FindViewById<TextView>(Resource.Id.edActualEff10);
            ActualEff11 = FindViewById<TextView>(Resource.Id.edActualEff11);
            ActualEff12 = FindViewById<TextView>(Resource.Id.edActualEff12);

            ActualPCS1 = FindViewById<TextView>(Resource.Id.edActualPCS1);
            ActualPCS2 = FindViewById<TextView>(Resource.Id.edActualPCS2);
            ActualPCS3 = FindViewById<TextView>(Resource.Id.edActualPCS3);
            ActualPCS4 = FindViewById<TextView>(Resource.Id.edActualPCS4);
            ActualPCS5 = FindViewById<TextView>(Resource.Id.edActualPCS5);
            ActualPCS6 = FindViewById<TextView>(Resource.Id.edActualPCS6);
            ActualPCS7 = FindViewById<TextView>(Resource.Id.edActualPCS7);
            ActualPCS8 = FindViewById<TextView>(Resource.Id.edActualPCS8);
            ActualPCS9 = FindViewById<TextView>(Resource.Id.edActualPCS9);
            ActualPCS10 = FindViewById<TextView>(Resource.Id.edActualPCS10);
            ActualPCS11 = FindViewById<TextView>(Resource.Id.edActualPCS11);
            ActualPCS12 = FindViewById<TextView>(Resource.Id.edActualPCS12);

            ActualRFT1 = FindViewById<TextView>(Resource.Id.edActualRFT1);
            ActualRFT2 = FindViewById<TextView>(Resource.Id.edActualRFT2);
            ActualRFT3 = FindViewById<TextView>(Resource.Id.edActualRFT3);
            ActualRFT4 = FindViewById<TextView>(Resource.Id.edActualRFT4);
            ActualRFT5 = FindViewById<TextView>(Resource.Id.edActualRFT5);
            ActualRFT6 = FindViewById<TextView>(Resource.Id.edActualRFT6);
            ActualRFT7 = FindViewById<TextView>(Resource.Id.edActualRFT7);
            ActualRFT8 = FindViewById<TextView>(Resource.Id.edActualRFT8);
            ActualRFT9 = FindViewById<TextView>(Resource.Id.edActualRFT9);
            ActualRFT10 = FindViewById<TextView>(Resource.Id.edActualRFT10);
            ActualRFT11 = FindViewById<TextView>(Resource.Id.edActualRFT11);
            ActualRFT12 = FindViewById<TextView>(Resource.Id.edActualRFT12);

            InLineDF1 = FindViewById<TextView>(Resource.Id.edInLineDF1);
            InLineDF2 = FindViewById<TextView>(Resource.Id.edInLineDF2);
            InLineDF3 = FindViewById<TextView>(Resource.Id.edInLineDF3);
            InLineDF4 = FindViewById<TextView>(Resource.Id.edInLineDF4);
            InLineDF5 = FindViewById<TextView>(Resource.Id.edInLineDF5);
            InLineDF6 = FindViewById<TextView>(Resource.Id.edInLineDF6);

            InlineDFList = FindViewById<TextView>(Resource.Id.tvTopInline);
            EndlineDFList = FindViewById<TextView>(Resource.Id.tvTopEndline);
            tvshowpercent = FindViewById<TextView>(Resource.Id.tvshowpercent);

            chartView = FindViewById<ChartView>(Resource.Id.chartView1);
            chartView2 = FindViewById<ChartView>(Resource.Id.chartView2);

            TargetTotalPCS = FindViewById<TextView>(Resource.Id.edTargetTotalPCS);
            ActualTotalPCS = FindViewById<TextView>(Resource.Id.edActualTotalPCS);
            TotalRJ = FindViewById<TextView>(Resource.Id.edTotalRJPCS);
            tvChartTitle = FindViewById<TextView>(Resource.Id.tvChartTitle); tvChartTitle.Text += "  " + CSDL.version;
            txtlinewip = FindViewById<TextView>(Resource.Id.tvlinewip);
            tvlastrefresh = FindViewById<TextView>(Resource.Id.tvlastrefresh);

            chkauto = FindViewById<CheckBox>(Resource.Id.chkauto);

            piechart1 = FindViewById<PlotView>(Resource.Id.chart1);
            piechart2 = FindViewById<PlotView>(Resource.Id.chart2);

            logo = FindViewById<ImageView>(Resource.Id.logo);
            #endregion

            chkauto.Click += delegate
              {
                  if (chkauto.Checked) autoload = true;
                  else autoload = false;
              };

            //initialize for screen stretching
            var metric = Resources.DisplayMetrics;
            CSDL.width = metric.WidthPixels;
            CSDL.height = metric.HeightPixels;
            CSDL.density = Resources.DisplayMetrics.Density;
            CSDL.SizingScrRt = CSDL.width / (CSDL.density * 1024); //CSDL.density *
            CSDL.SizingScrRtH = CSDL.height / (CSDL.density * 512);
            CSDL.TextRatio = 0.9;
            //if (CSDL.SizingScrRt < 1) CSDL.TextRatio = CSDL.SizingScrRt / CSDL.density;
            //else CSDL.TextRatio = (CSDL.SizingScrRt - CSDL.density * 0.25) / CSDL.density;

            //Toast.MakeText(this, "Res=" + CSDL.width + "x" + CSDL.height + " | Density=" + Resources.DisplayMetrics.Density + " | TextRatio=" + CSDL.TextRatio + " | SizingScrRt=" + CSDL.SizingScrRt + "\n" + CSDL.chuoi.Split(";")[0], ToastLength.Short).Show();

            //resizing layout
            var lvVwGroup = (ViewGroup)FindViewById<RelativeLayout>(Resource.Id.rlMainlayout);
            CSDL.ScreenStretching(1, lvVwGroup);

            Permission read = ContextCompat.CheckSelfPermission(Application.Context, Manifest.Permission.ReadExternalStorage);
            Permission write = ContextCompat.CheckSelfPermission(Application.Context, Manifest.Permission.WriteExternalStorage);

            if (read != Permission.Granted) RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage, }, 0);
            if (write != Permission.Granted) RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage, }, 0);

            try
            {
                myAppInitializer();
            }
            catch (Exception Ex)
            {
                Toast.MakeText(this, "InitializeData : dtinfo \n" + Ex.ToString(), ToastLength.Long).Show();
            }
            snFacList.ItemSelected += (sender, e) =>
            {
                try
                {
                    Spinner sp = (Spinner)sender;
                    Fac = sp.GetItemAtPosition(e.Position).ToString();
                    string ch = "select distinct FacLine from cpdtlsdays where FacLine like '" + Fac + "%' and ProDate between dateadd(day, -8, '" + CSDL.BeginDate.ToString() + "') and '" + CSDL.EndDate.ToString() + "' and FacLine !='' order by FacLine asc";
                    DataTable myLines = CSDL.Cnnt.Doc(ch).Tables[0];

                    AddLinetoList(myLines);

                    ISharedPreferences pre = GetSharedPreferences("FacSel", FileCreationMode.Private);
                    ISharedPreferencesEditor editor = pre.Edit();
                    editor.Clear();
                    editor.PutString("Fac", Fac);
                    editor.Apply();

                    Get_Time();
                }
                catch { }
            };
            ActualEff.Click += (sender, e) =>
            {
                string ImgName = "";
                try
                {
                    ImgName = CSDL.Cnnt.Doc("select top 1 [ImageSource] from InlineLBImageSource where FacLine = '" + CSDL.SelectedLine + "'").Tables[0].Rows[0][0].ToString();

                    A1ATeam.ShowImage show = new A1ATeam.ShowImage(ImgURL + ImgName);
                    show.ShowIDE();
                }
                catch
                {
                    Toast.MakeText(this, "Error: loading image fail !", ToastLength.Long).Show();
                }

            };
            ActualEff.LongClick += delegate
            {
                string ImgName = "";
                try
                {
                    ImgName = CSDL.Cnnt.Doc("select top 1 [ImageSource] from InlineLBImageSource where FacLine = '" + CSDL.SelectedLine + "'").Tables[0].Rows[0][0].ToString();

                    Android.App.AlertDialog.Builder b = new Android.App.AlertDialog.Builder(this, Android.Resource.Style.ThemeBlackNoTitleBarFullScreen);

                    ImageView ImV = new ImageView(this);

                    WebClient web = new WebClient();
                    byte[] ImageByteArray = web.DownloadData(ImgURL + ImgName);
                    var bitmap = BitmapFactory.DecodeByteArray(ImageByteArray, 0, ImageByteArray.Length);
                    ImV.SetImageBitmap(bitmap);

                    b.SetView(ImV);
                    Dialog d = b.Create();
                    d.Show();

                    ImV.Click += (s, r) =>
                    {
                        d.Dismiss();
                    };
                }
                catch
                {
                    Toast.MakeText(this, "Error: loading image fail !", ToastLength.Long).Show();
                }
            };

            snLineList.ItemSelected += (sender, e) =>
            {
                CSDL.SelectedLine = "";
                Spinner spinner = (Spinner)sender;
                CSDL.SelectedLine = spinner.GetItemAtPosition(e.Position).ToString();
                tvSelectedLine.Text = CSDL.SelectedLine;
                mySpinerPositionID = (int)spinner.SelectedItemId;
                CSDL.checkopen = 0;

                //if (!string.IsNullOrEmpty(WrkHrId))
                //{
                //    string str = " create table #t (WrkHrId int, ShiftId int) "
                //                + " Declare @Sql nvarchar(max) "
                //                + " set @Sql = 'select WrkHrId, ' + DATENAME(weekday, getdate()) + ' as ShiftId from InlineWrkHrSection where SectionNm = ''" + CSDL.SelectedLine + "'' and WrkHrId = ''" + WrkHrId + "'''"
                //                + " insert into #t EXEC sp_executesql @Sql"
                //                + " select top 1 [WrkHr01],[WrkHr02],[WrkHr03],[WrkHr04],[WrkHr05],[WrkHr06],[WrkHr07],[WrkHr08],[WrkHr09],[WrkHr10],[WrkHr11],[WrkHr12],[WrkHr13],[WrkHr14] "
                //                + " from InlineWrkHrMaster a right join #t b on a.WrkHrId = b.WrkHrId and a.ShiftId = b.ShiftId "
                //                + " drop table #t ";
                //    DataTable dt = CSDL.Cnnt.Doc(str).Tables[0];
                //    if (dt.Rows.Count > 0) for (int i = 0; i < 14; i++) CSDL.TimeArray[i] = string.IsNullOrEmpty(dt.Rows[0][i].ToString()) ? 2500 : int.Parse(dt.Rows[0][i].ToString());
                //}


                LoadInfoVideo();
                RefreshingView();


                ISharedPreferences pre = GetSharedPreferences("LineSel", FileCreationMode.Private);
                ISharedPreferencesEditor editor = pre.Edit();
                editor.Clear();
                editor.PutString("Line", CSDL.SelectedLine);
                editor.Apply();
            };
            snLangList.ItemSelected += (sender, e) =>
            {
                Spinner spinner = (Spinner)sender;
                string strLang = spinner.GetItemAtPosition(e.Position).ToString();

                DataRow[] drLa = myLineList.Tables[1].Select("Language='" + strLang + "'");
                if (drLa.Length > 0)
                {
                    DataRow dRw = drLa[0];
                    CSDL.LangRef = string.IsNullOrEmpty(dRw[1].ToString()) ? 1 : int.Parse(dRw[1].ToString());
                }

                CSDL.ltLang.Clear();
                foreach (DataRow drT in myLineList.Tables[2].Rows)
                {
                    CSDL.ltLang.Add(new MultiDimensionList { FirstText = drT[0].ToString(), Ref_Val1 = drT[CSDL.LangRef].ToString(), Ref_Val2 = drT[2].ToString() });
                }

                //mToast(CSDL.ltLang.Count.ToString(), 5000);

                ISharedPreferences pre = GetSharedPreferences("LangSel", FileCreationMode.Private);
                ISharedPreferencesEditor editor = pre.Edit();
                editor.Clear();
                editor.PutString("Lang", strLang);
                editor.Apply();

                //lbTitle.Text = CSDL.ltLang[CSDL.ltLang.FindIndex(x => x.ShCTN_ClPOorCTNCode == "M00001")].ShCTN_ClQtyorCTNNo;
                tvFromDate.Text = CSDL.Language("M00118");
                tvToDate.Text = CSDL.Language("M00119");
                tvServer.Text = CSDL.Language("M00007");
            };

            tvServer.Click += delegate
            {
                var itent = new Intent(this, typeof(ServerActivity));
                StartActivity(itent);
                Finish();
            };
            tvSelectedLine.Click += (sender, e) => RefreshingView();
            tvSelectedLine.LongClick += (sender, e) => tvWFT.Visibility = Android.Views.ViewStates.Visible;
            chartView.LongClick += (sender, e) => chartView2.Visibility = Android.Views.ViewStates.Visible;
            chartView2.LongClick += (sender, e) => chartView2.Visibility = Android.Views.ViewStates.Gone;
            chartView.Click += (sender, e) => tvshowpercent.Visibility = Android.Views.ViewStates.Visible;
            chartView2.Click += (sender, e) => tvshowpercent.Visibility = Android.Views.ViewStates.Visible;
            tvshowpercent.Click += (sender, e) => tvshowpercent.Visibility = Android.Views.ViewStates.Gone;
            //call image retriever

            InlineDFList.Click += delegate
             {
                 try
                 {
                     CSDL.ImageListInfo = CSDL.Language("M00123") + " " + tvSelectedLine.Text + " : " + CSDL.Language("M00120") + " " + CSDL.Language("M00118") + " " + tvBeginDate.Text + " " + CSDL.Language("M00119") + " " + tvEndDate.Text;
                     Toast.MakeText(this, "..." + CSDL.Language("M00121") + "... ", ToastLength.Long).Show();
                     CSDL.url = CSDL.ILIMUrl;
                     CSDL.in_or_end = 1;
                     var itent = new Intent(this, typeof(ImageRetrive));
                     StartActivity(itent);
                     //Finish();
                 }
                 catch (Exception Ex)
                 {
                     Toast.MakeText(this, CSDL.Language("M00018") + " /n" + Ex.ToString(), ToastLength.Long).Show();
                 }
             };

            EndlineDFList.Click += delegate
              {
                  try
                  {
                      CSDL.ImageListInfo = CSDL.Language("M00122") + " " + tvSelectedLine.Text + " : " + CSDL.Language("M00120") + " " + CSDL.Language("M00118") + " " + tvBeginDate.Text + " " + CSDL.Language("M00119") + " " + tvEndDate.Text;
                      Toast.MakeText(this, "..." + CSDL.Language("M00121") + "... ", ToastLength.Long).Show();
                      CSDL.url = CSDL.ELIMUrl;
                      CSDL.in_or_end = 2;
                      var itent = new Intent(this, typeof(ImageRetrive));
                      StartActivity(itent);
                      //Finish();
                  }
                  catch (Exception Ex)
                  {
                      Toast.MakeText(this, CSDL.Language("M00018") + " /n" + Ex.ToString(), ToastLength.Long).Show();
                  }
              };

            piechart1.Click += delegate
             {
                 try
                 {
                     CSDL.ImageListInfo = CSDL.Language("M00123") + " " + tvSelectedLine.Text + " : " + CSDL.Language("M00120") + " " + CSDL.Language("M00118") + " " + tvBeginDate.Text + " " + CSDL.Language("M00119") + " " + tvEndDate.Text;
                     Toast.MakeText(this, "..." + CSDL.Language("M00121") + "... ", ToastLength.Long).Show();
                     CSDL.url = CSDL.ILIMUrl;
                     CSDL.in_or_end = 1;
                     var itent = new Intent(this, typeof(ImageRetrive));
                     StartActivity(itent);
                     //Finish();
                 }
                 catch (Exception Ex)
                 {
                     Toast.MakeText(this, CSDL.Language("M00018") + " /n" + Ex.ToString(), ToastLength.Long).Show();
                 }
             };

            piechart2.Click += delegate
            {
                try
                {
                    CSDL.ImageListInfo = CSDL.Language("M00122") + " " + tvSelectedLine.Text + " : " + CSDL.Language("M00120") + " " + CSDL.Language("M00118") + " " + tvBeginDate.Text + " " + CSDL.Language("M00119") + " " + tvEndDate.Text;
                    Toast.MakeText(this, "..." + CSDL.Language("M00121") + "... ", ToastLength.Long).Show();
                    CSDL.url = CSDL.ELIMUrl;
                    CSDL.in_or_end = 2;
                    var itent = new Intent(this, typeof(ImageRetrive));
                    StartActivity(itent);
                    //Finish();
                }
                catch (Exception Ex)
                {
                    Toast.MakeText(this, CSDL.Language("M00018") + " /n" + Ex.ToString(), ToastLength.Long).Show();
                }
            };

            btBeginDateSelect.Click += delegate
            {
                BeginOrEnd = true;
                DateSelect_OnClick();
            };
            btEndDateSelect.Click += delegate
            {
                BeginOrEnd = false;
                DateSelect_OnClick();
            };
            btKanbanSelect.Click += delegate
            {
                try
                {
                    //A1ATeam.LayoutRequest rq = new LayoutRequest(1280, 752, 1);

                    Android.App.AlertDialog.Builder b = new Android.App.AlertDialog.Builder(this);
                    Dialog d = new Dialog(this);

                    ScrollView main = new ScrollView(this);

                    LinearLayout ln = new LinearLayout(this) { Orientation = Orientation.Vertical }; main.AddView(ln);
                    ln.SetGravity(GravityFlags.Center);

                    ViewGroup.MarginLayoutParams mr = new ViewGroup.MarginLayoutParams(rq.eWidht(500), ViewGroup.LayoutParams.WrapContent);
                    mr.SetMargins(rq.eWidht(10), rq.eWidht(10), 0, 0);

                    Button kanban = new Button(this) { Text = "KANBAN", LayoutParameters = mr }; ln.AddView(kanban);
                    Button balance = new Button(this) { Text = "LINE INPUT/OUTPUT", LayoutParameters = mr }; ln.AddView(balance);
                    Button wip = new Button(this) { Text = "LINE WIP", LayoutParameters = mr }; ln.AddView(wip);
                    Button bottle = new Button(this) { Text = "BOTTLENECK", LayoutParameters = mr }; ln.AddView(bottle);
                    Button inline = new Button(this) { Text = "INLINE TRAFFIC LIGHT", LayoutParameters = mr }; ln.AddView(inline);
                    Button trend = new Button(this) { Text = "DEFECT TRENDS", LayoutParameters = mr }; ln.AddView(trend);
                    Button stw = new Button(this) { Text = "STW CHECKLIST", LayoutParameters = mr }; ln.AddView(stw);

                    kanban.Click += async delegate
                    {
                        Toast.MakeText(this, "Openning The Kanban ....", ToastLength.Long).Show();
                        await Task.Delay(1000);
                        //Toast.MakeText(this, "..." + CSDL.Language("M00124") + "...", ToastLength.Long).Show();
                        var itent = new Intent(this, typeof(Kanban));
                        StartActivity(itent);

                        d.Dismiss();
                    };
                    kanban.LongClick += async delegate
                    {
                        Toast.MakeText(this, "Openning The Kanban List ....", ToastLength.Long).Show();
                        await Task.Delay(1000);

                        DateTime date = new DateTime();

                        if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday) date = DateTime.Now.AddDays(2);
                        else date = DateTime.Now.AddDays(1);

                        string ng = date.ToString("yyyyMMdd");
                        Android.App.AlertDialog.Builder b = new Android.App.AlertDialog.Builder(this);

                        HorizontalScrollView main = new HorizontalScrollView(this);

                        LinearLayout ln = new LinearLayout(this) { Orientation = Orientation.Vertical }; main.AddView(ln);

                        LinearLayout h = new LinearLayout(this) { Orientation = Orientation.Horizontal }; ln.AddView(h);

                        TextView txt = new TextView(this) { Text = "Kanban on :   " }; h.AddView(txt);
                        EditText ed = new EditText(this) { Text = date.ToString("dd/MM/yyyy"), LayoutParameters = new ViewGroup.LayoutParams(300, ViewGroup.LayoutParams.WrapContent), TextAlignment = TextAlignment.Center };
                        ed.Focusable = false; h.AddView(ed);
                        Button btn = new Button(this) { Text = "RUN" }; h.AddView(btn);

                        ListView list = new ListView(this); ln.AddView(list);

                        LoadList(ng);

                        b.SetView(main);
                        b.Create().Show();

                        ed.Click += delegate
                        {
                            DatePickerDialog dialog = new DatePickerDialog(this);
                            dialog.DateSet += (s, a) =>
                            {
                                ed.Text = a.Date.ToString("dd/MM/yyyy");
                                ng = a.Date.ToString("yyyyMMdd");
                                dialog.Dismiss();
                            };
                            dialog.Show();
                        };
                        btn.Click += delegate { LoadList(ng); };
                        void LoadList(string d)
                        {
                            DataTable dt = kn.Doc("exec GetLoadData 74,'" + d + "','" + Fac.Substring(Fac.Length - 2, 2) + "'").Tables[0];

                            list.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(dt, new List<int> { rq.eWidht(100), rq.eWidht(200), rq.eWidht(200), rq.eWidht(100), rq.eWidht(200), rq.eWidht(100) }, true, false)
                            {
                                RowSetColor = new List<ListHiline> { new ListHiline { Condition = new List<string> { "" }, RowColor = Color.Red, RowIndex = 1 } }
                            };
                        }
                    };
                    balance.Click += async delegate
                      {
                          Toast.MakeText(this, "Openning Line WIP ....", ToastLength.Long).Show();
                          await Task.Delay(1000);

                          Android.App.AlertDialog.Builder b1 = new Android.App.AlertDialog.Builder(this);

                          DataTable dt1 = kn.Doc("exec EndlineSewingLineBalacing 2,'" + Fac + "','" + CSDL.SelectedLine + "'", 120).Tables[0];
                          Toast.MakeText(this, dt1.Rows.Count.ToString(), ToastLength.Long).Show();

                          HorizontalScrollView mn = new HorizontalScrollView(this);
                          ListView ls1 = new ListView(this); mn.AddView(ls1);

                          ls1.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(dt1, new List<int> { rq.eWidht(100),rq.eWidht(200), rq.eWidht(200), rq.eWidht(200), rq.eWidht(100),
                              rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100) }, true)
                          {
                              TextSize = rq.eTextSize(18)
                          };

                          b1.SetView(mn);
                          Dialog dd = b1.Create();
                          dd.Show(); dd.Window.SetLayout(rq.eWidht(1240), rq.eHeight(800));

                          d.Dismiss();
                      };
                    wip.Click += async delegate
                    {
                        Toast.MakeText(this, "Analyzing Line WIP data ....", ToastLength.Long).Show();
                        await Task.Delay(1000);
                        Timer tm = new Timer();
                        tm.Interval = 2 * 60 * 1000;

                        Android.App.AlertDialog.Builder b1 = new Android.App.AlertDialog.Builder(this, Android.Resource.Style.ThemeLightNoTitleBarFullScreen);
                        Android.App.AlertDialog.Builder b2 = new Android.App.AlertDialog.Builder(this);

                        GradientDrawable border = new GradientDrawable();
                        //border.SetColor(Color.White);
                        border.SetStroke(2, Color.Blue);

                        ScrollView srm = new ScrollView(this);
                        LinearLayout main = new LinearLayout(this) { Orientation = Orientation.Horizontal }; main.SetBackgroundColor(Color.ParseColor("#B0C4DE")); srm.AddView(main);

                        LoadLayout();
                        tm.Start();

                        //b1.SetPositiveButton("BACK", (ss, aa) => { });

                        b1.SetView(srm);
                        b1.Create().Show();

                        tm.Elapsed += delegate
                        {
                            RunOnUiThread(() =>
                            {
                                //if (main.ChildCount > 0) main.RemoveAllViews();
                                LoadLayout();
                            });
                        };

                        async void LoadLayout()
                        {
                            Toast.MakeText(this, "Refreshing ....", ToastLength.Long).Show();
                            await Task.Delay(1000);

                            DataSet ds = kn.Doc("exec EndlineSewingLineBalacing 3,'" + Fac + "',''");

                            int limit = 1, dem = 1;
                            foreach (DataTable dt in ds.Tables)
                            {
                                if (limit < dt.Rows.Count && dem <= 5) limit = dt.Rows.Count;

                                dem++;
                            }

                            ViewGroup.MarginLayoutParams mr = new ViewGroup.MarginLayoutParams(rq.eWidht(240), rq.eWidht(110 * limit));
                            ViewGroup.MarginLayoutParams mr1 = new ViewGroup.MarginLayoutParams(rq.eWidht(200), ViewGroup.LayoutParams.WrapContent);
                            mr1.SetMargins(rq.eWidht(20), rq.eHeight(10), 0, 0);

                            if (main.ChildCount > 0) main.RemoveAllViews();

                            DataRow d = ds.Tables[5].Rows[0];

                            LinearLayout main1 = new LinearLayout(this) { Orientation = Orientation.Vertical, LayoutParameters = mr, Background = border }; main.AddView(main1);
                            TextView tv1 = new TextView(this) { Text = d[0].ToString(), TextAlignment = TextAlignment.Center, TextSize = rq.eTextSize(20), LayoutParameters = mr1 }; tv1.SetTextColor(Color.DarkViolet);//"WIP by hour < 1"
                            tv1.SetBackgroundColor(Color.White); main1.AddView(tv1); tv1.Click += delegate { ShowAllInfor(); };
                            LinearLayout main2 = new LinearLayout(this) { Orientation = Orientation.Vertical, LayoutParameters = mr, Background = border }; main.AddView(main2);
                            TextView tv2 = new TextView(this) { Text = d[1].ToString(), TextAlignment = TextAlignment.Center, TextSize = rq.eTextSize(20), LayoutParameters = mr1 }; tv2.SetTextColor(Color.DarkViolet);//"WIP by hour 1->2"
                            tv2.SetBackgroundColor(Color.White); main2.AddView(tv2); tv2.Click += delegate { ShowAllInfor(); };
                            LinearLayout main3 = new LinearLayout(this) { Orientation = Orientation.Vertical, LayoutParameters = mr, Background = border }; main.AddView(main3);
                            TextView tv3 = new TextView(this) { Text = d[2].ToString(), TextAlignment = TextAlignment.Center, TextSize = rq.eTextSize(20), LayoutParameters = mr1 }; tv3.SetTextColor(Color.DarkViolet);//"WIP by hour 2->3"
                            tv3.SetBackgroundColor(Color.White); main3.AddView(tv3); tv3.Click += delegate { ShowAllInfor(); };
                            LinearLayout main4 = new LinearLayout(this) { Orientation = Orientation.Vertical, LayoutParameters = mr, Background = border }; main.AddView(main4);
                            TextView tv4 = new TextView(this) { Text = d[3].ToString(), TextAlignment = TextAlignment.Center, TextSize = rq.eTextSize(20), LayoutParameters = mr1 }; tv4.SetTextColor(Color.DarkViolet);//"WIP by hour 3->5"
                            tv4.SetBackgroundColor(Color.White); main4.AddView(tv4); tv4.Click += delegate { ShowAllInfor(); };
                            LinearLayout main5 = new LinearLayout(this) { Orientation = Orientation.Vertical, LayoutParameters = mr, Background = border }; main.AddView(main5);
                            TextView tv5 = new TextView(this) { Text = d[4].ToString(), TextAlignment = TextAlignment.Center, TextSize = rq.eTextSize(20), LayoutParameters = mr1 }; tv5.SetTextColor(Color.DarkViolet);//"WIP by hour >= 5"
                            tv5.SetBackgroundColor(Color.White); main5.AddView(tv5); tv5.Click += delegate { ShowAllInfor(); };

                            foreach (DataRow r in ds.Tables[0].Rows) main1.AddView(LineItem(r, Color.White, Color.Black));
                            foreach (DataRow r in ds.Tables[1].Rows) main2.AddView(LineItem(r, Color.Red, Color.White));
                            foreach (DataRow r in ds.Tables[2].Rows) main3.AddView(LineItem(r, Color.Yellow, Color.Black));
                            foreach (DataRow r in ds.Tables[3].Rows) main4.AddView(LineItem(r, Color.Green, Color.Black));
                            foreach (DataRow r in ds.Tables[4].Rows) main5.AddView(LineItem(r, Color.Blue, Color.White));

                            void ShowAllInfor()
                            {
                                Android.App.AlertDialog.Builder b = new Android.App.AlertDialog.Builder(this);

                                HorizontalScrollView m = new HorizontalScrollView(this);
                                ListView l = new ListView(this); m.AddView(l);

                                l.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(ds.Tables[6], new List<int> { rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100),
                                    rq.eWidht(100), rq.eWidht(110), rq.eWidht(110), rq.eWidht(300) }, true)
                                { TextSize = rq.eTextSize(20) };

                                b.SetView(m);
                                b.Create().Show();

                                l.ItemClick += async (s, a) =>
                                {
                                    if (a.Position > 0)
                                    {
                                        Toast.MakeText(this, "Openning Detail ....", ToastLength.Long).Show();
                                        await Task.Delay(1000);

                                        LinearLayout v = A1ATeam.CustomListView.GetViewByPosition(a.Position, l) as LinearLayout;

                                        Android.App.AlertDialog.Builder b2 = new Android.App.AlertDialog.Builder(this);
                                        string f = ((TextView)v.GetChildAt(0)).Text;

                                        DataTable d = kn.Doc("exec GetDataFromQuery 48,'" + f + "','1','','',''").Tables[0];
                                        d.Columns[0].ColumnName = "PONO - " + f;

                                        HorizontalScrollView m2 = new HorizontalScrollView(this);
                                        ListView l2 = new ListView(this); m2.AddView(l2);

                                        l2.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(d, new List<int> { rq.eWidht(300), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100) }, true, false)
                                        { TextSize = rq.eTextSize(20) };

                                        b2.SetView(m2);
                                        b2.Create().Show();
                                    }
                                };
                            }
                        }
                    };
                    bottle.Click += async delegate
                    {
                        Toast.MakeText(this, "Openning Bottleneck Monitor ....", ToastLength.Long).Show();
                        await Task.Delay(1000);

                        BottleNeckMonitor(); ShowBottleneck = true;

                        d.Dismiss();
                    };
                    inline.Click += async delegate
                    {
                        Toast.MakeText(this, "Openning Inline Traffic Light ....", ToastLength.Long).Show();
                        await Task.Delay(1000);

                        Android.App.AlertDialog.Builder b1 = new Android.App.AlertDialog.Builder(this);

                        DataTable dt1 = kn.Doc("exec InlineQcTLFlag2 '" + CSDL.BeginDate + "','" + CSDL.SelectedLine + "'", 120).Tables[0];
                        Toast.MakeText(this, dt1.Rows.Count.ToString(), ToastLength.Long).Show();

                        HorizontalScrollView mn = new HorizontalScrollView(this);

                        LinearLayout ln = new LinearLayout(this) { Orientation = Orientation.Vertical }; mn.AddView(ln);
                        LinearLayout ln1 = new LinearLayout(this) { Orientation = Orientation.Horizontal }; ln.AddView(ln1);
                        LinearLayout ln2 = new LinearLayout(this); ln.AddView(ln2);

                        DataRow r = dt1.Rows[0];
                        var id = new TextView(this)
                        { Text = dt1.Columns[0].ColumnName, LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(120), ViewGroup.LayoutParams.WrapContent), TextSize = rq.eTextSize(18) };
                        id.SetBackgroundColor(Color.Blue); id.SetTextColor(Color.White); id.TextAlignment = TextAlignment.Center; ln1.AddView(id);
                        var tf01 = new TextView(this)
                        { Text = dt1.Columns[1].ColumnName, LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(50), ViewGroup.LayoutParams.WrapContent), TextSize = rq.eTextSize(18) };
                        tf01.SetBackgroundColor(Color.Blue); tf01.SetTextColor(Color.White); tf01.TextAlignment = TextAlignment.Center; ln1.AddView(tf01);
                        var tf02 = new TextView(this)
                        { Text = dt1.Columns[2].ColumnName, LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(50), ViewGroup.LayoutParams.WrapContent), TextSize = rq.eTextSize(18) };
                        tf02.SetBackgroundColor(Color.Blue); tf02.SetTextColor(Color.White); tf02.TextAlignment = TextAlignment.Center; ln1.AddView(tf02);
                        var tf03 = new TextView(this)
                        { Text = dt1.Columns[3].ColumnName, LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(50), ViewGroup.LayoutParams.WrapContent), TextSize = rq.eTextSize(18) };
                        tf03.SetBackgroundColor(Color.Blue); tf03.SetTextColor(Color.White); tf03.TextAlignment = TextAlignment.Center; ln1.AddView(tf03);
                        var tf04 = new TextView(this)
                        { Text = dt1.Columns[4].ColumnName, LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(50), ViewGroup.LayoutParams.WrapContent), TextSize = rq.eTextSize(18) };
                        tf04.SetBackgroundColor(Color.Blue); tf04.SetTextColor(Color.White); tf04.TextAlignment = TextAlignment.Center; ln1.AddView(tf04);
                        var tf05 = new TextView(this)
                        { Text = dt1.Columns[5].ColumnName, LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(50), ViewGroup.LayoutParams.WrapContent), TextSize = rq.eTextSize(18) };
                        tf05.SetBackgroundColor(Color.Blue); tf05.SetTextColor(Color.White); tf05.TextAlignment = TextAlignment.Center; ln1.AddView(tf05);
                        var tf06 = new TextView(this)
                        { Text = dt1.Columns[6].ColumnName, LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(50), ViewGroup.LayoutParams.WrapContent), TextSize = rq.eTextSize(18) };
                        tf06.SetBackgroundColor(Color.Blue); tf06.SetTextColor(Color.White); tf06.TextAlignment = TextAlignment.Center; ln1.AddView(tf06);
                        var oper = new TextView(this)
                        { Text = dt1.Columns[8].ColumnName, LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(500), ViewGroup.LayoutParams.WrapContent), TextSize = rq.eTextSize(18) };
                        oper.SetBackgroundColor(Color.Blue); oper.SetTextColor(Color.White); oper.TextAlignment = TextAlignment.Center; ln1.AddView(oper);


                        ListView ls1 = new ListView(this); ln2.AddView(ls1);

                        string customer = kn.Doc("exec GetDataFromQuery 32,'" + CSDL.SelectedLine.Substring(0, 2) + "','" + CSDL.SelectedLine + "','','',''").Tables[0].Rows[0][0].ToString();

                        if (CSDL.TLSCD.Rows.Count > 0) CSDL.TLSCD.Rows.Clear();
                        CSDL.TLSCD = kn.Doc("exec GetLoadData 43,'" + CSDL.BeginDate + "','" + CSDL.SelectedLine + "'").Tables[0];

                        ls1.Adapter = new TrafficLine(dt1, kn, customer);

                        b1.SetView(mn);
                        b1.Create().Show();

                        d.Dismiss();
                    };
                    trend.Click += async delegate
                    {
                        Toast.MakeText(this, "Openning The Defect Trends ....", ToastLength.Long).Show();
                        await Task.Delay(1000);

                        DataSet ds = new DataSet();
                        int itemclick = 0, i = 0;
                        Android.App.AlertDialog.Builder b1 = new Android.App.AlertDialog.Builder(this, Android.Resource.Style.ThemeLightNoTitleBarFullScreen);

                        HorizontalScrollView mn = new HorizontalScrollView(this);

                        LinearLayout ln = new LinearLayout(this) { Orientation = Orientation.Vertical }; mn.AddView(ln);
                        LinearLayout ln1 = new LinearLayout(this) { Orientation = Orientation.Horizontal }; ln.AddView(ln1);
                        LinearLayout ln2 = new LinearLayout(this) { Orientation = Orientation.Horizontal }; ln.AddView(ln2);

                        TextView txt1 = new TextView(this) { Text = "Months : " }; txt1.SetTextColor(Color.White); ln1.AddView(txt1);
                        TextView txt2 = new TextView(this) { Text = "1", LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(50), ViewGroup.LayoutParams.WrapContent) };
                        txt2.SetTextColor(Color.Pink); ln1.AddView(txt2);
                        txt2.Click += delegate { InputValue(txt2); };

                        Button bt = new Button(this) { Text = CSDL.SelectedLine }; ln1.AddView(bt);

                        RadioButton inline = new RadioButton(this) { Text = "INLINE", Checked = true }; inline.SetTextColor(Color.Blue); ln1.AddView(inline);
                        RadioButton endline = new RadioButton(this) { Text = "ENDLINE", Checked = false }; endline.SetTextColor(Color.Blue); ln1.AddView(endline);

                        TextView txt0 = new TextView(this) { Text = "  " }; ln1.AddView(txt0);
                        TextView txt3 = new TextView(this) { Text = "Style : " }; txt3.SetTextColor(Color.White); ln1.AddView(txt3);
                        TextView txt4 = new TextView(this) { Text = "Click here to input Style", LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(300), ViewGroup.LayoutParams.WrapContent) };
                        txt4.SetTextColor(Color.Pink); ln1.AddView(txt4);
                        txt4.Click += delegate
                        {
                            InputValue(txt4, false);
                        };

                        Button btline = new Button(this) { Text = CSDL.SelectedLine }; ln1.AddView(btline);
                        Button btallline = new Button(this) { Text = "All Line" }; ln1.AddView(btallline);

                        //TextView txt00 = new TextView(this) { Text = "  " }; ln1.AddView(txt00);
                        //TextView txt5 = new TextView(this) { Text = "Style : " }; txt5.SetTextColor(Color.White); ln1.AddView(txt5);
                        //TextView txt6 = new TextView(this) { Text = "Click here to input the defect name", LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(500), ViewGroup.LayoutParams.WrapContent) };
                        //txt6.SetTextColor(Color.Pink); ln1.AddView(txt6);
                        //txt6.Click += delegate
                        //{
                        //    SelectDefectName();
                        //};

                        //Button btrun = new Button(this) { Text = "RUN" }; ln1.AddView(btrun);

                        LinearLayout ln3 = new LinearLayout(this) { LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(500), ViewGroup.LayoutParams.WrapContent) }; ln2.AddView(ln3);
                        LinearLayout ln4 = new LinearLayout(this) { LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(700), ViewGroup.LayoutParams.WrapContent) }; ln2.AddView(ln4);

                        ListView dfl = new ListView(this); ln3.AddView(dfl);
                        ChartView ch = new ChartView(this); ln4.AddView(ch);

                        LoadData("exec GetLoadData 47,'" + txt2.Text + "','" + CSDL.SelectedLine + "'");

                        b1.SetView(mn);
                        b1.Create().Show();

                        d.Dismiss();

                        inline.CheckedChange += delegate
                        {
                            if (inline.Checked)
                            {
                                i = 0;
                                endline.Checked = false;

                                ShowData();
                            }
                        };
                        endline.CheckedChange += delegate
                        {
                            if (endline.Checked)
                            {
                                i = 1;
                                inline.Checked = false;

                                ShowData();
                            }
                        };
                        bt.Click += delegate
                        {
                            itemclick = 0;
                            LoadData("exec GetLoadData 47,'" + txt2.Text + "','" + CSDL.SelectedLine + "'");
                        };
                        btline.Click += delegate
                        {
                            itemclick = -1;
                            LoadData("exec GetLoadData 51,'" + txt4.Text.ToUpper() + "','" + CSDL.SelectedLine + "'");
                        };
                        btallline.Click += delegate
                        {
                            itemclick = 1;
                            LoadData("exec GetLoadData 52,'" + txt4.Text.ToUpper() + "',''");
                        };
                        dfl.ItemClick += (s, a) =>
                        {
                            try
                            {
                                if (a.Position > 0)
                                {
                                    DataRow r = ds.Tables[i].Rows[a.Position - 1];

                                    if (itemclick == 0)
                                    {
                                        if (i == 0)
                                        {
                                            DataTable dt = kn.Doc("exec GetDataFromQuery 1,'" + txt2.Text + "','" + CSDL.SelectedLine + "','" + r[0] + "','',''").Tables[0];

                                            ShowDetail(dt, CSDL.SelectedLine + " - QC Inline Defect : " + r[0] + " | " + r[1]);
                                        }
                                        else
                                        {
                                            DataTable dt = kn.Doc("exec GetDataFromQuery 2,'" + txt2.Text + "','" + CSDL.SelectedLine + "','" + r[0] + "','',''").Tables[0];

                                            ShowDetail(dt, CSDL.SelectedLine + " - QC Endline Defect : " + r[0] + " | " + r[1]);
                                        }
                                    }
                                    else if (itemclick == 1)
                                    {
                                        if (i == 0)
                                        {
                                            DataTable dt = kn.Doc("exec GetDataFromQuery 3,'" + txt4.Text + "','" + r[0] + "','','',''").Tables[0];

                                            ShowDetail(dt, txt4.Text + " - QC Inline Defect : " + r[0] + " | " + r[1]);
                                        }
                                        else
                                        {
                                            DataTable dt = kn.Doc("exec GetDataFromQuery 4,'" + txt4.Text + "','" + r[0] + "','','',''").Tables[0];

                                            ShowDetail(dt, txt4.Text + " - QC Endline Defect : " + r[0] + " | " + r[1]);
                                        }
                                    }
                                }
                            }
                            catch { }
                        };

                        void InputValue(TextView txt, bool num = true)
                        {
                            Dialog d = new Dialog(this);
                            Android.App.AlertDialog.Builder b2 = new Android.App.AlertDialog.Builder(this);

                            LinearLayout input = new LinearLayout(this) { Orientation = Orientation.Vertical };
                            EditText ed = new EditText(this) { Hint = "Input the value here", LayoutParameters = new ViewGroup.LayoutParams(300, ViewGroup.LayoutParams.WrapContent) };
                            input.AddView(ed);

                            if (num) ed.InputType = Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberFlagSigned;
                            else ed.InputType = Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextFlagCapCharacters;

                            if (num)
                            {
                                b2.SetPositiveButton("OK", (s, a) =>
                                {
                                    if (ed.Text == "") ed.Text = "1";

                                    txt.Text = ed.Text;
                                });
                            }
                            else
                            {
                                Button xacnhan = new Button(this) { Text = "ADD", LayoutParameters = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent) };
                                input.AddView(xacnhan);

                                ListView lst = new ListView(this); input.AddView(lst);

                                DataTable dtstyle = kn.Doc("exec GetDataFromQuery 5,'" + txt2.Text + "','" + CSDL.SelectedLine + "','','',''").Tables[0];

                                List<string> list = dtstyle.Select().Select(s => s[0].ToString()).ToList();
                                //ed.Text = list[0];

                                ArrayAdapter array = new ArrayAdapter(this, Resource.Layout.select_dialog_singlechoice_material, list);
                                array.SetDropDownViewResource(Resource.Layout.select_dialog_singlechoice_material);

                                ed.TextChanged += (s, a) => { array.Clear(); array.AddAll((ICollection)list.FindAll(d => d.Contains(ed.Text)).ToArray()); };

                                lst.Adapter = array;

                                xacnhan.Click += delegate
                                {
                                    txt.Text = ed.Text;

                                    d.Dismiss();
                                };
                                lst.ItemClick += (s, a) =>
                                {
                                    ed.Text = list[a.Position];
                                };
                            }

                            b2.SetView(input);
                            d = b2.Create();
                            d.Show();
                        }
                        void SelectDefectName()
                        {
                            try
                            {
                                DataTable dt = kn.Doc("exec LoadADSDefectManager").Tables[0];

                                Android.App.AlertDialog.Builder b2 = new Android.App.AlertDialog.Builder(this);

                                LinearLayout main = new LinearLayout(this) { Orientation = Orientation.Vertical };
                                EditText ed = new EditText(this) { Hint = "Input the defect name", LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(400), ViewGroup.LayoutParams.WrapContent) }; main.AddView(ed);
                                ListView lst = new ListView(this); main.AddView(lst);

                                lst.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(dt.DefaultView.ToTable(false, "Code", "DefectName", "DefectEN"), new List<int> { rq.eWidht(100), rq.eWidht(300), rq.eWidht(300) }) { TextSize = rq.eTextSize(20) };
                            }
                            catch { }
                        }
                        void LoadData(string query)
                        {
                            ds.Clear();

                            ds = kn.Doc(query, 120);

                            ShowData();
                        }
                        void ShowData()
                        {
                            DataTable dt = ds.Tables[i];

                            dfl.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(dt, new List<int> { rq.eWidht(100), rq.eWidht(330), rq.eWidht(70) }, true) { TextSize = rq.eTextSize(20) };

                            List<ChartEntry> lsentry = new List<ChartEntry>();

                            Random rd = new Random();
                            int top = 1;
                            foreach (DataRow r in dt.Rows)
                            {
                                SKColor cl = SKColor.Parse(string.Format("#{0:X6}", rd.Next(0x1000000)));
                                ChartEntry entry = new ChartEntry(float.Parse(r[2].ToString()))
                                {
                                    Label = r[0].ToString(),
                                    ValueLabel = r[2].ToString(),
                                    ValueLabelColor = cl,
                                    Color = cl//Color = SKColor.Parse("#266489")
                                };
                                lsentry.Add(entry);
                            }

                            Chart chart = new DonutChart() { Entries = lsentry, HoleRadius = 0.1f };
                            float ts = dt.Rows.Count > 20 ? 40 - dt.Rows.Count : 20;
                            if (ts < 10) ts = 10;
                            chart.LabelTextSize = rq.eTextSize(ts);
                            ch.Chart = chart;

                        }
                        void ShowDetail(DataTable dt, string header)
                        {
                            Android.App.AlertDialog.Builder b2 = new Android.App.AlertDialog.Builder(this);

                            LinearLayout main = new LinearLayout(this) { Orientation = Orientation.Vertical };
                            TextView head = new TextView(this) { Text = header, TextSize = rq.eTextSize(20) }; main.AddView(head);
                            ListView lst = new ListView(this); main.AddView(lst);

                            lst.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(dt, new List<int> { rq.eWidht(700), rq.eWidht(100) }, true) { TextSize = rq.eTextSize(20) };

                            b2.SetView(main);
                            b2.Create().Show();
                        }
                    };
                    stw.Click += delegate
                    {
                        Android.App.AlertDialog.Builder b1 = new Android.App.AlertDialog.Builder(this);

                        EditText ed = new EditText(this) { Hint = "Input your ID", LayoutParameters = new ViewGroup.LayoutParams(300, ViewGroup.LayoutParams.WrapContent) };
                        ed.InputType = Android.Text.InputTypes.ClassNumber | Android.Text.InputTypes.NumberFlagSigned;

                        b1.SetView(ed);
                        b1.SetPositiveButton("OK", (s, a) =>
                        {
                            DataTable dt = kn.Doc("select id_staff ID,fullname Name,type Pos,id_dept Dept from hr.dbo.staff where id_staff like '%" + ed.Text + "' and type in ('SP','LD','MG','HD','LO')").Tables[0];

                            if (dt.Rows.Count > 0)
                            {
                                int i = 0, index = 0; Color cl = new Color();
                                Android.App.AlertDialog.Builder b2 = new Android.App.AlertDialog.Builder(this);

                                ListView ls = new ListView(this);
                                ls.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(dt, new List<int> { rq.eWidht(200), rq.eWidht(350), rq.eWidht(100), rq.eWidht(100) }, true) { TextSize = rq.eTextSize(18) };
                                ls.ItemClick += (ss, aa) =>
                                {
                                    Toast.MakeText(this, aa.Position.ToString(), ToastLength.Long).Show();
                                    if (aa.Position > 0)
                                    {
                                        i = aa.Position - 1;

                                        if (index > 0)
                                        {
                                            View fv = GetViewByPosition(index, ls);
                                            if (fv != null) fv.SetBackgroundColor(cl);
                                        }

                                        View v = GetViewByPosition(aa.Position, ls);
                                        if (v != null) { cl = ((ColorDrawable)v.Background).Color; v.SetBackgroundColor(Color.Green); }

                                        index = aa.Position;
                                    }
                                };

                                b2.SetView(ls);
                                b2.SetPositiveButton("OK", (ss, aa) =>
                                {
                                    DataRow r = dt.Rows[i];
                                    CheckList(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString());
                                });
                                b2.SetNegativeButton("EXIT", (ss, aa) => { });

                                b2.SetCancelable(false);
                                b2.Create().Show();
                            }
                            else Toast.MakeText(this, "Your ID is not exist !!!", ToastLength.Long).Show();
                        });
                        b1.SetNegativeButton("EXIT", (s, a) => { });

                        b1.SetCancelable(false);
                        Dialog d1 = b1.Create();

                        d1.Show();
                        //Window window = d1.Window;
                        //window.SetLayout(rq.eWidht(1000), ViewGroup.LayoutParams.WrapContent);
                    };

                    kanban.LongClick += delegate
                    {
                        Toast.MakeText(this, kanban.Width + "/" + metric.WidthPixels, ToastLength.Long).Show();
                    };

                    b.SetView(main);
                    d = b.Create();
                    d.Show();

                    async void CheckList(string id, string name, string position, string dept)
                    {
                        TextView rtxt = null;
                        Toast.MakeText(this, "Openning The STW CheckList ....", ToastLength.Long).Show();
                        await Task.Delay(1000);

                        Timer tm = new Timer();
                        tm.Interval = 1000;
                        tm.Start();

                        int en = 1, dem = 0;
                        DataTable dt = kn.Doc("exec GetDataFromQuery 7,'A1A" + CSDL.SelectedLine.Substring(0, 2) + "','SEW','" + position + "','',''").Tables[0];

                        Android.App.AlertDialog.Builder b2 = new Android.App.AlertDialog.Builder(this, Android.Resource.Style.ThemeLightNoTitleBarFullScreen);

                        LinearLayout main = new LinearLayout(this) { Orientation = Orientation.Vertical }; main.SetBackgroundColor(Color.White);

                        ViewGroup.MarginLayoutParams mr = new ViewGroup.MarginLayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);

                        #region Header
                        LinearLayout head = new LinearLayout(this) { Orientation = Orientation.Horizontal }; main.AddView(head);

                        mr.SetMargins(5, 5, 0, 0);
                        TextView txtmonth = new TextView(this) { Text = "Month : ", LayoutParameters = mr }; head.AddView(txtmonth);
                        txtmonth.LongClick += delegate { Toast.MakeText(this, lsttxt.Count.ToString(), ToastLength.Long).Show(); };

                        mr.SetMargins(10, 0, 0, 0);
                        EditText edmonth = new EditText(this) { Text = DateTime.Now.ToString("MM"), LayoutParameters = mr }; edmonth.Focusable = false; edmonth.SetTextColor(Color.DeepPink); head.AddView(edmonth);

                        mr.SetMargins(50, 5, 0, 0);
                        TextView txtname = new TextView(this) { Text = "Name : ", LayoutParameters = mr }; head.AddView(txtname);

                        if (name.Length > 20)
                        {
                            string[] nm = name.Split(" ");
                            int g = (int)Math.Ceiling((decimal)nm.Count() / 2);

                            name = "";
                            for (int i = 0; i < g; i++) name += " " + nm[i];
                            name += "\n";
                            for (int i = g; i < nm.Count(); i++) name += " " + nm[i];

                            name = name.Substring(1);
                        }

                        mr.SetMargins(10, 0, 0, 0);
                        EditText edname = new EditText(this) { Text = name, LayoutParameters = mr }; edname.Focusable = false; edname.SetTextColor(Color.DeepPink); head.AddView(edname);

                        mr.SetMargins(50, 5, 0, 0);
                        TextView txtdept = new TextView(this) { Text = "Dept : ", LayoutParameters = mr }; head.AddView(txtdept);

                        mr.SetMargins(10, 0, 0, 0);
                        EditText eddept = new EditText(this) { Text = dept, LayoutParameters = mr }; eddept.Focusable = false; eddept.SetTextColor(Color.DeepPink); head.AddView(eddept);

                        mr.SetMargins(50, 5, 0, 0);
                        TextView txtpos = new TextView(this) { Text = "Position : ", LayoutParameters = mr }; head.AddView(txtpos);

                        mr.SetMargins(10, 0, 0, 0);
                        EditText edpos = new EditText(this) { Text = position, LayoutParameters = mr }; edpos.Focusable = false; edpos.SetTextColor(Color.DeepPink); head.AddView(edpos);
                        mr.SetMargins(10, 0, 0, 0);
                        CheckBox chken = new CheckBox(this) { Text = "EN", LayoutParameters = mr }; chken.Checked = false; head.AddView(chken);
                        #endregion

                        mr.SetMargins(0, 15, 0, 0);
                        ScrollView list = new ScrollView(this) { LayoutParameters = mr }; main.AddView(list);

                        LinearLayout mainls = new LinearLayout(this) { Orientation = Orientation.Vertical }; list.AddView(mainls);
                        GradientDrawable border = new GradientDrawable();
                        border.SetColor(Color.White);
                        border.SetStroke(1, Color.Black);

                        //for (int i = 0; i <= 20; i++)
                        //{
                        //    mr.SetMargins(10, 10, 0, 0);
                        //    TextView txt = new TextView(this) { Text = "Values = " + i.ToString("00"), LayoutParameters = mr };

                        //    txt.Background = border;
                        //    mainls.AddView(txt);
                        //}

                        //LoadList();

                        b2.SetView(main);
                        b2.Create().Show();

                        chken.Click += delegate
                        {
                            if (chken.Checked) en = 2;
                            else en = 1;

                            LoadList();
                        };
                        edmonth.Click += delegate { SelectMonth(); };
                        edmonth.LongClick += delegate
                        {
                            Android.Util.DisplayMetrics metric = Application.Context.Resources.DisplayMetrics;
                            Toast.MakeText(this, metric.WidthPixels + "/" + metric.HeightPixels + " " + metric.Density, ToastLength.Short).Show();
                        };

                        void LoadList()
                        {
                            lsttxt.Clear();
                            mainls.RemoveAllViews();

                            int month = int.Parse(edmonth.Text), year = DateTime.Now.Year, dem = 1;
                            int monthofyear = DateTime.DaysInMonth(year, month);
                            List<string> list = new List<string>();

                            for (int i = dem; i <= monthofyear; i++)
                            {
                                DateTime day = new DateTime(year, month, i);

                                if (day.DayOfWeek != DayOfWeek.Sunday) list.Add(i.ToString("00"));

                                dem++;

                                if (list.Count == 6) break;
                            }

                            mr.SetMargins(10, 0, 0, 0);
                            TextView txt1 = new TextView(this) { Text = " 1st Week - Tuần 1", LayoutParameters = mr }; txt1.SetTextColor(Color.DarkViolet); mainls.AddView(txt1);
                            mr.SetMargins(10, 0, 0, 10);
                            LinearLayout ln1 = Manager(dt, list, en); ln1.LayoutParameters = mr;
                            mainls.AddView(ln1);

                            list.Clear();
                            for (int i = dem; i <= monthofyear; i++)
                            {
                                DateTime day = new DateTime(year, month, i);

                                if (day.DayOfWeek != DayOfWeek.Sunday) list.Add(i.ToString("00"));

                                dem++;

                                if (list.Count == 6) break;
                            }

                            mr.SetMargins(10, 0, 0, 0);
                            TextView txt2 = new TextView(this) { Text = " 2nd Week - Tuần 2", LayoutParameters = mr }; txt2.SetTextColor(Color.DarkViolet); mainls.AddView(txt2);
                            mr.SetMargins(10, 0, 0, 10);
                            LinearLayout ln2 = Manager(dt, list, en); ln2.LayoutParameters = mr;
                            mainls.AddView(ln2);

                            list.Clear();
                            for (int i = dem; i <= monthofyear; i++)
                            {
                                DateTime day = new DateTime(year, month, i);

                                if (day.DayOfWeek != DayOfWeek.Sunday) list.Add(i.ToString("00"));

                                dem++;

                                if (list.Count == 6) break;
                            }

                            mr.SetMargins(10, 0, 0, 0);
                            TextView txt3 = new TextView(this) { Text = " 3rd Week - Tuần 3", LayoutParameters = mr }; txt3.SetTextColor(Color.DarkViolet); mainls.AddView(txt3);
                            mr.SetMargins(10, 0, 0, 10);
                            LinearLayout ln3 = Manager(dt, list, en); ln3.LayoutParameters = mr;
                            mainls.AddView(ln3);

                            list.Clear();
                            for (int i = dem; i <= monthofyear; i++)
                            {
                                DateTime day = new DateTime(year, month, i);

                                if (day.DayOfWeek != DayOfWeek.Sunday) list.Add(i.ToString("00"));

                                dem++;

                                if (list.Count == 6) break;
                            }

                            mr.SetMargins(10, 0, 0, 0);
                            TextView txt4 = new TextView(this) { Text = " 4th Week - Tuần 4", LayoutParameters = mr }; txt4.SetTextColor(Color.DarkViolet); mainls.AddView(txt4);
                            mr.SetMargins(10, 0, 0, 10);
                            LinearLayout ln4 = Manager(dt, list, en); ln4.LayoutParameters = mr;
                            mainls.AddView(ln4);

                            if (dem < monthofyear)
                            {
                                list.Clear();
                                for (int i = dem; i <= monthofyear; i++)
                                {
                                    DateTime day = new DateTime(year, month, i);

                                    if (day.DayOfWeek != DayOfWeek.Sunday) list.Add(i.ToString("00"));

                                    dem++;

                                    if (list.Count == 6) break;
                                }

                                mr.SetMargins(10, 0, 0, 0);
                                TextView txt5 = new TextView(this) { Text = " 5th Week - Tuần 5", LayoutParameters = mr }; txt5.SetTextColor(Color.DarkViolet); mainls.AddView(txt5);
                                mr.SetMargins(10, 0, 0, 10);
                                LinearLayout ln5 = Manager(dt, list, en); ln5.LayoutParameters = mr;
                                mainls.AddView(ln5);
                            }

                            DataTable dtrs = kn.Doc("exec GetDataFromQuery 8, '" + id + "', '" + edmonth.Text + "-" + DateTime.Now.ToString("yyyy") + "', '', '', ''").Tables[0];

                            foreach (CheckListItem it in lsttxt)
                            {
                                DataRow[] dtr = dtrs.Select("CheckedDate = '" + it.Value + "' and CheckListID = '" + it.CheckListID + "'");
                                if (dtr.Length > 0) { it.Txt.Text = "✓"; it.Txt.SetTextColor(Color.Blue); }
                                else
                                {
                                    int today = DateTime.Now.Day + DateTime.Now.Month * 100;
                                    int day = int.Parse(edmonth.Text + it.Value);

                                    if (day <= today) it.Txt.Text = "X";
                                }


                                string str1 = "", str2 = "", bt1 = "", bt2 = "";
                                if (en == 1)
                                {
                                    str1 = "Bạn đã hoàn thành hoạt động này !!!";
                                    str2 = "Hoàn thành hoạt động này ?";
                                    bt1 = "THOÁT"; bt2 = "XÁC NHẬN";
                                }
                                else
                                {
                                    str1 = "This activity is finished !!!";
                                    str2 = "Finish this activity ?";
                                    bt1 = "EXIT"; bt2 = "CONFIRM";
                                }

                                it.Txt.Click += delegate
                                {
                                    if (rtxt != null) rtxt.Background = border;

                                    rtxt = it.Txt; /*rcl = ((ColorDrawable)it.Txt.Background).Color;*/
                                    it.Txt.SetBackgroundColor(Color.Green);

                                    if (it.Txt.Text == "" || it.Txt.Text == "X")
                                    {
                                        Android.App.AlertDialog.Builder builder1 = new Android.App.AlertDialog.Builder(this);
                                        builder1.SetMessage(str2);
                                        builder1.SetPositiveButton(bt2, (s, a) =>
                                        {
                                            string insert = "insert into STWCheckListResult values ('" + it.Value + "','" + edmonth.Text + "-" + DateTime.Now.ToString("yyyy") + "','" + id + "','" + it.CheckListID + "',getdate())";

                                            kn.Ghi(insert);

                                            if (kn.ErrorMessage == "") { it.Txt.Text = "✓"; it.Txt.SetTextColor(Color.Blue); }
                                            else Toast.MakeText(this, kn.ErrorMessage, ToastLength.Long).Show();
                                        });
                                        builder1.SetNegativeButton(bt1, (s, a) => { });

                                        builder1.SetCancelable(false);
                                        builder1.Create().Show();
                                    }
                                    else Toast.MakeText(this, str1, ToastLength.Long).Show();
                                };
                            }
                        }
                        void SelectMonth()
                        {
                            Android.App.AlertDialog.Builder b = new Android.App.AlertDialog.Builder(this);
                            Dialog d = new Dialog(this);

                            List<string> months = new List<string>();
                            for (int i = 1; i <= 12; i++) months.Add(i.ToString("00"));

                            b.SetSingleChoiceItems(months.ToArray(), -1, (s, a) =>
                            {
                                edmonth.Text = months[a.Which];

                                LoadList();

                                d.Dismiss();
                            });

                            d = b.Create();
                            d.Show();
                        }

                        tm.Elapsed += delegate
                        {
                            RunOnUiThread(() =>
                            {
                                dem++;

                                if (dem == 1)
                                {
                                    Toast.MakeText(this, "Loaded !!!", ToastLength.Long).Show();
                                    LoadList();
                                    tm.Stop();
                                }
                            });
                        };
                    }
                }
                catch (Exception Ex)
                {
                    Toast.MakeText(this, CSDL.Language("M00018") + " /n" + Ex.ToString(), ToastLength.Long).Show();
                }
            };
            btKanbanSelect.LongClick += delegate
            {
                BottleNeckMonitor(); ShowBottleneck = true;
            };
            tvMyClock.Click += delegate
            {
                UpdateChecker();
            };
            tvMyClock.LongClick += delegate
            {
                myAppInitializer();
            };

            showbtn.Interval = 2 * 60 * 1000;
            showbtn.Enabled = true;
            showbtn.Elapsed += delegate
            {
                RunOnUiThread(() =>
                {
                    if (autoload)
                    {
                        if (ShowBottleneck) { d.Dismiss(); ShowBottleneck = false; disshowbtn++; }
                        else
                        {
                            if (disshowbtn >= 5)
                            {
                                BottleNeckMonitor(); ShowBottleneck = true; disshowbtn = 0;
                            }
                            else disshowbtn++;
                        }
                    }
                });
            };
            showbtn.Start();

            try
            {
                string url = kn.Doc("select * from InlineQCSystem where STT = 72").Tables[0].Rows[0][0].ToString();

                WebClient web = new WebClient();
                byte[] img = web.DownloadData(url);

                var bitmap = BitmapFactory.DecodeByteArray(img, 0, img.Length);
                logo.SetImageBitmap(bitmap);

                Toast.MakeText(this, img.Length.ToString(), ToastLength.Long).Show();

                logo.Click += delegate
                {
                    Toast.MakeText(this, url, ToastLength.Long).Show();
                };

            }
            catch { }
        }
        private LinearLayout LineItem(DataRow r, Color cl1, Color cl2)
        {
            GradientDrawable border1 = new GradientDrawable();
            border1.SetColor(cl1);
            border1.SetStroke(1, Color.Black);

            GradientDrawable border2 = new GradientDrawable();
            border2.SetColor(Color.LightGray);
            border2.SetStroke(1, Color.Black);


            ViewGroup.MarginLayoutParams mr = new ViewGroup.MarginLayoutParams(rq.eWidht(120), rq.eHeight(100));
            mr.SetMargins(rq.eWidht(20), rq.eHeight(10), 0, 0);
            LinearLayout ln = new LinearLayout(this) { Orientation = Orientation.Vertical, LayoutParameters = mr };
            ln.SetBackgroundColor(cl1);
            ln.Background = border1;

            ViewGroup.MarginLayoutParams mr1 = new ViewGroup.MarginLayoutParams(rq.eWidht(120), rq.eHeight(50));
            TextView tv = new TextView(this) { Text = r[0].ToString(), Gravity = GravityFlags.Center, TextSize = rq.eTextSize(30), LayoutParameters = mr1 }; tv.SetTextColor(cl2); tv.Background = border1; ln.AddView(tv);

            ViewGroup.MarginLayoutParams mr2 = new ViewGroup.MarginLayoutParams(rq.eWidht(120), rq.eHeight(50));
            LinearLayout con = new LinearLayout(this) { Orientation = Orientation.Horizontal, LayoutParameters = mr2 }; con.Background = border2; ln.AddView(con);
            ViewGroup.MarginLayoutParams mr3 = new ViewGroup.MarginLayoutParams(rq.eWidht(60), rq.eHeight(50));
            TextView tv1 = new TextView(this) { Text = "TotalWIP\n" + r[7].ToString(), Gravity = GravityFlags.Center, TextSize = rq.eTextSize(10), LayoutParameters = mr3 }; tv1.Background = border2; con.AddView(tv1);
            ViewGroup.MarginLayoutParams mr4 = new ViewGroup.MarginLayoutParams(rq.eWidht(60), rq.eHeight(50));
            TextView tv2 = new TextView(this) { Text = "HourWIP\n" + r[8].ToString(), Gravity = GravityFlags.Center, TextSize = rq.eTextSize(10), LayoutParameters = mr4 }; tv2.Background = border2; con.AddView(tv2);

            tv1.Click += delegate { ShowInfor(); };
            tv2.Click += delegate { ShowInfor(); };
            tv.Click += delegate { ShowDetail(); };

            void ShowInfor()
            {
                Android.App.AlertDialog.Builder b = new Android.App.AlertDialog.Builder(this);
                ViewGroup.MarginLayoutParams mr5 = new ViewGroup.MarginLayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent);
                mr5.SetMargins(rq.eWidht(10), rq.eWidht(5), 0, 0);

                LinearLayout ln = new LinearLayout(this) { Orientation = Orientation.Vertical };
                TextView txt = new TextView(this) { Text = r[0].ToString(), TextSize = rq.eTextSize(30) }; txt.SetTextColor(Color.DarkViolet); ln.AddView(txt);

                //LinearLayout ln1 = new LinearLayout(this) { Orientation = Orientation.Horizontal }; ln.AddView(ln1);
                //TextView txt11 = new TextView(this) { Text = "Input Qty : ", TextSize = rq.eTextSize(20), LayoutParameters = mr5 }; txt11.SetTextColor(Color.Blue); ln1.AddView(txt11);
                //TextView txt12 = new TextView(this) { Text = r[1].ToString(), TextSize = rq.eTextSize(20), LayoutParameters = mr5 }; txt12.SetTextColor(Color.DarkOrange); ln1.AddView(txt12);

                //LinearLayout ln2 = new LinearLayout(this) { Orientation = Orientation.Horizontal }; ln.AddView(ln2);
                //TextView txt21 = new TextView(this) { Text = "Output Qty : ", TextSize = rq.eTextSize(20), LayoutParameters = mr5 }; txt21.SetTextColor(Color.Blue); ln2.AddView(txt21);
                //TextView txt22 = new TextView(this) { Text = r[2].ToString(), TextSize = rq.eTextSize(20), LayoutParameters = mr5 }; txt22.SetTextColor(Color.DarkOrange); ln2.AddView(txt22);

                //LinearLayout ln3 = new LinearLayout(this) { Orientation = Orientation.Horizontal }; ln.AddView(ln3);
                //TextView txt31 = new TextView(this) { Text = "Actual WIP : ", TextSize = rq.eTextSize(20), LayoutParameters = mr5 }; txt31.SetTextColor(Color.Blue); ln3.AddView(txt31);
                //TextView txt32 = new TextView(this) { Text = r[3].ToString(), TextSize = rq.eTextSize(20), LayoutParameters = mr5 }; txt32.SetTextColor(Color.DarkOrange); ln3.AddView(txt32);

                LinearLayout ln4 = new LinearLayout(this) { Orientation = Orientation.Horizontal }; ln.AddView(ln4);
                TextView txt41 = new TextView(this) { Text = "Work Hour : ", TextSize = rq.eTextSize(20), LayoutParameters = mr5 }; txt41.SetTextColor(Color.Blue); ln4.AddView(txt41);
                TextView txt42 = new TextView(this) { Text = r[4].ToString(), TextSize = rq.eTextSize(20), LayoutParameters = mr5 }; txt42.SetTextColor(Color.DarkOrange); ln4.AddView(txt42);

                LinearLayout ln5 = new LinearLayout(this) { Orientation = Orientation.Horizontal }; ln.AddView(ln5);
                TextView txt51 = new TextView(this) { Text = "Manpower : ", TextSize = rq.eTextSize(20), LayoutParameters = mr5 }; txt51.SetTextColor(Color.Blue); ln5.AddView(txt51);
                TextView txt52 = new TextView(this) { Text = r[5].ToString(), TextSize = rq.eTextSize(20), LayoutParameters = mr5 }; txt52.SetTextColor(Color.DarkOrange); ln5.AddView(txt52);

                LinearLayout ln6 = new LinearLayout(this) { Orientation = Orientation.Horizontal }; ln.AddView(ln6);
                TextView txt61 = new TextView(this) { Text = "Output per hour : ", TextSize = rq.eTextSize(20), LayoutParameters = mr5 }; txt61.SetTextColor(Color.Blue); ln6.AddView(txt61);
                TextView txt62 = new TextView(this) { Text = r[6].ToString(), TextSize = rq.eTextSize(20), LayoutParameters = mr5 }; txt62.SetTextColor(Color.DarkOrange); ln6.AddView(txt62);

                LinearLayout ln7 = new LinearLayout(this) { Orientation = Orientation.Horizontal }; ln.AddView(ln7);
                TextView txt71 = new TextView(this) { Text = "Total WIP : ", TextSize = rq.eTextSize(20), LayoutParameters = mr5 }; txt71.SetTextColor(Color.Blue); ln7.AddView(txt71);
                TextView txt72 = new TextView(this) { Text = r[7].ToString(), TextSize = rq.eTextSize(20), LayoutParameters = mr5 }; txt72.SetTextColor(Color.DarkOrange); ln7.AddView(txt72);

                LinearLayout ln8 = new LinearLayout(this) { Orientation = Orientation.Horizontal }; ln.AddView(ln8);
                TextView txt81 = new TextView(this) { Text = "WIP by hour : ", TextSize = rq.eTextSize(20), LayoutParameters = mr5 }; txt81.SetTextColor(Color.Blue); ln8.AddView(txt81);
                TextView txt82 = new TextView(this) { Text = r[8].ToString(), TextSize = rq.eTextSize(20), LayoutParameters = mr5 }; txt82.SetTextColor(Color.DarkOrange); ln8.AddView(txt82);

                b.SetPositiveButton("OK", (ss, aa) => { });
                b.SetView(ln);
                b.Create().Show();
            }
            void ShowDetail()
            {
                Android.App.AlertDialog.Builder b = new Android.App.AlertDialog.Builder(this, Android.Resource.Style.ThemeLightNoTitleBarFullScreen);

                ViewGroup.MarginLayoutParams mr = new ViewGroup.MarginLayoutParams(rq.eWidht(600), ViewGroup.LayoutParams.WrapContent);
                mr.SetMargins(rq.eWidht(10), 0, 0, 0);
                DataSet ds = kn.Doc("exec EndlineSewingLineBalacing 4,'" + Fac + "','" + r[0].ToString() + "'");

                LinearLayout ln = new LinearLayout(this) { Orientation = Orientation.Horizontal }; ln.SetBackgroundColor(Color.LightBlue);

                GradientDrawable border = new GradientDrawable();
                border.SetColor(Color.White);
                border.SetStroke(3, Color.DarkOrange);

                LinearLayout ln1 = new LinearLayout(this) { Orientation = Orientation.Vertical, LayoutParameters = mr }; ln.AddView(ln1);

                ViewGroup.MarginLayoutParams mr1 = new ViewGroup.MarginLayoutParams(rq.eWidht(600), rq.eWidht(100));
                TextView txt1 = new TextView(this) { Text = r[0].ToString() + " - WIP", LayoutParameters = mr1, TextSize = rq.eTextSize(50), Gravity = GravityFlags.Center };
                txt1.SetTextColor(Color.DarkViolet); ln1.AddView(txt1);

                ListView ls = new ListView(this) { LayoutParameters = mr, Background = border }; ln1.AddView(ls);
                ls.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(ds.Tables[0], new List<int> { rq.eWidht(40), rq.eWidht(150), rq.eWidht(150), rq.eWidht(150), rq.eWidht(70), rq.eWidht(40) }, true, false)
                { TextSize = rq.eTextSize(10) };

                LinearLayout ln2 = new LinearLayout(this) { Orientation = Orientation.Vertical, LayoutParameters = mr }; ln.AddView(ln2);

                ViewGroup.MarginLayoutParams mr2 = new ViewGroup.MarginLayoutParams(rq.eWidht(570), ViewGroup.LayoutParams.WrapContent);
                mr2.SetMargins(rq.eWidht(15), rq.eWidht(10), 0, 0);
                LinearLayout ln21 = new LinearLayout(this) { Orientation = Orientation.Vertical, LayoutParameters = mr2 }; ln21.SetBackgroundColor(Color.Orange); ln2.AddView(ln21);

                LinearLayout ln211 = new LinearLayout(this) { Orientation = Orientation.Horizontal, LayoutParameters = mr2 }; ln21.AddView(ln211);

                ViewGroup.MarginLayoutParams mr3 = new ViewGroup.MarginLayoutParams(rq.eWidht(190), ViewGroup.LayoutParams.WrapContent);
                TextView txt21 = new TextView(this) { Text = "ACTUAL", LayoutParameters = mr3, TextSize = rq.eTextSize(30), Gravity = GravityFlags.Center }; ln211.AddView(txt21);
                TextView txt22 = new TextView(this) { Text = "KANBAN", LayoutParameters = mr3, TextSize = rq.eTextSize(30), Gravity = GravityFlags.Center }; ln211.AddView(txt22);
                TextView txt23 = new TextView(this) { Text = "W.I.P", LayoutParameters = mr3, TextSize = rq.eTextSize(30), Gravity = GravityFlags.Center }; ln211.AddView(txt23);

                string t1 = "", t2 = "";
                if (ds.Tables[2].Rows.Count > 0) { t1 = ds.Tables[2].Rows[0][2].ToString(); t2 = ds.Tables[2].Rows[0][1].ToString(); }

                LinearLayout ln212 = new LinearLayout(this) { Orientation = Orientation.Horizontal, LayoutParameters = mr2 }; ln21.AddView(ln212);
                TextView txt31 = new TextView(this) { Text = t1, LayoutParameters = mr3, TextSize = rq.eTextSize(30), Gravity = GravityFlags.Center }; ln212.AddView(txt31);
                TextView txt32 = new TextView(this) { Text = t2, LayoutParameters = mr3, TextSize = rq.eTextSize(30), Gravity = GravityFlags.Center }; ln212.AddView(txt32);
                TextView txt33 = new TextView(this) { Text = r[3].ToString(), LayoutParameters = mr3, TextSize = rq.eTextSize(30), Gravity = GravityFlags.Center }; ln212.AddView(txt33);

                LinearLayout ln22 = new LinearLayout(this) { Orientation = Orientation.Vertical, LayoutParameters = mr2 }; ln2.AddView(ln22);

                PlotView chart = new PlotView(this); ln22.AddView(chart); chart.SetBackgroundColor(Color.White);

                var plotModel = new PlotModel { Title = "Kanban - Daily" };

                CategoryAxis cot = new CategoryAxis();
                cot.Position = AxisPosition.Bottom;
                cot.MajorGridlineStyle = LineStyle.Solid;
                cot.MinorGridlineStyle = LineStyle.Dot;

                var Points = new List<DataPoint>();
                double max = 0;
                foreach (DataRow r in ds.Tables[1].Rows)
                {
                    cot.Labels.Add(r[0].ToString());

                    double vl1 = string.IsNullOrEmpty(r[1].ToString()) ? 0 : double.Parse(r[1].ToString());
                    double vl2 = string.IsNullOrEmpty(r[2].ToString()) ? 0 : double.Parse(r[2].ToString());
                    if (max < vl1) max = vl1;
                    if (max < vl2) max = vl2;

                    ColumnSeries s = new ColumnSeries();
                    s.LabelPlacement = LabelPlacement.Inside;
                    s.LabelFormatString = "{0}";
                    if (cot.Labels.Count == 1) s.Title = "Kanban";
                    s.IsStacked = false;
                    s.Items.Add(new ColumnItem(vl1, cot.Labels.Count - 1));

                    plotModel.Series.Add(s);

                    Points.Add(new DataPoint(cot.Labels.Count - 1, vl2));
                }

                LineSeries l = new LineSeries();
                l.MarkerType = MarkerType.Circle;
                l.Title = "Actual";
                l.ItemsSource = Points;

                plotModel.Series.Add(l);
                plotModel.Axes.Add(cot);
                plotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = 0, Maximum = max, MajorGridlineStyle = LineStyle.Solid, MinorGridlineStyle = LineStyle.Dot });
                chart.Model = plotModel;

                b.SetView(ln);
                b.Create().Show();
            }

            return ln;
        }
        private LinearLayout Manager(DataTable dt, List<string> lsmonth, int en)
        {
            ViewGroup.MarginLayoutParams mr = new ViewGroup.MarginLayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.MatchParent);

            LinearLayout ln = new LinearLayout(this) { Orientation = Orientation.Vertical };
            GradientDrawable border = new GradientDrawable();
            border.SetColor(Color.White);
            border.SetStroke(1, Color.Black);
            ln.Background = border;

            LinearLayout head = new LinearLayout(this) { Orientation = Orientation.Horizontal };
            head.Background = border; ln.AddView(head);

            mr.Width = rq.eWidht(150);
            TextView txt1 = new TextView(this) { Text = en == 1 ? "Thời Gian" : "Time", LayoutParameters = mr, TextAlignment = TextAlignment.Center }; txt1.Background = border;
            txt1.SetTextColor(Color.Blue); head.AddView(txt1);

            mr.Width = rq.eWidht(700);
            TextView txt2 = new TextView(this) { Text = en == 1 ? "Hoạt Động Trong Ngày" : "Daily Activity", LayoutParameters = mr, TextAlignment = TextAlignment.Center }; txt2.Background = border;
            txt2.SetTextColor(Color.Blue); head.AddView(txt2);

            for (int j = 0; j < lsmonth.Count; j++)
            {
                mr.Width = rq.eWidht(60);

                TextView txt = new TextView(this) { Text = lsmonth[j], LayoutParameters = mr, TextAlignment = TextAlignment.Center }; txt.Background = border;
                txt.SetTextColor(Color.Blue); head.AddView(txt);
            }

            foreach (DataRow r in dt.Rows)
            {
                string act = r["Description" + en].ToString();
                string act1 = "";

                if (act.Contains("-"))
                {
                    foreach (string str in act.Split('-'))
                    {
                        if (str != "" || str != " ") act1 += "    - " + str + "\n";
                    }
                }
                else act1 = "    - " + act + "\n";

                LinearLayout ls = new LinearLayout(this) { Orientation = Orientation.Horizontal };
                ls.Background = border; ln.AddView(ls);

                mr.Width = rq.eWidht(150);
                TextView txt3 = new TextView(this) { Text = r["TimeRange"].ToString(), LayoutParameters = mr, Gravity = GravityFlags.Center }; txt3.Background = border; ls.AddView(txt3);

                mr.Width = rq.eWidht(700);
                TextView txt4 = new TextView(this) { Text = act1, LayoutParameters = mr }; txt4.Background = border; ls.AddView(txt4);

                for (int j = 0; j < lsmonth.Count; j++)
                {
                    mr.Width = rq.eWidht(60);
                    TextView txt = new TextView(this) { Text = "", LayoutParameters = mr, TextAlignment = TextAlignment.Center }; txt.Background = border; txt.SetTextColor(Color.DeepPink); ls.AddView(txt);

                    lsttxt.Add(new CheckListItem
                    {
                        Txt = txt,
                        Value = lsmonth[j],
                        CheckListID = r["CheckListID"].ToString()
                    });
                }
            }

            return ln;
        }
        public View GetViewByPosition(int pos, ListView listView)
        {
            try
            {
                int firstListItemPosition = listView.FirstVisiblePosition;
                int lastListItemPosition = firstListItemPosition + listView.Adapter.Count - 1;

                if (pos < firstListItemPosition || pos > lastListItemPosition)
                {
                    return listView.Adapter.GetView(pos, null, listView);
                }
                else
                {
                    int childIndex = pos - firstListItemPosition;
                    return listView.GetChildAt(childIndex);
                }
            }
            catch (System.Exception ex)
            {
                Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show();
                return null;
            }
        }
        private void BottleNeckMonitor()
        {
            try
            {
                List<string> time = new List<string>();
                int start = CSDL.TimeArray[0];
                for (int i = 0; i < 16; i++)
                {
                    time.Add(start.ToString("0000").Insert(2, ":") + " " + (start + 100).ToString("0000").Insert(2, ":"));
                    start += 100;
                }

                View v = LayoutInflater.Inflate(Resource.Layout.bottleneck, null);

                TextView line = v.FindViewById<TextView>(Resource.Id.txtfacline); line.Text = CSDL.SelectedLine;
                RelativeLayout mainview = v.FindViewById<RelativeLayout>(Resource.Id.mainview); rq.Steching(mainview);
                RelativeLayout bottlenecklayout = v.FindViewById<RelativeLayout>(Resource.Id.bottlenecklayout); rq.Steching(bottlenecklayout);
                ListView mainlist = v.FindViewById<ListView>(Resource.Id.mainlist);

                for (int i = 0; i < mainview.ChildCount; i++)
                {
                    View chil = mainview.GetChildAt(i);

                    if (chil is TextView)
                    {
                        TextView txt = chil as TextView;

                        if (txt.Text.Contains("R"))
                        {
                            txt.Text = time[int.Parse(txt.Text.Replace("R", "")) - 1];
                        }
                    }
                }

                int Target = 1, BottleneckRow = 0;
                try
                {
                    DataTable dttarget = kn.Doc("exec SmartLineLoadData 2,'" + CSDL.tg.ToString("yyyyMMdd") + "','" + CSDL.SelectedLine + "'").Tables[0];
                    if (dttarget.Rows.Count > 0) Target = int.Parse(dttarget.Rows[0][0].ToString());
                }
                catch { }

                string ch = "exec SmartLineLoadData 1,'" + CSDL.tg.ToString("yyyyMMdd") + "','" + CSDL.SelectedLine + "'";
                DataTable d1 = kn.Doc(ch, 120).Tables[0];

                bool show = true;
                if (d1.Rows.Count > 0)
                {
                    d1.Columns.Add("CD", typeof(int));

                    foreach (DataRow r in d1.Rows)
                    {
                        string[] op = r[0].ToString().Split('-');

                        r["CD"] = int.Parse(op[1]);
                    }

                    DataView dv = d1.DefaultView;
                    dv.Sort = "CD asc";

                    DataTable d2 = dv.ToTable();
                    for (int i = 1; i < 17; i++)
                    {
                        d2.Columns.Add("P" + i, typeof(decimal));
                    }

                    d2.Columns.Add("OperName", typeof(string));
                    d2.Columns.Add("SewerID", typeof(string));
                    d2.Columns.Add("SewerName", typeof(string));
                    //Get Operation Name
                    DataTable oper = kn.Doc("select distinct CODEOPT,STYLE_NO,OPERATION,EMPLOYEE,ORDNUM from MasterLayout where LINEST = '" + CSDL.SelectedLine + "' and charindex(STYLE_NO,'" + d2.Rows[0][0].ToString() + "') > 0").Tables[0];
                    if (oper.Rows.Count > 0)
                    {
                        foreach (DataRow r in d2.Rows)
                        {
                            foreach (DataRow r1 in oper.Rows)
                            {
                                if (r["Operation"].ToString().Trim().Equals(r1[1].ToString().Trim() + "-" + r1[0].ToString().Trim()) && r["Alias1"].ToString() == r1["ORDNUM"].ToString())
                                {
                                    r["OperName"] = r1[2].ToString(); r["SewerID"] = r1[3].ToString(); break;
                                }
                            }
                        }
                    }

                    //Get Sewer Name
                    DataTable sewer = kn.Doc("SELECT id_staff,code,fullname FROM [Hr].[dbo].[staff] where id_dept like '" + CSDL.SelectedLine.Substring(0, 3) + "%'").Tables[0];
                    if (sewer.Rows.Count > 0)
                    {
                        foreach (DataRow r in d2.Rows)
                        {
                            foreach (DataRow r1 in sewer.Rows)
                            {
                                if (r["SewerID"].ToString().Trim().Equals(r1[0].ToString().Trim()))
                                {
                                    r["SewerName"] = r1[1].ToString() + " - " + r1[2].ToString().Split(' ').Last();
                                    break;
                                }
                            }
                        }
                    }

                    List<string> cdd = new List<string>();
                    foreach (DataRow rr in d2.Rows)
                    {
                        int[] bl = new int[16];
                        if (!cdd.Contains(rr["OperName"].ToString()))
                        {
                            DataRow[] ex = d2.Select("OperName = '" + rr["OperName"].ToString() + "'");

                            if (ex.Length > 1)
                            {
                                foreach (DataRow ex1 in ex)
                                {
                                    bl[0] += (int)ex1["Total"];

                                    for (int i = 1; i < 16; i++) bl[i] += (int)ex1["R" + i];
                                }

                                foreach (DataRow ex1 in ex)
                                {
                                    ex1["Total"] = bl[0];

                                    for (int i = 1; i < 16; i++) ex1["R" + i] = bl[i];
                                }
                            }
                            cdd.Add(rr["OperName"].ToString());

                            d2.AcceptChanges();
                        }
                    }

                    int cd1 = (int)d2.Rows[0]["Total"];
                    decimal btr = Target > 0 ? Math.Round(((decimal)cd1 / Target) * 100, 2) : cd1 * 100; d2.Rows[0]["P1"] = 0; d2.Rows[0]["SewerID"] = cd1 + "/" + Target;
                    for (int j = 1; j < d2.Rows.Count; j++)
                    {
                        int sl1 = (int)d2.Rows[j - 1]["Total"];

                        DataRow r = d2.Rows[j];
                        int sl2 = (int)r["Total"];

                        decimal cur = sl1 > 0 ? Math.Round(((decimal)sl2 / sl1) * 100, 2) : sl2 * 100; d2.Rows[j]["SewerID"] = sl2 + "/" + sl1;

                        if (btr > cur) { btr = cur; BottleneckRow = j; }

                        d2.Rows[j]["P1"] = BottleneckRow;
                    }

                    mainlist.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(d2.DefaultView.ToTable(false, "OperName", "SewerName", "CycleTime", "Total", "R1", "R2", "R3", "R4", "R5", "R6", "R7", "R8", "R9", "R10", "R11", "R12", "R13", "R14", "R15", "R16"),
                        new List<int> { rq.eWidht(500), rq.eWidht(200), rq.eWidht(110), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100), rq.eWidht(100) })
                    {
                        SetRowColor = new List<RowColor> { new RowColor { RowIndex = BottleneckRow } },
                        TextSize = rq.eTextSize(20)
                    };

                    mainlist.SetSelection(BottleneckRow);
                }
                else show = false;
                d.SetContentView(v);

                if (show) d.Show();

                Toast.MakeText(this, d1.Rows.Count.ToString(), ToastLength.Long).Show();
            }
            catch (Exception ex) { Toast.MakeText(this, ex.ToString(), ToastLength.Long).Show(); }
        }
        private void myAppInitializer()
        {
            ISharedPreferences pre = GetSharedPreferences("server", FileCreationMode.Private);
            ISharedPreferences pre1 = GetSharedPreferences("FacSel", FileCreationMode.Private);
            //string ch = pre.GetString("chuoi", "").ToString();
            Fac = pre1.GetString("Fac", "");

            //if (ch != "") CSDL.chuoi = ch;

            //try
            //{
            //    SqlConnection con = new SqlConnection(CSDL.chuoi);
            //    con.Open();
            //    con.Close();

            //    load();
            //}
            //catch
            //{
            //    var itent = new Intent(this, typeof(ServerActivity));
            //    StartActivity(itent);
            //    Finish();
            //}

            load();

            void load()
            {
                CSDL.Cnnt = new Connect(CSDL.chuoi);

                WrkHrId = pre.GetString("WrkHrId", "").ToString();
                ShiftId = pre.GetString("ShiftId", "").ToString();

                DataTable dtinfo = CSDL.Cnnt.Doc("SELECT  [IPSERVER],[TIME],[Descpt],[ForceUpdate],[STT]  FROM  InlineQCSystem").Tables[0];

                DataRow[] tempr = dtinfo.Select("Descpt = 'ELTVVDO'");
                CSDL.ipserver = tempr[0][0].ToString();
                if (int.Parse(tempr[0][1].ToString()) > 0) CSDL.timeplay = int.Parse(tempr[0][1].ToString()) * 60000;
                else CSDL.timeplay = 900000;
                forceUpdate = string.IsNullOrEmpty(tempr[0]["ForceUpdate"].ToString()) ? false : (int.Parse(tempr[0]["ForceUpdate"].ToString()) > 0 ? true : false);

                tempr = dtinfo.Select("Descpt = 'LBCHPHP'");
                ImgURL = tempr[0][0].ToString();

                tempr = dtinfo.Select("Descpt = 'ELSWIM'");
                CSDL.ELIMUrl = tempr[0][0].ToString();

                tempr = dtinfo.Select("Descpt ='ILSWIM'");
                CSDL.ILIMUrl = tempr[0][0].ToString();

                tempr = dtinfo.Select("Descpt = 'ELSWUP'");
                LinkUpdateAPP = tempr[0][0].ToString();

                tempr = dtinfo.Select("Descpt = 'REToELTV'");
                RefT = int.Parse(tempr[0][1].ToString()) * 60000;
                if (RefT != 600000)
                {
                    MyTimer.Stop();
                    MyTimer.Interval = RefT;
                    MyTimer.Start();
                }
                Get_Time();

                SpinnerInitialize();
                snLineList.RequestFocus();
                InputMethodManager inputManager = (InputMethodManager)GetSystemService(Context.InputMethodService);
                var currentFocus = Window.CurrentFocus;
                if (currentFocus != null)
                {
                    inputManager.HideSoftInputFromWindow(currentFocus.WindowToken, HideSoftInputFlags.None);
                }

                tvBeginDate.Text = CSDL.tg.ToString("MMMM, dd yyyy").ToUpper();
                tvEndDate.Text = CSDL.tg.ToString("MMMM, dd yyyy").ToUpper();
                CSDL.BeginDate = CSDL.tg.ToString("yyyyMMdd");
                CSDL.EndDate = CSDL.tg.ToString("yyyyMMdd");
            }
        }

        private void TimeTitle()
        {
            //if (CSDL.strTimeTitle == "")
            //{
            //    ISharedPreferences pre = GetSharedPreferences("server", FileCreationMode.Private);
            //    string ch = pre.GetString("timefr", "").ToString();
            //    if (ch != "")
            //    {
            //        CSDL.strTimeTitle = ch;
            //        string[] timfr = ch.Split(',');
            //        for (int i = 0; i < 14; i++)
            //        {
            //            CSDL.TimeArray[i] = int.Parse(timfr[i]);
            //        }
            //    };
            //}

            tv1hr.Text = FmTime(CSDL.TimeArray[0]) + "-" + FmTime(CSDL.TimeArray[1]);
            tv2hr.Text = FmTime(CSDL.TimeArray[1] + 1) + "-" + FmTime(CSDL.TimeArray[2]);
            tv3hr.Text = FmTime(CSDL.TimeArray[2] + 1) + "-" + FmTime(CSDL.TimeArray[3]);
            tv4hr.Text = FmTime(CSDL.TimeArray[3] + 1) + "-" + FmTime(CSDL.TimeArray[4]);
            tv5hr.Text = FmTime(CSDL.TimeArray[4] + 1) + "-" + FmTime(CSDL.TimeArray[5]);
            tv6hr.Text = FmTime(CSDL.TimeArray[5] + 1) + "-" + FmTime(CSDL.TimeArray[6]);
            tv7hr.Text = FmTime(CSDL.TimeArray[6] + 1) + "-" + FmTime(CSDL.TimeArray[7]);
            tv8hr.Text = FmTime(CSDL.TimeArray[7] + 1) + "-" + FmTime(CSDL.TimeArray[8]);
            tv9hr.Text = FmTime(CSDL.TimeArray[8] + 1) + "-" + FmTime(CSDL.TimeArray[9]);
            tv10hr.Text = FmTime(CSDL.TimeArray[9] + 1) + "-" + FmTime(CSDL.TimeArray[10]);
            tv11hr.Text = FmTime(CSDL.TimeArray[10] + 1) + "-" + FmTime(CSDL.TimeArray[11]);
            tv12hr.Text = FmTime(CSDL.TimeArray[11] + 1) + "-" + FmTime(CSDL.TimeArray[12]);
        }
        private string FmTime(int mytime)
        {
            if (mytime.ToString().Length > 2 && mytime.ToString().Length < 5)
            {
                return mytime.ToString("0000").Substring(0, 2) + ":" + mytime.ToString().Substring(mytime.ToString().Length - 2, 2);
            }
            else return "Err";
        }

        private void AddLinetoList(DataTable myLines)
        {
            List<string> LineList = new List<string>();
            ISharedPreferences lpre = GetSharedPreferences("LineSel", FileCreationMode.Private);
            string l = lpre.GetString("Line", "").ToString();
            if (l == "") LineList.Add("FxAxx");
            else
            {
                LineList.Add(l);
                mySpinerPositionID = 1;
                CSDL.SelectedLine = l;
                CSDL.checkopen = 0;
            }
            foreach (DataRow myr in myLines.Rows) if (!LineList.Contains(myr[0].ToString())) LineList.Add(myr[0].ToString());
            ArrayAdapter adater = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, LineList);
            adater.SetDropDownViewResource(Android.Resource.Layout.SimpleListItemSingleChoice);
            snLineList.Adapter = adater;


        }

        private void UpdateChecker()
        {
            try
            {
                if ((int)Build.VERSION.SdkInt > 23)
                {
                    if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
                    {
                        Android.Support.V4.App.ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, 1000);
                    }
                }
                if (!isadShow)
                {
                    newVer = false;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(LinkUpdateAPP);
                    // If required by the server, set the credentials.
                    //request.Credentials = CredentialCache.DefaultCredentials;
                    //request.IfModifiedSince = DateTime.Parse("01-01-1990");

                    using (WebResponse response = request.GetResponse())
                    {
                        DateTime dt = System.IO.File.GetCreationTime(Android.OS.Environment.ExternalStorageDirectory + "/Download/" + "TTPefmTVIndivSewL.apk"); //.GetLastWriteTime("/sdcard/Dowload/Inspections.Inspections-Signed.apk");
                        DateTime appDate = DateTime.Parse(response.Headers["Last-Modified"].ToString());
                        //mToast("dt=" + dt.ToString() + " | NewAppDate = " + appDate.ToString(), 5000);
                        if (appDate > dt)
                        {
                            try
                            {
                                newVer = true;
                                ad = new Android.App.AlertDialog.Builder(this).Create();
                                ad.SetTitle(CSDL.Language("M00146"));
                                ad.SetMessage(CSDL.Language("M00140") + "\n \n" + CSDL.Language("M00143") + "\n \n" + "Last-Modified:" + appDate.ToString() + "\n" + "CreationTime:" + dt);
                                ad.SetButton(CSDL.Language("M00144"), delegate
                                {
                                    isadShow = false;
                                    try
                                    {
                                        string m_filePath = Android.OS.Environment.ExternalStorageDirectory + "/Download/" + "TTPefmTVIndivSewL.apk";

                                        //System.IO.File.Delete(Android.OS.Environment.DirectoryDownloads + "/SEWTVDisplay.SEWTVDisplay-Signed.apk");//To Delete old file...
                                        //DownloadFile(LinkUpdateAPP, Android.OS.Environment.ExternalStorageDirectory + "/Download/" + "TTPefmTVIndivSewL.apk"); // To download new file
                                        var webClient = new WebClient();
                                        var url = new System.Uri(LinkUpdateAPP);

                                        webClient.OpenRead(url);
                                        TTDL = Convert.ToInt64(webClient.ResponseHeaders["Content-Length"]);

                                        webClient.DownloadFileAsync(url, m_filePath);

                                        webClient.DownloadProgressChanged += (sender, e) =>
                                        {
                                            TTRC = e.BytesReceived;
                                            PercentComplete = (int)(TTRC * 100 / TTDL);
                                            if (Progr == 0) ProgressCalling();
                                        };

                                        webClient.DownloadFileCompleted += async (s, e) =>
                                        {
                                            //File apkFile = new File(m_filePath);
                                            //Android.Net.Uri apkUri = Android.Net.Uri.FromFile(apkFile);
                                            //Intent webIntent = new Intent(Intent.ActionInstallPackage);//Intent.ACTION_VIEW
                                            //webIntent.SetDataAndType(Android.Net.Uri.FromFile(apkFile), "application/vnd.android.package-archive");
                                            //webIntent.SetFlags(ActivityFlags.NewTask);
                                            //webIntent.AddFlags(ActivityFlags.GrantReadUriPermission);
                                            //StartActivity(webIntent);
                                            //Finish();

                                            await Launcher.OpenAsync(new OpenFileRequest
                                            {

                                                File = new ReadOnlyFile(m_filePath)

                                            });
                                        };
                                    }
                                    catch (ActivityNotFoundException ex)
                                    {
                                        ad = new Android.App.AlertDialog.Builder(this).Create();
                                        ad.SetTitle("WebClient()");
                                        ad.SetMessage(CSDL.Language("M00019") + ex);
                                        ad.SetCanceledOnTouchOutside(true);
                                        ad.Show();
                                    }
                                });
                                ad.SetButton2(CSDL.Language("M00145"), delegate
                                {
                                    isadShow = false;
                                    return;
                                });
                                ad.SetCanceledOnTouchOutside(true);
                                ad.Show();
                                isadShow = true;
                            }
                            catch (System.Exception ex)
                            {
                                isadShow = false;
                                ad = new Android.App.AlertDialog.Builder(this).Create();
                                ad.SetTitle("AlertDialogDownloadFail");
                                ad.SetMessage(ex.Message);
                                ad.SetCanceledOnTouchOutside(true);
                                ad.Show();
                            }
                        }
                        else
                        {
                            try
                            {
                                string file = Android.OS.Environment.ExternalStorageDirectory + "/download/" + "TTPefmTVIndivSewL.apk";

                                PackageManager manager = PackageManager;
                                PackageInfo info = manager.GetPackageInfo(PackageName, 0);
                                PackageInfo info1 = manager.GetPackageArchiveInfo(file, 0);

                                if (info1.VersionCode > info.VersionCode)
                                {
                                    newVer = true;
                                    ad = new Android.App.AlertDialog.Builder(this).Create();

                                    ad.SetMessage(CSDL.Language("M00146"));
                                    ad.SetButton(CSDL.Language("M00141"), (s, a) =>
                                    {
                                        Java.IO.File apkFile = new Java.IO.File(file);
                                        Android.Net.Uri uri = Android.Net.Uri.FromFile(apkFile);//Android.Net.Uri.FromFile(apkFile);

                                        Intent webIntent = new Intent(Intent.ActionInstallPackage);//Intent.ACTION_VIEW
                                        webIntent.SetDataAndType(uri, "application/vnd.android.package-archive");
                                        webIntent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.NewTask | ActivityFlags.GrantPersistableUriPermission);
                                        webIntent.PutExtra(Intent.ExtraNotUnknownSource, true);
                                        Application.Context.StartActivity(webIntent);
                                    });

                                    ad.SetCanceledOnTouchOutside(true);
                                    ad.Show();
                                    isadShow = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                Toast.MakeText(this, "Check Version : " + ex.ToString(), ToastLength.Long).Show();
                            }
                        }
                    }
                }
            }
            catch (Exception ext)
            {
                ad = new Android.App.AlertDialog.Builder(this).Create();
                ad.SetTitle("UpdateChecker() - WebResponse ?");
                ad.SetMessage(ext.Message);
                ad.SetCanceledOnTouchOutside(true);
                ad.Show();
            }

        }

        private void LoadInfoVideo()
        {
            CSDL.LoadVideo.Enabled = true;
            CSDL.LoadVideo.Interval = CSDL.timeplay;
            CSDL.LoadVideo.Elapsed += LoadVideo_Elapsed;
            CSDL.LoadVideo.Start();
        }
        private void LoadVideo_Elapsed(object sender, ElapsedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                try
                {
                    Toast.MakeText(this, TimeTemp, ToastLength.Long).Show();
                    string st = "SELECT *  FROM InlineQCPlay p, InlineQCVideos v where p.ID_VIDEO = v.ID_VIDEO and p.ID_FACLINE = '" + CSDL.SelectedLine + "' and len(p.STATUS_PLAY)> 0 and len(p.TIMEPLAY) = 6 and CAST(p.TIMEPLAY as INT) >= '" + int.Parse(TimeTemp) + "'  order by p.TIMEPLAY ASC";
                    CSDL.videodata = CSDL.Cnnt.Doc(st).Tables[0];
                    if (CSDL.videodata.Rows.Count > 0)
                    {
                        CSDL.timeopen = CSDL.videodata.Rows[0][2].ToString();
                        CSDL.videoname = CSDL.videodata.Rows[0][7].ToString();
                        CSDL.loopplay = CSDL.videodata.Rows[0][4].ToString();
                    }
                    else
                    {
                        CSDL.timeopen = "";
                        CSDL.videoname = "";
                    }

                    DataSet dts = new DataSet();
                    DataTable dtinfo = new DataTable();
                    string stinfo = "SELECT  *  FROM  InlineQCSystem where Descpt = 'ELTVVDO'"
                                    + "SELECT  IPSERVER  FROM  InlineQCSystem where Descpt = 'ELSWUP'";
                    dts = CSDL.Cnnt.Doc(stinfo);
                    LinkUpdateAPP = dts.Tables[1].Rows[0][0].ToString();
                    dtinfo = dts.Tables[0];
                    CSDL.ipserver = dtinfo.Rows[0][0].ToString();

                    if (int.Parse(dtinfo.Rows[0][1].ToString()) > 0)
                    {
                        CSDL.timeplay = int.Parse(dtinfo.Rows[0][1].ToString()) * 60000;
                    }
                    else
                    {
                        CSDL.timeplay = 900000;
                    }
                    //CSDL.LoadVideo.Interval = CSDL.timeplay;
                    //CSDL.LoadVideo.Stop();
                    //CSDL.LoadVideo.Start();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "LoadVideo_Elapsed() : " + ex.Message, ToastLength.Long).Show();
                }
                //if (NoEr) UpdateChecker();
                //else Toast.MakeText(this, "CheckUpdate FAIL", ToastLength.Long).Show();
            });
        }

        private void Get_Time()
        {
            try
            {
                CSDL.tg = DateTime.Parse(CSDL.Cnnt.Doc("select getdate()").Tables[0].Rows[0][0].ToString());
                ss = CSDL.tg.Second; mm = CSDL.tg.Minute; hh = CSDL.tg.Hour;

                if (autoload)
                {
                    tvBeginDate.Text = CSDL.tg.ToString("MMMM dd, yyyy").ToUpper();
                    CSDL.BeginDate = CSDL.tg.ToString("yyyyMMdd");
                    tvEndDate.Text = CSDL.tg.ToString("MMMM dd, yyyy").ToUpper();
                    CSDL.EndDate = CSDL.tg.ToString("yyyyMMdd");
                }

                DataTable dt = CSDL.Cnnt.Doc("exec GetDataFromQuery 29,'" + Fac + "','" + CSDL.SelectedLine + "','','',''").Tables[0];

                if (dt.Rows.Count > 0) for (int i = 0; i < 14; i++) CSDL.TimeArray[i] = string.IsNullOrEmpty(dt.Rows[0][i + 3].ToString()) ? 2500 : int.Parse(dt.Rows[0][i + 3].ToString());
            }
            catch (Exception Ex)
            {
                Toast.MakeText(this, "Get_Time() : " + Ex.ToString(), ToastLength.Long).Show();
            }
        }
        private void Clock()
        {
            if (ss >= 59)
            {
                ss = 0; mm++;
            }
            if (mm >= 59)
            {
                mm = 0; hh++;
            }
            if (hh >= 24) hh = 0;

            ////-------- Load Videos ---------------
            TimeTemp = hh.ToString("00") + mm.ToString("00") + ss.ToString("00");
            //Toast.MakeText(this, TimeTemp + " = " + CSDL.timeopen + "  " + CSDL.checkopen, ToastLength.Short).Show();
            if (TimeTemp == CSDL.timeopen && CSDL.checkopen == 0)
            {
                Toast.MakeText(this, TimeTemp, ToastLength.Short).Show();
                Intent refresh = new Intent(this, typeof(VideosPlayer));
                StartActivity(refresh);
            }

            tvMyClock.Text = hh.ToString() + ":" + mm.ToString("00") + ":" + ss.ToString("00");
        }
        private void SecondTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                ss = DateTime.Now.Second;
                Clock();
                //Toast.MakeText(this, CSDL.mytest, ToastLength.Short).Show();
                //Toast.MakeText(this, "Res=" + CSDL.width + "x" + CSDL.height + " | Density=" + Resources.DisplayMetrics.Density + " | TextRatio=" + CSDL.TextRatio + " | SizingScrRt=" + CSDL.SizingScrRt + "\n" + CSDL.chuoi.Split(";")[0], ToastLength.Short).Show();
            });
        }

        private void LanguageTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RunOnUiThread(() => { ChangeLanguage(); });
        }

        private void ChangeLanguage()
        {
            tvSelectedLine.Text = CSDL.SelectedLine;
            try
            {
                TimeTitle();
                if (myLanguague)
                {
                    ActualEff.Text = CSDL.Language("M00126");
                    ActualOutput.Text = CSDL.Language("M00127");
                    ActualRFT.Text = CSDL.Language("M00128");
                    InLineDF.Text = CSDL.Language("M00129");
                    TargetPH = CSDL.Language("M00130") + ": ";
                    LangTTTargetPCS = CSDL.Language("M00131");
                    LangTTActualPCS = CSDL.Language("M00132");
                    LangTTDFPCS = CSDL.Language("M00133");
                    RunDay = CSDL.Language("M00134") + ": ";
                    ProductType = CSDL.Language("M00135") + ": ";
                    if (CSDL.Top3InlineDF.Rows.Count > 0)
                    {
                        InlineDF = 0;
                        InlineDFList.Text = "";
                        //InlineDFList.Text = CSDL.Language("M00136") + ":\n";
                        foreach (DataRow InLineDFString in CSDL.Top3InlineDF.Rows)
                        {
                            //InlineDFList.Text += (CSDL.Top3InlineDF.Rows.IndexOf(InLineDFString) + 1).ToString() + ". " + InLineDFString["DFNAMEVN"].ToString().ToUpper() + ": " + InLineDFString["SUMDF"].ToString() + "\n";
                            InlineDF += string.IsNullOrEmpty(InLineDFString["SUMDF"].ToString()) ? 0 : int.Parse(InLineDFString["SUMDF"].ToString());
                        }

                        piechart1.Model = CreateChart(CSDL.Top3InlineDF.DefaultView.ToTable(false, "DFNAMEVN", "SUMDF"), CSDL.Language("M00136"));
                    }
                    if (CSDL.Top3EndlineDF.Rows.Count > 0) // this is for end line
                    {
                        //EndlineDFList.Text = "";
                        ////EndlineDFList.Text = CSDL.Language("M00137") + ":\n";
                        //foreach (DataRow EndLineDFString in CSDL.Top3EndlineDF.Rows)
                        //{
                        //    EndlineDFList.Text += (CSDL.Top3EndlineDF.Rows.IndexOf(EndLineDFString) + 1).ToString() + ". " + EndLineDFString["DefectVN"].ToString().ToUpper() + ": " + EndLineDFString["RejCodeQty"].ToString() + "\n";
                        //}

                        piechart2.Model = CreateChart(CSDL.Top3EndlineDF.DefaultView.ToTable(false, "DefectVN", "RejCodeQty"), CSDL.Language("M00137"));
                    }
                    myLanguague = false;
                }
                else
                {
                    ActualEff.Text = CSDL.Language2nd("M00126");
                    ActualOutput.Text = CSDL.Language2nd("M00127");
                    ActualRFT.Text = CSDL.Language2nd("M00128");
                    InLineDF.Text = CSDL.Language2nd("M00129");
                    TargetPH = CSDL.Language2nd("M00130") + ": ";
                    LangTTTargetPCS = CSDL.Language2nd("M00131");
                    LangTTActualPCS = CSDL.Language2nd("M00132");
                    LangTTDFPCS = CSDL.Language2nd("M00133");
                    RunDay = CSDL.Language2nd("M00134") + ": ";
                    ProductType = CSDL.Language2nd("M00135") + ": ";
                    if (CSDL.Top3InlineDF.Rows.Count > 0)
                    {
                        InlineDF = 0;
                        InlineDFList.Text = "";
                        //InlineDFList.Text = CSDL.Language2nd("M00136") + ":\n";
                        foreach (DataRow InLineDFString in CSDL.Top3InlineDF.Rows)
                        {
                            //InlineDFList.Text += (CSDL.Top3InlineDF.Rows.IndexOf(InLineDFString) + 1).ToString() + ". " + InLineDFString["DFNAMEEN"].ToString().ToUpper() + ": " + InLineDFString["SUMDF"].ToString() + "\n";
                            InlineDF += string.IsNullOrEmpty(InLineDFString["SUMDF"].ToString()) ? 0 : int.Parse(InLineDFString["SUMDF"].ToString());
                        }

                        piechart1.Model = CreateChart(CSDL.Top3InlineDF.DefaultView.ToTable(false, "DFNAMEEN", "SUMDF"), CSDL.Language2nd("M00136"));
                    }
                    if (CSDL.Top3EndlineDF.Rows.Count > 0) // this is for endline
                    {
                        EndlineDFList.Text = "";
                        //EndlineDFList.Text = CSDL.Language2nd("M00137") + ":\n";
                        //foreach (DataRow EndLineDFString in CSDL.Top3EndlineDF.Rows)
                        //{
                        //    EndlineDFList.Text += (CSDL.Top3EndlineDF.Rows.IndexOf(EndLineDFString) + 1).ToString() + ". " + EndLineDFString["Description"].ToString().ToUpper() + ": " + EndLineDFString["RejCodeQty"].ToString() + "\n";
                        //}

                        piechart2.Model = CreateChart(CSDL.Top3EndlineDF.DefaultView.ToTable(false, "Description", "RejCodeQty"), CSDL.Language2nd("M00137"));
                    }
                    myLanguague = true;
                }
                TargetTotalPCS.Text = LangTTTargetPCS + "\n   " + ShowTTTargetPCS;
                ActualTotalPCS.Text = LangTTActualPCS + "\n   " + ShowTTActualPCS;
                TotalRJ.Text = LangTTDFPCS + "\n   " + ShowTTRJ;

                if (dtMyLineData.Rows.Count > 0) tvProductType.Text = ProductType + dtMyLineData.Rows[0]["GmtType"].ToString();
                if (dtMyLineData.Rows.Count > 0) tvRunDay.Text = RunDay + RunDayNum;
                if (dtMyLineData.Rows.Count > 0) tvTargetPH.Text = TargetPH + ((TargetPCS_total - ActualPCS_total) / ((ColumnCount < 8 ? 8 : ColumnCount) - ColumnCount + 1)).ToString("0") + (myLanguague ? " pcs" : (" " + CSDL.Language("M00236")));
            }
            catch (Exception ex) { Toast.MakeText(this, "ChangeLanguage() : " + ex.ToString(), ToastLength.Long).Show(); }
        }

        private void MyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                Get_Time();
                RefreshingView();
            });
        }
        private void ReSet()
        {
            ActualEff1.Text = "-----";
            ActualEff2.Text = "-----";
            ActualEff3.Text = "-----";
            ActualEff4.Text = "-----";
            ActualEff5.Text = "-----";
            ActualEff6.Text = "-----";
            ActualEff7.Text = "-----";
            ActualEff8.Text = "-----";
            ActualEff9.Text = "-----";
            ActualEff10.Text = "-----";
            ActualEff11.Text = "-----";
            ActualEff12.Text = "-----";

            ActualPCS1.Text = "-----";
            ActualPCS2.Text = "-----";
            ActualPCS3.Text = "-----";
            ActualPCS4.Text = "-----";
            ActualPCS5.Text = "-----";
            ActualPCS6.Text = "-----";
            ActualPCS7.Text = "-----";
            ActualPCS8.Text = "-----";
            ActualPCS9.Text = "-----";
            ActualPCS10.Text = "-----";
            ActualPCS11.Text = "-----";
            ActualPCS12.Text = "-----";

            ActualRFT1.Text = "-----";
            ActualRFT2.Text = "-----";
            ActualRFT3.Text = "-----";
            ActualRFT4.Text = "-----";
            ActualRFT5.Text = "-----";
            ActualRFT6.Text = "-----";
            ActualRFT7.Text = "-----";
            ActualRFT8.Text = "-----";
            ActualRFT9.Text = "-----";
            ActualRFT10.Text = "-----";
            ActualRFT11.Text = "-----";
            ActualRFT12.Text = "-----";

            ActualEff1.SetBackgroundColor(Color.LightGray);
            ActualEff2.SetBackgroundColor(Color.LightGray);
            ActualEff3.SetBackgroundColor(Color.LightGray);
            ActualEff4.SetBackgroundColor(Color.LightGray);
            ActualEff5.SetBackgroundColor(Color.LightGray);
            ActualEff6.SetBackgroundColor(Color.LightGray);
            ActualEff7.SetBackgroundColor(Color.LightGray);
            ActualEff8.SetBackgroundColor(Color.LightGray);
            ActualEff9.SetBackgroundColor(Color.LightGray);
            ActualEff10.SetBackgroundColor(Color.LightGray);
            ActualEff11.SetBackgroundColor(Color.LightGray);
            ActualEff12.SetBackgroundColor(Color.LightGray);

            InLineDF1.Text = "-----";
            InLineDF2.Text = "-----";
            InLineDF3.Text = "-----";
            InLineDF4.Text = "-----";
            InLineDF5.Text = "-----";
            InLineDF6.Text = "-----";

            InLineDF1.SetBackgroundColor(Color.LightGray);
            InLineDF2.SetBackgroundColor(Color.LightGray);
            InLineDF3.SetBackgroundColor(Color.LightGray);
            InLineDF4.SetBackgroundColor(Color.LightGray);
            InLineDF5.SetBackgroundColor(Color.LightGray);
            InLineDF6.SetBackgroundColor(Color.LightGray);

            piechart1.Model = null;
            piechart2.Model = null;

            //tvDT.Text = "";
            tvshowpercent.Text = "";
            tvWFT.Text = "";

            TargetTotalPCS.Text = "";
            ActualTotalPCS.Text = "";
            TotalRJ.Text = "";
            InlineDF = 0; EndlineDF = 0;
        }
        private void RefreshingView()
        {
            if (forceUpdate && newVer)
            {
                Toast.MakeText(this, "Update require !!", ToastLength.Long).Show();
                UpdateChecker();
            }
            else
            {
                try
                {
                    if (snLineList.GetItemAtPosition(mySpinerPositionID).ToString() != "FxAxx" && CSDL.SelectedLine != "")
                    {
                        CSDL.checkopen = 0;
                        dtMyLineData.Rows.Clear();
                        CSDL.Top3InlineDF.Rows.Clear();
                        CSDL.Top3EndlineDF.Rows.Clear();
                        //QrMyLineData.Clear();
                        tvSelectedLine.Text = CSDL.SelectedLine;

                        //get data from downtime database
                        try
                        {
                            //CSDL.chuoiDT = CSDL.chuoi.Replace("Initial Catalog=DtradeProduction", "Initial Catalog=Maintenance");
                            //SqlConnection con = new SqlConnection(CSDL.chuoiDT);
                            //con.Open();
                            //SqlCommand cmd = new SqlCommand("MyLineMcDTime", con) { CommandType = CommandType.StoredProcedure };
                            //cmd.Parameters.AddWithValue("@BeginDate", CSDL.BeginDate.ToString());
                            //cmd.Parameters.AddWithValue("@EndDate", CSDL.EndDate.ToString());
                            //cmd.Parameters.AddWithValue("@MyLine", CSDL.SelectedLine);
                            //cmd.ExecuteNonQuery();
                            //SqlDataAdapter MDTReader = new SqlDataAdapter { SelectCommand = cmd };
                            //DataTable MyLineDT = new DataTable();
                            //MDTReader.Fill(MyLineDT);
                            //con.Close();

                            Connect knn = new Connect(CSDL.chuoiDT);
                            DataTable MyLineDT = knn.Doc("exec MyLineMcDTime '" + CSDL.BeginDate + "','" + CSDL.EndDate + "','" + CSDL.SelectedLine + "'").Tables[0];

                            tvDT.Text = "Machine Downtime : " + "\n";
                            if (MyLineDT.Rows.Count > 0)
                            {
                                foreach (DataRow DTrow in MyLineDT.Rows)
                                {
                                    tvDT.Text += (MyLineDT.Rows.IndexOf(DTrow) + 1).ToString() + ". " + DTrow[0].ToString() + ": " + DTrow[1].ToString() + "\n";
                                }
                            }

                            //string mss = tvDT.Text;
                            //Android.App.AlertDialog.Builder b = new Android.App.AlertDialog.Builder(this);
                            //b.SetMessage(mss);
                            //b.Create().Show();

                        }
                        catch (Exception ex) { Toast.MakeText(this, "MyLineMcDTime: " + ex.Message.ToString(), ToastLength.Long).Show(); }

                        //get data from DtradeProduction
                        try
                        {
                            Connect kn = new Connect(CSDL.chuoi);
                            string qr = "exec InlineQcMyLineData '" + CSDL.BeginDate + "','" + CSDL.EndDate + "','" + CSDL.SelectedLine + "'";
                            QrMyLineData = kn.Doc(qr, 120);
                            //QrMyLineData = CSDL.Cnnt.Proc("InlineQcMyLineData", new List<string> { "@BeginDate=" + CSDL.BeginDate.ToString(), "@EndDate=" + CSDL.EndDate.ToString(), "@MyLine=" + CSDL.SelectedLine });
                            dtMyLineData = QrMyLineData.Tables[0];

                            //Android.App.AlertDialog.Builder b = new Android.App.AlertDialog.Builder(this);
                            //ListView ls = new ListView(this);
                            //ls.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(QrMyLineData.Tables[4], new List<int>(), true);
                            //b.SetView(ls);

                            //b.Create().Show();
                        }
                        catch (Exception ex) { Toast.MakeText(this, "InlineQcMyLineData: " + ex.Message.ToString(), ToastLength.Long).Show(); }

                        if (QrMyLineData != null)
                        {
                            ReSet(); tvlastrefresh.Text = "Last refresh : " + DateTime.Now.ToString();

                            //display data
                            if (dtMyLineData.Rows.Count > 0)
                            {
                                try
                                {
                                    Per1 = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per1"].ToString()) ? 0 : decimal.Parse(dtMyLineData.Rows[0]["Per1"].ToString());
                                    Per2 = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per2"].ToString()) ? 0 : decimal.Parse(dtMyLineData.Rows[0]["Per2"].ToString());
                                    Per3 = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per3"].ToString()) ? 0 : decimal.Parse(dtMyLineData.Rows[0]["Per3"].ToString());
                                    Per4 = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per4"].ToString()) ? 0 : decimal.Parse(dtMyLineData.Rows[0]["Per4"].ToString());
                                    Per5 = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per5"].ToString()) ? 0 : decimal.Parse(dtMyLineData.Rows[0]["Per5"].ToString());
                                    Per6 = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per6"].ToString()) ? 0 : decimal.Parse(dtMyLineData.Rows[0]["Per6"].ToString());
                                    Per7 = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per7"].ToString()) ? 0 : decimal.Parse(dtMyLineData.Rows[0]["Per7"].ToString());
                                    Per8 = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per8"].ToString()) ? 0 : decimal.Parse(dtMyLineData.Rows[0]["Per8"].ToString());
                                    Per9 = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per9"].ToString()) ? 0 : decimal.Parse(dtMyLineData.Rows[0]["Per9"].ToString());
                                    Per10 = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per10"].ToString()) ? 0 : decimal.Parse(dtMyLineData.Rows[0]["Per10"].ToString());
                                    Per11 = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per11"].ToString()) ? 0 : decimal.Parse(dtMyLineData.Rows[0]["Per11"].ToString());
                                    Per12 = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per12"].ToString()) ? 0 : decimal.Parse(dtMyLineData.Rows[0]["Per12"].ToString());

                                    ActualEff1.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per1"].ToString()) ? "-----" : Per1.ToString("0") + " %";
                                    ActualEff2.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per2"].ToString()) ? "-----" : Per2.ToString("0") + " %";
                                    ActualEff3.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per3"].ToString()) ? "-----" : Per3.ToString("0") + " %";
                                    ActualEff4.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per4"].ToString()) ? "-----" : Per4.ToString("0") + " %";
                                    ActualEff5.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per5"].ToString()) ? "-----" : Per5.ToString("0") + " %";
                                    ActualEff6.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per6"].ToString()) ? "-----" : Per6.ToString("0") + " %";
                                    ActualEff7.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per7"].ToString()) ? "-----" : Per7.ToString("0") + " %";
                                    ActualEff8.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per8"].ToString()) ? "-----" : Per8.ToString("0") + " %";
                                    ActualEff9.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per9"].ToString()) ? "-----" : Per9.ToString("0") + " %";
                                    ActualEff10.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per10"].ToString()) ? "-----" : Per10.ToString("0") + " %";
                                    ActualEff11.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per11"].ToString()) ? "-----" : Per11.ToString("0") + " %";
                                    ActualEff12.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["Per12"].ToString()) ? "-----" : Per12.ToString("0") + " %";

                                    ActualPCS1.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["LT1"].ToString()) ? "-----" : dtMyLineData.Rows[0]["LT1"].ToString() + " | " + dtMyLineData.Rows[0]["W01"].ToString();
                                    ActualPCS2.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["LT2"].ToString()) ? "-----" : dtMyLineData.Rows[0]["LT2"].ToString() + " | " + dtMyLineData.Rows[0]["W02"].ToString();
                                    ActualPCS3.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["LT3"].ToString()) ? "-----" : dtMyLineData.Rows[0]["LT3"].ToString() + " | " + dtMyLineData.Rows[0]["W03"].ToString();
                                    ActualPCS4.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["LT4"].ToString()) ? "-----" : dtMyLineData.Rows[0]["LT4"].ToString() + " | " + dtMyLineData.Rows[0]["W04"].ToString();
                                    ActualPCS5.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["LT5"].ToString()) ? "-----" : dtMyLineData.Rows[0]["LT5"].ToString() + " | " + dtMyLineData.Rows[0]["W05"].ToString();
                                    ActualPCS6.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["LT6"].ToString()) ? "-----" : dtMyLineData.Rows[0]["LT6"].ToString() + " | " + dtMyLineData.Rows[0]["W06"].ToString();
                                    ActualPCS7.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["LT7"].ToString()) ? "-----" : dtMyLineData.Rows[0]["LT7"].ToString() + " | " + dtMyLineData.Rows[0]["W07"].ToString();
                                    ActualPCS8.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["LT8"].ToString()) ? "-----" : dtMyLineData.Rows[0]["LT8"].ToString() + " | " + dtMyLineData.Rows[0]["W08"].ToString();
                                    ActualPCS9.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["LT9"].ToString()) ? "-----" : dtMyLineData.Rows[0]["LT9"].ToString() + " | " + dtMyLineData.Rows[0]["W09"].ToString();
                                    ActualPCS10.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["LT10"].ToString()) ? "-----" : dtMyLineData.Rows[0]["LT10"].ToString() + " | " + dtMyLineData.Rows[0]["W10"].ToString();
                                    ActualPCS11.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["LT11"].ToString()) ? "-----" : dtMyLineData.Rows[0]["LT11"].ToString() + " | " + dtMyLineData.Rows[0]["W11"].ToString();
                                    ActualPCS12.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["LT12"].ToString()) ? "-----" : dtMyLineData.Rows[0]["LT12"].ToString() + " | " + dtMyLineData.Rows[0]["W12"].ToString();

                                    ActualRFT1.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["RFT1"].ToString()) ? "-----" : decimal.Parse(dtMyLineData.Rows[0]["RFT1"].ToString()).ToString("0") + " %";
                                    ActualRFT2.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["RFT2"].ToString()) ? "-----" : decimal.Parse(dtMyLineData.Rows[0]["RFT2"].ToString()).ToString("0") + " %";
                                    ActualRFT3.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["RFT3"].ToString()) ? "-----" : decimal.Parse(dtMyLineData.Rows[0]["RFT3"].ToString()).ToString("0") + " %";
                                    ActualRFT4.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["RFT4"].ToString()) ? "-----" : decimal.Parse(dtMyLineData.Rows[0]["RFT4"].ToString()).ToString("0") + " %";
                                    ActualRFT5.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["RFT5"].ToString()) ? "-----" : decimal.Parse(dtMyLineData.Rows[0]["RFT5"].ToString()).ToString("0") + " %";
                                    ActualRFT6.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["RFT6"].ToString()) ? "-----" : decimal.Parse(dtMyLineData.Rows[0]["RFT6"].ToString()).ToString("0") + " %";
                                    ActualRFT7.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["RFT7"].ToString()) ? "-----" : decimal.Parse(dtMyLineData.Rows[0]["RFT7"].ToString()).ToString("0") + " %";
                                    ActualRFT8.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["RFT8"].ToString()) ? "-----" : decimal.Parse(dtMyLineData.Rows[0]["RFT8"].ToString()).ToString("0") + " %";
                                    ActualRFT9.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["RFT9"].ToString()) ? "-----" : decimal.Parse(dtMyLineData.Rows[0]["RFT9"].ToString()).ToString("0") + " %";
                                    ActualRFT10.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["RFT10"].ToString()) ? "-----" : decimal.Parse(dtMyLineData.Rows[0]["RFT10"].ToString()).ToString("0") + " %";
                                    ActualRFT11.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["RFT11"].ToString()) ? "-----" : decimal.Parse(dtMyLineData.Rows[0]["RFT11"].ToString()).ToString("0") + " %";
                                    ActualRFT12.Text = string.IsNullOrEmpty(dtMyLineData.Rows[0]["RFT12"].ToString()) ? "-----" : decimal.Parse(dtMyLineData.Rows[0]["RFT12"].ToString()).ToString("0") + " %";

                                }
                                catch (Exception ex) { Toast.MakeText(this, "DisplayData : " + ex.ToString(), ToastLength.Short).Show(); }
                            }
                            else { Toast.MakeText(this, CSDL.Language("M00043") + CSDL.SelectedLine + " " + dtMyLineData.Rows.Count.ToString(), ToastLength.Long).Show(); };

                            // add hourly inline defect--------------------------------------
                            try
                            {
                                DataTable InlineDFPCS = QrMyLineData.Tables[3];

                                foreach (DataRow dr in InlineDFPCS.Rows)
                                {
                                    int TmFrm = string.IsNullOrEmpty(dr[0].ToString()) ? 0 : int.Parse(dr[0].ToString());
                                    string QDeft = string.IsNullOrEmpty(dr[1].ToString()) ? "-----" : dr[1].ToString();
                                    switch (TmFrm)
                                    {
                                        case 0:
                                            Toast.MakeText(this, "Inline: OUT OF TIME FRAME", ToastLength.Short).Show();
                                            break;
                                        case 1:
                                            InLineDF1.Text = QDeft;

                                            if (QDeft != "-----")
                                            {
                                                InLineDF1.SetBackgroundColor(Android.Graphics.Color.Red);
                                                InLineDF1.SetTextColor(Android.Graphics.Color.White);
                                            }
                                            break;
                                        case 2:
                                            InLineDF2.Text = QDeft;

                                            if (QDeft != "-----")
                                            {
                                                InLineDF2.SetBackgroundColor(Android.Graphics.Color.Red);
                                                InLineDF2.SetTextColor(Android.Graphics.Color.White);
                                            }
                                            break;
                                        case 3:
                                            InLineDF3.Text = QDeft;

                                            if (QDeft != "-----")
                                            {
                                                InLineDF3.SetBackgroundColor(Android.Graphics.Color.Red);
                                                InLineDF3.SetTextColor(Android.Graphics.Color.White);
                                            }
                                            break;
                                        case 4:
                                            InLineDF4.Text = QDeft;

                                            if (QDeft != "-----")
                                            {
                                                InLineDF4.SetBackgroundColor(Android.Graphics.Color.Red);
                                                InLineDF4.SetTextColor(Android.Graphics.Color.White);
                                            }
                                            break;
                                        case 5:
                                            InLineDF5.Text = QDeft;

                                            if (QDeft != "-----")
                                            {
                                                InLineDF5.SetBackgroundColor(Android.Graphics.Color.Red);
                                                InLineDF5.SetTextColor(Android.Graphics.Color.White);
                                            }
                                            break;
                                        case 6:
                                            InLineDF6.Text = QDeft;

                                            if (QDeft != "-----")
                                            {
                                                InLineDF6.SetBackgroundColor(Android.Graphics.Color.Red);
                                                InLineDF6.SetTextColor(Android.Graphics.Color.White);
                                            }
                                            break;
                                            //case 7:
                                            //    InLineDF4.Text = QDeft;
                                            //    break;
                                            //case 8:
                                            //    InLineDF1.Text = QDeft;
                                            //    break;
                                            //case 9:
                                            //    InLineDF1.Text = QDeft;
                                            //    break;
                                            //case 10:
                                            //    InLineDF1.Text = QDeft;
                                            //    break;
                                            //case 11:
                                            //    InLineDF1.Text = QDeft;
                                            //    break;
                                            //case 12:
                                            //    InLineDF1.Text = QDeft;
                                            //    break;
                                    }
                                }
                                //InLineDF2.Text = string.IsNullOrEmpty(InlineDFPCS.Rows[1]["SUMDF"].ToString()) ? "-----" : InlineDFPCS.Rows[1]["SUMDF"].ToString();
                                //InLineDF3.Text = string.IsNullOrEmpty(InlineDFPCS.Rows[2]["SUMDF"].ToString()) ? "-----" : InlineDFPCS.Rows[2]["SUMDF"].ToString();
                                //InLineDF4.Text = string.IsNullOrEmpty(InlineDFPCS.Rows[3]["SUMDF"].ToString()) ? "-----" : InlineDFPCS.Rows[3]["SUMDF"].ToString();
                            }
                            catch (Exception ex) { Toast.MakeText(this, CSDL.Language("M00136") + " " + CSDL.Language("M00019") + "\n" + ex.Message, ToastLength.Short).Show(); }

                            //end-------------------------------------------------------------------
                            try
                            {
                                ColumnCount = 0;
                                foreach (DataColumn mycl in dtMyLineData.Columns)
                                {
                                    if (mycl.ColumnName.Substring(0, 2) == "LT" && !string.IsNullOrEmpty(dtMyLineData.Rows[0][mycl.ColumnName].ToString()))
                                    {
                                        ColumnCount++;
                                    }
                                }
                                TargetPCS_total = string.IsNullOrEmpty(dtMyLineData.Rows[0]["TargetPerDay"].ToString()) ? 0 : decimal.Parse(dtMyLineData.Rows[0]["TargetPerDay"].ToString()); //(ColumnCount > MyX ? ColumnCount : MyX) / 8;
                                ActualPCS_total = string.IsNullOrEmpty(dtMyLineData.Rows[0]["TotalPK"].ToString()) ? 0 : decimal.Parse(dtMyLineData.Rows[0]["TotalPK"].ToString());
                                ActualRFT_total = string.IsNullOrEmpty(dtMyLineData.Rows[0]["AvgRFT"].ToString()) ? 0 : decimal.Parse(dtMyLineData.Rows[0]["AvgRFT"].ToString());
                                AveEFFLine_total = string.IsNullOrEmpty(dtMyLineData.Rows[0]["AveEFFLine"].ToString()) ? 0 : decimal.Parse(dtMyLineData.Rows[0]["AveEFFLine"].ToString());
                                TargetEFF_total = string.IsNullOrEmpty(dtMyLineData.Rows[0]["TargetEFF"].ToString()) ? 0 : decimal.Parse(dtMyLineData.Rows[0]["TargetEFF"].ToString());
                                EndlineDF = string.IsNullOrEmpty(dtMyLineData.Rows[0]["TotalRJ"].ToString()) ? 0 : int.Parse(dtMyLineData.Rows[0]["TotalRJ"].ToString());

                                ShowTTTargetPCS = TargetPCS_total.ToString("0") + " | " + (string.IsNullOrEmpty(TargetEFF_total.ToString()) ? "0 " : decimal.Parse(TargetEFF_total.ToString()).ToString("0")) + "%";
                                ShowTTActualPCS = ActualPCS_total.ToString("0") + " | " + (string.IsNullOrEmpty(dtMyLineData.Rows[0]["AveEFFLine"].ToString()) ? "0" : decimal.Parse(dtMyLineData.Rows[0]["AveEFFLine"].ToString()).ToString("0")) + "%";
                                ShowTTRJ = (string.IsNullOrEmpty(dtMyLineData.Rows[0]["TotalRJ"].ToString()) ? "0" : decimal.Parse(dtMyLineData.Rows[0]["TotalRJ"].ToString()).ToString("0")) + " | " + ActualRFT_total.ToString("0") + "%";

                                tvshowpercent.Text = (string.IsNullOrEmpty(dtMyLineData.Rows[0]["AveEFFLine"].ToString()) ? "0" : decimal.Parse(dtMyLineData.Rows[0]["AveEFFLine"].ToString()).ToString("0")) + "%";

                                TargetTotalPCS.Text = LangTTTargetPCS + "\n   " + ShowTTTargetPCS;
                                ActualTotalPCS.Text = LangTTActualPCS + "\n   " + ShowTTActualPCS;
                                TotalRJ.Text = LangTTDFPCS + "\n   " + ShowTTRJ;

                                #region change color of cell
                                //int[] TimeTemp = new int[CSDL.TimeArray.Length];
                                //CSDL.TimeArray.CopyTo(TimeTemp, 0); //{ 1930, 1830, 1730, 1630, 1530, 1430, 1330, 1130, 1030, 930, 830, 730 };
                                //Array.Sort(TimeTemp);
                                //int[] TimeArray = new int[12];
                                //for (int mi = 0; mi < 12; mi++) TimeArray[mi] = TimeTemp[mi];
                                //Array.Reverse(TimeArray);
                                //int CurrentTime = int.Parse(DateTime.Now.ToString("HHmm"));
                                //int MyX = 0;
                                //for (int itime = 1; itime < TimeArray.Length; itime++)
                                //{
                                //    if (CurrentTime >= TimeArray[itime])
                                //    {
                                //        MyX = TimeArray.Length - itime;
                                //        break;
                                //    }
                                //}

                                decimal mWorkHour = string.IsNullOrEmpty(dtMyLineData.Rows[0]["WorkHour"].ToString()) ? 8 : decimal.Parse(dtMyLineData.Rows[0]["WorkHour"].ToString());

                                if (TargetPCS_total * ColumnCount / mWorkHour > ActualPCS_total) { ActualTotalPCS.SetBackgroundColor(Android.Graphics.Color.Red); }
                                else if (TargetPCS_total * ColumnCount / mWorkHour <= ActualPCS_total) { ActualTotalPCS.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF669900")); }

                                if (97 > ActualRFT_total) { TotalRJ.SetBackgroundColor(Android.Graphics.Color.Red); TotalRJ.SetTextColor(Color.White); }
                                else if (ActualRFT_total >= 97 && ActualRFT_total < 98) { TotalRJ.SetBackgroundColor(Android.Graphics.Color.Yellow); TotalRJ.SetTextColor(Color.Red); }
                                else { TotalRJ.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF669900")); TotalRJ.SetTextColor(Android.Graphics.Color.ParseColor("#FF00E676")); }

                                if (ActualEff1.Text.Length > 2 && ActualEff1.Text != "-----")
                                { if (Per1 >= TargetEFF_total) { ActualEff1.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF669900")); ActualEff1.SetTextColor(Android.Graphics.Color.White); } else { ActualEff1.SetBackgroundColor(Android.Graphics.Color.Red); ActualEff1.SetTextColor(Android.Graphics.Color.White); } }
                                else { ActualEff1.Background.ClearColorFilter(); ActualEff1.SetTextColor(Android.Graphics.Color.Black); }
                                if (ActualEff2.Text.Length > 2 && ActualEff2.Text != "-----")
                                { if (Per2 >= TargetEFF_total) { ActualEff2.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF669900")); ActualEff2.SetTextColor(Android.Graphics.Color.White); } else { ActualEff2.SetBackgroundColor(Android.Graphics.Color.Red); ActualEff2.SetTextColor(Android.Graphics.Color.White); } }
                                else { ActualEff2.Background.ClearColorFilter(); ActualEff2.SetTextColor(Android.Graphics.Color.Black); }
                                if (ActualEff3.Text.Length > 2 && ActualEff3.Text != "-----")
                                { if (Per3 >= TargetEFF_total) { ActualEff3.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF669900")); ActualEff3.SetTextColor(Android.Graphics.Color.White); } else { ActualEff3.SetBackgroundColor(Android.Graphics.Color.Red); ActualEff3.SetTextColor(Android.Graphics.Color.White); } }
                                else { ActualEff3.Background.ClearColorFilter(); ActualEff3.SetTextColor(Android.Graphics.Color.Black); }
                                if (ActualEff4.Text.Length > 2 && ActualEff4.Text != "-----")
                                { if (Per4 >= TargetEFF_total) { ActualEff4.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF669900")); ActualEff4.SetTextColor(Android.Graphics.Color.White); } else { ActualEff4.SetBackgroundColor(Android.Graphics.Color.Red); ActualEff4.SetTextColor(Android.Graphics.Color.White); } }
                                else { ActualEff4.Background.ClearColorFilter(); ActualEff4.SetTextColor(Android.Graphics.Color.Black); }
                                if (ActualEff5.Text.Length > 2 && ActualEff5.Text != "-----")
                                { if (Per5 >= TargetEFF_total) { ActualEff5.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF669900")); ActualEff5.SetTextColor(Android.Graphics.Color.White); } else { ActualEff5.SetBackgroundColor(Android.Graphics.Color.Red); ActualEff5.SetTextColor(Android.Graphics.Color.White); } }
                                else { ActualEff5.Background.ClearColorFilter(); ActualEff5.SetTextColor(Android.Graphics.Color.Black); }
                                if (ActualEff6.Text.Length > 2 && ActualEff6.Text != "-----")
                                { if (Per6 >= TargetEFF_total) { ActualEff6.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF669900")); ActualEff6.SetTextColor(Android.Graphics.Color.White); } else { ActualEff6.SetBackgroundColor(Android.Graphics.Color.Red); ActualEff6.SetTextColor(Android.Graphics.Color.White); } }
                                else { ActualEff6.Background.ClearColorFilter(); ActualEff6.SetTextColor(Android.Graphics.Color.Black); }
                                if (ActualEff7.Text.Length > 2 && ActualEff7.Text != "-----")
                                { if (Per7 >= TargetEFF_total) { ActualEff7.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF669900")); ActualEff7.SetTextColor(Android.Graphics.Color.White); } else { ActualEff7.SetBackgroundColor(Android.Graphics.Color.Red); ActualEff7.SetTextColor(Android.Graphics.Color.White); } }
                                else { ActualEff7.Background.ClearColorFilter(); ActualEff7.SetTextColor(Android.Graphics.Color.Black); }
                                if (ActualEff8.Text.Length > 2 && ActualEff8.Text != "-----")
                                { if (Per8 >= TargetEFF_total) { ActualEff8.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF669900")); ActualEff8.SetTextColor(Android.Graphics.Color.White); } else { ActualEff8.SetBackgroundColor(Android.Graphics.Color.Red); ActualEff8.SetTextColor(Android.Graphics.Color.White); } }
                                else { ActualEff8.Background.ClearColorFilter(); ActualEff8.SetTextColor(Android.Graphics.Color.Black); }
                                if (ActualEff9.Text.Length > 2 && ActualEff9.Text != "-----")
                                { if (Per9 >= TargetEFF_total) { ActualEff9.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF669900")); ActualEff9.SetTextColor(Android.Graphics.Color.White); } else { ActualEff9.SetBackgroundColor(Android.Graphics.Color.Red); ActualEff9.SetTextColor(Android.Graphics.Color.White); } }
                                else { ActualEff9.Background.ClearColorFilter(); ActualEff9.SetTextColor(Android.Graphics.Color.Black); }
                                if (ActualEff10.Text.Length > 2 && ActualEff10.Text != "-----")
                                { if (Per10 >= TargetEFF_total) { ActualEff10.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF669900")); ActualEff10.SetTextColor(Android.Graphics.Color.White); } else { ActualEff10.SetBackgroundColor(Android.Graphics.Color.Red); ActualEff10.SetTextColor(Android.Graphics.Color.White); } }
                                else { ActualEff10.Background.ClearColorFilter(); ActualEff10.SetTextColor(Android.Graphics.Color.Black); }
                                if (ActualEff11.Text.Length > 2 && ActualEff11.Text != "-----")
                                { if (Per11 >= TargetEFF_total) { ActualEff11.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF669900")); ActualEff11.SetTextColor(Android.Graphics.Color.White); } else { ActualEff11.SetBackgroundColor(Android.Graphics.Color.Red); ActualEff1.SetTextColor(Android.Graphics.Color.White); } }
                                else { ActualEff11.Background.ClearColorFilter(); ActualEff11.SetTextColor(Android.Graphics.Color.Black); }
                                if (ActualEff12.Text.Length > 2 && ActualEff12.Text != "-----")
                                { if (Per12 >= TargetEFF_total) { ActualEff12.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF669900")); ActualEff12.SetTextColor(Android.Graphics.Color.White); } else { ActualEff12.SetBackgroundColor(Android.Graphics.Color.Red); ActualEff12.SetTextColor(Android.Graphics.Color.White); } }
                                else { ActualEff12.Background.ClearColorFilter(); ActualEff12.SetTextColor(Android.Graphics.Color.Black); }
                                #endregion
                            }
                            catch (Exception ex) { Toast.MakeText(this, "Color : " + "\n" + ex.Message, ToastLength.Short).Show(); }

                            //check realtime eff
                            if (ActualPCS1.Text != "-----" && ActualPCS1.Text.Split('|')[1].ToString().Trim().Length == 0) InlineScanOPCalRTime(1, ActualEff1);
                            if (ActualPCS2.Text != "-----" && ActualPCS2.Text.Split('|')[1].ToString().Trim().Length == 0) InlineScanOPCalRTime(2, ActualEff2);
                            if (ActualPCS3.Text != "-----" && ActualPCS3.Text.Split('|')[1].ToString().Trim().Length == 0) InlineScanOPCalRTime(3, ActualEff3);
                            if (ActualPCS4.Text != "-----" && ActualPCS4.Text.Split('|')[1].ToString().Trim().Length == 0) InlineScanOPCalRTime(4, ActualEff4);
                            if (ActualPCS5.Text != "-----" && ActualPCS5.Text.Split('|')[1].ToString().Trim().Length == 0) InlineScanOPCalRTime(5, ActualEff5);
                            if (ActualPCS6.Text != "-----" && ActualPCS6.Text.Split('|')[1].ToString().Trim().Length == 0) InlineScanOPCalRTime(6, ActualEff6);
                            if (ActualPCS7.Text != "-----" && ActualPCS7.Text.Split('|')[1].ToString().Trim().Length == 0) InlineScanOPCalRTime(7, ActualEff7);
                            if (ActualPCS8.Text != "-----" && ActualPCS8.Text.Split('|')[1].ToString().Trim().Length == 0) InlineScanOPCalRTime(8, ActualEff8);
                            if (ActualPCS9.Text != "-----" && ActualPCS9.Text.Split('|')[1].ToString().Trim().Length == 0) InlineScanOPCalRTime(9, ActualEff8);
                            if (ActualPCS10.Text != "-----" && ActualPCS10.Text.Split('|')[1].ToString().Trim().Length == 0) InlineScanOPCalRTime(10, ActualEff8);
                            if (ActualPCS11.Text != "-----" && ActualPCS11.Text.Split('|')[1].ToString().Trim().Length == 0) InlineScanOPCalRTime(11, ActualEff8);
                            if (ActualPCS12.Text != "-----" && ActualPCS12.Text.Split('|')[1].ToString().Trim().Length == 0) InlineScanOPCalRTime(12, ActualEff8);


                            //Top 3 defect & WFT
                            try
                            {
                                InlineDFList.Text = "";
                                CSDL.Top3InlineDF = QrMyLineData.Tables[2];
                                if (CSDL.Top3InlineDF.Rows.Count > 0)
                                {
                                    InlineDF = 0;
                                    //InlineDFList.Text = CSDL.Language("M00136") + ":\n";
                                    foreach (DataRow InLineDFString in CSDL.Top3InlineDF.Rows)
                                    {
                                        //InlineDFList.Text += (CSDL.Top3InlineDF.Rows.IndexOf(InLineDFString) + 1).ToString() + ". " + InLineDFString["DFNAMEEN"].ToString().ToUpper() + ": " + InLineDFString["SUMDF"].ToString() + "\n";
                                        InlineDF += string.IsNullOrEmpty(InLineDFString["SUMDF"].ToString()) ? 0 : int.Parse(InLineDFString["SUMDF"].ToString());
                                    }

                                    piechart1.Model = CreateChart(CSDL.Top3InlineDF.DefaultView.ToTable(false, "DFNAMEEN", "SUMDF"), "TOP 5 Inline Defect");
                                }
                                tvWFT.Text = "WFT \n " + ((InlineDF + EndlineDF) * 100 / ActualPCS_total).ToString("0.0") + " %";
                            }
                            catch (Exception ex) { Toast.MakeText(this, CSDL.Language("M00136") + " " + CSDL.Language("M00019") + "\n" + ex.Message, ToastLength.Short).Show(); }
                            //end-------------------------------------------------------------------

                            //list of style and smv-------------------------------------------------
                            DataTable MyLineStyleList = QrMyLineData.Tables[5];
                            if (MyLineStyleList.Rows.Count > 0)
                            {
                                tvSMV.Text = "";
                                foreach (DataRow myr in MyLineStyleList.Rows)
                                {
                                    tvSMV.Text += myr[0].ToString() + ": " + (string.IsNullOrEmpty(myr[1].ToString()) ? "0.00" : decimal.Parse(myr[1].ToString()).ToString("0.00")) + "\n";
                                }
                            }
                            try
                            {
                                //running day
                                DataTable RunningDayList = QrMyLineData.Tables[6];
                                if (RunningDayList.Rows.Count == 2 && MyLineStyleList.Rows.Count == 1)
                                {
                                    DateTime lastdate = DateTime.Parse(RunningDayList.Rows[0][1].ToString());
                                    DateTime firstdate = DateTime.Parse(RunningDayList.Rows[1][1].ToString());
                                    RunDayNum = (lastdate - firstdate).TotalDays.ToString() + "|" + RunningDayList.Rows[0][2].ToString();
                                }
                                else
                                {
                                    RunDayNum = "NA";
                                }
                            }
                            catch (Exception ex) { Toast.MakeText(this, "Runday : " + "\n" + ex.Message, ToastLength.Short).Show(); }
                            //top 3 endline defect
                            try
                            {
                                EndlineDFList.Text = "";
                                CSDL.Top3EndlineDF = QrMyLineData.Tables[4];

                                if (CSDL.Top3EndlineDF.Rows.Count > 0)
                                {
                                    //EndlineDFList.Text = CSDL.Language("M00137") + ":\n";
                                    //foreach (DataRow EndLineDFString in CSDL.Top3EndlineDF.Rows)
                                    //{
                                    //    EndlineDFList.Text += (CSDL.Top3EndlineDF.Rows.IndexOf(EndLineDFString) + 1).ToString() + ". " + EndLineDFString["Description"].ToString().ToUpper() + ": " + EndLineDFString["RejCodeQty"].ToString() + "\n";
                                    //}

                                    piechart2.Model = CreateChart(CSDL.Top3EndlineDF.DefaultView.ToTable(false, "Description", "RejCodeQty"), "TOP 5 Endline Defect");
                                }
                            }
                            catch (Exception ex) { Toast.MakeText(this, CSDL.Language("M00137") + " " + CSDL.Language("M00019") + "\n" + ex.Message, ToastLength.Short).Show(); }
                            //generate chart---------------------------------------------
                            try
                            {
                                DataTable MyLineChartData = QrMyLineData.Tables[1];
                                List<ChartEntry> entries = new List<ChartEntry>();
                                List<ChartEntry> entries2 = new List<ChartEntry>();
                                foreach (DataRow ChartRow in MyLineChartData.Rows)
                                {
                                    if (!string.IsNullOrEmpty(ChartRow[0].ToString()))
                                    {
                                        float vl = float.Parse(ChartRow[0].ToString());

                                        if (vl > 0)
                                        {
                                            ChartEntry Entr2 = new ChartEntry(vl)
                                            {
                                                Label = DateTime.Parse(ChartRow[1].ToString()).ToString("dd"),
                                                ValueLabel = float.Parse(ChartRow[0].ToString()).ToString("0.0") + "%",
                                                Color = SKColor.Parse("#266489")
                                            };
                                            entries2.Add(Entr2);
                                            ChartEntry Entr = new ChartEntry(float.Parse(ChartRow[2].ToString()))
                                            {
                                                Label = DateTime.Parse(ChartRow[1].ToString()).ToString("dd"),
                                                ValueLabel = float.Parse(ChartRow[2].ToString()).ToString("0") + " pcs",
                                                Color = SKColor.Parse("#266489")
                                            };
                                            entries.Add(Entr);
                                        }
                                    }
                                }
                                ChartEntry EnTrTT2 = new ChartEntry((float)AveEFFLine_total)
                                {
                                    Label = "Today",
                                    ValueLabel = AveEFFLine_total.ToString("0.0") + "%",
                                    Color = TargetEFF_total > AveEFFLine_total ? SKColor.Parse("#FF0000") : SKColor.Parse("#FF669900")
                                };
                                entries2.Add(EnTrTT2); //add "today PCS" in chart1
                                ChartEntry EnTrTT = new ChartEntry((float)ActualPCS_total)
                                {
                                    Label = "Today",
                                    ValueLabel = ActualPCS_total.ToString("0") + " pcs",
                                    Color = TargetEFF_total > AveEFFLine_total ? SKColor.Parse("#FF0000") : SKColor.Parse("#FF669900")
                                };
                                entries.Add(EnTrTT);//add "today EFF" in chart2

                                chart = new BarChart() { Entries = entries };
                                chartView.Chart = chart;

                                chart2 = new LineChart() { Entries = entries2 };
                                chartView2.Chart = chart2;

                                //end chart--------------------------------

                                try
                                {
                                    DataTable wip = kn.Doc("exec EndlineSewingLineBalacing 5,'" + CSDL.SelectedLine.Substring(0, 2) + "','" + CSDL.SelectedLine + "'").Tables[0];

                                    if (wip.Rows.Count > 0) txtlinewip.Text = "Line WIP : " + wip.Rows[0][0].ToString() + "|" + wip.Rows[0][1].ToString();
                                    else txtlinewip.Text = "Line WIP : ";
                                }
                                catch { }
                            }
                            catch (Exception ex) { Toast.MakeText(this, "PlotGraph : " + "\n" + ex.ToString(), ToastLength.Short).Show(); }
                            Toast.MakeText(this, CSDL.Language("M00138"), ToastLength.Long).Show();
                        }
                    }

                    //tvDT.Text = strTest;
                }
                catch (Exception ex1) { Toast.MakeText(this, "RefreshingView() : " + ex1.Message.ToString(), ToastLength.Short).Show(); }
            }

        }
        private PlotModel CreateChart(DataTable dt, string tt)
        {
            PlotModel MyModel = new PlotModel();

            List<Tuple<string, int>> data = new List<Tuple<string, int>>();

            foreach (DataRow r in dt.Rows)
            {
                data.Add(new Tuple<string, int>(r[0].ToString(), int.Parse(r[1].ToString())));
            }

            PieSeries pieSeries = new PieSeries();
            pieSeries.FontSize = rq.eTextSize(10);
            pieSeries.TextColor = OxyColors.Black;
            pieSeries.InsideLabelColor = OxyColors.Black;
            pieSeries.StrokeThickness = 1;

            foreach (Tuple<string, int> t in data)
                pieSeries.Slices.Add(new PieSlice(t.Item1, t.Item2));

            MyModel.Series.Add(pieSeries);

            MyModel.Title = tt;
            MyModel.TitleColor = OxyColors.Teal;
            MyModel.TitleFontSize = rq.eTextSize(15);

            MyModel.Axes.Add(new LinearAxis() { Position = AxisPosition.Bottom, IsAxisVisible = false });
            MyModel.Axes.Add(new LinearAxis() { Position = AxisPosition.Left, IsAxisVisible = false });

            int total = data.Sum(p => p.Item2);

            TextAnnotation annotation = new TextAnnotation();
            annotation.Text = total.ToString();//string.Format("{0:C2}", total);
            annotation.TextColor = OxyColors.Red;
            annotation.Stroke = OxyColors.Red;
            annotation.StrokeThickness = rq.eTextSize(2);
            annotation.FontSize = rq.eTextSize(10);
            annotation.TextPosition = new DataPoint(5, 10);

            MyModel.Annotations.Add(annotation);

            return MyModel;
        }
        private void InlineScanOPCalRTime(int ArrItemCutofTime, TextView tvEff)
        {
            try
            {
                string MPHour = "01";
                float MP = 0; float EarnM = 0; float InveM = 0;
                tvEff.Background.ClearColorFilter();

                if (ArrItemCutofTime > 1) MPHour = (ArrItemCutofTime - 1).ToString("00");

                DataTable dt = CSDL.Cnnt.Proc("InlineScanOPCalRTime", new List<string> { "@FacLine=" + CSDL.SelectedLine, "@FDMark=" + ArrItemCutofTime.ToString(), "@MDate=" + CSDL.EndDate.ToString(), "@MCol=W" + MPHour }).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (!string.IsNullOrEmpty(dr[6].ToString())) MP = float.Parse(dr[6].ToString());
                        EarnM += (string.IsNullOrEmpty(dr[2].ToString()) ? 0 : float.Parse(dr[2].ToString())) * (string.IsNullOrEmpty(dr[4].ToString()) ? 0 : float.Parse(dr[4].ToString()));
                    }

                    int CrTime = int.Parse(hh.ToString("00") + mm.ToString("00"));
                    int bkT = 0;

                    if (CrTime > CSDL.tmBrStr && CrTime < CSDL.tmBrEnd) bkT = CrTime - CSDL.tmBrStr;
                    else if (CSDL.TimeArray[ArrItemCutofTime - 1] <= CSDL.tmBrStr && CrTime >= CSDL.tmBrEnd && CrTime <= CSDL.TimeArray[ArrItemCutofTime])
                    {
                        int mHour = CSDL.tmBrEnd / 100 - CSDL.tmBrStr / 100;
                        int mMint = CSDL.tmBrEnd % 100 - CSDL.tmBrStr % 100;

                        bkT = mHour * 60 + mMint;
                        if (bkT < 0) bkT = 0;
                    }

                    int prHour = CrTime / 100 - CSDL.TimeArray[ArrItemCutofTime - 1] / 100;
                    int prMint = CrTime % 100 - CSDL.TimeArray[ArrItemCutofTime - 1] % 100;

                    int pssMin = prHour * 60 + prMint - bkT;

                    InveM = (MP * pssMin) <= 0 ? MP : (MP * (pssMin > 60 ? 60 : pssMin));

                    strTest = "CrTime=" + CrTime + " pssMin=" + pssMin.ToString() + " Ar[-1]=" + CSDL.TimeArray[ArrItemCutofTime - 1].ToString() + " bkT=" + bkT + " InveM=" + InveM + " ArrItemCutofTime= " + ArrItemCutofTime + " tmBrEnd=" + CSDL.tmBrEnd + " tmBrStr=" + CSDL.tmBrStr;

                    float CrEff = 100 * EarnM / InveM;

                    tvEff.Text = CrEff.ToString("00") + " %";
                    if (CrEff >= float.Parse(TargetEFF_total.ToString())) tvEff.SetBackgroundColor(Android.Graphics.Color.ParseColor("#FF669900"));
                    else tvEff.SetBackgroundColor(Android.Graphics.Color.Red);

                }
            }
            catch (Exception ext)
            {
                tvEff.Text = "invl";
                Toast.MakeText(this, "InlineScanOPCalRTime() : " + ext.Message.ToString(), ToastLength.Short).Show();
            }
        }
        private void SpinnerInitialize()
        {
            try
            {
                ISharedPreferences pre = GetSharedPreferences("FacSel", FileCreationMode.Private);
                Fac = pre.GetString("Fac", "").ToString();

                string ch = " select distinct left(FacLine,2) from cpdtlsdays where len(left(FacLine,2)) > 1"
                            + " select * from InLLanguageMst "
                            + " select * from InLMeasageMaster where substring(MeasgCode,1,2) = 'M0' ";
                myLineList = CSDL.Cnnt.Doc(ch);

                List<string> FacList = new List<string>();
                if (Fac == "") FacList.Add("Fx");
                else FacList.Add(Fac);

                foreach (DataRow myr in myLineList.Tables[0].Rows) if (!FacList.Contains(myr[0].ToString())) FacList.Add(myr[0].ToString());
                ArrayAdapter adater1 = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, FacList);
                adater1.SetDropDownViewResource(Android.Resource.Layout.SimpleListItemSingleChoice);
                snFacList.Adapter = adater1;
                //Toast.MakeText(this, "FacList" + myLineList.Tables[0].Rows.Count, ToastLength.Long).Show();

                pre = GetSharedPreferences("LangSel", FileCreationMode.Private);
                string f = pre.GetString("Lang", "").ToString();

                List<string> LangList = new List<string>();
                foreach (DataRow myr in myLineList.Tables[1].Rows) LangList.Add(myr[0].ToString());
                ArrayAdapter adater2 = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, LangList);
                adater2.SetDropDownViewResource(Android.Resource.Layout.SimpleListItemSingleChoice);
                snLangList.Adapter = adater2;
                if (LangList.Contains(f)) snLangList.SetSelection(LangList.IndexOf(f));

            }
            catch (Exception ex) { Toast.MakeText(this, "SpinnerInitialize() : " + ex.Message.ToString(), ToastLength.Long).Show(); }
        }

        void DateSelect_OnClick()
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                if (BeginOrEnd)
                {
                    tvBeginDate.Text = time.ToString("MMMM dd, yyyy").ToUpper();
                    CSDL.BeginDate = time.ToString("yyyyMMdd");
                    Toast.MakeText(this, CSDL.BeginDate, ToastLength.Short).Show();
                } //ToLongDateString();
                else
                {
                    tvEndDate.Text = time.ToString("MMMM dd, yyyy").ToUpper();
                    CSDL.EndDate = time.ToString("yyyyMMdd");
                    Toast.MakeText(this, CSDL.EndDate, ToastLength.Short).Show();
                }
                SpinnerInitialize();
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }


        private async void ProgressCalling()
        {
            Progr++;
            using (IProgressDialog pros = UserDialogs.Instance.Progress("Downloading", null, null, true, MaskType.Black))
            {
                do
                {
                    pros.PercentComplete = PercentComplete;
                    await Task.Delay(500);
                } while (PercentComplete < 100);
            }
            Progr = 0;
        }

        private async void ShowingLoading(string Tiltle, int lenght)
        {
            using (IProgressDialog progress = UserDialogs.Instance.Progress(Tiltle, null, null, true, MaskType.Black))
            {
                int prog = 0;
                do
                {
                    progress.PercentComplete = prog * 100 / lenght;
                    await Task.Delay(1000);
                    prog++;
                } while (prog <= lenght);
            }

        }

        private static void mToast(string ToastMssg, int ToastLength)
        {
            ToastConfig toastconfig = new ToastConfig(ToastMssg);
            toastconfig.SetDuration(ToastLength);
            toastconfig.SetBackgroundColor(System.Drawing.Color.DimGray);
            UserDialogs.Instance.Toast(toastconfig);
        }

        public void downloadwithauthorization(string apkurl)
        {
            try
            {
                //successfully download image
                //var dm = (DownloadManager)GetSystemService(Context.DownloadService);
                //string webUri = apkurl;
                //var dir = new Java.IO.File(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/Download/");
                //if (!dir.Exists())
                //    dir.Mkdirs();
                //var uri = Android.Net.Uri.FromFile(new Java.IO.File(dir.AbsolutePath + "/Test.jpg"));
                //var request = new DownloadManager.Request(Android.Net.Uri.Parse(webUri));
                //request.SetDestinationUri(uri);
                //dm.Enqueue(request);


                //successfully download image
                //string m_filePath = Android.OS.Environment.ExternalStorageDirectory + "/download/" + "Test.jpg";
                //WebClient web = new WebClient();
                //byte[] dwldfile = web.DownloadData(PathName);
                //System.IO.File.WriteAllBytes(m_filePath, dwldfile);


                DownloadCompleteReceiver receiver;
                var user = "cho";
                var password = "?"; //new Random().Next(int.MinValue, int.MaxValue).ToString();
                var uriString = apkurl;

                using (var uri = Android.Net.Uri.Parse(uriString))
                using (var request = new DownloadManager.Request(uri))
                {
                    var basicAuthentication = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));
                    request.AddRequestHeader("Authorization", $"Basic {basicAuthentication}");
                    request.SetNotificationVisibility(DownloadVisibility.VisibleNotifyCompleted);
                    request.SetDestinationInExternalPublicDir(Android.OS.Environment.DirectoryDownloads, "Test.jpg");
                    using (var downloadManager = (DownloadManager)GetSystemService(DownloadService))
                    {
                        var id = downloadManager.Enqueue(request);
                        receiver = new DownloadCompleteReceiver(id, (sender, e) =>
                        {
                            Toast.MakeText(Application.Context, $"Download Complete {id}", ToastLength.Long).Show();
                            if (sender is DownloadCompleteReceiver rec)
                            {
                                UnregisterReceiver(rec);
                                rec.Dispose();
                            }
                        });
                        RegisterReceiver(receiver, new IntentFilter(DownloadManager.ActionDownloadComplete));
                        Toast.MakeText(Application.Context, $"Downloading File: {id}", ToastLength.Short).Show();
                    }
                }
            }
            catch (IOException e)
            {
                Toast.MakeText(this, "download error! " + e.Message, ToastLength.Long).Show();
            }
        }


    }
    public class CheckListItem
    {
        public TextView Txt { get; set; }
        public string Value { get; set; } = "";
        public string CheckListID { get; set; }
    }
    public class DatePickerFragment : DialogFragment,
                                      DatePickerDialog.IOnDateSetListener
    {
        // TAG can be any string of your choice.
        public static readonly string TAG = "X:" + typeof(DatePickerFragment).Name.ToUpper();

        // Initialize this value to prevent NullReferenceExceptions.
        Action<DateTime> dateSelectedHandler = delegate { };

        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected)
        {
            DatePickerFragment frag = new DatePickerFragment
            {
                dateSelectedHandler = onDateSelected
            };
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            DateTime currently = CSDL.tg;
            DatePickerDialog dialog = new DatePickerDialog(Activity,
                                                           this,
                                                           currently.Year,
                                                           currently.Month - 1,
                                                           currently.Day);
            return dialog;
        }

        public void OnDateSet(Android.Widget.DatePicker view, int year, int monthOfYear, int dayOfMonth)
        {
            // Note: monthOfYear is a value between 0 and 11, not 1 and 12!
            DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
            Android.Util.Log.Debug(TAG, selectedDate.ToString("MMMM dd, yyyy")); //ToLongDateString()ToString("MMMM dd, yyyy")
            dateSelectedHandler(selectedDate);
        }
    }
}

