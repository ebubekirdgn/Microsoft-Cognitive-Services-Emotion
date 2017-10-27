using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CognitiveServicesFirstExample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private CameraCaptureUI captureUI = new CameraCaptureUI();
        private StorageFile photo;
        private IRandomAccessStream imageStream;

        private const string APIKEY = "f52cc107e53646caba2565a35252141d";
        private EmotionServiceClient emotionServiceClient = new EmotionServiceClient(APIKEY);
        private Emotion[] emotionResult;

        public MainPage()
        {
            this.InitializeComponent();
            captureUI.PhotoSettings.Format = CameraCaptureUIPhotoFormat.Jpeg;
            captureUI.PhotoSettings.CroppedSizeInPixels = new Size(200, 200);
        }

        private async void takePhoto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                photo = await captureUI.CaptureFileAsync(CameraCaptureUIMode.Photo);
                if (photo == null)
                {
                    //Kullanıcı resimi iptal etti
                    return;
                }
                else
                {
                    imageStream = await photo.OpenAsync(FileAccessMode.Read);
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);
                    SoftwareBitmap softwareBitmap = await decoder.GetSoftwareBitmapAsync();

                    SoftwareBitmap softwareBitmapBGR8 = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                    SoftwareBitmapSource bitmapSource = new SoftwareBitmapSource();
                    await bitmapSource.SetBitmapAsync(softwareBitmapBGR8);
                    image.Source = bitmapSource;
                }
            }
            catch
            {
                output.Text = "Hata!Lütfen bir fotoğraf çekin";
            }
        }

        private async void getEmotion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                emotionResult = await emotionServiceClient.RecognizeAsync(imageStream.AsStream());
                var score = emotionResult[0].Scores;
                if (emotionResult != null)
                {
                    output.Text = "Duygularınız :\n" + "Mutluluk : " + score.Happiness + "\n"
                                                  + "Üzgün : " + score.Sadness + "\n"
                                                  + "Şaşırma : " + score.Surprise + "\n"
                                                  + "Korku : " + score.Fear + "\n"
                                                  + "Kızgın : " + score.Sadness + "\n"
                                                  + "Aşağılama : " + score.Contempt + "\n"
                                                  + "İğrenme : " + score.Disgust + "\n"
                                                  + "Tarafsızlık : " + score.Neutral + "\n";
                }
            }
            catch
            {
                output.Text = "Hata!Emotion algılanamadı.";
            }
        }
    }
}