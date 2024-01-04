using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;
using Java.Net;

namespace SEWTVDisplay
{
    [Activity(Label = "ImageRetrive", ScreenOrientation = ScreenOrientation.Landscape)]
    public class ImageRetrive : Activity
    {

        #region -----------------------New-------------------------------------------------
        List<string> DefectList = new List<string>();
        List<ImageView> ListImage = new List<ImageView>();
        byte[] ImageByteArray = new byte[] { };
        Spinner spMyFilter;
        DataTable QrImageData = new DataTable();
        DataTable Filter = new DataTable();
        TextView tvImageList, tvPages, tvImageDetail, tvdetail;
        ImageView ZoomImage, imImage1, imImage2, imImage3, imImage4, imImage5, imImage6, imImage7, imImage8;
        Button NextPage, PrevPage;
        int CurrPage = 1, MaxPage = 1; Timer tg = new Timer();
        byte[] img = null;
        WebClient web = new WebClient();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Image);

            tg.Interval = 1000;
            tg.Enabled = true;
            tg.Elapsed += Tg_Elapsed;
            tg.Start();

            spMyFilter = FindViewById<Spinner>(Resource.Id.spMyfilter);
            tvImageList = FindViewById<TextView>(Resource.Id.tvImageList);
            tvPages = FindViewById<TextView>(Resource.Id.tvPageNo);
            tvImageDetail = FindViewById<TextView>(Resource.Id.tvImageDetail);
            tvdetail = FindViewById<TextView>(Resource.Id.tvdetail);
            ZoomImage = FindViewById<ImageView>(Resource.Id.imShowImage);
            imImage1 = FindViewById<ImageView>(Resource.Id.imImage1);
            imImage2 = FindViewById<ImageView>(Resource.Id.imImage2);
            imImage3 = FindViewById<ImageView>(Resource.Id.imImage3);
            imImage4 = FindViewById<ImageView>(Resource.Id.imImage4);
            imImage5 = FindViewById<ImageView>(Resource.Id.imImage5);
            imImage6 = FindViewById<ImageView>(Resource.Id.imImage6);
            imImage7 = FindViewById<ImageView>(Resource.Id.imImage7);
            imImage8 = FindViewById<ImageView>(Resource.Id.imImage8);
            PrevPage = FindViewById<Button>(Resource.Id.btPrevPage);
            NextPage = FindViewById<Button>(Resource.Id.btNextPage);

            ListImage.Add(imImage1); ListImage.Add(imImage2); ListImage.Add(imImage3); ListImage.Add(imImage4);
            ListImage.Add(imImage5); ListImage.Add(imImage6); ListImage.Add(imImage7); ListImage.Add(imImage8);

            //resizing layout
            var lvVwGroup = (ViewGroup)FindViewById<RelativeLayout>(Resource.Id.relativeLayout1);
            CSDL.ScreenStretching(1, lvVwGroup);


            tvImageList.Text = CSDL.ImageListInfo;
            ImageInitialize();
            SpinnerInitialize();

            Toast.MakeText(this, Filter.Rows.Count.ToString(), ToastLength.Long).Show();
            tvPages.Text = CurrPage.ToString() + " | " + MaxPage.ToString() + " (" + Filter.Rows.Count.ToString() + ")";

            ActivityManager actManager = (ActivityManager)GetSystemService(ActivityService);
            ActivityManager.MemoryInfo memInfo = new ActivityManager.MemoryInfo();
            actManager.GetMemoryInfo(memInfo);
            int ram = (int)Math.Ceiling((decimal)memInfo.TotalMem / 1073741824);
            decimal avai = Math.Round((decimal)memInfo.AvailMem / 1073741824, 2);
            tvdetail.Text = Build.Model + " \n Ram : " + (ram - avai).ToString() + "/" + ram.ToString() + " GB";

