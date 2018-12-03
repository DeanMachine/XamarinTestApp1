using Foundation;
using System;
using System.IO;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;

namespace App1
{
    public partial class ViewController : UIViewController
    {

        public ViewController (IntPtr handle) : base (handle)
        {
        }

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad ();

            string translatedNumber = "";

            TranslateButton.TouchUpInside += (object sender, EventArgs e) => {

                // Convert the phone number with text to a number
                // using PhoneTranslator.cs
                translatedNumber = PhoneTranslator.ToNumber(PhoneNumberText.Text);

                // Dismiss the keyboard if text field was tapped
                PhoneNumberText.ResignFirstResponder();

                if (translatedNumber == "")
                {
                    CallButton.SetTitle("Call", UIControlState.Normal);
                    CallButton.Enabled = false;
                }
                else
                {
                    CallButton.SetTitle("Call " + translatedNumber, UIControlState.Normal);
                    CallButton.Enabled = true;
                }
            };

            CallButton.TouchUpInside += (object sender, EventArgs e) => {
                var url = new NSUrl("tel:" + translatedNumber);

                // Use URL handler with tel: prefix to invoke Apple's Phone app,
                // otherwise show an alert dialog

                if (!UIApplication.SharedApplication.OpenUrl(url))
                {
                    var alert = UIAlertController.Create("Not supported", "Scheme 'tel:' is not supported on this device", UIAlertControllerStyle.Alert);
                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));
                    PresentViewController(alert, true, null);
                }
            };

            signatureView.Layer.BorderColor = UIColor.FromRGBA(184, 134, 11, 255).CGColor;
            signatureView.Layer.BorderWidth = 1f;

            signatureView.StrokeCompleted += (sender, e) => UpdateControls();
            signatureView.Cleared += (sender, e) => UpdateControls();

            UpdateControls();
        }

        async partial void SaveSigButton_TouchUpInside(UIButton sender)
        {
            //var points = signatureView.Points;

            //var image = signatureView.GetImage();
            
            //image.SaveToPhotosAlbum((i, e) =>
            //{
            //    var test = string.Empty;
            //});

            using (var image2 = await signatureView.GetImageStreamAsync(Xamarin.Controls.SignatureImageFormat.Png))
            using (var data = NSData.FromStream(image2))
            {
                //image = UIImage.LoadFromData(data);

                //Image img = System.Drawing.Bitmap.FromStream(image2);

                //img.Save(System.IO.Path.GetTempPath() + "\\myImage.Jpeg", ImageFormat.Jpeg);
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                var filename = Path.Combine(documents, "_image.png");

                try
                {

                    data.Save(filename, false);

                    var sigAlertController = UIAlertController.Create("Success", "Signature Saved Successfully", UIAlertControllerStyle.Alert);

                    sigAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

                    PresentViewController(sigAlertController, true, null);

                    //DisplayAlert("Alert", "Signature Saved Successfully", "OK");
                }
                catch (Exception ex)
                {
                    //DisplayAlert("Alert", "Signature error", "OK");

                }

            }
        }

        private void UpdateControls()
        {

        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}