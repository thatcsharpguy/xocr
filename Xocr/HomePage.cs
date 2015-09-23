using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Tesseract;
using Xamarin.Forms;
using XLabs.Ioc;
using XLabs.Platform.Device;
using XLabs.Platform.Services.Media;

namespace Xocr
{
    public class HomePage : ContentPage
    {
        private Button _takePictureButton;
        private Label _recognizedTextLabel;
        private Image _takenImage;

        private readonly ITesseractApi _tesseractApi;
        private readonly IDevice _device;

        public HomePage()
        {
			if (Device.OS == TargetPlatform.iOS)
				Padding = new Thickness (0, 25, 0, 0);

            _tesseractApi = Resolver.Resolve<ITesseractApi>();
            _device = Resolver.Resolve<IDevice>();

            BuildUi();

            WireEvents();
        }


        private void BuildUi()
        {
            _takePictureButton = new Button
            {
                Text = "New picture"
            };

            _recognizedTextLabel = new Label();

			_takenImage = new Image() { HeightRequest = 200 };

			Content = new ScrollView 
			{
				Content = new StackLayout
	            {
	                Children =
	                {
	                    _takePictureButton,
	                    _takenImage,
	                    _recognizedTextLabel
	                }
	            }
			};

        }
        private void WireEvents()
        {
            _takePictureButton.Clicked += TakePictureButton_Clicked;
        }

        async void TakePictureButton_Clicked(object sender, EventArgs e)
        {
            if (!_tesseractApi.Initialized)
                await _tesseractApi.Init("eng");

            var photo = await TakePic();
            if (photo != null)
            {
				// When setting an ImageSource using a stream, 
				// the stream gets closed, so to avoid that I backed up
				// the image into a byte array with the following code:
                var imageBytes = new byte[photo.Source.Length];
                photo.Source.Position = 0;
                photo.Source.Read(imageBytes, 0, (int)photo.Source.Length);
                photo.Source.Position = 0;

                _takenImage.Source = ImageSource.FromStream(() => photo.Source);

                var tessResult = await _tesseractApi.SetImage(imageBytes);

                if (tessResult)
                {
                    _recognizedTextLabel.Text = _tesseractApi.Text;
                }
            }
        }

        private async Task<MediaFile> TakePic()
        {
            var mediaStorageOptions = new CameraMediaStorageOptions
            {
                DefaultCamera = CameraDevice.Rear
            };
            var mediaFile = await _device.MediaPicker.TakePhotoAsync(mediaStorageOptions);

            return mediaFile;
        }
    }
}
