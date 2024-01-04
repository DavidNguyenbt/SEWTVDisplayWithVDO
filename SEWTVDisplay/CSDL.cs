using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using CSDL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Timers;

namespace SEWTVDisplay
{
    public class CSDL
    {
        public static string chuoi = "Data Source=192.168.1.245;Initial Catalog=DtradeProduction;Integrated Security=False;User ID=prog4;Password=DeS;Connect Timeout=30;Encrypt=False;";
        public static string chuoiDT = "Data Source=192.168.1.245;Initial Catalog=Maintenance;Integrated Security=False;User ID=prog4;Password=DeS;Connect Timeout=30;Encrypt=False;";
        //public static string chuoi = "Data Source=192.168.50.253;Initial Catalog=DtradeProduction;Integrated Security=False;User ID=sa;Password=Sql4116!;Connect Timeout=30;Encrypt=False;";
        //public static string chuoiDT = "Data Source=192.168.50.253;Initial Catalog=Maintenance;Integrated Security=False;User ID=sa;Password=Sql4116!;Connect Timeout=30;Encrypt=False;";
        //public static string chuoi = "Data Source=192.168.54.8;Initial Catalog=DtradeProduction;Integrated Security=False;User ID=sa;Password=Admin@168*;Connect Timeout=30;Encrypt=False;";
        //public static string chuoiDT = "Data Source=192.168.54.8;Initial Catalog=Maintenance;Integrated Security=False;User ID=sa;Password=Admin@168*;Connect Timeout=30;Encrypt=False;";

        //public static string chuoi = "Data Source=192.168.50.253;Initial Catalog=DtradeProduction;Integrated Security=False;User ID=sa;Password=Sql4116!;Connect Timeout=30;Encrypt=False;";
        //public static string chuoiDT = "Data Source=192.168.50.253;Initial Catalog=Maintenance;Integrated Security=False;User ID=sa;Password=Sql4116!;Connect Timeout=30;Encrypt=False;";

        public static string SelectedLine = "", BeginDate = "", EndDate = "", ImageListInfo = "", version = "V2.1";
        public static DataTable Top3InlineDF = new DataTable();
        public static DataTable Top3EndlineDF = new DataTable();
        public static DataTable TLSCD = new DataTable();
        public static int i = 0;
        public SqlConnection con;
        public static DateTime tg;
        public static string ipserver = "", ELIMUrl = "", ILIMUrl = "", videoname = "", loopplay = "";
        public static string timeopen = "", strTimeTitle = "";
        public static int timeplay = 1, check = 0, checkopen = 0;
        public static DataTable videodata;
        public static string url = "", mytest = "A";
        public static int[] TimeArray = { 0730, 0830, 0930, 1030, 1130, 1330, 1430, 1530, 1630, 1730, 1830, 1930, 2030, 2130};
        public static int tmBrStr = 1200, tmBrEnd = 1300;
        public static Timer LoadVideo = new Timer();
        public static int width = 0, height = 0, LangRef = 0;
        public static double TextRatio = 0.9, SizingScrRt = 0, density = 0, SizingScrRtH=0; //TextRatio = 0.35
        public static List<MultiDimensionList> ltLang = new List<MultiDimensionList>();
        public static Connect Cnnt = new Connect(chuoi);

