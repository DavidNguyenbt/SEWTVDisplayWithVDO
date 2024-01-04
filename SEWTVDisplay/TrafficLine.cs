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

namespace SEWTVDisplay
{
    class TrafficLine : BaseAdapter<DataRow>
    {
        DataTable d = new DataTable();
        A1ATeam.LayoutRequest rq = new A1ATeam.LayoutRequest(1280, 752, 1);
        Connect kn;
        string customer = "ADS";
        public TrafficLine(DataTable _d, Connect _kn, string _customer)
        {
            d = _d; kn = _kn; customer = _customer;
        }

        public override DataRow this[int position]
        {
            get
            {
                return d.Rows[position];
            }
        }

        public override int Count
        {
            get
            {
                return d.Rows.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            LinearLayout ln = new LinearLayout(parent.Context);

            DataRow r = d.Rows[position];

            List<TextView> Temptv = new List<TextView>();

            var id = new TextView(parent.Context)
            { Text = r[0].ToString(), LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(120), ViewGroup.LayoutParams.WrapContent), TextSize = rq.eTextSize(18) }; ln.AddView(id);
            var tf01 = new TextView(parent.Context)
            { Text = r[1].ToString(), LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(50), ViewGroup.LayoutParams.WrapContent), TextSize = rq.eTextSize(18) }; ln.AddView(tf01); Temptv.Add(tf01);
            var tf02 = new TextView(parent.Context)
            { Text = r[2].ToString(), LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(50), ViewGroup.LayoutParams.WrapContent), TextSize = rq.eTextSize(18) }; ln.AddView(tf02); Temptv.Add(tf02);
            var tf03 = new TextView(parent.Context)
            { Text = r[3].ToString(), LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(50), ViewGroup.LayoutParams.WrapContent), TextSize = rq.eTextSize(18) }; ln.AddView(tf03); Temptv.Add(tf03);
            var tf04 = new TextView(parent.Context)
            { Text = r[4].ToString(), LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(50), ViewGroup.LayoutParams.WrapContent), TextSize = rq.eTextSize(18) }; ln.AddView(tf04); Temptv.Add(tf04);
            var tf05 = new TextView(parent.Context)
            { Text = r[5].ToString(), LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(50), ViewGroup.LayoutParams.WrapContent), TextSize = rq.eTextSize(18) }; ln.AddView(tf05); Temptv.Add(tf05);
            var tf06 = new TextView(parent.Context)
            { Text = r[6].ToString(), LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(50), ViewGroup.LayoutParams.WrapContent), TextSize = rq.eTextSize(18) }; ln.AddView(tf06); Temptv.Add(tf06);
            var oper = new TextView(parent.Context)
            { Text = r[8].ToString(), LayoutParameters = new ViewGroup.LayoutParams(rq.eWidht(500), ViewGroup.LayoutParams.WrapContent), TextSize = rq.eTextSize(18) }; ln.AddView(oper);

            if (customer == "ADS")
            {
                int mau = 1;// 0: trang 1: xanh  2: vang  3: do
                for (int i = 0; i < Temptv.Count; i++)
                {
                    Temptv[i].TextAlignment = TextAlignment.Center;
                    if (Temptv[i].Text == "") { Temptv[i].SetBackgroundColor(Android.Graphics.Color.White); mau = 0; }
                    else if (Temptv[i].Text == "0")
                    {
                        if (mau == 3) { Temptv[i].SetBackgroundColor(Android.Graphics.Color.Yellow); mau = 2; }
                        else { Temptv[i].SetBackgroundColor(Android.Graphics.Color.Green); mau = 1; }
                    }
                    else if (Temptv[i].Text == "1")
                    {
                        try
                        {
                            //string cd = kn.Doc("select CD from SubInlineQc where LINEST='" + CSDL.SelectedLine + "' and BARCODE like '" + CSDL.BeginDate + "%' " +
                            //" and RANGETIME = '" + (i + 1).ToString("00") + "' and ORDNUM in (Select ORDNUM from InlineQcLayout where convert(nvarchar,UPD_DATE,112) = '" + CSDL.BeginDate + "' " +
                            //"and LINEST='" + CSDL.SelectedLine + "' and EMPLOYEE = '" + r[0].ToString() + "')").Tables[0].Rows[0][0].ToString();

                            string cd = CSDL.TLSCD.Select().FirstOrDefault(s => s[1].ToString() == (i + 1).ToString("00") && s[5].ToString().Contains(r[0].ToString()))["CD"].ToString();

                            if (cd == "1" || cd == "2") { Temptv[i].SetBackgroundColor(Android.Graphics.Color.Red); mau = 3; }
                            else { Temptv[i].SetBackgroundColor(Android.Graphics.Color.Yellow); mau = 2; }
                        }
                        catch
                        {
                            Temptv[i].SetBackgroundColor(Android.Graphics.Color.Red); mau = 3;
                        }
                    }
                    else { Temptv[i].SetBackgroundColor(Android.Graphics.Color.Red); mau = 3; }
                }
            }
            else
            {
                for (int i = 0; i < Temptv.Count; i++)
                {
                    if (Temptv[i].Text == "") Temptv[i].SetBackgroundColor(Android.Graphics.Color.White);
                    else if (Temptv[i].Text == "0") Temptv[i].SetBackgroundColor(Android.Graphics.Color.Green);
                    else if (Temptv[i].Text == "1") Temptv[i].SetBackgroundColor(Android.Graphics.Color.Yellow);
                    else Temptv[i].SetBackgroundColor(Android.Graphics.Color.Red);
                }
            }

            return ln;
        }
    }
}