            #region ImageClick
            imImage1.Click += delegate { ZoomImage.SetImageDrawable(imImage1.Drawable); ImageLabel((CurrPage - 1) * 8 + 0); };
            imImage2.Click += delegate { ZoomImage.SetImageDrawable(imImage2.Drawable); ImageLabel((CurrPage - 1) * 8 + 1); };
            imImage3.Click += delegate { ZoomImage.SetImageDrawable(imImage3.Drawable); ImageLabel((CurrPage - 1) * 8 + 2); };
            imImage4.Click += delegate { ZoomImage.SetImageDrawable(imImage4.Drawable); ImageLabel((CurrPage - 1) * 8 + 3); };
            imImage5.Click += delegate { ZoomImage.SetImageDrawable(imImage5.Drawable); ImageLabel((CurrPage - 1) * 8 + 4); };
            imImage6.Click += delegate { ZoomImage.SetImageDrawable(imImage6.Drawable); ImageLabel((CurrPage - 1) * 8 + 5); };
            imImage7.Click += delegate { ZoomImage.SetImageDrawable(imImage7.Drawable); ImageLabel((CurrPage - 1) * 8 + 6); };
            imImage8.Click += delegate { ZoomImage.SetImageDrawable(imImage8.Drawable); ImageLabel((CurrPage - 1) * 8 + 7); };
            #endregion

            PrevPage.Text = CSDL.Language("M00149");
            NextPage.Text = CSDL.Language("M00150");

            spMyFilter.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(Spinner_selected);
            NextPage.Click += delegate
            {
                if (CurrPage < MaxPage)
                {
                    CurrPage = CurrPage + 1;
                    LoadingImage();
                    tvPages.Text = CurrPage.ToString() + " | " + MaxPage.ToString() + " (" + Filter.Rows.Count.ToString() + ")";
                }
                else Toast.MakeText(this, "...." + CSDL.Language("M00147"), ToastLength.Short).Show();

            };

            PrevPage.Click += delegate
            {
                if (CurrPage > 1)
                {
                    CurrPage = CurrPage - 1;
                    LoadingImage();
                    tvPages.Text = CurrPage.ToString() + " | " + MaxPage.ToString() + " (" + Filter.Rows.Count.ToString() + ")";
                }
                else Toast.MakeText(this, CSDL.Language("M00148") + "....", ToastLength.Short).Show();
            };
            tvdetail.Click += delegate
            {
                LoadImg();
            };

