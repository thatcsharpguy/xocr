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

            _takenImage = new Image();

            Content = new StackLayout
            {
                Children =
                {
                    _takePictureButton,
                    _takenImage,
                    _recognizedTextLabel
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
                _tesseractApi.Init("eng");

            var photo = await TakePic();
            if (photo != null)
            {
                _takenImage.Source = ImageSource.FromStream(() => photo.Source);
                var tessResult = await _tesseractApi.SetImage(photo.Source);
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
