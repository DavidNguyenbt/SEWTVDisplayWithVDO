using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SEWTVDisplay
{
    [Activity(Label = "Server Configuration", ScreenOrientation = ScreenOrientation.Landscape)]
    public class ServerActivity : Activity
    {
        string strWrkHrId = "", strShiftId = "";
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.ServerConfig);

            EditText name = FindViewById<EditText>(Resource.Id.edsvname);
            EditText user = FindViewById<EditText>(Resource.Id.edsvuser);
            EditText pass = FindViewById<EditText>(Resource.Id.edsvpas);
            EditText data = FindViewById<EditText>(Resource.Id.edsvdatabase);
            EditText sv = FindViewById<EditText>(Resource.Id.edsv);
            Button add = FindViewById<Button>(Resource.Id.btsvadd);
            Button back = FindViewById<Button>(Resource.Id.btsvback);
            Button ShiftNm = FindViewById<Button>(Resource.Id.btShiftNm);

            List<EditText> edList = new List<EditText>();
            EditText edtf01 = FindViewById<EditText>(Resource.Id.edtf01); edList.Add(edtf01);
            EditText edtf02 = FindViewById<EditText>(Resource.Id.edtf02); edList.Add(edtf02);
            EditText edtf03 = FindViewById<EditText>(Resource.Id.edtf03); edList.Add(edtf03);
            EditText edtf04 = FindViewById<EditText>(Resource.Id.edtf04); edList.Add(edtf04);
            EditText edtf05 = FindViewById<EditText>(Resource.Id.edtf05); edList.Add(edtf05);
            EditText edtf06 = FindViewById<EditText>(Resource.Id.edtf06); edList.Add(edtf06);
            EditText edtf07 = FindViewById<EditText>(Resource.Id.edtf07); edList.Add(edtf07);
            EditText edtf08 = FindViewById<EditText>(Resource.Id.edtf08); edList.Add(edtf08);
            EditText edtf09 = FindViewById<EditText>(Resource.Id.edtf09); edList.Add(edtf09);
            EditText edtf10 = FindViewById<EditText>(Resource.Id.edtf10); edList.Add(edtf10);
            EditText edtf11 = FindViewById<EditText>(Resource.Id.edtf11); edList.Add(edtf11);
            EditText edtf12 = FindViewById<EditText>(Resource.Id.edtf12); edList.Add(edtf12);
            EditText edtf13 = FindViewById<EditText>(Resource.Id.edtf13); edList.Add(edtf13);
            EditText edtf14 = FindViewById<EditText>(Resource.Id.edtf14); edList.Add(edtf14);
            EditText edBrT1 = FindViewById<EditText>(Resource.Id.edBrT1);
            EditText edBrT2 = FindViewById<EditText>(Resource.Id.edBrT2);

            //resizing layout
            var lvVwGroup = (ViewGroup)FindViewById<RelativeLayout>(Resource.Id.SVrelativelayout1);
            CSDL.ScreenStretching(1, lvVwGroup);


            try
            {
                ISharedPreferences pre = GetSharedPreferences("server", FileCreationMode.Private);
                string ch = pre.GetString("timefr", "").ToString();
                //if (ch != "")
                //{
                //    string[] timfr = ch.Split(',');
                //    for (int i = 0; i < 14; i++)
                //    {
                //        edList[i].Text = timfr[i];
                //        CSDL.TimeArray[i] = int.Parse(timfr[i]);
                //    }
                //    CSDL.strTimeTitle = ""; // this is to reset
                //};

                ch = "";
                ch = pre.GetString("chuoi", "").ToString();
                if (ch != "")
                {
                    string[] strAr = ch.Split(';');
                    name.Text = strAr[0].ToString().Split('=')[1];
                    data.Text = strAr[1].ToString().Split('=')[1];
                }

                ShiftNm.Text = pre.GetString("ShiftNm", "").ToString();

                ch = "";
                ch = pre.GetString("timebk", "").ToString();
                if (ch != "")
                {
                    edBrT1.Text = ch.Split(',')[0].ToString();
                    edBrT2.Text = ch.Split(',')[1].ToString();
                    CSDL.tmBrStr = int.Parse(edBrT1.Text);
                    CSDL.tmBrEnd = int.Parse(edBrT2.Text);
                }
            }
            catch { }

            if (CSDL.chuoi != null)
            {
                string[] c = CSDL.chuoi.Split(';');
                sv.Text = c[0].ToString() + " | " + c[1].ToString();
            }

            back.Click += delegate
            {
                var itent = new Intent(this, typeof(MainActivity));
                StartActivity(itent);
                Finish();
            };
            add.Click += delegate
            {
                try
                {
                    string ch = "Data Source=" + name.Text + ";Initial Catalog=" + data.Text + ";Integrated Security=False;User ID=" + user.Text + ";Password=" + pass.Text + ";Connect Timeout=30;Encrypt=False;";
                    SqlConnection con = new SqlConnection(ch);
                    con.Open();
                    Toast.MakeText(this, "Add new Server succeeded !!!", ToastLength.Long).Show();
                    con.Close();

                    string tf = ""; //edtf01.Text+","+ edtf02.Text + "," + edtf03.Text + "," + edtf04.Text + "," + edtf05.Text + "," + edtf06.Text + "," + edtf07.Text + "," + edtf08.Text + "," + edtf09.Text + "," + edtf10.Text + "," + edtf11.Text + "," + edtf12.Text + "," + edtf13.Text + "," + edtf14.Text;
                    for (int i = 0; i < 14; i++)
                    {
                        if (i < 13) tf += edList[i].Text + ",";
                        else tf += edList[i].Text;
                        CSDL.TimeArray[i] = string.IsNullOrEmpty(edList[i].Text) ? 2500 : int.Parse(edList[i].Text);
                    }
                    ISharedPreferences pre = GetSharedPreferences("server", FileCreationMode.Private);
                    ISharedPreferencesEditor editor = pre.Edit();

                    editor.Clear();
                    editor.PutString("chuoi", ch);
                    editor.PutString("timefr", tf);
                    editor.PutString("timebk", edBrT1.Text + "," + edBrT2.Text);
                    editor.PutString("WrkHrId", strWrkHrId);
                    editor.PutString("ShiftId", strShiftId);
                    editor.PutString("ShiftNm", ShiftNm.Text);
                    editor.Apply();

                    CSDL.chuoi = ch;
                }
                catch
                {
                    Toast.MakeText(this, "Connect to Server failed !!!", ToastLength.Long).Show();
                }
            };

            ShiftNm.Click += delegate
            {
                ShiftNm.Text = "";
                DataTable dt = CSDL.Cnnt.Doc("select distinct ShiftNm, WrkHrId, ShiftId from InlineWrkHrMaster").Tables[0];
                string[] arstr = new string[dt.Rows.Count];
                for (int i = 0; i < arstr.Length; i++) arstr[i] = dt.Rows[i]["ShiftNm"].ToString();

                AlertDialog al;
                AlertDialog.Builder bd1 = new AlertDialog.Builder(this); // this click not close alert dialog => this is a builder not a dialog
                bd1.SetTitle("Shift Name"); //"Shift Name"
                bd1.SetItems(arstr, (sender, e) =>
                {
                    string str = "select top 1 [WrkHr01],[WrkHr02],[WrkHr03],[WrkHr04],[WrkHr05],[WrkHr06],[WrkHr07],[WrkHr08],[WrkHr09],[WrkHr10],[WrkHr11],[WrkHr12],[WrkHr13],[WrkHr14],[StrBr01],[FnBr01] " +
                                 " from InlineWrkHrMaster where ShiftNm = '" + arstr[e.Which].ToString() + "'";

                    DataTable d = CSDL.Cnnt.Doc(str).Tables[0];

                    if (d.Rows.Count > 0)
                    {
                        for (int i = 0; i < edList.Count; i++) edList[i].Text = d.Rows[0][i].ToString();
                        edBrT1.Text = d.Rows[0]["StrBr01"].ToString(); //break time start
                        edBrT2.Text = d.Rows[0]["FnBr01"].ToString(); //break time finish

                        strWrkHrId = dt.Rows[e.Which]["WrkHrId"].ToString();
                        strShiftId = dt.Rows[e.Which]["ShiftId"].ToString();
                        ShiftNm.Text = arstr[e.Which].ToString();
                    }
                    else Toast.MakeText(this, "Retrieve time-data failed !!!", ToastLength.Long).Show();
                });

                bd1.SetPositiveButton(CSDL.Language("M00032"), (sender, e) => //chọn
                {
                    bd1.Dispose();
                });

                al = bd1.Create();
                al.Show();
            };
        }
    }
}