            web.DownloadDataCompleted += Web_DownloadDataCompleted;
        }

        private void Web_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            img = e.Result;
        }

        int tick = 0;
        private void Tg_Elapsed(object sender, ElapsedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                if (tick < 6) tick++;

                ActivityManager actManager = (ActivityManager)GetSystemService(ActivityService);
                ActivityManager.MemoryInfo memInfo = new ActivityManager.MemoryInfo();
                actManager.GetMemoryInfo(memInfo);
                int ram = (int)Math.Ceiling((decimal)memInfo.TotalMem / 1073741824);
                decimal avai = Math.Round((decimal)memInfo.AvailMem / 1073741824, 2);
                tvdetail.Text = Build.Model + " \n Ram (" + DateTime.Now.ToString("ss") + ") : " + (ram - avai).ToString() + "/" + ram.ToString() + " GB";

                if (Check() && tick < 6) LoadImg();
            });
        }

        private void SpinnerInitialize()
        {
            try
            {
                if (CSDL.in_or_end == 1 && CSDL.Top3InlineDF.Rows.Count > 0)
                {
                    if (DefectList.Count > 0) DefectList.Clear();
                    DefectList.Add(CSDL.Language("M00120"));
                    foreach (DataRow myr in CSDL.Top3InlineDF.Rows)
                    {
                        DefectList.Add(myr[0].ToString() + " .  " + myr[2].ToString() + " | " + myr[3].ToString());
                    }
                    ArrayAdapter adaterL = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, DefectList);
                    adaterL.SetDropDownViewResource(Android.Resource.Layout.SimpleListItemSingleChoice);
                    spMyFilter.Adapter = adaterL;
                }

                if (CSDL.in_or_end == 2 && CSDL.Top3EndlineDF.Rows.Count > 0)
                {
                    if (DefectList.Count > 0) DefectList.Clear();
                    DefectList.Add(CSDL.Language("M00120"));
                    foreach (DataRow myr in CSDL.Top3EndlineDF.Rows)
                    {
                        DefectList.Add(myr[0].ToString() + " .  " + myr[2].ToString() + " | " + myr[3].ToString());
                    }
                    ArrayAdapter adaterL = new ArrayAdapter(this, Android.Resource.Layout.SimpleSpinnerItem, DefectList);
                    adaterL.SetDropDownViewResource(Android.Resource.Layout.SimpleListItemSingleChoice);
                    spMyFilter.Adapter = adaterL;
                }
            }
            catch
            {
                Toast.MakeText(this, "Spinner initialize fail....", ToastLength.Short).Show();
            }
        }
        private void ImageInitialize()
        {
            try
            {
                SqlConnection con = new SqlConnection(CSDL.chuoi);
                con.Open();
                SqlCommand cmd = new SqlCommand("InlineQcRetriveImage", con) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@Or", CSDL.in_or_end);
                cmd.Parameters.AddWithValue("@BeginDate", CSDL.BeginDate.ToString());
                cmd.Parameters.AddWithValue("@EndDate", CSDL.EndDate.ToString());
                cmd.Parameters.AddWithValue("@MyLine", CSDL.SelectedLine.ToString());
                cmd.ExecuteNonQuery();
                SqlDataAdapter MReader = new SqlDataAdapter { SelectCommand = cmd };
                MReader.Fill(QrImageData);
                con.Close();
            }
            catch (Exception ex) { Toast.MakeText(this, "SQLImageLoading ERROR: " + ex.Message.ToString(), ToastLength.Long).Show(); }

            if (QrImageData.Rows.Count > 0)
            {
                MaxPage = (int)Math.Ceiling((decimal)QrImageData.Rows.Count / 8);//identify max page of images
            }
            else
            {
                Toast.MakeText(this, CSDL.Language("M00043"), ToastLength.Long).Show();
            }
        }
        int FilterPos = 0; string FilterValue = "";
        private void Spinner_selected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            FilterPos = e.Position;
            SpinnerClick();
        }
        private void SpinnerClick()
        {
            try
            {
                Filter.Clear();
                FilterValue = "";
                CurrPage = 1;
                if (FilterPos == 0)
                {
                    DataView v = QrImageData.DefaultView;
                    v.RowFilter = "";
                    Filter = v.ToTable();
                    //Toast.MakeText(this, "Select ALL DEFECT...." + "Table " + QrImageData.Rows.Count.ToString(), ToastLength.Short).Show();
                    MaxPage = (int)Math.Ceiling((decimal)Filter.Rows.Count / 8);
                    LoadingImage();
                }
                else
                {
                    if (CSDL.in_or_end == 1) FilterValue = CSDL.Top3InlineDF.Rows[FilterPos - 1][0].ToString();
                    else FilterValue = CSDL.Top3EndlineDF.Rows[FilterPos - 1][0].ToString();

                    if (Filter.Rows.Count > 0) Filter.Clear();

                    DataView v = QrImageData.DefaultView;
                    v.RowFilter = "CODEDEFECT = '" + FilterValue + "'";
                    //Toast.MakeText(this, "Table " + QrImageData.Rows.Count.ToString() + "Top 3 " + CSDL.Top3InlineDF.Rows.Count.ToString() + "Value " + FilterValue + "View " + v.Table.Rows.Count.ToString(), ToastLength.Short).Show();
                    Filter = v.ToTable();
                    MaxPage = (int)Math.Ceiling((decimal)Filter.Rows.Count / 8);
                    LoadingImage();
                }
                tvPages.Text = CurrPage.ToString() + " | " + MaxPage.ToString() + " (" + Filter.Rows.Count.ToString() + ")";
            }
            catch
            {
                Toast.MakeText(this, CSDL.Language("M00019") + "@Spinner ", ToastLength.Short).Show();
            }
        }
        private void LoadImg()
        {
            //Toast.MakeText(this, QrImageData.Rows[0]["TitleOfImage"].ToString(),ToastLength.Long).Show();
            int im = 0;
            foreach (DataRow r in QrImageData.Rows)
            {
                string url = CSDL.url + (r["TitleOfImage"].ToString().Contains(".jpg") ? r["TitleOfImage"].ToString() : r["TitleOfImage"].ToString() + ".jpg");
                //web.DownloadDataAsync(new Uri(url));
                try
                {
                    img = web.DownloadData(url);
                }
                catch
                {
                    img = null;
                }

                if (img != null && img.Length > 0)
                {
                    r["MyImage"] = img; im++; img = null;
                }
            }
            Toast.MakeText(this, "Found: " + im.ToString() + "/" + QrImageData.Rows.Count.ToString(), ToastLength.Long).Show();

            SpinnerClick();
        }
        private bool Check()
        {
            bool fn = false;
            foreach (DataRow r in QrImageData.Rows)
            {
                if (string.IsNullOrEmpty(r["MyImage"].ToString())) fn = true;
            }
            return fn;
        }
        private void LoadingImage()
        {
            ResetImage();
            int d = Filter.Rows.Count > CurrPage * 8 ? CurrPage * 8 : Filter.Rows.Count;
            try
            {
                int l = 0;
                for (int j = (CurrPage - 1) * 8; j < d; j++)
                {
                    if (!string.IsNullOrEmpty(Filter.Rows[j][2].ToString()))
                    {
                        ImageByteArray = (byte[])Filter.Rows[j][2];
                        var bitmap = BitmapFactory.DecodeByteArray(ImageByteArray, 0, ImageByteArray.Length);
                        ListImage[l].SetImageBitmap(bitmap);

                        //ListImage[l].SetImageBitmap(BitmapFactory.DecodeStream(new URL(CSDL.url + (Filter.Rows[j]["TitleOfImage"].ToString().Contains(".jpg") ? Filter.Rows[j]["TitleOfImage"].ToString() : Filter.Rows[j]["TitleOfImage"].ToString() + ".jpg")).OpenConnection().InputStream));
                        l++;
                    }
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, CSDL.Language("M00019") + "@loading Image" + ex.ToString(), ToastLength.Short).Show();
            }
        }
        private void ResetImage()
        {
            tvImageDetail.Text = "";
            imImage1.SetImageBitmap(null);
            imImage2.SetImageBitmap(null);
            imImage3.SetImageBitmap(null);
            imImage4.SetImageBitmap(null);
            imImage5.SetImageBitmap(null);
            imImage6.SetImageBitmap(null);
            imImage7.SetImageBitmap(null);
            imImage8.SetImageBitmap(null);
            ZoomImage.SetImageBitmap(null);

        }
        private void ImageLabel(int row)
        {
            try
            {
                string emp = Filter.Rows[row][1].ToString().Replace(".jpg", "");

                if (CSDL.in_or_end == 1)
                {
                    tvImageDetail.Text = "Employee : " + emp.Substring(emp.Length - 10, 10) + "\n" +
                                     "Operation : " + Filter.Rows[row][3].ToString() + "\n" +
                                     "Defect : " + Filter.Rows[row][4].ToString() + " | " + Filter.Rows[row][6].ToString() + " | " + Filter.Rows[row][7].ToString();
                }
                else
                {
                    tvImageDetail.Text = "QC : " + emp.Substring(emp.Length - 10, 10) + "\n" +
                                     "Defect By : " + Filter.Rows[row][3].ToString() + "\n" +
                                     "Description : " + Filter.Rows[row][4].ToString() + " | " + Filter.Rows[row][6].ToString() + " | " + Filter.Rows[row][7].ToString();
                }
            }
            catch
            {
                tvImageDetail.Text = "";
            }
        }
        #endregion
    }
}