using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;


using TinyIoC;
using Tesseract;
using Tesseract.iOS;
using XLabs.Ioc;
using XLabs.Ioc.TinyIOC;
using XLabs.Platform.Device;

namespace Xocr.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init ();



			var container = TinyIoCContainer.Current;
			container.Register<IDevice>(AppleDevice.CurrentDevice);
			container.Register<ITesseractApi>((cont, parameters) =>
			{
				return new TesseractApi();
			});
			Resolver.SetResolver(new TinyResolver(container));

			LoadApplication (new App ());

			return base.FinishedLaunching (app, options);
		}
	}
}