        /// <summary>
        /// 1 : URL cho Inline 2 : URL cho Endline.
        /// </summary>
        public static int in_or_end = 0;//inline or endline
        /// <summary>
        /// Mở kết nối với máy chủ.
        /// </summary>
        public CSDL()
        {
            con = new SqlConnection(chuoi);
        }
        /// <summary>
        /// Đọc dữ liệu từ máy chủ với câu truy vấn.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public DataTable Doc(string c)
        {
            if (con.State == ConnectionState.Closed)
                con.Open();
            SqlDataAdapter doc = new SqlDataAdapter(c, con);
            DataTable dt = new DataTable();
            doc.Fill(dt);
            con.Close();
            return dt;
        }
        /// <summary>
        /// Ghi dữ liệu vào máy chủ với câu truy vấn.
        /// </summary>
        /// <param name="c"></param>
        public void Ghi(string c)
        {
            if (con.State == ConnectionState.Closed)
                con.Open();
            SqlCommand cmd = new SqlCommand(c, con);
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public static string Language(string c)
        {
            try { return CSDL.ltLang[CSDL.ltLang.FindIndex(x => x.FirstText == c)].Ref_Val1; }
            catch { return "error"; }
        }
        public static string Language2nd(string c)
        {
            try { return CSDL.ltLang[CSDL.ltLang.FindIndex(x => x.FirstText == c)].Ref_Val2; }
            catch { return "error"; }
        }


        //public static void ScreenStretching(Context t, ViewGroup viewGroup)
        //{
        //    //var viewGroup = (ViewGroup)FindViewById<RelativeLayout>(Resource.Id.rlMnPlanLoadLayout);
        //    for (int i = 0; i < viewGroup.ChildCount; i++)
        //    {
        //        var childView = viewGroup.GetChildAt(i);

        //        RelativeLayout.LayoutParams rlLayoutTv = (RelativeLayout.LayoutParams)childView.LayoutParameters;
        //        rlLayoutTv.SetMargins((int)(CSDL.SizingScrRt * rlLayoutTv.LeftMargin), (int)(CSDL.SizingScrRtH * rlLayoutTv.TopMargin), (int)(CSDL.SizingScrRt * rlLayoutTv.RightMargin), (int)(CSDL.SizingScrRtH * rlLayoutTv.BottomMargin));
        //        childView.LayoutParameters = rlLayoutTv;

        //        int Owid = rlLayoutTv.Width;

        //        childView.LayoutParameters.Width = (int)(CSDL.SizingScrRt * rlLayoutTv.Width);
        //        childView.LayoutParameters.Height = (int)(CSDL.SizingScrRtH * rlLayoutTv.Height);

        //        int Nwid = childView.LayoutParameters.Width;

        //        Type tp = childView.GetType();

        //        //txt1.TextSize = (ww1 * 20 / 319) / metric.Density;

        //        if (tp.ToString().Contains("EditText"))
        //        {
        //            EditText mojt = childView as EditText;
        //            float texS = mojt.TextSize;
        //            mojt.TextSize = (float)((Nwid * texS / Owid) * CSDL.TextRatio / CSDL.density);
        //            //if(SizingScrRt < 1) mojt.TextSize = (float)(mojt.TextSize * SizingScrRt * CSDL.TextRatio);//* 0.6
        //            //else mojt.TextSize = (float)(mojt.TextSize * SizingScrRt * CSDL.TextRatio); //
        //        }
        //        else if (tp.ToString().Contains("TextView"))
        //        {
        //            TextView mojt = childView as TextView;
        //            float texS = mojt.TextSize;
        //            mojt.TextSize = (float)((Nwid * texS / Owid) * CSDL.TextRatio / CSDL.density);
        //            //mojt.TextSize = (float)(mojt.TextSize * SizingScrRt * CSDL.TextRatio);
        //        }
        //        else if (tp.ToString().Contains("CheckBox"))
        //        {
        //            CheckBox mojt = childView as CheckBox;
        //            float texS = mojt.TextSize;
        //            mojt.TextSize = (float)((Nwid * texS / Owid) * CSDL.TextRatio / CSDL.density);
        //            //mojt.TextSize = (float)(mojt.TextSize * SizingScrRt * CSDL.TextRatio);
        //        }
        //        else if (tp.ToString().Contains("RadioButton"))
        //        {
        //            RadioButton mojt = childView as RadioButton;
        //            float texS = mojt.TextSize;
        //            mojt.TextSize = (float)((Nwid * texS / Owid) * CSDL.TextRatio / CSDL.density);
        //            //mojt.TextSize = (float)(mojt.TextSize * SizingScrRt * CSDL.TextRatio);
        //        }
        //        else if (tp.ToString().Contains("Button"))
        //        {
        //            Button mojt = childView as Button;
        //            float texS = mojt.TextSize;
        //            mojt.TextSize = (float)((Nwid * texS / Owid) * CSDL.TextRatio / CSDL.density);
        //            //mojt.TextSize = (float)(mojt.TextSize * SizingScrRt * CSDL.TextRatio);
        //        }
        //    }
        //}
        public static void ScreenStretching(int ScaleParent, ViewGroup viewGroup)
        {
            if (ScaleParent == 1)
            {
                //lvMessage += Application.Context.Resources.GetResourceName(viewGroup.Id).Split('/')[1] + "    |   " + viewGroup.LayoutParameters.Width.ToString() + "x" + viewGroup.LayoutParameters.Height.ToString();
                ViewGroup.MarginLayoutParams mlPara;
                try
                {
                    mlPara = (ViewGroup.MarginLayoutParams)viewGroup.LayoutParameters;
                    mlPara.SetMargins((int)(CSDL.SizingScrRt * mlPara.LeftMargin), (int)(CSDL.SizingScrRtH * mlPara.TopMargin), (int)(CSDL.SizingScrRt * mlPara.RightMargin), (int)(CSDL.SizingScrRtH * mlPara.BottomMargin));
                }
                catch
                {
                    ScaleParent = 0;
                }
                finally
                {
                    if (ScaleParent == 1 && viewGroup.LayoutParameters.Height > 0 && viewGroup.LayoutParameters.Width > 0)
                    {
                        mlPara = (ViewGroup.MarginLayoutParams)viewGroup.LayoutParameters;
                        mlPara.SetMargins((int)(CSDL.SizingScrRt * mlPara.LeftMargin), (int)(CSDL.SizingScrRtH * mlPara.TopMargin), (int)(CSDL.SizingScrRt * mlPara.RightMargin), (int)(CSDL.SizingScrRtH * mlPara.BottomMargin));
                        viewGroup.LayoutParameters = mlPara;
                        viewGroup.LayoutParameters.Width = (int)(CSDL.SizingScrRt * mlPara.Width);
                        viewGroup.LayoutParameters.Height = (int)(CSDL.SizingScrRtH * mlPara.Height);
                    }
                };
            }
            for (int i = 0; i < viewGroup.ChildCount; i++)
            {
                View childView = viewGroup.GetChildAt(i);
                Type tp = childView.GetType();

                ViewGroup.MarginLayoutParams rlLayoutTv = (ViewGroup.MarginLayoutParams)childView.LayoutParameters;

                rlLayoutTv.SetMargins((int)(CSDL.SizingScrRt * rlLayoutTv.LeftMargin), (int)(CSDL.SizingScrRt * rlLayoutTv.TopMargin), (int)(CSDL.SizingScrRt * rlLayoutTv.RightMargin), (int)(CSDL.SizingScrRt * rlLayoutTv.BottomMargin));
                childView.LayoutParameters = rlLayoutTv;

                int Owid = childView.LayoutParameters.Width;
                int Ohei = childView.LayoutParameters.Height;

                childView.LayoutParameters.Width = (int)(CSDL.SizingScrRt * rlLayoutTv.Width);
                childView.LayoutParameters.Height = (int)(CSDL.SizingScrRtH * rlLayoutTv.Height);

                int Nwid = childView.LayoutParameters.Width;
                int Nhei = childView.LayoutParameters.Height;

                try
                {
                    if (((ViewGroup)childView).ChildCount > 0)
                    {
                        ViewGroup vv = childView as ViewGroup;
                        ScreenStretching(0, vv);
                    }
                }
                catch
                {
                    if (tp.ToString().Contains("EditText"))
                    {
                        EditText mojt = childView as EditText;
                        float texS = mojt.TextSize;
                        mojt.TextSize = (float)((Nwid * texS / Owid) * 0.81 / CSDL.density);
                        //if (SizingScrRt < 1) mojt.TextSize = (float)(mojt.TextSize * SizingScrRt * CSDL.TextRatio * 0.6);
                        //else mojt.TextSize = (float)(mojt.TextSize * SizingScrRt * CSDL.TextRatio);
                    }
                    else if (tp.ToString().Contains("TextView"))
                    {
                        TextView mojt = childView as TextView;
                        float texS = mojt.TextSize;
                        mojt.TextSize = (float)((Nwid * texS / Owid) * CSDL.TextRatio / CSDL.density);
                        //mojt.TextSize = (float)(mojt.TextSize * SizingScrRt * CSDL.TextRatio);
                    }
                    else if (tp.ToString().Contains("CheckBox"))
                    {
                        CheckBox mojt = childView as CheckBox;
                        float texS = mojt.TextSize;
                        mojt.TextSize = (float)((Nwid * texS / Owid) * CSDL.TextRatio / CSDL.density);

                        int ORLm = ((ViewGroup.MarginLayoutParams)mojt.LayoutParameters).LeftMargin;
                        mojt.ScaleX = ((float)Nwid / Owid + 1) / 2;
                        mojt.ScaleY = ((float)Nhei / Ohei + 1) / 2;
                        ((ViewGroup.MarginLayoutParams)mojt.LayoutParameters).LeftMargin = (int)(ORLm - (1 - mojt.ScaleX) * mojt.LayoutParameters.Width / 2);
                    }
                    else if (tp.ToString().Contains("RadioButton"))
                    {
                        RadioButton mojt = childView as RadioButton;
                        float texS = mojt.TextSize;
                        mojt.TextSize = (float)((Nwid * texS / Owid) * CSDL.TextRatio / CSDL.density);

                        int ORLm = ((ViewGroup.MarginLayoutParams)mojt.LayoutParameters).LeftMargin;
                        mojt.ScaleX = ((float)Nwid / Owid + 1) / 2;
                        mojt.ScaleY = ((float)Nhei / Ohei + 1) / 2;
                        ((ViewGroup.MarginLayoutParams)mojt.LayoutParameters).LeftMargin = (int)(ORLm - (1 - mojt.ScaleX) * mojt.LayoutParameters.Width / 2);
                    }
                    else if (tp.ToString().Contains("Button"))
                    {
                        Button mojt = childView as Button;
                        float texS = mojt.TextSize;
                        mojt.TextSize = (float)((Nwid * texS / Owid) * CSDL.TextRatio / CSDL.density);
                    }
                    //continue;
                }
            }
        }
    }
}




public class DownloadCompleteReceiver : BroadcastReceiver
{
    long id;
    EventHandler handler;
    public DownloadCompleteReceiver(long id, EventHandler handler)
    {
        this.id = id;
        this.handler = handler;
    }
    public override void OnReceive(Context context, Intent intent)
    {
        if (intent.Action == DownloadManager.ActionDownloadComplete &&
             id == intent.GetLongExtra(DownloadManager.ExtraDownloadId, 0))
        {
            handler.Invoke(this, EventArgs.Empty);
        }
    }
}