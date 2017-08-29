using System;
using System.ComponentModel;
using Android.Text;
using Android.Widget;
using SquadBuilder;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer (typeof (HtmlLabel), typeof (HtmlLabelRenderer))]

namespace SquadBuilder {
	public class HtmlLabelRenderer : LabelRenderer {

		protected override void OnElementChanged (ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged (e);
			var text = Element.Text.Replace ("xwing-miniatures", "xwing-miniatures.ttf#xwing-miniatures").Replace ("x-wing-ships", "xwing-miniatures-ships.ttf#x-wing-ships") ?? ""; 
			Control?.SetText (Html.FromHtml (text), TextView.BufferType.Spannable);
		}

		protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			if (e.PropertyName == Label.TextProperty.PropertyName) {
				try {
					var text = "<span>" + Element?.Text?.Replace ("xwing-miniatures", "xwing-miniatures.ttf#xwing-miniatures").Replace ("x-wing-ships", "xwing-miniatures-ships#x-wing-ships") ?? "" + "</span>";
					Control?.SetText (Html.FromHtml (text), TextView.BufferType.Spannable);
				} catch (Exception ex) {
					Console.WriteLine (ex.Message);
				}
			}
		}

		//protected override void OnElementChanged (ElementChangedEventArgs<Label> e)
		//{
			//base.OnElementChanged (e);

			//if (Control != null && Element != null && !string.IsNullOrEmpty (Element.Text)) {
			//	this.Control.htm
			//}

			//if (Control != null && Element != null && !string.IsNullOrWhiteSpace (Element.Text)) {
			//	var attr = new NSAttributedStringDocumentAttributes ();
			//	var nsError = new NSError ();
			//	attr.DocumentType = NSDocumentType.HTML;

			//	nfloat r, g, b, a;
			//	Control.TextColor.GetRGBA (out r, out g, out b, out a);
			//	var textColor = string.Format ("#{0:X2}{1:X2}{2:X2}", (int) (r * 255.0), (int) (g * 255.0), (int) (b * 255.0));

			//	var font = Control.Font;
			//	var fontName = font.Name;
			//	var fontSize = font.PointSize;
			//	var alignment = Control.TextAlignment;
			//	var htmlContents = "<span style=\"font-family: '" + fontName + "'; color: " + textColor + "; font-size: " + fontSize + "\">" + Element.Text + "</span>";
			//	var myHtmlData = NSData.FromString (htmlContents, NSStringEncoding.Unicode);

			//	Control.Lines = 0;
			//	var str = new NSAttributedString (myHtmlData, attr, ref nsError);
			//	var mutableString = new NSMutableAttributedString (str);

			//	NSMutableParagraphStyle style = new NSMutableParagraphStyle ();
			//	style.Alignment = alignment;

			//	mutableString.AddAttribute (UIStringAttributeKey.ParagraphStyle, style, new NSRange (0, mutableString.Length));

			//	Control.AttributedText = mutableString;
			//}
		//}

		//protected override void OnElementPropertyChanged (object sender, PropertyChangedEventArgs e)
		//{
		//	//base.OnElementPropertyChanged (sender, e);

		//	//if (e.PropertyName == Label.TextProperty.PropertyName) {
		//	//	if (Control != null && Element != null && !string.IsNullOrWhiteSpace (Element.Text)) {
		//	//		var attr = new NSAttributedStringDocumentAttributes ();
		//	//		var nsError = new NSError ();
		//	//		attr.DocumentType = NSDocumentType.HTML;

		//	//		var myHtmlData = NSData.FromString (Element.Text, NSStringEncoding.Unicode);
		//	//		Control.Lines = 0;
		//	//		Control.AttributedText = new NSAttributedString (myHtmlData, attr, ref nsError);
		//	//	}
		//	//}
		//}
	}
}