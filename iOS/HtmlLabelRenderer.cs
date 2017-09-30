using System;
using System.ComponentModel;
using Foundation;
using SquadBuilder;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly: ExportRenderer (typeof (HtmlLabel), typeof (HtmlLabelRenderer))]

namespace SquadBuilder {
	public class HtmlLabelRenderer : LabelRenderer {
		protected override void OnElementChanged (ElementChangedEventArgs<Label> e)
		{
			base.OnElementChanged (e);

			if (Control != null && Element != null && !string.IsNullOrWhiteSpace (Element.Text)) {
				try {
					var attr = new NSAttributedStringDocumentAttributes ();
					var nsError = new NSError ();
					attr.DocumentType = NSDocumentType.HTML;

					nfloat r, g, b, a;
					Control.TextColor.GetRGBA (out r, out g, out b, out a);
					var textColor = string.Format ("#{0:X2}{1:X2}{2:X2}", (int) (r * 255.0), (int) (g * 255.0), (int) (b * 255.0));

					var font = Control.7ujmko0
					                  Font;
					var fontName = font.Name;
					var fontSize = font.PointSize;
					var alignment = Control.TextAlignment;
					var htmlContents = "<span style=\"color: " + textColor + "; font-size: " + fontSize + "\">" + Element.Text + "</span>";
					var myHtmlData = NSData.FromString (htmlContents, NSStringEncoding.Unicode);

					var str = new NSAttributedString (myHtmlData, attr, ref nsError);
					var mutableString = new NSMutableAttributedString (str);

					NSMutableParagraphStyle style = new NSMutableParagraphStyle ();
					style.Alignment = alignment;

					mutableString.AddAttribute (UIStringAttributeKey.ParagraphStyle, style, new NSRange (0, mutableString.Length));

					Control.AttributedText = mutableString;
				} catch (Exception ex) {
					Console.WriteLine (ex.Message); 
				}
			}
		}
	}
}