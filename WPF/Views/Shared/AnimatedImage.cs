using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Diagnostics;

namespace EpisodeTracker.WPF.Views.Shared {
	/// <summary> 
	/// Animate GIF. 
	/// </summary> 
	public class AnimatedImage : Image {
		#region Public properties

		public int FrameIndex {
			get { return (int)GetValue(FrameIndexProperty); }
			set { SetValue(FrameIndexProperty, value); }
		}

		public new ImageSource Source {
			get { return (ImageSource)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		#endregion

		#region Protected interface

		/// <summary>
		/// Provides derived classes an opportunity to handle changes to the Source property.
		/// </summary>
		protected virtual void OnSourceChanged(DependencyPropertyChangedEventArgs e) {
			ClearAnimation();

			var source = e.NewValue as BitmapImage;
			if(source == null) {
				Uri uri = e.NewValue as Uri;

				if(uri == null) {
					var bitmapframe = e.NewValue as BitmapFrame;

					if(bitmapframe == null) {
						var imgsrc = e.NewValue as ImageSource;
						base.Source = imgsrc;
						return;
					}

					uri = new Uri(e.NewValue.ToString());
				}

				source = new BitmapImage();
				source.BeginInit();
				source.UriSource = uri;
				source.CacheOption = BitmapCacheOption.OnLoad;
				source.EndInit();
			}

			if(!IsAnimatedGifImage(source)) {
				base.Source = source;
				return;
			}

			PrepareAnimation(source);
		}

		#endregion

		#region Private properties

		private Int32Animation Animation { get; set; }
		private GifBitmapDecoder Decoder { get; set; }
		private bool IsAnimationWorking { get; set; }

		#endregion

		#region Private methods

		private void ClearAnimation() {
			if(Animation != null) {
				BeginAnimation(FrameIndexProperty, null);
			}

			IsAnimationWorking = false;
			Animation = null;
			Decoder = null;
		}

		private void PrepareAnimation(BitmapImage aBitmapImage) {
			Debug.Assert(aBitmapImage != null);

			if(aBitmapImage.UriSource != null) {
				Decoder = new GifBitmapDecoder(
					aBitmapImage.UriSource,
					BitmapCreateOptions.PreservePixelFormat,
					BitmapCacheOption.Default);
			} else {
				aBitmapImage.StreamSource.Position = 0;
				Decoder = new GifBitmapDecoder(
					aBitmapImage.StreamSource,
					BitmapCreateOptions.PreservePixelFormat,
					BitmapCacheOption.Default);
			}

			Animation =
				new Int32Animation(
					0,
					Decoder.Frames.Count - 1,
					new Duration(
						new TimeSpan(
							0,
							0,
							0,
							Decoder.Frames.Count / 10,
							(int)((Decoder.Frames.Count / 10.0 - Decoder.Frames.Count / 10) * 1000)))) {
								RepeatBehavior = RepeatBehavior.Forever
							};

			base.Source = Decoder.Frames[0];
			BeginAnimation(FrameIndexProperty, Animation);
			IsAnimationWorking = true;
		}

		private bool IsAnimatedGifImage(BitmapImage aBitmapImage) {
			Debug.Assert(aBitmapImage != null);

			bool lResult = false;
			if(aBitmapImage.UriSource != null) {
				BitmapDecoder lBitmapDecoder = BitmapDecoder.Create(
					aBitmapImage.UriSource,
					BitmapCreateOptions.PreservePixelFormat,
					BitmapCacheOption.Default);
				lResult = lBitmapDecoder is GifBitmapDecoder;
			} else if(aBitmapImage.StreamSource != null) {
				try {
					long lStreamPosition = aBitmapImage.StreamSource.Position;
					aBitmapImage.StreamSource.Position = 0;
					GifBitmapDecoder lBitmapDecoder =
						new GifBitmapDecoder(
							aBitmapImage.StreamSource,
							BitmapCreateOptions.PreservePixelFormat,
							BitmapCacheOption.Default);
					lResult = lBitmapDecoder.Frames.Count > 1;

					aBitmapImage.StreamSource.Position = lStreamPosition;
				} catch {
					lResult = false;
				}
			}

			return lResult;
		}

		private static void ChangingFrameIndex
			(DependencyObject aObject, DependencyPropertyChangedEventArgs aEventArgs) {
			AnimatedImage lAnimatedImage = aObject as AnimatedImage;

			if(lAnimatedImage == null || !lAnimatedImage.IsAnimationWorking) {
				return;
			}

			int lFrameIndex = (int)aEventArgs.NewValue;
			((Image)lAnimatedImage).Source = lAnimatedImage.Decoder.Frames[lFrameIndex];
			lAnimatedImage.InvalidateVisual();
		}

		/// <summary>
		/// Handles changes to the Source property.
		/// </summary>
		private static void OnSourceChanged
			(DependencyObject aObject, DependencyPropertyChangedEventArgs aEventArgs) {
			((AnimatedImage)aObject).OnSourceChanged(aEventArgs);
		}

		#endregion

		#region Dependency Properties

		/// <summary>
		/// FrameIndex Dependency Property
		/// </summary>
		public static readonly DependencyProperty FrameIndexProperty =
			DependencyProperty.Register(
				"FrameIndex",
				typeof(int),
				typeof(AnimatedImage),
				new UIPropertyMetadata(0, ChangingFrameIndex));

		/// <summary>
		/// Source Dependency Property
		/// </summary>
		public new static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register(
				"Source",
				typeof(ImageSource),
				typeof(AnimatedImage),
				new FrameworkPropertyMetadata(
					null,
					FrameworkPropertyMetadataOptions.AffectsRender |
					FrameworkPropertyMetadataOptions.AffectsMeasure,
					OnSourceChanged));

		#endregion
	}
}
