using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CSDL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using A1ATeam;
using System.Collections;
using Android.Util;

namespace SEWTVDisplay
{
    [Activity(Theme = "@style/Theme.AppCompat.Light.NoActionBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.SensorLandscape)]
    public class Kanban : Activity
    {
        ListView mainlist, lvsumqty, lvwip;
        TextView txtdate, txtjob, txtcolor, txtsize, txtpo, txtqty;
        Button btdate, btjob, btadd;
        RelativeLayout lymain;
        Connect kn;
        A1ATeam.LayoutRequest request;
        List<string> lsjob = new List<string>();
        List<string> lscolor = new List<string>();
        List<string> lssize = new List<string>();
        List<string> lspo = new List<string>();
        DataTable plan = new DataTable();
        DateTime date;
        Android.Util.DisplayMetrics metric = Application.Context.Resources.DisplayMetrics;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.layout1);

            kn = new Connect(CSDL.chuoi);
            Window.AddFlags(WindowManagerFlags.Fullscreen);

            #region initalize
            mainlist = FindViewById<ListView>(Resource.Id.listView1);
            lvsumqty = FindViewById<ListView>(Resource.Id.lstsumqty);
            lvwip = FindViewById<ListView>(Resource.Id.lstwip);

            txtdate = FindViewById<TextView>(Resource.Id.txtdate);
            txtjob = FindViewById<TextView>(Resource.Id.txtjob);
            txtcolor = FindViewById<TextView>(Resource.Id.txtcolor);
            txtsize = FindViewById<TextView>(Resource.Id.txtsize);
            txtpo = FindViewById<TextView>(Resource.Id.txtpo);
            txtqty = FindViewById<TextView>(Resource.Id.txtqty);

            btdate = FindViewById<Button>(Resource.Id.btdate);
            btjob = FindViewById<Button>(Resource.Id.btjob);
            btadd = FindViewById<Button>(Resource.Id.btadd);

            if (CSDL.tg.DayOfWeek == DayOfWeek.Saturday) date = CSDL.tg.AddDays(2);
            else date = CSDL.tg.AddDays(1);

            txtdate.Text = date.ToString("dd-MM-yyyy");

            lymain = FindViewById<RelativeLayout>(Resource.Id.layoutkanban);
            #endregion

            request = new LayoutRequest(1280, 752, 1);//new A1ATeam.LayoutRequest(1920, 1280, 1.5f);
            request.Steching(lymain);

            #region Layout
            //request layout
            try
            {
                //if (lymain.ChildCount > 0)
                //{
                //    for (int i = 0; i < lymain.ChildCount; i++)
                //    {
                //        View v = lymain.GetChildAt(i);

                //        ViewGroup.MarginLayoutParams mr = v.LayoutParameters as ViewGroup.MarginLayoutParams;

                //        mr.SetMargins(request.Widht(mr.LeftMargin), request.Height(mr.TopMargin), request.Widht(mr.RightMargin), request.Height(mr.BottomMargin));

                //        if (mr.Width > 0) mr.Width = request.Widht(mr.Width);
                //        if (mr.Height > 0) mr.Height = request.Height(mr.Height);

                //        v.LayoutParameters = mr;

                //        if (v.GetType().ToString().Contains("TextView"))
                //        {
                //            TextView ed = v as TextView;

                //            ed.TextSize = request.TextSize(20);// ed.TextSize);
                //        }
                //        if (v.GetType().ToString().Contains("Button"))
                //        {
                //            Button ed = v as Button;

                //            ed.TextSize = request.TextSize(20);// ed.TextSize);
                //        }
                //    }
                //}

                plan = kn.Doc("exec GetLoadData 41,'" + CSDL.SelectedLine + "',''", 120).Tables[0];

                if (plan.Rows.Count > 0) lsjob = plan.Select().Select(s => s["Job_No"].ToString()).Distinct().ToList();//{ lsjob = plan.DefaultView.ToTable(true, "Job_No").Select().Select(s => s[0].ToString()).ToList(); lsjob.Sort(); }
            }
            catch { }
            #endregion

            RefreshKanban();
            LoadWIP();

            txtjob.Click += (s, e) => { PickUp(txtjob, lsjob, false); txtcolor.Text = "ALl Color"; txtsize.Text = "All Size"; txtpo.Text = "All PO"; txtqty.Text = "-vetPO-"; };
            txtcolor.Click += (s, e) => PickUp(txtcolor, lscolor, false);
            txtsize.Click += (s, e) => PickUp(txtsize, lssize, false);
            txtpo.Click += (s, e) => PickUp(txtpo, lspo, false);
            txtqty.Click += (s, e) => PickUp(txtqty, new List<string> { "-vetPO-", "100", "200", "300", "400", "500", "1000", "1500" }, true);

            btdate.Click += delegate
            {
                try
                {
                    Android.App.AlertDialog.Builder b = new AlertDialog.Builder(this);

                    DatePicker dd = new DatePicker(this);

                    b.SetView(dd);
                    b.SetPositiveButton("OK", (s, a) =>
                    {
                        date = dd.DateTime;
                        txtdate.Text = date.ToString("dd/MM/yyyy");
                    });
                    Dialog d = b.Create();
                    d.Show();

                    dd.DateChanged += (s, a) =>
                    {
                        date = dd.DateTime;
                        txtdate.Text = date.ToString("dd/MM/yyyy");
                        d.Dismiss();
                    };
                }
                catch { }
            };
            btdate.LongClick += delegate
            {
                //DisplayMetrics mtr = Resources.DisplayMetrics;
                //Toast.MakeText(this, mtr.WidthPixels + " - " + mtr.HeightPixels + " - " + mtr.Density, ToastLength.Short).Show();

                Toast.MakeText(this, txtdate.TextSize.ToString(), ToastLength.Short).Show();
                //txtdate.TextSize = 20 / 1.5f;
            };
            btjob.Click += delegate
            {
                try
                {
                    lscolor.Clear(); lssize.Clear(); lspo.Clear();

                    DataRow[] filter = plan.Select("Job_No = '" + txtjob.Text + "'");
                    lscolor = filter.Select(s => s["Color"].ToString()).Distinct().ToList(); lscolor.Sort(); lscolor.Insert(0, "All Color");
                    lspo = filter.Select(s => s["PONO"].ToString()).Distinct().ToList(); lspo.Sort(); lspo.Insert(0, "All PO");

                    DataTable size = kn.Doc("select * from sahasm where OrderNo = '" + txtjob.Text + "'").Tables[0];
                    lssize = size.Select().Select(s => s["Sizx"].ToString()).Distinct().ToList(); lssize.Sort(); lssize.Insert(0, "All Size");

                    Toast.MakeText(this, "Data is loaded !!", ToastLength.Long).Show();
                }
                catch { }
            };
            btadd.Click += delegate
            {
                if (txtdate.Text == "") Toast.MakeText(this, "Date is empty !!!", ToastLength.Long).Show();
                else if (txtjob.Text == "") Toast.MakeText(this, "Job is empty !!!", ToastLength.Long).Show();
                else if (txtcolor.Text == "") Toast.MakeText(this, "Color is empty !!!", ToastLength.Long).Show();
                else if (txtsize.Text == "") Toast.MakeText(this, "Size is empty !!!", ToastLength.Long).Show();
                else if (txtpo.Text == "") Toast.MakeText(this, "PO is empty !!!", ToastLength.Long).Show();
                else if (txtqty.Text == "") Toast.MakeText(this, "Qty is empty !!!", ToastLength.Long).Show();
                else
                {
                    try
                    {
                        string ch = "insert into InlineQcKanban (DateNeed,FacLine,JobNo,ColorCode,Size,PONo,ReqQty,RecordDate,KBStatus ) values ('"
                                    + date.ToString("yyyyMMdd") + "','" + CSDL.SelectedLine + "','" + txtjob.Text + "','" + txtcolor.Text + "','" + txtsize.Text + "','" + txtpo.Text + "','" + txtqty.Text + "',getdate(),'Wait') ";

                        kn.Doc(ch);

                        if (kn.ErrorMessage == "")
                        {
                            Toast.MakeText(this, "Done !!!", ToastLength.Long).Show();

                            RefreshKanban();
                        }
                        else Toast.MakeText(this, kn.ErrorMessage, ToastLength.Long).Show();
                    }
                    catch { }
                }
            };

            mainlist.ItemLongClick += (s, e) =>
            {
                try
                {
                    LinearLayout ln = A1ATeam.CustomListView.GetViewByPosition(e.Position, mainlist) as LinearLayout;

                    string stt = ((TextView)ln.GetChildAt(0)).Text;

                    Android.App.AlertDialog.Builder b = new AlertDialog.Builder(this);

                    b.SetMessage("Are you sure delete this record - " + stt + " ?");
                    b.SetPositiveButton("YES", (ss, ee) =>
                    {
                        kn.Doc("delete from InlineQcKanban where Id = " + stt);

                        if (kn.ErrorMessage == "")
                        {
                            Toast.MakeText(this, "Done !!!", ToastLength.Long).Show();

                            RefreshKanban();
                        }
                        else Toast.MakeText(this, kn.ErrorMessage, ToastLength.Long).Show();
                    });
                    b.SetNegativeButton("NO", (ss, ee) => { });

                    b.Create().Show();
                }
                catch { }
            };
        }
        private void PickUp(TextView txt, List<string> ls, bool number)
        {
            try
            {
                Android.App.AlertDialog.Builder b = new AlertDialog.Builder(this);

                LinearLayout ln = new LinearLayout(this) { Orientation = Orientation.Vertical };

                EditText ed = new EditText(this) { Hint = "Value", LayoutParameters = new ViewGroup.LayoutParams(request.Widht(Number(500)), ViewGroup.LayoutParams.WrapContent) };
                if (number) ed.InputType = Android.Text.InputTypes.ClassNumber;
                else ed.InputType = Android.Text.InputTypes.TextFlagCapCharacters;

                ListView lv = new ListView(this);

                ln.AddView(ed); ln.AddView(lv);

                ArrayAdapter array = new ArrayAdapter(this, Resource.Layout.select_dialog_singlechoice_material, ls);
                array.SetDropDownViewResource(Resource.Layout.select_dialog_singlechoice_material);

                lv.Adapter = array;

                b.SetPositiveButton("OK", (s, e) =>
                {
                    txt.Text = ed.Text.ToUpper();
                });

                b.SetView(ln);
                Dialog d = b.Create();
                d.Show();

                ed.TextChanged += delegate { array.Clear(); array.AddAll((ICollection)ls.FindAll(d => d.Contains(ed.Text)).ToArray()); };
                lv.ItemClick += (s, e) => { txt.Text = lv.GetItemAtPosition(e.Position).ToString(); d.Dismiss(); };
            }
            catch { }
        }
        private int Number(int i)
        {
            var metric = Application.Context.Resources.DisplayMetrics;
            return (int)(i * metric.Density);
        }
        private void RefreshKanban()
        {
            try
            {
                string getKB = "exec GetLoadData 42,'" + CSDL.SelectedLine + "',''";
                DataTable dt = kn.Doc(getKB, 120).Tables[0];

                //mainlist.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(dt.DefaultView.ToTable(true, "Id", "DateNeed", "FacLine", "JobNo", "ColorCode", "Size", "PONo", "ReqQty"),
                // new List<int>() { request.eWidht(Number(100)), request.eWidht(Number(200)), request.eWidht(Number(100)), request.eWidht(Number(250)), request.eWidht(Number(150)), request.eWidht(Number(150)), request.eWidht(Number(250)), request.eWidht(Number(100)) }, false, false)
                //{ TextSize = request.TextSize(20) };

                //mainlist.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(dt.DefaultView.ToTable(true, "Id", "DateNeed", "FacLine", "JobNo", "ColorCode", "Size", "PONo", "ReqQty"),
                //                            new List<int>() { 100, 200, 100, 250, 150, 150, 250, 100 }, false, false)
                //{ TextSize = 20 / metric.Density };

                mainlist.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(dt.DefaultView.ToTable(true, "Id", "DateNeed", "FacLine", "JobNo", "ColorCode", "Size", "PONo", "ReqQty"),
                 new List<int>() { request.eWidht((100)), request.eWidht((200)), request.eWidht((100)), request.eWidht((250)), request.eWidht((150)), request.eWidht((150)), request.eWidht((250)), request.eWidht((100)) }, false, false)
                { TextSize = request.eTextSize(20) };

                //Toast.MakeText(this, "..." + CSDL.Language("M00082") + "..." + " @AddKanban", ToastLength.Short).Show();
            }
            catch { Toast.MakeText(this, "..." + CSDL.Language("M00019") + "..." + " @AddKanban", ToastLength.Short).Show(); }
        }

        private void LoadWIP()
        {
            try
            {
                DataSet ds = kn.Doc("exec InlineProcKanban '" + CSDL.SelectedLine + "'", 120);

                //lvsumqty.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(ds.Tables[0], new List<int>() { request.Widht(Number(150)), request.Widht(Number(80)), request.Widht(Number(80)), request.Widht(Number(80)),request.Widht(Number(100)), request.Widht(Number(80)),
                //            request.Widht(Number(80)),request.Widht(Number(80)),request.Widht(Number(100))}, true, false)
                //{ TextSize = request.TextSize(15) };

                //lvwip.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(ds.Tables[1], new List<int>() { request.Widht(Number(200)), request.Widht(Number(100)) }, true, false) { TextSize = request.TextSize(20) };

                //lvsumqty.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(ds.Tables[0], new List<int>() { 150, 80, 80, 80, 120, 80, 80, 80, 100 }, true, false)
                //{ TextSize = 15 / metric.Density };

                //lvwip.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(ds.Tables[1], new List<int>() { 200, 100 }, true, false) { TextSize = 20 / metric.Density };

                lvsumqty.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(ds.Tables[0], new List<int>() { request.eWidht((150)), request.eWidht((80)), request.eWidht((80)), request.eWidht((80)),request.eWidht((120)), request.eWidht((80)),
                            request.eWidht((80)),request.eWidht((80)),request.eWidht((100))}, true, false)
                { TextSize = request.eTextSize(15) };

                lvwip.Adapter = new A1ATeam.ListViewAdapterWithNoLayout(ds.Tables[1], new List<int>() { request.eWidht((200)), request.eWidht((100)) }, true, false) { TextSize = request.eTextSize(20) };
            }
            catch { }
        }
    }
}