using Android.App;
using CSDL;
using Android.OS;
using Android.Widget;
using Android.Content.PM;

namespace SEWTVDisplay
{
    [Activity(Label = "VideosPlayer", ScreenOrientation = ScreenOrientation.Landscape)]
    public class VideosPlayer : Activity
    {
        VideoView videoView;
        //ConnectToSQL conn = new ConnectToSQL();
        Connect kn = new Connect(CSDL.chuoi);
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.VideosPlayer);
            videoView = FindViewById<VideoView>(Resource.Id.videoView1);
            int i = 0;
            CSDL.checkopen = 1;
            PlayVideo();
            videoView.Completion += delegate
            {
                try
                {
                    if (i == 0)
                    {
                        string st = "SELECT *  FROM InlineQCPlay p, InlineQCVideos v where p.ID_VIDEO = v.ID_VIDEO and p.ID_FACLINE = '" + CSDL.SelectedLine + "' and len(p.STATUS_PLAY)> 0 and len(p.TIMEPLAY) = 6 order by p.TIMEPLAY ASC";
                        CSDL.videodata = CSDL.Cnnt.Doc(st).Tables[0];
                        if (CSDL.videodata.Rows.Count == 0)
                        {
                            string stupdate = "Update InlineQCPlay SET STATUS_PLAY = 'PLAY' where ID_FACLINE = '" + CSDL.SelectedLine + "' and len(TIMEPLAY) = 6 and LOOP_PLAY = '1'";
                            CSDL.Cnnt.Ghi(stupdate);
                        }
                    }
                }
                catch { }
                CSDL.checkopen = 0;
                Finish();
            };
            videoView.Error += delegate
            {
                Toast.MakeText(this, "Error.....", ToastLength.Long).Show();
                CSDL.checkopen = 0;
                Finish();
            };

            void PlayVideo()
            {
                try
                {
                    i = 1;
                    string st = "Update InlineQCPlay SET STATUS_PLAY = '' where ID_FACLINE = '" + CSDL.SelectedLine + "' and TIMEPLAY = '" + CSDL.timeopen + "'";
                    CSDL.Cnnt.Ghi(st);
                    string st2 = "SELECT *  FROM InlineQCPlay p, InlineQCVideos v where p.ID_VIDEO = v.ID_VIDEO and p.ID_FACLINE = '" + CSDL.SelectedLine + "' and len(p.STATUS_PLAY)> 0 and len(p.TIMEPLAY) = 6 order by p.TIMEPLAY ASC";
                    CSDL.videodata = CSDL.Cnnt.Doc(st2).Tables[0];
                    if (CSDL.videodata.Rows.Count == 0)
                    {
                        string stupdate = "Update InlineQCPlay SET STATUS_PLAY = 'PLAY' where ID_FACLINE = '" + CSDL.SelectedLine + "' and len(TIMEPLAY) = 6 and LOOP_PLAY = '1'";
                        CSDL.Cnnt.Ghi(stupdate);
                    }
                    st = CSDL.ipserver + CSDL.videoname;
                    var mediaController = new MediaController(this);
                    videoView.SetVideoURI(Android.Net.Uri.Parse(st));
                    mediaController.SetAnchorView(videoView);
                    videoView.SetMediaController(mediaController);
                    videoView.RequestFocus();
                    videoView.Start();
                    Toast.MakeText(this, "Play.....", ToastLength.Long).Show();
                }
                catch
                {
                    Toast.MakeText(this, "Error......", ToastLength.Long).Show();
                }
            }
        }
    }
}