using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SEWTVDisplay
{
    [Activity(Label = "KanbanUpdate", ScreenOrientation = ScreenOrientation.Landscape)]
    public class KanbanUpdate : Activity
    {
        Button btSelectDate, btToDownload, btKanbanUpload, btKanbanDelete;
        TextView tvSelectedDate, tvKanbanNo;
        EditText edSumQty, edJobNo, edColor, edPONo, edReqQty, edWIPSPMK, tvKanbanView, edSize;
        Spinner spJobNo, spColor, spPONo, spSeqNo, spSize;
        List<String> KBJobList = new List<String>();
        List<String> listSumJob = new List<String>();

        int SeqNo = 0, SeqId = 0;
        string KBRDate = "";

        DataTable dtKBJobList = new DataTable();
        DataTable dtKBColorList = new DataTable();
        DataTable dtKBPONoList = new DataTable();
        DataTable dtKBView = new DataTable();

        CSDL kn = new CSDL();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.KanbanWrite);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);

            btSelectDate = FindViewById<Button>(Resource.Id.btToSelectDate);
            btToDownload = FindViewById<Button>(Resource.Id.btToDownload);
            btKanbanUpload = FindViewById<Button>(Resource.Id.btKanbanUpload);
            btKanbanDelete = FindViewById<Button>(Resource.Id.btKanbanDelete);

            spJobNo = FindViewById<Spinner>(Resource.Id.spJobNo);
            spColor = FindViewById<Spinner>(Resource.Id.spColor);
            spPONo = FindViewById<Spinner>(Resource.Id.spPONo);
            spSeqNo = FindViewById<Spinner>(Resource.Id.spinner1);
            spSize = FindViewById<Spinner>(Resource.Id.spsize);

            tvSelectedDate = FindViewById<TextView>(Resource.Id.tvKanbanDate);
            tvKanbanNo = FindViewById<TextView>(Resource.Id.tvKanbanNo);

            edSumQty = FindViewById<EditText>(Resource.Id.edSumQty);
            edJobNo = FindViewById<EditText>(Resource.Id.edJobNo);
            edColor = FindViewById<EditText>(Resource.Id.edColor);
            edPONo = FindViewById<EditText>(Resource.Id.edPONo);
            edReqQty = FindViewById<EditText>(Resource.Id.edReqQty);
            edWIPSPMK = FindViewById<EditText>(Resource.Id.edWIPSPMK);
            edSize = FindViewById<EditText>(Resource.Id.edsize);

            tvKanbanView = FindViewById<EditText>(Resource.Id.edKanbanView);


            //resizing layout
            var lvVwGroup = (ViewGroup)FindViewById<RelativeLayout>(Resource.Id.kbrelativelayout);
            CSDL.ScreenStretching(1, lvVwGroup);


            tvSelectedDate.Text = CSDL.tg.AddDays(1).ToString("dd-MM-yyyy");
            KBRDate = CSDL.tg.AddDays(1).ToString("yyyyMMdd");

            SpJobNoInitialize();

            TvSumQtyJobnPO();

            RefreshKanban();

            GetWIPatSPMK();

            //tvKanbanView.LongClick += delegate { RefreshKanban(); };
            btSelectDate.Click += delegate { DateSelect_OnClick(); };
            spJobNo.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Sp_JobNoSelection);
            spColor.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Sp_ColorSelection);
            spSize.ItemSelected += SpSize_ItemSelected;
            spPONo.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Sp_PONoSelection);
            spSeqNo.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Sp_DelKanban);
            edReqQty.LongClick += delegate { edReqQty.Text = "-vétPO-"; };

            btKanbanDelete.Click += delegate
            {
                if (spSeqNo.SelectedItemId != 0 && SeqId != 0)
                {
                    string DelId = dtKBView.Rows[SeqId - 1][0].ToString();
                    string DelKB = "delete from InlineQcKanban where Id = " + DelId.ToString();
                    CSDL.Cnnt.Ghi(DelKB);
                    RefreshKanban();
                }
                else
                {
                    Toast.MakeText(this, "..." + CSDL.Language("M00152") + "...", ToastLength.Short).Show();
                }
            };

            btKanbanUpload.Click += delegate
            {
                try
                {
                    if (KBRDate.Trim().Length != 0 && CSDL.SelectedLine != "FxAxx" && edJobNo.Text.Trim().Length > 0 && edColor.Text.Trim().Length > 0 && edPONo.Text.Trim().Length > 0 && edReqQty.Text.Trim().Length > 0 && int.Parse(KBRDate) >= int.Parse(DateTime.Today.ToString("yyyyMMdd")))
                    {
                        string ch = "insert into InlineQcKanban (DateNeed,FacLine,JobNo,ColorCode,Size,PONo,ReqQty,RecordDate,KBStatus ) values ('"
                                    + KBRDate + "','" + CSDL.SelectedLine + "','" + edJobNo.Text + "','" + edColor.Text + "','" + edSize.Text + "','" + edPONo.Text + "','" + edReqQty.Text + "',getdate(),'Wait') ";
                        CSDL.Cnnt.Ghi(ch);

                        RefreshKanban();
                    }
                    else
                    {
                        Toast.MakeText(this, "..." + CSDL.Language("M00153") + "...", ToastLength.Short).Show();
                    }
                }
                catch { Toast.MakeText(this, CSDL.Language("M00019") + " @Add Kanban fail", ToastLength.Short).Show(); }
            };
            btToDownload.Click += delegate
             {
                 if (edJobNo.Text.Trim().Length == 13)
                 {
                     if (!KBJobList.Contains(edJobNo.Text))
                     {
                         KBJobList.Add(edJobNo.Text);
                         ArrayAdapter adaterL = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, KBJobList);
                         adaterL.SetDropDownViewResource(Android.Resource.Layout.SimpleListItemSingleChoice);
                         spJobNo.Adapter = adaterL;
                         try
                         {
                             SqlConnection con = new SqlConnection(CSDL.chuoi);
                             con.Open();
                             SqlCommand cmd = new SqlCommand("InlineQcKanbanSPAddJob", con) { CommandType = CommandType.StoredProcedure };
                             cmd.Parameters.AddWithValue("@AddedJob", edJobNo.Text.Trim());
                             cmd.ExecuteNonQuery();
                             SqlDataAdapter MReader = new SqlDataAdapter { SelectCommand = cmd };
                             DataSet dsKanBanSPAddJob = new DataSet();
                             MReader.Fill(dsKanBanSPAddJob);
                             con.Close();

                             for (int i = 0; i < dsKanBanSPAddJob.Tables.Count; i++)
                             {
                                 DataTable AddJob = new DataTable();
                                 AddJob = dsKanBanSPAddJob.Tables[i];
                                 if (AddJob.Rows.Count > 0)
                                 {
                                     if (i == 0)
                                     {
                                         foreach (DataRow myrow in AddJob.Rows)
                                         {
                                             DataRow mynewrow = dtKBColorList.NewRow();
                                             mynewrow[0] = myrow[0];
                                             mynewrow[1] = myrow[1];
                                             mynewrow[2] = myrow[2];
                                             dtKBColorList.Rows.Add(mynewrow);
                                             dtKBColorList.AcceptChanges();
                                         }
                                     }
                                     else
                                     {
                                         foreach (DataRow myrow in AddJob.Rows)
                                         {
                                             DataRow mynewrow1 = dtKBPONoList.NewRow();
                                             mynewrow1[0] = myrow[0];
                                             mynewrow1[1] = myrow[1];
                                             dtKBPONoList.Rows.Add(mynewrow1);
                                             dtKBPONoList.AcceptChanges();
                                         }
                                     }
                                 }
                                 else { Toast.MakeText(this, "..." + CSDL.Language("M00043") + "..." + " @JobNo..", ToastLength.Short).Show(); }
                             }
                             spJobNo.SetSelection(spJobNo.Count - 1);
                             Toast.MakeText(this, "..." + CSDL.Language("M00082") + "...", ToastLength.Short).Show();
                         }
                         catch { Toast.MakeText(this, "Catch: SQLAddedJob Fail", ToastLength.Short).Show(); }
                     }
                     else { Toast.MakeText(this, "..." + CSDL.Language("M00070") + "..." + "@Job/No", ToastLength.Short).Show(); }
                 }
                 else { Toast.MakeText(this, "..." + CSDL.Language("M00153") + "..." + "@Job/No", ToastLength.Short).Show(); }
             };
        }

        private void SpSize_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            edSize.Text = spSize.GetItemAtPosition(e.Position).ToString();
        }

        private void Sp_DelKanban(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            SeqId = (int)spinner.SelectedItemId;
            tvKanbanNo.Text = SeqId.ToString();
        }

        private void RefreshKanban()
        {
            try
            {
                dtKBView.Clear();
                SeqNo = 0;
                string getKB = "select * from InlineQcKanban where FacLine='" + CSDL.SelectedLine + "' and DateNeed >= dateadd(day,-7, getdate()) and KBStatus = 'Wait' order by DateNeed desc ";
                dtKBView = CSDL.Cnnt.Doc(getKB).Tables[0];
                tvKanbanView.Text = CSDL.Language("M00163");
                foreach (DataRow myKB in dtKBView.Rows)
                {
                    SeqNo++; //-----------------stt-----------------------------date--------------------------------------------line---------------------------------job-------------------------------color----------------------------po--------------------------------qty
                    tvKanbanView.Text += "\n" + SeqNo.ToString() + ".      " + myKB[1].ToString().Split(' ')[0] + "       " + myKB[2].ToString() + "         " + myKB[3].ToString() + "        " + myKB[4].ToString() + "       " + myKB[5].ToString() + "         " + myKB[6].ToString() + "      " + myKB[7].ToString();
                }
                List<String> SeqNoList = new List<String>();
                for (int i = 0; i <= dtKBView.Rows.Count; i++) { SeqNoList.Add(i.ToString()); }
                ArrayAdapter adaterL = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, SeqNoList);
                adaterL.SetDropDownViewResource(Android.Resource.Layout.SimpleListItemSingleChoice);
                spSeqNo.Adapter = adaterL;

                Toast.MakeText(this, "..." + CSDL.Language("M00082") + "..." + " @AddKanban", ToastLength.Short).Show();
            }
            catch { Toast.MakeText(this, "..." + CSDL.Language("M00019") + "..." + " @AddKanban", ToastLength.Short).Show(); }
        }
        private void GetWIPatSPMK()
        {
            try
            {
                //------show available wip at supmarket
                string getLineWIP = "exec GetWIPatSPMK '" + CSDL.SelectedLine + "'";
                DataTable dtWIPSPMK = new DataTable();
                dtWIPSPMK = CSDL.Cnnt.Doc(getLineWIP).Tables[0];
                if (dtWIPSPMK.Rows.Count > 0)
                {
                    edWIPSPMK.Text = CSDL.Language("M00154") + " :";
                    for (int i = 1; i < 6; i++)
                    {
                        edWIPSPMK.Text += "\n" + dtWIPSPMK.Rows[i][0].ToString() + " : " + dtWIPSPMK.Rows[i][1].ToString();
                    }
                }
            }
            catch { Toast.MakeText(this, CSDL.Language("M00019") + "..." + " @GetWIPatSPMK", ToastLength.Short).Show(); }
        }

        private void Sp_PONoSelection(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            edPONo.Text = spinner.GetItemAtPosition(e.Position).ToString();
        }

        private void Sp_ColorSelection(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;
            edColor.Text = spinner.GetItemAtPosition(e.Position).ToString();
        }

        private void Sp_JobNoSelection(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            try
            {
                Spinner spinner = (Spinner)sender;
                edJobNo.Text = spinner.GetItemAtPosition(e.Position).ToString();
                int mySpPID = (int)spinner.SelectedItemId;
                if (mySpPID > 0)
                {
                    if (dtKBColorList.Rows.Count > 0)
                    {
                        List<DataRow> mylist = dtKBColorList.Select(" JobNo='" + edJobNo.Text + "' ").ToList();
                        List<string> ColorName = new List<string>();
                        List<string> SizeName = new List<string>();
                        ColorName.Add(CSDL.Language("M00158"));
                        SizeName.Add(CSDL.Language("M00159"));
                        SizeName.Add(CSDL.Language("M00160"));
                        for (int j = 0; j < mylist.Count; j++)
                        {
                            ColorName.Add(mylist[j][1].ToString());
                            SizeName.Add(mylist[j][2].ToString());
                        }

                        ArrayAdapter adaterL = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, ColorName);
                        adaterL.SetDropDownViewResource(Android.Resource.Layout.SimpleListItemSingleChoice);
                        spColor.Adapter = adaterL;

                        ArrayAdapter adaterS = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, SizeName);
                        adaterS.SetDropDownViewResource(Android.Resource.Layout.SimpleListItemSingleChoice);
                        spSize.Adapter = adaterS;
                    }
                    else { Toast.MakeText(this, CSDL.Language("M00155"), ToastLength.Short).Show(); }

                    if (dtKBPONoList.Rows.Count > 0)
                    {
                        List<DataRow> mylist = dtKBPONoList.Select(" OrderNo='" + edJobNo.Text + "' ").ToList();
                        List<string> PONoName = new List<string>();
                        PONoName.Add(CSDL.Language("M00157"));
                        for (int j = 0; j < mylist.Count; j++)
                        {
                            PONoName.Add(mylist[j][1].ToString());
                        }
                        ArrayAdapter adaterL = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, PONoName);
                        adaterL.SetDropDownViewResource(Android.Resource.Layout.SimpleListItemSingleChoice);
                        spPONo.Adapter = adaterL;
                    }
                    else { Toast.MakeText(this, CSDL.Language("M00156"), ToastLength.Short).Show(); }
                }
            }
            catch { Toast.MakeText(this, "Catch: JobSelection Fail", ToastLength.Short).Show(); }
        }

        private void TvSumQtyJobnPO()
        {
            try
            {
                SqlConnection con = new SqlConnection(CSDL.chuoi);
                con.Open();
                SqlCommand cmd = new SqlCommand("InlineQcProcKanban", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@MyLine", CSDL.SelectedLine.ToString());
                cmd.Parameters.AddWithValue("@EndDate", CSDL.EndDate.ToString());
                cmd.ExecuteNonQuery();
                SqlDataAdapter MReader = new SqlDataAdapter { SelectCommand = cmd };
                DataTable dtSumJobList = new DataTable();
                MReader.Fill(dtSumJobList);
                con.Close();
                if (dtSumJobList.Rows.Count > 0)
                {
                    foreach (DataRow mySJL in dtSumJobList.Rows)
                    {
                        if (!listSumJob.Contains(mySJL[0].ToString())) listSumJob.Add(mySJL[0].ToString());
                    }
                    edSumQty.Text = CSDL.Language("M00162") + "\n" + "\n";
                    for (int i = 0; i < listSumJob.Count; i++)
                    {
                        int mysum = 0;
                        //int sumorder = 0;
                        List<DataRow> mylist = dtSumJobList.Select(" JobNo='" + listSumJob[i].ToString() + "' ").ToList();

                        List<string> QtyPO = new List<string>();
                        for (int j = 0; j < mylist.Count; j++)
                        {
                            mysum += int.Parse(mylist[j][2].ToString());
                            int LackOutput = (string.IsNullOrEmpty(mylist[j][2].ToString()) ? 0 : int.Parse(mylist[j][2].ToString())) - (string.IsNullOrEmpty(mylist[j][4].ToString()) ? 0 : int.Parse(mylist[j][4].ToString()));
                            QtyPO.Add(mylist[j][1].ToString() + " : " + mylist[j][2].ToString() + " / " + mylist[j][4].ToString() + " (" + LackOutput.ToString() + ") | Ship : " + DateTime.Parse(mylist[j][3].ToString()).ToString("dd/MM/yy") + "\n"
                                + "                 "+ CSDL.Language("M00161") + " : " + mylist[j][5].ToString() + " | WIP : " + mylist[j][6].ToString());

                        }
                        edSumQty.Text += mylist[0][0].ToString() + " : " + mysum.ToString() + "\n";
                        for (int k = 0; k < QtyPO.Count; k++)
                        {
                            edSumQty.Text += "       " + QtyPO[k].ToString() + "\n";
                        }
                    }
                }
                else { Toast.MakeText(this, CSDL.Language("M00043"), ToastLength.Short).Show(); }
            }
            catch (Exception ex) { Toast.MakeText(this, "Catch: SQLtvSumQtyJobnPO ERROR: " + ex.Message.ToString(), ToastLength.Long).Show(); }
        }

        private void SpJobNoInitialize()
        {
            try
            {
                SqlConnection con = new SqlConnection(CSDL.chuoi);
                con.Open();
                SqlCommand cmd = new SqlCommand("InlineQcKanbanSP", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@EndDate", CSDL.EndDate.ToString());
                cmd.Parameters.AddWithValue("@MyLine", CSDL.SelectedLine.ToString());
                cmd.ExecuteNonQuery();
                SqlDataAdapter MReader = new SqlDataAdapter { SelectCommand = cmd };
                DataSet dsKanBanSP = new DataSet();
                MReader.Fill(dsKanBanSP);
                dtKBJobList.Clear();
                dtKBColorList.Clear();
                dtKBPONoList.Clear();
                dtKBJobList = dsKanBanSP.Tables[0];
                dtKBColorList = dsKanBanSP.Tables[1];//color and size table
                dtKBPONoList = dsKanBanSP.Tables[2];
                con.Close();
            }
            catch (Exception ex) { Toast.MakeText(this, "SQLKBSpinner ERROR: " + ex.Message.ToString(), ToastLength.Long).Show(); }

            if (KBJobList.Count > 0) KBJobList.Clear();
            KBJobList.Add(CSDL.Language("M00032") + " Job");
            if (dtKBJobList.Rows.Count > 0)
            {
                foreach (DataRow myr in dtKBJobList.Rows)
                {
                    KBJobList.Add(myr[0].ToString());
                }
                ArrayAdapter adaterL = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, KBJobList);
                adaterL.SetDropDownViewResource(Android.Resource.Layout.SimpleListItemSingleChoice);
                spJobNo.Adapter = adaterL;
                spJobNo.SetSelection(1);
            }
            else
            {
                Toast.MakeText(this, CSDL.Language("M00043") + " @JobRecord", ToastLength.Long).Show();
            }
        }

        private void DateSelect_OnClick()
        {
            DatePickerFragment frag = DatePickerFragment.NewInstance(delegate (DateTime time)
            {
                tvSelectedDate.Text = time.ToString("dd-MM-yyyy").ToUpper();
                KBRDate = time.ToString("yyyyMMdd");
                SpJobNoInitialize();
            });
            frag.Show(FragmentManager, DatePickerFragment.TAG);
        }
    }
}